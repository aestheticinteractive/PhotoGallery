using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Daemon.Fabric {

	/*================================================================================================*/
	public class Service {

		private static ConcurrentDictionary<string, SavedSession> SavedSessByIdMap;
		private static ConcurrentDictionary<int, string> SavedSessIdByThreadMap;

		private readonly ServiceContext vSvcCtx;
		private readonly IFabricClient vDbClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Service(ServiceContext pSvcCtx, long pAppId, string pAppSecret, long pDataProvId) {
			vSvcCtx = pSvcCtx;

			if ( !FabricClient.IsInitialized ) {
				SavedSessByIdMap = new ConcurrentDictionary<string, SavedSession>();
				SavedSessIdByThreadMap = new ConcurrentDictionary<int, string>();

				IFabricSessionContainer dpSess = new FabricSessionContainer();
				const string apiUrl = "http://api.inthefabric.com";

				FabricClient.InitOnce(new FabricClientConfig("main", apiUrl,
					pAppId, pAppSecret, pDataProvId, "NONE", ProvideFabricSession));

				FabricClient.AddConfig(new FabricClientConfig("dataProv", apiUrl,
					pAppId, pAppSecret, pDataProvId, "NONE", (k => dpSess)));
			}

			vDbClient = vSvcCtx.FabClientProv("dataProv");
			vDbClient.UseDataProviderPerson = true;
			StartDataProv();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartDataProv() {
			Log.Debug("StartDataProv");

			var ec = new ExporterContext();
			ec.SessProv = vSvcCtx.SessProv;
			ec.Query = vSvcCtx.Query;

			var t = new Thread(() => {
				try {
					var fe = vSvcCtx.ExportProv(ec, vDbClient, null);
					fe.SendAll(0);
				}
				catch ( Exception e ) {
					Log.Error("StartDataProv Exception: "+e.Message, e);
				}
			});

			t.Start();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartUser(SavedSession pSaved) {
			Log.Debug("StartUser: "+pSaved.SessionId);

			var ec = new ExporterContext();
			ec.SessProv = vSvcCtx.SessProv;
			ec.Query = vSvcCtx.Query;
			ec.SavedSession = pSaved;

			var t = new Thread(() => {
				try {
					RegisterSession(ec.SavedSession);
					IFabricClient fab = vSvcCtx.FabClientProv(null); //gets thread's SavedSession
					FabricUser u;

					using ( ISession s = ec.SessProv.OpenSession() ) {
						u = vSvcCtx.Query.GetCurrentUser(s, fab);
					}

					var fe = vSvcCtx.ExportProv(ec, fab, u);
					fe.SendAll(0);
					CompleteSession(ec.SavedSession);
				}
				catch ( Exception e ) {
					Log.Error("StartUser Exception: "+e.Message, e);
				}
			});

			t.Start();
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer ProvideFabricSession(string pConfigKey) {
			string sessId = SavedSessIdByThreadMap[Thread.CurrentThread.ManagedThreadId];

			var fsc = new FabricSessionContainer();
			fsc.Person = SavedSessByIdMap[sessId];
			return fsc;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void FindSessions() {
			using ( ISession s = vSvcCtx.SessProv.OpenSession() ) {
				IList<FabricPersonSession> list = vSvcCtx.Query.FindUpdatableSessions(s);
				Log.Debug("FindSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					vSvcCtx.Query.TurnOffSessionUpdate(s, fps);
					var saved = new SavedSession(fps);

					if ( !IsSessionActive(saved) ) {
						StartUser(saved);
					}
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void DeleteOldSessions() {
			using ( ISession s = vSvcCtx.SessProv.OpenSession() ) {
				IList<FabricPersonSession> list = vSvcCtx.Query.FindExpiredSessions(s);
				Log.Debug("DeleteOldSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					vSvcCtx.Query.DeleteSession(s, fps);
					var saved = new SavedSession(fps);

					if ( IsSessionActive(saved) ) {
						CompleteSession(saved);
					}
				}
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void RegisterSession(SavedSession pSaved) {
			Log.Info("RegisterSession: "+pSaved.SessionId);
			SavedSessByIdMap.GetOrAdd(pSaved.SessionId, pSaved);
			SavedSessIdByThreadMap.GetOrAdd(Thread.CurrentThread.ManagedThreadId, pSaved.SessionId);
		}

		/*--------------------------------------------------------------------------------------------*/
		private bool IsSessionActive(SavedSession pSaved) {
			bool act = SavedSessByIdMap.ContainsKey(pSaved.SessionId);
			Log.Info("IsSessionActive: "+pSaved.SessionId+" / "+act);
			return act;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void CompleteSession(SavedSession pSaved) {
			Log.Info("CompleteSession: "+pSaved.SessionId);
			string id;

			SavedSessByIdMap.TryRemove(pSaved.SessionId, out pSaved);
			SavedSessIdByThreadMap.TryRemove(Thread.CurrentThread.ManagedThreadId, out id);
			pSaved.ClearSession();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void StopAllThreads() {
			Log.Debug("StopAllThreads");
			Exporter.StopThreads = true;
		}

	}

}
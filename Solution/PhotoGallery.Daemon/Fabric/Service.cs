using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;

namespace PhotoGallery.Daemon.Fabric {

	/*================================================================================================*/
	public class Service {

		private static ConcurrentDictionary<string, SavedSession> SavedSessMap;

		private readonly ISessionProvider vSessProv;
		private readonly Queries vQuery;
		private readonly Func<string, IFabricClient> vFabClientProv;
		private readonly IFabricClient vDbClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Service(ISessionProvider pSessProv, Queries pQuery, 
													Func<string, IFabricClient> pFabClientProv,
													long pAppId, string pAppSecret, long pDataProvId) {
			vSessProv = pSessProv;
			vQuery = pQuery;
			vFabClientProv = pFabClientProv;

			if ( SavedSessMap == null ) {
				SavedSessMap = new ConcurrentDictionary<string, SavedSession>();
				const string apiUrl = "http://api.inthefabric.com";
				IFabricSessionContainer dpSess = new FabricSessionContainer();

				FabricClient.InitOnce(new FabricClientConfig("main", apiUrl,
					pAppId, pAppSecret, pDataProvId, "NONE", ProvideFabricSession));

				FabricClient.AddConfig(new FabricClientConfig("dataProv", apiUrl,
					pAppId, pAppSecret, pDataProvId, "NONE", (k => dpSess)));
			}

			vDbClient = vFabClientProv("dataProv");
			vDbClient.UseDataProviderPerson = true;
			StartDataProv();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartDataProv() {
			Log.Debug("StartDataProv");

			var fd = new ExporterData();
			fd.SessProv = vSessProv;
			fd.Query = vQuery;

			var t = new Thread(() => {
				var fe = new Exporter(fd, vDbClient, null);
				fe.SendAll(0);
			});

			t.Start();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartUser(SavedSession pSaved) {
			Log.Debug("StartUser: "+pSaved.SessionId);

			var fd = new ExporterData();
			fd.SessProv = vSessProv;
			fd.Query = vQuery;
			fd.SavedSession = pSaved;

			var t = new Thread(() => {
				RegisterSession(fd.SavedSession);
				IFabricClient fab = vFabClientProv(null); //obtains this thread's SavedSession
				FabricUser u;

				using ( ISession s = fd.SessProv.OpenSession() ) {
					u = HomeService.GetCurrentUser(fab, s);
				}

				var fe = new Exporter(fd, fab, u);
				fe.SendAll(0);
				CompleteSession(fd.SavedSession);
			});

			t.Start(fd);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer ProvideFabricSession(string pConfigKey) {
			SavedSession saved = (SavedSession)Thread.CurrentContext.GetProperty(SavedSession.PropName);
			return new FabricSessionContainer { Person = saved };
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void FindSessions() {
			using ( ISession s = vSessProv.OpenSession() ) {
				IList<FabricPersonSession> list = vQuery.FindUpdatableSessions(s);
				Log.Debug("FindSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					vQuery.TurnOffSessionUpdate(s, fps);
					var saved = new SavedSession(fps);

					if ( !IsSessionActive(saved) ) {
						StartUser(saved);
					}

				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void DeleteOldSessions() {
			using ( ISession s = vSessProv.OpenSession() ) {
				IList<FabricPersonSession> list = vQuery.FindExpiredSessions(s);
				Log.Debug("DeleteOldSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					vQuery.DeleteSession(s, fps);
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
			SavedSessMap.GetOrAdd(pSaved.SessionId, pSaved);
			Thread.CurrentContext.SetProperty(pSaved);
		}

		/*--------------------------------------------------------------------------------------------*/
		private bool IsSessionActive(SavedSession pSaved) {
			bool act = SavedSessMap.ContainsKey(pSaved.SessionId);
			Log.Info("IsSessionActive: "+act);
			return act;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void CompleteSession(SavedSession pSaved) {
			Log.Info("CompleteSession: "+pSaved.SessionId);
			SavedSessMap.TryRemove(pSaved.SessionId, out pSaved);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void StopAllThreads() {
			Log.Debug("StopAllThreads");
			Exporter.StopThreads = true;
		}

	}

}
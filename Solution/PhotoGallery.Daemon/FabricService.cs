using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class FabricService {

		private static ConcurrentDictionary<string, SavedSession> SavedSessMap;

		private readonly ISessionProvider vSessProv;
		private readonly Queries vQuery;
		private readonly IFabricSessionContainer vDpSess;
		private readonly IFabricClient vDbClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricService(ISessionProvider pSessProv, Queries pQuery,
													long pAppId, string pAppSecret, long pDataProvId) {
			vSessProv = pSessProv;
			vQuery = pQuery;
			vDpSess = new FabricSessionContainer();

			if ( SavedSessMap == null ) {
				SavedSessMap = new ConcurrentDictionary<string, SavedSession>();
			}

			////

			FabricClient.InitOnce(new FabricClientConfig("main", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, "NONE", ProvideFabricSession));

			FabricClient.AddConfig(new FabricClientConfig("dataProv", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, "NONE", (k => vDpSess)));

			vDbClient = new FabricClient("dataProv");
			vDbClient.UseDataProviderPerson = true;
			vDbClient.Config.Logger = new LogFabric { WriteToConsole = true };

			StartDataProv();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartDataProv() {
			Log.Debug("StartDataProv");

			var fd = new FabricExporterData();
			fd.SessProv = vSessProv;
			fd.Query = vQuery;
			fd.Fab = vDbClient;

			var t = new Thread(FabricExporter.StartDataProvThread);
			t.Start(fd);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void StartUser(SavedSession pSaved) {
			Log.Debug("StartUser: "+pSaved.SessionId);

			var fd = new FabricExporterData();
			fd.SessProv = vSessProv;
			fd.Query = vQuery;
			fd.SavedSession = pSaved;
			fd.RegisterSession = RegisterSession;
			fd.CompleteSession = CompleteSession;

			var t = new Thread(FabricExporter.StartUserThread);
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
			FabricExporter.StopThreads = true;
		}

	}

}
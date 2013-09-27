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
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public static class FabricService {

		private static IFabricSessionContainer DpSess;
		private static IFabricClient DbClient;
		private static ConcurrentDictionary<string, SavedSession> SavedSessMap;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Init(string pBaseUrl, long pAppId, string pAppSecret, long pDataProvId) {
			DpSess = new FabricSessionContainer();

			FabricClient.InitOnce(new FabricClientConfig("main", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, "NONE", ProvideSession));

			FabricClient.AddConfig(new FabricClientConfig("dataProv", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, "NONE", (k => DpSess)));

			DbClient = new FabricClient("dataProv");
			DbClient.UseDataProviderPerson = true;
			SetupClientLogger(DbClient);

			var t = new Thread(FabricExporter.StartDataProvThread);
			t.Start(DbClient);

			SavedSessMap = new ConcurrentDictionary<string, SavedSession>();
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer ProvideSession(string pConfigKey) {
			SavedSession saved = (SavedSession)Thread.CurrentContext.GetProperty(SavedSession.PropName);
			return new FabricSessionContainer { Person = saved };
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void FindSessions() {
			using ( ISession s = NewSession() ) {
				IList<FabricPersonSession> list = s.QueryOver<FabricPersonSession>()
					.Where(x => x.TryUpdate)
					.List();

				Log.Debug("FindSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					fps.TryUpdate = false;
					s.Save(fps);

					var saved = new SavedSession(fps);

					if ( IsSessionActive(saved) ) {
						continue;
					}

					var t = new Thread(FabricExporter.StartUserThread);
					t.Start(saved);
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void DeleteOldSessions() {
			using ( ISession s = NewSession() ) {
				IList<FabricPersonSession> list = s.QueryOver<FabricPersonSession>()
					.Where(x => x.Expiration <= DateTime.UtcNow.Ticks)
					.List();

				Log.Debug("DeleteOldSessions: "+list.Count);

				foreach ( FabricPersonSession fps in list ) {
					s.Delete(fps);

					var saved = new SavedSession(fps);

					if ( IsSessionActive(saved) ) {
						CompleteSession(saved);
					}
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RegisterSession(SavedSession pSaved) {
			Log.Info("RegisterSession: "+pSaved.SessionId);
			SavedSessMap.GetOrAdd(pSaved.SessionId, pSaved);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static bool IsSessionActive(SavedSession pSaved) {
			bool act = SavedSessMap.ContainsKey(pSaved.SessionId);
			Log.Info("IsSessionActive: "+act);
			return act;
		}

		/*--------------------------------------------------------------------------------------------*/
		public static bool CompleteSession(SavedSession pSaved) {
			Log.Info("CompleteSession: "+pSaved.SessionId);
			return SavedSessMap.TryRemove(pSaved.SessionId, out pSaved);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			Log.Debug("StopAllThreads");
			FabricExporter.StopThreads = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void SetupClientLogger(IFabricClient pFab) {
			pFab.Config.Logger = new LogFabric { WriteToConsole = true };
		}

		/*--------------------------------------------------------------------------------------------*/
		public static ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}
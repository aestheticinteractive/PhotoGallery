using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public static class FabricService {

		private static IFabricSessionContainer DpSess;
		private static IFabricClient DbClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Init(string pBaseUrl, long pAppId, string pAppSecret, long pDataProvId) {
			DpSess = new FabricSessionContainer();
			string redir = pBaseUrl+"/Oauth/FabricRedirect";

			FabricClient.InitOnce(new FabricClientConfig("main", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, redir, ProvideSession));

			FabricClient.AddConfig(new FabricClientConfig("dataProv", "http://api.inthefabric.com",
				pAppId, pAppSecret, pDataProvId, redir, (k => DpSess)));

			DbClient = new FabricClient("dataProv");
			DbClient.UseDataProviderPerson = true;
			SetupClientLogger(DbClient);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer ProvideSession(string pConfigKey) {
			//Thread.CurrentContext.
			return null;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void StartThreads(IFabricClient pUserFabClient=null) {
			var t = new Thread(FabricExporter.StartDataProvThread);
			t.Start(DbClient);

			if ( pUserFabClient != null ) {
				t = new Thread(FabricExporter.StartUserThread);
				t.Start(pUserFabClient);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			FabricExporter.StopThreads = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void SetupClientLogger(IFabricClient pFab) {
			pFab.Config.Logger = new LogFabric();
		}

		/*--------------------------------------------------------------------------------------------*/
		public static ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}
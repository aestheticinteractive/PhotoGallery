using System;
using System.Configuration;
using System.Threading;
using Fabric.Clients.Cs;
using PhotoGallery.Daemon.Fabric;
using PhotoGallery.Database;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class Program {

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;
		private static long FabricDataProvId;
		private static Service FabSvc;
		private static bool Stopped;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Main(string[] pArgs) {
			Log.ConfigureOnce();
			Log.WriteToConsole = true;

			Log.Info("Starting PhotoGallery Daemon...");
			Console.CancelKeyPress += OnCancelKeyPress;

			Log.Debug("Initializing database connection...");
			BaseService.InitDatabase();

			Log.Debug("Initializing Fabric Client...");
			FabSvc = BuildFabricService();

			while ( true ) {
				if ( !Stopped ) {
					FabSvc.FindSessions();
					FabSvc.DeleteOldSessions();
				}

				Thread.Sleep(10000);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void OnCancelKeyPress(object pSender, ConsoleCancelEventArgs pEventArgs) {
			if ( Stopped ) {
				pEventArgs.Cancel = false;
				Log.Debug("**** Program stopping ****");
				return;
			}

			Log.Debug("**** Program cancelled. Stopping all threads ****");
			pEventArgs.Cancel = true;
			FabSvc.StopAllThreads();
			Stopped = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static Service BuildFabricService() {
#if !DEBUG
			const string prefix = "Prod_";
#else
			const string prefix = "Dev_";
#endif

#if MONO_DEV
			BaseUrl = "http://localhost:3333";
			FabricAppId = 0000;
			FabricAppSecret = "xxxx";
			FabricDataProvId = 0000;
#else
			BaseUrl = ConfigurationManager.AppSettings[prefix+"BaseUrl"];
			FabricAppId = long.Parse(ConfigurationManager.AppSettings["Fabric_AppId"]);
			FabricAppSecret = ConfigurationManager.AppSettings["Fabric_AppSecret"];
			FabricDataProvId = long.Parse(ConfigurationManager.AppSettings["Fabric_DataProvId"]);
#endif

			var sc = new ServiceContext();
			sc.SessProv = new SessionProvider();
			sc.Query = new Queries();
			sc.FabClientProv = FabClientProv;
			sc.ExportProv = (ctx, fc, user) => new Exporter(ctx, fc, user);

			return new Service(sc, FabricAppId, FabricAppSecret, FabricDataProvId);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private static IFabricClient FabClientProv(string pConfigKey) {
			var fab = new FabricClient(pConfigKey);
			fab.Config.Logger = new LogFabric { WriteToConsole = true };
			return fab;
		}

	}

}
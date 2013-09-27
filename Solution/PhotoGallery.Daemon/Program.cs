using System;
using System.Configuration;
using System.Threading;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class Program {

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;
		private static long FabricDataProvId;
		private static bool Stopped;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Main(string[] pArgs) {
			Console.CancelKeyPress += OnCancelKeyPress;
			Log.ConfigureOnce();
			Log.WriteToConsole = true;

			Log.Debug("Initializing database connection...");
			BaseService.InitDatabase();

			Log.Debug("Initializing Fabric Client...");
			SetupFabricClient();


			while ( true ) {
				if ( !Stopped ) {
					FabricService.FindSessions();
					FabricService.DeleteOldSessions();
				}

				Thread.Sleep(5000);
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
			FabricService.StopAllThreads();
			Stopped = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SetupFabricClient() {
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

			FabricService.Init(BaseUrl, FabricAppId, FabricAppSecret, FabricDataProvId);
		}

	}

}
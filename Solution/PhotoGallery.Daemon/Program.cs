using System;
using System.Configuration;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using PhotoGallery.Daemon.Export;
using PhotoGallery.Database;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class Program {

		private static GalleryExport GalExp;
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
			GalExp = BuildGalleryExport();
			GalExp.Start();
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
			GalExp.Stop();
			Stopped = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static GalleryExport BuildGalleryExport() {
#if !DEBUG
			const string prefix = "Prod_";
#else
			const string prefix = "Dev_";
#endif
			var appId = long.Parse(ConfigurationManager.AppSettings["Fabric_AppId"]);
			var appSecret = ConfigurationManager.AppSettings["Fabric_AppSecret"];
			var dataProvId = long.Parse(ConfigurationManager.AppSettings["Fabric_DataProvId"]);

			var query = new Queries(new SessionProvider());
			return new GalleryExport(query, FabClientProv, appId, appSecret, dataProvId);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private static IFabricClient FabClientProv(IFabricPersonSession pPerson){
			var fab = (pPerson == null ? new FabricClient() : new FabricClient(pPerson));
			fab.Config.Logger = new LogFabric { WriteToConsole = true };
			return fab;
		}

	}

}
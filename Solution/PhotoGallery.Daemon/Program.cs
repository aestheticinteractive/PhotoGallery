using System.Configuration;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class Program {

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;
		private static long FabricDataProvId;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Main(string[] pArgs) {
			Log.ConfigureOnce();
			BaseService.InitDatabase();
			SetupFabricClient();
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
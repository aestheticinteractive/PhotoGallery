using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services;
using PhotoGallery.Services.Fabric;
using PhotoGallery.Web.Application.Windsor;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class FabricWebApplication : HttpApplication {

		private static IWindsorContainer WindsorContainer;

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;
		private static long FabricDataProvId;

		private static IFabricSessionContainer FabricDataProvSess;
		private static IFabricClient FabricDataProvClient;

		private Stopwatch vTimer;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected void Application_Start() {
			vTimer = Stopwatch.StartNew();

			Log.ConfigureOnce();
			Log.Debug("### Application_Start");
			BaseService.InitDatabase();
			SetupFabricClient();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();

			BootstrapWindsorContainer();
		}

		/*--------------------------------------------------------------------------------------------*/
		protected void Application_End() {
			Log.Debug("### Application_End: "+vTimer.Elapsed.TotalSeconds+" sec");
			WindsorContainer.Dispose();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SetupFabricClient() {
			if ( BaseUrl != null ) {
				return;
			}

#if !DEBUG
			const string prefix = "Prod_";
#else
			const string prefix = "Dev_";
#endif

#if MONO_DEV
			BaseUrl = "http://localhost:3333";
			FabricAppId = 0000;
			FabricAppSecret = "xxxx";
			FabricUserId = 0000;
#else
			BaseUrl = ConfigurationManager.AppSettings[prefix+"BaseUrl"];
			FabricAppId = long.Parse(ConfigurationManager.AppSettings["Fabric_AppId"]);
			FabricAppSecret = ConfigurationManager.AppSettings["Fabric_AppSecret"];
			FabricDataProvId = long.Parse(ConfigurationManager.AppSettings["Fabric_DataProvId"]);
#endif

			FabricDataProvSess = new FabricSessionContainer();
			string redir = BaseUrl+"/Oauth/FabricRedirect";

			var config = new FabricClientConfig("main", "http://api.inthefabric.com",
				FabricAppId, FabricAppSecret, FabricDataProvId, redir, FabricSessProv);

			var dataProvConfig = new FabricClientConfig("dataProv", "http://api.inthefabric.com",
				FabricAppId, FabricAppSecret, FabricDataProvId, redir, (k => FabricDataProvSess));

			FabricClient.InitOnce(config);
			FabricClient.AddConfig(dataProvConfig);

			FabricDataProvClient = new FabricClient("dataProv");
			FabricService.SetDataProvClient(FabricDataProvClient);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer FabricSessProv(string pConfigKey) {
			return FabricSessionContainer.FromHttpContext(HttpContext.Current, pConfigKey);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void BootstrapWindsorContainer() {
			WindsorContainer = new WindsorContainer().Install(FromAssembly.This());
			WindsorControllerFactory cf = new WindsorControllerFactory(WindsorContainer.Kernel);
			ControllerBuilder.Current.SetControllerFactory(cf);
		}

	}

}
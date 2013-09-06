using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Session;
using PhotoGallery.Infrastructure;
using PhotoGallery.Web.Application.Windsor;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class FabricWebApplication : HttpApplication {

		private static IWindsorContainer WindsorContainer;

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;
		private static long FabricDataProvId;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected void Application_Start() {
			SetupFabricClient();
			Log.ConfigureOnce();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();

			BootstrapWindsorContainer();
		}

		/*--------------------------------------------------------------------------------------------*/
		protected void Application_End() {
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

			string redir = BaseUrl+"/Home/OauthRedir";

			var config = new FabricClientConfig("main", "http://api.inthefabric.com",
				FabricAppId, FabricAppSecret, FabricDataProvId, redir, FabricSessProv);

			FabricClient.InitOnce(config);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer FabricSessProv(string pConfigKey) {
			string key = "Gallery_"+pConfigKey;
			HttpSessionState sess = HttpContext.Current.Session;

			if ( sess == null ) {
				return null;
			}

			IFabricSessionContainer contain = (sess[key] as IFabricSessionContainer);

			if ( contain == null ) {
				sess[key] = (contain = new FabricSessionContainer());
			}

			return contain;
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
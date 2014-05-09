using System;
using System.Configuration;
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
using PhotoGallery.Web.Application.Windsor;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class FabricWebApplication : HttpApplication {

		public const string DataProvConfigKey = "dataProv";

		private static IWindsorContainer WindsorContainer;

		private static string BaseUrl;
		private static long FabricAppId;
		private static string FabricAppSecret;

		private static IFabricSessionContainer FabricDataProvSess;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected void Application_Start() {
			Log.ConfigureOnce();
			Log.Debug("==== Application_Start ====");
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
			Log.Debug("==== Application_End ====");
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
#endif

			FabricDataProvSess = new FabricSessionContainer();

			var config = new FabricClientConfig("main", "http://api.inthefabric.com",
				FabricAppId, FabricAppSecret, RedirProv, FabricSessProv);

			var dataProvConfig = new FabricClientConfig(DataProvConfigKey, "http://api.inthefabric.com",
				FabricAppId, FabricAppSecret, RedirProv, (k => FabricDataProvSess));

			FabricClient.InitOnce(config);
			FabricClient.AddConfig(dataProvConfig);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static string RedirProv(string pConfigKey) {
			return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)+
				"/Oauth/FabricRedirect";
		}

		/*--------------------------------------------------------------------------------------------*/
		private static IFabricSessionContainer FabricSessProv(string pConfigKey) {
			string key = "Fabric_"+pConfigKey;

			if ( HttpContext.Current != null ) {
				var sess = HttpContext.Current.Session;
				Log.Debug("SESSION: "+sess[key]);
				IFabricSessionContainer sc = (sess[key] as IFabricSessionContainer);
				Log.Debug("SESSION SC: "+(sc == null ? "no contain" : 
					(sc.Person == null ? "no person" : sc.Person.SessionId)));
			}
			else {
				Log.Debug("NO SESSION!");
			}

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
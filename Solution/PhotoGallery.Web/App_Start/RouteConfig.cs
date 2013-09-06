using System.Web.Mvc;
using System.Web.Routing;
using PhotoGallery.Web.Areas.Main;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class RouteConfig {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void RegisterRoutes(RouteCollection pRoutes) {
			pRoutes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			/*pRoutes.MapRoute(
				name: "Default",
				url: "Default/{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "PhotoGallery.Web.Controllers" }
			);*/

			RegisterArea(new MainAreaRegistration()); //Must be last
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void RegisterArea(AreaRegistration pArea) {
			var appCtx = new AreaRegistrationContext(pArea.AreaName, RouteTable.Routes);
			pArea.RegisterArea(appCtx);
		}

	}

}
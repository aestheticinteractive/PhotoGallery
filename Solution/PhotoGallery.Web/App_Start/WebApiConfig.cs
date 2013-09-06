using System.Web.Http;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public static class WebApiConfig {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void Register(HttpConfiguration pConfig) {
			pConfig.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

	}

}
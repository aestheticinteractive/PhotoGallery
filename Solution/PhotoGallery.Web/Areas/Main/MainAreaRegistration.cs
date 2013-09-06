using System.Web.Mvc;

namespace PhotoGallery.Web.Areas.Main {

	/*================================================================================================*/
	public class MainAreaRegistration : AreaRegistration {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override string AreaName {
			get { return "Main"; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void RegisterArea(AreaRegistrationContext pContext) {
			pContext.MapRoute(
				name: "Main_Explore_Traversal",
				url: "Explore/Traverse/{*path}",
				defaults: new { controller = "Explore", action = "Traverse" },
				namespaces: new[] { "PhotoGallery.Web.Areas.Main.Controllers" }
			);

			pContext.MapRoute(
				name: "Main_default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "PhotoGallery.Web.Areas.Main.Controllers" }
			);
		}

	}

}
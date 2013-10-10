using System.Web.Mvc;

namespace PhotoGallery.Web.Areas.Admin {

	/*================================================================================================*/
	public class AdminAreaRegistration : AreaRegistration {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override string AreaName {
			get { return "Admin"; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void RegisterArea(AreaRegistrationContext pContext) {
			pContext.MapRoute(
				name: "Admin_default",
				url: "Admin/{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "PhotoGallery.Web.Areas.Admin.Controllers" }
			);
		}

	}

}
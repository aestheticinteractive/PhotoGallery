using System.Web.Mvc;

namespace PhotoGallery.Web.Areas.Account {

	/*================================================================================================*/
	public class AccountAreaRegistration : AreaRegistration {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override string AreaName {
			get { return "Account"; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void RegisterArea(AreaRegistrationContext pContext) {
			pContext.MapRoute(
				name: "Account_default",
				url: "Account/{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "PhotoGallery.Web.Areas.Account.Controllers" }
			);
		}

	}

}
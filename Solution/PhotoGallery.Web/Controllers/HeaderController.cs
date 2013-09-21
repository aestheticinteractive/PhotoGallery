using System.Web.Mvc;
using PhotoGallery.Services.Account;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Controllers {

	/*================================================================================================*/
	public partial class HeaderController : Controller {

		private readonly HomeService vAcctHome;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HeaderController(HomeService pAcctHome) {
			vAcctHome = pAcctHome;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Data() {
			var m = new HeaderModel();
			m.IsPersonAuthenticated = vAcctHome.IsPersonAuthenticated();
			m.GetPersonLoginOpenScript = vAcctHome.GetPersonLoginOpenScript;
			m.User = vAcctHome.GetActiveFabUser();
			return PartialView(MVC.Header.Views._Header, m);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual RedirectToRouteResult PersonLogout() {
			vAcctHome.Logout();
			return RedirectToAction(MVC.Main.Home.Index());
		}

	}

}
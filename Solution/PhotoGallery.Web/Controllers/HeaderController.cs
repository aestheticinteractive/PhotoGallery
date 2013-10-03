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
		public virtual ActionResult Full() {
			return PartialView(MVC.Header.Views._Header, BuildModel());
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Login() {
			return PartialView(MVC.Header.Views._Login, BuildModel());
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual RedirectToRouteResult PersonLogout() {
			vAcctHome.Logout();
			return RedirectToAction(MVC.Main.Home.Index());
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private HeaderModel BuildModel() {
			var m = new HeaderModel();
			m.IsPersonAuthenticated = vAcctHome.IsPersonAuthenticated();
			m.GetPersonLoginOpenScript = vAcctHome.GetPersonLoginOpenScript;
			m.User = vAcctHome.GetActiveFabUser();
			return m;
		}

	}

}
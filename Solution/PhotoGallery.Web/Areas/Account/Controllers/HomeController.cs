using System.Web.Mvc;
using PhotoGallery.Services.Account;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Account.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Account.Controllers {

	/*================================================================================================*/
	public partial class HomeController : BaseController {

		private readonly HomeService vHome;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeController(HomeService pHome) {
			vHome = pHome;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		public virtual ActionResult Index() {
			var m = new HomeModel();
			m.User = vHome.GetWebUser();
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Login() {
			var m = new HomeModel();
			return View(m);
		}

	}

}
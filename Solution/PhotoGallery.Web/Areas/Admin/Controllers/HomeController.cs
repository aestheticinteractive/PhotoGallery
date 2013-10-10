using System.Web.Mvc;
using PhotoGallery.Services.Admin;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Admin.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Admin.Controllers {

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
		[AdminAuthorize]
		public virtual ActionResult Index() {
			var m = new HomeModel();
			return View(m);
		}

	}

}
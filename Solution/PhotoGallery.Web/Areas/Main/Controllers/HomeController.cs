using System.Web.Mvc;
using PhotoGallery.Logic.Main;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class HomeController : BaseController {

		private readonly HomeLogic vHome;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeController(HomeLogic pHome) {
			vHome = pHome;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Index() {
			var m = new HomeModel();
			m.AlbumNames = vHome.GetAlbumNames();
			return View(m);
		}

	}

}
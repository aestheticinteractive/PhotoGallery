using System.Web.Mvc;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class HomeController : BaseController {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Index() {
			return View(CreateModel<HomeModel>());
		}

	}

}
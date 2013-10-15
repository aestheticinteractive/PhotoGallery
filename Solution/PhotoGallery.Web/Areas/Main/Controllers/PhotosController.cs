using System.Web.Mvc;
using Fabric.Clients.Cs;
using PhotoGallery.Services.Main;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class PhotosController : BaseController {

		private readonly SearchService vSearch;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotosController() {
			//TODO: inject this using a custom IFabricClientDataProv interface
			vSearch = new SearchService(new FabricClient(FabricWebApplication.DataProvConfigKey));
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[HttpPost]
		public virtual JsonResult Tags(string id, bool? first) {
			return Json(vSearch.FindTags(id, (first ?? false)));
		}

	}

}
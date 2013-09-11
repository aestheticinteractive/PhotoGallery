using System.Web.Mvc;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Account.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Account.Controllers {

	/*================================================================================================*/
	public partial class AlbumsController : BaseController {

		private readonly AlbumsService vAlbums;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumsController(AlbumsService pAlbums) {
			vAlbums = pAlbums;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		public virtual ActionResult Index() {
			var m = new AlbumsModel();
			return View(m);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		public virtual ActionResult Create() {
			var m = new AlbumCreateModel();
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual PartialViewResult UpdateAlbumTitle(AlbumCreateTitleModel pModel) {
			Log.Debug("TITLE: "+pModel.Title);
			Response.Write("99");
			return null;
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual PartialViewResult UploadImage(AlbumCreateImageModel pModel) {
			Log.Debug("IMAGE: "+pModel.AlbumId+" // "+pModel.Image);
			Response.Write("1");
			return null;
		}

	}

}
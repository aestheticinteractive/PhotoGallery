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

		//http://hacks.mozilla.org/2011/01/how-to-develop-a-html5-image-uploader/
		//http://stackoverflow.com/questions/16122949/submit-mvc-form-with-jquery-ajax
		//http://www.codeforest.net/html5-image-upload-resize-and-crop
		//http://msdn.microsoft.com/en-us/library/system.web.mvc.ajax.ajaxoptions%28v=vs.108%29.aspx

		/*
		 * general idea:
		 * - use ajax to create/edit the album name
		 * - obtain the album ID from that ajax call
		 * - for each image (on the client side): resize, upload, attach to album, update progress
		 */


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
			Response.Write("1");
			return null;
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual PartialViewResult UploadImage(AlbumCreateImageModel pModel) {
			Log.Debug("IMAGE: "+pModel.Image);
			Response.Write("1");
			return null;
		}

	}

}
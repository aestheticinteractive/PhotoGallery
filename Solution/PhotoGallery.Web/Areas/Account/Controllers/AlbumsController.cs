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
		public virtual ActionResult Create(AlbumCreateModel pModel) {
			if ( !ModelState.IsValid ) {
				return View(pModel);
			}

			int? albumId = null; //vAlbums.AddAlbum(pModel.Title);

			if ( albumId == null ) {
				ModelState.AddModelError("", "A new album was not created.");
				return View(pModel);
			}

			return RedirectToAction(MVC.Account.Albums.Index());
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		public virtual ActionResult Edit(int id) {
			var m = new AlbumCreateModel();
			m.EditAlbumId = id;
			return View(MVC.Account.Albums.Views.Create, m);
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual ActionResult Edit(AlbumCreateModel pModel) {
			if ( !ModelState.IsValid ) {
				return View(MVC.Account.Albums.Views.Create, pModel);
			}

			int? albumId = null; //vAlbums.EditAlbum(pModel.Title);

			if ( albumId == null ) {
				ModelState.AddModelError("", "A new album was not created.");
				return View(MVC.Account.Albums.Views.Create, pModel);
			}

			return RedirectToAction(MVC.Account.Albums.Index());
		}

	}

}
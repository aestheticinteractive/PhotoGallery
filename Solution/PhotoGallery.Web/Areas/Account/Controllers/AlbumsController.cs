using System.Net;
using System.Web.Mvc;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Fabric;
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

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		public virtual ActionResult StopFabricBg() {
			FabricService.StopAllThreads();
			Response.Write("Stopping Fabric background tasks...");
			Log.Debug("Stopping Fabric background tasks...");
			return null;
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
		public virtual ActionResult Edit(int id) {
			var m = new AlbumCreateModel();
			m.EditAlbumId = id;
			m.Title = vAlbums.GetAlbumTitle(id);

			if ( m.Title == null ) {
				return RedirectToAction(MVC.Account.Albums.Index());
			}

			return View(MVC.Account.Albums.Views.Create, m);
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual ActionResult CreateAlbum(AlbumCreateTitleModel pModel) {
			int? albumId = vAlbums.AddAlbum(pModel.Title);

			if ( albumId == null ) {
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
			}

			Response.Write(albumId);
			return null;
		}

		/*--------------------------------------------------------------------------------------------*/
		[FabricAuthorize]
		[HttpPost]
		public virtual ActionResult UploadImage(AlbumCreateImageModel pModel) {
			WebUploadResult res = vAlbums.AddAlbumPhoto(Server, pModel.AlbumId,
				pModel.Filename, pModel.ExifData, pModel.ImageData, pModel.LastImage);
			
			if ( res.Status == WebUploadResult.UploadStatus.Success ) {
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
			}

			Response.Write(res.PhotoId);
			return null;
		}

	}

}
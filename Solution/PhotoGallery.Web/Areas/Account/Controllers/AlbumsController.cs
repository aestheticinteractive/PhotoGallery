using System.Net;
using System.Web.Mvc;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;
using PhotoGallery.Services.Account.Dto;
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
		public virtual ActionResult UpdateAlbumTitle(AlbumCreateTitleModel pModel) {
			int? albumId = 99; //vAlbums.AddAlbum(pModel.Title);
			Log.Debug("UpdateAlbumTitle: "+pModel.Title+" / "+albumId);

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
			Log.Debug("UploadImage A: "+pModel.AlbumId+" // "+pModel.Filename);
			WebUploadResult res = vAlbums.AddAlbumPhoto(
				pModel.AlbumId, pModel.Filename, pModel.ExifData, pModel.ImageData);
			Log.Debug("UploadImage B: "+pModel.AlbumId+" // "+res.Status);

			if ( res.Status == WebUploadResult.UploadStatus.Success ) {
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
			}

			Response.Write(res.PhotoId);
			return null;
		}

	}

}
using System.Web.Mvc;
using PhotoGallery.Services.Main;
using PhotoGallery.Services.Main.Dto;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class AlbumsController : BaseController {

		private readonly HomeService vHome;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumsController(HomeService pHome) {
			vHome = pHome;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Index() {
			var m = new AlbumsModel();
			m.Albums = vHome.GetAlbums(0, 12);
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult All(int? i) {
			var m = new AllAlbumsModel();
			m.StartItemIndex = (i ?? 0);
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual PartialViewResult AllPage(int i) {
			var p = new DataPage<WebAlbum>(i, 10, vHome.GetAlbums(i, 10), vHome.GetAlbumCount(),
				x => MVC.Main.Albums.AllPage(x), MVC.Main.Albums.Views._AllPage);
			return PartialView(MVC.Shared.Views._DataPage, p);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Photos(int id) {
			var m = new AlbumModel();
			m.Album = vHome.GetAlbum(id);
			m.Stats = vHome.GetAlbumStats(id);

			if ( m.Album != null ) {
				var gs = new GallerySession(Session);
				gs.PhotoSet = vHome.GetAlbumPhotoSet(m.Album);
				m.Photos = gs.PhotoSet.GetAll();
			}

			return View(m);
		}

	}

}
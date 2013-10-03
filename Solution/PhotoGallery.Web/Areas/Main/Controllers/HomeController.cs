using System.Web.Mvc;
using PhotoGallery.Services.Main;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

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
		public virtual ActionResult Index() {
			var m = new HomeModel();
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Albums(int? id) {
			if ( id == null ) {
				var hm = new AlbumsModel();
				hm.Albums = vHome.GetAlbums(10);
				return View(hm);
			}

			var m = new AlbumModel();
			m.Album = vHome.GetAlbum((int)id);

			if ( m.Album != null ) {
				var gs = new GallerySession(Session);
				gs.PhotoSet = vHome.GetAlbumPhotoSet(m.Album);
				m.Photos = gs.PhotoSet.GetAll();
			}

			return View(MVC.Main.Home.Views.Album, m);
		}

	}

}
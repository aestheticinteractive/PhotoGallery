using System.Web.Mvc;
using PhotoGallery.Services.Main;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

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
			m.Albums = vHome.GetAlbums(24);
			return View(m);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Photos(int id) {
			var m = new AlbumModel();
			m.Album = vHome.GetAlbum(id);

			if ( m.Album != null ) {
				var gs = new GallerySession(Session);
				gs.PhotoSet = vHome.GetAlbumPhotoSet(m.Album);
				m.Photos = gs.PhotoSet.GetAll();
			}

			return View(m);
		}

	}

}
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebAlbumCore {

		public int AlbumId { get; internal set; }
		public string Title { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbumCore() {}
		
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbumCore(Album pAlbum) {
			AlbumId = pAlbum.Id;
			Title = pAlbum.Title;
		}

	}

}
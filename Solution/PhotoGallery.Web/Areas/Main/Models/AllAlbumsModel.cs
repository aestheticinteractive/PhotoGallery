using PhotoGallery.Services.Main.Dto;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class AllAlbumsModel : MainBaseModel {

		public int StartItemIndex { get; set; }
		public DataPage<WebAlbum> AlbumPage { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "All Albums"; } }

	}

}
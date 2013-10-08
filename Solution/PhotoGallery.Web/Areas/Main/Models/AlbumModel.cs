using System.Collections.Generic;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class AlbumModel : MainBaseModel {

		public WebAlbum Album { get; set; }
		public IList<IWebPhoto> Photos { get; set; }
		public WebAlbumStats Stats { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle {
			get {
				return (Album == null ? "Album" : Album.Title);
			}
		}

	}

}
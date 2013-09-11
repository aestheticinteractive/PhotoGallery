using System.Collections.Generic;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Account.Models {

	/*================================================================================================*/
	public class AlbumsModel : AccountBaseModel {

		public IList<WebAlbum> Albums { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Albums"; } }

	}

}
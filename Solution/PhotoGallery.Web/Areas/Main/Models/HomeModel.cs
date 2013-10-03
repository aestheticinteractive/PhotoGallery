using System.Collections.Generic;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class HomeModel : MainBaseModel {

		public IList<WebAlbum> Albums { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Home"; } }

	}

}
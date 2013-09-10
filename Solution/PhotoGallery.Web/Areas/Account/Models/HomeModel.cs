using System.Collections.Generic;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Account.Models {

	/*================================================================================================*/
	public class HomeModel : AccountBaseModel {

		public WebUser User { get; set; }
		public IList<WebAlbum> Albums { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Home"; } }

	}

}
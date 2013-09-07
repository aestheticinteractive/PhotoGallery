using System.Collections.Generic;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class HomeModel : MainBaseModel {

		public IList<string> AlbumNames { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Home"; } }

	}

}
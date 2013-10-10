using System.Collections.Generic;
using PhotoGallery.Services.Admin.Dto;

namespace PhotoGallery.Web.Areas.Admin.Models {

	/*================================================================================================*/
	public class PeopleModel : AdminBaseModel {

		public IList<WebPersonTag> PersonTags { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "People"; } }

	}

}
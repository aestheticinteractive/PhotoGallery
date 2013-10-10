using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PhotoGallery.Services.Admin.Dto;

namespace PhotoGallery.Web.Areas.Admin.Models {

	/*================================================================================================*/
	public class PeopleModel : AdminBaseModel {

		public IList<WebPersonTag> PersonTags { get; set; }

		[Required]
		[Display(Name = "Name")]
		[StringLength(128, ErrorMessage="Name cannot exceed 128 characters.")]
		public string AddName { get; set; }

		[Display(Name = "Is Male?")]
		public bool AddIsMale { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "People"; } }

	}

}
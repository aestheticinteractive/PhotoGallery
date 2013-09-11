using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PhotoGallery.Web.Areas.Account.Models {

	/*================================================================================================*/
	public class AlbumCreateModel : AccountBaseModel {

		[Required]
		[StringLength(64, ErrorMessage="Title cannot exceed 64 characters.")]
		public string Title { get; set; }

		[Display(Name="Image Files")]
		public IEnumerable<HttpPostedFileBase> Files { get; set; }

		public int? EditAlbumId { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Create New Album"; } }

	}


	/*================================================================================================*/
	public class AlbumCreateTitleModel {

		public string Title { get; set; }

	}


	/*================================================================================================*/
	public class AlbumCreateImageModel {

		public int AlbumId { get; set; }
		public string ImageData { get; set; }

	}

}
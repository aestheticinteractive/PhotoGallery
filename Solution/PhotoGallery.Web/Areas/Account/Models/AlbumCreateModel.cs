using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Account.Models {

	/*================================================================================================*/
	public class AlbumCreateModel : AccountBaseModel {

		[Required]
		[StringLength(64, ErrorMessage="Title cannot exceed 64 characters.")]
		public string Title { get; set; }

		[Display(Name="Image Files")]
		public IEnumerable<HttpPostedFileBase> Files { get; set; }

		public int? EditAlbumId { get; set; }
		public IList<WebUploadResult> UploadResults { get; set; }


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

		public HttpPostedFileBase Image { get; set; }

	}

}
using System.Web.Mvc;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public interface IMvcLink {

		string Text { get; set; }
		ActionResult Action { get; set; }

	}

}
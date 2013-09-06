using System.Web.Mvc;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public class MvcLink : IMvcLink {

		public string Text { get; set; }
		public ActionResult Action { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public MvcLink(string pText, ActionResult pAction) {
			Text = pText;
			Action = pAction;
		}

	}

}
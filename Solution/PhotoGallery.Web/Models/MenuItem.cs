using System.Collections.Generic;
using System.Web.Mvc;
using PhotoGallery.Web.Application.Helpers;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public class MenuItem : IMenuItem {

		public IMvcLink Link { get; private set; }
		public List<IMenuItem> SubItems { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public MenuItem(IMvcLink pLink) {
			Link = pLink;
			SubItems = new List<IMenuItem>();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public MenuItem(string pText, ActionResult pAction) {
			Link = new MvcLink(pText, pAction);
			SubItems = new List<IMenuItem>();
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string GetLinkUrl(HtmlHelper pHelp) {
			return pHelp.ActionLinkUrl(Link);
		}

	}

}
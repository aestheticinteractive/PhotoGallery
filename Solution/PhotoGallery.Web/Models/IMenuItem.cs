using System.Collections.Generic;
using System.Web.Mvc;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public interface IMenuItem {

		IMvcLink Link { get; }
		List<IMenuItem> SubItems { get; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		string GetLinkUrl(HtmlHelper pHel);

	}

}
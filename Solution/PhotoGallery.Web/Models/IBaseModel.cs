using System.Collections.Generic;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public interface IBaseModel {

		string HtmlTitle { get; }
		string AreaTitle { get; }
		IMvcLink AreaLink { get; }
		List<IMvcLink> AreaLinks { get; }

	}

}
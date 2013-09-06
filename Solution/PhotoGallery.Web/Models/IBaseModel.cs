using System.Collections.Generic;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public interface IBaseModel {

		string AreaTitle { get; }
		List<IMvcLink> AreaLinks { get; }

		string HtmlTitle { get; }

		IMenuItem MenuRoot { get; }
		string MenuTitle { get; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		void AddSubItem(MenuItem pItem);

	}

}
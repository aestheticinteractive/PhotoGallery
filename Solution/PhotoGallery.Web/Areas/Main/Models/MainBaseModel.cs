using System.Collections.Generic;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class MainBaseModel : BaseModel {

		public static IMvcLink HomeLink = new MvcLink("Home", MVC.Main.Home.Index());

		public static List<IMvcLink> Links = new List<IMvcLink>(
			new [] { HomeLink });

		public override string HtmlTitle { get { return "Kinstner Photo Gallery :: "+PageTitle; } }
		public override string AreaTitle { get { return ""; } }
		public override IMvcLink AreaLink { get { return null; } }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override List<IMvcLink> AreaLinks {
			get { return Links; }
		}

	}


	/*================================================================================================*/
	public class MainBaseModel<T> : MainBaseModel {

		public T Object { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public MainBaseModel() {}

		/*--------------------------------------------------------------------------------------------*/
		public MainBaseModel(T pObject) {
			Object = pObject;
		}

	}

}
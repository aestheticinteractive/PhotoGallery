using System.Collections.Generic;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Areas.Account.Models {

	/*================================================================================================*/
	public class AccountBaseModel : BaseModel {

		public static IMvcLink HomeLink = new MvcLink("Home", MVC.Account.Home.Index());

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
	public class AccountBaseModel<T> : AccountBaseModel {

		public T Object { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AccountBaseModel() {}

		/*--------------------------------------------------------------------------------------------*/
		public AccountBaseModel(T pObject) {
			Object = pObject;
		}

	}

}
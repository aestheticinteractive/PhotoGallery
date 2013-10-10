using System.Collections.Generic;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Areas.Admin.Models {

	/*================================================================================================*/
	public class AdminBaseModel : BaseModel {

		public static IMvcLink HomeLink = new MvcLink("Home", MVC.Admin.Home.Index());
		public static IMvcLink PeopleLink = new MvcLink("People", MVC.Admin.People.Index());

		public static List<IMvcLink> Links = new List<IMvcLink>(
			new [] { HomeLink, PeopleLink });

		public override string HtmlTitle { get { return "Kinstner Photo Gallery :: "+PageTitle; } }
		public override string AreaTitle { get { return ""; } }
		public override IMvcLink AreaLink { get { return null; } }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override List<IMvcLink> AreaLinks {
			get { return Links; }
		}

	}

}
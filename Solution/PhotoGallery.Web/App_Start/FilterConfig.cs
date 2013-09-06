using System.Web.Mvc;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class FilterConfig {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void RegisterGlobalFilters(GlobalFilterCollection pFilters) {
			pFilters.Add(new HandleErrorAttribute());
			//pFilters.Add(new AuthorizeAttribute());
		}

	}

}
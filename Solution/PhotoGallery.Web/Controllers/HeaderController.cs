using System.Web.Mvc;
using Fabric.Clients.Cs;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Controllers {

	/*================================================================================================*/
	public partial class HeaderController : Controller {

		private readonly IFabricClient vFabric;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HeaderController(IFabricClient pFabric) {
			vFabric = pFabric;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult Data() {
			return PartialView(MVC.Header.Views._Header, new HeaderModel(vFabric));
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual RedirectToRouteResult PersonLogout() {
			vFabric.PersonSession.Logout();
			return RedirectToAction(MVC.Main.Home.Index());
		}

	}

}
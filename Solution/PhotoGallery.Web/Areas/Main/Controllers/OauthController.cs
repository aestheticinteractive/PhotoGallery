using System.Web.Mvc;
using PhotoGallery.Services.Main;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class OauthController : BaseController {

		private readonly OauthService vOauth;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public OauthController(OauthService pOauth) {
			vOauth = pOauth;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult FabricRedirect() {
			var m = new OauthModel();
			m.LoginSuccess = vOauth.PersonOauthSuccess(Request, Response);
			return View(m);
		}

	}

}
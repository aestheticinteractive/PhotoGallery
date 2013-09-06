using System.Web.Mvc;
using PhotoGallery.Logic.Main;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class OauthController : BaseController {

		private readonly OauthLogic vOauth;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public OauthController(OauthLogic pOauth) {
			vOauth = pOauth;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult FabricRedirect() {
			if ( vOauth.PersonOauthSuccess(Request) ) {
				Response.Write(OauthLogic.CloseWindowScript);
			}

			return null;
		}

	}

}
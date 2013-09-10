using System.Web;
using System.Web.Mvc;
using Fabric.Clients.Cs;

namespace PhotoGallery.Web.Application {

	/*================================================================================================*/
	public class FabricAuthorizeAttribute : AuthorizeAttribute {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override bool AuthorizeCore(HttpContextBase pHttpContext) {
			return (new FabricClient()).PersonSession.IsAuthenticated;
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override void HandleUnauthorizedRequest(AuthorizationContext pFilterContext) {
			base.HandleUnauthorizedRequest(pFilterContext);
		}

	}

}
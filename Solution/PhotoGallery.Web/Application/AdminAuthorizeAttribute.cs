using System.Web;
using System.Web.Mvc;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Web.Application {

	/*================================================================================================*/
	public class AdminAuthorizeAttribute : AuthorizeAttribute {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override bool AuthorizeCore(HttpContextBase pHttpContext) {
			FabUser u = FabricService.GetActiveUser(new FabricClient());
			return (u != null && u.Name == "zachkinstner");
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override void HandleUnauthorizedRequest(AuthorizationContext pFilterContext) {
			base.HandleUnauthorizedRequest(pFilterContext);
			//var url = new UrlHelper(pFilterContext.RequestContext);
			//pFilterContext.Result = new RedirectResult(url.Action(MVC.Account.Home.Login()));
		}

	}

}
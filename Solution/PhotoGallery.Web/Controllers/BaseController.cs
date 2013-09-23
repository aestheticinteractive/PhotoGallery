using System.Collections.Generic;
using System.Web.Mvc;
using PhotoGallery.Services;

namespace PhotoGallery.Web.Controllers {

	/*================================================================================================*/
	public abstract partial class BaseController : Controller {

		private readonly List<BaseService> vLogicList;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseController() {
			vLogicList = new List<BaseService>();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected void RegisterInjectedLogic(BaseService pService) {
			vLogicList.Add(pService);
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override void OnActionExecuting(ActionExecutingContext pContext) {
			/*if ( IsLoginRequired(pContext.ActionDescriptor.ActionName) && !FwSession.IsLoggedIn ) {
				pContext.Result = RedirectToAction("Login", "Account", new { Area = "" });
				return;
			}*/

			string area = (string)pContext.RouteData.DataTokens["area"];
			string paramList = "";

			foreach ( string key in pContext.ActionParameters.Keys ) {
				paramList += (paramList == "" ? "" : ", ")+key+"="+pContext.ActionParameters[key];
			}

			base.OnActionExecuting(pContext);
		}

	}

}
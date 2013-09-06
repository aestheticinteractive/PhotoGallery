using System.Collections.Generic;
using System.Web.Mvc;
using PhotoGallery.Logic;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Controllers {

	/*================================================================================================*/
	public abstract partial class BaseController : Controller {

		private readonly List<BaseLogic> vLogicList;
		private Dictionary<string, bool> vLoginReqForActionMap;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseController() {
			vLogicList = new List<BaseLogic>();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected void RegisterInjectedLogic(BaseLogic pLogic) {
			vLogicList.Add(pLogic);
		}

		/*--------------------------------------------------------------------------------------------*/
		protected T CreateModel<T>() where T : IBaseModel, new() {
			T model = new T();
			return model;
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


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool IsLoginRequired(string pActionName) {
			bool defaultReq = GetDefaultIsLoginRequired();
			string action = pActionName.ToLower();

			if ( vLoginReqForActionMap == null ) {
				vLoginReqForActionMap = new Dictionary<string, bool>();
				List<string> logReq = LoginActions(true);
				List<string> logNotReq = LoginActions(false);

				for ( int i = 0 ; logReq != null && i < logReq.Count ; ++i ) {
					vLoginReqForActionMap.Add(logReq[i].ToLower(), true);
				}

				for ( int i = 0 ; logNotReq != null && i < logNotReq.Count ; ++i ) {
					vLoginReqForActionMap.Add(logNotReq[i].ToLower(), false);
				}
			}

			if ( vLoginReqForActionMap.ContainsKey(action) ) {
				return vLoginReqForActionMap[action];
			}

			return defaultReq;
		}

		/*--------------------------------------------------------------------------------------------*/
		protected virtual bool GetDefaultIsLoginRequired() {
#if ( !DEBUG || LIVEDB )
			return true;
#else
			return false;
#endif
		}

		/*--------------------------------------------------------------------------------------------*/
		protected virtual List<string> LoginActions(bool pRequiresLogin) {
			return null;
		}

		/*--------------------------------------------------------------------------------------------*/
		public int GetInjectedLogicCount() {
			return vLogicList.Count;
		}

	}

}
using System;
using System.Web.Mvc;

namespace PhotoGallery.Web.Application.Helpers {

	/*================================================================================================*/
	public static class UrlHelperExt {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string Absolute(this UrlHelper pHelp, string pUrl) {
			Uri reqUrl = pHelp.RequestContext.HttpContext.Request.Url;
			if ( reqUrl == null ) { throw new NullReferenceException("reqUrl"); }
			return string.Format("{0}://{1}{2}", reqUrl.Scheme, reqUrl.Authority, pUrl);
		}

	}

}
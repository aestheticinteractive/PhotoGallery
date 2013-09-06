using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Xml.Linq;
using PhotoGallery.Web.Models;

namespace PhotoGallery.Web.Application.Helpers {

	/*================================================================================================*/
	public static class HtmlHelperExt {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static MvcHtmlString ActionLink(this HtmlHelper pHelp, IMvcLink pLink) {
			return pHelp.ActionLink(pLink.Text, pLink.Action);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string ActionLinkUrl(this HtmlHelper pHelp, ActionResult pAction) {
			MvcHtmlString html = pHelp.ActionLink("x", pAction);
			string url = html.ToString();
			int i = url.IndexOf("href=\"")+6;
			return url.Substring(i, url.IndexOf("\">")-i);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string ActionLinkUrl(this HtmlHelper pHelp, IMvcLink pLink) {
			return ActionLinkUrl(pHelp, pLink.Action);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static bool IsHeaderLinkActive(this HtmlHelper pHelp, IMvcLink pLink) {
			RouteData rd = pHelp.ViewContext.RouteData;

			if ( pLink.Action.GetT4MVCResult().Controller != rd.GetRequiredString("controller") ) {
				return false;
			}

			return true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		//derived from http://stackoverflow.com/questions/12107263/
		//	why-is-validationsummarytrue-displaying-an-empty-summary-for-property-errors
		public static MvcHtmlString CustomValidationSummary(this HtmlHelper pHtml, string pMessage) {
			MvcHtmlString html = pHtml.ValidationSummary(true, pMessage);
			if ( html == null ) { return null; }

			XElement xe = XElement.Parse(html.ToHtmlString());
			xe = xe.Element("ul");
			if ( xe == null ) { return null; }

			IEnumerable<XElement> items = xe.Elements("li");
			if ( items.Count() == 1 && items.First().Value == "" ) { return null; }

			return html;
		}

	}

}
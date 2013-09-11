using System.Web.Optimization;

namespace PhotoGallery.Web {

	/*================================================================================================*/
	public class BundleConfig {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection pBundles) {
			pBundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			pBundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.unobtrusive*",
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when 
			// you're ready for production, use the build tool at http://modernizr.com to pick only 
			// the tests you need.
			pBundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			pBundles.Add(new StyleBundle("~/Content/css").Include(
						"~/Content/gallery/*.css"));

			#region Foundation Bundles
			pBundles.Add(new StyleBundle("~/Content/foundation/css").Include(
					   "~/Content/foundation/foundation.css",
					   "~/Content/foundation/foundation.mvc.css",
					   "~/Content/foundation/app.css"));

			pBundles.Add(new ScriptBundle("~/bundles/foundation").Include(
					  "~/Scripts/foundation/jquery.*",
					  "~/Scripts/foundation/app.js"));
			#endregion
		}

	}

}
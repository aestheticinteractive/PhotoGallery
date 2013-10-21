﻿// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public static class MVC
{
    static readonly AccountClass s_Account = new AccountClass();
    public static AccountClass Account { get { return s_Account; } }
    static readonly AdminClass s_Admin = new AdminClass();
    public static AdminClass Admin { get { return s_Admin; } }
    static readonly MainClass s_Main = new MainClass();
    public static MainClass Main { get { return s_Main; } }
    public static PhotoGallery.Web.Controllers.HeaderController Header = new PhotoGallery.Web.Controllers.T4MVC_HeaderController();
    public static T4MVC.HomeController Home = new T4MVC.HomeController();
    public static T4MVC.SharedController Shared = new T4MVC.SharedController();
}

namespace T4MVC
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class AccountClass
    {
        public readonly string Name = "Account";
        public PhotoGallery.Web.Areas.Account.Controllers.AlbumsController Albums = new PhotoGallery.Web.Areas.Account.Controllers.T4MVC_AlbumsController();
        public PhotoGallery.Web.Areas.Account.Controllers.HomeController Home = new PhotoGallery.Web.Areas.Account.Controllers.T4MVC_HomeController();
    }
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class AdminClass
    {
        public readonly string Name = "Admin";
        public PhotoGallery.Web.Areas.Admin.Controllers.HomeController Home = new PhotoGallery.Web.Areas.Admin.Controllers.T4MVC_HomeController();
        public PhotoGallery.Web.Areas.Admin.Controllers.PeopleController People = new PhotoGallery.Web.Areas.Admin.Controllers.T4MVC_PeopleController();
    }
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class MainClass
    {
        public readonly string Name = "Main";
        public PhotoGallery.Web.Areas.Main.Controllers.AlbumsController Albums = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_AlbumsController();
        public PhotoGallery.Web.Areas.Main.Controllers.HomeController Home = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_HomeController();
        public PhotoGallery.Web.Areas.Main.Controllers.OauthController Oauth = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_OauthController();
        public PhotoGallery.Web.Areas.Main.Controllers.PhotosController Photos = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_PhotosController();
        public T4MVC.Main.SharedController Shared = new T4MVC.Main.SharedController();
    }
}

namespace T4MVC
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class Dummy
    {
        private Dummy() { }
        public static Dummy Instance = new Dummy();
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal partial class T4MVC_System_Web_Mvc_ActionResult : System.Web.Mvc.ActionResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_ActionResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
     
    public override void ExecuteResult(System.Web.Mvc.ControllerContext context) { }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}
[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal partial class T4MVC_System_Web_Mvc_RedirectToRouteResult : System.Web.Mvc.RedirectToRouteResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_RedirectToRouteResult(string area, string controller, string action, string protocol = null): base(default(System.Web.Routing.RouteValueDictionary))
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}
[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal partial class T4MVC_System_Web_Mvc_PartialViewResult : System.Web.Mvc.PartialViewResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_PartialViewResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}
[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal partial class T4MVC_System_Web_Mvc_JsonResult : System.Web.Mvc.JsonResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_JsonResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}



namespace Links
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Scripts {
        private const string URLPATH = "~/Scripts";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string _references_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/_references.min.js") ? Url("_references.min.js") : Url("_references.js");
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class exif {
            private const string URLPATH = "~/Scripts/exif";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string binaryajax_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/binaryajax.min.js") ? Url("binaryajax.min.js") : Url("binaryajax.js");
            public static readonly string exif_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/exif.min.js") ? Url("exif.min.js") : Url("exif.js");
            public static readonly string imageinfo_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/imageinfo.min.js") ? Url("imageinfo.min.js") : Url("imageinfo.js");
        }
    
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class foundation {
            private const string URLPATH = "~/Scripts/foundation";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string foundation_abide_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.abide.min.js") ? Url("foundation.abide.min.js") : Url("foundation.abide.js");
            public static readonly string foundation_alerts_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.alerts.min.js") ? Url("foundation.alerts.min.js") : Url("foundation.alerts.js");
            public static readonly string foundation_clearing_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.clearing.min.js") ? Url("foundation.clearing.min.js") : Url("foundation.clearing.js");
            public static readonly string foundation_cookie_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.cookie.min.js") ? Url("foundation.cookie.min.js") : Url("foundation.cookie.js");
            public static readonly string foundation_dropdown_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.dropdown.min.js") ? Url("foundation.dropdown.min.js") : Url("foundation.dropdown.js");
            public static readonly string foundation_forms_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.forms.min.js") ? Url("foundation.forms.min.js") : Url("foundation.forms.js");
            public static readonly string foundation_interchange_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.interchange.min.js") ? Url("foundation.interchange.min.js") : Url("foundation.interchange.js");
            public static readonly string foundation_joyride_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.joyride.min.js") ? Url("foundation.joyride.min.js") : Url("foundation.joyride.js");
            public static readonly string foundation_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.min.js") ? Url("foundation.min.js") : Url("foundation.js");
            public static readonly string foundation_magellan_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.magellan.min.js") ? Url("foundation.magellan.min.js") : Url("foundation.magellan.js");
            public static readonly string foundation_orbit_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.orbit.min.js") ? Url("foundation.orbit.min.js") : Url("foundation.orbit.js");
            public static readonly string foundation_placeholder_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.placeholder.min.js") ? Url("foundation.placeholder.min.js") : Url("foundation.placeholder.js");
            public static readonly string foundation_reveal_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.reveal.min.js") ? Url("foundation.reveal.min.js") : Url("foundation.reveal.js");
            public static readonly string foundation_section_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.section.min.js") ? Url("foundation.section.min.js") : Url("foundation.section.js");
            public static readonly string foundation_tooltips_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.tooltips.min.js") ? Url("foundation.tooltips.min.js") : Url("foundation.tooltips.js");
            public static readonly string foundation_topbar_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.topbar.min.js") ? Url("foundation.topbar.min.js") : Url("foundation.topbar.js");
        }
    
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class gallery {
            private const string URLPATH = "~/Scripts/gallery";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string createAlbum_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/createAlbum.min.js") ? Url("createAlbum.min.js") : Url("createAlbum.js");
            public static readonly string fabPopup_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/fabPopup.min.js") ? Url("fabPopup.min.js") : Url("fabPopup.js");
            public static readonly string gallery_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/gallery.min.js") ? Url("gallery.min.js") : Url("gallery.js");
            public static readonly string PhotoData_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/PhotoData.min.js") ? Url("PhotoData.min.js") : Url("PhotoData.js");
            public static readonly string PhotoSet_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/PhotoSet.min.js") ? Url("PhotoSet.min.js") : Url("PhotoSet.js");
            public static readonly string photoViewer_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/photoViewer.min.js") ? Url("photoViewer.min.js") : Url("photoViewer.js");
        }
    
        public static readonly string jquery_2_0_3_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-2.0.3-vsdoc.min.js") ? Url("jquery-2.0.3-vsdoc.min.js") : Url("jquery-2.0.3-vsdoc.js");
        public static readonly string jquery_2_0_3_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-2.0.3.min.js") ? Url("jquery-2.0.3.min.js") : Url("jquery-2.0.3.js");
        public static readonly string jquery_2_0_3_min_js = Url("jquery-2.0.3.min.js");
        public static readonly string jquery_2_0_3_min_map = Url("jquery-2.0.3.min.map");
        public static readonly string jquery_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.min.js") ? Url("jquery.min.js") : Url("jquery.js");
        public static readonly string jquery_unobtrusive_ajax_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.unobtrusive-ajax.min.js") ? Url("jquery.unobtrusive-ajax.min.js") : Url("jquery.unobtrusive-ajax.js");
        public static readonly string jquery_unobtrusive_ajax_min_js = Url("jquery.unobtrusive-ajax.min.js");
        public static readonly string jquery_validate_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate-vsdoc.min.js") ? Url("jquery.validate-vsdoc.min.js") : Url("jquery.validate-vsdoc.js");
        public static readonly string jquery_validate_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.min.js") ? Url("jquery.validate.min.js") : Url("jquery.validate.js");
        public static readonly string jquery_validate_min_js = Url("jquery.validate.min.js");
        public static readonly string jquery_validate_unobtrusive_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.unobtrusive.min.js") ? Url("jquery.validate.unobtrusive.min.js") : Url("jquery.validate.unobtrusive.js");
        public static readonly string jquery_validate_unobtrusive_min_js = Url("jquery.validate.unobtrusive.min.js");
        public static readonly string modernizr_2_6_2_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/modernizr-2.6.2.min.js") ? Url("modernizr-2.6.2.min.js") : Url("modernizr-2.6.2.js");
        public static readonly string zepto_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/zepto.min.js") ? Url("zepto.min.js") : Url("zepto.js");
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Content {
        private const string URLPATH = "~/Content";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class foundation {
            private const string URLPATH = "~/Content/foundation";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string app_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/app.min.css") ? Url("app.min.css") : Url("app.css");
                 
            public static readonly string foundation_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.min.css") ? Url("foundation.min.css") : Url("foundation.css");
                 
            public static readonly string foundation_mvc_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.mvc.min.css") ? Url("foundation.mvc.min.css") : Url("foundation.mvc.css");
                 
            public static readonly string normalize_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/normalize.min.css") ? Url("normalize.min.css") : Url("normalize.css");
                 
        }
    
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class gallery {
            private const string URLPATH = "~/Content/gallery";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string arrow_left_png = Url("arrow-left.png");
            public static readonly string arrow_right_png = Url("arrow-right.png");
            public static readonly string close_png = Url("close.png");
            public static readonly string Site_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/Site.min.css") ? Url("Site.min.css") : Url("Site.css");
                 
            public static readonly string tag_png = Url("tag.png");
            public static readonly string wait_gif = Url("wait.gif");
        }
    
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static partial class Bundles
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static partial class Scripts {}
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static partial class Styles {}
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal static class T4MVCHelpers {
    // You can change the ProcessVirtualPath method to modify the path that gets returned to the client.
    // e.g. you can prepend a domain, or append a query string:
    //      return "http://localhost" + path + "?foo=bar";
    private static string ProcessVirtualPathDefault(string virtualPath) {
        // The path that comes in starts with ~/ and must first be made absolute
        string path = VirtualPathUtility.ToAbsolute(virtualPath);
        
        // Add your own modifications here before returning the path
        return path;
    }

    // Calling ProcessVirtualPath through delegate to allow it to be replaced for unit testing
    public static Func<string, string> ProcessVirtualPath = ProcessVirtualPathDefault;

    // Calling T4Extension.TimestampString through delegate to allow it to be replaced for unit testing and other purposes
    public static Func<string, string> TimestampString = System.Web.Mvc.T4Extensions.TimestampString;

    // Logic to determine if the app is running in production or dev environment
    public static bool IsProduction() { 
        return (HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled); 
    }
}





#endregion T4MVC
#pragma warning restore 1591



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
    static readonly MainClass s_Main = new MainClass();
    public static MainClass Main { get { return s_Main; } }
    public static PhotoGallery.Web.Controllers.HeaderController Header = new PhotoGallery.Web.Controllers.T4MVC_HeaderController();
    public static T4MVC.SharedController Shared = new T4MVC.SharedController();
}

namespace T4MVC
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class MainClass
    {
        public readonly string Name = "Main";
        public PhotoGallery.Web.Areas.Main.Controllers.HomeController Home = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_HomeController();
        public PhotoGallery.Web.Areas.Main.Controllers.OauthController Oauth = new PhotoGallery.Web.Areas.Main.Controllers.T4MVC_OauthController();
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



namespace Links
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Scripts {
        private const string URLPATH = "~/Scripts";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string _references_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/_references.min.js") ? Url("_references.min.js") : Url("_references.js");
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class foundation {
            private const string URLPATH = "~/Scripts/foundation";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string app_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/app.min.js") ? Url("app.min.js") : Url("app.js");
            public static readonly string foundation_min_js = Url("foundation.min.js");
            public static readonly string jquery_foundation_accordion_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.accordion.min.js") ? Url("jquery.foundation.accordion.min.js") : Url("jquery.foundation.accordion.js");
            public static readonly string jquery_foundation_alerts_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.alerts.min.js") ? Url("jquery.foundation.alerts.min.js") : Url("jquery.foundation.alerts.js");
            public static readonly string jquery_foundation_buttons_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.buttons.min.js") ? Url("jquery.foundation.buttons.min.js") : Url("jquery.foundation.buttons.js");
            public static readonly string jquery_foundation_clearing_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.clearing.min.js") ? Url("jquery.foundation.clearing.min.js") : Url("jquery.foundation.clearing.js");
            public static readonly string jquery_foundation_forms_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.forms.min.js") ? Url("jquery.foundation.forms.min.js") : Url("jquery.foundation.forms.js");
            public static readonly string jquery_foundation_joyride_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.joyride.min.js") ? Url("jquery.foundation.joyride.min.js") : Url("jquery.foundation.joyride.js");
            public static readonly string jquery_foundation_magellan_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.magellan.min.js") ? Url("jquery.foundation.magellan.min.js") : Url("jquery.foundation.magellan.js");
            public static readonly string jquery_foundation_mediaQueryToggle_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.mediaQueryToggle.min.js") ? Url("jquery.foundation.mediaQueryToggle.min.js") : Url("jquery.foundation.mediaQueryToggle.js");
            public static readonly string jquery_foundation_navigation_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.navigation.min.js") ? Url("jquery.foundation.navigation.min.js") : Url("jquery.foundation.navigation.js");
            public static readonly string jquery_foundation_orbit_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.orbit.min.js") ? Url("jquery.foundation.orbit.min.js") : Url("jquery.foundation.orbit.js");
            public static readonly string jquery_foundation_reveal_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.reveal.min.js") ? Url("jquery.foundation.reveal.min.js") : Url("jquery.foundation.reveal.js");
            public static readonly string jquery_foundation_tabs_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.tabs.min.js") ? Url("jquery.foundation.tabs.min.js") : Url("jquery.foundation.tabs.js");
            public static readonly string jquery_foundation_tooltips_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.foundation.tooltips.min.js") ? Url("jquery.foundation.tooltips.min.js") : Url("jquery.foundation.tooltips.js");
            public static readonly string jquery_placeholder_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.placeholder.min.js") ? Url("jquery.placeholder.min.js") : Url("jquery.placeholder.js");
            public static readonly string modernizr_foundation_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/modernizr.foundation.min.js") ? Url("modernizr.foundation.min.js") : Url("modernizr.foundation.js");
            public static readonly string XXXjquery_foundation_topbar_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/XXXjquery.foundation.topbar.min.js") ? Url("XXXjquery.foundation.topbar.min.js") : Url("XXXjquery.foundation.topbar.js");
        }
    
        public static readonly string jquery_2_0_3_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-2.0.3-vsdoc.min.js") ? Url("jquery-2.0.3-vsdoc.min.js") : Url("jquery-2.0.3-vsdoc.js");
        public static readonly string jquery_2_0_3_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-2.0.3.min.js") ? Url("jquery-2.0.3.min.js") : Url("jquery-2.0.3.js");
        public static readonly string jquery_2_0_3_min_js = Url("jquery-2.0.3.min.js");
        public static readonly string jquery_2_0_3_min_map = Url("jquery-2.0.3.min.map");
        public static readonly string jquery_unobtrusive_ajax_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.unobtrusive-ajax.min.js") ? Url("jquery.unobtrusive-ajax.min.js") : Url("jquery.unobtrusive-ajax.js");
        public static readonly string jquery_unobtrusive_ajax_min_js = Url("jquery.unobtrusive-ajax.min.js");
        public static readonly string jquery_validate_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate-vsdoc.min.js") ? Url("jquery.validate-vsdoc.min.js") : Url("jquery.validate-vsdoc.js");
        public static readonly string jquery_validate_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.min.js") ? Url("jquery.validate.min.js") : Url("jquery.validate.js");
        public static readonly string jquery_validate_min_js = Url("jquery.validate.min.js");
        public static readonly string jquery_validate_unobtrusive_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.unobtrusive.min.js") ? Url("jquery.validate.unobtrusive.min.js") : Url("jquery.validate.unobtrusive.js");
        public static readonly string jquery_validate_unobtrusive_min_js = Url("jquery.validate.unobtrusive.min.js");
        public static readonly string modernizr_2_6_2_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/modernizr-2.6.2.min.js") ? Url("modernizr-2.6.2.min.js") : Url("modernizr-2.6.2.js");
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
            public static readonly string foundation_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.min.css") ? Url("foundation.min.css") : Url("foundation.css");
                 
            public static readonly string foundation_mvc_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/foundation.mvc.min.css") ? Url("foundation.mvc.min.css") : Url("foundation.mvc.css");
                 
        }
    
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class gallery {
            private const string URLPATH = "~/Content/gallery";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string Site_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/Site.min.css") ? Url("Site.min.css") : Url("Site.css");
                 
        }
    
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class images {
            private const string URLPATH = "~/Content/images";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public static class foundation {
                private const string URLPATH = "~/Content/images/foundation";
                public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
                public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
                [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
                public static class orbit {
                    private const string URLPATH = "~/Content/images/foundation/orbit";
                    public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
                    public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
                    public static readonly string bullets_jpg = Url("bullets.jpg");
                    public static readonly string left_arrow_small_png = Url("left-arrow-small.png");
                    public static readonly string left_arrow_png = Url("left-arrow.png");
                    public static readonly string loading_gif = Url("loading.gif");
                    public static readonly string mask_black_png = Url("mask-black.png");
                    public static readonly string pause_black_png = Url("pause-black.png");
                    public static readonly string right_arrow_small_png = Url("right-arrow-small.png");
                    public static readonly string right_arrow_png = Url("right-arrow.png");
                    public static readonly string rotator_black_png = Url("rotator-black.png");
                    public static readonly string timer_black_png = Url("timer-black.png");
                }
            
            }
        
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



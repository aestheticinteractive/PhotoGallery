// <auto-generated />
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
namespace PhotoGallery.Web.Areas.Main.Controllers
{
    public partial class AlbumsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected AlbumsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult All()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.All);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.PartialViewResult AllPage()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.AllPage);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Photos()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Photos);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public AlbumsController Actions { get { return MVC.Main.Albums; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Main";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Albums";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Albums";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string All = "All";
            public readonly string AllPage = "AllPage";
            public readonly string Photos = "Photos";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string All = "All";
            public const string AllPage = "AllPage";
            public const string Photos = "Photos";
        }


        static readonly ActionParamsClass_All s_params_All = new ActionParamsClass_All();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_All AllParams { get { return s_params_All; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_All
        {
            public readonly string i = "i";
        }
        static readonly ActionParamsClass_AllPage s_params_AllPage = new ActionParamsClass_AllPage();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AllPage AllPageParams { get { return s_params_AllPage; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AllPage
        {
            public readonly string i = "i";
        }
        static readonly ActionParamsClass_Photos s_params_Photos = new ActionParamsClass_Photos();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Photos PhotosParams { get { return s_params_Photos; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Photos
        {
            public readonly string id = "id";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _AlbumThumb = "_AlbumThumb";
                public readonly string _AllPage = "_AllPage";
                public readonly string All = "All";
                public readonly string Index = "Index";
                public readonly string Photos = "Photos";
            }
            public readonly string _AlbumThumb = "~/Areas/Main/Views/Albums/_AlbumThumb.cshtml";
            public readonly string _AllPage = "~/Areas/Main/Views/Albums/_AllPage.cshtml";
            public readonly string All = "~/Areas/Main/Views/Albums/All.cshtml";
            public readonly string Index = "~/Areas/Main/Views/Albums/Index.cshtml";
            public readonly string Photos = "~/Areas/Main/Views/Albums/Photos.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_AlbumsController : PhotoGallery.Web.Areas.Main.Controllers.AlbumsController
    {
        public T4MVC_AlbumsController() : base(Dummy.Instance) { }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        partial void AllOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int? i);

        public override System.Web.Mvc.ActionResult All(int? i)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.All);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "i", i);
            AllOverride(callInfo, i);
            return callInfo;
        }

        partial void AllPageOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, int i);

        public override System.Web.Mvc.PartialViewResult AllPage(int i)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.AllPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "i", i);
            AllPageOverride(callInfo, i);
            return callInfo;
        }

        partial void PhotosOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int id);

        public override System.Web.Mvc.ActionResult Photos(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Photos);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            PhotosOverride(callInfo, id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591

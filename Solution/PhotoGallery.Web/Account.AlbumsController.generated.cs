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
namespace PhotoGallery.Web.Areas.Account.Controllers
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
        public virtual System.Web.Mvc.PartialViewResult UpdateAlbumTitle()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.UpdateAlbumTitle);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.PartialViewResult UploadImage()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.UploadImage);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public AlbumsController Actions { get { return MVC.Account.Albums; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Account";
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
            public readonly string Create = "Create";
            public readonly string UpdateAlbumTitle = "UpdateAlbumTitle";
            public readonly string UploadImage = "UploadImage";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Create = "Create";
            public const string UpdateAlbumTitle = "UpdateAlbumTitle";
            public const string UploadImage = "UploadImage";
        }


        static readonly ActionParamsClass_UpdateAlbumTitle s_params_UpdateAlbumTitle = new ActionParamsClass_UpdateAlbumTitle();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpdateAlbumTitle UpdateAlbumTitleParams { get { return s_params_UpdateAlbumTitle; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpdateAlbumTitle
        {
            public readonly string pModel = "pModel";
        }
        static readonly ActionParamsClass_UploadImage s_params_UploadImage = new ActionParamsClass_UploadImage();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UploadImage UploadImageParams { get { return s_params_UploadImage; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UploadImage
        {
            public readonly string pModel = "pModel";
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
                public readonly string Create = "Create";
                public readonly string Index = "Index";
            }
            public readonly string Create = "~/Areas/Account/Views/Albums/Create.cshtml";
            public readonly string Index = "~/Areas/Account/Views/Albums/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_AlbumsController : PhotoGallery.Web.Areas.Account.Controllers.AlbumsController
    {
        public T4MVC_AlbumsController() : base(Dummy.Instance) { }

        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        partial void CreateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Create()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Create);
            CreateOverride(callInfo);
            return callInfo;
        }

        partial void UpdateAlbumTitleOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, PhotoGallery.Web.Areas.Account.Models.AlbumCreateTitleModel pModel);

        public override System.Web.Mvc.PartialViewResult UpdateAlbumTitle(PhotoGallery.Web.Areas.Account.Models.AlbumCreateTitleModel pModel)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.UpdateAlbumTitle);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "pModel", pModel);
            UpdateAlbumTitleOverride(callInfo, pModel);
            return callInfo;
        }

        partial void UploadImageOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, PhotoGallery.Web.Areas.Account.Models.AlbumCreateImageModel pModel);

        public override System.Web.Mvc.PartialViewResult UploadImage(PhotoGallery.Web.Areas.Account.Models.AlbumCreateImageModel pModel)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.UploadImage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "pModel", pModel);
            UploadImageOverride(callInfo, pModel);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
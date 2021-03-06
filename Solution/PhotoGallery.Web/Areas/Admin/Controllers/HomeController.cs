﻿using System.Web.Mvc;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Admin.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Admin.Controllers {

	/*================================================================================================*/
	public partial class HomeController : BaseController {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeController() {
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[AdminAuthorize]
		public virtual ActionResult Index() {
			var m = new HomeModel();
			return View(m);
		}

	}

}
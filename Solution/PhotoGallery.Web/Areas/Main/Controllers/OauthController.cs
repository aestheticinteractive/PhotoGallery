﻿using System.Web.Mvc;
using PhotoGallery.Logic.Main;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class OauthController : BaseController {

		private readonly OauthService vOauth;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public OauthController(OauthService pOauth) {
			vOauth = pOauth;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual ActionResult FabricRedirect() {
			if ( vOauth.PersonOauthSuccess(Request) ) {
				Response.Write(OauthService.CloseWindowScript);
			}

			return null;
		}

	}

}
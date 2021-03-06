﻿using System.Web.Mvc;
using Fabric.Clients.Cs;
using PhotoGallery.Services.Account;
using PhotoGallery.Services.Main;
using PhotoGallery.Web.Areas.Main.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Main.Controllers {

	/*================================================================================================*/
	public partial class PhotosController : BaseController {

		private readonly PhotosService vPhotos;
		private readonly CreateService vCreate;
		private readonly SearchService vSearch;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotosController(PhotosService pPhotos, CreateService pCreate) {
			vPhotos = pPhotos;
			vCreate = pCreate;
			//TODO: inject this using a custom IFabricClientDataProv interface
			vSearch = new SearchService(new FabricClient(FabricWebApplication.DataProvConfigKey));
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[HttpPost]
		public virtual JsonResult Tags(int? id) {
			if ( id == null ) {
				return null;
			}

			return Json(vPhotos.GetTags((int)id));
		}
		
		/*--------------------------------------------------------------------------------------------*/
		[HttpPost]
		public virtual JsonResult FindTags(string id, bool? first) {
			return Json(vSearch.FindTags(id, (first ?? false)));
		}

		/*--------------------------------------------------------------------------------------------*/
		[HttpPost]
		public virtual JsonResult AddTag(PhotoAddTagModel pModel) {
			bool res = false;

			if ( ModelState.IsValid ) {
				long artId = long.Parse(pModel.ArtifactId);
				res = vCreate.AddTag(pModel.PhotoId, artId, pModel.PosX, pModel.PosY);
			}

			return Json(new { success = res });
		}

	}

}
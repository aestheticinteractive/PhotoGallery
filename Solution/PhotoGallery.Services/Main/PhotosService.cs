﻿using System.Collections.Generic;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {

	/*================================================================================================*/
	public class PhotosService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotosService(IFabricClient pFab) : base(pFab) { }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebAlbum> AddTag(long pPhotoId, long pArtifactId, double pPosX, double pPosY) {
			using ( ISession s = NewSession() ) {
				Photo p = s.QueryOver<Photo>()
					.Where(x => x.Id == pPhotoId)
					.Fetch(x => x.FabricArtifact).Eager
					.SingleOrDefault();

				var fb = new FabricFactorBuilder(null, "<photo "+pPhotoId+"> refers to "+
						"<artifact "+pArtifactId+"> [loc 2d "+pPosX+", "+pPosY+"]");
				fb.Init(
					p.FabricArtifact,
					FabEnumsData.DescriptorTypeId.RefersTo,
					pArtifactId,
					FabEnumsData.FactorAssertionId.Fact,
					false
				);
				fb.AddLocator(
					FabEnumsData.LocatorTypeId.RelPos2D,
					pPosX,
					pPosY,
					0
				);
				fb.DesRelatedArtifactRefineId = LiveArtifactId.PhotographAlbum;
				s.Save(fb.ToFactor());

			};
		}

	}

}
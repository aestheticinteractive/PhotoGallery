using System.Collections.Generic;
using System.Linq;
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
		public IList<WebPhotoTag> GetTags(int pPhotoId) {
			using ( ISession s = NewSession() ) {
				Photo photo = s.Get<Photo>(pPhotoId);

				IList<FabricFactor> factors = s.QueryOver<FabricFactor>()
					.Where(x => x.Primary.Id == photo.FabricArtifact.Id)
					.Where(x => x.DesTypeId == (byte)DescriptorTypeId.RefersTo)
					.Where(x => x.DesTypeRefineId == (long)LiveArtifactId.Depict)
					.Fetch(x => x.Related).Eager
					.List();

				return factors.Select(ff => new WebPhotoTag(photo, ff)).ToList();
			}
		}

	}

}
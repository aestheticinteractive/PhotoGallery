using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Services.Main {

	/*================================================================================================*/
	public class PhotosService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotosService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool AddTag(long pPhotoId, long pArtifactId, double pPosX, double pPosY) {
			using ( ISession s = NewSession() ) {
				Photo p = s.QueryOver<Photo>()
					.Where(x => x.Id == pPhotoId)
					.Fetch(x => x.FabricArtifact).Eager
					.SingleOrDefault();
				
				var fabUser = FabricService.GetActiveUser(Fab);

				if ( fabUser == null ) {
					return false;
				}
				
				Fab.PersonSession.RefreshTokenIfNecessary();

				var userArt = s.Load<FabricArtifact>(fabUser.ArtifactId);
				var fb = new FabricFactorBuilder(userArt, "<photo "+pPhotoId+"> refers to ('depict') "+
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
				fb.DesTypeRefineId = LiveArtifactId.Depict;
				s.Save(fb.ToFactor());

				return true;
			};
		}

	}

}
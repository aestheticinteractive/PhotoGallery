using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Main;

namespace PhotoGallery.Services.Account {

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
				
				FabricUser u = HomeService.GetCurrentUser(Fab, s);

				if ( u == null ) {
					return false;
				}
				
				Fab.PersonSession.RefreshTokenIfNecessary();
				var userArt = s.Load<FabricArtifact>(u.FabricArtifact.Id);

				var fb = new FabricFactorBuilder(userArt, "<photo "+pPhotoId+"> refers to ('depict') "+
						"<artifact> [loc 2d "+pPosX+", "+pPosY+"]");
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

				OauthService.AddFabricPersonSession(Fab.PersonSession, u, s);
				return true;
			};
		}

	}

}
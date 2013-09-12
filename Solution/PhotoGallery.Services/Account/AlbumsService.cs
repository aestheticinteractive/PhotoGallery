using System.Web;
using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Account.Tools;

namespace PhotoGallery.Services.Account {
	
	/*================================================================================================*/
	public class AlbumsService : BaseLogic {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumsService(IFabricClient pFab) : base(pFab) {}

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public int? AddAlbum(string pTitle) {
			using ( ISession s = NewSession() ) {
				FabricUser u = HomeService.GetCurrentUser(Fab, s);

				if ( u == null ) {
					return null;
				}

				using ( ITransaction tx = s.BeginTransaction() ) {
					var fa = new FabricArtifact();
					fa.Type = (byte)FabricArtifact.ArtifactType.Album;
					s.Save(fa);

					var a = new Album();
					a.Title = pTitle;
					a.FabricUser = u;
					a.FabricArtifact = fa;
					s.Save(a);

					tx.Commit();

					return a.Id;
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebUploadResult AddAlbumPhoto(HttpServerUtilityBase pServer, int pAlbumId,
												string pFilename, string pExifData, string pImageData) {
			using ( ISession s = NewSession() ) {
				FabricUser u = HomeService.GetCurrentUser(Fab, s);

				if ( u == null ) {
					return null;
				}

				Album a = s.Get<Album>(pAlbumId);

				if ( a == null || a.FabricUser.Id != u.Id ) {
					return null;
				}

				var up = new PhotoUploader(pServer, pAlbumId, pFilename, pExifData, pImageData);
				up.SaveFile(s);
				return up.Result;
			}
		}

	}

}
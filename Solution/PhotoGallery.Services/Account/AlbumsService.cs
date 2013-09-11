using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
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

				var a = new Album();
				a.Title = pTitle;
				a.FabricUser = u;
				s.Save(a);

				return a.Id;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebUploadResult AddAlbumPhoto(int pAlbumId, string pFilename, 
																string pExifData, string pImageData) {
			using ( ISession s = NewSession() ) {
				FabricUser u = HomeService.GetCurrentUser(Fab, s);
				Log.Debug("AddPhotoAlbum user: "+u);

				if ( u == null ) {
					return null;
				}

				/*Album a = s.Get<Album>(pAlbumId);
				Log.Debug("AddPhotoAlbum album: "+a);
				Log.Debug("AddPhotoAlbum owner: "+a.FabricUser.Id+" == "+u.Id);

				if ( a == null || a.FabricUser.Id != u.Id ) {
					return null;
				}*/

				Log.Debug("AddPhotoAlbum preup");
				var up = new PhotoUploader(pAlbumId, pFilename, pExifData, pImageData);
				Log.Debug("AddPhotoAlbum up: "+up);
				up.SaveFile(s);
				Log.Debug("AddPhotoAlbum save: "+up.Result);
				Log.Debug("AddPhotoAlbum res: "+up.Result.Status+" / "+up.Result.PhotoId);
				return up.Result;
			}
		}

	}

}
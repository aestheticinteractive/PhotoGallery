using System;
using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
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
					var albumArt = new FabricArtifact();
					albumArt.Type = (byte)FabricArtifact.ArtifactType.Album;
					s.Save(albumArt);

					var a = new Album();
					a.Title = pTitle;
					a.FabricUser = u;
					a.FabricArtifact = albumArt;
					a.Created = DateTime.UtcNow.Ticks;
					s.Save(a);

					////

					var cre = new DateTime(a.Created);
					var userArt = s.Load<FabricArtifact>(u.FabricArtifact.Id);					
					
					var fb = new FabricFactorBuilder(userArt, "<album "+a.Title+"> refers to "+
						"'Kinstner Photo Gallery' ('photograph album') [iden: 'key' "+a.Id+"]");
					fb.Init(
						albumArt,
						FabEnumsData.DescriptorTypeId.RefersTo,
						LiveArtifactId.KinstnerPhotoGallery,
						FabEnumsData.FactorAssertionId.Fact,
						true
					);
					fb.AddIdentor(
						FabEnumsData.IdentorTypeId.Key,
						a.Id+""
					);
					fb.DesRelatedArtifactRefineId = LiveArtifactId.PhotographAlbum;
					s.Save(fb.ToFactor());

					////

					fb = new FabricFactorBuilder(userArt, 
						"<album "+a.Title+"> is a <photograph album>");
					fb.Init(
						albumArt,
						FabEnumsData.DescriptorTypeId.IsA,
						LiveArtifactId.PhotographAlbum,
						FabEnumsData.FactorAssertionId.Fact,
						true
					);
					s.Save(fb.ToFactor());

					////

					fb = new FabricFactorBuilder(userArt, 
						"<album> created by <user "+u.Name+"> [eventor: occur "+cre+"]");
					fb.Init(
						albumArt,
						FabEnumsData.DescriptorTypeId.IsCreatedBy,
						userArt,
						FabEnumsData.FactorAssertionId.Fact,
						true
					);
					fb.AddEventor(
						FabEnumsData.EventorTypeId.Occur,
						cre
					);
					s.Save(fb.ToFactor());

					////
					
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

				var up = new PhotoUploader(pServer, u, a, pFilename, pExifData, pImageData);
				up.SaveFile(s);
				return up.Result;
			}
		}

	}

}
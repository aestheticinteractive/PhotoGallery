using System;
using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Services.Account {
	
	/*================================================================================================*/
	public class AlbumsService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumsService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string GetAlbumTitle(int pAlbumId) {
			using ( ISession s = NewSession() ) {
				Album a = s.Get<Album>(pAlbumId);
				return (a == null ? null : a.Title);
			}
		}

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public int? AddAlbum(string pTitle) {
			using ( ISession s = NewSession() ) {
				FabricUser u = HomeService.GetCurrentUser(Fab, s);

				if ( u == null ) {
					return null;
				}

				var userArt = s.Load<FabricArtifact>(u.FabricArtifact.Id);

				using ( ITransaction tx = s.BeginTransaction() ) {
					var albumArt = new FabricArtifact();
					albumArt.Type = (byte)FabricArtifact.ArtifactType.Album;
					albumArt.Name = pTitle;
					albumArt.Disamb = "photograph album";
					albumArt.Creator = userArt;
					s.Save(albumArt);

					var a = new Album();
					a.Title = pTitle;
					a.FabricUser = u;
					a.FabricArtifact = albumArt;
					a.Created = DateTime.UtcNow.Ticks;
					s.Save(a);

					////

					var cre = new DateTime(a.Created);

					var fb = new FabricFactorBuilder(null, "<album "+a.Title+"> refers to "+
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

					fb = new FabricFactorBuilder(null, 
						"<album "+a.Title+"> is an instance of <photograph album>");
					fb.Init(
						albumArt,
						FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
						LiveArtifactId.PhotographAlbum,
						FabEnumsData.FactorAssertionId.Fact,
						true
					);
					s.Save(fb.ToFactor());

					////

					fb = new FabricFactorBuilder(null, 
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
							string pFilename, string pExifData, string pImageData, bool pLastImage) {
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

				if ( pLastImage ) {
					FabricService.CheckForNewTasks(Fab);
				}

				return up.Result;
			}
		}

	}

}
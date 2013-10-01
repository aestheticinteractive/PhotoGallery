using System;
using System.Linq;
using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using Fabric.Clients.Cs.Session;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class OauthService : BaseService {

		public const string CloseWindowScript = 
			"<script type='text/javascript'>"+
				"window.opener.location.reload();"+
				"window.close();"+
			"</script>";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public OauthService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool PersonOauthSuccess(HttpRequestBase pRequest, HttpResponseBase pResponse) {
			FabOauthAccess result = Fab.PersonSession.HandleGrantCodeRedirect(pRequest);
			Fab.PersonSession.SaveToCookies(pResponse.Cookies);

			if ( Fab.PersonSession.IsAuthenticated ) {
				FabricUser u = CreateUser();
				AddFabricPersonSession(Fab.PersonSession, u, NewSession());
				return true;
			}

			Log.Error("FabricRedirect: "+result);
			return false;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private FabricUser CreateUser() {
			using ( ISession sess = NewSession() ) {
				FabUser fabUser = FabricService.GetActiveUser(Fab);

				FabricUser u = sess.QueryOver<FabricUser>()
					.Where(x => x.Name == fabUser.Name)
					.SingleOrDefault();

				if ( u != null ) {
					return u;
				}

				var a = new FabricArtifact();
				a.ArtifactId = fabUser.ArtifactId;
				a.Type = (byte)FabricArtifact.ArtifactType.User;
				a.Name = fabUser.Name;
				a.FromFab = true;
				sess.Save(a);

				u = new FabricUser();
				u.Name = fabUser.Name;
				u.Created = DateTime.UtcNow.Ticks;
				u.FabricArtifact = a;
				sess.Save(u);

				return u;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static void AddFabricPersonSession(IFabricPersonSession pPerson,
																	FabricUser pUser, ISession pSess) {
			FabricPersonSession fps = pSess.QueryOver<FabricPersonSession>()
				.Where(x => x.SessionId == pPerson.SessionId)
				.Take(1)
				.List()
				.FirstOrDefault();

			if ( fps == null ) {
				fps = new FabricPersonSession();
				fps.FabricUser = pUser;
				fps.SessionId = pPerson.SessionId;
			}

			fps.GrantCode = pPerson.GrantCode;
			fps.BearerToken = pPerson.BearerToken;
			fps.RefreshToken = pPerson.RefreshToken;
			fps.Expiration = pPerson.Expiration.Ticks;
			fps.TryUpdate = true;

			pSess.SaveOrUpdate(fps);
			pSess.Flush();
		}

	}

}
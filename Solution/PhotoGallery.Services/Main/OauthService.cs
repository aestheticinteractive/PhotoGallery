using System;
using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class OauthService : BaseLogic {

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
		public bool PersonOauthSuccess(HttpRequestBase pRequest) {
			FabOauthAccess result = Fab.PersonSession.HandleGrantCodeRedirect(pRequest);

			if ( Fab.PersonSession.IsAuthenticated ) {
				CreateUser();
				return true;
			}

			Log.Error("FabricRedirect: "+result);
			return false;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void CreateUser() {
			using ( ISession sess = NewSession() ) {
				FabUser fabUser = Fab.Services.Traversal.GetActiveUser.Get().FirstDataItem();

				FabricUser u = sess.QueryOver<FabricUser>()
					.Where(x => x.Name == fabUser.Name)
					.SingleOrDefault();

				if ( u != null ) {
					return;
				}

				var a = new FabricArtifact();
				a.ArtifactId = fabUser.ArtifactId;
				a.Type = (byte)FabricArtifact.ArtifactType.User;
				sess.Save(a);

				u = new FabricUser();
				u.Name = fabUser.Name;
				u.Created = DateTime.UtcNow.Ticks;
				u.FabricArtifact = a;
				sess.Save(u);
			}
		}

	}

}
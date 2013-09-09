using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
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
				return true;
			}

			Log.Error("FabricRedirect: "+result);
			return false;
		}

	}

}
using System.Web;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Logic.Main {
	
	/*================================================================================================*/
	public class OauthLogic : BaseLogic {

		public const string CloseWindowScript = 
			"<script type='text/javascript'>"+
				"window.opener.location.reload();"+
				"window.close();"+
			"</script>";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public OauthLogic(IFabricClient pFab) : base(pFab) {}


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
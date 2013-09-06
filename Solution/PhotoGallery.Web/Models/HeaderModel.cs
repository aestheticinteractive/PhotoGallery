using Fabric.Clients.Cs;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public class HeaderModel {

		private readonly IFabricClient vFabric;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HeaderModel(IFabricClient pFabric) {
			vFabric = pFabric;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool IsPersonAuthenticated() {
			return vFabric.PersonSession.IsAuthenticated;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public string GetPersonLoginOpenScript(bool pSwitch) {
			string url = vFabric.PersonSession.GetGrantCodeUrl(pSwitch);
			return vFabric.PersonSession.GetGrantWindowOpenScript(url);
		}

	}

}
using Fabric.Clients.Cs;
using PhotoGallery.Database;

namespace PhotoGallery.Logic {

	/*================================================================================================*/
	public class BaseLogic {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseLogic(IFabricClient pFab) {
			Fab = pFab;
			Connect.InitOnce();
		}

	}

}
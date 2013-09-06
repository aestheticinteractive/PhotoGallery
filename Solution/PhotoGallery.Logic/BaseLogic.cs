using Fabric.Clients.Cs;

namespace PhotoGallery.Logic {

	/*================================================================================================*/
	public class BaseLogic {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseLogic(IFabricClient pFab) {
			Fab = pFab;
		}

	}

}
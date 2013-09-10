using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	public class BaseLogic {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseLogic(IFabricClient pFab) {
			Fab = pFab;

			if ( Connect.SessionFactory == null ) {
				Connect.InitOnce();
				//Connect.UpdateSchema();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		protected ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}
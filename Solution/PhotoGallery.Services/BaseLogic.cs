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
			Fab.Config.Logger = new LogFabric();
			//OneTimeLogic.CreateIsoSpeedClass(pFab);

			if ( Connect.SessionFactory == null ) {
				Connect.InitOnce();
				Connect.UpdateSchema();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}
using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	public class BaseService {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseService(IFabricClient pFab) {
			Fab = pFab;
			Fab.Config.Logger = new LogFabric();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void InitDatabase() {
			if ( Connect.SessionFactory != null ) {
				return;
			}

			Connect.InitOnce();
			Connect.UpdateSchema();
			//OneTimeLogic.CreateIsoSpeedClass(pFab);
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}
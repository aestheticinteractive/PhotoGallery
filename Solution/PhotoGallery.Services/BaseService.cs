using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	public class BaseService {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseService(IFabricClient pFab) {
			Fab = pFab;
			FabricService.SetupClientLogger(Fab);
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
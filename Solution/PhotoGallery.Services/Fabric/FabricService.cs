using System.Threading;
using Fabric.Clients.Cs;
using PhotoGallery.Services.Fabric.Tools;

namespace PhotoGallery.Services.Fabric {
	
	/*================================================================================================*/
	public static class FabricService {

		private static IFabricClient DataProvClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void SetDataProvClient(IFabricClient pFab) {
			DataProvClient = pFab;
			DataProvClient.UseDataProviderPerson = true;
			SetupClientLogger(pFab);

			CheckForNewTasks();
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static void SetupClientLogger(IFabricClient pFab) {
			pFab.Config.Logger = new LogFabric();
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void CheckForNewTasks(IFabricClient pUserFabClient=null) {
			var t = new Thread(FabricExporter.StartDataProvThread);
			t.Start(DataProvClient);

			if ( pUserFabClient != null ) {
				t = new Thread(FabricExporter.StartUserThread);
				t.Start(pUserFabClient);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			FabricExporter.StopThreads = true;
		}

	}

}
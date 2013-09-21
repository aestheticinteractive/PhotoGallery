using System;
using System.Runtime.Caching;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Fabric.Tools;

namespace PhotoGallery.Services.Fabric {
	
	/*================================================================================================*/
	public static class FabricService {

		private static readonly MemoryCache ActiveUsers = new MemoryCache("ActiveUsers");
		private static IFabricClient DataProvClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static FabUser GetActiveUser(IFabricClient pFab) {
			if ( !pFab.PersonSession.IsAuthenticated ) {
				ActiveUsers.Remove(pFab.PersonSession.SessionId);
				return null;
			}

			FabUser fu = (ActiveUsers.Get(pFab.PersonSession.SessionId) as FabUser);

			//Log.Debug("FabricService ActiveUser Cache: Hit? "+
			//	(fu != null)+" ("+pFab.PersonSession.SessionId+")");

			if ( fu == null ) {
				fu = pFab.Services.Traversal.GetActiveUser.Get().FirstDataItem();
				fu.Uri = null;
				fu.FabType = null;

				var cp = new CacheItemPolicy();
				cp.SlidingExpiration = new TimeSpan(2, 0, 0);

				ActiveUsers.Add(pFab.PersonSession.SessionId, fu, cp);
			}

			return fu;
		}


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
			//Log.Debug("FabricService: SKIPPING TASKS FOR NOW");
			//return;

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
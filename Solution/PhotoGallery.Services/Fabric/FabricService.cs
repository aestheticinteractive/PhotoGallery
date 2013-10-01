using System;
using System.Runtime.Caching;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;

namespace PhotoGallery.Services.Fabric {
	
	/*================================================================================================*/
	public static class FabricService {

		private static readonly MemoryCache ActiveUsers = new MemoryCache("ActiveUsers");


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

			if ( fu != null ) {
				return fu;
			}

			fu = pFab.Services.Traversal.GetActiveUser.Get().FirstDataItem();

			if ( fu == null ) {
				return null;
			}

			fu.Uri = null;
			fu.FabType = null;

			var cp = new CacheItemPolicy();
			cp.SlidingExpiration = new TimeSpan(2, 0, 0);

			ActiveUsers.Add(pFab.PersonSession.SessionId, fu, cp);
			return fu;
		}

	}

}
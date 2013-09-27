using System;
using System.Collections.Generic;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Daemon {

	/*================================================================================================*/
	public class Queries {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricPersonSession> FindUpdatableSessions(ISession pSess) {
			return pSess.QueryOver<FabricPersonSession>()
				.Where(x => x.TryUpdate)
				.List();
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void TurnOffSessionUpdate(ISession pSess, FabricPersonSession pPersonSess) {
			pPersonSess.TryUpdate = false;
			pSess.Save(pPersonSess);
			Log.Debug("TurnOffSessionUpdate: "+pPersonSess.SessionId);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricPersonSession> FindExpiredSessions(ISession pSess) {
			return pSess.QueryOver<FabricPersonSession>()
				.Where(x => x.Expiration <= DateTime.UtcNow.Ticks)
				.List();
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void DeleteSession(ISession pSess, FabricPersonSession pPersonSess) {
			pSess.Delete(pPersonSess);
			Log.Debug("DeleteSession: "+pPersonSess.SessionId);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricArtifact> GetFabricArtifacts(ISession pSess, FabricUser pUser=null) {
			var q = pSess.QueryOver<FabricArtifact>()
				.Where(x => x.ArtifactId == null);

			if ( pUser == null ) {
				q = q.Where(x => x.Creator == null);
			}
			else {
				q = q.Where(x => x.Creator.Id == pUser.Id);
			}

			return q.Take(10).List();
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricFactor> GetFabricFactors(ISession pSess, FabricUser pUser=null) {
			var q = pSess.QueryOver<FabricFactor>()
				.Where(x => x.FactorId == null);

			if ( pUser == null ) {
				q = q.Where(x => x.Creator == null);
			}
			else {
				q = q.Where(x => x.Creator.Id == pUser.Id);
			}

			return q
				.Fetch(x => x.Primary).Eager
				.Fetch(x => x.Related).Eager
				.Take(20)
				.List();
		}

	}

}
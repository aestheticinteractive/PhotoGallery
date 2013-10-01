using System;
using System.Collections.Generic;
using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;

namespace PhotoGallery.Daemon.Export {

	/*================================================================================================*/
	public class Queries {

		private readonly ISessionProvider vSessProv;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Queries(ISessionProvider pSessProv) {
			vSessProv = pSessProv;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricPersonSession> FindUpdatableSessions() {
			using ( ISession sess = vSessProv.OpenSession() ) {
				IList<FabricPersonSession> list = sess.QueryOver<FabricPersonSession>()
					.Where(x => x.TryUpdate)
					.List();

				foreach ( FabricPersonSession fps in list ) {
					Log.Debug("FindUpdatableSessions: "+fps.SessionId);
					fps.TryUpdate = false;
				}

				sess.Flush(); //don't need to call sess.Update()
				return list;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricPersonSession> FindExpiredSessions() {
			using ( ISession sess = vSessProv.OpenSession() ) {
				return sess.QueryOver<FabricPersonSession>()
					.Where(x => x.Expiration <= DateTime.UtcNow.Ticks)
					.List();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void DeleteSession(FabricPersonSession pPersonSess) {
			using ( ISession sess = vSessProv.OpenSession() ) {
				sess.Delete(pPersonSess);
				Log.Debug("DeleteSession: "+pPersonSess.SessionId);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual FabricUser GetCurrentUser(IFabricClient pFabClient) {
			using ( ISession sess = vSessProv.OpenSession() ) {
				return HomeService.GetCurrentUser(pFabClient, sess);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricArtifact> GetFabricArtifacts(int pCount, FabricUser pUser=null) {
			using ( ISession sess = vSessProv.OpenSession() ) {
				var q = sess.QueryOver<FabricArtifact>()
					.Where(x => x.ArtifactId == null);

				if ( pUser == null ) {
					q = q.Where(x => x.Creator == null);
				}
				else {
					q = q.Where(x => x.Creator.Id == pUser.Id);
				}

				return q.Take(pCount).List();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual IList<FabricFactor> GetFabricFactors(int pCount, FabricUser pUser=null) {
			using ( ISession sess = vSessProv.OpenSession() ) {
				var q = sess.QueryOver<FabricFactor>()
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
					.Take(pCount)
					.List();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void UpdateObjects(IList<object> pObjects) {
			using ( ISession sess = vSessProv.OpenSession() ) {
				using ( ITransaction tx = sess.BeginTransaction() ) {
					foreach ( object o in pObjects ) {
						sess.Update(o);
					}

					tx.Commit();
				}
			}
		}

	}

}
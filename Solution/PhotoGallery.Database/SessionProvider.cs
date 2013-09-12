using System.Diagnostics;
using NHibernate;
using NHibernate.SqlCommand;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Database {

	/*================================================================================================*/
	public class SessionProvider : EmptyInterceptor, ISessionProvider {

		public bool OutputSql { get; set; }

		private Stopwatch vTimer;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public ISession OpenSession() {
			return Connect.SessionFactory.OpenSession(this);
		}

		/*--------------------------------------------------------------------------------------------*/
		public override SqlString OnPrepareStatement(SqlString pSql) {
			if ( OutputSql ) {
				Log.Debug("...... "+pSql);
			}

			return base.OnPrepareStatement(pSql);
		}

	}

}
using System.Collections.Generic;
using System.Linq;
using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Logic.Main {
	
	/*================================================================================================*/
	public class HomeLogic : BaseLogic {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeLogic(IFabricClient pFab) : base(pFab) {}

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<string> GetAlbumNames() {
			var prov = new SessionProvider();
			prov.OutputSql = true;
			
			IList<Album> albums;
			Log.Debug("GET ALBUM NAMES:");

			using ( ISession s = prov.OpenSession() ) {
				s.Get<Album>(1);
				albums = s.QueryOver<Album>().List();
				Log.Debug(" - AFTER: "+albums.Count);
			}

			return albums.Select(a => a.Title).ToList();
		}

	}

}
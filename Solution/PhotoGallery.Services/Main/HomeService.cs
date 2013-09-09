using System.Collections.Generic;
using System.Linq;
using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;

namespace PhotoGallery.Logic.Main {
	
	/*================================================================================================*/
	public class HomeService : BaseLogic {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeService(IFabricClient pFab) : base(pFab) {}

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<string> GetAlbumNames() {
			var prov = new SessionProvider();
			prov.OutputSql = true;
			
			IList<Album> albums;

			using ( ISession s = prov.OpenSession() ) {
				albums = s.QueryOver<Album>().List();
			}

			return albums.Select(a => a.Title).ToList();
		}

	}

}
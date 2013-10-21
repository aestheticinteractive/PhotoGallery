using System.Collections.Generic;
using System.Linq;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using NHibernate.SqlCommand;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class AlbumsService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumsService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebAlbumTag> GetTagCounts(int pAlbumId) {
			using ( ISession s = NewSession() ) {
				FabricArtifact artAlias = null;
				FabricFactor facAlias = null;

				IList<object[]> list = s.QueryOver<Photo>()
					.Where(x => x.Album.Id == pAlbumId)
					.JoinQueryOver(x => x.FabricArtifact, () => artAlias, JoinType.InnerJoin)
					.JoinQueryOver(x => x.PrimaryFactors, () => facAlias, JoinType.InnerJoin)
					.Where(x => x.DesTypeRefineId == (long)LiveArtifactId.Depict)
					.Where(x => x.DesTypeId == (byte)FabEnumsData.DescriptorTypeId.RefersTo)
					.SelectList(sl => sl
						.Select(() => facAlias.Related.Id)
						.Select(x => x.Id)
					)
					.List<object[]>();

				var artMap = new Dictionary<int, IList<int>>();

				foreach ( object[] vals in list ) {
					int artId = (int)vals[0];

					if ( !artMap.ContainsKey(artId) ) {
						artMap.Add(artId, new List<int>());
					}

					artMap[artId].Add((int)vals[1]);
				}

				var tags = new List<WebAlbumTag>();
				int i = 0;
				const int n = 20;

				while ( true ) {
					IEnumerable<int> ids = artMap.Keys.Skip(i).Take(n);
					i += n;

					IList<FabricArtifact> arts = s.QueryOver<FabricArtifact>()
						.WhereRestrictionOn(x => x.Id).IsInG(ids)
						.List();

					if ( arts.Count == 0 ) {
						break;
					}

					tags.AddRange(arts.Select(art => new WebAlbumTag(art, artMap[art.Id])));
				}

				return tags.OrderBy(x => -x.PhotoIds.Count).ToList();
			}
		}

	}

}
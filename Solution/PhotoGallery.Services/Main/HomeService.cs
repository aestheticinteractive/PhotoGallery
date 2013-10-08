using System.Collections.Generic;
using System.Linq;
using Fabric.Clients.Cs;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using PhotoGallery.Domain;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class HomeService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebAlbum> GetAlbums(int pOffset, int pLimit) {
			using ( ISession s = NewSession() ) {
				Photo phoAlias = null;

				return GetAlbumQuery(s)
					.OrderBy(Projections.Max(() => phoAlias.Date)).Desc
					.Skip(pOffset)
					.Take(pLimit == 0 ? 9999 : pLimit)
					.List<WebAlbum>();
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static IQueryOver<Album, Album> GetAlbumQuery(ISession pSession) {
			Album albAlias = null;
			Photo phoAlias = null;
			FabricUser userAlias = null;
			WebAlbum dto = null;

			return pSession.QueryOver<Album>(() => albAlias)
				.JoinAlias(a => a.Photos, () => phoAlias, JoinType.InnerJoin)
				.JoinAlias(a => a.FabricUser, () => userAlias, JoinType.InnerJoin)
				.SelectList(list => list
					.SelectGroup(a => a.Id).WithAlias(() => dto.AlbumId)
					.SelectMin(a => a.Title).WithAlias(() => dto.Title)
					.SelectMin(() => userAlias.Id).WithAlias(() => dto.UserId)
					.SelectMin(() => userAlias.Name).WithAlias(() => dto.UserName)
					.SelectCount(() => phoAlias.Id).WithAlias(() => dto.NumPhotos)
					.SelectMin(() => phoAlias.Id).WithAlias(() => dto.FirstPhotoId)
					.SelectMin(() => phoAlias.Date).WithAlias(() => dto.StartDateTicks)
					.SelectMax(() => phoAlias.Date).WithAlias(() => dto.EndDateTicks)
				)
				.TransformUsing(Transformers.AliasToBean<WebAlbum>());
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public int GetAlbumCount() {
			using ( ISession s = NewSession() ) {
				return s.QueryOver<Album>().RowCount();
			};
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbum GetAlbum(int pAlbumId) {
			using ( ISession s = NewSession() ) {
				return GetAlbumQuery(s)
					.Where(x => x.Id == pAlbumId)
					.Take(1)
					.List<WebAlbum>()
					.FirstOrDefault(); //linq
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoSet GetAlbumPhotoSet(WebAlbum pAlbum) {
			int aid = pAlbum.AlbumId;

			var wps = new WebPhotoSet((skip, take) => {
				using ( ISession s = NewSession() ) {
					IList<Photo> photos = s.QueryOver<Photo>()
						.Where(x => x.Album.Id == aid)
						.Skip(skip)
						.Take(take)
						.List();

					return photos.Select(p => new WebPhoto(p)).Cast<IWebPhoto>().ToList();
				}
			});

			wps.Title = pAlbum.Title;
			return wps;
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebAlbumStats GetAlbumStats(int pAlbumId) {
			using ( ISession s = NewSession() ) {
				WebAlbumStats dto = null;

				return s.QueryOver<Photo>()
					.Where(x => x.Album.Id == pAlbumId)
					.SelectList(list => list
						.SelectGroup(x => x.Album.Id)
						.SelectCount(x => x.Id).WithAlias(() => dto.PhotoCount)
						.SelectAvg(x => x.FNum).WithAlias(() => dto.AvgFNum)
						.SelectAvg(x => x.Iso).WithAlias(() => dto.AvgIso)
						.SelectAvg(x => x.ExpTime).WithAlias(() => dto.AvgExpTime)
						.SelectAvg(x => x.FocalLen).WithAlias(() => dto.AvgFocalLen)
						.SelectSubQuery(
							QueryOver.Of<Photo>()
							.Where(x => x.Album.Id == pAlbumId && x.Flash == true)
							.ToRowCountQuery()
						).WithAlias(() => dto.FlashCount)

					)
					.TransformUsing(Transformers.AliasToBean<WebAlbumStats>())
					.List<WebAlbumStats>()
					.FirstOrDefault(); //linq
			};
		}

	}

}
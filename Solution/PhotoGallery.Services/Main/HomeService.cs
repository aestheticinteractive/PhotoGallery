﻿using System.Collections.Generic;
using Fabric.Clients.Cs;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using PhotoGallery.Domain;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class HomeService : BaseLogic {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebAlbum> GetAlbums(int pLimit) {
			using ( ISession s = NewSession() ) {
				Photo phoAlias = null;

				return GetAlbumQuery(s)
					.OrderBy(Projections.Max(() => phoAlias.Date)).Desc
					.Take(pLimit == 0 ? 9999 : pLimit)
					.List<WebAlbum>();
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static IQueryOver<Album, Album> GetAlbumQuery(ISession pSession) {
			Album albAlias = null;
			Photo phoAlias = null;
			WebAlbum dto = null;

			return pSession.QueryOver<Album>(() => albAlias)
				.JoinAlias(a => a.Photos, () => phoAlias, JoinType.InnerJoin)
				.SelectList(list => list
					.SelectGroup(a => a.Id).WithAlias(() => dto.AlbumId)
					.SelectMin(a => a.Title).WithAlias(() => dto.Title)
					.SelectCount(() => phoAlias.Id).WithAlias(() => dto.NumPhotos)
					.SelectMin(() => phoAlias.Id).WithAlias(() => dto.FirstPhotoId)
					.SelectMin(() => phoAlias.Date).WithAlias(() => dto.StartDateTicks)
					.SelectMax(() => phoAlias.Date).WithAlias(() => dto.EndDateTicks)
				)
				.TransformUsing(Transformers.AliasToBean<WebAlbum>());
		}

	}

}
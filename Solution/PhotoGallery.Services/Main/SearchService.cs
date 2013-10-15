using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class SearchService : BaseService {

		private static readonly MemoryCache TagCache = new MemoryCache("Tag");


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public SearchService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebSearchTag> FindTags(string pName, bool pFirst) {
			WebSearchTagState state;

			if ( TagCache.Contains(pName) ) {
				state = (WebSearchTagState)TagCache[pName];

				if ( pFirst ) {
					return state.List;
				}
			}
			else {
				state = new WebSearchTagState(pName);
				state.SetMode(WebSearchTagState.Mode.Local, 0, 20);

				var pol = new CacheItemPolicy();
				pol.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddHours(1));
				TagCache.Add(pName, state, pol);
			}

			switch ( state.SearchMode ) {
				case WebSearchTagState.Mode.Local:
					FindLocalTags(state);
					break;

				case WebSearchTagState.Mode.Class:
					FindFabClass(state);
					break;

				case WebSearchTagState.Mode.ClassContains:
					FindFabClassContains(state);
					break;

				case WebSearchTagState.Mode.InstanceContains:
					FindFabInstanceContains(state);
					break;

				case WebSearchTagState.Mode.Done:
					return new List<WebSearchTag>();
			}

			if ( state.List.Count >= 50 ) {
				state.SetMode(WebSearchTagState.Mode.Done, 0, 0);
			}

			if ( state.LatestList.Count == 0 ) {
				return (state.SearchMode == WebSearchTagState.Mode.Done ?
					new List<WebSearchTag>() : FindTags(pName, false));
			}

			return state.LatestList;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void FindLocalTags(WebSearchTagState pState) {
			using ( ISession s = NewSession() ) {
				IList<Tag> tags = s.QueryOver<Tag>()
					.WhereRestrictionOn(x => x.Name).IsInsensitiveLike("%"+pState.Name+"%")
					.Fetch(x => x.FabricArtifact).Eager
					.Skip(pState.SearchIndex)
					.Take(pState.SearchSize)
					.List();

				var webTags = new List<WebSearchTag>();

				foreach ( Tag t in tags ) {
					if ( t.FabricArtifact.ArtifactId != null ) {
						webTags.Add(new WebSearchTag(t));
					}
				}

				pState.AddToList(webTags);
				
				if ( pState.LatestList.Count == 0 ) {
					pState.SetMode(WebSearchTagState.Mode.Class, 0, 10);
				}
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		private void FindFabClass(WebSearchTagState pState) {
			FabResponse<FabClass> fr = Fab.Services.Traversal.GetRootStep
				.ClassName(pState.Name).Limit(pState.SearchIndex, pState.SearchSize).Get();

			if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.ClassContains, 0, 10);
				return;
			}

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
		}

		/*--------------------------------------------------------------------------------------------*/
		private void FindFabClassContains(WebSearchTagState pState) {
			FabResponse<FabClass> fr = Fab.Services.Traversal.GetRootStep
				.ClassNameContains(pState.Name).Limit(pState.SearchIndex, pState.SearchSize).Get();

			if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.InstanceContains, 0, 10);
				return;
			}

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
		}

		/*--------------------------------------------------------------------------------------------*/
		private void FindFabInstanceContains(WebSearchTagState pState) {
			FabResponse<FabInstance> fr = Fab.Services.Traversal.GetRootStep
				.InstanceNameContains(pState.Name).Limit(pState.SearchIndex, pState.SearchSize).Get();

			if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.Done, 0, 0);
				return;
			}

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
		}

		/*--------------------------------------------------------------------------------------------*/
		private bool IsEmptyFabResponse<T>(FabResponse<T> pResp, WebSearchTagState pState) {
			if ( pResp == null || pResp.Data == null || pResp.Data.Count == 0 ) {
				pState.AddToList(new List<WebSearchTag>());
				return true;
			}

			return false;
		}

	}

}
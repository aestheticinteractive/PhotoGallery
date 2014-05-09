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
			string name = pName.ToLower();

			if ( TagCache.Contains(name) ) {
				state = (WebSearchTagState)TagCache[name];

				if ( pFirst ) {
					return state.List;
				}
			}
			else {
				state = new WebSearchTagState(name);
				state.SetMode(WebSearchTagState.Mode.Local, 0, 20);

				var pol = new CacheItemPolicy();
				pol.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddHours(1));
				TagCache.Add(name, state, pol);
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
					new List<WebSearchTag>() : FindTags(name, false));
			}

			return state.LatestList;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void FindLocalTags(WebSearchTagState pState) {
			using ( ISession s = NewSession() ) {
				IList<Tag> tags = s.QueryOver<Tag>()
					.WhereRestrictionOn(x => x.Name).IsInsensitiveLike("%"+pState.Name+"%")
					.Fetch(x => x.FabricArtifact).Eager
					//.Skip(pState.SearchIndex)
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
			FabResponse<FabClass> fr = Fab.Services.Traversal.Classes
				.WithName(pState.Name).Take(pState.SearchSize).Get();

			/*if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.ClassContains, 0, 10);
				return;
			}*/

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
			pState.SetMode(WebSearchTagState.Mode.ClassContains, 0, 10);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void FindFabClassContains(WebSearchTagState pState) {
			FabResponse<FabClass> fr = Fab.Services.Traversal.Classes
				.WhereNameContains(pState.Name).Take(pState.SearchSize).Get();

			/*if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.InstanceContains, 0, 10);
				return;
			}*/

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
			pState.SetMode(WebSearchTagState.Mode.InstanceContains, 0, 10);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void FindFabInstanceContains(WebSearchTagState pState) {
			FabResponse<FabInstance> fr = Fab.Services.Traversal.Instances
				.WhereNameContains(pState.Name).Take(pState.SearchSize).Get();

			/*if ( IsEmptyFabResponse(fr, pState) ) {
				pState.SetMode(WebSearchTagState.Mode.Done, 0, 0);
				return;
			}*/

			pState.AddToList(fr.Data.Select(x => new WebSearchTag(x)));
			pState.SetMode(WebSearchTagState.Mode.Done, 0, 10);
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
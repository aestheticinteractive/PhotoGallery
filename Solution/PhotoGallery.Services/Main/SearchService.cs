using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Services.Main {
	
	/*================================================================================================*/
	public class SearchService : BaseService {

		private static readonly MemoryCache TagLocalCache = new MemoryCache("TagLocal");
		private static readonly MemoryCache TagFabCache = new MemoryCache("TagFab");


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public SearchService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebSearchTag> FindLocalTags(string pName) {
			if ( TagLocalCache.Contains(pName) ) {
				return (IList<WebSearchTag>)TagLocalCache[pName];
			}

			using ( ISession s = NewSession() ) {
				IList<Tag> tags = s.QueryOver<Tag>()
					.WhereRestrictionOn(x => x.Name).IsInsensitiveLike("%"+pName+"%")
					.Fetch(x => x.FabricArtifact).Eager
					.List();

				var webTags = new List<WebSearchTag>();

				foreach ( Tag t in tags ) {
					if ( t.FabricArtifact.ArtifactId != null ) {
						webTags.Add(new WebSearchTag(t));
					}
				}

				var pol = new CacheItemPolicy();
				pol.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddHours(1));
				TagLocalCache.Add(pName, webTags, pol);

				return webTags;
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<WebSearchTag> FindFabricTags(string pName) {
			if ( TagFabCache.Contains(pName) ) {
				return (IList<WebSearchTag>)TagFabCache[pName];
			}

			Fab.UseDataProviderPerson = true;

			IList<FabClass> classNames = null;
			IList<FabClass> classConts = null;
			IList<FabInstance> instConts = null;

			Parallel.Invoke(new Action[] {
				() => {
					classNames = GetFabList((r, i, n) => r.ClassName(pName).Limit(i, n).Get());
				},
				() => {
					classConts = GetFabList((r, i, n) => r.ClassNameContains(pName).Limit(i, n).Get());
				},
				() => {
					instConts = GetFabList((r,i,n) => r.InstanceNameContains(pName).Limit(i, n).Get());
				}
			});

			var webTags = new List<WebSearchTag>();
			webTags.AddRange(classNames.Select(x => new WebSearchTag(x)));
			webTags.AddRange(classConts.Select(x => new WebSearchTag(x)));
			webTags.AddRange(instConts.Select(x => new WebSearchTag(x)));

			webTags = webTags.GroupBy(x => x.ArtifactId).Select(x => x.First()).ToList(); //de-dup

			var pol = new CacheItemPolicy();
			pol.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddHours(1));
			TagFabCache.Add(pName, webTags, pol);

			return webTags;
		}

		/*--------------------------------------------------------------------------------------------*/
		private IList<T> GetFabList<T>(Func<IFabRootStep, int, int, FabResponse<T>> pFunc) 
																				where T : FabVertex {
			IFabRootStep root = Fab.Services.Traversal.GetRootStep;
			var list = new List<T>();
			int i = 0;
			const int n = 20;
					
			while ( true ) {
				FabResponse<T> resp = pFunc(root, i, n);

				if ( resp == null || resp.Data == null || resp.Data.Count == 0 ) {
					break;
				}

				list.AddRange(resp.Data);
				i += n;

				if ( !resp.HasMore || i >= 100 ) {
					break;
				}
			}

			return list;
		}

	}

}
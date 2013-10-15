using System.Collections.Generic;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebSearchTagState {

		public enum Mode {
			Local = 1,
			Class,
			ClassContains,
			InstanceContains,
			Done
		}

		public string Name { get; private set; }
		public Mode SearchMode { get; private set; }
		public int SearchIndex { get; private set; }
		public int SearchSize { get; private set; }
		public IList<WebSearchTag> List { get; private set; }
		public IList<WebSearchTag> LatestList { get; private set; }
		public HashSet<long> ArtifactIdMap { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebSearchTagState(string pName) {
			Name = pName;
			SearchMode = Mode.Local;
			SearchIndex = 0;
			SearchSize = 20;
			List = new List<WebSearchTag>();
			ArtifactIdMap = new HashSet<long>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void SetMode(Mode pMode, int pIndex, int pSize) {
			SearchMode = pMode;
			SearchIndex = pIndex;
			SearchSize = pSize;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddToList(IEnumerable<WebSearchTag> pList) {
			SearchIndex += SearchSize;
			LatestList = new List<WebSearchTag>();

			foreach ( WebSearchTag st in pList ) {
				if ( ArtifactIdMap.Contains(st.ArtifactId) ) {
					continue;
				}

				List.Add(st);
				LatestList.Add(st);
				ArtifactIdMap.Add(st.ArtifactId);
			}

			//Log.Debug("State.AddToList: n="+Name+", m="+SearchMode+", i="+(SearchIndex-SearchSize)+
			//	", l="+List.Count+", ll="+LatestList.Count);
		}

	}

}
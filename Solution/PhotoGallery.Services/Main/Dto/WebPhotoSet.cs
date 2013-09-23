using System;
using System.Collections.Generic;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhotoSet {

		public string Title { get; set; }

		public int Index { get; private set; }
		public int PageSize { get; private set; }
		public int MaxIndex { get; private set; }

		private readonly Func<int, int, IList<IWebPhoto>> vPhotoProv;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoSet(Func<int, int, IList<IWebPhoto>> pPhotoProvider) {
			PageSize = 20;
			vPhotoProv = pPhotoProvider;
			MaxIndex = -1;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void RestartWithPageSize(int pPageSize) {
			PageSize = pPageSize;
			Index = 0;
			MaxIndex = -1;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<IWebPhoto> GetCurrentPage() {
			return vPhotoProv(Index, PageSize);
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<IWebPhoto> GetAll() {
			return vPhotoProv(0, 9999);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool CanGetPreviousPage() {
			return (Index > 0);
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool CanGetNextPage() {
			return (Index < MaxIndex);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public IList<IWebPhoto> GetPreviousPage() {
			if ( !CanGetPreviousPage() ) {
				return null;
			}

			Index -= PageSize;
			return vPhotoProv(Index, PageSize);
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<IWebPhoto> GetNextPage() {
			if ( !CanGetNextPage() ) {
				return null;
			}

			Index += PageSize;

			IList<IWebPhoto> photos = vPhotoProv(Index, PageSize+1);

			if ( photos.Count == PageSize+1 ) {
				photos.RemoveAt(PageSize);
			}
			else {
				MaxIndex = Index;
			}

			return photos;
		}

	}

}
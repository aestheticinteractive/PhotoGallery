using System.Collections.Generic;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class PhotoModel : MainBaseModel {

		public int PhotoId { get; private set; }
		public IList<IWebPhoto> Photos { get; private set; }
		public int SelectedPhotoIndex { get; private set; }
		public string PhotoSetTitle { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoModel(int pPhotoId, WebPhotoSet pPhotoSet) {
			PhotoId = pPhotoId;
			Photos = pPhotoSet.GetCurrentPage();
			PhotoSetTitle = pPhotoSet.Title+" Photos";

			for ( int i = 0 ; i < Photos.Count ; i++ ) {
				if ( Photos[i].PhotoId == pPhotoId ) {
					SelectedPhotoIndex = i;
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle {
			get {
				return PhotoSetTitle;
			}
		}

	}

}
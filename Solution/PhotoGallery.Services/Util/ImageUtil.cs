using System;

namespace PhotoGallery.Services.Util {

	/*================================================================================================*/
	public static class ImageUtil {

		public enum PhotoSize {
			Large = 1,
			Thumb
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string BuildImageUrl(int pAlbumId, int pPhotoId, PhotoSize pSize) {
			return "http://www.zachkinstner.com/gallery"+BuildPhotoPath(pAlbumId, pPhotoId, pSize);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string BuildPhotoPath(int pAlbumId) {
			return "/upload/photos/"+pAlbumId;
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string BuildPhotoPath(int pAlbumId, int pPhotoId, PhotoSize pSize) {
			string s = "";

			if ( pSize == PhotoSize.Thumb ) {
				s = "-thumb";
			}

			return BuildPhotoPath(pAlbumId)+"/"+pPhotoId+s+".jpg";
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static T NumToEnum<T>(int pNumber) {
			return (T)Enum.ToObject(typeof(T), pNumber);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static DateTime ParseMetaDate(string pString) {
			pString = pString.Substring(0, 4)+"-"+pString.Substring(5, 2)+"-"+pString.Substring(8);
			return DateTime.Parse(pString);
		}

	}

}
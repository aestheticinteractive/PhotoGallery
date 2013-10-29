using System;
using PhotoGallery.Domain;
using PhotoGallery.Services.Util;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhoto : IWebPhoto {
		
		public int PhotoId { get; internal set; }
		public string ImgName { get; internal set; }
		public int AlbumId { get; internal set; }
		public float Ratio { get; internal set; }
		public DateTime Taken { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto() {}

		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto(Photo p) {
			PhotoId = p.Id;
			ImgName = p.ImgName;
			AlbumId = p.Album.Id;
			Ratio = p.Ratio;

			DateTime date = (p.Date == null ? DateTime.UtcNow : new DateTime((long)p.Date));
#if DEBUG
			const string estName = "Eastern Standard Time"; //Windows
#else
			const string estName = "US/Eastern"; //Unix
#endif
			TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById(estName);
			Taken = TimeZoneInfo.ConvertTimeFromUtc(date, est);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string ImageUrl {
			get {
				return GetUrl(ImageUtil.PhotoSize.Large);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public string ThumbUrl {
			get {
				return GetUrl(ImageUtil.PhotoSize.Thumb);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private string GetUrl(ImageUtil.PhotoSize pSize) {
			return ImageUtil.BuildPhotoPath(AlbumId, PhotoId, pSize);
		}

	}

}
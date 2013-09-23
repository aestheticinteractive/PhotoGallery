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
		public DateTime Created { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto() {}

		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto(Photo p) {
			PhotoId = p.Id;
			ImgName = p.ImgName;
			AlbumId = p.Album.Id;
			Ratio = p.Ratio;
			Created = new DateTime(p.Created);
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
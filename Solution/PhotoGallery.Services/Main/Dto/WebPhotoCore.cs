﻿using System;
using PhotoGallery.Domain;
using PhotoGallery.Services.Util;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhotoCore {
		
		public int PhotoId { get; internal set; }
		public string ImgName { get; internal set; }
		public int Favorite { get; internal set; }
		public int AlbumId { get; internal set; }
		public DateTime ExifDtOrig { get; internal set; }
		public int FabricArtifactId { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoCore() {}

		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoCore(Photo p) {
			PhotoId = p.Id;
			ImgName = p.ImgName;
			Favorite = p.Favorite;
			AlbumId = p.Album.Id;
			ExifDtOrig = p.ExifDTOrig;
			FabricArtifactId = p.FabricArtifactId;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string FullUrl { get { return GetUrl(ImageUtil.PhotoSize.Full); } }
		public string ImageUrl { get { return GetUrl(ImageUtil.PhotoSize.Med); } }
		public string ThumbUrl { get { return GetUrl(ImageUtil.PhotoSize.Sm); } }

		/*--------------------------------------------------------------------------------------------*/
		private string GetUrl(ImageUtil.PhotoSize pSize) {
			return ImageUtil.BuildImageUrl(AlbumId, PhotoId, pSize);
		}

	}

}
using System;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public interface IWebPhoto {
		
		int PhotoId { get; }
		string ImgName { get; }
		int AlbumId { get; }
		float Ratio { get; }
		DateTime Taken { get; }

		string ImageUrl { get; }
		string ThumbUrl { get; }

	}

}
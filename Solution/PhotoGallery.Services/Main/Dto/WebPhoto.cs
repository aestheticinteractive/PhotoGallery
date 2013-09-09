using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhoto : WebPhotoCore {
		
		public string AlbumName { get; internal set; }

		public double ExifExposureTime { get; internal set; }
		public double ExifIsoSpeed { get; internal set; }
		public double ExifFNumber { get; internal set; }
		public double ExifFocalLength { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto() {}

		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto(Photo p) : base(p) {
			AlbumName = p.Album.Title;

			ExifExposureTime = p.ExifExposureTime;
			ExifIsoSpeed = p.ExifISOSpeed;
			ExifFNumber = p.ExifFNumber;
			ExifFocalLength = p.ExifFocalLength;
		}

	}

}
using System;
using System.Collections.Generic;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebAlbum : WebAlbumCore {

		public int Index { get; internal set; }
		public int NumPhotos { get; internal set; }
		public int NumFavs { get; internal set; }
		public int FirstPhotoId { get; internal set; }
		public DateTime StartDate { get; internal set; }
		public DateTime EndDate { get; internal set; }
		public IList<WebAlbumTag> Tags { get; internal set; }
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbum() {}
		public WebAlbum(Album pAlbum) : base(pAlbum) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhoto FirstPhoto {
			get {
				var p = new WebPhoto();
				p.AlbumId = AlbumId;
				p.PhotoId = FirstPhotoId;
				p.ImgName = Title;
				return p;
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string DateRangeString {
			get {
				string start = Convert.ToDateTime(StartDate).ToString("MMM d");
				string end = Convert.ToDateTime(EndDate).ToString("MMM d, yyyy");
				return start+" - "+end;
			}
		}

	}

}
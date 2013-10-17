namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class PhotoAddTagModel {

		//use string for ArtifactId because Javascript chokes on 64-bit integers

		public int PhotoId { get; set; }
		public string ArtifactId { get; set; }
		public double PosX { get; set; }
		public double PosY { get; set; }

	}

}
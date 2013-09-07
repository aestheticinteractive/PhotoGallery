namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class PhotoTag {

		public virtual int Id { get; set; }
		public virtual Photo Photo { get; set; }
		public virtual Tag Tag { get; set; }

    }

}

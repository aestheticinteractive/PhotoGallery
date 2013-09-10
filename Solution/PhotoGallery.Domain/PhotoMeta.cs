namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class PhotoMeta {

		public virtual int Id { get; set; }
		public virtual string Label { get; set; }
		public virtual string Type { get; set; }
		public virtual string Value { get; set; }

		public virtual Photo Photo { get; set; }

    }

}

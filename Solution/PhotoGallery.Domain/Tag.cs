namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Tag {

		public enum TagType {
			Unspecified = 0,
			Person,
			CameraMake,
			CameraModel,
			Pet
		};

		public virtual int Id { get; set; }
		public virtual byte Type { get; set; }
		public virtual string Name { get; set; }

		public virtual FabricArtifact FabricArtifact { get; set; }

    }

}

﻿namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Tag {

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Disamb { get; set; }
		public virtual string Note { get; set; }

		public virtual FabricArtifact FabricArtifact { get; set; }

    }

}

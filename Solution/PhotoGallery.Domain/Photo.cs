using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Photo {

		public virtual int Id { get; set; }
		public virtual string ImgName { get; set; }
		public virtual float Ratio { get; set; }

		public virtual FabricArtifact FabricArtifact { get; set; }
		public virtual Album Album { get; set; }
		
		public virtual IList<PhotoMeta> PhotoMetas { get; set; }

    }

}

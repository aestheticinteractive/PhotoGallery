using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Photo {

		public virtual int Id { get; set; }
		public virtual string ImgName { get; set; }
		public virtual long Width { get; set; }
		public virtual long Height { get; set; }
		public virtual float Ratio { get; set; }
		public virtual long Created { get; set; }

		public virtual string Make { get; set; }
		public virtual string Model { get; set; }

		public virtual long? FNum { get; set; }
		public virtual long? Iso { get; set; }
		public virtual long? Date { get; set; }
		public virtual long? ExpTime { get; set; }
		public virtual long? FocalLen { get; set; }
		public virtual bool? Flash { get; set; }

		public virtual double? GpsLat { get; set; }
		public virtual double? GpsLng { get; set; }
		public virtual double? GpsAlt { get; set; }

		public virtual FabricArtifact FabricArtifact { get; set; }
		public virtual Album Album { get; set; }
		public virtual FabricUser FabricUser { get; set; }
		
		public virtual IList<PhotoMeta> PhotoMetas { get; set; }

    }

}

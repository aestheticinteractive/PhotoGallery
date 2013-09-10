using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Photo {

		public virtual int Id { get; set; }
		public virtual string ImgName { get; set; }
		
		/*public virtual int Favorite { get; set; }
		public virtual DateTime ExifDTOrig { get; set; }
		public virtual double ExifExposureTime { get; set; }
		public virtual double ExifISOSpeed { get; set; }
		public virtual double ExifFNumber { get; set; }
		public virtual double ExifFocalLength { get; set; }*/

		public virtual FabricInstance FabricInstance { get; set; }
		public virtual Album Album { get; set; }
		
		public virtual IList<PhotoMeta> PhotoMetas { get; set; }

    }

}

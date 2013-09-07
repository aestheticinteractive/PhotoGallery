using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Album {

		public virtual int Id { get; set; }
		public virtual string Title { get; set; }
		public virtual string LocalPath { get; set; }
		
		public virtual IList<Photo> Photos { get; set; }

    }

}

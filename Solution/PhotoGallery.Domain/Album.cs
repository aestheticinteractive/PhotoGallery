using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Album {

		public virtual int Id { get; set; }
		public virtual string Title { get; set; }

		public virtual FabricUser FabricUser { get; set; }
		public virtual FabricInstance FabricInstance { get; set; }
		
		public virtual IList<Photo> Photos { get; set; }

    }

}

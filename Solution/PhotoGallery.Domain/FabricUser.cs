using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class FabricUser {

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual long Created { get; set; }

		public virtual FabricInstance FabricInstance { get; set; }

		public virtual IList<Album> Albums { get; set; }

    }

}

using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class FabricArtifact {

		public virtual int Id { get; set; }
		public virtual long ArtifactId { get; set; }
		public virtual byte Type { get; set; }
		
		public virtual IList<Album> Albums { get; set; } //0 or 1
		public virtual IList<Photo> Photos { get; set; } //0 or 1
		public virtual IList<Human> Humans { get; set; } //0 or 1
		public virtual IList<FabricUser> FabricUsers { get; set; } //0 or 1

		public virtual IList<FabricFactor> PrimaryFactors { get; set; }
		public virtual IList<FabricFactor> RelatedFactors { get; set; }

    }

}

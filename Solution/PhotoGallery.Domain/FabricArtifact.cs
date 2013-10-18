using System.Collections.Generic;

namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class FabricArtifact {

		public enum ArtifactType {
			FabUser = 1,
			Album,
			Photo,
			Tag,
			FabClass,
			FabInstance,
			FabApp
		};

		public virtual int Id { get; set; }
		public virtual long? ArtifactId { get; set; }
		public virtual string Name { get; set; }
		public virtual string Disamb { get; set; }
		public virtual string Note { get; set; }
		public virtual byte Type { get; set; }
		public virtual bool FromFab { get; set; }
		public virtual FabricArtifact Creator { get; set; }
		
		public virtual IList<Album> Albums { get; set; } //0 or 1
		public virtual IList<Photo> Photos { get; set; } //0 or 1
		public virtual IList<Tag> Humans { get; set; } //0 or 1
		public virtual IList<FabricUser> FabricUsers { get; set; } //0 or 1

		public virtual IList<FabricFactor> PrimaryFactors { get; set; }
		public virtual IList<FabricFactor> RelatedFactors { get; set; }

    }

}

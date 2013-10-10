using PhotoGallery.Domain;

namespace PhotoGallery.Services.Admin.Dto {

	/*================================================================================================*/
	public class WebPersonTag {

		public int TagId { get; internal set; }
		public string Name { get; internal set; }
		public string Disamb { get; internal set; }
		public long? ArtifactId { get; internal set; }

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPersonTag(Tag pTag) {
			TagId = pTag.Id;
			Name = pTag.Name;
			Disamb = pTag.FabricArtifact.Disamb;
			ArtifactId = pTag.FabricArtifact.ArtifactId;
		}

	}

}
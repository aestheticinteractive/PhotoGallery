using System.Collections.Generic;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebAlbumTag {

		public int Id { get; internal set; }
		public string Name { get; internal set; }
		public string Disamb { get; internal set; }
		public string Note { get; internal set; }
		public IList<int> PhotoIds { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbumTag(FabricArtifact pArt, IList<int> pPhotoIds) {
			Id = pArt.Id;
			Name = pArt.Name;
			Disamb = pArt.Disamb;
			Note = pArt.Note;
			PhotoIds = pPhotoIds;
		}

	}

}
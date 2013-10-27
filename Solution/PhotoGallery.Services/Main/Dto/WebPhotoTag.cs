using System;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhotoTag {

		public int PhotoId { get; internal set; }
		public string PhotoArtifactId { get; internal set; }

		public string ArtifactId { get; internal set; }
		public string Name { get; internal set; }
		public string Disamb { get; internal set; }
		public string Note { get; internal set; }
		public double? PosX { get; internal set; }
		public double? PosY { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoTag(Photo pPhoto, FabricFactor pTagFactor) {
			FabricArtifact tagArt = pTagFactor.Related;

			if ( tagArt == null || tagArt.ArtifactId == null ) {
				throw new Exception("FabricFactor or FabricFactor.ArtifactId is null.");
			}

			PhotoId = pPhoto.Id;
			PhotoArtifactId = pPhoto.FabricArtifact.Id+"";

			ArtifactId = tagArt.ArtifactId+"";
			Name = tagArt.Name;
			Disamb = tagArt.Disamb;
			Note = tagArt.Note;
			PosX = pTagFactor.LocValueX;
			PosX = pTagFactor.LocValueY;
		}

	}

}
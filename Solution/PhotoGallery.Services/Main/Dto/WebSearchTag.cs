using System;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebSearchTag {

		//use string for ArtifactId because Javascript chokes on 64-bit integers

		public string ArtifactId { get; internal set; }
		public string Name { get; internal set; }
		public string Disamb { get; internal set; }
		public string Note { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebSearchTag(Tag pTag) {
			FabricArtifact fa = pTag.FabricArtifact;

			if ( fa.ArtifactId == null ) {
				throw new Exception("Null FabricArtifact.ArtifactId.");
			}

			ArtifactId = fa.ArtifactId+"";
			Name = fa.Name;
			Disamb = fa.Disamb;
			Note = fa.Note;
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebSearchTag(FabClass pClass) {
			ArtifactId = pClass.ArtifactId+"";
			Name = pClass.Name;
			Disamb = pClass.Disamb;
			Note = pClass.Note;
		}

		/*--------------------------------------------------------------------------------------------*/
		public WebSearchTag(FabInstance pInst) {
			ArtifactId = pInst.ArtifactId+"";
			Name = pInst.Name;
			Disamb = pInst.Disamb;
			Note = pInst.Note;
		}

	}

}
using System.Collections.Generic;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Domain;

namespace PhotoGallery.Daemon.Export {

	/*================================================================================================*/
	public static class FactorTasks {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static bool FixFactorRefs(FabricFactor pFac, Queries pQueries, IFabricClient pClient) {
			bool update = false;
			var saves = new List<object>();

			if ( pFac.Primary == null && pFac.PrimaryArtifactId != null ) {
				long artId = (long)pFac.PrimaryArtifactId;
				pFac.Primary = GetOrBuildArtifact(artId, pQueries, pClient);
				update = true;

				if ( pFac.Primary != null && pFac.Primary.Id == 0 ) {
					saves.Add(pFac.Primary);
				}
			}

			if ( pFac.Related == null && pFac.RelatedArtifactId != null ) {
				long artId = (long)pFac.RelatedArtifactId;
				pFac.Related = GetOrBuildArtifact(artId, pQueries, pClient);
				update = true;

				if ( pFac.Related != null && pFac.Related.Id == 0 ) {
					saves.Add(pFac.Related);
				}
			}

			pQueries.SaveObjects(saves);
			return update;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static FabricArtifact GetOrBuildArtifact(long pArtId, Queries pQueries,
																				IFabricClient pClient) {
			FabricArtifact fa = pQueries.GetFabricArtifactByArtifactId(pArtId);
			return (fa ?? BuildArtifact(pClient, pArtId));
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private static FabricArtifact BuildArtifact(IFabricClient pClient, long pArtId) {
			ITraversalService trav = pClient.Services.Traversal;

			var art = new FabricArtifact();
			art.ArtifactId = pArtId;

			////

			FabClass fc = trav.Classes.WithId(pArtId).Get().FirstDataItem();

			if ( fc != null ) {
				art.Name = fc.Name;
				art.Disamb = fc.Disamb;
				art.Note = fc.Note;
				art.Type = (byte)FabricArtifact.ArtifactType.FabClass;
				return art;
			}

			////

			FabInstance fi = trav.Instances.WithId(pArtId).Get().FirstDataItem();

			if ( fi != null ) {
				art.Name = fi.Name;
				art.Disamb = fi.Disamb;
				art.Note = fi.Note;
				art.Type = (byte)FabricArtifact.ArtifactType.FabInstance;
				return art;
			}

			return null;
		}

	}

}
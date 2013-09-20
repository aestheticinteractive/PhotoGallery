using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	internal class OneTimeService {

		private static bool IsoSpeedCreated;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		internal static void CreateIsoSpeedClass(IFabricClient pFab) {
			if ( IsoSpeedCreated ) {
				return;
			}

			Log.Debug("CreateIsoSpeedClass...");
			IsoSpeedCreated = true;
			pFab.UseDataProviderPerson = true;

			//Live Fabric Artifact IDs
			const long unitOfMeasurementId = 55429986427863041;
			const long filmId = 55434464925319168;
			const long sensitivityId = 55431229554556929;
			const long measureId = 55438002794528768;

			////

			try {
				FabClass c = pFab.Services.Modify.AddClass.Post("ISO Speed", "film speed",
					"the measure of a photographic film's sensitivity to light").FirstDataItem();
				Log.Debug("ISO Speed: "+c.ArtifactId);
				//after creation: ArtifactId = 56242555254210560

				FabBatchNewFactor f0 = new FabBatchNewFactor();
				f0.BatchId = 0;
				f0.PrimaryArtifactId = c.ArtifactId;
				f0.RelatedArtifactId = unitOfMeasurementId;
				f0.FactorAssertionId = (byte)FabEnumsData.FactorAssertionId.Fact;
				f0.IsDefining = true;
				f0.Note = "ISO Speed is a unit of measurement.";
				f0.Descriptor = new FabBatchNewFactorDescriptor();
				f0.Descriptor.TypeId = (byte)FabEnumsData.DescriptorTypeId.IsA;
				//after creation: FactorId = 56254024255537152

				FabBatchNewFactor f1 = new FabBatchNewFactor();
				f0.BatchId = 1;
				f1.PrimaryArtifactId = c.ArtifactId;
				f1.RelatedArtifactId = filmId;
				f1.FactorAssertionId = (byte)FabEnumsData.FactorAssertionId.Fact;
				f1.IsDefining = true;
				f1.Note = "ISO Speed measures film sensitivity.";
				f1.Descriptor = new FabBatchNewFactorDescriptor();
				f1.Descriptor.TypeId = (byte)FabEnumsData.DescriptorTypeId.RefersTo;
				f1.Descriptor.TypeRefineId = measureId;
				f1.Descriptor.RelatedArtifactRefineId = sensitivityId;
				//after creation: FactorId = 56254024248197120

				////

				var list = new[] { f0, f1 };
				FabResponse<FabBatchResult> res = pFab.Services.Modify.AddFactors.Post(list);

				foreach ( FabBatchResult br in res.Data ) {
					Log.Debug("FactorId #"+br.BatchId+": "+br.ResultId+" / "+br.Error);
				}
			}
			catch ( FabricErrorException fe ) {
				Log.Error("Fabric Error: "+fe.Message, fe);
			}
			finally {
				pFab.UseDataProviderPerson = false;
			}
		}

	}

}
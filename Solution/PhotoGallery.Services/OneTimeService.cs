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
			pFab.UseAppDataProvider = true;

			//Live Fabric Artifact IDs
			const long unitOfMeasurementId = 55429986427863041;
			const long filmId = 55434464925319168;
			const long sensitivityId = 55431229554556929;
			const long measureId = 55438002794528768;

			////

			try {
				var cre = new CreateFabClass();
				cre.Name = "ISO Speed";
				cre.Disamb = "film speed";
				cre.Note = "the measure of a photographic film's sensitivity to light";

				FabClass c = pFab.Services.Modify.Classes.Post(cre).FirstDataItem();
				Log.Debug("ISO Speed: "+c.Id);
				//after creation: ArtifactId = 56242555254210560

				var f0 = new CreateFabFactor();
				f0.UsesPrimaryArtifactId = c.Id;
				f0.UsesRelatedArtifactId = unitOfMeasurementId;
				f0.AssertionType = FactorAssertionId.Fact;
				f0.IsDefining = true;
				f0.Note = "ISO Speed is a unit of measurement.";
				f0.Descriptor = new CreateFabDescriptor();
				f0.Descriptor.Type = DescriptorTypeId.IsA;
				//after creation: FactorId = 56254024255537152

				var f1 = new CreateFabFactor();
				f1.UsesPrimaryArtifactId = c.Id;
				f1.UsesRelatedArtifactId= filmId;
				f1.AssertionType = FactorAssertionId.Fact;
				f1.IsDefining = true;
				f1.Note = "ISO Speed measures film sensitivity.";
				f1.Descriptor = new CreateFabDescriptor();
				f1.Descriptor.Type = DescriptorTypeId.RefersTo;
				f1.Descriptor.RefinesTypeWithArtifactId = measureId;
				f1.Descriptor.RefinesRelatedWithArtifactId = sensitivityId;
				//after creation: FactorId = 56254024248197120

				////

				FabFactor res0 = pFab.Services.Modify.Factors.Post(f0).FirstDataItem();
				Log.Debug("FactorId 0: "+res0.Id);

				FabFactor res1 = pFab.Services.Modify.Factors.Post(f1).FirstDataItem();
				Log.Debug("FactorId 1: "+res1.Id);
			}
			catch ( FabricErrorException fe ) {
				Log.Error("Fabric Error: "+fe.Message, fe);
			}
			finally {
				pFab.UseAppDataProvider = false;
			}
		}

	}

}
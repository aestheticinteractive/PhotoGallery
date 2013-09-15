using System;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Account.Tools {

	/*================================================================================================*/
	public enum LiveArtifactId : long {
		KinstnerPhotoGallery = 55612410061389824,
		PhotographAlbum = 55434350943010816,

		Camera = 55435679045255168,
		CameraMake = 55434672205725697,
		CameraModel = 55434672209920000,
		Record = 55437863092748288,

		Photograph = 55434279714291712,
		FNumber = 55431237157781505,
		ISOSpeed = 56242555254210560,
		Shutter = 55434431082528768,
		Utilize = 55435285740126208,
		Flash = 55435679049449472,
		Width = 55433968328114177,
		Height = 55433968330211328,
		FocalLength = 55435397586485248
	};


	/*================================================================================================*/
	public class FabricFactorBuilder {

		public FabricArtifact PrimaryArtifact { get; private set; }
		public LiveArtifactId PrimaryArtifactId { get; private set; }
		public FabricArtifact RelatedArtifact { get; private set; }
		public LiveArtifactId RelatedArtifactId { get; private set; }

		public FabEnumsData.FactorAssertionId FactorAssertion { get; private set; }
		public bool IsDefining { get; private set; }
		public string Note { get; set; }
		public string InternalNote { get; private set; }

		public FabEnumsData.DescriptorTypeId DesType { get; private set; }
		public LiveArtifactId? DesPrimaryArtifactRefineId { get; set; }
		public LiveArtifactId? DesRelatedArtifactRefineId { get; set; }
		public LiveArtifactId? DesTypeRefineId { get; set; }

		public FabEnumsData.DirectorTypeId? DirType { get; private set; }
		public FabEnumsData.DirectorActionId? DirPrimaryAction { get; private set; }
		public FabEnumsData.DirectorActionId? DirRelatedAction { get; private set; }

		public FabEnumsData.EventorTypeId? EveType { get; private set; }
		public FabEnumsData.EventorPrecisionId? EvePrecision { get; private set; }
		public DateTime EveDateTime { get; private set; }

		public FabEnumsData.IdentorTypeId? IdenType { get; private set; }
		public string IdenValue { get; private set; }

		public FabEnumsData.LocatorTypeId? LocType { get; private set; }
		public double? LocValueX { get; private set; }
		public double? LocValueY { get; private set; }
		public double? LocValueZ { get; private set; }

		public FabEnumsData.VectorTypeId? VecType { get; private set; }
		public FabEnumsData.VectorUnitId? VecUnit { get; private set; }
		public FabEnumsData.VectorUnitPrefixId? VecUnitPrefix { get; private set; }
		public long VecValue { get; private set; }
		public LiveArtifactId? VecAxisArtifactId { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricFactorBuilder() {}

		/*--------------------------------------------------------------------------------------------*/
		public FabricFactorBuilder(string pInternalNote) {
			InternalNote = pInternalNote;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Init(FabricArtifact pPrimary, FabEnumsData.DescriptorTypeId pDesType,
						FabricArtifact pRelated, FabEnumsData.FactorAssertionId pAsrt, bool pIsDef) {
			PrimaryArtifact = pPrimary;
			DesType = pDesType;
			RelatedArtifact = pRelated;
			FactorAssertion = pAsrt;
			IsDefining = pIsDef;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Init(FabricArtifact pPrimary, FabEnumsData.DescriptorTypeId pDesType,
						LiveArtifactId pRelatedId, FabEnumsData.FactorAssertionId pAsrt, bool pIsDef) {
			PrimaryArtifact = pPrimary;
			DesType = pDesType;
			RelatedArtifactId = pRelatedId;
			FactorAssertion = pAsrt;
			IsDefining = pIsDef;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddVector(LiveArtifactId? pVecAxisArtifactId, FabEnumsData.VectorTypeId pVecType,
										long pVecValue, FabEnumsData.VectorUnitPrefixId pVecUnitPrefix,
										FabEnumsData.VectorUnitId pVecUnit) {
			VecAxisArtifactId = pVecAxisArtifactId;
			VecType = pVecType;
			VecValue = pVecValue;
			VecUnitPrefix = pVecUnitPrefix;
			VecUnit = pVecUnit;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddIdentor(FabEnumsData.IdentorTypeId pKey, string pValue) {
			IdenType = pKey;
			IdenValue = pValue;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddEventor(FabEnumsData.EventorTypeId pEveType, 
										FabEnumsData.EventorPrecisionId pEvePrec, DateTime pDateTime) {
			EveType = pEveType;
			EvePrecision = pEvePrec;
			EveDateTime = pDateTime;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddLocator(FabEnumsData.LocatorTypeId pLocType, double pX, double pY, double pZ) {
			LocType = pLocType;
			LocValueX = pX;
			LocValueY = pY;
			LocValueZ = pZ;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddDirector(FabEnumsData.DirectorTypeId pDirType,
				FabEnumsData.DirectorActionId pPrimaryAct, FabEnumsData.DirectorActionId pRelatedAct) {
			DirType = pDirType;
			DirPrimaryAction = pPrimaryAct;
			DirRelatedAction = pRelatedAct;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricFactor ToFactor() {
			var ff = new FabricFactor();

			ff.Primary = PrimaryArtifact;
			ff.PrimaryArtifactId = (long)PrimaryArtifactId;
			ff.Related = RelatedArtifact;
			ff.RelatedArtifactId = (long)RelatedArtifactId;

			ff.FactorAssertionId = (byte)FactorAssertion;
			ff.IsDefining = IsDefining;
			ff.Note = Note;
			ff.InternalNote = InternalNote;

			ff.DesTypeId = (byte)DesType;
			ff.DesPrimaryArtifactRefineId = (long?)DesPrimaryArtifactRefineId;
			ff.DesRelatedArtifactRefineId = (long?)DesRelatedArtifactRefineId;
			ff.DesTypeRefineId = (long?)DesTypeRefineId;

			ff.DirTypeId = (byte?)DirType;
			ff.DirPrimaryActionId = (byte?)DirPrimaryAction;
			ff.DirRelatedActionId = (byte?)DirRelatedAction;

			ff.EveTypeId = (byte?)EveType;
			ff.EvePrecisionId = (byte?)EvePrecision;
			ff.EveDateTime = EveDateTime.ToUniversalTime().Ticks;

			ff.IdenTypeId = (byte?)IdenType;
			ff.IdenValue = IdenValue;

			ff.LocTypeId = (byte?)LocType;
			ff.LocValueX = LocValueX;
			ff.LocValueY = LocValueY;
			ff.LocValueZ = LocValueZ;

			ff.VecTypeId = (byte?)VecType;
			ff.VecUnitId = (byte?)VecUnit;
			ff.VecUnitPrefixId = (byte?)VecUnitPrefix;
			ff.VecValue = VecValue;
			ff.VecAxisArtifactId = (long?)VecAxisArtifactId;

			return ff;
		}

	}

}
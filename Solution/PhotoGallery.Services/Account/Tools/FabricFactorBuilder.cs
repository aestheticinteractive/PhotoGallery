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
		Flash = 55435679049449472,
		Width = 55433968328114177,
		Height = 55433968330211328,
		FocalLength = 55435397586485248,
		ComputerFile = 55435461735219200,
		Upload = 55437908941733888,

		Person = 55429859899342848,
		MalePerson = 55429865850011648,
		FemalePerson = 55429868074041344,

		Depict = 55437768435695617,

		LhasaApso = 55435726858223616,
		GreatDane = 55435082418094080,
		Dog = 55435554928459776,
		Pet = 55434242198339585
	};


	/*================================================================================================*/
	public class FabricFactorBuilder {

		public FabricArtifact CreatorUserArtifact { get; private set; }

		public FabricArtifact PrimaryArtifact { get; private set; }
		public long? PrimaryArtifactId { get; private set; }
		public FabricArtifact RelatedArtifact { get; private set; }
		public long? RelatedArtifactId { get; private set; }

		public FactorAssertionId FactorAssertion { get; private set; }
		public bool IsDefining { get; private set; }
		public string Note { get; set; }
		public string InternalNote { get; private set; }

		public DescriptorTypeId DesType { get; private set; }
		public LiveArtifactId? DesPrimaryArtifactRefineId { get; set; }
		public LiveArtifactId? DesRelatedArtifactRefineId { get; set; }
		public LiveArtifactId? DesTypeRefineId { get; set; }

		public DirectorTypeId? DirType { get; private set; }
		public DirectorActionId? DirPrimaryAction { get; private set; }
		public DirectorActionId? DirRelatedAction { get; private set; }

		public EventorTypeId? EveType { get; private set; }
		public long? EveYear { get; private set; }
		public byte? EveMonth { get; private set; }
		public byte? EveDay { get; private set; }
		public byte? EveHour { get; private set; }
		public byte? EveMinute { get; private set; }
		public byte? EveSecond { get; private set; }

		public IdentorTypeId? IdenType { get; private set; }
		public string IdenValue { get; private set; }

		public LocatorTypeId? LocType { get; private set; }
		public double? LocValueX { get; private set; }
		public double? LocValueY { get; private set; }
		public double? LocValueZ { get; private set; }

		public VectorTypeId? VecType { get; private set; }
		public VectorUnitId? VecUnit { get; private set; }
		public VectorUnitPrefixId? VecUnitPrefix { get; private set; }
		public long? VecValue { get; private set; }
		public LiveArtifactId? VecAxisArtifactId { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricFactorBuilder(FabricArtifact pCreatorUserArtifact) {
			CreatorUserArtifact = pCreatorUserArtifact;
		}

		/*--------------------------------------------------------------------------------------------*/
		public FabricFactorBuilder(FabricArtifact pCreatorUserArtifact, string pInternalNote) {
			CreatorUserArtifact = pCreatorUserArtifact;
			InternalNote = pInternalNote;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Init(FabricArtifact pPrimary, DescriptorTypeId pDesType,
						FabricArtifact pRelated, FactorAssertionId pAsrt, bool pIsDef) {
			PrimaryArtifact = pPrimary;
			DesType = pDesType;
			RelatedArtifact = pRelated;
			FactorAssertion = pAsrt;
			IsDefining = pIsDef;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Init(FabricArtifact pPrimary, DescriptorTypeId pDesType,
						LiveArtifactId pRelatedId, FactorAssertionId pAsrt, bool pIsDef) {
			Init(pPrimary, pDesType, (long)pRelatedId, pAsrt, pIsDef);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Init(FabricArtifact pPrimary, DescriptorTypeId pDesType,
						long pRelatedId, FactorAssertionId pAsrt, bool pIsDef) {
			PrimaryArtifact = pPrimary;
			DesType = pDesType;
			RelatedArtifactId = pRelatedId;
			FactorAssertion = pAsrt;
			IsDefining = pIsDef;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddVector(LiveArtifactId? pVecAxisArtifactId, VectorTypeId pVecType,
										long pVecValue, VectorUnitPrefixId pVecUnitPrefix,
										VectorUnitId pVecUnit) {
			VecAxisArtifactId = pVecAxisArtifactId;
			VecType = pVecType;
			VecValue = pVecValue;
			VecUnitPrefix = pVecUnitPrefix;
			VecUnit = pVecUnit;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddIdentor(IdentorTypeId pKey, string pValue) {
			IdenType = pKey;
			IdenValue = pValue;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddEventor(EventorTypeId pEveType, DateTime pDateTimeUtc) {
			EveType = pEveType;
			EveYear = pDateTimeUtc.Year;
			EveMonth = (byte?)pDateTimeUtc.Month;
			EveDay = (byte?)pDateTimeUtc.Day;
			EveHour = (byte?)pDateTimeUtc.Hour;
			EveMinute = (byte?)pDateTimeUtc.Minute;
			EveSecond = (byte?)pDateTimeUtc.Second;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddLocator(LocatorTypeId pLocType, double pX, double pY, double pZ) {
			LocType = pLocType;
			LocValueX = pX;
			LocValueY = pY;
			LocValueZ = pZ;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddDirector(DirectorTypeId pDirType,
				DirectorActionId pPrimaryAct, DirectorActionId pRelatedAct) {
			DirType = pDirType;
			DirPrimaryAction = pPrimaryAct;
			DirRelatedAction = pRelatedAct;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricFactor ToFactor() {
			var ff = new FabricFactor();
			ff.Creator = CreatorUserArtifact;

			ff.Primary = PrimaryArtifact;
			ff.PrimaryArtifactId = PrimaryArtifactId;
			ff.Related = RelatedArtifact;
			ff.RelatedArtifactId = RelatedArtifactId;

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
			ff.EveYear = EveYear;
			ff.EveMonth = EveMonth;
			ff.EveDay = EveDay;
			ff.EveHour = EveHour;
			ff.EveMinute = EveMinute;
			ff.EveSecond = EveSecond;

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

		/*--------------------------------------------------------------------------------------------*/
		public static CreateFabFactor DbFactorToBatchFactor(FabricFactor pFac) {
			Func<long?, long> toLong = (x => (x == null ? 0 : (long)x));
			Func<byte?, byte> toByte = (x => (x == null ? (byte)0 : (byte)x));
			Func<double?, double> toDouble = (x => (x == null ? 0 : (double)x));

			var f = new CreateFabFactor();
			f.UsesPrimaryArtifactId = (pFac.Primary == null ? 
				toLong(pFac.PrimaryArtifactId) : toLong(pFac.Primary.ArtifactId));
			f.UsesRelatedArtifactId = (pFac.Related == null ? 
				toLong(pFac.RelatedArtifactId) : toLong(pFac.Related.ArtifactId));
			f.AssertionType = (FactorAssertionId)pFac.FactorAssertionId;
			f.IsDefining = pFac.IsDefining;
			f.Note = pFac.Note;

			f.Descriptor = new CreateFabDescriptor();
			f.Descriptor.Type = (DescriptorTypeId)pFac.DesTypeId;
			f.Descriptor.RefinesPrimaryWithArtifactId = pFac.DesPrimaryArtifactRefineId;
			f.Descriptor.RefinesRelatedWithArtifactId = pFac.DesRelatedArtifactRefineId;
			f.Descriptor.RefinesTypeWithArtifactId = pFac.DesTypeRefineId;

			if ( pFac.DirTypeId != null ) {
				f.Director = new CreateFabDirector();
				f.Director.Type = (DirectorTypeId)pFac.DirTypeId;
				f.Director.PrimaryAction = (DirectorActionId)pFac.DirPrimaryActionId;
				f.Director.RelatedAction = (DirectorActionId)pFac.DirRelatedActionId;
			}

			if ( pFac.EveTypeId != null ) {
				f.Eventor = new CreateFabEventor();
				f.Eventor.Type = (EventorTypeId)pFac.EveTypeId;
				f.Eventor.Year = toLong(pFac.EveYear);
				f.Eventor.Month = toByte(pFac.EveMonth);
				f.Eventor.Day = toByte(pFac.EveDay);
				f.Eventor.Hour = toByte(pFac.EveHour);
				f.Eventor.Minute = toByte(pFac.EveMinute);
				f.Eventor.Second = toByte(pFac.EveSecond);
			}

			if ( pFac.IdenTypeId != null ) {
				f.Identor = new CreateFabIdentor();
				f.Identor.Type = (IdentorTypeId)pFac.IdenTypeId;
				f.Identor.Value = pFac.IdenValue;
			}

			if ( pFac.LocTypeId != null ) {
				f.Locator = new CreateFabLocator();
				f.Locator.Type = (LocatorTypeId)pFac.LocTypeId;
				f.Locator.ValueX = toDouble(pFac.LocValueX);
				f.Locator.ValueY = toDouble(pFac.LocValueY);
				f.Locator.ValueZ = toDouble(pFac.LocValueZ);
			}

			if ( pFac.VecTypeId != null ) {
				f.Vector = new CreateFabVector();
				f.Vector.Type = (VectorTypeId)pFac.VecTypeId;
				f.Vector.UsesAxisArtifactId = toLong(pFac.VecAxisArtifactId);
				f.Vector.Unit = (VectorUnitId)pFac.VecUnitId;
				f.Vector.UnitPrefix = (VectorUnitPrefixId)pFac.VecUnitPrefixId;
				f.Vector.Value = toLong(pFac.VecValue);
			}

			return f;
		}

	}

}
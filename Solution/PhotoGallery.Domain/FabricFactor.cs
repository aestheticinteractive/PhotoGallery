namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class FabricFactor {

		public virtual int Id { get; set; }
		public virtual long? FactorId { get; set; }
		public virtual FabricArtifact Creator { get; set; }

		public virtual FabricArtifact Primary { get; set; }
		public virtual long PrimaryArtifactId { get; set; }
		public virtual FabricArtifact Related { get; set; }
		public virtual long RelatedArtifactId { get; set; }

		public virtual byte FactorAssertionId { get; set; }
		public virtual bool IsDefining { get; set; }
		public virtual string Note { get; set; }
		public virtual string InternalNote { get; set; }

		public virtual byte DesTypeId { get; set; }
		public virtual long? DesPrimaryArtifactRefineId { get; set; }
		public virtual long? DesRelatedArtifactRefineId { get; set; }
		public virtual long? DesTypeRefineId { get; set; }

		public virtual byte? DirTypeId { get; set; }
		public virtual byte? DirPrimaryActionId { get; set; }
		public virtual byte? DirRelatedActionId { get; set; }

		public virtual byte? EveTypeId { get; set; }
		public virtual long? EveYear { get; set; }
		public virtual byte? EveMonth { get; set; }
		public virtual byte? EveDay { get; set; }
		public virtual byte? EveHour { get; set; }
		public virtual byte? EveMinute { get; set; }
		public virtual byte? EveSecond { get; set; }

		public virtual byte? IdenTypeId { get; set; }
		public virtual string IdenValue { get; set; }

		public virtual byte? LocTypeId { get; set; }
		public virtual double? LocValueX { get; set; }
		public virtual double? LocValueY { get; set; }
		public virtual double? LocValueZ { get; set; }

		public virtual byte? VecTypeId { get; set; }
		public virtual byte? VecUnitId { get; set; }
		public virtual byte? VecUnitPrefixId { get; set; }
		public virtual long? VecValue { get; set; }
		public virtual long? VecAxisArtifactId { get; set; }

    }

}
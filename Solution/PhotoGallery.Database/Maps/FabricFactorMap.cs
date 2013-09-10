using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class FabricFactorMap : ClassMap<FabricFactor> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricFactorMap() {
			string name = typeof(FabricFactor).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.FactorId).Unique();
			References(x => x.PrimaryInstance).Nullable();
			Map(x => x.PrimaryArtifactId);
			References(x => x.RelatedInstance).Nullable();
			Map(x => x.RelatedArtifactId);

			Map(x => x.FactorAssertionId);
			Map(x => x.IsDefining).Default("0");
			Map(x => x.Note).Length(256).Nullable();

			Map(x => x.DesTypeId);
			Map(x => x.DesPrimaryArtifactRefineId).Nullable();
			Map(x => x.DesRelatedArtifactRefineId).Nullable();
			Map(x => x.DesTypeRefineId).Nullable();

			Map(x => x.DirTypeId).Nullable();
			Map(x => x.DirPrimaryActionId).Nullable();
			Map(x => x.DirRelatedActionId).Nullable();

			Map(x => x.EveTypeId).Nullable();
			Map(x => x.EvePrecisionId).Nullable();
			Map(x => x.EveDateTime).Nullable();

			Map(x => x.IdenTypeId).Nullable();
			Map(x => x.IdenValue).Length(128).Nullable();

			Map(x => x.LocTypeId).Nullable();
			Map(x => x.LocValueX).Nullable();
			Map(x => x.LocValueY).Nullable();
			Map(x => x.LocValueZ).Nullable();

			Map(x => x.VecTypeId).Nullable();
			Map(x => x.VecUnitId).Nullable();
			Map(x => x.VecUnitPrefixId).Nullable();
			Map(x => x.VecValue).Nullable();
			Map(x => x.VecAxisArtifactId).Nullable();
		}

	}

}
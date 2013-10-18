﻿using FluentNHibernate.Mapping;
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

			Map(x => x.FactorId).Nullable().Index(name+"_FactorId");
			References(x => x.Creator).Nullable();

			References(x => x.Primary).Nullable();
			Map(x => x.PrimaryArtifactId).Nullable();
			References(x => x.Related).Nullable();
			Map(x => x.RelatedArtifactId).Nullable();

			Map(x => x.FactorAssertionId);
			Map(x => x.IsDefining).Default("0");
			Map(x => x.Note).Length(256).Nullable();
			Map(x => x.InternalNote).Length(256).Nullable();

			Map(x => x.DesTypeId);
			Map(x => x.DesPrimaryArtifactRefineId).Nullable();
			Map(x => x.DesRelatedArtifactRefineId).Nullable();
			Map(x => x.DesTypeRefineId).Nullable();

			Map(x => x.DirTypeId).Nullable();
			Map(x => x.DirPrimaryActionId).Nullable();
			Map(x => x.DirRelatedActionId).Nullable();

			Map(x => x.EveTypeId).Nullable();
			Map(x => x.EveYear).Nullable();
			Map(x => x.EveMonth).Nullable();
			Map(x => x.EveDay).Nullable();
			Map(x => x.EveHour).Nullable();
			Map(x => x.EveMinute).Nullable();
			Map(x => x.EveSecond).Nullable();

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
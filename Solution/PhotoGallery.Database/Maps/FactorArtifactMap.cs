using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class FabricArtifactMap : ClassMap<FabricArtifact> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricArtifactMap() {
			string name = typeof(FabricArtifact).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.ArtifactId).Nullable().Index(name+"_ArtifactId");
			Map(x => x.Type);
			Map(x => x.FromFab).Default("0");
			References(x => x.Creator).Nullable();

			HasMany(x => x.Albums); //0 or 1
			HasMany(x => x.Photos); //0 or 1
			HasMany(x => x.Humans); //0 or 1
			HasMany(x => x.FabricUsers); //0 or 1

			HasMany(x => x.PrimaryFactors).KeyColumn("PrimaryId");
			HasMany(x => x.RelatedFactors).KeyColumn("RelatedId");
		}

	}

}
using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class FabricUserMap : ClassMap<FabricUser> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricUserMap() {
			string name = typeof(FabricUser).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Name).Length(32).Unique();
			Map(x => x.Created);

			References(x => x.FabricArtifact).Nullable();

			HasMany(x => x.Albums);
		}

	}

}
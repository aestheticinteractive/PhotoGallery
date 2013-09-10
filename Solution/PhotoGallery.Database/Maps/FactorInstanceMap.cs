using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class FabricInstanceMap : ClassMap<FabricInstance> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricInstanceMap() {
			string name = typeof(FabricInstance).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.InstanceId).Unique();
			Map(x => x.Type);

			HasMany(x => x.Albums); //0 or 1
			HasMany(x => x.Photos); //0 or 1
			HasMany(x => x.Humans); //0 or 1
			HasMany(x => x.FabricUsers); //0 or 1

			HasMany(x => x.PrimaryFactors).KeyColumn("PrimaryInstance");
			HasMany(x => x.RelatedFactors).KeyColumn("RelatedInstance");
		}

	}

}
using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class TagMap : ClassMap<Tag> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public TagMap() {
			string name = typeof(Tag).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Name).Length(128).UniqueKey("nd");
			Map(x => x.Disamb).Length(128).Nullable().UniqueKey("nd");
			Map(x => x.Note).Length(256).Nullable();

			References(x => x.FabricArtifact).Nullable();
		}

	}

}
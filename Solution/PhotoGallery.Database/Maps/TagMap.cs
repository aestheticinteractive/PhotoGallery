using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class TagMap : ClassMap<Tag> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public TagMap() {
			Id(x => x.Id)
				.Column(typeof(Tag).Name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Name).Length(24);
			HasMany(x => x.PhotoTags);
		}

	}

}
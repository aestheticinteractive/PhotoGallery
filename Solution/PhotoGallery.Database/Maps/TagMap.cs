using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class TagMap : IAutoMappingOverride<Tag> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Override(AutoMapping<Tag> pMapping) {
			pMapping.Map(x => x.Name).Length(24);
			pMapping.HasMany(x => x.PhotoTags);
		}

	}

}
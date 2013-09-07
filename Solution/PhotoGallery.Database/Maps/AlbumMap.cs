using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class AlbumMap : IAutoMappingOverride<Album> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Override(AutoMapping<Album> pMapping) {
			pMapping.Map(x => x.Title).Length(64);
			pMapping.Map(x => x.LocalPath).Length(128).Nullable();
			pMapping.HasMany(x => x.Photos);
		}

	}

}
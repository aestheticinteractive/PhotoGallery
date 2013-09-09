using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class AlbumMap : ClassMap<Album> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumMap() {
			Id(x => x.Id)
				.Column(typeof(Album).Name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Title).Length(64);
			Map(x => x.LocalPath).Length(128).Nullable();
			HasMany(x => x.Photos);
		}

	}

}
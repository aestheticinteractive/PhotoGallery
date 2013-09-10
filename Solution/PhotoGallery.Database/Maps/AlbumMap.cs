using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class AlbumMap : ClassMap<Album> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public AlbumMap() {
			string name = typeof(Album).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Title).Length(64);

			References(x => x.FabricUser);
			References(x => x.FabricInstance).Nullable();

			HasMany(x => x.Photos);
		}

	}

}
using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoTagMap : ClassMap<PhotoTag> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoTagMap() {
			Id(x => x.Id)
				.Column(typeof(PhotoTag).Name+"Id")
				.GeneratedBy.Native();

			References(x => x.Photo).UniqueKey("pt");
			References(x => x.Tag).UniqueKey("pt");
		}

	}

}
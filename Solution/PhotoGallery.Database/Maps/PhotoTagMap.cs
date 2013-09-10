using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoTagMap : ClassMap<PhotoTag> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoTagMap() {
			string name = typeof(PhotoTag).Name;
			Table(name+"2");

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			References(x => x.Photo).UniqueKey("pt");
			References(x => x.Tag).UniqueKey("pt");
		}

	}

}
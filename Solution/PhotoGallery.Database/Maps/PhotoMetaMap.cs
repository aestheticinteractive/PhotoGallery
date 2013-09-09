using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMetaMap : ClassMap<PhotoMeta> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoMetaMap() {
			Id(x => x.Id)
				.Column(typeof(PhotoMeta).Name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Label).Length(32);
			Map(x => x.Type).Length(10);
			Map(x => x.Value).Length(128);
			References(x => x.Photo);
		}

	}

}
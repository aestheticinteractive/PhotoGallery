using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMetaMap : ClassMap<PhotoMeta> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoMetaMap() {
			string name = typeof(PhotoMeta).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.Label).Length(32);
			Map(x => x.Value).Length(128);

			References(x => x.Photo);
			References(x => x.FabricFactor).Nullable();
		}

	}

}
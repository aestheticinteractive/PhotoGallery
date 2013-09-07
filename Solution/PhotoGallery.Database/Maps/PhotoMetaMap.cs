using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMetaMap : IAutoMappingOverride<PhotoMeta> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Override(AutoMapping<PhotoMeta> pMapping) {
			pMapping.Map(x => x.Label).Length(32);
			pMapping.Map(x => x.Type).Length(10);
			pMapping.Map(x => x.Value).Length(128);
			pMapping.References(x => x.Photo);
		}

	}

}
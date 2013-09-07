using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMap : IAutoMappingOverride<Photo> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Override(AutoMapping<Photo> pMapping) {
			pMapping.Map(x => x.ImgName).Length(64);
			pMapping.Map(x => x.ExifDTOrig);
			pMapping.Map(x => x.Favorite).Default("0");

			pMapping.Map(x => x.ExifDTOrig).Nullable();
			pMapping.Map(x => x.ExifExposureTime).Nullable();
			pMapping.Map(x => x.ExifISOSpeed).Nullable();
			pMapping.Map(x => x.ExifFNumber).Nullable();
			pMapping.Map(x => x.ExifFocalLength).Nullable();

			pMapping.Map(x => x.FabricArtifactId).Nullable();
			pMapping.Map(x => x.FabricTalkId).Nullable();

			pMapping.References(x => x.Album);
			pMapping.HasMany(x => x.PhotoTags);
			pMapping.HasMany(x => x.PhotoMetas);
		}

	}

}
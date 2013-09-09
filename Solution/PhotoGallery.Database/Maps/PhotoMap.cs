using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMap : ClassMap<Photo> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoMap() {
			Id(x => x.Id)
				.Column(typeof(Photo).Name+"Id")
				.GeneratedBy.Native();

			Map(x => x.ImgName).Length(64);
			Map(x => x.Favorite).Default("0");

			Map(x => x.ExifDTOrig).Nullable();
			Map(x => x.ExifExposureTime).Nullable();
			Map(x => x.ExifISOSpeed).Nullable();
			Map(x => x.ExifFNumber).Nullable();
			Map(x => x.ExifFocalLength).Nullable();

			Map(x => x.FabricArtifactId).Nullable();
			Map(x => x.FabricTalkId).Nullable();

			References(x => x.Album);
			HasMany(x => x.PhotoTags);
			HasMany(x => x.PhotoMetas);
		}

	}

}
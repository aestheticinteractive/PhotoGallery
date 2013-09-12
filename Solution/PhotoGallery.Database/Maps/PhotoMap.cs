using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class PhotoMap : ClassMap<Photo> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoMap() {
			string name = typeof(Photo).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.ImgName).Length(64);
			Map(x => x.Ratio);
			
			/*Map(x => x.Favorite).Default("0");
			Map(x => x.ExifDTOrig).Nullable();
			Map(x => x.ExifExposureTime).Nullable();
			Map(x => x.ExifISOSpeed).Nullable();
			Map(x => x.ExifFNumber).Nullable();
			Map(x => x.ExifFocalLength).Nullable();*/

			References(x => x.FabricArtifact).Nullable();
			References(x => x.Album);

			HasMany(x => x.PhotoMetas);
		}

	}

}
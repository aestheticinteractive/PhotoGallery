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
			Map(x => x.Width);
			Map(x => x.Height);
			Map(x => x.Ratio);
			Map(x => x.Created);

			Map(x => x.Make).Length(128).Nullable();
			Map(x => x.Model).Length(128).Nullable();

			Map(x => x.FNum).Nullable();
			Map(x => x.Iso).Nullable();
			Map(x => x.Date).Nullable();
			Map(x => x.ExpTime).Nullable();
			Map(x => x.FocalLen).Nullable();
			Map(x => x.Flash).Nullable();

			Map(x => x.GpsLat).Nullable();
			Map(x => x.GpsLng).Nullable();
			Map(x => x.GpsAlt).Nullable();

			References(x => x.FabricArtifact).Nullable();
			References(x => x.Album);
			References(x => x.FabricUser);

			HasMany(x => x.PhotoMetas);
		}

	}

}
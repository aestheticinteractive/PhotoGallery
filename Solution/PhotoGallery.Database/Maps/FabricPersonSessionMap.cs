using FluentNHibernate.Mapping;
using PhotoGallery.Domain;

namespace PhotoGallery.Database.Maps {
	
	/*================================================================================================*/
	public class FabricPersonSessionMap : ClassMap<FabricPersonSession> {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabricPersonSessionMap() {
			string name = typeof(FabricPersonSession).Name;
			Table(name);

			Id(x => x.Id)
				.Column(name+"Id")
				.GeneratedBy.Native();

			Map(x => x.SessionId).Length(32);
			Map(x => x.GrantCode).Length(32).Nullable();
			Map(x => x.BearerToken).Length(32);
			Map(x => x.RefreshToken).Length(32).Nullable();
			Map(x => x.Expiration);
			Map(x => x.TryUpdate).Default("1");

			References(x => x.FabricUser);
		}

	}

}
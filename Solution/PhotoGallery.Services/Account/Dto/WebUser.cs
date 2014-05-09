using System;
using Fabric.Clients.Cs.Api;
using PhotoGallery.Domain;

namespace PhotoGallery.Services.Account.Dto {

	/*================================================================================================*/
	public class WebUser {

		public string Name { get; internal set; }
		public long ArtifactId { get; internal set; }
		public int FabricUserId { get; internal set; }
		public DateTime Created { get; internal set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebUser(FabUser pFabUser, FabricUser pFabricUser) {
			Name = pFabUser.Name;
			ArtifactId = pFabUser.Id;
			FabricUserId = pFabricUser.Id;

			Created = new DateTime(pFabricUser.Created, DateTimeKind.Utc)
				.AddTicks(DateTime.Now.Ticks-DateTime.UtcNow.Ticks);
		}
	}

}
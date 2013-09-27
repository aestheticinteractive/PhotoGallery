using System;
using Fabric.Clients.Cs;
using PhotoGallery.Database;
using PhotoGallery.Domain;

namespace PhotoGallery.Daemon.Fabric {

	/*================================================================================================*/
	public class ServiceContext {

		public virtual ISessionProvider SessProv { get; set; }
		public virtual Queries Query { get; set; }
		public virtual Func<string, IFabricClient> FabClientProv { get; set; }
		public virtual Func<ExporterContext, IFabricClient, FabricUser, Exporter> ExportProv { get; set; }

	}

}
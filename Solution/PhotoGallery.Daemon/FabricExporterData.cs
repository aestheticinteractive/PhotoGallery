using System;
using Fabric.Clients.Cs;
using PhotoGallery.Database;

namespace PhotoGallery.Daemon {
	
	/*================================================================================================*/
	public class FabricExporterData {

		public ISessionProvider SessProv { get; set; }
		public Queries Query { get; set; }
		public IFabricClient Fab { get; set; }
		public SavedSession SavedSession { get; set; }
		public Action<SavedSession> RegisterSession { get; set; }
		public Action<SavedSession> CompleteSession { get; set; }

	}

}
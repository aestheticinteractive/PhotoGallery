using PhotoGallery.Database;

namespace PhotoGallery.Daemon {
	
	/*================================================================================================*/
	public class FabricExporterData {

		public ISessionProvider SessProv { get; set; }
		public Queries Query { get; set; }
		public SavedSession SavedSession { get; set; }

	}

}
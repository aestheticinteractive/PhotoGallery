using PhotoGallery.Database;

namespace PhotoGallery.Daemon.Fabric {
	
	/*================================================================================================*/
	public class ExporterContext {

		public ISessionProvider SessProv { get; set; }
		public Queries Query { get; set; }
		public SavedSession SavedSession { get; set; }

	}

}
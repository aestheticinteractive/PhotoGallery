namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebAlbumStats {

		public int PhotoCount { get; set; }
		public double AvgFNum { get; set; }
		public double AvgIso { get; set; }
		public double AvgExpTime { get; set; }
		public double AvgFocalLen { get; set; }
		public int FlashCount { get; set; }

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string GetAvgFNumString() {
			return "f/"+(AvgFNum/1000).ToString("0.00");
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public string GetFlashPercentString() {
			return (FlashCount/(double)PhotoCount*100).ToString("0.0")+"%";
		}

	}

}
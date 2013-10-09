namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebPhotoMeta {

		internal long? OrigFNumber { get; set; }
		internal long? OrigIsoSpeed { get; set; }
		internal long? OrigExposure { get; set; }
		internal long? OrigFocalLen { get; set; }
		internal bool? OrigUsesFlash { get; set; }

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public double? FNumber {
			get {
				return (OrigFNumber == null ? null : OrigFNumber/1000.0);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public long? IsoSpeed {
			get {
				return OrigIsoSpeed;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public double? Exposure {
			get {
				return (OrigExposure == null ? null : OrigExposure/1000000.0);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public long? FocalLen {
			get {
				return OrigFocalLen;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool? UsesFlash {
			get {
				return OrigUsesFlash;
			}
		}

	}

}
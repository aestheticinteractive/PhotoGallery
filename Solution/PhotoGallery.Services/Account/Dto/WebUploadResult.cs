﻿namespace PhotoGallery.Services.Account.Dto {

	/*================================================================================================*/
	public class WebUploadResult {

		public enum UploadStatus {
			NotStarted = 0,
			InProgress,
			Success,
			ConvertResizeError,
			DatabaseInsertError,
			SavePhotoError
		};

		public string Filename { get; internal set; }
		public UploadStatus Status { get; internal set; }
		public int PhotoId { get; internal set; }

	}

}
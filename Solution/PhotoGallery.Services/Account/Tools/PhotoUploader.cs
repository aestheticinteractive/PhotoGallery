using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Util;

namespace PhotoGallery.Services.Account.Tools {
	
	/*================================================================================================*/
	public class PhotoUploader {

		private const string ImageHeaderPattern = @"data:image/(?<type>.+?),(?<data>.+)";

		public WebUploadResult Result { get; private set; }
		
		private readonly HttpServerUtilityBase vServer;
		private readonly int vAlbumId;
		private readonly string vExifData;
		private readonly string vImageData;

		private Stopwatch vTimer;
		private Image vOrig;
		private Image vImage;
		private Image vThumb;
		private Photo vPhoto;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoUploader(HttpServerUtilityBase pServer, int pAlbumId, string pFilename,
																string pExifData, string pImageData) {
			vServer = pServer;
			vAlbumId = pAlbumId;
			vExifData = pExifData;
			vImageData = pImageData;

			Result = new WebUploadResult();
			Result.Filename = pFilename;
			Result.Status = WebUploadResult.UploadStatus.NotStarted;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveFile(ISession pSess) {
			vTimer = Stopwatch.StartNew();
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				string base64Data = Regex.Match(vImageData, ImageHeaderPattern).Groups["data"].Value;
				byte[] binData = Convert.FromBase64String(base64Data);
				LogTimer("Base64 Complete");

				using ( var ms = new MemoryStream(binData) ) {
					Resize(ms);
					Insert(pSess);
					Save();
				}

				AddExif(pSess);
			}
			catch ( Exception ex ) {
				Log.Error("SaveFile failed: "+Result.Status);
				Log.Error(ex.ToString());
				return;
			}

			Result.Status = WebUploadResult.UploadStatus.Success;
			Log.Debug("SaveFile: "+Result.Filename+" - "+Result.Status);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void LogTimer(string pName) {
			Log.Info("PhotoUploader: "+pName+" @ "+vTimer.Elapsed.TotalMilliseconds+"ms");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void Resize(MemoryStream pOrigStream) {
			try {
				vOrig = Image.FromStream(pOrigStream);
				vImage = ResizeImage(vOrig, new Size(1024, 1024));
				vThumb = ResizeImage(vOrig, new Size(100, 100));
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.ConvertResizeError;
				throw;
			}

			LogTimer("Resize Complete");
		}

		/*--------------------------------------------------------------------------------------------*/
		private void Insert(ISession pSess) {
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				using ( ITransaction tx = pSess.BeginTransaction() ) {
					var fa = new FabricArtifact();
					fa.Type = (byte)FabricArtifact.ArtifactType.Photo;
					pSess.Save(fa);

					vPhoto = new Photo();
					vPhoto.ImgName = (Result.Filename ?? "unknown");
					vPhoto.Album = pSess.Load<Album>(vAlbumId);
					vPhoto.Ratio = vOrig.Width/(float)vOrig.Height;
					vPhoto.FabricArtifact = fa;
					pSess.Save(vPhoto);
					Result.PhotoId = vPhoto.Id;

					tx.Commit();
				}
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.DatabaseInsertError;
				throw;
			}

			LogTimer("Insert Complete");
		}

		/*--------------------------------------------------------------------------------------------*/
		private void Save() {
			try {
				string dir = vServer.MapPath("~"+ImageUtil.BuildPhotoPath(vAlbumId));

				if ( !Directory.Exists(dir) ) {
					Directory.CreateDirectory(dir);
				}

				string path = ImageUtil.BuildPhotoPath(vAlbumId, vPhoto.Id, ImageUtil.PhotoSize.Large);
				SaveJpeg(vServer.MapPath("~"+path), vImage, 90);

				path = ImageUtil.BuildPhotoPath(vAlbumId, vPhoto.Id, ImageUtil.PhotoSize.Thumb);
				SaveJpeg(vServer.MapPath("~"+path), vThumb, 75);
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.SavePhotoError;
				throw;
			}

			LogTimer("Save Complete");
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddExif(ISession pSess) {
			var exif = new PhotoExif(vPhoto, vExifData);
			exif.SaveData(pSess);
			LogTimer("Exif Complete");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		//www.switchonthecode.com/tutorials/csharp-tutorial-image-editing-saving-cropping-and-resizing
		/*--------------------------------------------------------------------------------------------*/
		private static Image ResizeImage(Image pSrcImg, Size pSize) {
			int srcW = pSrcImg.Width;
			int srcH = pSrcImg.Height;
			float scaleW = pSize.Width/(float)srcW;
			float scaleH = pSize.Height/(float)srcH;
			float scale = Math.Min(scaleH, scaleW);

			if ( scale >= 1 ) {
				return pSrcImg;
			}

			Log.Info("PhotoUploader: Resizing "+srcW+"x"+srcH);

			int destW = (int)Math.Floor(srcW*scale);
			int destH = (int)Math.Floor(srcH*scale);

			Bitmap b = new Bitmap(destW, destH);
			Graphics g = Graphics.FromImage(b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.DrawImage(pSrcImg, 0, 0, destW, destH);
			g.Dispose();
			return b;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SaveJpeg(string pPath, Image pImage, int pQuality) {
			EncoderParameter qualParam = new EncoderParameter(Encoder.Quality, pQuality);
			ImageCodecInfo jpeg = GetEncoderInfo("image/jpeg");
			if ( jpeg == null ) { return; }

			EncoderParameters encParams = new EncoderParameters(1);
			encParams.Param[0] = qualParam;
			pImage.Save(pPath, jpeg, encParams);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static ImageCodecInfo GetEncoderInfo(string pMimeType) {
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			return codecs.FirstOrDefault(t => t.MimeType == pMimeType);
		}

	}

}
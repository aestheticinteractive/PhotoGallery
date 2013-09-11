using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
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
		
		private readonly int vAlbumId;
		private readonly string vExifData;
		private readonly string vImageData;

		private Image vOrig;
		private Image vImage;
		private Image vThumb;
		private Photo vPhoto;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoUploader(int pAlbumId, string pFilename, string pExifData, string pImageData) {
			vAlbumId = pAlbumId;
			vExifData = pExifData;
			vImageData = pImageData;

			Result = new WebUploadResult();
			Result.Filename = pFilename;
			Result.Status = WebUploadResult.UploadStatus.NotStarted;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveFile(ISession pSess) {
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				//ResizeFile();
				//AddPhoto(pSess);
				//SaveImages();
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


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void ResizeFile() {
			try {
				string base64Data = Regex.Match(vImageData, ImageHeaderPattern).Groups["data"].Value;
				byte[] binData = Convert.FromBase64String(base64Data);

				using ( var ms = new MemoryStream(binData) ) {
					vOrig = Image.FromStream(ms);
				}

				vImage = ResizeImage(vOrig, new Size(1024, 1024));
				vThumb = ResizeImage(vOrig, new Size(100, 100));
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.ConvertResizeError;
				throw;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddPhoto(ISession pSess) {
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				vPhoto = new Photo();
				vPhoto.Album = pSess.Load<Album>(vAlbumId);
				vPhoto.ImgName = Result.Filename;
				pSess.Save(vPhoto);
				Result.PhotoId = vPhoto.Id;
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.DatabaseInsertError;
				throw;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void SaveImages() {
			try {
				string dir = ImageUtil.BuildPhotoPath(vAlbumId);

				if ( !Directory.Exists(dir) ) {
					Directory.CreateDirectory(dir);
				}

				string path = ImageUtil.BuildPhotoPath(vAlbumId, vPhoto.Id, ImageUtil.PhotoSize.Large);
				SaveJpeg(path, vImage, 90);

				path = ImageUtil.BuildPhotoPath(vAlbumId, vPhoto.Id, ImageUtil.PhotoSize.Thumb);
				SaveJpeg(path, vThumb, 75);
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.SavePhotoError;
				throw;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddExif(ISession pSess) {
			var map = new Dictionary<string, string>();
			int i = vExifData.IndexOf("\":");
			//Log.Debug("EXIF: "+vExifData);

			while ( i != -1 ) {
				int prevI = i;
				int tagI = vExifData.LastIndexOf('"', prevI-1);
				string tag = vExifData.Substring(tagI+1, prevI-tagI-1);

				i = vExifData.IndexOf("\":", prevI+2);
				int postValueI = (i == -1 ? vExifData.Length-1 : vExifData.LastIndexOf(',', i));
				string value = vExifData.Substring(prevI+2, postValueI-prevI-2);
				value = value.Trim(new[] { ' ', '"' });

				//Log.Debug("TAG "+tag+": "+value);
				map.Add(tag, value);
			}
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
				Log.Info("Resize not needed: "+pSize.Width+" / "+scale);
				return pSrcImg;
			}

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

			foreach ( ImageCodecInfo t in codecs ) {
				if ( t.MimeType == pMimeType ) { return t; }
			}

			return null;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------* /
		private void ProcessPhotoMetadata(ISession pSession) {
			Dictionary<PropertyTagId,
				KeyValuePair<PropertyTagType, Object>> imgMeta = ImageUtil.BuildPropMap(vImage);

			foreach ( KeyValuePair<PropertyTagId,
					KeyValuePair<PropertyTagType, Object>> prop in imgMeta ) {
				var meta = new PhotoMeta();
				meta.Photo = pPhoto;
				meta.Label = prop.Key.ToString();
				if ( meta.Label.Substring(0, 5) == "Thumb" ) { continue; }

				meta.Type = prop.Value.Key.ToString();
				if ( meta.Type == "Byte" ) { continue; }
				if ( meta.Type == "Undefined" ) { continue; }

				meta.Value = prop.Value.Value.ToString();
				pSession.Save(meta);
			}

			////

			/*pPhoto.ExifDTOrig = ImageUtil.ParseMetaDate(
				(string)imgMeta[PropertyTagId.ExifDTOrig].Value);
			pPhoto.ExifISOSpeed = Convert.ToDouble(imgMeta[PropertyTagId.ExifISOSpeed].Value);
			pPhoto.ExifExposureTime = Convert.ToDouble(imgMeta[PropertyTagId.ExifExposureTime].Value);
			pPhoto.ExifFNumber = Convert.ToDouble(imgMeta[PropertyTagId.ExifFNumber].Value);
			pPhoto.ExifFocalLength = Convert.ToDouble(imgMeta[PropertyTagId.ExifFocalLength].Value);

			pSession.SaveOrUpdate(pPhoto);* /
			//Log.Debug("META: "+pPhoto.ExifFNumber+" / "+pPhoto.ExifFocalLength+
			//	" / "+pPhoto.ExifExposureTime+" / "+pPhoto.ExifISOSpeed+" / "+pPhoto.ExifDTOrig);
		}*/

	}

}
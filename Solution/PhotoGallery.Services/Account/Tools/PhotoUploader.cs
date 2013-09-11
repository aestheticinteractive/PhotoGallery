using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Util;

namespace PhotoGallery.Services.Account.Tools {
	
	/*================================================================================================*/
	public class PhotoUploader {

		public WebUploadResult Result { get; private set; }
		
		private readonly int vAlbumId;
		private readonly HttpPostedFileBase vFile;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoUploader(int pAlbumId, HttpPostedFileBase pFile) {
			vAlbumId = pAlbumId;
			vFile = pFile;

			Result = new WebUploadResult();
			Result.Filename = vFile.FileName;
			Result.Status = WebUploadResult.UploadStatus.NotStarted;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveFile(ISession pSess) {
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				Image orig;
				Image image;
				Image thumb;

				ResizeFile(out orig, out image, out thumb);
				Photo photo = AddPhotoAndMeta(pSess, orig);
				SaveImages(photo.Id, image, thumb);
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
		private void ResizeFile(out Image pOrig, out Image pImage, out Image pThumb) {
			try {
				pOrig = Image.FromStream(vFile.InputStream, true, true);
				pImage = ResizeImage(pOrig, new Size(1024, 1024));
				pThumb = ResizeImage(pOrig, new Size(100, 100));
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.ConvertResizeError;
				throw;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private Photo AddPhotoAndMeta(ISession pSess, Image pOrig) {
			Result.Status = WebUploadResult.UploadStatus.InProgress;

			try {
				Photo photo = new Photo();
				photo.Album = pSess.Load<Album>(vAlbumId);
				photo.ImgName = vFile.FileName;
				pSess.Save(photo);
				Result.PhotoId = photo.Id;

				ProcessPhotoMetadata(pSess, photo, pOrig);
				return photo;
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.DatabaseInsertError;
				throw;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void SaveImages(int pPhotoId, Image pImage, Image pThumb) {
			try {
				string dir = ImageUtil.BuildPhotoPath(vAlbumId);

				if ( !Directory.Exists(dir) ) {
					Directory.CreateDirectory(dir);
				}

				string path = ImageUtil.BuildPhotoPath(vAlbumId, pPhotoId, ImageUtil.PhotoSize.Large);
				SaveJpeg(path, pImage, 90);

				path = ImageUtil.BuildPhotoPath(vAlbumId, pPhotoId, ImageUtil.PhotoSize.Thumb);
				SaveJpeg(path, pThumb, 75);
			}
			catch ( Exception ) {
				Result.Status = WebUploadResult.UploadStatus.SavePhotoError;
				throw;
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
		/*--------------------------------------------------------------------------------------------*/
		private void ProcessPhotoMetadata(ISession pSession, Photo pPhoto, Image pImage) {
			Dictionary<PropertyTagId,
				KeyValuePair<PropertyTagType, Object>> imgMeta = ImageUtil.BuildPropMap(pImage);

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

			pSession.SaveOrUpdate(pPhoto);*/
			//Log.Debug("META: "+pPhoto.ExifFNumber+" / "+pPhoto.ExifFocalLength+
			//	" / "+pPhoto.ExifExposureTime+" / "+pPhoto.ExifISOSpeed+" / "+pPhoto.ExifDTOrig);
		}

	}

}
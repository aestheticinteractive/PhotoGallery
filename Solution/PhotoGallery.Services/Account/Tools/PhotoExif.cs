using System.Collections.Generic;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Dto;

namespace PhotoGallery.Services.Account.Tools {
	
	/*================================================================================================*/
	public class PhotoExif {

		public enum Tag {
			Make,
			Model,
			ExposureTime,
			FNumber,
			ISOSpeedRatings,
			DateTimeOriginal,
			ShutterSpeedValue,
			ApertureValue,
			BrightnessValue,
			Flash, //look for "fired"
			PixelXDimension,
			PixelYDimension,
			FocalLength,

			GPSLatitudeRef,
			GPSLatitude,
			GPSLongitudeRef,
			GPSLongitude,
			GPSAltitudeRef,
			GPSAltitude
		};

		public WebUploadResult Result { get; private set; }

		private readonly Photo vPhoto;
		private readonly string vData;
		private readonly IDictionary<string, string> vTagMap;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoExif(Photo pPhoto, string pData) {
			vPhoto = pPhoto;
			vData = pData;
			vTagMap = new Dictionary<string, string>();

			int i = vData.IndexOf("\":");

			while ( i != -1 ) {
				int prevI = i;
				int tagI = vData.LastIndexOf('"', prevI-1);
				string tag = vData.Substring(tagI+1, prevI-tagI-1);

				i = vData.IndexOf("\":", prevI+2);
				int postValueI = (i == -1 ? vData.Length-1 : vData.LastIndexOf(',', i));

				string value = vData.Substring(prevI+2, postValueI-prevI-2);
				value = value.Trim(new[] { ' ', '"' });

				//Log.Debug("TAG   "+tag+": "+value);
				vTagMap.Add(tag, value);
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveData(ISession pSess) {
			using ( ITransaction tx = pSess.BeginTransaction() ) {
				foreach ( KeyValuePair<string, string> pair in vTagMap ) {
					string key = pair.Key;

					if ( key.Length >= 5 && key.Substring(0, 5) == "Thumb" ) { continue; }

					/*var ff = BuildFactor(pair.Key, pair.Value);

					if ( ff != null ) {
						pSess.Save(ff);
					}*/

					var pm = new PhotoMeta();
					pm.Label = key;
					pm.Value = pair.Value;
					pm.Photo = vPhoto;
					//pm.FabricFactor = ff;
					pSess.Save(pm);
				}

				tx.Commit();
			}
		}

		//TODO: Create Factors
		/*--------------------------------------------------------------------------------------------* /
		private FabricFactor BuildFactor(string pKey, string pValue) {
			var ff = new FabricFactor();

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
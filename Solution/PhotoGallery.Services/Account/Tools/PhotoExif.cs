using System.Collections.Generic;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Dto;

namespace PhotoGallery.Services.Account.Tools {
	
	/*================================================================================================*/
	public class PhotoExif {

		public enum ExifTag {
			Make,
			Model,
			FNumber,
			ISOSpeedRatings,
			DateTimeOriginal,
			ShutterSpeedValue,
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

		public enum TagFabricArtifactId : long {
			Camera = 55435679045255168,
			CameraMake = 55434672205725697, //[make] <make name> is a camera(make) ...
			CameraModel = 55434672209920000, //[model] <model name> is a camera(model) ...

			Photograph = 55434279714291712, //<photo> is a photograph
			FNumber = 55431237157781505, //<photo> is a photograph, vector(fnumber <num> unit)
			ISOSpeed = 0, //<photo> is a photograph, vector(isospeed <iso> units)
			Shutter = 55434431082528768, //<photo> is a photograph, vector(shutter <time> seconds)
			Flash = 55435679049449472, //<photo> uses a flash (if true)
			Width = 55433968328114177, //<photo> is a photograph, vector(width <w> pixels)
			Height = 55433968330211328, //<photo> is a photograph, vector(height <h> pixels)
			FocalLength = 55435397586485248, //<photo> is a photograph, vector(focallen <f> millimeters)

			//TODO: create a "ISOSpeed" Fabric Class
			//Film = 55434464925319168, //
			//FilmSpeed = 55433969909366785, //[speed]: <photo> uses film(speed), vector(speed <iso> unit)
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
			InsertMetas(pSess);
			InsertFactors(pSess);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void InsertMetas(ISession pSess) {
			using ( ITransaction tx = pSess.BeginTransaction() ) {
				foreach ( KeyValuePair<string, string> pair in vTagMap ) {
					string key = pair.Key;

					if ( key.Length >= 5 && key.Substring(0, 5) == "Thumb" ) { continue; }

					var pm = new PhotoMeta();
					pm.Label = key;
					pm.Value = pair.Value;
					pm.Photo = vPhoto;
					pSess.Save(pm);
				}

				tx.Commit();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void InsertFactors(ISession pSess) {
			Tag makeTag = GetMakeTag(pSess);

			/*Make,
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
			GPSAltitude*/

			/*ff.Primary = pSess.Load<FabricArtifact>(vPhoto.FabricArtifact.Id);
			ff.Related = fa;
			ff.FactorAssertionId = (byte)FabEnumsData.FactorAssertionId.Fact;
			ff.IsDefining = false;
			ff.DesTypeId = (byte)FabEnumsData.DescriptorTypeId.IsCreatedBy;*/

			/*pPhoto.ExifDTOrig = ImageUtil.ParseMetaDate(
				(string)imgMeta[PropertyTagId.ExifDTOrig].Value);
			pPhoto.ExifISOSpeed = Convert.ToDouble(imgMeta[PropertyTagId.ExifISOSpeed].Value);
			pPhoto.ExifExposureTime = Convert.ToDouble(imgMeta[PropertyTagId.ExifExposureTime].Value);
			pPhoto.ExifFNumber = Convert.ToDouble(imgMeta[PropertyTagId.ExifFNumber].Value);
			pPhoto.ExifFocalLength = Convert.ToDouble(imgMeta[PropertyTagId.ExifFocalLength].Value);

			pSession.SaveOrUpdate(pPhoto);* /
			//Log.Debug("META: "+pPhoto.ExifFNumber+" / "+pPhoto.ExifFocalLength+
			//	" / "+pPhoto.ExifExposureTime+" / "+pPhoto.ExifISOSpeed+" / "+pPhoto.ExifDTOrig);*/
		}

		/*--------------------------------------------------------------------------------------------*/
		private Tag GetMakeTag(ISession pSess) {
			string makeKey = ExifTag.Make+"";

			if ( !vTagMap.ContainsKey(makeKey) ) {
				return null;
			}

			string makeVal = vTagMap[makeKey];
			const string makeDisamb = "camera make";

			Tag tag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == makeVal && x.Disamb == makeDisamb)
				.Take(1)
				.SingleOrDefault();

			if ( tag != null ) {
				return tag;
			}

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				FabricArtifact fa = new FabricArtifact();
				fa.Type = (byte)FabricArtifact.ArtifactType.Tag;
				pSess.Save(fa);

				tag = new Tag();
				tag.Name = makeVal;
				tag.Disamb = makeDisamb;
				tag.Note = "A make or brand of photographic cameras.";
				tag.FabricArtifact = fa;
				pSess.Save(fa);

				tx.Commit();
			}

			return tag;
		}

		/*--------------------------------------------------------------------------------------------*/
		private Tag GetModelTag(ISession pSess, Tag pMakeTag) {
			string modelKey = ExifTag.Model+"";

			if ( !vTagMap.ContainsKey(modelKey) ) {
				return null;
			}

			string modelVal = vTagMap[modelKey];
			const string modelDisamb = "camera model";
			FabricArtifact fa;

			Tag tag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == modelVal && x.Disamb == modelDisamb)
				.Take(1)
				.SingleOrDefault();

			if ( tag != null ) {
				return tag;
			}

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				fa = new FabricArtifact();
				fa.Type = (byte)FabricArtifact.ArtifactType.Tag;
				pSess.Save(fa);

				tag = new Tag();
				tag.Name = modelVal;
				tag.Disamb = modelDisamb;
				tag.Note = "A model of photographic camera.";
				tag.FabricArtifact = fa;
				pSess.Save(fa);

				if ( pMakeTag != null ) {
					var ff = new FabricFactor(); //<model> is created by <make>
					ff.Primary = fa;
					ff.DesTypeId = (byte)FabEnumsData.DescriptorTypeId.IsCreatedBy;
					ff.Related = pSess.Load<FabricArtifact>(pMakeTag.FabricArtifact.Id);
					ff.FactorAssertionId = (byte)FabEnumsData.FactorAssertionId.Fact;
					ff.IsDefining = true;
					pSess.Save(ff);
				}

				tx.Commit();
			}

			//<photo> is created by (record) <model>

			return tag;
		}

	}

}
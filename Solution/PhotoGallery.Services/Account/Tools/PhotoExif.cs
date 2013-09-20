using System;
using System.Collections.Generic;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Util;

namespace PhotoGallery.Services.Account.Tools {
	
	/*================================================================================================*/
	public class PhotoExif {

		public enum ExifTag {
			Make,
			Model,
			FNumber,
			ISOSpeedRatings,
			DateTimeOriginal,
			ExposureTime,
			Flash,
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
		private readonly Album vAlbum;
		private readonly string vData;
		private readonly string vPhotoLbl;
		private readonly IDictionary<string, string> vTagMap;

		private FabricArtifact vPhotoArt;
		private FabricArtifact vUserArt;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoExif(Photo pPhoto, Album pAlbum, string pData) {
			vPhoto = pPhoto;
			vAlbum = pAlbum;
			vData = pData;
			vPhotoLbl = "<photo "+pPhoto.Id+">";
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
				vTagMap.Add(tag, value);
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveData(ISession pSess) {
			vPhotoArt = pSess.Load<FabricArtifact>(vPhoto.FabricArtifact.Id);
			vUserArt = pSess.Load<FabricArtifact>(vPhoto.FabricUser.FabricArtifact.Id);
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
			FabricArtifact makeArt = TryMake(pSess);
			TryModel(pSess, makeArt);

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				AddBasicData(pSess);
				TryDateTimeOriginal(pSess);
				TryPixelXDimension(pSess);
				TryPixelYDimension(pSess);
				pSess.SaveOrUpdate(vPhoto);
				tx.Commit();
			}

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				TryExposureTime(pSess);
				TryFNumber(pSess);
				TryFocalLength(pSess);
				TryIsoSpeed(pSess);
				TryFlash(pSess);
				TryGpsPos(pSess);

				pSess.SaveOrUpdate(vPhoto);
				tx.Commit();
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private FabricArtifact TryMake(ISession pSess) {
			string key = PhotoHasTag(ExifTag.Make);

			if ( key == null ) {
				return null;
			}

			vPhoto.Make = vTagMap[key];
			const string makeDisamb = "camera make";

			FabricArtifact makeArt = pSess.QueryOver<FabricArtifact>()
				.Where(x => x.Name == vPhoto.Make && x.Disamb == makeDisamb)
				.Take(1)
				.SingleOrDefault();

			if ( makeArt != null ) {
				return makeArt; //tag/artifact already created, don't do anything more
			}

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				makeArt = new FabricArtifact();
				makeArt.Type = (byte)FabricArtifact.ArtifactType.Tag;
				makeArt.Name = vPhoto.Make;
				makeArt.Disamb = makeDisamb;
				makeArt.Note = "A make or brand of photographic cameras.";
				pSess.Save(makeArt);

				Tag makeTag = new Tag();
				makeTag.Name = makeArt.Name;
				makeTag.FabricArtifact = makeArt;
				pSess.Save(makeTag);

				var fb = new FabricFactorBuilder(null, "<make> is an instance of 'make' ('camera')");
				fb.Init(
					makeArt,
					FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
					LiveArtifactId.CameraMake,
					FabEnumsData.FactorAssertionId.Fact,
					true
				);
				fb.DesRelatedArtifactRefineId = LiveArtifactId.Camera;
				pSess.Save(fb.ToFactor());

				tx.Commit();
			}

			return makeArt;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryModel(ISession pSess, FabricArtifact pMakeArt) {
			string key = PhotoHasTag(ExifTag.Model);

			if ( key == null ) {
				return;
			}

			vPhoto.Model = vTagMap[key];
			const string modelDisamb = "camera model";

			FabricArtifact modelArt = pSess.QueryOver<FabricArtifact>()
				.Where(x => x.Name == vPhoto.Model && x.Disamb == modelDisamb)
				.Take(1)
				.SingleOrDefault();

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				if ( modelArt == null ) {
					modelArt = BuildModel(pSess, vPhoto.Model, modelDisamb, pMakeArt);
				}

				var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is created by ('record') "+
					"<model "+vPhoto.Model+">");
				fb.Init(
					vPhotoArt,
					FabEnumsData.DescriptorTypeId.IsCreatedBy,
					modelArt,
					FabEnumsData.FactorAssertionId.Fact,
					true
				);
				fb.DesTypeRefineId = LiveArtifactId.Record;
				pSess.Save(fb.ToFactor());

				tx.Commit();
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private FabricArtifact BuildModel(ISession pSess, string pName, string pDisamb, 
																			FabricArtifact pMakeArt) {
			FabricArtifact modelArt = new FabricArtifact();
			modelArt.Type = (byte)FabricArtifact.ArtifactType.Tag;
			modelArt.Name = pName;
			modelArt.Disamb = pDisamb;
			modelArt.Note = "A model of photographic camera.";
			pSess.Save(modelArt);

			var modelTag = new Tag();
			modelTag.Name = modelArt.Name;
			modelTag.FabricArtifact = modelArt;
			pSess.Save(modelTag);

			if ( pMakeArt == null ) {
				return modelArt;
			}

			var fb = new FabricFactorBuilder(null, "<model "+modelTag.Name+"> is created by "+
				"<make "+pMakeArt.Name+">");
			fb.Init(
				modelArt,
				FabEnumsData.DescriptorTypeId.IsCreatedBy,
				pMakeArt,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			pSess.Save(fb.ToFactor());
			return modelArt;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddBasicData(ISession pSess) {
			var fb = new FabricFactorBuilder(null, vPhotoLbl+" refers to 'Kinstner Photo Gallery' "+
				"('photograph') [iden: 'key' "+vPhoto.Id+"]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.RefersTo,
				LiveArtifactId.KinstnerPhotoGallery,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.AddIdentor(
				FabEnumsData.IdentorTypeId.Key,
				vPhoto.Id+""
			);
			fb.DesRelatedArtifactRefineId = LiveArtifactId.Photograph;
			pSess.Save(fb.ToFactor());

			////

			fb = new FabricFactorBuilder(null, vPhotoLbl+" belongs to <album "+vAlbum.Title+">");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.BelongsTo,
				vAlbum.FabricArtifact,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			pSess.Save(fb.ToFactor());

			////

			var cre = new DateTime(vPhoto.Created);

			fb = new FabricFactorBuilder(null, vPhotoLbl+" ('computer file') created by ('upload') "+
				"<user "+vPhoto.FabricUser.Name+"> [eventor: occur "+cre+"]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsCreatedBy,
				vUserArt,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.AddEventor(
				FabEnumsData.EventorTypeId.Occur,
				cre
			);
			fb.DesPrimaryArtifactRefineId = LiveArtifactId.ComputerFile;
			fb.DesTypeRefineId = LiveArtifactId.Upload;
			pSess.Save(fb.ToFactor());
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void TryDateTimeOriginal(ISession pSess) {
			string key = PhotoHasTag(ExifTag.DateTimeOriginal);

			if ( key == null ) {
				return;
			}

			DateTime val = ImageUtil.ParseMetaDate(vTagMap[key]); //assume EST for now
			TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			val = TimeZoneInfo.ConvertTimeToUtc(val, est);
			vPhoto.Date = val.Ticks;

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[eventor: occur "+val+"]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.AddEventor(
				FabEnumsData.EventorTypeId.Occur,
				val
			);
			pSess.Save(fb.ToFactor());
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void TryPixelXDimension(ISession pSess) {
			string key = PhotoHasTag(ExifTag.PixelXDimension);

			if ( key == null ) {
				return;
			}

			vPhoto.Width = (long)Convert.ToDouble(vTagMap[key]);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'width' "+vPhoto.Width+" base pixels]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.Width,
				FabEnumsData.VectorTypeId.PosLong,
				vPhoto.Width,
				FabEnumsData.VectorUnitPrefixId.Base,
				FabEnumsData.VectorUnitId.Pixel
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryPixelYDimension(ISession pSess) {
			string key = PhotoHasTag(ExifTag.PixelYDimension);

			if ( key == null ) {
				return;
			}

			vPhoto.Height = (long)Convert.ToDouble(vTagMap[key]);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'height' "+vPhoto.Height+" base pixels]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.Height,
				FabEnumsData.VectorTypeId.PosLong,
				vPhoto.Height,
				FabEnumsData.VectorUnitPrefixId.Base,
				FabEnumsData.VectorUnitId.Pixel
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryExposureTime(ISession pSess) {
			string key = PhotoHasTag(ExifTag.ExposureTime);

			if ( key == null ) {
				return;
			}

			vPhoto.ExpTime = (long)(Convert.ToDouble(vTagMap[key])*1000000);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'shutter' "+vPhoto.ExpTime+" micro secs]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.Shutter,
				FabEnumsData.VectorTypeId.PosLong,
				(long)vPhoto.ExpTime,
				FabEnumsData.VectorUnitPrefixId.Micro,
				FabEnumsData.VectorUnitId.Second
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryFNumber(ISession pSess) {
			string key = PhotoHasTag(ExifTag.FNumber);

			if ( key == null ) {
				return;
			}

			vPhoto.FNum = (long)(Convert.ToDouble(vTagMap[key])*1000);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'fnumber' "+vPhoto.FNum+" milli units]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.FNumber,
				FabEnumsData.VectorTypeId.PosLong,
				(long)vPhoto.FNum,
				FabEnumsData.VectorUnitPrefixId.Milli,
				FabEnumsData.VectorUnitId.Unit
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryFocalLength(ISession pSess) {
			string key = PhotoHasTag(ExifTag.FocalLength);

			if ( key == null ) {
				return;
			}

			vPhoto.FocalLen = (long)Convert.ToDouble(vTagMap[key]);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'focal length' "+vPhoto.FocalLen+" milli meters]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.FocalLength,
				FabEnumsData.VectorTypeId.PosLong,
				(long)vPhoto.FocalLen,
				FabEnumsData.VectorUnitPrefixId.Milli,
				FabEnumsData.VectorUnitId.Metre
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryIsoSpeed(ISession pSess) {
			string key = PhotoHasTag(ExifTag.ISOSpeedRatings);

			if ( key == null ) {
				return;
			}

			vPhoto.Iso = (long)Convert.ToDouble(vTagMap[key]);

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[vec: 'iso speed' "+vPhoto.Iso+" base units]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.AddVector(
				LiveArtifactId.ISOSpeed,
				FabEnumsData.VectorTypeId.PosLong,
				(long)vPhoto.Iso,
				FabEnumsData.VectorUnitPrefixId.Base,
				FabEnumsData.VectorUnitId.Unit
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryFlash(ISession pSess) {
			string key = PhotoHasTag(ExifTag.Flash);

			if ( key == null ) {
				return;
			}

			vPhoto.Flash = (vTagMap[key].ToLower().IndexOf("fired") != -1);

			if ( vPhoto.Flash != true ) {
				return;
			}

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" uses 'flash'");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.Uses,
				LiveArtifactId.Flash,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			pSess.Save(fb.ToFactor());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryGpsPos(ISession pSess) {
			string keyLat = PhotoHasTag(ExifTag.GPSLatitude);
			string keyLatRef = PhotoHasTag(ExifTag.GPSLatitudeRef);
			string keyLng = PhotoHasTag(ExifTag.GPSLongitude);
			string keyLngRef = PhotoHasTag(ExifTag.GPSLongitudeRef);
			double alt = 0;

			if ( keyLat == null || keyLatRef == null || keyLng == null || keyLngRef == null ) {
				return;
			}

			try {
				vPhoto.GpsLat = StringToCoord(vTagMap[keyLat], vTagMap[keyLatRef]);
				vPhoto.GpsLng = StringToCoord(vTagMap[keyLng], vTagMap[keyLngRef]);

				string keyAlt = PhotoHasTag(ExifTag.GPSAltitude);
				string keyAltRef = PhotoHasTag(ExifTag.GPSAltitudeRef);

				if ( keyAlt != null && keyAltRef != null ) {
					alt = double.Parse(vTagMap[keyAlt]);

					if ( vTagMap[keyAltRef] == "1" ) {
						alt *= -1;
					}

					vPhoto.GpsAlt = alt;
				}
			}
			catch ( Exception e ) {
				Log.Error("GPS: "+e.Message, e);
				return;
			}

			var fb = new FabricFactorBuilder(vUserArt, vPhotoLbl+" is an instance of 'photograph' "+
				"[loc: earth coord "+vPhoto.GpsLat+", "+vPhoto.GpsLng+", "+vPhoto.GpsAlt+"]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.AddLocator(
				FabEnumsData.LocatorTypeId.EarthCoord,
				(double)vPhoto.GpsLat,
				(double)vPhoto.GpsLng,
				alt
			);
			pSess.Save(fb.ToFactor());
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private string PhotoHasTag(ExifTag pTag) {
			string key = pTag+"";
			return (vTagMap.ContainsKey(key) ? key : null);
		}

		/*--------------------------------------------------------------------------------------------*/
		private double StringToCoord(string pCoordVal, string pRefVal) {
			string[] v = pCoordVal.Trim(new[] { '[', ']' }).Split(',');
			double c = double.Parse(v[0]);

			if ( v.Length > 1 ) {
				c += double.Parse(v[1])/60;
			}

			if ( v.Length > 2 ) {
				c += double.Parse(v[2])/3600;
			}

			if ( pRefVal == "S" || pRefVal == "W" ) {
				c *= -1;
			}

			return c;
		}

	}

}
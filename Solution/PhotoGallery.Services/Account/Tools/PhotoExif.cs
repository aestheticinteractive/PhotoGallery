﻿using System;
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
		private readonly IDictionary<string, string> vTagMap;

		private FabricArtifact vPhotoArt;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PhotoExif(Photo pPhoto, Album pAlbum, string pData) {
			vPhoto = pPhoto;
			vAlbum = pAlbum;
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
				vTagMap.Add(tag, value);
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SaveData(ISession pSess) {
			vPhotoArt = pSess.Load<FabricArtifact>(vPhoto.FabricArtifact.Id);
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
			Tag makeTag = TryMake(pSess);
			TryModel(pSess, makeTag);

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
		private Tag TryMake(ISession pSess) {
			string key = PhotoHasTag(ExifTag.Make);

			if ( key == null ) {
				return null;
			}

			vPhoto.Make = vTagMap[key];
			const string makeDisamb = "camera make";

			Tag makeTag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == vPhoto.Make && x.Disamb == makeDisamb)
				.Take(1)
				.SingleOrDefault();

			if ( makeTag != null ) {
				return makeTag; //tag/artifact already created, don't do anything more
			}

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				FabricArtifact makeArt = new FabricArtifact();
				makeArt.Type = (byte)FabricArtifact.ArtifactType.Tag;
				pSess.Save(makeArt);

				makeTag = new Tag();
				makeTag.Name = vPhoto.Make;
				makeTag.Disamb = makeDisamb;
				makeTag.Note = "A make or brand of photographic cameras.";
				makeTag.FabricArtifact = makeArt;
				pSess.Save(makeTag);

				var fb = new FabricFactorBuilder("<make> is an instance of 'make' ('camera')");
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

			return makeTag;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void TryModel(ISession pSess, Tag pMakeTag) {
			string key = PhotoHasTag(ExifTag.Model);

			if ( key == null ) {
				return;
			}

			vPhoto.Model = vTagMap[key];
			const string modelDisamb = "camera model";

			Tag modelTag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == vPhoto.Model && x.Disamb == modelDisamb)
				.Take(1)
				.SingleOrDefault();

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				FabricArtifact modelArt;

				if ( modelTag == null ) {
					modelArt = BuildModel(pSess, vPhoto.Model, modelDisamb, pMakeTag);
				}
				else {
					modelArt = pSess.Load<FabricArtifact>(modelTag.FabricArtifact.Id);
				}

				var fb = new FabricFactorBuilder("<photo> is created by ('record') <model>");
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
		private FabricArtifact BuildModel(ISession pSess, string pName, string pDisamb, Tag pMakeTag) {
			FabricArtifact modelArt = new FabricArtifact();
			modelArt.Type = (byte)FabricArtifact.ArtifactType.Tag;
			pSess.Save(modelArt);

			var modelTag = new Tag();
			modelTag.Name = pName;
			modelTag.Disamb = pDisamb;
			modelTag.Note = "A model of photographic camera.";
			modelTag.FabricArtifact = modelArt;
			pSess.Save(modelTag);

			if ( pMakeTag == null ) {
				return modelArt;
			}

			var fb = new FabricFactorBuilder("<model> is created by <make>");
			fb.Init(
				modelArt,
				FabEnumsData.DescriptorTypeId.IsCreatedBy,
				pSess.Load<FabricArtifact>(pMakeTag.FabricArtifact.Id),
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			pSess.Save(fb.ToFactor());
			return modelArt;
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddBasicData(ISession pSess) {
			var fb = new FabricFactorBuilder("<photo> refers to 'Kinstner Photo Gallery' "+
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
			pSess.Save(fb.ToFactor());

			////

			fb = new FabricFactorBuilder("<photo> belongs to <album> ('photograph album')");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.BelongsTo,
				vAlbum.FabricArtifact,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.DesRelatedArtifactRefineId = LiveArtifactId.PhotographAlbum;
			pSess.Save(fb.ToFactor());
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void TryDateTimeOriginal(ISession pSess) {
			string key = PhotoHasTag(ExifTag.DateTimeOriginal);

			if ( key == null ) {
				return;
			}

			DateTime val = ImageUtil.ParseMetaDate(vTagMap[key]);
			vPhoto.Date = val.Ticks;

			//TODO: Ensure the photo's timezones are handled correctly.

			var fb = new FabricFactorBuilder(
				"<photo> is an instance of 'photograph' [eventor: occur second "+val+"]");
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

			var fb = new FabricFactorBuilder(
				"<photo> is an instance of 'photograph' [vec: 'width' "+vPhoto.Width+" base pixels]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				true
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

			var fb = new FabricFactorBuilder(
				"<photo> is an instance of 'photograph' [vec: 'height' "+vPhoto.Height+" base pixels]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				true
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

			var fb = new FabricFactorBuilder("<photo> is an instance of 'photograph' "+
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

			var fb = new FabricFactorBuilder("<photo> is an instance of 'photograph' "+
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

			var fb = new FabricFactorBuilder("<photo> is an instance of 'photograph' "+
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

			var fb = new FabricFactorBuilder("<photo> is an instance of 'photograph' "+
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

			var fb = new FabricFactorBuilder("<photo> consumes ('utilize') 'flash'");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.Consumes,
				LiveArtifactId.Flash,
				FabEnumsData.FactorAssertionId.Fact,
				false
			);
			fb.DesTypeRefineId = LiveArtifactId.Utilize;
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
				}
			}
			catch ( Exception e ) {
				Log.Error("GPS: "+e.Message, e);
				return;
			}

			var fb = new FabricFactorBuilder("<photo> is an instance of 'photograph' "+
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
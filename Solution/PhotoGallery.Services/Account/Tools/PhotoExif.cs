using System;
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
			ExposureTime,
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

		private FabricArtifact vPhotoArt;


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

			int n = 0;

			n += TryExposureTime(pSess);

			if ( n == 0 ) { //if no others were created
				var fb = new FabricFactorBuilder("<photo> is an instance of a 'photograph'");
				fb.Init(
					vPhotoArt,
					FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
					LiveArtifactId.Photograph,
					FabEnumsData.FactorAssertionId.Fact,
					true
				);
				pSess.Save(fb.ToFactor());
			}

			/*
			FNumber,
			ISOSpeedRatings,
			DateTimeOriginal,
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

			/*pPhoto.ExifDTOrig = ImageUtil.ParseMetaDate(
				(string)imgMeta[PropertyTagId.ExifDTOrig].Value);
			pPhoto.ExifISOSpeed = Convert.ToDouble(imgMeta[PropertyTagId.ExifISOSpeed].Value);
			pPhoto.ExifFNumber = Convert.ToDouble(imgMeta[PropertyTagId.ExifFNumber].Value);
			pPhoto.ExifFocalLength = Convert.ToDouble(imgMeta[PropertyTagId.ExifFocalLength].Value);

			pSession.SaveOrUpdate(pPhoto);* /
			//Log.Debug("META: "+pPhoto.ExifFNumber+" / "+pPhoto.ExifFocalLength+
			//	" / "+pPhoto.ExifExposureTime+" / "+pPhoto.ExifISOSpeed+" / "+pPhoto.ExifDTOrig);*/
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private Tag TryMake(ISession pSess) {
			string makeKey = PhotoHasTag(ExifTag.Make);

			if ( makeKey == null ) {
				return null;
			}

			string makeVal = vTagMap[makeKey];
			const string makeDisamb = "camera make";

			Tag makeTag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == makeVal && x.Disamb == makeDisamb)
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
				makeTag.Name = makeVal;
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
			string modelKey = PhotoHasTag(ExifTag.Model);

			if ( modelKey == null ) {
				return;
			}

			string modelVal = vTagMap[modelKey];
			const string modelDisamb = "camera model";

			Tag modelTag = pSess.QueryOver<Tag>()
				.Where(x => x.Name == modelVal && x.Disamb == modelDisamb)
				.Take(1)
				.SingleOrDefault();

			using ( ITransaction tx = pSess.BeginTransaction() ) {
				FabricArtifact modelArt;

				if ( modelTag == null ) {
					modelArt = BuildModel(pSess, modelVal, modelDisamb, pMakeTag);
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


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private int TryExposureTime(ISession pSess) {
			string expKey = PhotoHasTag(ExifTag.ExposureTime);

			if ( expKey == null ) {
				return 0;
			}

			string expVal = vTagMap[expKey];

			var fb = new FabricFactorBuilder(
				"<photo> is an instance of 'photograph' [vec: 'shutter' <time> microsec]");
			fb.Init(
				vPhotoArt,
				FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
				LiveArtifactId.Photograph,
				FabEnumsData.FactorAssertionId.Fact,
				true
			);
			fb.AddVector(
				LiveArtifactId.Shutter,
				FabEnumsData.VectorTypeId.PosLong,
				(long)Convert.ToDouble(expVal)*1000000,
				FabEnumsData.VectorUnitPrefixId.Micro,
				FabEnumsData.VectorUnitId.Second
			);
			pSess.Save(fb.ToFactor());

			return 1;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private string PhotoHasTag(ExifTag pTag) {
			string key = pTag+"";
			return (vTagMap.ContainsKey(key) ? key : null);
		}

	}

}
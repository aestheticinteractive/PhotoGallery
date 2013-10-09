using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoGallery.Services.Main.Dto {

	/*================================================================================================*/
	public class WebAlbumMeta {

		public enum Metric {
			FNumber,
			IsoSpeed,
			Exposure,
			FocalLength
		}

		public int AlbumId { get; private set; }
		public IList<WebPhotoMeta> PhotoMetas { get; private set; }
		public int PhotoCount { get; private set; }
		public int FlashCount { get; private set; }

		private readonly Dictionary<Metric, IList<double>> vValues;
		private readonly Dictionary<Metric, double?> vAverages;

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebAlbumMeta(int pAlbumId, IList<WebPhotoMeta> pPhotoMetas) {
			AlbumId = pAlbumId;
			PhotoMetas = pPhotoMetas;
			PhotoCount = PhotoMetas.Count;
			FlashCount = 0;

			vValues = new Dictionary<Metric, IList<double>>();
			vAverages = new Dictionary<Metric, double?>();
			var sums = new Dictionary<Metric, double>();
			var counts = new Dictionary<Metric, int>();

			foreach ( Metric m in Enum.GetValues(typeof(Metric)).Cast<Metric>() ) {
				vValues.Add(m, new List<double>());
				sums.Add(m, 0);
				counts.Add(m, 0);
			}

			foreach ( WebPhotoMeta pm in PhotoMetas ) {
				FlashCount += (pm.UsesFlash == true ? 1 : 0);

				if ( pm.FNumber != null ) {
					vValues[Metric.FNumber].Add((double)pm.FNumber);
					sums[Metric.FNumber] += (long)pm.FNumber;
					counts[Metric.FNumber]++;
				}

				if ( pm.IsoSpeed != null ) {
					vValues[Metric.IsoSpeed].Add((double)pm.IsoSpeed);
					sums[Metric.IsoSpeed] += (long)pm.IsoSpeed;
					counts[Metric.IsoSpeed]++;
				}

				if ( pm.Exposure != null ) {
					vValues[Metric.Exposure].Add((double)pm.Exposure);
					sums[Metric.Exposure] += (double)pm.Exposure;
					counts[Metric.Exposure]++;
				}

				if ( pm.FocalLen != null ) {
					vValues[Metric.FocalLength].Add((double)pm.FocalLen);
					sums[Metric.FocalLength] += (long)pm.FocalLen;
					counts[Metric.FocalLength]++;
				}
			}

			foreach ( KeyValuePair<Metric, int> pair in counts ) {
				double? avg = null;

				if ( pair.Value > 0 ) {
					avg = sums[pair.Key]/pair.Value;
				}

				vAverages.Add(pair.Key, avg);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool HasAverage(Metric pMetric) {
			return (vAverages[pMetric] != null);
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<double> GetValues(Metric pMetric) {
			return vValues[pMetric];
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetValuesString(Metric pMetric) {
			return String.Join(",", vValues[pMetric]);
		}

		/*--------------------------------------------------------------------------------------------*/
		public double? GetAverage(Metric pMetric) {
			return vAverages[pMetric];
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string GetAverageFNumberString() {
			double? fn = vAverages[Metric.FNumber];

			if ( fn == null ) {
				return null;
			}

			double fnd = (double)fn;
			return "f/"+fnd.ToString("0.00");
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public string GetAverageIsoSpeedString() {
			double? i = vAverages[Metric.IsoSpeed];

			if ( i == null ) {
				return null;
			}

			double id = (double)i;
			return id.ToString("0.0");
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetAveragExposureString() {
			double? ex = vAverages[Metric.Exposure];

			if ( ex == null ) {
				return null;
			}

			double exd = (double)ex;

			if ( ex > 0.25 ) {
				return exd.ToString("0.00")+" sec";
			}

			return "1/"+(1/exd).ToString("0.00")+" sec";
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetAverageFocalLengthString() {
			double? fl = vAverages[Metric.FocalLength];

			if ( fl == null ) {
				return null;
			}

			double fld = (double)fl;
			return fld.ToString("0.0")+" mm";
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetFlashUsageString() {
			return (FlashCount/(double)PhotoCount*100).ToString("0.0")+"%";
		}

	}

}
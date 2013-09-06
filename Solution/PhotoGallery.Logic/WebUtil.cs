using System;

namespace PhotoGallery.Logic {

	/*================================================================================================*/
	public static class WebUtil {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static DateTime GetUtcDateTime(long pUtcTicks) {
			return new DateTime(pUtcTicks, DateTimeKind.Utc);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string GetTimeAgoString(long pUtcTicks) {
			return GetTimeAgoString((DateTime.UtcNow.Ticks-pUtcTicks)/10000000.0);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string GetSmartTimeAgoString(long pUtcTicks) {
			long t = DateTime.UtcNow.Ticks-pUtcTicks;
			double sec = t/10000000.0;

			if ( sec < 0 ) {
				return "in "+GetTimeAgoString(-sec);
			}

			return GetTimeAgoString(sec)+" ago";
		}

		/*--------------------------------------------------------------------------------------------*/
		public static string GetTimeAgoString(double pSecondsAgo) {
			long s = (long)pSecondsAgo;
			long m = (long)Math.Floor(s/60.0);
			long h = (long)Math.Floor(m/60.0);
			long d = (long)Math.Floor(h/24.0);
			long w = (long)Math.Floor(d/7.0);

			long n;
			string text;
			if ( w > 0 ) { n = w; text = "week"; }
			else if ( d > 0 ) { n = d; text = "day"; }
			else if ( h > 0 ) { n = h; text = "hour"; }
			else if ( m > 0 ) { n = m; text = "minute"; }
			else { n = s; text = "second"; }
			return n+" "+text+(n == 1 ? "" : "s");
		}

	}

}
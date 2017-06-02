using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace geodesy
{
    public static class DMS
    {
        public const char Separator = '\u202f';

        /// <summary>
        /// Parses string representing degrees/minutes/seconds into numeric degrees.
        /// This is very flexible on formats, allowing signed decimal degrees, or deg-min-sec optionally
        /// suffixed by compass direction(NSEW). A variety of separators are accepted(eg 3° 37′ 09″W).
        /// Seconds and minutes may be omitted.
        /// </summary>
        /// <param name="dmsStr">Degrees or deg/min/sec in variety of formats.</param>
        /// <returns>Degrees as decimal number.</returns>
        /// <example>
        ///     var lat = Dms.parseDMS('51° 28′ 40.12″ N');
        ///     var lon = Dms.parseDMS('000° 00′ 05.31″ W');
        ///     var p1 = new LatLon(lat, lon); // 51.4778°N, 000.0015°W
        /// </example>
        public static double ParseDMS(string dmsStr)
        {
            // check for signed decimal degrees without NSEW, if so return it directly
            double testDms;
            if (double.TryParse(dmsStr, out testDms)) { return testDms; }

            // strip off any sign or compass dir'n & split out separate d/m/s
            dmsStr = dmsStr.Trim();
            var Rgx = new Regex("^-");
            var CleanDmsStr = Rgx.Replace(dmsStr, "");
            Rgx = new Regex("[NSEWnsew]$");
            CleanDmsStr = Rgx.Replace(CleanDmsStr, "");
            var DmsList = new List<string>(Regex.Split(CleanDmsStr, "[^0-9.,]+"));
            if (DmsList[DmsList.Count - 1] == "") { DmsList.RemoveAt(DmsList.Count - 1); }  // from trailing symbol
            if (DmsList.Count == 0) { throw new ArgumentOutOfRangeException(); }

            // and convert to decimal degrees...
            double deg;
            switch (DmsList.Count)
            {
                case 3:  // interpret 3-part result as d/m/s
                    deg = double.Parse(DmsList[0]) / 1 + double.Parse(DmsList[1]) / 60 + double.Parse(DmsList[2]) / 3600;
                    break;
                case 2:  // interpret 2-part result as d/m
                    deg = double.Parse(DmsList[0]) / 1 + double.Parse(DmsList[1]) / 60;
                    break;
                case 1:  // just d (possibly decimal) or non-separated dddmmss
                    deg = double.Parse(DmsList[0]);
                    // check for fixed-width unseparated format eg 0033709W
                    //if (/[NS]/i.test(dmsStr)) deg = '0' + deg;  // - normalise N/S to 3-digit degrees
                    //if (/[0-9]{7}/.test(deg)) deg = deg.slice(0,3)/1 + deg.slice(3,5)/60 + deg.slice(5)/3600;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Regex.IsMatch(dmsStr, "^-|[WSws]$"))// take '-', west and south as -ve
            {
                deg = -deg;
            }
            return deg;
        }

        /// <summary>
        /// Converts decimal degrees to deg/min/sec format
        /// - degree, prime, double-prime symbols are added, but sign is discarded, though no compass direction is added.
        /// </summary>
        /// <param name="degrees">Degrees to be formatted as specified.</param>
        /// <param name="format">Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
        /// <param name="decimals">Number of decimal places to use – default 0 for dms, 2 for dm, 4 for d.</param>
        /// <returns></returns>
        public static string ToDMS(double degrees, string format, int? decimals)
        {
            int Decimals;
            if (format == null) { format = "dms"; }
            if (decimals != null)
            {
                Decimals = (int)decimals;
            }
            else
            {
                switch (format)
                {
                    case "d":
                    case "deg":
                        Decimals = 4;
                        break;
                    case "dm":
                    case "deg+min":
                        Decimals = 2;
                        break;
                    case "dms":
                    case "deg+min+sec":
                        Decimals = 0;
                        break;
                    default:
                        format = "dms";
                        Decimals = 0; break;  // be forgiving on invalid format
                }
            }

            degrees = Math.Abs(degrees);

            string DMS;
            double D;
            double M;
            double S;

            switch (format)
            {
                default: // invalid format spec!
                case "d":
                case "deg":
                    DMS = degrees.ToFormattedString(3, Decimals) + "°";
                    break;
                case "dm":
                case "deg+min":
                    D = Math.Floor(degrees);            // get component deg
                    M = ((degrees * 60) % 60);          // get component min
                    if (M == 60) { M = 0; D++; }        // check for rounding up
                    DMS = string.Format("{0:000}°{1}{2}′", D, Separator, M.ToFormattedString(2, Decimals));
                    break;
                case "dms":
                case "deg+min+sec":
                    D = Math.Floor(degrees);
                    M = Math.Floor((degrees * 3600) / 60) % 60;
                    S = (degrees * 3600 % 60);
                    if (S == 60) { S = 0; M++; }        // check for rounding up
                    if (M == 60) { M = 0; D++; }        // check for rounding up
                    DMS = string.Format("{0:000}°{1}{2:00}′{1}{3}″", D, Separator, M, S.ToFormattedString(2, Decimals));
                    break;
            }
            return DMS;

        }


		public static string ToLat(double degrees, string format, int decimals)
		{
			var Lat = ToDMS(degrees, format, decimals);
			return Lat == null ? "-" : Lat.Substring(1) + Separator + (degrees	< 0 ? 'S' : 'N');  // knock off initial '0' for lat!
		}

		public static string ToLon(double degrees, string format, int decimals)
		{
			var Lon = ToDMS(degrees, format, decimals);
			return Lon == null ? "-" : Lon + Separator + (degrees < 0 ? 'W' : 'E');
		}
	}
}

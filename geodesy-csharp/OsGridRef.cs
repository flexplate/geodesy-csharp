using System;
using System.Text.RegularExpressions;

namespace geodesy
{
	public class OsGridRef
	{
		/// <summary>
		/// Creates an OsGridRef object.
		/// </summary>
		/// <param name="easting">Easting in metres from OS false origin.</param>
		/// <param name="northing">Northing in metres from OS false origin.</param>
		/// <example>var grid = new OsGridRef(651409, 313177);</example>
		public OsGridRef(double easting, double northing)
		{
			Easting = easting;
			Northing = northing;
		}

		public double Easting { get; set; }
		public double Northing { get; set; }

		/// <summary>Converts Ordnance Survey grid reference easting/northing coordinate to latitude/longitude (SW corner of grid square).
		/// 
		/// Note formulation implemented here due to Thomas, Redfearn, etc is as published by OS, but is
		/// inferior to Krüger as used by e.g. Karney 2011.</summary>
		///<param name="gridRef">Grid ref E/N to be converted to lat/long (SW corner of grid square).</param>
		///<param name="datum">Datum to convert grid reference into.</param>
		///<example>var gridref = new OsGridRef(651409.903, 313177.270);
		/// var pWgs84 = OsGridRef.osGridToLatLon(gridref);                     // 52°39′28.723″N, 001°42′57.787″E
		/// to obtain (historical) OSGB36 latitude/longitude point:
		/// var pOsgb = OsGridRef.osGridToLatLon(gridref, LatLon.datum.OSGB36); // 52°39′27.253″N, 001°43′04.518″E
		///</example>
		///<remarks>Currently not accurate enough, owing to a lack of precision in C#'s number handling. Will be ported to use arbitrary-precision maths.</remarks>
		public LatLonEllipsoidal ToLatLon(Datum datum)
		{
			var E = Easting;
			var N = Northing;

			var a = 6377563.396;
			var b = 6356256.909;              // Airy 1830 major & minor semi-axes
			var F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
			var φ0 = (49.0).ToRadians();
			var λ0 = (-2.0).ToRadians();  // NatGrid true origin is 49°N,2°W
			var N0 = -100000;
			var E0 = 400000;                     // northing & easting of true origin, metres
			var e2 = 1 - b * b / (a * a);                          // eccentricity squared
			var n = (a - b) / (a + b);
			var n2 = n * n;
			var n3 = n * n * n;         // n, n², n³

			var φ = φ0;
			var M = 0.0;

			do
			{
				φ = (N - N0 - M) / (a * F0) + φ;

				var Ma = (1 + n + 1.25 * n2 + 1.25 * n3) * (φ - φ0);
				var Mb = (3 * n + 3 * n * n + 2.625 * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
				var Mc = (1.875 * n2 + 1.875 * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
				var Md = (35.0 / 24.0) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
				M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

			} while (N - N0 - M >= 0.00001);  // ie until < 0.01mm

			var cosφ = Math.Cos(φ);
			var sinφ = Math.Sin(φ);
			var ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
			var ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature
			var η2 = ν / ρ - 1;                                    // eta = ?

			var tanφ = Math.Tan(φ);
			var tan2φ = tanφ * tanφ;
			var tan4φ = tan2φ * tan2φ;
			var tan6φ = tan4φ * tan2φ;
			var secφ = 1 / cosφ;
			var ν3 = ν * ν * ν;
			var ν5 = ν3 * ν * ν;
			var ν7 = ν5 * ν * ν;
			var VII = tanφ / (2 * ρ * ν);
			var VIII = tanφ / (24 * ρ * ν3) * (5 + 3 * tan2φ + η2 - 9 * tan2φ * η2);
			var IX = tanφ / (720 * ρ * ν5) * (61 + 90 * tan2φ + 45 * tan4φ);
			var X = secφ / ν;
			var XI = secφ / (6 * ν3) * (ν / ρ + 2 * tan2φ);
			var XII = secφ / (120 * ν5) * (5 + 28 * tan2φ + 24 * tan4φ);
			var XIIA = secφ / (5040 * ν7) * (61 + 662 * tan2φ + 1320 * tan4φ + 720 * tan6φ);

			var dE = (E - E0);
			var dE2 = dE * dE;
			var dE3 = dE2 * dE;
			var dE4 = dE2 * dE2;
			var dE5 = dE3 * dE2;
			var dE6 = dE4 * dE2;
			var dE7 = dE5 * dE2;
			φ = φ - VII * dE2 + VIII * dE4 - IX * dE6;
			var λ = λ0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;

			var Point = new LatLonEllipsoidal(φ.ToDegrees(), λ.ToDegrees(), Datum.OSGB36);
			if (datum != Datum.OSGB36) Point = Point.ConvertDatum(datum);

			return Point;
		}

		/// <summary>
		/// Converts ‘this’ numeric grid reference to standard OS grid reference.
		/// </summary>
		/// <param name="digits">Precision of returned grid reference (10 digits = metres); digits=0 will return grid reference in numeric format.</param>
		/// <returns></returns>
		public string ToString(int digits = 10)
		{
			if (digits % 2 != 0 || digits > 16)
			{
				throw new ArgumentOutOfRangeException("Invalid precision");
			}

			var E = Easting;
			var N = Northing;

			// use digits = 0 to return numeric format (in metres, allowing for decimals & for northing > 1e6)
			if (digits == 0)
			{
				var EInt = Math.Floor(E);
				var EDec = E - EInt;
				var NInt = Math.Floor(N);
				var NDec = N - NInt;
				var EPad = EInt.ToString("000000") + (EDec > 0 ? EDec.ToString(".000") : "");
				var NPad = NInt.ToString("000000") + (NDec > 0 ? NDec.ToString(".000") : "");
				return EPad + "," + NPad;
			}

			// get the 100km-grid indices
			var E100k = Math.Floor(E / 100000);
			var N100k = Math.Floor(N / 100000);

			if (E100k < 0 || E100k > 6 || N100k < 0 || N100k > 12) return "";

			// translate those into numeric equivalents of the grid letters
			var L1 = (19 - N100k) - (19 - N100k) % 5 + Math.Floor((E100k + 10) / 5);
			var L2 = (19 - N100k) * 5 % 25 + E100k % 5;

			// compensate for skipped "I" and build grid letter-pairs
			if (L1 > 7) L1++;
			if (L2 > 7) L2++;
			string LetterPair = Convert.ToChar((int)L1 + 65).ToString() + Convert.ToChar((int)L2 + 65); //65 is char code for "A"

			// strip 100km-grid indices from easting & northing, and reduce precision
			E = Math.Floor((E % 100000) / Math.Pow(10, 5 - digits / 2));
			N = Math.Floor((N % 100000) / Math.Pow(10, 5 - digits / 2));

			return string.Format("{0} {1} {2}", LetterPair, E.ToFormattedString(digits / 2, 0), N.ToFormattedString(digits / 2, 0));
		}

		/// <summary>
		/// Parses grid reference to OsGridRef object.
		/// Accepts standard grid references(eg 'SU 387 148'), with or without whitespace separators, from
		/// two-digit references up to 10-digit references(1m × 1m square), or fully numeric comma-separated
		/// references in metres(eg '438700,114800').
		/// </summary>
		/// <param name="gridRef">Standard format OS grid reference.</param>
		/// <returns></returns>
		public static OsGridRef Parse(string gridRef)
		{
			gridRef = gridRef.Trim();

			// check for fully numeric comma-separated gridref format
			var Matches = Regex.Matches(gridRef, @"^(\d+),\s*(\d+)$");
			double Easting;
			double Northing;
			if (Matches.Count > 0 && Matches[0].Groups.Count == 3 && double.TryParse(Matches[0].Groups[1].ToString(), out Easting) & double.TryParse(Matches[0].Groups[2].ToString(), out Northing))
			{
				return new OsGridRef(Easting, Northing);
			}

			// validate format
			if (!Regex.IsMatch(gridRef, @"^[A-Za-z]{2}\s*[0-9]+\s*[0-9]+$")) { throw new ArgumentException("Invalid grid reference"); }

			// get numeric values of letter references, mapping A->0, B->1, C->2, etc:
			var L1 = gridRef.ToUpper().ToCharArray()[0] - 65;
			var L2 = gridRef.ToUpper().ToCharArray()[1] - 65;
			// shuffle down letters after 'I' since 'I' is not used in grid:
			if (L1 > 7) L1--;
			if (L2 > 7) L2--;

			// convert grid letters into 100km-square indexes from false origin (grid square SV):
			var E100km = ((L1 - 2) % 5) * 5 + (L2 % 5);
			var N100km = (19 - Math.Floor((decimal)L1 / 5) * 5) - Math.Floor((decimal)L2 / 5);

			// skip grid letters to get numeric (easting/northing) part of ref
			var Digits = Regex.Split(gridRef.Substring(2).Trim(), @"\s+");
			// if e/n not whitespace separated, split half way
			if (Digits.Length == 1)
			{
				Digits = new string[] { Digits[0].Substring(0, Digits[0].Length / 2), Digits[0].Substring(Digits[0].Length / 2) };
			}

			// validation
			if (E100km < 0 || E100km > 6 || N100km < 0 || N100km > 12) { throw new ArgumentException("Invalid grid reference"); }
			if (Digits.Length != 2) { throw new ArgumentException("Invalid grid reference"); }
			if (Digits[0].Length != Digits[1].Length) { throw new ArgumentException("Invalid grid reference"); }

			// standardise to 10-digit refs (metres)
			Digits[0] = (Digits[0] + "00000").Substring(0, 5);
			Digits[1] = (Digits[1] + "00000").Substring(0, 5);

			var E = double.Parse(E100km + Digits[0]);
			var N = double.Parse(N100km + Digits[1]);

			return new OsGridRef(E, N);
		}
	}
}

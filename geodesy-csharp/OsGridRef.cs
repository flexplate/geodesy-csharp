using System;

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
		public LatLon ToLatLon(Datum datum)
		{
			double E = Easting;
			double N = Northing;

			double a = 6377563.396;
			double b = 6356256.909;              // Airy 1830 major & minor semi-axes
			double F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
			double φ0 = (49.0).ToRadians();
			double λ0 = (-2.0).ToRadians();  // NatGrid true origin is 49°N,2°W
			double N0 = -100000;
			double E0 = 400000;                     // northing & easting of true origin, metres
			double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
			double n = (a - b) / (a + b);
			double n2 = n * n;
			double n3 = n * n * n;         // n, n², n³

			double φ = φ0;
			double M = 0;
			do
			{
				φ = (N - N0 - M) / (a * F0) + φ;

				double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
				double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
				double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
				double Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
				M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

			} while (N - N0 - M >= 0.00001);  // ie until < 0.01mm

			double cosφ = Math.Cos(φ);
			double sinφ = Math.Sin(φ);
			double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
			double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature
			double η2 = ν / ρ - 1;                                    // eta = ?

			double tanφ = Math.Tan(φ);
			double tan2φ = tanφ * tanφ;
			double tan4φ = tan2φ * tan2φ;
			double tan6φ = tan4φ * tan2φ;
			double secφ = 1 / cosφ;
			double ν3 = ν * ν * ν;
			double ν5 = ν3 * ν * ν;
			double ν7 = ν5 * ν * ν;
			double VII = tanφ / (2 * ρ * ν);
			double VIII = tanφ / (24 * ρ * ν3) * (5 + 3 * tan2φ + η2 - 9 * tan2φ * η2);
			double IX = tanφ / (720 * ρ * ν5) * (61 + 90 * tan2φ + 45 * tan4φ);
			double X = secφ / ν;
			double XI = secφ / (6 * ν3) * (ν / ρ + 2 * tan2φ);
			double XII = secφ / (120 * ν5) * (5 + 28 * tan2φ + 24 * tan4φ);
			double XIIA = secφ / (5040 * ν7) * (61 + 662 * tan2φ + 1320 * tan4φ + 720 * tan6φ);

			double dE = (E - E0);
			double dE2 = dE * dE;
			double dE3 = dE2 * dE;
			double dE4 = dE2 * dE2;
			double dE5 = dE3 * dE2;
			double dE6 = dE4 * dE2;
			double dE7 = dE5 * dE2;
			φ = φ - VII * dE2 + VIII * dE4 - IX * dE6;
			double λ = λ0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;

			var Point = new LatLon(φ.ToDegrees(), λ.ToDegrees(), Datum.OSGB36);
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
				var eInt = Math.Floor(E);
				var eDec = E - eInt;
				var nInt = Math.Floor(N);
				var nDec = N - nInt;
				var ePad = eInt.ToString("000000") + (eDec > 0 ? eDec.ToString("000.000") : "");
				var nPad = nInt.ToString("000000") + (nDec > 0 ? nDec.ToString("000.000") : "");
				return ePad + "," + nPad;
			}

			// get the 100km-grid indices
			var e100k = Math.Floor(E / 100000);
			var n100k = Math.Floor(N / 100000);

			if (e100k < 0 || e100k > 6 || n100k < 0 || n100k > 12) return "";

			// translate those into numeric equivalents of the grid letters
			var l1 = (19 - n100k) - (19 - n100k) % 5 + Math.Floor((e100k + 10) / 5);
			var l2 = (19 - n100k) * 5 % 25 + e100k % 5;

			// compensate for skipped "I" and build grid letter-pairs
			if (l1 > 7) l1++;
			if (l2 > 7) l2++;
			string letterPair = Convert.ToChar((int)l1 + 65).ToString() + Convert.ToChar((int)l2 + 65);

			// strip 100km-grid indices from easting & northing, and reduce precision
			E = Math.Floor((E % 100000) / Math.Pow(10, 5 - digits / 2));
			N = Math.Floor((N % 100000) / Math.Pow(10, 5 - digits / 2));

			return string.Format("{0} {1} {2}", letterPair, E.ToFormattedString(digits / 2, 0), N.ToFormattedString(digits / 2, 0));
		}
	}
}

using System;
using System.Linq;

namespace geodesy
{
	public class LatLonEllipsoidal : LatLon
	{
		/// <summary>
		/// Creates lat/lon(polar) point with latitude & longitude values, on a specified datum.
		/// </summary>
		/// <param name="latitude">Geodetic latitude in degrees.</param>
		/// <param name="longitude">Longitude in degrees.</param>
		/// <param name="datum">Datum this point is defined within.</param>
		public LatLonEllipsoidal(double latitude, double longitude, Datum datum) : base(latitude,longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
			Datum = datum;
		}
		
		public Datum Datum { get; set; }

		/// <summary>
		/// /Converts ‘this’ lat/lon coordinate to new coordinate system.
		/// </summary>
		/// <param name="toDatum">Datum this coordinate is to be converted to.</param>
		/// <example>
		/// var pWGS84 = new LatLon(51.4778, -0.0016, Datum.WGS84);
		/// var pOSGB = pWGS84.convertDatum(Datum.OSGB36); // 51.4773°N, 000.0000°E
		/// </example>
		public LatLonEllipsoidal ConvertDatum(Datum toDatum)
		{
			LatLonEllipsoidal OldLatLon = this;
			double[] Transform = toDatum.Transform;
			bool UsingWgs84 = false;

			if (OldLatLon.Datum == Datum.WGS84)
			{
				// converting from WGS 84
				Transform = toDatum.Transform;
				UsingWgs84 = true;
			}
			if (toDatum == Datum.WGS84)
			{
				// converting to WGS 84; use inverse transform (don't overwrite original!)
				UsingWgs84 = true;
				Transform = new double[7];
				for (var p = 0; p < 7; p++)
				{
					Transform[p] = -OldLatLon.Datum.Transform[p];
				}
			}
			if (!UsingWgs84)
			{
				// neither this.datum nor toDatum are WGS84: convert this to WGS84 first
				OldLatLon = ConvertDatum(Datum.WGS84);
			}

			var OldCartesian = OldLatLon.ToCartesian();                // convert polar to cartesian...
			var NewCartesian = OldCartesian.ApplyTransform(Transform); // ...apply transform...
			var NewLatLon = NewCartesian.ToLatLonE(toDatum);           // ...and convert cartesian to polar

			return NewLatLon;

		}

		/// <summary>
		/// Converts ‘this’ point from (geodetic) latitude/longitude coordinates to (geocentric) cartesian (x/y/z) coordinates.
		/// </summary>
		public Vector3D ToCartesian()
		{
			var φ = this.Latitude.ToRadians();
			var λ = this.Longitude.ToRadians();
			var h = 0; // height above ellipsoid - not currently used
			var A = Datum.Ellipsoid.Major;
			var F = this.Datum.Ellipsoid.Flattening;

			var Sinφ = Math.Sin(φ);
			var Cosφ = Math.Cos(φ);
			var Sinλ = Math.Sin(λ);
			var Cosλ = Math.Cos(λ);

			var ESq = 2 * F - F * F;                      // 1st eccentricity squared ≡ (a²-b²)/a²
			var V = A / Math.Sqrt(1 - ESq * Sinφ * Sinφ); // radius of curvature in prime vertical

			var X = (V + h) * Cosφ * Cosλ;
			var Y = (V + h) * Cosφ * Sinλ;
			var Z = (V * (1 - ESq) + h) * Sinφ;

			var Point = new Vector3D(X, Y, Z);

			return Point;
		}

		public OsGridRef ToGridRef()
		{
			// if necessary convert to OSGB36 first
			if (Datum != Datum.OSGB36)
			{
				var Point = new LatLonEllipsoidal(Latitude, Longitude, Datum);
				Point = Point.ConvertDatum(Datum.OSGB36);
				Latitude = Point.Latitude;
				Longitude = Point.Longitude;
				Datum = Datum.OSGB36;
			}

			var φ = Latitude.ToRadians();
			var λ = Longitude.ToRadians();

			var A = 6377563.396;
			var B = 6356256.909;              // Airy 1830 major & minor semi-axes
			var F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
			var φ0 = (49.0).ToRadians();
			var λ0 = (-2.0).ToRadians();  // NatGrid true origin is 49°N,2°W
			var N0 = -100000;
			var E0 = 400000;                     // northing & easting of true origin, metres
			var E2 = 1 - (B * B) / (A * A);                          // eccentricity squared
			var N = (A - B) / (A + B);
			var N2 = N * N;
			var N3 = N * N * N;         // n, n², n³

			var Cosφ = Math.Cos(φ);
			var Sinφ = Math.Sin(φ);
			var ν = A * F0 / Math.Sqrt(1 - E2 * Sinφ * Sinφ);            // nu = transverse radius of curvature
			var ρ = A * F0 * (1 - E2) / Math.Pow(1 - E2 * Sinφ * Sinφ, 1.5); // rho = meridional radius of curvature
			var η2 = ν / ρ - 1;                                    // eta = ?

			var Ma = (1 + N + 1.25 * N2 + 1.25 * N3) * (φ - φ0);
			var Mb = (3 * N + 3 * N * N + 2.625 * N3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
			var Mc = (1.875 * N2 + 1.875 * N3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
			var Md = (35 / 24) * N3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
			var M = B * F0 * (Ma - Mb + Mc - Md);              // meridional arc

			var Cos3φ = Cosφ * Cosφ * Cosφ;
			var Cos5φ = Cos3φ * Cosφ * Cosφ;
			var Tan2φ = Math.Tan(φ) * Math.Tan(φ);
			var Tan4φ = Tan2φ * Tan2φ;

			var I = M + N0;
			var II = (ν / 2) * Sinφ * Cosφ;
			var III = (ν / 24) * Sinφ * Cos3φ * (5 - Tan2φ + 9 * η2);
			var IIIA = (ν / 720) * Sinφ * Cos5φ * (61 - 58 * Tan2φ + Tan4φ);
			var IV = ν * Cosφ;
			var V = (ν / 6) * Cos3φ * (ν / ρ - Tan2φ);
			var VI = (ν / 120) * Cos5φ * (5 - 18 * Tan2φ + Tan4φ + 14 * η2 - 58 * Tan2φ * η2);

			var Δλ = λ - λ0;
			var Δλ2 = Δλ * Δλ;
			var Δλ3 = Δλ2 * Δλ;
			var Δλ4 = Δλ3 * Δλ;
			var Δλ5 = Δλ4 * Δλ;
			var Δλ6 = Δλ5 * Δλ;

			var North = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;
			var East = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

			North = Math.Round(North, 3); //North.toFixed(3)); // round to mm precision
			East = Math.Round(East, 3);

			return new OsGridRef(East, North); // gets truncated to SW corner of 1m grid square
		}
		
	}
}
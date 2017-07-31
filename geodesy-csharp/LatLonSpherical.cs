using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geodesy
{
	class LatLonSpherical : LatLon
	{
		public LatLonSpherical(double latitude, double longitude) : base(latitude, longitude) { }

		/// <summary>
		/// Return distance from this point to destination point (using haversine formula)
		/// </summary>
		/// <param name="point">Latitude/longitude of destination point.</param>
		/// <param name="radius">(Mean) radius of earth (defaults to radius in metres).</param>
		/// <returns>Distance between this point and destination point, in same units as radius.</returns>
		public double DistanceTo(LatLon point, double radius = 6371e3)
		{
			var R = radius;
			double φ1 = Latitude.ToRadians();
			double λ1 = Longitude.ToRadians();
			double φ2 = point.Latitude.ToRadians();
			double λ2 = point.Longitude.ToRadians();
			var Δφ = φ2 - φ1;
			var Δλ = λ2 - λ1;

			var A = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
			var C = 2 * Math.Atan2(Math.Sqrt(A), Math.Sqrt(1 - A));
			var D = R * C;

			return D;
		}

		/// <summary>
		/// Returns the (initial) bearing from ‘this’ point to destination point.
		/// </summary>
		/// <param name="point">Latitude/longitude of destination point.</param>
		/// <returns>Initial bearing in degrees from north.</returns>
		public double BearingTo(LatLon point)
		{
			double φ1 = Latitude.ToRadians();
			double φ2 = point.Latitude.ToRadians();
			var Δλ = (point.Longitude - this.Longitude).ToRadians();

			// see http://mathforum.org/library/drmath/view/55417.html
			var y = Math.Sin(Δλ) * Math.Cos(φ2);
			var x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);
			var θ = Math.Atan2(y, x);

			return (θ.ToDegrees() + 360) % 360;
		}

		/// <summary>
		/// Returns final bearing arriving at destination destination point from this point; the final bearing
		/// will differ from the initial bearing by varying degrees according to distance and latitude.
		/// </summary>
		/// <param name="point">Destination point.</param>
		/// <returns>Final bearing in degrees from north.</returns>
		public double FinalBearingTo(LatLonSpherical point)
		{
			return (point.BearingTo(this) + 180) % 360;
		}

		/// <summary>
		/// Returns the midpoint between this point and the supplied point.
		/// </summary>
		/// <param name="point">Destination point.</param>
		/// <returns>Midpoint between this point and the supplied point.</returns>
		public LatLon MidpointTo(LatLon point)
		{
			var φ1 = Latitude.ToRadians();
			var λ1 = Longitude.ToRadians();
			var φ2 = point.Latitude.ToRadians();
			var Δλ = (point.Longitude - this.Longitude).ToRadians();

			var Bx = Math.Cos(φ2) * Math.Cos(Δλ);
			var By = Math.Cos(φ2) * Math.Sin(Δλ);

			var X = Math.Sqrt((Math.Cos(φ1) + Bx) * (Math.Cos(φ1) + Bx) + By * By);
			var Y = Math.Sin(φ1) + Math.Sin(φ2);
			var φ3 = Math.Atan2(Y, X);

			var λ3 = λ1 + Math.Atan2(By, Math.Cos(φ1) + Bx);

			return new LatLon(φ3.ToDegrees(), (λ3.ToDegrees() + 540) % 360 - 180); // normalise to −180..+180°
		}
	}
}

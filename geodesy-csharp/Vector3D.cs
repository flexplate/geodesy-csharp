using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geodesy_csharp
{
    public class Vector3D
    {
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D ApplyTransform(double[] transform)
        {
            // this point
            var X1 = this.X;
            var Y1 = this.Y;
            var Z1 = this.Z;

            // transform parameters
            var Tx = transform[0];                      // x-shift
            var Ty = transform[1];                      // y-shift
            var Tz = transform[2];                      // z-shift
            var S1 = transform[3] / 1e6 + 1;            // scale: normalise parts-per-million to (s+1)
            var Rx = (transform[4] / 3600).ToRadians(); // x-rotation: normalise arcseconds to radians
            var Ry = (transform[5] / 3600).ToRadians(); // y-rotation: normalise arcseconds to radians
            var Rz = (transform[6] / 3600).ToRadians(); // z-rotation: normalise arcseconds to radians

            // apply transform
            var X2 = Tx + X1 * S1 - Y1 * Rz + Z1 * Ry;
            var Y2 = Ty + X1 * Rz + Y1 * S1 - Z1 * Rx;
            var Z2 = Tz - X1 * Ry + Y1 * Rx + Z1 * S1;

            return new Vector3D(X2, Y2, Z2);
        }

        public LatLon ToLatLonE(Datum toDatum)
        {
            var Major = toDatum.Ellipsoid.Major;
            var Minor = toDatum.Ellipsoid.Minor;
            var Flattening = toDatum.Ellipsoid.Flattening;

            var E2 = 2 * Flattening - Flattening * Flattening;   // 1st eccentricity squared ≡ (a²-b²)/a²
            var ε2 = E2 / (1 - E2); // 2nd eccentricity squared ≡ (a²-b²)/b²
            var P = Math.Sqrt(X * X + Y * Y); // distance from minor axis
            var R = Math.Sqrt(P * P + Z * Z); // polar radius

            // parametric latitude (Bowring eqn 17, replacing tanβ = z·a / p·b)
            var Tanβ = (Minor * Z) / (Major * P) * (1 + ε2 * Minor / R);
            var Sinβ = Tanβ / Math.Sqrt(1 + Tanβ * Tanβ);
            var Cosβ = Sinβ / Tanβ;

            // geodetic latitude (Bowring eqn 18: tanφ = z+ε²bsin³β / p−e²cos³β)
            var φ = Math.Atan2(Z + ε2 * Minor * Sinβ * Sinβ * Sinβ, P - E2 * Major * Cosβ * Cosβ * Cosβ);

            // longitude
            var λ = Math.Atan2(Y, X);

            // height above ellipsoid (Bowring eqn 7) [not currently used]
            var Sinφ = Math.Sin(φ);
            var Cosφ = Math.Cos(φ);
            var V = Major / Math.Sqrt(1 - E2 * Sinφ * Sinφ); // length of the normal terminated by the minor axis
            var H = P * Cosφ + Z * Sinφ - (Major * Major / V);

            var point = new LatLon(φ.ToDegrees(), λ.ToDegrees(), toDatum);

            return point;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geodesy_csharp
{
    public class OsGridRef
    {
        public OsGridRef(int easting, int northing)
        {
            Easting = easting;
            Northing = northing;
        }

        public int Easting { get; set; }
        public int Northing { get; set; }


        public LatLon ToLatLon(OsGridRef gridRef, Datum datum)
        {
            var E = gridRef.Easting;
            var N = gridRef.Northing;

            var a = 6377563.396;
            var b = 6356256.909;              // Airy 1830 major & minor semi-axes
            var F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
            var φ0 = (49.0).ToRadians();
            var λ0 = (-2.0).ToRadians();      // NatGrid true origin is 49°N,2°W
            var N0 = -100000;
            var E0 = 400000;                     // northing & easting of true origin, metres
            var e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
            var n = (a - b) / (a + b);
            var n2 = n * n;
            var n3 = n * n * n;         // n, n², n³

            var φ = φ0;
            double M = 0;

            do
            {
                φ = (N - N0 - M) / (a * F0) + φ;

                var Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
                var Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
                var Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
                var Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
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

            var point = new LatLon(φ.ToDegrees(), λ.ToDegrees(), Datum.OSGB36);
            if (datum != Datum.OSGB36) point = point.ConvertDatum(datum);

            return point;
        }

    }
}

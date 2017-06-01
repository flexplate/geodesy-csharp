using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geodesy_csharp
{
    public class Datum
    {
        public Ellipsoid Ellipsoid;
        public double[] Transform;  // formatted like so: [tx, ty, tz, s, rx, ry, rz]

        internal static readonly Datum ED50 = new Datum { Ellipsoid = Ellipsoid.WGS84, Transform = new double[] { 89.5, 93.8, 123.1, -1.2, 0.0, 0.0, 0.156 } };
        internal static readonly Datum rl1975 = new Datum { Ellipsoid = Ellipsoid.AiryModified, Transform = new double[] { -482.530, 130.596, -564.557, -8.150, -1.042, -0.214, -0.631 } };
        internal static readonly Datum NAD27 = new Datum { Ellipsoid = Ellipsoid.Clarke1866, Transform = new double[] { 8, -160, -176, 0, 0, 0, 0 } };
        internal static readonly Datum NAD83 = new Datum { Ellipsoid = Ellipsoid.GRS80, Transform = new double[] { 1.004, -1.910, -0.515, -0.0015, 0.0267, 0.00034, 0.011 } };
        internal static readonly Datum NTF = new Datum { Ellipsoid = Ellipsoid.Clarke1880IGN, Transform = new double[] { 168, 60, -320, 0, 0, 0, 0 } };
        internal static readonly Datum OSGB36 = new Datum { Ellipsoid = Ellipsoid.Airy1830, Transform = new double[] { -446.448, 125.157, -542.060, 20.4894, -0.1502, -0.2470, -0.8421 } };
        internal static readonly Datum Potsdam = new Datum { Ellipsoid = Ellipsoid.Bessel1841, Transform = new double[] { -582, -105, -414, -8.3, 1.04, 0.35, -3.08 } };
        internal static readonly Datum TokyoJapan = new Datum { Ellipsoid = Ellipsoid.Bessel1841, Transform = new double[] { 148, -507, -685, 0, 0, 0, 0 } };
        internal static readonly Datum WGS72 = new Datum { Ellipsoid = Ellipsoid.WGS72, Transform = new double[] { 0, 0, -4.5, -0.22, 0, 0, 0.554 } };
        internal static readonly Datum WGS84 = new Datum { Ellipsoid = Ellipsoid.WGS84, Transform = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 } };

    }
}

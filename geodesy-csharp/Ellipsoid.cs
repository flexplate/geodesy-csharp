using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geodesy_csharp
{
    public class Ellipsoid
    {
        public double Major { get; set; }
        public double Minor { get; set; }
        public double Flattening { get; set; }

        /**
         * Ellipsoid parameters; major axis (a), minor axis (b), and flattening (f) for each ellipsoid.
         */
        internal static readonly Ellipsoid WGS84 = new Ellipsoid { Major = 6378137, Minor = 6356752.314245, Flattening = 1 / 298.257223563 };
        internal static readonly Ellipsoid Airy1830 = new Ellipsoid { Major = 6377563.396, Minor = 6356256.909, Flattening = 1 / 299.3249646 };
        internal static readonly Ellipsoid AiryModified = new Ellipsoid { Major = 6377340.189, Minor = 6356034.448, Flattening = 1 / 299.3249646 };
        internal static readonly Ellipsoid Bessel1841 = new Ellipsoid { Major = 6377397.155, Minor = 6356078.962818, Flattening = 1 / 299.1528128 };
        internal static readonly Ellipsoid Clarke1866 = new Ellipsoid { Major = 6378206.4, Minor = 6356583.8, Flattening = 1 / 294.978698214 };
        internal static readonly Ellipsoid Clarke1880IGN = new Ellipsoid { Major = 6378249.2, Minor = 6356515.0, Flattening = 1 / 293.466021294 };
        internal static readonly Ellipsoid GRS80 = new Ellipsoid { Major = 6378137, Minor = 6356752.314140, Flattening = 1 / 298.257222101 };
        internal static readonly Ellipsoid Intl1924 = new Ellipsoid { Major = 6378388, Minor = 6356911.946, Flattening = 1 / 297 }; // aka Hayford
        internal static readonly Ellipsoid WGS72 = new Ellipsoid { Major = 6378135, Minor = 6356750.5, Flattening = 1 / 298.26 };
    }

}

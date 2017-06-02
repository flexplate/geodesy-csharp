namespace geodesy
{
    /// <summary>
    /// Datums; with associated ellipsoid, and Helmert transform parameters to convert from WGS 84 into
    /// given datum.
    /// Note that precision of various datums will vary, and WGS-84 (original) is not defined to be
    /// accurate to better than ±1 metre.No transformation should be assumed to be accurate to better
    /// than a meter; for many datums somewhat less.
    /// </summary>
    public class Datum
    {
        public Ellipsoid Ellipsoid;
        public double[] Transform;  // formatted like so: [tx, ty, tz, s, rx, ry, rz]

        public static readonly Datum ED50 = new Datum { Ellipsoid = Ellipsoid.WGS84, Transform = new double[] { 89.5, 93.8, 123.1, -1.2, 0.0, 0.0, 0.156 } };
        public static readonly Datum rl1975 = new Datum { Ellipsoid = Ellipsoid.AiryModified, Transform = new double[] { -482.530, 130.596, -564.557, -8.150, -1.042, -0.214, -0.631 } };
        public static readonly Datum NAD27 = new Datum { Ellipsoid = Ellipsoid.Clarke1866, Transform = new double[] { 8, -160, -176, 0, 0, 0, 0 } };
        public static readonly Datum NAD83 = new Datum { Ellipsoid = Ellipsoid.GRS80, Transform = new double[] { 1.004, -1.910, -0.515, -0.0015, 0.0267, 0.00034, 0.011 } };
        public static readonly Datum NTF = new Datum { Ellipsoid = Ellipsoid.Clarke1880IGN, Transform = new double[] { 168, 60, -320, 0, 0, 0, 0 } };
        public static readonly Datum OSGB36 = new Datum { Ellipsoid = Ellipsoid.Airy1830, Transform = new double[] { -446.448, 125.157, -542.060, 20.4894, -0.1502, -0.2470, -0.8421 } };
        public static readonly Datum Potsdam = new Datum { Ellipsoid = Ellipsoid.Bessel1841, Transform = new double[] { -582, -105, -414, -8.3, 1.04, 0.35, -3.08 } };
        public static readonly Datum TokyoJapan = new Datum { Ellipsoid = Ellipsoid.Bessel1841, Transform = new double[] { 148, -507, -685, 0, 0, 0, 0 } };
        public static readonly Datum WGS72 = new Datum { Ellipsoid = Ellipsoid.WGS72, Transform = new double[] { 0, 0, -4.5, -0.22, 0, 0, 0.554 } };
        public static readonly Datum WGS84 = new Datum { Ellipsoid = Ellipsoid.WGS84, Transform = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 } };
        
        /* sources:
         * - ED50:          www.gov.uk/guidance/oil-and-gas-petroleum-operations-notices#pon-4
         * - Irl1975:       www.osi.ie/wp-content/uploads/2015/05/transformations_booklet.pdf
         *   ... note: many sources have opposite sign to rotations - to be checked!
         * - NAD27:         en.wikipedia.org/wiki/Helmert_transformation
         * - NAD83: (2009); www.uvm.edu/giv/resources/WGS84_NAD83.pdf
         *   ... note: functionally ≡ WGS84 - if you *really* need to convert WGS84<->NAD83, you need more knowledge than this!
         * - NTF:           Nouvelle Triangulation Francaise geodesie.ign.fr/contenu/fichiers/Changement_systeme_geodesique.pdf
         * - OSGB36:        www.ordnancesurvey.co.uk/docs/support/guide-coordinate-systems-great-britain.pdf
         * - Potsdam:       kartoweb.itc.nl/geometrics/Coordinate%20transformations/coordtrans.html
         * - TokyoJapan:    www.geocachingtoolbox.com?page=datumEllipsoidDetails
         * - WGS72:         www.icao.int/safety/pbn/documentation/eurocontrol/eurocontrol wgs 84 implementation manual.pdf
         *
         * more transform parameters are available from earth-info.nga.mil/GandG/coordsys/datums/NATO_DT.pdf,
         * www.fieldenmaps.info/cconv/web/cconv_params.js
         */

    }
}

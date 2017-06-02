using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geodesy;

namespace geodesy.test
{
    [TestClass]
    public class OsGridRefTests
    {
        [TestMethod]
        public void C1E()
        {
            var Osgb = new LatLon(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
            var GridRef = Osgb.ToGridRef();
            Assert.AreEqual(651409.903, GridRef.Easting);
        }
        [TestMethod]
        public void C1N()
        {
            var Osgb = new LatLon(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
            var GridRef = Osgb.ToGridRef();
            Assert.AreEqual(313177.270, GridRef.Northing);
        }
        [TestMethod]
        public void C1RoundTrip()
        {
            var Osgb = new LatLon(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
            var GridRef = Osgb.ToGridRef();
            var OsGb2 = GridRef.ToLatLon(Datum.OSGB36);
        }

		[TestMethod]
		public void C2()
		{
			var GridRef = new OsGridRef(651409.903, 313177.270);
			var Osgb = GridRef.ToLatLon(Datum.OSGB36);
			Assert.AreEqual("52°" + DMS.Separator+ "39′" + DMS.Separator + "27.2531″N, 001°" + DMS.Separator + "43′" + DMS.Separator + "04.5177″E", Osgb.ToString("dms", 4));
		}
		[TestMethod]
		public void C2ERoundTrip()
		{
			var GridRef = new OsGridRef(651409.903, 313177.270);
			var Osgb = GridRef.ToLatLon(Datum.OSGB36);
			var GridRef2 = Osgb.ToGridRef();
			Assert.AreEqual(651409.903, GridRef2.Easting);
		}
		[TestMethod]
		public void C2NRoundTrip()
		{
			var GridRef = new OsGridRef(651409.903, 313177.270);
			var Osgb = GridRef.ToLatLon(Datum.OSGB36);
			var GridRef2 = Osgb.ToGridRef();
			Assert.AreEqual(313177.270, GridRef2.Northing);
		}

		#region limits
		[TestMethod]
		public void SWRegular()
		{
			Assert.AreEqual("SV 00000 00000", new OsGridRef(0, 0).ToString());
		}
		[TestMethod]
		public void NERegular()
		{
			Assert.AreEqual("JM 99999 99999", new OsGridRef(699999, 1299999).ToString());
		}
		[TestMethod]
		public void SWNumeric()
		{
			Assert.AreEqual("000000,000000", new OsGridRef(0, 0).ToString(0));
		}
		[TestMethod]
		public void NENumeric()
		{
			Assert.AreEqual("699999,1299999", new OsGridRef(699999, 1299999).ToString(0));
		}

		#endregion

		#region WGS84
		[TestMethod]
		public void ConvertWgs84ToOsgb36()
		{
			var GreenwichWGS84 = new LatLon(51.4778, -0.0016, Datum.WGS84); // default WGS84
			var GreenwichOSGB36 = GreenwichWGS84.ConvertDatum(Datum.OSGB36);
			Assert.AreEqual("51.4773°" + DMS.Separator + "N, 000.0000°" + DMS.Separator + "E", GreenwichOSGB36.ToString("d", 4));
		}
		[TestMethod]
		public void ConvertOsgb36ToWgs84()
		{
			var GreenwichWGS84 = new LatLon(51.4778, -0.0016, Datum.WGS84); // default WGS84
			var GreenwichOSGB36 = GreenwichWGS84.ConvertDatum(Datum.OSGB36);
			Assert.AreEqual("51.4778°"+DMS.Separator+ "N, 000.0016°" + DMS.Separator + "W", GreenwichOSGB36.ConvertDatum(Datum.WGS84).ToString("d", 4));
		}
		#endregion
	}
}

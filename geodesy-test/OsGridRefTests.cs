using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace geodesy.test
{
	[TestClass]
	public class OsGridRefTests
	{
		[TestMethod]
		public void C1E()
		{
			var Osgb = new LatLonEllipsoidal(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
			var GridRef = Osgb.ToGridRef();
			Assert.AreEqual(651409.903, GridRef.Easting);
		}
		[TestMethod]
		public void C1N()
		{
			var Osgb = new LatLonEllipsoidal(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
			var GridRef = Osgb.ToGridRef();
			Assert.AreEqual(313177.270, GridRef.Northing);
		}
		[TestMethod]
		public void C1RoundTrip()
		{
			var Osgb = new LatLonEllipsoidal(DMS.ParseDMS("52°39′27.2531″N"), DMS.ParseDMS("1°43′4.5177″E"), Datum.OSGB36);
			var GridRef = Osgb.ToGridRef();
			var OsGb2 = GridRef.ToLatLon(Datum.OSGB36);
		}

		[TestMethod]
		public void C2()
		{
			var GridRef = new OsGridRef(651409.903, 313177.270);
			var Osgb = GridRef.ToLatLon(Datum.OSGB36);
			Assert.AreEqual("52°" + DMS.Separator + "39′" + DMS.Separator + "27.2531″N, 001°" + DMS.Separator + "43′" + DMS.Separator + "04.5177″E", Osgb.ToString("dms", 4));
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
			var GreenwichWGS84 = new LatLonEllipsoidal(51.4778, -0.0016, Datum.WGS84); // default WGS84
			var GreenwichOSGB36 = GreenwichWGS84.ConvertDatum(Datum.OSGB36);
			Assert.AreEqual("51.4773°" + DMS.Separator + "N, 000.0000°" + DMS.Separator + "E", GreenwichOSGB36.ToString("d", 4));
		}
		[TestMethod]
		public void ConvertOsgb36ToWgs84()
		{
			var GreenwichWGS84 = new LatLonEllipsoidal(51.4778, -0.0016, Datum.WGS84); // default WGS84
			var GreenwichOSGB36 = GreenwichWGS84.ConvertDatum(Datum.OSGB36);
			Assert.AreEqual("51.4778°" + DMS.Separator + "N, 000.0016°" + DMS.Separator + "W", GreenwichOSGB36.ConvertDatum(Datum.WGS84).ToString("d", 4));
		}
		#endregion

		#region Parse
		[TestMethod]
		public void Parse100kmOrigin()
		{
			Assert.AreEqual("SU 00000 00000", OsGridRef.Parse("SU00").ToString());
		}
		[TestMethod]
		public void Parse100kmOrigin2()
		{
			Assert.AreEqual("SU 00000 00000", OsGridRef.Parse("SU 0 0").ToString());
		}
		[TestMethod]
		public void ParseNoWhitespace()
		{
			Assert.AreEqual("SU 38700 14800", OsGridRef.Parse("SU387148").ToString());
		}
		[TestMethod]
		public void Parse6Digit()
		{
			Assert.AreEqual("SU 38700 14800", OsGridRef.Parse("SU 387 148").ToString());
		}
		[TestMethod]
		public void Parse10Digit()
		{
			Assert.AreEqual("SU 38700 14800", OsGridRef.Parse("SU 38700 14800").ToString());
		}
		[TestMethod]
		public void ParseNumeric()
		{
			Assert.AreEqual("SU 38700 14800", OsGridRef.Parse("438700,114800").ToString());
		}
		#endregion

		#region DG Round-trip
		[TestMethod]
		public void DgRoundTripOsgb36()
		{
			OsGridRef DgGridRef = OsGridRef.Parse("TQ 44359 80653");
			LatLonEllipsoidal DgOsgb = DgGridRef.ToLatLon(Datum.OSGB36);
			Assert.AreEqual(DgOsgb.ToGridRef().ToString(), DgGridRef.ToString());
		}
		[TestMethod]
		public void DgRoundTripOsgb36Numeric()
		{
			OsGridRef DgGridRef = OsGridRef.Parse("TQ 44359 80653");
			LatLonEllipsoidal DgOsgb = DgGridRef.ToLatLon(Datum.OSGB36);
			Assert.AreEqual("544359,180653", DgOsgb.ToGridRef().ToString(0));
		}
		#endregion

		#region Reverse Helmert Inaccuracy
		// reversing Helmert transform (OSGB->WGS->OSGB) introduces small error (≈ 3mm in UK), so WGS84
		// round-trip is not quite perfect: test needs to incorporate 3mm error to pass
		[TestMethod]
		public void DgRoundTripWgs84Numeric()
		{
			var DgGridRef = OsGridRef.Parse("TQ 44359 80653");
			var DgWgs = DgGridRef.ToLatLon(Datum.WGS84);
			Assert.AreEqual("544358.997,180653", DgWgs.ToGridRef().ToString(0));
		}
		#endregion
	}
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace geodesy.test
{
    [TestClass]
    public class DmsTest
    {
        #region Zero Degrees
        [TestMethod]
        public void Parse1()
        {
            Assert.AreEqual(0, DMS.ParseDMS("0.0°"));
        }
        [TestMethod]
        public void Parse2()
        {
            Assert.AreEqual(0, DMS.ParseDMS("0°"));
        }
        [TestMethod]
        public void Parse3()
        {
            Assert.AreEqual(0, DMS.ParseDMS("000 00 0"));
        }
        [TestMethod]
        public void Parse4()
        {
            Assert.AreEqual(0, DMS.ParseDMS("000°00′00″"));
        }
        [TestMethod]
        public void Parse5()
        {
            Assert.AreEqual(0, DMS.ParseDMS("000°00′00.0″"));
        }
        [TestMethod]
        public void Parse6()
        {
            Assert.AreEqual(0, DMS.ParseDMS("0"));
        }
        [TestMethod]
        public void Output1()
        {
            Assert.AreEqual("000.0000°", DMS.ToDMS(0, "d", null));
        }
        [TestMethod]
        public void Output2()
        {
            Assert.AreEqual("000°", DMS.ToDMS(0, "d", 0));
        }
        [TestMethod]
        public void Output3()
        {
            Assert.AreEqual("000°" + DMS.Separator + "00′" + DMS.Separator + "00″", DMS.ToDMS(0, "dms", 0));
        }
        [TestMethod]
        public void Output4()
        {
            Assert.AreEqual("000°" + DMS.Separator + "00′" + DMS.Separator + "00.00″", DMS.ToDMS(0, "dms", 2));
        }
        #endregion

        #region Parse Variations

        string[] Variations = {
                "45.76260",
                "45.76260 ",
                "45.76260°",
                "45°45.756′",
                "45° 45.756′",
                "45 45.756",
                "45°45′45.36″",
                "45º45\"45.36\"",
                "45°45’45.36”",
                "45 45 45.36 ",
                "45° 45′ 45.36″",
                "45º 45\" 45.36\"",
                "45° 45’ 45.36”",
            };

        [TestMethod]
        public void ParseVariations()
        {
            foreach (var v in Variations) { Assert.AreEqual(45.76260, DMS.ParseDMS(v)); }
        }
        [TestMethod]
        public void ParseVariationsMinus()
        {
            foreach (var v in Variations) { Assert.AreEqual(-45.76260, DMS.ParseDMS('-' + v)); }
        }
        [TestMethod]
        public void ParseVariationsNorth()
        {
            foreach (var v in Variations) { Assert.AreEqual(45.76260, DMS.ParseDMS(v + 'N')); }
        }
        [TestMethod]
        public void ParseVariationsSouth()
        {
            foreach (var v in Variations) { Assert.AreEqual(-45.76260, DMS.ParseDMS(v + 'S')); }
        }
        [TestMethod]
        public void ParseVariationsEast()
        {
            foreach (var v in Variations) { Assert.AreEqual(45.76260, DMS.ParseDMS(v + 'E')); }
        }
        [TestMethod]
        public void ParseVariationsWest()
        {
            foreach (var v in Variations) { Assert.AreEqual(-45.76260, DMS.ParseDMS(v + 'W')); }
        }

        [TestMethod]
        public void ParseWhiteSpaceWrapped()
        {
            Assert.AreEqual(45.76260, DMS.ParseDMS(" 45°45′45.36″ "));
        }
        #endregion

        #region Out Of Range
        [TestMethod]
        public void Parse185()
        {
            Assert.AreEqual(185, DMS.ParseDMS("185"));
        }
        [TestMethod]
        public void Parse365()
        {
            Assert.AreEqual(365, DMS.ParseDMS("365"));
        }
        [TestMethod]
        public void ParseMinus185()
        {
            Assert.AreEqual(-185, DMS.ParseDMS("-185"));
        }
        [TestMethod]
        public void ParseMinus365()
        {
            Assert.AreEqual(-365, DMS.ParseDMS("-365"));
        }
        #endregion

        #region Output Variations
        [TestMethod]
        public void OutputDms()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45′" + DMS.Separator + "45″", DMS.ToDMS(45.76260, null, null));
        }
        [TestMethod]
        public void OutputDmsD()
        {
            Assert.AreEqual("045.7626°", DMS.ToDMS(45.76260, "d", null));
        }
        [TestMethod]
        public void OutputDmsDM()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45.76′", DMS.ToDMS(45.76260, "dm", null));
        }
        [TestMethod]
        public void OutputDmsDms()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45′" + DMS.Separator + "45″", DMS.ToDMS(45.76260, "dms", null));
        }
        [TestMethod]
        public void OutputDm6()
        {
            Assert.AreEqual("045.762600°", DMS.ToDMS(45.76260, "d", 6));
        }
        [TestMethod]
        public void OutputDm4()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45.7560′", DMS.ToDMS(45.76260, "dm", 4));
        }
        [TestMethod]
        public void OutputDms2()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45′" + DMS.Separator + "45.36″", DMS.ToDMS(45.76260, "dms", 2));
        }
        [TestMethod]
        public void OutputDmsXxx()
        {
            Assert.AreEqual("045°" + DMS.Separator + "45′" + DMS.Separator + "45″", DMS.ToDMS(45.76260, "xxx", null));
        }
        [TestMethod]
        public void OutputDmsXxx6()
        {
            Assert.AreEqual("045.762600°", DMS.ToDMS(45.76260, "xxx", 6));
        }
        #endregion

        #region Compass Points
        [TestMethod]
        public void North1()
        {
            Assert.AreEqual("N", DMS.CompassPoint(1));
        }
        [TestMethod]
        public void North0()
        {
            Assert.AreEqual("N", DMS.CompassPoint(0));
        }
        [TestMethod]
        public void NorthMinus1()
        {
            Assert.AreEqual("N", DMS.CompassPoint(-1));
        }
        [TestMethod]
        public void North359()
        {
            Assert.AreEqual("N", DMS.CompassPoint(359));
        }
        [TestMethod]
        public void NorthNorthEast24()
        {
            Assert.AreEqual("NNE", DMS.CompassPoint(24));
        }
        [TestMethod]
        public void OneDigitNorth24()
        {
            Assert.AreEqual("N", DMS.CompassPoint(24, 1));
        }
        [TestMethod]
        public void TwoDigitNorthEast24()
        {
            Assert.AreEqual("NE", DMS.CompassPoint(24, 2));
        }
        [TestMethod]
        public void ThreeDigiNorthNorthEast24()
        {
            Assert.AreEqual("NNE", DMS.CompassPoint(24, 3));
        }
        [TestMethod]
        public void SouthWest226()
        {
            Assert.AreEqual("SW", DMS.CompassPoint(226));
        }
        [TestMethod]
        public void OneDigitWest226()
        {
            Assert.AreEqual("W", DMS.CompassPoint(226, 1));
        }
        [TestMethod]
        public void TwoDigitSouthWest226()
        {
            Assert.AreEqual("SW", DMS.CompassPoint(226, 2));
        }
        [TestMethod]
        public void ThreeDigitSouthWest226()
        {
            Assert.AreEqual("SW", DMS.CompassPoint(226, 3));
        }
        [TestMethod]
        public void WestSouthWest237()
        {
            Assert.AreEqual("WSW", DMS.CompassPoint(237));
        }
        [TestMethod]
        public void OneDigitWest237()
        {
            Assert.AreEqual("W", DMS.CompassPoint(237, 1));
        }
        [TestMethod]
        public void TwoDigitSouthWest237()
        {
            Assert.AreEqual("SW", DMS.CompassPoint(237, 2));
        }
        [TestMethod]
        public void ThreeDigitWestSouthWest237()
        {
            Assert.AreEqual("WSW", DMS.CompassPoint(237, 3));
        }
        #endregion

        #region Misc
        [TestMethod]
        public void ToLatNum() { Assert.AreEqual("51°" + DMS.Separator + "12′" + DMS.Separator + "00″" + DMS.Separator + "N", DMS.ToLat(51.2, "dms")); }

        [TestMethod]
        public void ToLatStr() { Assert.AreEqual("51°" + DMS.Separator + "12′" + DMS.Separator + "00″" + DMS.Separator + "N", DMS.ToLat("51.2", "dms")); }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ToLatxxx() { Assert.AreEqual("–", DMS.ToLat("xxx", "dms")); }

        [TestMethod]
        public void ToLonNum() { Assert.AreEqual("000°" + DMS.Separator + "19′" + DMS.Separator + "48″" + DMS.Separator + "E", DMS.ToLon(0.33, "dms")); }

        [TestMethod]
        public void ToLonStr() { Assert.AreEqual("000°" + DMS.Separator + "19′" + DMS.Separator + "48″" + DMS.Separator + "E", DMS.ToLon("0.33", "dms")); }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ToLonxxx() { Assert.AreEqual("–", DMS.ToLon("xxx", "dms")); }

        [TestMethod]
        public void ToDMSrndUpD() { Assert.AreEqual("051.2000°", DMS.ToDMS(51.19999999999999, "d")); }

        [TestMethod]
        public void ToDMSrndUpDM() { Assert.AreEqual("051°" + DMS.Separator + "12.00′", DMS.ToDMS(51.19999999999999, "dm")); }

        [TestMethod]
        public void ToDMSrndUpDMS() { Assert.AreEqual("051°" + DMS.Separator + "12′" + DMS.Separator + "00″", DMS.ToDMS(51.19999999999999, "dms")); }

        [TestMethod]
        public void ToBrng() { Assert.AreEqual("001°" + DMS.Separator + "00′" + DMS.Separator + "00″", DMS.ToBearing(1)); }
        #endregion

        #region Parse Failures
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Parse0000()
        {
            DMS.ParseDMS("0 0 0 0");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ParseXXX()
        {
            DMS.ParseDMS("xxx");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseEmpty()
        {
            DMS.ParseDMS("");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNull()
        {
            DMS.ParseDMS(null);
        }
        #endregion
    }
}

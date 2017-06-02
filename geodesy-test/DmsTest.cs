using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geodesy;

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

		
	}
}

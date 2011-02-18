using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbNumDef
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void DefaultMinMaxTest()
		{
			DbNumDef numDef = new DbNumDef ();
			
			numDef.DigitPrecision = 3;
			numDef.DigitShift = 0;
			
			Assert.AreEqual (false, numDef.IsMinMaxDefined);
			Assert.AreEqual (true, numDef.IsDigitDefined);
			Assert.AreEqual (999M, numDef.MaxValue);
			Assert.AreEqual (-999M, numDef.MinValue);
			
			numDef.DigitPrecision = 6;
			numDef.DigitShift = 3;
			
			Assert.AreEqual (999.999M, numDef.MaxValue);
			Assert.AreEqual (-999.999M, numDef.MinValue);
		}


		[TestMethod]
		public void DefaultPrecisionShiftTest()
		{
			DbNumDef numDef = new DbNumDef ();
			
			numDef.MinValue = 0;
			numDef.MaxValue = 9;
			
			Assert.AreEqual (false, numDef.IsDigitDefined);
			Assert.AreEqual (true, numDef.IsMinMaxDefined);
			Assert.AreEqual (1, numDef.DigitPrecision);
			Assert.AreEqual (0, numDef.DigitShift);
			
			Assert.AreEqual (0, numDef.ToDecimalRange ().Minimum);
			Assert.AreEqual (9, numDef.ToDecimalRange ().Maximum);
			Assert.AreEqual (1.0M, numDef.ToDecimalRange ().Resolution);
			
			numDef.MinValue = 0.01M;
			numDef.MaxValue = 10.00M;
			
			Assert.AreEqual (false, numDef.IsDigitDefined);
			Assert.AreEqual (4, numDef.DigitPrecision);
			Assert.AreEqual (2, numDef.DigitShift);
			
			Assert.AreEqual (0.01M, numDef.ToDecimalRange ().Minimum);
			Assert.AreEqual (10.00M, numDef.ToDecimalRange ().Maximum);
			Assert.AreEqual (0.01M, numDef.ToDecimalRange ().Resolution);
			
			numDef.MinValue = -9.999M;
			numDef.MaxValue =  9.999M;
			
			Assert.AreEqual (false, numDef.IsDigitDefined);
			Assert.AreEqual (4, numDef.DigitPrecision);
			Assert.AreEqual (3, numDef.DigitShift);
			
			Assert.AreEqual (-9.999M, numDef.ToDecimalRange ().Minimum);
			Assert.AreEqual (9.999M, numDef.ToDecimalRange ().Maximum);
			Assert.AreEqual (0.001M, numDef.ToDecimalRange ().Resolution);
		}


		[TestMethod]
		public void InvalidCaseTest()
		{
			DbNumDef numDef = new DbNumDef (1,0);
			
			numDef.MinValue = -1;
			numDef.MaxValue = 99;
			
			Assert.AreEqual (true, numDef.IsDigitDefined);
			Assert.AreEqual (true, numDef.IsMinMaxDefined);
			Assert.AreEqual (1, numDef.DigitPrecision);
			Assert.AreEqual (0, numDef.DigitShift);
			Assert.AreEqual (false, numDef.IsValid);
		}


		[TestMethod]
		public void MinimumBitsIntTest()
		{
			DbNumDef numDdef = new DbNumDef (3, 0);
			
			numDdef.MinValue = 0;
			numDdef.MaxValue = 127;
			Assert.AreEqual (7, numDdef.MinimumBits);
			
			numDdef.MinValue = -127;
			numDdef.MaxValue =  127;
			Assert.AreEqual (8, numDdef.MinimumBits);
			
			numDdef.MinValue = -128;
			numDdef.MaxValue =  127;
			Assert.AreEqual (8, numDdef.MinimumBits);
			
			numDdef.MinValue = -128;
			numDdef.MaxValue =  128;
			Assert.AreEqual (9, numDdef.MinimumBits);
			
			numDdef.MinValue = 1;
			numDdef.MaxValue = 256;
			Assert.AreEqual (8, numDdef.MinimumBits);
			
			numDdef.MinValue = 500;
			numDdef.MaxValue = 515;
			Assert.AreEqual (4, numDdef.MinimumBits);
		}


		[TestMethod]
		public void MinimumBitsFracTest()
		{
			DbNumDef numDef = new DbNumDef (5, 2);
			
			numDef.MinValue =   0.00M;
			numDef.MaxValue = 127.00M; //	127.00 => BIN(11000110011100) * 0.01
			Assert.AreEqual (14, numDef.MinimumBits);
			
			numDef.MinValue = -1.27M;
			numDef.MaxValue =  1.27M;
			Assert.AreEqual (8, numDef.MinimumBits);
			
			numDef.MinValue = -1.28M;
			numDef.MaxValue =  1.27M;
			Assert.AreEqual (8, numDef.MinimumBits);
			
			numDef.MinValue = -1.28M;
			numDef.MaxValue =  1.28M;
			Assert.AreEqual (9, numDef.MinimumBits);
			
			numDef.MinValue = 0.01M;
			numDef.MaxValue = 2.56M;
			Assert.AreEqual (8, numDef.MinimumBits);
			
			numDef.MinValue = 5.00M;
			numDef.MaxValue = 5.15M;
			Assert.AreEqual (4, numDef.MinimumBits);
		}


		[TestMethod]
		public void DbRawTypesTest()
		{
			DbNumDef numDef = DbNumDef.FromRawType (DbRawType.Int16);
			
			Assert.IsTrue (numDef.CheckCompatibility (System.Int16.MinValue));
			Assert.IsTrue (numDef.CheckCompatibility (System.Int16.MaxValue));
			Assert.IsFalse (numDef.CheckCompatibility (System.Int16.MinValue-1));
			Assert.IsFalse (numDef.CheckCompatibility (System.Int16.MaxValue+1));
			
			numDef = DbNumDef.FromRawType (DbRawType.Int32);
			
			Assert.IsTrue (numDef.CheckCompatibility (System.Int32.MinValue));
			Assert.IsTrue (numDef.CheckCompatibility (System.Int32.MaxValue));
			Assert.IsFalse (numDef.CheckCompatibility (System.Int32.MinValue-1L));
			Assert.IsFalse (numDef.CheckCompatibility (System.Int32.MaxValue+1L));
			
			numDef = DbNumDef.FromRawType (DbRawType.Int64);
			
			Assert.IsTrue (numDef.CheckCompatibility (System.Int64.MinValue));
			Assert.IsTrue (numDef.CheckCompatibility (System.Int64.MaxValue));
			Assert.AreEqual (64, numDef.MinimumBits);
			
			numDef = DbNumDef.FromRawType (DbRawType.SmallDecimal);
			
			Assert.IsTrue (numDef.CheckCompatibility (0.000000001M));
			Assert.IsFalse (numDef.CheckCompatibility (0.0000000001M));
			Assert.IsTrue (numDef.CheckCompatibility ( 999999999M));
			Assert.IsFalse (numDef.CheckCompatibility (1000000000M));
			
			numDef = DbNumDef.FromRawType (DbRawType.LargeDecimal);
			
			Assert.IsTrue (numDef.CheckCompatibility (0.001M));
			Assert.IsFalse (numDef.CheckCompatibility (0.0001M));
			Assert.IsTrue (numDef.CheckCompatibility ( 999999999999999M));
			Assert.IsFalse (numDef.CheckCompatibility (1000000000000000M));
		}


		[TestMethod]
		public void ConvertTest1()
		{
			DbNumDef numDef = new DbNumDef (5, 2, 5.00M, 5.15M);

			Assert.AreEqual (0, numDef.ConvertToInt64 (numDef.ConvertFromInt64 (0)));
			Assert.AreEqual (15, numDef.ConvertToInt64 (numDef.ConvertFromInt64 (15)));

			Assert.AreEqual (5.00M, numDef.ConvertFromInt64 (numDef.ConvertToInt64 (5.00M)));
			Assert.AreEqual (5.15M, numDef.ConvertFromInt64 (numDef.ConvertToInt64 (5.15M)));

			Assert.AreEqual (-8, numDef.ConvertToInt64 (5.00M));
			Assert.AreEqual (7, numDef.ConvertToInt64 (5.15M));

			Assert.AreEqual (5.00M, numDef.ConvertFromInt64 (-8));
			Assert.AreEqual (5.15M, numDef.ConvertFromInt64 (7));
		}


		[TestMethod]
		public void ConvertTest2()
		{
			DbNumDef numDef = new DbNumDef (5, 2, 1.00M, 0.01M);

			Assert.AreEqual (0, numDef.ConvertToInt64 (numDef.ConvertFromInt64 (0)));
			Assert.AreEqual (40, numDef.ConvertToInt64 (numDef.ConvertFromInt64 (40)));

			Assert.AreEqual (0.01M, numDef.ConvertFromInt64 (numDef.ConvertToInt64 (0.01M)));
			Assert.AreEqual (0.99M, numDef.ConvertFromInt64 (numDef.ConvertToInt64 (0.99M)));

			Assert.AreEqual (1, numDef.ConvertToInt64 (0.01M));
			Assert.AreEqual (100, numDef.ConvertToInt64 (1.00M));

			Assert.AreEqual (0.01M, numDef.ConvertFromInt64 (1));
			Assert.AreEqual (1.00M, numDef.ConvertFromInt64 (100));
		}


		[TestMethod]
		public void CheckCompatibilityTest()
		{
			DbNumDef numDef = new DbNumDef (6, 2, 0, 1000.00M);
			
			Assert.AreEqual (false, numDef.CheckCompatibility (-1));
			Assert.AreEqual (true,  numDef.CheckCompatibility (0));
			Assert.AreEqual (false, numDef.CheckCompatibility (0.001M));
			Assert.AreEqual (true,  numDef.CheckCompatibility (0.010M));
			Assert.AreEqual (true,  numDef.CheckCompatibility (1000.00M));
			Assert.AreEqual (true,  numDef.CheckCompatibility (999.99M));
			Assert.AreEqual (false, numDef.CheckCompatibility (5.250001M));
		}


		[TestMethod]
		public void RoundTest()
		{
			DbNumDef def = new DbNumDef (6, 2, -1000.00M, 1000.00M);
			
			Assert.AreEqual (2000.05M, def.Round (2000.05M));
			Assert.AreEqual (false, def.CheckCompatibility (2000.05M));
			
			Assert.AreEqual (0.00M, def.Round (0.001M));
			Assert.AreEqual (0.00M, def.Round (-0.001M));
			Assert.AreEqual (0.99M, def.Round (0.994M));
			Assert.AreEqual (0.99M, def.Round (0.9949999M));
			Assert.AreEqual (1.00M, def.Round (0.995M));
			Assert.AreEqual (1.00M, def.Round (1.0049999M));
		}


		[TestMethod]
		public void ClipTest()
		{
			DbNumDef numDef = new DbNumDef (6, 2, -1000.00M, 1000.00M);
			
			Assert.AreEqual (0.001M, numDef.Clip (0.001M));
			Assert.AreEqual (-1000.00M, numDef.Clip (-2000.00M));
			Assert.AreEqual ( 1000.00M, numDef.Clip ( 2000.00M));
		}


		[TestMethod]
		public void ParseTest()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);

			Assert.AreEqual (10.5M, def.Parse ("10.5", System.Globalization.CultureInfo.InvariantCulture));
		}


		[TestMethod]
		public void TestParseException()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);

			ExceptionAssert.Throw<System.OverflowException>
			(
				() => def.Parse ("50.0", System.Globalization.CultureInfo.InvariantCulture)
			);
			
			def = new DbNumDef (3, 1, 0.0M, 49.9M);

			ExceptionAssert.Throw<System.OverflowException>
			(
				() => def.Parse ("-1")
			);
			
			def = new DbNumDef (3, 1, 0.0M, 49.9M);
			
			ExceptionAssert.Throw<System.FormatException>
			(
				() => def.Parse ("x")
			);
		}


		[TestMethod]
		public void ToStringTest()
		{
			System.IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture;

			DbNumDef numDef = new DbNumDef (3, 1, 0.0M, 49.9M);
			Assert.AreEqual ("10.5", numDef.ToString (10.5M, formatProvider));
			
			numDef = new DbNumDef (6, 3, 0.0M, 49.995M);
			Assert.AreEqual ("10.500", numDef.ToString (10.5M, formatProvider));
			
			numDef = new DbNumDef (3, 2, -9.99M, 9.99M);
			Assert.AreEqual ("1.00", numDef.ToString (1.000M, formatProvider));
			Assert.AreEqual ("-3.50", numDef.ToString (-3.5M, formatProvider));
		}


	}


}

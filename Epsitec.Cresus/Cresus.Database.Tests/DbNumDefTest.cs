using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbNumDefTest
	{
		[Test] public void CheckDefaultMinMax()
		{
			DbNumDef def = new DbNumDef ();
			
			def.DigitPrecision = 3;
			def.DigitShift     = 0;
			
			Assert.AreEqual (false, def.IsMinMaxDefined);
			Assert.AreEqual (true, def.IsDigitDefined);
			Assert.AreEqual ( 999M, def.MaxValue);
			Assert.AreEqual (-999M, def.MinValue);
			
			def.DigitPrecision = 6;
			def.DigitShift     = 3;
			
			Assert.AreEqual ( 999.999M, def.MaxValue);
			Assert.AreEqual (-999.999M, def.MinValue);
		}
		
		[Test] public void CheckDefaultPrecisionShift()
		{
			DbNumDef def = new DbNumDef ();
			
			def.MinValue = 0;
			def.MaxValue = 9;
			
			Assert.AreEqual (false, def.IsDigitDefined);
			Assert.AreEqual (true, def.IsMinMaxDefined);
			Assert.AreEqual (1, def.DigitPrecision);
			Assert.AreEqual (0, def.DigitShift);
			
			Assert.AreEqual (0,    def.ToDecimalRange ().Minimum);
			Assert.AreEqual (9,    def.ToDecimalRange ().Maximum);
			Assert.AreEqual (1.0M, def.ToDecimalRange ().Resolution);
			
			def.MinValue =  0.01M;
			def.MaxValue = 10.00M;
			
			Assert.AreEqual (false, def.IsDigitDefined);
			Assert.AreEqual (4, def.DigitPrecision);
			Assert.AreEqual (2, def.DigitShift);
			
			Assert.AreEqual ( 0.01M, def.ToDecimalRange ().Minimum);
			Assert.AreEqual (10.00M, def.ToDecimalRange ().Maximum);
			Assert.AreEqual ( 0.01M, def.ToDecimalRange ().Resolution);
			
			def.MinValue = -9.999M;
			def.MaxValue =  9.999M;
			
			Assert.AreEqual (false, def.IsDigitDefined);
			Assert.AreEqual (4, def.DigitPrecision);
			Assert.AreEqual (3, def.DigitShift);
			
			Assert.AreEqual (-9.999M, def.ToDecimalRange ().Minimum);
			Assert.AreEqual ( 9.999M, def.ToDecimalRange ().Maximum);
			Assert.AreEqual ( 0.001M, def.ToDecimalRange ().Resolution);
		}

		[Test] public void CheckInvalidCase()
		{
			DbNumDef def = new DbNumDef (1,0);
			
			def.MinValue = -1;
			def.MaxValue = 99;
			
			Assert.AreEqual (true, def.IsDigitDefined);
			Assert.AreEqual (true, def.IsMinMaxDefined);
			Assert.AreEqual (1, def.DigitPrecision);
			Assert.AreEqual (0, def.DigitShift);
			Assert.AreEqual (false, def.IsValid);
		}

/*		[Test] [Ignore("Assert in code, no exception thrown")] [ExpectedException (typeof (System.OverflowException))] public void CheckOverflowMax()
		{
			DbNumDef def = new DbNumDef ();
			def.MinValue = 0;
			def.MaxValue = 99999999999999999999999.0M;
			def.MaxValue++;		// Assert, car dépasse 24 digits
		}*/

		[Test] public void CheckMinimumBitsInt()
		{
			DbNumDef def = new DbNumDef (3, 0);
			
			def.MinValue = 0;
			def.MaxValue = 127;
			Assert.AreEqual (7, def.MinimumBits);
			
			def.MinValue = -127;
			def.MaxValue =  127;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = -128;
			def.MaxValue =  127;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = -128;
			def.MaxValue =  128;
			Assert.AreEqual (9, def.MinimumBits);
			
			def.MinValue = 1;
			def.MaxValue = 256;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = 500;
			def.MaxValue = 515;
			Assert.AreEqual (4, def.MinimumBits);
		}
		
		[Test] public void CheckMinimumBitsFrac()
		{
			DbNumDef def = new DbNumDef (5, 2);
			
			def.MinValue =   0.00M;
			def.MaxValue = 127.00M;			//	127.00 => BIN(11000110011100) * 0.01
			Assert.AreEqual (14, def.MinimumBits);
			
			def.MinValue = -1.27M;
			def.MaxValue =  1.27M;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = -1.28M;
			def.MaxValue =  1.27M;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = -1.28M;
			def.MaxValue =  1.28M;
			Assert.AreEqual (9, def.MinimumBits);
			
			def.MinValue = 0.01M;
			def.MaxValue = 2.56M;
			Assert.AreEqual (8, def.MinimumBits);
			
			def.MinValue = 5.00M;
			def.MaxValue = 5.15M;
			Assert.AreEqual (4, def.MinimumBits);
		}
		
		[Test] public void CheckDbRawTypes()
		{
			DbNumDef def;
			
			def = DbNumDef.FromRawType (DbRawType.Int16);
			
			Assert.IsTrue (def.CheckCompatibility (System.Int16.MinValue));
			Assert.IsTrue (def.CheckCompatibility (System.Int16.MaxValue));
			Assert.IsTrue (def.CheckCompatibility (System.Int16.MinValue-1) == false);
			Assert.IsTrue (def.CheckCompatibility (System.Int16.MaxValue+1) == false);
			
			def = DbNumDef.FromRawType (DbRawType.Int32);
			
			Assert.IsTrue (def.CheckCompatibility (System.Int32.MinValue));
			Assert.IsTrue (def.CheckCompatibility (System.Int32.MaxValue));
			Assert.IsTrue (def.CheckCompatibility (System.Int32.MinValue-1L) == false);
			Assert.IsTrue (def.CheckCompatibility (System.Int32.MaxValue+1L) == false);
			
			def = DbNumDef.FromRawType (DbRawType.Int64);
			
			Assert.IsTrue (def.CheckCompatibility (System.Int64.MinValue));
			Assert.IsTrue (def.CheckCompatibility (System.Int64.MaxValue));
			Assert.AreEqual (64, def.MinimumBits);
			
			def = DbNumDef.FromRawType (DbRawType.SmallDecimal);
			
			Assert.IsTrue (def.CheckCompatibility (0.000000001M));
			Assert.IsTrue (def.CheckCompatibility (0.0000000001M) == false);
			Assert.IsTrue (def.CheckCompatibility ( 999999999M));
			Assert.IsTrue (def.CheckCompatibility (1000000000M) == false);
			
			def = DbNumDef.FromRawType (DbRawType.LargeDecimal);
			
			Assert.IsTrue (def.CheckCompatibility (0.001M));
			Assert.IsTrue (def.CheckCompatibility (0.0001M) == false);
			Assert.IsTrue (def.CheckCompatibility ( 999999999999999M));
			Assert.IsTrue (def.CheckCompatibility (1000000000000000M) == false);
		}

		[Test]
		public void CheckConvert1()
		{
			DbNumDef def = new DbNumDef (5, 2, 5.00M, 5.15M);

			Assert.AreEqual (0, def.ConvertToInt64 (def.ConvertFromInt64 (0)));
			Assert.AreEqual (15, def.ConvertToInt64 (def.ConvertFromInt64 (15)));

			Assert.AreEqual (5.00M, def.ConvertFromInt64 (def.ConvertToInt64 (5.00M)));
			Assert.AreEqual (5.15M, def.ConvertFromInt64 (def.ConvertToInt64 (5.15M)));

			Assert.AreEqual (-8, def.ConvertToInt64 (5.00M));
			Assert.AreEqual (7, def.ConvertToInt64 (5.15M));

			Assert.AreEqual (5.00M, def.ConvertFromInt64 (-8));
			Assert.AreEqual (5.15M, def.ConvertFromInt64 (7));
		}

		[Test]
		public void CheckConvert2()
		{
			DbNumDef def = new DbNumDef (5, 2, 1.00M, 0.01M);

			Assert.AreEqual (0, def.ConvertToInt64 (def.ConvertFromInt64 (0)));
			Assert.AreEqual (40, def.ConvertToInt64 (def.ConvertFromInt64 (40)));

			Assert.AreEqual (0.01M, def.ConvertFromInt64 (def.ConvertToInt64 (0.01M)));
			Assert.AreEqual (0.99M, def.ConvertFromInt64 (def.ConvertToInt64 (0.99M)));

			Assert.AreEqual (1, def.ConvertToInt64 (0.01M));
			Assert.AreEqual (100, def.ConvertToInt64 (1.00M));

			Assert.AreEqual (0.01M, def.ConvertFromInt64 (1));
			Assert.AreEqual (1.00M, def.ConvertFromInt64 (100));
		}

		[Test]
		public void CheckCheckCompatibility()
		{
			DbNumDef def = new DbNumDef (6, 2, 0, 1000.00M);
			
			Assert.AreEqual (false, def.CheckCompatibility (-1));
			Assert.AreEqual (true,  def.CheckCompatibility (0));
			Assert.AreEqual (false, def.CheckCompatibility (0.001M));
			Assert.AreEqual (true,  def.CheckCompatibility (0.010M));
			Assert.AreEqual (true,  def.CheckCompatibility (1000.00M));
			Assert.AreEqual (true,  def.CheckCompatibility (999.99M));
			Assert.AreEqual (false, def.CheckCompatibility (5.250001M));
		}
		
		[Test] public void CheckRound()
		{
			DbNumDef def = new DbNumDef (6, 2, -1000.00M, 1000.00M);
			
			//	Si la valeur est hors des bornes, l'arrondi doit quand-même se faire !
			
			Assert.AreEqual (2000.05M, def.Round (2000.05M));
			Assert.AreEqual (false, def.CheckCompatibility (2000.05M));
			
			Assert.AreEqual (0.00M, def.Round (0.001M));
			Assert.AreEqual (0.00M, def.Round (-0.001M));
			Assert.AreEqual (0.99M, def.Round (0.994M));
			Assert.AreEqual (0.99M, def.Round (0.9949999M));
			Assert.AreEqual (1.00M, def.Round (0.995M));
			Assert.AreEqual (1.00M, def.Round (1.0049999M));
		}
		
		[Test] public void CheckClip()
		{
			DbNumDef def = new DbNumDef (6, 2, -1000.00M, 1000.00M);
			
			//	Si la valeur a une précision trop élevée, la valeur n'est pas
			//	arrondie...
			
			Assert.AreEqual (0.001M, def.Clip (0.001M));
			Assert.AreEqual (-1000.00M, def.Clip (-2000.00M));
			Assert.AreEqual ( 1000.00M, def.Clip ( 2000.00M));
		}
		
		[Test] public void CheckParse0()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);
			Assert.AreEqual (10.5M, def.Parse ("10.5", System.Globalization.CultureInfo.InvariantCulture));
		}
		
		[Test] [ExpectedException (typeof (System.OverflowException))] public void CheckParse1()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);
			def.Parse ("50.0", System.Globalization.CultureInfo.InvariantCulture);
		}
		
		[Test] [ExpectedException (typeof (System.OverflowException))] public void CheckParse2()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);
			def.Parse ("-1");
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckParse3()
		{
			DbNumDef def = new DbNumDef (3, 1, 0.0M, 49.9M);
			def.Parse ("x");
		}
		
		[Test] public void CheckToString()
		{
			System.IFormatProvider format_provider = System.Globalization.CultureInfo.InvariantCulture;
			DbNumDef def;
			def = new DbNumDef (3, 1, 0.0M, 49.9M);
			Assert.AreEqual ("10.5", def.ToString (10.5M, format_provider));
			
			def = new DbNumDef (6, 3, 0.0M, 49.995M);
			Assert.AreEqual ("10.500", def.ToString (10.5M, format_provider));
			
			def = new DbNumDef (3, 2, -9.99M, 9.99M);
			Assert.AreEqual ("1.00", def.ToString (1.000M, format_provider));
			Assert.AreEqual ("-3.50", def.ToString (-3.5M, format_provider));
		}
	}
}

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
			
			Assertion.AssertEquals (false, def.IsMinMaxDefined);
			Assertion.AssertEquals (true, def.IsDigitDefined);
			Assertion.AssertEquals ( 999, def.MaxValue);
			Assertion.AssertEquals (-999, def.MinValue);
			
			def.DigitPrecision = 6;
			def.DigitShift     = 3;
			
			Assertion.AssertEquals ( 999.999M, def.MaxValue);
			Assertion.AssertEquals (-999.999M, def.MinValue);
		}
		
		[Test] public void CheckDefaultPrecisionShift()
		{
			DbNumDef def = new DbNumDef ();
			
			def.MinValue = 0;
			def.MaxValue = 9;
			
			Assertion.AssertEquals (false, def.IsDigitDefined);
			Assertion.AssertEquals (true, def.IsMinMaxDefined);
			Assertion.AssertEquals (1, def.DigitPrecision);
			Assertion.AssertEquals (0, def.DigitShift);
			
			def.MinValue =  0.01M;
			def.MaxValue = 10.00M;
			
			Assertion.AssertEquals (false, def.IsDigitDefined);
			Assertion.AssertEquals (4, def.DigitPrecision);
			Assertion.AssertEquals (2, def.DigitShift);
			
			def.MinValue = -9.999M;
			def.MaxValue =  9.999M;
			
			Assertion.AssertEquals (false, def.IsDigitDefined);
			Assertion.AssertEquals (4, def.DigitPrecision);
			Assertion.AssertEquals (3, def.DigitShift);
		}
		
		[Test] public void CheckMinimumBitsInt()
		{
			DbNumDef def = new DbNumDef (3, 0);
			
			def.MinValue = 0;
			def.MaxValue = 127;
			Assertion.AssertEquals (7, def.MinimumBits);
			
			def.MinValue = -127;
			def.MaxValue =  127;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = -128;
			def.MaxValue =  127;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = -128;
			def.MaxValue =  128;
			Assertion.AssertEquals (9, def.MinimumBits);
			
			def.MinValue = 1;
			def.MaxValue = 256;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = 500;
			def.MaxValue = 515;
			Assertion.AssertEquals (4, def.MinimumBits);
		}
		
		[Test] public void CheckMinimumBitsFrac()
		{
			DbNumDef def = new DbNumDef (5, 2);
			
			def.MinValue =   0.00M;
			def.MaxValue = 127.00M;			//	127.00 => BIN(11000110011100) * 0.01
			Assertion.AssertEquals (14, def.MinimumBits);
			
			def.MinValue = -1.27M;
			def.MaxValue =  1.27M;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = -1.28M;
			def.MaxValue =  1.27M;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = -1.28M;
			def.MaxValue =  1.28M;
			Assertion.AssertEquals (9, def.MinimumBits);
			
			def.MinValue = 0.01M;
			def.MaxValue = 2.56M;
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MinValue = 5.00M;
			def.MaxValue = 5.15M;
			Assertion.AssertEquals (4, def.MinimumBits);
		}
		
		[Test] public void CheckDbRawTypes()
		{
			DbNumDef def;
			
			def = DbNumDef.FromRawType (DbRawType.Int16);
			
			Assertion.Assert (def.CheckCompatibility (System.Int16.MinValue));
			Assertion.Assert (def.CheckCompatibility (System.Int16.MaxValue));
			Assertion.Assert (def.CheckCompatibility (System.Int16.MinValue-1) == false);
			Assertion.Assert (def.CheckCompatibility (System.Int16.MaxValue+1) == false);
			
			def = DbNumDef.FromRawType (DbRawType.Int32);
			
			Assertion.Assert (def.CheckCompatibility (System.Int32.MinValue));
			Assertion.Assert (def.CheckCompatibility (System.Int32.MaxValue));
			Assertion.Assert (def.CheckCompatibility (System.Int32.MinValue-1L) == false);
			Assertion.Assert (def.CheckCompatibility (System.Int32.MaxValue+1L) == false);
			
			def = DbNumDef.FromRawType (DbRawType.Int64);
			
			Assertion.Assert (def.CheckCompatibility (System.Int64.MinValue));
			Assertion.Assert (def.CheckCompatibility (System.Int64.MaxValue));
			Assertion.AssertEquals (64, def.MinimumBits);
			
			def = DbNumDef.FromRawType (DbRawType.SmallDecimal);
			
			Assertion.Assert (def.CheckCompatibility (0.000000001M));
			Assertion.Assert (def.CheckCompatibility (0.0000000001M) == false);
			Assertion.Assert (def.CheckCompatibility ( 999999999M));
			Assertion.Assert (def.CheckCompatibility (1000000000M) == false);
			
			def = DbNumDef.FromRawType (DbRawType.LargeDecimal);
			
			Assertion.Assert (def.CheckCompatibility (0.001M));
			Assertion.Assert (def.CheckCompatibility (0.0001M) == false);
			Assertion.Assert (def.CheckCompatibility ( 999999999999999M));
			Assertion.Assert (def.CheckCompatibility (1000000000000000M) == false);
		}
		
		[Test] public void CheckConvert()
		{
			DbNumDef def = new DbNumDef (5, 2, 5.00M, 5.15M);
			
			Assertion.AssertEquals ( 0, def.ConvertToInt64 (def.ConvertFromInt64 (0)));
			Assertion.AssertEquals (15, def.ConvertToInt64 (def.ConvertFromInt64 (15)));
			
			Assertion.AssertEquals (5.00M, def.ConvertFromInt64 (def.ConvertToInt64 (5.00M)));
			Assertion.AssertEquals (5.15M, def.ConvertFromInt64 (def.ConvertToInt64 (5.15M)));
			
			Assertion.AssertEquals ( 0, def.ConvertToInt64 (5.00M));
			Assertion.AssertEquals (15, def.ConvertToInt64 (5.15M));
			
			Assertion.AssertEquals (5.00M, def.ConvertFromInt64 (0));
			Assertion.AssertEquals (5.15M, def.ConvertFromInt64 (15));
		}
		
		[Test] public void CheckCheckCompatibility()
		{
			DbNumDef def = new DbNumDef (6, 2, 0, 1000);
			
			Assertion.AssertEquals (false, def.CheckCompatibility (-1));
			Assertion.AssertEquals (true,  def.CheckCompatibility (0));
			Assertion.AssertEquals (false, def.CheckCompatibility (0.001M));
			Assertion.AssertEquals (true,  def.CheckCompatibility (0.010M));
			Assertion.AssertEquals (true,  def.CheckCompatibility (1000.00M));
			Assertion.AssertEquals (true,  def.CheckCompatibility (999.99M));
			Assertion.AssertEquals (false, def.CheckCompatibility (5.250001M));
		}
		
		[Test] public void CheckRound()
		{
			DbNumDef def = new DbNumDef (6, 2, -1000, 1000);
			
			//	Si la valeur est hors des bornes, l'arrondi doit quand-même se faire !
			
			Assertion.AssertEquals (2000.05M, def.Round (2000.05M));
			Assertion.AssertEquals (false, def.CheckCompatibility (2000.05M));
			
			Assertion.AssertEquals (0.00M, def.Round (0.001M));
			Assertion.AssertEquals (0.00M, def.Round (-0.001M));
			Assertion.AssertEquals (0.99M, def.Round (0.994M));
			Assertion.AssertEquals (0.99M, def.Round (0.9949999M));
			Assertion.AssertEquals (1.00M, def.Round (0.995M));
			Assertion.AssertEquals (1.00M, def.Round (1.0049999M));
		}
		
		[Test] public void CheckClip()
		{
			DbNumDef def = new DbNumDef (6, 2, -1000, 1000);
			
			//	Si la valeur a une précision trop élevée, la valeur n'est pas
			//	arrondie...
			
			Assertion.AssertEquals (0.001M, def.Clip (0.001M));
			Assertion.AssertEquals (-1000.00M, def.Clip (-2000.00M));
			Assertion.AssertEquals ( 1000.00M, def.Clip ( 2000.00M));
		}
	}
}

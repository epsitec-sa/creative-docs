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
			Assertion.AssertEquals (999, def.MaxValue);
			Assertion.AssertEquals (-999, def.MinValue);
			
			def.DigitPrecision = 6;
			def.DigitShift     = 3;
			
			Assertion.AssertEquals (999.999, def.MaxValue);
			Assertion.AssertEquals (-999.999, def.MinValue);
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
		
		[Test] public void CheckMinimumBits()
		{
			DbNumDef def = new DbNumDef ();

			def.DigitPrecision = 3;
			def.DigitShift     = 0;
			def.MinValue       = 0;
			def.MaxValue       = 255;
			
			Assertion.AssertEquals (8, def.MinimumBits);
			
			def.MaxValue = 256;
			Assertion.AssertEquals (9, def.MinimumBits);
			
			def.MinValue = 1;
			Assertion.AssertEquals (8, def.MinimumBits);
		}
		[Test] public void CheckCheckCompatibility()
		{
			DbNumDef def = new DbNumDef (6, 2, 0, 1000);
			
			Assertion.AssertEquals (false, def.CheckCompatibility (-1));
			Assertion.AssertEquals (true, def.CheckCompatibility (0));
			Assertion.AssertEquals (false, def.CheckCompatibility (0.001M));
			Assertion.AssertEquals (true, def.CheckCompatibility (0.010M));
			Assertion.AssertEquals (true, def.CheckCompatibility (1000.00M));
			Assertion.AssertEquals (true, def.CheckCompatibility (999.99M));
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

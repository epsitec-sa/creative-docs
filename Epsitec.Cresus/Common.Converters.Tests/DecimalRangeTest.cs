using NUnit.Framework;

namespace Epsitec.Common.Converters
{
	[TestFixture] public class DecimalRangeTest
	{
		[Test] public void CheckIsValid()
		{
			DecimalRange range1 = new DecimalRange ();
			DecimalRange range2 = new DecimalRange (0.0M, -1.0M);
			DecimalRange range3 = new DecimalRange (0.0M, 1.0M, 0.05M);
			
			Assertion.AssertEquals (true,  range1.IsValid);
			Assertion.AssertEquals (false, range2.IsValid);
			Assertion.AssertEquals (true,  range3.IsValid);
		}
		
		[Test] public void CheckConstrain()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.05M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 4.00M);
			DecimalRange range4 = new DecimalRange (-100.0M, 100.0M, 1.00M);
			
			Assertion.Assert (0.50M == range1.Constrain (0.50M));
			Assertion.Assert (0.50M == range1.Constrain (0.501M));
			Assertion.Assert (0.50M == range1.Constrain (0.496M));
			Assertion.Assert (0.50M == range1.Constrain (0.495M));
			Assertion.Assert (0.49M == range1.Constrain (0.494M));
			
			Assertion.Assert (0.50M == range2.Constrain (0.50M));
			Assertion.Assert (0.50M == range2.Constrain (0.52M));
			Assertion.Assert (0.50M == range2.Constrain (0.475M));
			Assertion.Assert (0.50M == range2.Constrain (0.52499M));
			Assertion.Assert (0.45M == range2.Constrain (0.474M));
			
			Assertion.Assert (12.0M == range3.Constrain (13.0M));
			Assertion.Assert (12.0M == range3.Constrain (10.0M));
			Assertion.Assert (12.0M == range3.Constrain (13.999M));
			Assertion.Assert (16.0M == range3.Constrain (14.0M));
			Assertion.Assert ( 8.0M == range3.Constrain (9.9999M));
			
			Assertion.Assert ( 1.0M == range4.Constrain ( 0.500M));
			Assertion.Assert ( 1.0M == range4.Constrain ( 1.499M));
			Assertion.Assert ( 2.0M == range4.Constrain ( 1.500M));
			Assertion.Assert ( 0.0M == range4.Constrain ( 0.499M));
			Assertion.Assert ( 0.0M == range4.Constrain (-0.499M));
			Assertion.Assert (-1.0M == range4.Constrain (-0.500M));
			Assertion.Assert (-1.0M == range4.Constrain (-1.499M));
			Assertion.Assert (-2.0M == range4.Constrain (-1.500M));
		}
		
		[Test] public void CheckPrecision()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 1.0M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.1M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range4 = new DecimalRange (0.0M, 100.0M, 10.0M);
			
			Assertion.AssertEquals (50M,    range1.Constrain (50.0M));
			Assertion.AssertEquals (50.0M,  range2.Constrain (50.0M));
			Assertion.AssertEquals (50.00M, range3.Constrain (50.0M));
			Assertion.AssertEquals (50M,    range4.Constrain (50.0M));
		}
	}
}

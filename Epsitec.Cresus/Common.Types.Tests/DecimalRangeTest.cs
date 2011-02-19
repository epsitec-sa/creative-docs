using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class DecimalRangeTest
	{
		[Test] public void CheckIsValid()
		{
			DecimalRange range1 = new DecimalRange ();
			DecimalRange range2 = new DecimalRange (0.0M, -1.0M);
			DecimalRange range3 = new DecimalRange (0.0M, 1.0M, 0.05M);
			
			Assert.IsTrue (range1.IsValid);
			Assert.IsFalse (range2.IsValid);
			Assert.IsTrue (range3.IsValid);

			Assert.IsTrue (range1.IsEmpty);
			Assert.IsFalse (range2.IsEmpty);
			Assert.IsFalse (range3.IsEmpty);
		}
		
		[Test] public void CheckConstrain()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.05M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 4.00M);
			DecimalRange range4 = new DecimalRange (-100.0M, 100.0M, 1.00M);
			
			Assert.IsTrue (0.50M == range1.Constrain (0.50M));
			Assert.IsTrue (0.50M == range1.Constrain (0.501M));
			Assert.IsTrue (0.50M == range1.Constrain (0.496M));
			Assert.IsTrue (0.50M == range1.Constrain (0.495M));
			Assert.IsTrue (0.49M == range1.Constrain (0.494M));
			
			Assert.IsTrue (0.50M == range2.Constrain (0.50M));
			Assert.IsTrue (0.50M == range2.Constrain (0.52M));
			Assert.IsTrue (0.50M == range2.Constrain (0.475M));
			Assert.IsTrue (0.50M == range2.Constrain (0.52499M));
			Assert.IsTrue (0.45M == range2.Constrain (0.474M));
			
			Assert.IsTrue (12.0M == range3.Constrain (13.0M));
			Assert.IsTrue (12.0M == range3.Constrain (10.0M));
			Assert.IsTrue (12.0M == range3.Constrain (13.999M));
			Assert.IsTrue (16.0M == range3.Constrain (14.0M));
			Assert.IsTrue ( 8.0M == range3.Constrain (9.9999M));
			
			Assert.IsTrue ( 1.0M == range4.Constrain ( 0.500M));
			Assert.IsTrue ( 1.0M == range4.Constrain ( 1.499M));
			Assert.IsTrue ( 2.0M == range4.Constrain ( 1.500M));
			Assert.IsTrue ( 0.0M == range4.Constrain ( 0.499M));
			Assert.IsTrue ( 0.0M == range4.Constrain (-0.499M));
			Assert.IsTrue (-1.0M == range4.Constrain (-0.500M));
			Assert.IsTrue (-1.0M == range4.Constrain (-1.499M));
			Assert.IsTrue (-2.0M == range4.Constrain (-1.500M));
		}
		
		[Test] public void CheckConstrainToZero()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.05M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 4.00M);
			DecimalRange range4 = new DecimalRange (-100.0M, 100.0M, 1.00M);
			
			Assert.IsTrue (0.50M == range1.ConstrainToZero (0.50M));
			Assert.IsTrue (0.50M == range1.ConstrainToZero (0.501M));
			Assert.IsTrue (0.49M == range1.ConstrainToZero (0.496M));
			Assert.IsTrue (0.49M == range1.ConstrainToZero (0.495M));
			Assert.IsTrue (0.49M == range1.ConstrainToZero (0.494M));
			
			Assert.IsTrue (0.50M == range2.ConstrainToZero (0.50M));
			Assert.IsTrue (0.50M == range2.ConstrainToZero (0.52M));
			Assert.IsTrue (0.45M == range2.ConstrainToZero (0.475M));
			Assert.IsTrue (0.50M == range2.ConstrainToZero (0.52499M));
			Assert.IsTrue (0.45M == range2.ConstrainToZero (0.474M));
			
			Assert.IsTrue (12.0M == range3.ConstrainToZero (13.0M));
			Assert.IsTrue ( 8.0M == range3.ConstrainToZero (10.0M));
			Assert.IsTrue (12.0M == range3.ConstrainToZero (13.999M));
			Assert.IsTrue (12.0M == range3.ConstrainToZero (14.0M));
			Assert.IsTrue ( 8.0M == range3.ConstrainToZero (9.9999M));
			
			Assert.IsTrue ( 0.0M == range4.ConstrainToZero ( 0.500M));
			Assert.IsTrue ( 1.0M == range4.ConstrainToZero ( 1.499M));
			Assert.IsTrue ( 1.0M == range4.ConstrainToZero ( 1.500M));
			Assert.IsTrue ( 0.0M == range4.ConstrainToZero ( 0.499M));
			Assert.IsTrue ( 0.0M == range4.ConstrainToZero (-0.499M));
			Assert.IsTrue ( 0.0M == range4.ConstrainToZero (-0.500M));
			Assert.IsTrue (-1.0M == range4.ConstrainToZero (-1.499M));
			Assert.IsTrue (-1.0M == range4.ConstrainToZero (-1.500M));
		}
		
		[Test] public void CheckPrecision()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 1.0M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.1M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range4 = new DecimalRange (0.0M, 100.0M, 10.0M);
			DecimalRange range5 = new DecimalRange (0.090M, 0.099M, 0.002M);
			
			Assert.AreEqual (50M,    range1.Constrain (50.0M));
			Assert.AreEqual (50.0M,  range2.Constrain (50.0M));
			Assert.AreEqual (50.00M, range3.Constrain (50.0M));
			Assert.AreEqual (50M,    range4.Constrain (50.0M));
			Assert.AreEqual (0.094M, range5.ConstrainToZero (0.0959M));
			
			Assert.AreEqual (0M,     range1.Constrain (0.0M));
			Assert.AreEqual (0.0M,   range2.Constrain (0.0M));
			Assert.AreEqual (0.00M,  range3.Constrain (0.0M));
			Assert.AreEqual (0M,     range4.Constrain (0.0M));
			Assert.AreEqual (0.090M, range5.Constrain (0.0M));

			Assert.AreEqual (0, range1.FractionalDigits);
			Assert.AreEqual (1, range2.FractionalDigits);
			Assert.AreEqual (2, range3.FractionalDigits);
			Assert.AreEqual (0, range4.FractionalDigits);
			Assert.AreEqual (3, range5.FractionalDigits);

			Assert.AreEqual (3, range1.GetMaximumDigitCount ());
			Assert.AreEqual (4, range2.GetMaximumDigitCount ());
			Assert.AreEqual (5, range3.GetMaximumDigitCount ());
			Assert.AreEqual (3, range4.GetMaximumDigitCount ());
			Assert.AreEqual (2, range5.GetMaximumDigitCount ());	//	..90 --> ..99 = 2 digits
		}
		
		[Test] public void CheckConvertToString()
		{
			DecimalRange range1 = new DecimalRange (0.0M, 100.0M, 1.0M);
			DecimalRange range2 = new DecimalRange (0.0M, 100.0M, 0.1M);
			DecimalRange range3 = new DecimalRange (0.0M, 100.0M, 0.01M);
			DecimalRange range4 = new DecimalRange (0.0M, 100.0M, 10.0M);
			
			Assert.AreEqual ("50",    range1.ConvertToString (50.0M));
			Assert.AreEqual ("50.0",  range2.ConvertToString (50.0M));
			Assert.AreEqual ("50.00", range3.ConvertToString (50.0M));
			Assert.AreEqual ("50",    range4.ConvertToString (50.0M));
			
			Assert.AreEqual ("0",     range1.ConvertToString (0.0M));
			Assert.AreEqual ("0.0",   range2.ConvertToString (0.0M));
			Assert.AreEqual ("0.00",  range3.ConvertToString (0.0M));
			Assert.AreEqual ("0",     range4.ConvertToString (0.0M));
		}
	}
}

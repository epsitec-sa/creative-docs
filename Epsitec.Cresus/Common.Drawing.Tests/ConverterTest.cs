using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class ConverterTest
	{
		[Test] public void CheckPoint()
		{
			Point a = new Point (10, 20);
			
			System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Point));
			string sa = conv.ConvertToString (a);
			Point b = (Point) conv.ConvertFromString (sa);
			
			Assertion.AssertEquals (a, b);
			Assertion.AssertEquals (sa, conv.ConvertToString (b));
		}
		
		[Test] public void CheckSize()
		{
			Size a = new Size (10, 20);
			
			System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Size));
			string sa = conv.ConvertToString (a);
			Size b = (Size) conv.ConvertFromString (sa);
			
			Assertion.AssertEquals (a, b);
			Assertion.AssertEquals (sa, conv.ConvertToString (b));
		}
		
		[Test] public void CheckRectangle()
		{
			Rectangle a = new Rectangle (10, 20, 30, 40);
			
			System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Rectangle));
			string sa = conv.ConvertToString (a);
			Rectangle b = (Rectangle) conv.ConvertFromString (sa);
			
			Assertion.AssertEquals (a, b);
			Assertion.AssertEquals (sa, conv.ConvertToString (b));
		}
		
		[Test] public void CheckColor()
		{
			Color a1 = new Color (0.1, 0.2, 0.3);
			Color a2 = new Color (0.1, 0.2, 0.3, 0.4);
			
			System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Color));
			
			string sa1 = conv.ConvertToString (a1);
			string sa2 = conv.ConvertToString (a2);
			
			Color b1 = (Color) conv.ConvertFromString (sa1);
			Color b2 = (Color) conv.ConvertFromString (sa2);
			
			Assertion.AssertEquals (a1, b1);
			Assertion.AssertEquals (a2, b2);
			Assertion.AssertEquals (sa1, conv.ConvertToString (b1));
			Assertion.AssertEquals (sa2, conv.ConvertToString (b2));
		}
	}
}

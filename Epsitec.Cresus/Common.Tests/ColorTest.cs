using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class ColorTest
	{
		[Test] public void CheckCreation()
		{
			double br = 0.8;
			
			Color c1 = Color.FromBrightness (br);
			Color c2 = Color.FromARGB (1.0, br, br, br);
			Color c3 = Color.FromRGB (br, br, br);
			Color c4 = Color.FromName ("White");
			Color c5 = new Color (System.Drawing.Color.Empty);
			
			Assertion.Assert (c1 == c2);
			Assertion.Assert (c1 == c3);
			Assertion.Assert (c1 != c4);
			Assertion.Assert (c1.IsEmpty == false);
			Assertion.Assert (c1.IsInRange == true);
			Assertion.Assert (c4.IsEmpty == false);
			Assertion.Assert (c4.IsInRange == true);
			Assertion.Assert (c5.IsEmpty);
			Assertion.AssertEquals ("Empty test failed", c5, Color.Empty);
		}
		
		[Test] public void CheckComponents()
		{
			float r = 0.4f;
			float g = 0.1f;
			float b = 0.6f;
			float a = 0.8f;
			
			Color color = Color.FromARGB (a, r, g, b);
			
			Assertion.Assert (color.A == a);
			Assertion.Assert (color.R == r);
			Assertion.Assert (color.G == g);
			Assertion.Assert (color.B == b);
			
			Assertion.AssertEquals ("Color.ToString failure", color.ToString (), "{R=0.40,G=0.10,B=0.60,A=0.80}");
		}
		
		[Test] public void CheckRanges()
		{
			Color color = Color.FromARGB (1.8, 0.5, -0.1, 10);
			
			Assertion.Assert (color.IsEmpty == false);
			Assertion.Assert (color.IsInRange == false);
			
			color.ClipToRange ();
			
			Assertion.Assert (color.IsInRange);
			Assertion.Assert (color.A == 1.0);
			Assertion.Assert (color.G == 0.0);
			Assertion.Assert (color.B == 1.0);
		}
		
		[Test] public void CheckFromNameAndHexa()
		{
			Color color_1 = Color.FromARGB (1, 1, 0, 0);
			Color color_2 = Color.FromName ("red");
			Color color_3 = Color.FromName ("#ff0000");
			Color color_4 = Color.FromName ("#blabla");
			
			Assertion.AssertEquals (color_1, color_2);
			Assertion.AssertEquals (color_1, color_3);
			Assertion.AssertEquals (Color.Empty, color_4);
		}
	}
}

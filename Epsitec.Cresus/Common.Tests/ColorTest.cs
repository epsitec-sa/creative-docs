using NUnit.Framework;
using Epsitec.Common.Widgets;
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
		
		
		[Test] public void CheckNamedColors()
		{
			WindowFrame window = new WindowFrame ();
			
			window.ClientSize = new System.Drawing.Size (870, 360);
			window.Text = "CheckNamedColors";
			window.Root.PaintForeground += new PaintEventHandler(NamedColors_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		private void NamedColors_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double dx = root.Client.Width;
			double dy = root.Client.Height;
			
			Font     font     = Font.GetFont ("Tahoma", "Regular");
			double   size     = 9.0;
			Color    black    = Color.FromBrightness (0);
			Graphics graphics = e.Graphics;
			string[] colors   = System.Enum.GetNames (typeof (System.Drawing.KnownColor));
			
			System.Array.Sort (colors);
			
			int n = 0;
			
			for (int i = 0; i < 35; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					if (n < colors.Length)
					{
						string color_name = colors[n++];
						
						double x = 5 + j * 20 * size;
						double y = dy - 15 - i * (size + 1);
						
						graphics.AddText (x+50, y, color_name, font, size);
						graphics.AddRectangle (x, y - 2, 45, size);
						graphics.RenderSolid (black);
						
						graphics.AddFilledRectangle (x, y - 2, 45, size);
						graphics.RenderSolid (Color.FromName (color_name));
					}
				}
			}
		}
	}
}

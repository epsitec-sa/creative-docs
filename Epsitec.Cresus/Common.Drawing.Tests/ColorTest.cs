using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
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
			Assertion.Assert (c1.IsValid);
			Assertion.Assert (c1.IsInRange == true);
			Assertion.Assert (c4.IsValid);
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
			
			Assertion.Assert (color.IsValid);
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
			Window window = new Window ();
			
			window.ClientSize = new Size (870, 360);
			window.Text = "CheckNamedColors";
			window.Root.PaintForeground += new PaintEventHandler(NamedColors_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckHSVConvert()
		{
			Color c1, c2;
			double h,s,v;

			// rouge
			c1 = Color.FromRGB(1.0, 0.0, 0.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// jaune
			c1 = Color.FromRGB(1.0, 1.0, 0.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 60.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// vert
			c1 = Color.FromRGB(0.0, 1.0, 0.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 120.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// cyan
			c1 = Color.FromRGB(0.0, 1.0, 1.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 180.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// bleu
			c1 = Color.FromRGB(0.0, 0.0, 1.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 240.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// magenta
			c1 = Color.FromRGB(1.0, 0.0, 1.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 300.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// rose
			c1 = Color.FromRGB(1.0, 0.5, 0.5);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 0.5);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// brun-rouge
			c1 = Color.FromRGB(0.5, 0.0, 0.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 1.0);
			Assertion.Assert(v == 0.5);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// brun
			c1 = Color.FromRGB(0.5, 0.25, 0.25);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 0.5);
			Assertion.Assert(v == 0.5);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// gris
			c1 = Color.FromRGB(0.5, 0.5, 0.5);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 0.0);
			Assertion.Assert(v == 0.5);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// blanc
			c1 = Color.FromRGB(1.0, 1.0, 1.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 0.0);
			Assertion.Assert(v == 1.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// noir
			c1 = Color.FromRGB(0.0, 0.0, 0.0);
			c1.ToHSV(out h, out s, out v);
			Assertion.Assert(h == 0.0);
			Assertion.Assert(s == 0.0);
			Assertion.Assert(v == 0.0);
			c2 = Color.FromHSV(h,s,v);
			Assertion.Assert(c1 == c2);

			// jaune
			c1 = Color.FromHSV(60+360,1,1);
			Assertion.Assert(c1.R == 1.0);
			Assertion.Assert(c1.G == 1.0);
			Assertion.Assert(c1.B == 0.0);

			c1 = Color.FromHSV(60-360,1,1);
			Assertion.Assert(c1.R == 1.0);
			Assertion.Assert(c1.G == 1.0);
			Assertion.Assert(c1.B == 0.0);
		}

		private void NamedColors_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double dx = root.Client.Width;
			double dy = root.Client.Height;
			
			double sx = dx / 870.0;
			double sy = dy / 360.0;
			double s  = System.Math.Min (sx, sy);
			
			Font     font     = Font.GetFont ("Tahoma", "Regular");
			double   size     = 9.0;
			Color    black    = Color.FromBrightness (0);
			Graphics graphics = e.Graphics;
			string[] colors   = System.Enum.GetNames (typeof (System.Drawing.KnownColor));
			
			graphics.ScaleTransform (s, s, 0, 0);
			
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
						double y = 360.0 - 15 - i * (size + 1);
						
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

using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class ColorTest
	{
		[SetUp]
		public void Initialize()
		{
			Widget.Initialize ();
		}

		[Test] public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}
		
		[Test] public void CheckCreation()
		{
			double br = 0.8;
			
			Color c1 = Color.FromBrightness (br);
			Color c2 = Color.FromAlphaRgb (1.0, br, br, br);
			Color c3 = Color.FromRgb (br, br, br);
			Color c4 = Color.FromName ("White");
			Color c5 = new Color (System.Drawing.Color.Empty);
			
			Assert.IsTrue (c1 == c2);
			Assert.IsTrue (c1 == c3);
			Assert.IsTrue (c1 != c4);
			Assert.IsTrue (c1.IsValid);
			Assert.IsTrue (c1.IsInRange == true);
			Assert.IsTrue (c4.IsValid);
			Assert.IsTrue (c4.IsInRange == true);
			Assert.IsTrue (c5.IsEmpty);
			Assert.AreEqual (c5, Color.Empty, "Empty test failed");
		}
		
		[Test] public void CheckComponents()
		{
			float a = 0.8f;
			float r = 0.4f;
			float g = 0.1f;
			float b = 0.6f;
			
			Color color = Color.FromAlphaRgb (a, r, g, b);
			
			Assert.IsTrue (color.A == a);
			Assert.IsTrue (color.R == r);
			Assert.IsTrue (color.G == g);
			Assert.IsTrue (color.B == b);
			
			Assert.AreEqual ("#C;#6;#199A;#9", color.ToString
				(), "Color.ToString failure");
		}
		
		[Test] public void CheckRanges()
		{
			Color color = Color.FromAlphaRgb (1.8, 0.5, -0.1, 10);
			
			Assert.IsTrue (color.IsValid);
			Assert.IsTrue (color.IsInRange == false);
			
			color = color.ClipToRange ();
			
			Assert.IsTrue (color.IsInRange);
			Assert.IsTrue (color.A == 1.0);
			Assert.IsTrue (color.G == 0.0);
			Assert.IsTrue (color.B == 1.0);
		}
		
		[Test] public void CheckFromNameAndHexa()
		{
			Color color_1 = Color.FromAlphaRgb (1, 1, 0, 0);
			Color color_2 = Color.FromName ("red");
			Color color_3 = Color.FromName ("#ff0000");
			Color color_4 = Color.FromName ("#blabla");
			
			Assert.AreEqual (color_1, color_2);
			Assert.AreEqual (color_1, color_3);
			Assert.AreEqual (Color.Empty, color_4);
		}
		
		
		[Test] public void CheckToStringParse()
		{
			Color c1 = new Color (128.0/255.0, 32.0/255.0, 5.0/255.0);
			Color c2 = new Color (0.0, 2.0/15.0, 5.0/15.0);
			Color c3 = new Color (1.0, 1.0, 1.0);
			Color c4 = new Color (0.5, 0.2, 0.1);
			
			Assert.AreEqual ("#80;#20;#05", Color.ToString (c1));
			Assert.AreEqual ("#0;#2;#5", Color.ToString (c2));
			Assert.AreEqual ("#F;#F;#F", Color.ToString (c3));
			Assert.AreEqual ("#8000;#3;#199A", Color.ToString (c4));
			
			Assert.IsTrue (Color.Parse (Color.ToString (c1)) == c1);
			Assert.IsTrue (Color.Parse (Color.ToString (c2)) == c2);
			Assert.IsTrue (Color.Parse (Color.ToString (c3)) == c3);
			Assert.IsTrue (Color.Parse (Color.ToString (c4)) == c4);
		}
		
		[Test] public void CheckRichToStringParse()
		{
			RichColor c1 = new RichColor (128.0/255.0, 32.0/255.0, 5.0/255.0);
			RichColor c2 = new RichColor (0.95, 0.0, 2.0/15.0, 5.0/15.0);
			RichColor c3 = new RichColor (1.0, 1.0, 1.0);
			RichColor c4 = new RichColor (1.0, 0.5, 0.2, 0.1, 0x1234/65535.0);
			
			Assert.AreEqual ("RGB;#80;#20;#05", RichColor.ToString (c1));
			Assert.AreEqual ("αRGB;#F332;#0;#2;#5", RichColor.ToString (c2));
			Assert.AreEqual ("RGB;#F;#F;#F", RichColor.ToString (c3));
			Assert.AreEqual ("CMYK;#8000;#3;#199A;#1234", RichColor.ToString (c4));
			
			Assert.IsTrue (RichColor.Parse (RichColor.ToString (c1)) == c1);
			Assert.IsTrue (RichColor.Parse (RichColor.ToString (c2)) == c2);
			Assert.IsTrue (RichColor.Parse (RichColor.ToString (c3)) == c3);
			Assert.IsTrue (RichColor.Parse (RichColor.ToString (c4)) == c4);
		}
		
		[Test] public void CheckNamedColors()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (870, 360);
			window.Text = "CheckNamedColors";
			window.Root.PaintForeground += NamedColors_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckHsvConvert()
		{
			Color c1, c2;
			double h,s,v;

			//	rouge
			c1 = Color.FromRgb(1.0, 0.0, 0.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	jaune
			c1 = Color.FromRgb(1.0, 1.0, 0.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 60.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	vert
			c1 = Color.FromRgb(0.0, 1.0, 0.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 120.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	cyan
			c1 = Color.FromRgb(0.0, 1.0, 1.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 180.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	bleu
			c1 = Color.FromRgb(0.0, 0.0, 1.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 240.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	magenta
			c1 = Color.FromRgb(1.0, 0.0, 1.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 300.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	rose
			c1 = Color.FromRgb(1.0, 0.5, 0.5);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 0.5);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	brun-rouge
			c1 = Color.FromRgb(0.5, 0.0, 0.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 1.0);
			Assert.IsTrue(v == 0.5);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	brun
			c1 = Color.FromRgb(0.5, 0.25, 0.25);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 0.5);
			Assert.IsTrue(v == 0.5);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	gris
			c1 = Color.FromRgb(0.5, 0.5, 0.5);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 0.0);
			Assert.IsTrue(v == 0.5);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	blanc
			c1 = Color.FromRgb(1.0, 1.0, 1.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 0.0);
			Assert.IsTrue(v == 1.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	noir
			c1 = Color.FromRgb(0.0, 0.0, 0.0);
			c1.GetHsv(out h, out s, out v);
			Assert.IsTrue(h == 0.0);
			Assert.IsTrue(s == 0.0);
			Assert.IsTrue(v == 0.0);
			c2 = Color.FromHsv(h,s,v);
			Assert.IsTrue(c1 == c2);

			//	jaune
			c1 = Color.FromHsv(60+360,1,1);
			Assert.IsTrue(c1.R == 1.0);
			Assert.IsTrue(c1.G == 1.0);
			Assert.IsTrue(c1.B == 0.0);

			c1 = Color.FromHsv(60-360,1,1);
			Assert.IsTrue(c1.R == 1.0);
			Assert.IsTrue(c1.G == 1.0);
			Assert.IsTrue(c1.B == 0.0);
		}

		
		private void NamedColors_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double dx = root.Client.Size.Width;
			double dy = root.Client.Size.Height;
			
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

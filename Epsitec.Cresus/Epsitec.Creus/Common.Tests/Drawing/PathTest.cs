using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class PathTest
	{
		[Test] public void CheckAppendPaths()
		{
			Path p1  = new Path ();
			Path p1r = new Path ();
			Path p2  = new Path ();
			
			p1.MoveTo (10, 10);
			p1.LineTo (50, 10);
			p1.LineTo (30, 60);
			p1.Close ();
			
			p1r.MoveTo (10, 10);
			p1r.LineTo (30, 60);
			p1r.LineTo (50, 10);
			p1r.Close ();
			
			p2.MoveTo (20, 5);
			p2.LineTo (60, 0);
			p2.LineTo (15, 40);
			p2.Close ();
			
			Rectangle r1 = p1.ComputeBounds ();
			Rectangle r2 = p2.ComputeBounds ();
			Rectangle r  = Rectangle.Union (r1, r2);
			
			System.Console.Out.WriteLine ("Path1:");
			System.Console.Out.WriteLine (p1.ToString ());
			System.Console.Out.WriteLine ("Path2:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			Path p = new Path ();
			
			p.Append (p1);
			Assert.AreEqual (r1, p.ComputeBounds ());
			
			System.Console.Out.WriteLine ("Path: p1");
			System.Console.Out.WriteLine (p.ToString ());
			
			p.Append (p2);
			
			System.Console.Out.WriteLine ("Path: p1 UNION p2");
			System.Console.Out.WriteLine (p.ToString ());
			
			Assert.AreEqual (r, p.ComputeBounds ());
			
			Path pp1 = new Path();
			Path pp2 = new Path();
			
			pp1.Append(p1,  1, 2);
			pp2.Append(p1r, 1, 2);
			
			System.Console.Out.WriteLine ("Path: direct, bold 2");
			System.Console.Out.WriteLine (pp1.ToString ());
			
			System.Console.Out.WriteLine ("Path: reversed, bold 2");
			System.Console.Out.WriteLine (pp2.ToString ());
		}
		
		[Test] public void CheckCombinePaths()
		{
			Path p1  = new Path ();
			Path p1r = new Path ();
			Path p2  = new Path ();
			
			p1.MoveTo (10, 10);
			p1.LineTo (50, 10);
			p1.LineTo (30, 60);
			p1.Close ();
			
			p1r.MoveTo (10, 10);
			p1r.LineTo (30, 60);
			p1r.LineTo (50, 10);
			p1r.Close ();
			
			p2.MoveTo (20, 5);
			p2.LineTo (60, 0);
			p2.LineTo (15, 40);
			p2.Close ();
			
			Rectangle r1 = p1.ComputeBounds ();
			Rectangle r2 = p2.ComputeBounds ();
			Rectangle r  = Rectangle.Union (r1, r2);
			
			System.Console.Out.WriteLine ("Path1:");
			System.Console.Out.WriteLine (p1.ToString ());
			System.Console.Out.WriteLine ("Path2:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			Path p = Path.Combine (p1, p2, PathOperation.Or);
			
			System.Console.Out.WriteLine ("Path: p1 OR p2");
			System.Console.Out.WriteLine (p.ToString ());
			
			Assert.AreEqual (r, p.ComputeBounds ());
			
			p = Path.Combine (p1, p2, PathOperation.And);
			
			System.Console.Out.WriteLine ("Path: p1 AND p2");
			System.Console.Out.WriteLine (p.ToString ());
			
			p = Path.Combine (p1, p2, PathOperation.AMinusB);
			
			System.Console.Out.WriteLine ("Path: p1 MINUS p2");
			System.Console.Out.WriteLine (p.ToString ());
			
			p = Path.Combine (p1, p2, PathOperation.BMinusA);
			
			System.Console.Out.WriteLine ("Path: p2 MINUS p1");
			System.Console.Out.WriteLine (p.ToString ());
			
			p = Path.Combine (p1, p2, PathOperation.Xor);
			
			System.Console.Out.WriteLine ("Path: p1 XOR p2");
			System.Console.Out.WriteLine (p.ToString ());
		}
		
		[Test] public void CheckAppendGlyph()
		{
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			
			ushort glyph = font.GetGlyphIndex ('f');
			
			path.Append (font, glyph, 0, 0, 12.0);
			
			System.Console.Out.WriteLine ("Glyph path:");
			System.Console.Out.WriteLine (path.ToString ());
			
			Rectangle rp = path.ComputeBounds ();
			Rectangle rf = font.GetGlyphBounds (glyph);
			
			rf.Scale (12.0);
			
			Assert.AreEqual (rp, rf);
		}
		
		[Test] public void CheckGetElementsSetElements()
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('a');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			Path copy = new Path ();
			
			PathElement[] elements;
			Point[]       points;
			
			path.GetElements (out elements, out points);
			copy.SetElements (elements, points);
			
			Assert.AreEqual (path.ToString (), copy.ToString ());
		}
		
		[Test] public void CheckGetBlobOfElementsSetBlobOfElements()
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('a');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			Path   copy = new Path ();
			byte[] blob = path.GetBlobOfElements ();
			
			System.Console.Out.WriteLine ("Blob size: {0}", blob.Length);
			
			copy.SetBlobOfElements (blob);
			
			Assert.AreEqual (path.ToString (), copy.ToString ());
		}
		
		[Test] public void CheckGlyphPathToStringTimes()
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('a');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			System.Console.Out.WriteLine (path.ToString ());
		}
		
		[Test] public void CheckGlyphPathToStringTahoma()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('i');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			System.Console.Out.WriteLine (path.ToString ());
		}
		
		[Test] public void CheckComputeBounds()
		{
			Path path = new Path ();
			Rectangle bounds;
			
			path.MoveTo (110, 110);
			path.LineTo (110, 210);
			path.LineTo (210, 160);
			path.Close ();
			
			bounds = path.ComputeBounds ();
			Assert.AreEqual (110, bounds.Left);
			Assert.AreEqual (110, bounds.Bottom);
			Assert.AreEqual (210, bounds.Right);
			Assert.AreEqual (210, bounds.Top);
			
			path.MoveTo (120, 125);
			path.LineTo (195, 160);
			path.LineTo (120, 195);
			path.Close ();
			
			bounds = path.ComputeBounds ();
			Assert.AreEqual (110, bounds.Left);
			Assert.AreEqual (110, bounds.Bottom);
			Assert.AreEqual (210, bounds.Right);
			Assert.AreEqual (210, bounds.Top);
		}
		
		[Test] public void CheckSystemInterop()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			form.Paint += this.HandleFormPaint;
			form.Width  = 800;
			form.Height = 400;
			form.Text   = "CheckSystemInterop";
			form.SizeChanged += this.HandleFormSizeChanged;
			
			form.Show ();
		}

		[Test] public void CheckAppendPathOutlines1()
		{
			Path p1  = new Path ();
			Path p2  = new Path ();
			Path p3  = new Path ();
			
			p1.MoveTo ( 5.5,  5.5);
			p1.LineTo (15.5,  5.5);
			p1.LineTo (15.5, 15.5);
			p1.LineTo ( 5.5, 15.5);
			p1.Close ();
			
			System.Console.Out.WriteLine ("Path:");
			System.Console.Out.WriteLine (p1.ToString ());
			
			p2.Append (p1, 1.0, CapStyle.Round, JoinStyle.Round, 5.0, 2);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 2:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			p3.Append (p1, 1.0, CapStyle.Round, JoinStyle.Round, 5.0, 29.4);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 29.4:");
			System.Console.Out.WriteLine (p3.ToString ());
		}
		
		[Test] public void CheckAppendPathOutlines2()
		{
			Path p1  = new Path ();
			Path p2  = new Path ();
			Path p3  = new Path ();
			
			p1.MoveTo ( 5.5, 15.5);
			p1.LineTo ( 5.5,  5.5);
			p1.LineTo (15.5,  5.5);
			p1.LineTo (15.5, 15.5);
			p1.Close ();
			
			System.Console.Out.WriteLine ("Path:");
			System.Console.Out.WriteLine (p1.ToString ());
			
			p2.Append (p1, 1.0, CapStyle.Round, JoinStyle.Round, 5.0, 2.0);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 2.0:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			p3.Append (p1, 1.0, CapStyle.Round, JoinStyle.Round, 5.0, 20.0);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 20.0:");
			System.Console.Out.WriteLine (p3.ToString ());
		}
		
		[Test] public void CheckAppendPathOutlines3()
		{
			Path p1  = new Path ();
			Path p2  = new Path ();
			Path p3  = new Path ();
			
			p1.MoveTo ( 55, 155);
			p1.LineTo ( 55,  55);
			p1.LineTo (155,  55);
			p1.LineTo (155, 155);
			p1.Close ();
			
			System.Console.Out.WriteLine ("Path:");
			System.Console.Out.WriteLine (p1.ToString ());
			
			p2.Append (p1, 10.0, CapStyle.Round, JoinStyle.Round, 5.0, 2);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 2:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			p3.Append (p1, 10.0, CapStyle.Round, JoinStyle.Round, 5.0, 29.4);
			
			System.Console.Out.WriteLine ("Path approximation zoom = 29.4:");
			System.Console.Out.WriteLine (p3.ToString ());
		}
		
		
		static readonly int cpu_speed = 1700;
		
		private void HandleFormPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
			
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			
			path.Append (font, font.GetGlyphIndex ('f'),  70, 100, 300.0);
			path.Append (font, font.GetGlyphIndex ('j'), 200, 100, 300.0);
			path.Append (font, font.GetGlyphIndex ('@'), 400, 100, 300.0);
			
			System.Drawing.Drawing2D.GraphicsPath os_path = path.CreateSystemPath ();
			
			long cc = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c0 = c2 - c1;
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			e.Graphics.TranslateTransform (0, form.ClientSize.Height);
			e.Graphics.ScaleTransform (1, -1);
			e.Graphics.DrawPath (System.Drawing.Pens.Black, os_path);
			e.Graphics.FillPath (System.Drawing.Brushes.Gold, os_path);
			
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			e.Graphics.ResetTransform ();
			e.Graphics.FillRectangle (System.Drawing.Brushes.White, 0, 0, form.Width, form.Height);
			e.Graphics.DrawString (string.Format ("Drawing & Filling the path: {1} ms, high speed.", c2, c2 / cpu_speed / 1000), form.Font, System.Drawing.Brushes.Black, 10, 5);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			e.Graphics.TranslateTransform (0, form.ClientSize.Height);
			e.Graphics.ScaleTransform (1, -1);
			e.Graphics.DrawPath (System.Drawing.Pens.Black, os_path);
			e.Graphics.FillPath (System.Drawing.Brushes.Gold, os_path);
			
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			e.Graphics.ResetTransform ();
			e.Graphics.DrawString (string.Format ("Drawing & Filling the path: {1} ms, high quality.", c2, c2 / cpu_speed / 1000), form.Font, System.Drawing.Brushes.Black, 10, 20);
			
			//	Mesures: 3ms en HighSpeed, 15ms en HighQuality; en cas de redimensionnement: 50ms et 200ms ?!
			
#if false
			e.Graphics.DrawString ("Hello, world. 14pt", font.GetOsFont (14.0), System.Drawing.Brushes.Black, 10,  80);
			e.Graphics.DrawString ("Hello, world. 13pt", font.GetOsFont (13.0), System.Drawing.Brushes.Black, 10,  90);
			e.Graphics.DrawString ("Hello, world. 12pt", font.GetOsFont (12.0), System.Drawing.Brushes.Black, 10, 100);
			e.Graphics.DrawString ("Hello, world. 11pt", font.GetOsFont (11.0), System.Drawing.Brushes.Black, 10, 110);
			e.Graphics.DrawString ("Hello, world. 10pt", font.GetOsFont (10.0), System.Drawing.Brushes.Black, 10, 120);
			e.Graphics.DrawString ("Hello, world. 9pt",  font.GetOsFont (9.0),  System.Drawing.Brushes.Black, 10, 130);
			
			this.DrawString ("Hello, world. 14pt", 10, 60, 14, e.Graphics, font);
#endif

		}

#if false
		private void DrawString(string text, double x, double y, double size, System.Drawing.Graphics os_graphics, Font font)
		{
			System.Drawing.Font os_font = font.GetOsFont (size);
			
			double[] xxx;
			double ox = 0;
			
			font.GetTextCharEndX (text, out xxx);
			
			for (int i = 0; i < text.Length; i++)
			{
				os_graphics.DrawString (text.Substring (i, 1), os_font, System.Drawing.Brushes.Black, (float) x, (float) y);
				
				x += (xxx[i]-ox) * size;
				ox = xxx[i];
			}
		}
#endif

		private void HandleFormSizeChanged(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
			form.Invalidate ();
		}
	}
}

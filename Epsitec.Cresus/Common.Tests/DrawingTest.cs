using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class Drawing
	{
		public Drawing()
		{
		}
		
		[Test] public void CheckVersion()
		{
			string version = Common.Drawing.Agg.Library.Current.Version;
			string product = Common.Drawing.Agg.Library.Current.ProductName;
			
			Assertion.AssertNotNull (version);
			Assertion.AssertNotNull (product);
			
			System.Console.Out.WriteLine ("Version: " + version);
			System.Console.Out.WriteLine ("Product: " + product);
		}

		[Test] public void CheckPixmap()
		{
			Pixmap pixmap = new Pixmap ();
			
			pixmap.Size = new System.Drawing.Size (200, 100);
			pixmap.Size = new System.Drawing.Size (100, 50);
			
			pixmap.Dispose ();
		}

		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx1()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetColors (0, 0, 0, 0, 1, 1, 1, 1);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx2()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetParameters (0, 100);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx3()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.Fill = Common.Drawing.GradientFill.Conic;
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckRendererGradientEx4()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetColors (new double[100], new double[256], new double[256], new double[256]);
		}
		
		
		[Test] public void CheckRendererGradient()
		{
			Pixmap pixmap = new Pixmap ();
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			
			pixmap.Size = new System.Drawing.Size (200, 200);
			gradient.Pixmap = pixmap;
			gradient.Fill = Common.Drawing.GradientFill.Circle;
			gradient.SetColors (Color.FromBrightness (0.0), Color.FromBrightness (1.0));
			gradient.SetParameters (0, 100);
			
			gradient.Dispose ();
			pixmap.Dispose ();
		}
		
		[Test] public void CheckPixmapInForm()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.Size = new System.Drawing.Size (600, 300);
			form.Text = "Pixmap + RendererSolid";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint);
			
			this.global_pixmap = new Pixmap ();
			this.global_pixmap.Size = form.ClientSize;
			this.global_pixmap.Clear ();
			
			this.global_renderer_solid = new Common.Drawing.Renderer.Solid ();
			this.global_renderer_solid.Pixmap = this.global_pixmap;
			this.global_renderer_solid.Clear (Color.FromRGB (0, 0.5, 1.0));
			
			int angle = 5;
			
			double cx = form.ClientSize.Width / 2;
			double cy = form.ClientSize.Height / 2;
			
			Point pt = new Point (cx, cy);
			Transform tr = Transform.FromRotation (angle, pt);
			
			this.global_rasterizer = new Rasterizer ();
			this.global_rasterizer.Transform = tr;
			
			Path path = new Path ();
			
			path.Clear ();
			path.MoveTo (cx-5, cy-5);
			path.LineTo (cx+5, cy-5);
			path.LineTo (cx+5, cy+5);
			path.LineTo (cx-5, cy+5);
			path.Close ();
			path.MoveTo (10, 10);
			path.LineTo (110, 60);
			path.LineTo (10, 110);
			path.Close ();
			path.MoveTo (20, 25);
			path.LineTo (95, 60);
			path.LineTo (20, 95);
			path.Close ();
			
			this.global_renderer_solid.SetColor (Color.FromBrightness (1.0));
			this.global_rasterizer.FillMode = FillMode.EvenOdd;
			this.global_rasterizer.AddSurface (path);
			this.global_rasterizer.Render (this.global_renderer_solid);
			
			path.Clear ();
			path.MoveTo (10+.5, 10+.5);
			path.LineTo (110-.5, 60);
			path.LineTo (10+.5, 110-.5);
			path.Close ();
			
			this.global_renderer_solid.SetColor (1, 0, 0, 1);
			this.global_rasterizer.AddOutline (path, 2);
			this.global_rasterizer.Render (this.global_renderer_solid);
			
			path.Clear ();
			path.MoveTo  (200, 150);
			path.CurveTo (200, 100, 150, 100);
			path.CurveTo (100, 100, 100, 150);
			path.CurveTo (100, 200, 150, 200);
			path.CurveTo (200, 200, 200, 150);
			path.Close ();
			path.MoveTo  (200-20, 150);
			path.CurveTo (200-20, 100+20, 150,    100+20);
			path.CurveTo (100+20, 100+20, 100+20, 150);
			path.CurveTo (100+20, 200-20, 150,    200-20);
			path.CurveTo (200-20, 200-20, 200-20, 150);
			path.Close ();
			
			this.global_renderer_solid.SetColor (0, 1, 0, 1);
			this.global_rasterizer.AddSurface (path);
			this.global_rasterizer.Render (this.global_renderer_solid);
			
			Font font = Font.GetFont ("Tahoma", "Regular", "");
			Assertion.AssertNotNull (font);
			
			double x = 10;
			
			this.global_rasterizer.FillMode = FillMode.NonZero;
			
			this.global_rasterizer.AddGlyph (font, font.GetGlyphIndex ('H'), x, 50, 20);	x += font.GetCharAdvance ('H') * 20;
			this.global_rasterizer.AddGlyph (font, font.GetGlyphIndex ('a'), x, 50, 20);	x += font.GetCharAdvance ('a') * 20;
			this.global_rasterizer.AddGlyph (font, font.GetGlyphIndex ('&'), x, 50, 20);	x += font.GetCharAdvance ('&') * 20;
			this.global_renderer_solid.SetColor (0, 0, 0, 0.8);
			this.global_rasterizer.Render (this.global_renderer_solid);
			
			string text = "AGG says: 'The quick brown fox jumps over the lazy dog', and it's quite fast... Pierre Arnaud. Yverdon !";
			x = 10;
			
			foreach (char c in text)
			{
				this.global_rasterizer.AddGlyph (font, font.GetGlyphIndex (c), x, 80, 12);	x += font.GetCharAdvance (c) * 12;
				System.Console.Out.WriteLine ("'" + c + "' => " + font.GetGlyphIndex (c) + ", width = " + font.GetCharAdvance (c));
			}
			
			this.global_renderer_solid.SetColor (0, 0, 0, 0.8);
			this.global_rasterizer.Render (this.global_renderer_solid);
			
			path.Clear ();
			path.MoveTo (60, 200);
			path.LineTo (80, 30);
			path.LineTo (280, 250);
			path.Close ();
			
			this.global_renderer_gradient = new Common.Drawing.Renderer.Gradient ();
			
			this.global_renderer_gradient.Pixmap = this.global_pixmap;
			this.global_renderer_gradient.Fill = Common.Drawing.GradientFill.Circle;
			this.global_renderer_gradient.SetColors (0,0,0.5,1,  0,0,0.5,0);		//	R,G,B,A -> R,G,B,A
			this.global_renderer_gradient.SetParameters (0, 150);
			this.global_renderer_gradient.Transform = Transform.Multiply (tr, Transform.FromTranslation (170, 140));
			
			
			this.global_rasterizer.AddOutline (path, 15, CapStyle.Round, JoinStyle.Round);
			this.global_rasterizer.Render (this.global_renderer_gradient);
			
			form.Show ();
		}

		private void form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap.Paint (e.Graphics, e.ClipRectangle);
		}
		
		
		Common.Drawing.Rasterizer			global_rasterizer;
		Common.Drawing.Renderer.Solid		global_renderer_solid;
		Common.Drawing.Renderer.Gradient	global_renderer_gradient;
		Common.Drawing.Pixmap				global_pixmap;
	}
}

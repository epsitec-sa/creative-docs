using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class FontTest
	{
		public FontTest()
		{
			Font.Initialise ();
		}
		
		[Test] /*[Ignore ("too slow") ]*/ public void CheckInit()
		{
			int n = Font.Count;
			System.Console.Out.WriteLine (n.ToString () + " fonts found");
			
			for (int i = 0; i < n; i++)
			{
				Font font = Font.GetFont (i);
				Assertion.AssertNotNull (font);
				
				if (font.GetGlyphIndex ('e') == 0)
				{
					System.Console.WriteLine (font.FullName + " (" + font.LocalStyleName + ") is not a Latin font");
				}
				else
				{
					System.Console.Out.WriteLine (font.FullName + " (" + font.LocalStyleName + ") / e=" + font.GetGlyphIndex ('e').ToString ());
					System.Console.Out.WriteLine ("  A=" + font.Ascender + " D=" + font.Descender + " H=" + (font.LineHeight-font.Ascender+font.Descender) + " w=" + font.GetCharAdvance ('e'));
				}
				Font find = Font.GetFont (font.FaceName, font.StyleName, font.OpticalName);
				
				Assertion.AssertNotNull (find);
				Assertion.AssertEquals (font.Handle, find.Handle);
			}
			
			Assertion.AssertNull (Font.GetFont (n+1));
		}

		[Test] public void CheckSyntheticFont()
		{
			Font font = Font.GetFont ("Tahoma", "Italic");
			
			Assertion.AssertNotNull (font);
			Assertion.AssertEquals ("Oblique", font.StyleName);
			Assertion.AssertEquals (true, font.IsSynthetic);
			Assertion.AssertEquals (font, Font.GetFont ("Tahoma", "Italic"));
			Assertion.AssertEquals (font, Font.GetFont ("Tahoma", "Oblique"));
			
			font = Font.GetFont ("Tahoma", "Bold Italic");
			
			Assertion.AssertNotNull (font);
			Assertion.AssertEquals ("Bold Oblique", font.StyleName);
			Assertion.AssertEquals (true, font.IsSynthetic);
			Assertion.AssertEquals (font, Font.GetFont ("Tahoma", "Bold Italic"));
			Assertion.AssertEquals (font, Font.GetFont ("Tahoma", "Bold Oblique"));
		}
		
		[Test] public void CheckFontGeometry()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			
			Assertion.AssertNotNull (font);
			
			double ascender  = font.Ascender;
			double descender = font.Descender;
			double height    = font.LineHeight;
			
			Assertion.Assert (ascender > 0);
			Assertion.Assert (descender < 0);
			Assertion.Assert (height >= ascender-descender);
		}
		
		[Test] public void CheckFontTextCharEndX()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			
			Assertion.AssertNotNull (font);
			
			string   text  = "Hello";
			double[] end_x = font.GetTextCharEndX (text);
			double   width = font.GetTextAdvance (text);
			
			Assertion.Assert (end_x.Length == text.Length);
			Assertion.Assert (end_x[end_x.Length-1] == width);
		}
		
		[Test] public void CheckTextBreak()
		{
			Font   font = Font.GetFont ("Times New Roman", "Regular");
			string text = "The quick brown     fox jumps over the lazy dog. Whatever, we just need a piece of long text to break apart.";
			double width = 100;
			TextBreak tb = new TextBreak (font, text, 12.0, TextBreakMode.None);
			
			string break_text;
			double break_width;
			
			int line_count = 0;
			int n_char;
			string[] chunk = new string[10];
			
			while (tb.GetNextBreak (width, out break_text, out break_width, out n_char))
			{
				Assertion.Assert (break_width <= width);
				Assertion.Assert (break_text.Length > 0);
				Assertion.Assert (n_char > 0);
				Assertion.Assert (! break_text.StartsWith (" "));
				Assertion.Assert (! break_text.EndsWith (" "));
				
				chunk[line_count++] = break_text;
				
				Assertion.AssertEquals (break_width, font.GetTextAdvance (break_text)*12.0);
				Assertion.AssertEquals ((line_count < 6), tb.MoreText);
			}
			
			Assertion.AssertEquals (6, line_count);
			Assertion.AssertEquals ("", break_text);
			Assertion.AssertEquals (0, break_width);
			Assertion.AssertEquals (0, n_char);
			
			Assertion.AssertEquals ("apart.", chunk[5]);
			
			Assertion.AssertEquals (false, tb.GetNextBreak (width, out break_text, out break_width, out n_char));
			
			tb.Dispose ();
			
			tb = new TextBreak (font, "absolutely   ", 12.0, TextBreakMode.None);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("", break_text);
			Assertion.AssertEquals (0.0, break_width);
			Assertion.AssertEquals (true, tb.GetNextBreak (55.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("absolutely", break_text);
			Assertion.AssertEquals (false, tb.GetNextBreak (55.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreak (font, "absolutely   ", 12.0, TextBreakMode.None);
			Assertion.AssertEquals (true, tb.GetNextBreak (60.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("absolutely   ", break_text);
			Assertion.AssertEquals (false, tb.GetNextBreak (60.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreak (font, "absolutely, really", 12.0, TextBreakMode.Split);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("absolute", break_text);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("ly,", break_text);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("really", break_text);
			Assertion.AssertEquals (false, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreak (font, "absolutely, really", 12.0, TextBreakMode.Ellipsis);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("absol\u2026", break_text);
			Assertion.AssertEquals (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assertion.AssertEquals ("really", break_text);
			Assertion.AssertEquals (false, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
		}
		
		[Test] public void CheckGlyphPaint()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (460, 320);
			form.Text = "CheckGlyphPaint";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint1);
			
			this.global_pixmap_1 = new Pixmap ();
			this.global_pixmap_1.Size = form.ClientSize;
			this.global_pixmap_1.Clear ();
			
			Epsitec.Common.Drawing.Renderer.Solid renderer = new Common.Drawing.Agg.SolidRenderer ();
			renderer.Pixmap = this.global_pixmap_1;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Agg.Rasterizer ();
			rasterizer.Gamma = 1.2;
			
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			Path rect = new Path ();
			
			double x = 20;
			double y = 80;
			double size = 320;
			
			path.MoveTo (0, y);
			path.LineTo (form.ClientSize.Width, y);
			
			string text = "tjfa";
			
			rasterizer.FillMode = FillMode.NonZero;
			
			foreach (char c in text)
			{
				int glyph = font.GetGlyphIndex (c);
				
				rasterizer.AddGlyph (font, glyph, x, y, size);
				
				path.MoveTo (x, 0);
				path.LineTo (x, form.ClientSize.Height);
			
				Rectangle r = font.GetGlyphBounds (glyph);
				r.Scale (size);
				r.Offset (x, y);
			
				rect.MoveTo (r.Left,  r.Bottom);
				rect.LineTo (r.Right, r.Bottom);
				rect.LineTo (r.Right, r.Top);
				rect.LineTo (r.Left,  r.Top);
				rect.Close ();
				
				x += font.GetGlyphAdvance (glyph) * size;
			}
			
			path.MoveTo (x, 0);
			path.LineTo (x, form.ClientSize.Height);
			
			renderer.Color = Color.FromBrightness (0);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRGB (0, 1, 0);
			rasterizer.AddOutline (path, 1);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRGB (1, 0, 0);
			rasterizer.AddOutline (rect, 1);
			rasterizer.Render (renderer);
			
			form.Show ();
		}

		[Test] public void CheckSyntheticGlyphPaint()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (460, 320);
			form.Text = "CheckSyntheticGlyphPaint";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint2);
			
			this.global_pixmap_2 = new Pixmap ();
			this.global_pixmap_2.Size = form.ClientSize;
			this.global_pixmap_2.Clear ();
			
			Epsitec.Common.Drawing.Renderer.Solid renderer = new Common.Drawing.Agg.SolidRenderer ();
			renderer.Pixmap = this.global_pixmap_2;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Agg.Rasterizer ();
			rasterizer.Gamma = 1.2;
			rasterizer.Transform = Transform.FromScale (0.8, 0.8);
			
			Font font  = Font.GetFont ("Tahoma", "Italic");
			Font font2 = Font.GetFont ("Tahoma", "Regular");
			Path path  = new Path ();
			Path path2 = new Path ();
			Path rect  = new Path ();
			
			double x = 20;
			double y = 80;
			double size = 320;
			
			path.MoveTo (0, y);
			path.LineTo (form.ClientSize.Width, y);
			
			string text = "tjfa";
			
			rasterizer.FillMode = FillMode.NonZero;
			
			foreach (char c in text)
			{
				int glyph = font.GetGlyphIndex (c);
				
				rasterizer.AddGlyph (font, glyph, x, y, size);
				
				path2.Append (font2, glyph, x, y, size);
				
				path.MoveTo (x, 0);
				path.LineTo (x, form.ClientSize.Height);
				
				Rectangle r = font.GetGlyphBounds (glyph);
				r.Scale (size);
				r.Offset (x, y);
				
				rect.MoveTo (r.Left,  r.Bottom);
				rect.LineTo (r.Right, r.Bottom);
				rect.LineTo (r.Right, r.Top);
				rect.LineTo (r.Left,  r.Top);
				rect.Close ();
				
				x += font.GetGlyphAdvance (glyph) * size;
			}
			
			path.MoveTo (x, 0);
			path.LineTo (x, form.ClientSize.Height);
			
			renderer.Color = Color.FromBrightness (0);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromARGB (0.2, 0, 0, 0);
			rasterizer.AddSurface (path2);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRGB (0, 1, 0);
			rasterizer.AddOutline (path, 1);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRGB (1, 0, 0);
			rasterizer.AddOutline (rect, 1);
			rasterizer.Render (renderer);
			
			
			
			form.Show ();
		}

		[Test] public void CheckDefaultFont()
		{
			Font   font = Font.DefaultFont;
			double size = Font.DefaultFontSize;
			
			Assertion.AssertNotNull (font);
			Assertion.Assert (size > 0);
		}

		[Test] public void CheckGlyphDynamicBold()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (160, 480);
			form.Text = "CheckGlyphPaint";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint3);
			
			this.global_pixmap_3 = new Pixmap ();
			this.global_pixmap_3.Size = form.ClientSize;
			this.global_pixmap_3.Clear ();
			
			Epsitec.Common.Drawing.Renderer.Solid renderer = new Common.Drawing.Agg.SolidRenderer ();
			renderer.Pixmap = this.global_pixmap_3;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Agg.Rasterizer ();
			rasterizer.Gamma = 1.2;
			rasterizer.Transform = Transform.FromScale (0.25, 0.25);
			
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			for (int bold_width = 0; bold_width < 40; bold_width += 8)
			{
				double size = 320;
				double x = 20 + bold_width / 2;
				double y = 80 + bold_width / 2 * (font.Ascender) / font.LineHeight + size * bold_width / 8;
				
				path.MoveTo (0, y);
				path.LineTo (form.ClientSize.Width, y);
				
				string text = "tjfa";
				
				rasterizer.FillMode = FillMode.NonZero;
				
				foreach (char c in text)
				{
					int glyph = font.GetGlyphIndex (c);
					
					path.Append (font, glyph, size, 0, 0, size, x, y, bold_width);
					
					x += font.GetGlyphAdvance (glyph) * size + bold_width;
				}
				
				renderer.Color = Color.FromARGB (1, 0, 0, 0);
				rasterizer.AddSurface (path);
				rasterizer.Render (renderer);
				
				path.Clear ();
			}
			
			form.Show ();
		}
		static readonly int cpu_speed = 1700;

		[Test] public void CheckRenderingSpeed()
		{
			Graphics gra  = new Epsitec.Common.Drawing.Agg.Graphics ();
			Font     font = Font.GetFont ("Tahoma", "Regular");
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			
			long c1 = AntiGrain.Interface.DebugGetCycleDelta ();
			long c2 = AntiGrain.Interface.DebugGetCycleDelta ();
			long c0 = AntiGrain.Interface.DebugGetCycleDelta ();
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = AntiGrain.Interface.DebugGetCycleDelta ();
			AntiGrain.Interface.NoOp ();
			c2 = AntiGrain.Interface.DebugGetCycleDelta () - c0;
			System.Console.Out.WriteLine ("No-op work: " + c2.ToString ());
			
			c1 = AntiGrain.Interface.DebugGetCycleDelta ();
			AntiGrain.Interface.NoOpString (text);
			c2 = AntiGrain.Interface.DebugGetCycleDelta () - c0;
			System.Console.Out.WriteLine ("No-op work with string: " + c2.ToString ());
			
			gra.SetPixmapSize (1000, 400);
			gra.SolidRenderer.Color = Color.FromBrightness (0);
			
			c1 = AntiGrain.Interface.DebugGetCycleDelta ();
			gra.AddText (10, 200, text, font, size);
			gra.RenderSolid ();
			c2 = AntiGrain.Interface.DebugGetCycleDelta () - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = AntiGrain.Interface.DebugGetCycleDelta ();
				gra.AddText (10, 200, text, font, size);
				gra.RenderSolid ();
				c2 = AntiGrain.Interface.DebugGetCycleDelta () - c0;
				
				tot += c2;
			}
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / 100).ToString () + " -> " + (tot / 100 / cpu_speed / text.Length) + "us / char in AGG");
		}
		
		[Test] public void CheckRenderingSpeedGDIPlus()
		{
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			
			System.Drawing.Bitmap image = new System.Drawing.Bitmap (1000, 400, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Graphics gra = System.Drawing.Graphics.FromImage (image);
			System.Drawing.Font font = new System.Drawing.Font ("Tahoma", (float) size);
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = c2 - c1;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			gra.DrawString (text, font, System.Drawing.Brushes.Black, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
				gra.DrawString (text, font, System.Drawing.Brushes.Black, 10, 200);
				c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
				
				tot += c2;
			}
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / 100).ToString () + " -> " + (tot * 1000 / 100 / cpu_speed / text.Length) + "ns / char in GDI+");
		}
		
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] extern static void TextOut(System.IntPtr hdc, int x, int y, string text, int len);
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] extern static void SelectObject(System.IntPtr hdc, System.IntPtr hfont);

		[Test] public void CheckRenderingSpeedGDI()
		{
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			
			System.Drawing.Bitmap image = new System.Drawing.Bitmap (1000, 400, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Graphics gra = System.Drawing.Graphics.FromImage (image);
			System.Drawing.Font font = new System.Drawing.Font ("Tahoma", (float) size);
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = c2 - c1;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			System.IntPtr hFont = font.ToHfont ();
			System.IntPtr hDC   = gra.GetHdc ();
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
			
			System.Console.Out.WriteLine ("Setup: " + c2.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			SelectObject(hDC, hFont);
			TextOut (hDC, 10, 200, text, text.Length);
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
				SelectObject(hDC, hFont);
				TextOut (hDC, 10, 200, text, text.Length);
				c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
				
				tot += c2;
			}
			
			gra.ReleaseHdc (hDC);
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / 100).ToString () + " -> " + (tot * 1000 / 100 / cpu_speed / text.Length) + "ns / char in GDI");
		}
		
		[Test] public void CheckFillPixelCache()
		{
			Graphics gra  = new Epsitec.Common.Drawing.Agg.Graphics ();
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			Font     font = Font.GetFont ("Tahoma", "Regular");
			
			long cc = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = c2 - c1;
			
			System.Console.Out.WriteLine ("Timer overhead: " + (c0) + " cycles -> " + (c0 / cpu_speed) + "us");
			
			gra.SetPixmapSize (1000, 400);
			gra.SolidRenderer.Color = Color.FromBrightness (0);
			
			font.FillPixelCache ("", size, 10, 200);
			font.RenderPixelCache (gra.Pixmap, "", size, 10, 200);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			font.FillPixelCache ("", size, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
			
			System.Console.Out.WriteLine ("Calling FillPixelCache, no work done: " + (c2) + " cycles -> " + (c2 * 1000 / cpu_speed) + "ns");
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			font.FillPixelCache (text, size, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
			
			System.Console.Out.WriteLine ("Filling the cache: " + (c2) + " cycles -> " + (c2 * 1000 / cpu_speed / text.Length) + "ns / char");
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
				font.RenderPixelCache (gra.Pixmap, text, size, 10, 200);
				c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1 - c0;
				
				tot += c2;
			}
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / 100).ToString () + " cycles -> " + (tot * 10 / cpu_speed / text.Length) + "ns / char in Cached AGG");
			
			
			font.FillPixelCache (text, size, 10, 240);
			font.RenderPixelCache (gra.Pixmap, text, size, 10, 240);
			
			gra.AddText (10, 220, text, font, size);
			gra.RenderSolid ();
			
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (1000, 400);
			form.Text = "CheckFillPixelCache";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint4);
			
			this.global_pixmap_4 = gra.Pixmap;
			
			form.Show ();
		}
		
		private void form_Paint1(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap_1.Paint (e.Graphics, e.ClipRectangle);
		}
		
		private void form_Paint2(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap_2.Paint (e.Graphics, e.ClipRectangle);
		}
		
		private void form_Paint3(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap_3.Paint (e.Graphics, e.ClipRectangle);
		}
		
		private void form_Paint4(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap_4.Paint (e.Graphics, e.ClipRectangle);
		}
		
		private Pixmap					global_pixmap_1;
		private Pixmap					global_pixmap_2;
		private Pixmap					global_pixmap_3;
		private Pixmap					global_pixmap_4;
	}
}

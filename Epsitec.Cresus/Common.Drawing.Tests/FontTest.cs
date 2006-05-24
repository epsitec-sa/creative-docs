using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class FontTest
	{
		public FontTest()
		{
			Font.Initialise ();
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckGetUnicodeName()
		{
			System.Console.Out.WriteLine ("% --> {0}", TextBreak.GetUnicodeName ('%'));
			System.Console.Out.WriteLine ("¼ --> {0}", TextBreak.GetUnicodeName ('¼'));
			System.Console.Out.WriteLine ("ç --> {0}", TextBreak.GetUnicodeName ('ç'));
		}
		
		[Test] /*[Ignore ("too slow") ]*/ public void CheckInit()
		{
			int n = Font.Count;
			System.Console.Out.WriteLine (n.ToString () + " fonts found");
			
			for (int i = 0; i < n; i++)
			{
				Font font = Font.GetFont (i);
				Assert.IsNotNull (font);
				
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
				
				Assert.IsNotNull (find);
				Assert.AreEqual (font.Handle, find.Handle);
			}
			
			Assert.IsNull (Font.GetFont (n+1));
		}

		[Test] public void CheckAllFontComposites()
		{
			int n = Font.Count;
			System.Console.Out.WriteLine (n.ToString () + " fonts found");
			
			FontTest.last_font_chunk += 20;
			
			if (FontTest.last_font_chunk - 20 >= n)
			{
				FontTest.last_font_chunk = 20;
			}
			
			for (int i = FontTest.last_font_chunk - 20; i < n; i++)
			{
				if (i > FontTest.last_font_chunk) break;
				
				Font font = Font.GetFont (i);
				Assert.IsNotNull (font);
				
//				if ((font.FullName == "Haettenschweiler Regular")/* ||
//					(font.FullName == "Century Gothic Bold") ||
//					(font.FullName == "Garamond Regular") ||
//					(font.FullName == "Batang Regular")*/)
//				{
//				}
//				else
//				{
//					continue;
//				}
				
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				for (char unicode = ' '; unicode < 65500; unicode++)
				{
					int glyph_index = font.GetGlyphIndex (unicode);
					if (glyph_index > 0)
					{
						buffer.Append (unicode);
					}
				}
				if (buffer.Length > 0)
				{
					Widgets.Window         window = new Widgets.Window ();
					Widgets.TextFieldMulti text   = new Widgets.TextFieldMulti ();
					
					window.Text       = font.FullName;
					window.ClientSize = new Size (1000, 100);
					
					text.Dock   = Widgets.DockStyle.Fill;
					text.SetParent (window.Root);
					text.Text   = Widgets.TextLayout.ConvertToTaggedText (buffer.ToString ());
					text.TextLayout.DefaultFont = font;
					text.TextLayout.DefaultFontSize = 60;
					
					window.Show ();
					Widgets.Window.RunInTestEnvironment (window);
				}
			}
		}
		
		static int last_font_chunk = 0;

		[Test] public void CheckFaces()
		{
			Font.FaceInfo[] faces = Font.Faces;
			System.Console.Out.WriteLine ("{0} font faces found.", faces.Length);
			
			for (int i = 0; i < faces.Length; i++)
			{
				string   face   = faces[i].Name;
				string[] styles = faces[i].StyleNames;
				
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				Font font_regular     = faces[i].GetFont (false, false, 12.0);
				Font font_italic      = faces[i].GetFont (false, true, 12.0);
				Font font_bold        = faces[i].GetFont (true, false, 12.0);
				Font font_bold_italic = faces[i].GetFont (true, true, 12.0);
				
				if (font_regular != null     && font_regular.IsSynthetic)		buffer.Append ("* regular ");
				if (font_italic != null      && font_italic.IsSynthetic)		buffer.Append ("* italic ");
				if (font_bold != null        && font_bold.IsSynthetic)			buffer.Append ("* bold ");
				if (font_bold_italic != null && font_bold_italic.IsSynthetic)	buffer.Append ("* bold italic ");
				
				System.Console.WriteLine ("{0}: {1} {2}", face, string.Join (", ", styles), buffer.ToString ());
			}
		}
		
		[Test] public void CheckRegularFacesMissing()
		{
			Font.FaceInfo[] faces = Font.Faces;
			System.Console.Out.WriteLine ("{0} font faces found.", faces.Length);
			
			for (int i = 0; i < faces.Length; i++)
			{
				string   face   = faces[i].Name;
				string[] styles = faces[i].StyleNames;
				
				Font font_regular  = faces[i].GetFont (false, false, 12.0);
				Font fallback_font = Font.GetFontFallback (face);
				
				if (font_regular == null)
				{
					System.Console.WriteLine ("{0}: {1} --> {2}", face, string.Join (", ", styles), fallback_font.FullName);
					
					Font[] fonts = faces[i].GetFonts ();
					
					foreach (Font font in fonts)
					{
						Assert.IsFalse (font.IsStyleRegular);
						System.Console.WriteLine ("   {0}", font.UniqueName);
					}
				}
			}
		}
		
		[Test] public void CheckRegularFacesFound()
		{
			Font.FaceInfo[] faces = Font.Faces;
			System.Console.Out.WriteLine ("{0} font faces found.", faces.Length);
			
			for (int i = 0; i < faces.Length; i++)
			{
				string   face   = faces[i].Name;
				string[] styles = faces[i].StyleNames;
				
				Font font_regular = faces[i].GetFont (false, false, 12.0);
				
				if (font_regular != null)
				{
					System.Console.WriteLine ("{0}: {1}", face, string.Join (", ", styles));
					
					Assert.IsTrue (font_regular.IsStyleRegular);
					
					Font[] fonts = faces[i].GetFonts ();
					
					foreach (Font font in fonts)
					{
						System.Console.WriteLine ("   {0}", font.UniqueName);
					}
				}
			}
		}
		
		[Test] public void CheckSyntheticFont()
		{
			Font font = Font.GetFont ("Tahoma", "Italic");
			
			Assert.IsNotNull (font);
			Assert.AreEqual ("Oblique", font.StyleName);
			Assert.AreEqual (true, font.IsSynthetic);
			Assert.AreEqual (font, Font.GetFont ("Tahoma", "Italic"));
			Assert.AreEqual (font, Font.GetFont ("Tahoma", "Oblique"));
			
			font = Font.GetFont ("Tahoma", "Bold Italic");
			
			Assert.IsNotNull (font);
			Assert.AreEqual ("Bold Oblique", font.StyleName);
			Assert.AreEqual (true, font.IsSynthetic);
			Assert.AreEqual (font, Font.GetFont ("Tahoma", "Bold Italic"));
			Assert.AreEqual (font, Font.GetFont ("Tahoma", "Bold Oblique"));
		}
		
		[Test] public void CheckFontGeometry()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			
			Assert.IsNotNull (font);
			
			double ascender  = font.Ascender;
			double descender = font.Descender;
			double height    = font.LineHeight;
			
			Assert.IsTrue (ascender > 0);
			Assert.IsTrue (descender < 0);
			Assert.IsTrue (height >= ascender-descender);
		}
		
		[Test] public void CheckFontTextCharEndX()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			
			Assert.IsNotNull (font);
			
			string   text  = "Hello";
			double[] end_x;
			double   width = font.GetTextAdvance (text);
			
			font.GetTextCharEndX (text, out end_x);
			
			Assert.IsTrue (end_x.Length == text.Length);
			Assert.IsTrue (end_x[end_x.Length-1] == width);
		}
		
#if false
		[Test] public void CheckTextBreak()
		{
			Font   font = Font.GetFont ("Times New Roman", "Regular");
			string text = "The quick brown     fox jumps over the lazy dog. Whatever, we just need a piece of long text to break apart.";
			double width = 100;
			TextBreakOld tb = new TextBreakOld (font, text, 12.0, TextBreakMode.None);
			
			string break_text;
			double break_width;
			
			int line_count = 0;
			int n_char;
			string[] chunk = new string[10];
			
			while (tb.GetNextBreak (width, out break_text, out break_width, out n_char))
			{
				Assert.IsTrue (break_width <= width);
				Assert.IsTrue (break_text.Length > 0);
				Assert.IsTrue (n_char > 0);
				Assert.IsTrue (! break_text.StartsWith (" "));
//				Assert.IsTrue (! break_text.EndsWith (" "));
				
				chunk[line_count++] = break_text;
				
				Assert.AreEqual (break_width, font.GetTextAdvance (break_text)*12.0);
				Assert.AreEqual ((line_count < 6), tb.MoreText);
			}
			
			Assert.AreEqual (6, line_count);
			Assert.AreEqual ("", break_text);
			Assert.AreEqual (0, break_width);
			Assert.AreEqual (0, n_char);
			
			Assert.AreEqual ("apart.", chunk[5]);
			
			Assert.AreEqual (false, tb.GetNextBreak (width, out break_text, out break_width, out n_char));
			
			tb.Dispose ();
			
			tb = new TextBreakOld (font, "absolutely   ", 12.0, TextBreakMode.None);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("", break_text);
			Assert.AreEqual (0.0, break_width);
			Assert.AreEqual (true, tb.GetNextBreak (55.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("absolutely ", break_text);
			Assert.AreEqual (true, tb.GetNextBreak (55.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreakOld (font, "absolutely   ", 12.0, TextBreakMode.None);
			Assert.AreEqual (true, tb.GetNextBreak (60.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("absolutely   ", break_text);
			Assert.AreEqual (false, tb.GetNextBreak (60.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreakOld (font, "absolutely, really", 12.0, TextBreakMode.Split);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("absolute", break_text);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("ly,", break_text);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("really", break_text);
			Assert.AreEqual (false, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
			
			tb = new TextBreakOld (font, "absolutely, really", 12.0, TextBreakMode.Ellipsis);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("absol\u2026", break_text);
			Assert.AreEqual (true, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			Assert.AreEqual ("really", break_text);
			Assert.AreEqual (false, tb.GetNextBreak (40.0, out break_text, out break_width, out n_char));
			tb.Dispose ();
		}
#endif
		
		[Test] public void CheckGlyphPaint()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (460, 320);
			form.Text = "CheckGlyphPaint";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint1);
			
			this.global_pixmap_1 = new Pixmap ();
			this.global_pixmap_1.Size = form.ClientSize;
			this.global_pixmap_1.Clear ();
			
			Epsitec.Common.Drawing.Renderers.Solid renderer = new Common.Drawing.Renderers.Solid ();
			renderer.Pixmap = this.global_pixmap_1;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Rasterizer ();
			rasterizer.Gamma = 1.2;
			
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			Path rect = new Path ();
			
			System.Console.Out.WriteLine ("Times New Roman caret slope set to {0}.", font.CaretSlope);
			
			double x = 20;
			double y = 80;
			double size = 320;
			
			path.MoveTo (0, y);
			path.LineTo (form.ClientSize.Width, y);
			
			string text = "tjfà";
			
			Rectangle text_r = font.GetTextBounds (text);
			text_r.Scale (size);
			text_r.Offset (x, y);
			renderer.Color = Color.FromAlphaRgb (0.2, 0, 0, 1);
			
			path.MoveTo (text_r.BottomLeft);
			path.LineTo (text_r.BottomRight);
			path.LineTo (text_r.TopRight);
			path.LineTo (text_r.TopLeft);
			path.Close ();
			
			rasterizer.AddSurface (path);
			rasterizer.Render (renderer);
			
			path.Clear ();
			
//-			rasterizer.FillMode = FillMode.NonZero;
			
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
			
			renderer.Color = Color.FromRgb (0, 1, 0);
			rasterizer.AddOutline (path, 1);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRgb (1, 0, 0);
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
			
			Epsitec.Common.Drawing.Renderers.Solid renderer = new Common.Drawing.Renderers.Solid ();
			renderer.Pixmap = this.global_pixmap_2;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Rasterizer ();
			rasterizer.Gamma = 1.2;
			rasterizer.Transform = Transform.FromScale (0.8, 0.8);
			
			Font font  = Font.GetFont ("Tahoma", "Italic");
			Font font2 = Font.GetFont ("Tahoma", "Regular");
			Path path  = new Path ();
			Path path2 = new Path ();
			Path rect  = new Path ();
			
			System.Console.Out.WriteLine ("Tahoma Italic caret slope set to {0}.", font.CaretSlope);
			System.Console.Out.WriteLine ("Tahoma Regular caret slope set to {0}.", font2.CaretSlope);
			
			double x = 20;
			double y = 80;
			double size = 320;
			
			path.MoveTo (0, y);
			path.LineTo (form.ClientSize.Width, y);
			
			string text = "ijfà";
			
//-			rasterizer.FillMode = FillMode.NonZero;
			
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
			
			renderer.Color = Color.FromAlphaRgb (0.2, 0, 0, 0);
			rasterizer.AddSurface (path2);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRgb (0, 1, 0);
			rasterizer.AddOutline (path, 1);
			rasterizer.Render (renderer);
			
			renderer.Color = Color.FromRgb (1, 0, 0);
			rasterizer.AddOutline (rect, 1);
			rasterizer.Render (renderer);
			
			
			
			form.Show ();
		}

		[Test] public void CheckDefaultFont()
		{
			Font   font = Font.DefaultFont;
			double size = Font.DefaultFontSize;
			
			Assert.IsNotNull (font);
			Assert.IsTrue (size > 0);
		}

		[Test] public void CheckGlyphDynamicBold()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (160, 480);
			form.Text = "CheckGlyphDynamicBold";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint3);
			
			this.global_pixmap_3 = new Pixmap ();
			this.global_pixmap_3.Size = form.ClientSize;
			this.global_pixmap_3.Clear ();
			
			Epsitec.Common.Drawing.Renderers.Solid renderer = new Common.Drawing.Renderers.Solid ();
			renderer.Pixmap = this.global_pixmap_3;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Epsitec.Common.Drawing.Rasterizer ();
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
				
//-				rasterizer.FillMode = FillMode.NonZero;
				
				foreach (char c in text)
				{
					int glyph = font.GetGlyphIndex (c);
					
					path.Append (font, glyph, size, 0, 0, size, x, y, bold_width);
					
					x += font.GetGlyphAdvance (glyph) * size + bold_width;
				}
				
				renderer.Color = Color.FromAlphaRgb (1, 0, 0, 0);
				rasterizer.AddSurface (path);
				rasterizer.Render (renderer);
				
				path.Clear ();
			}
			
			form.Show ();
		}
		
		static readonly int cpu_speed = 1700;

		[Test] public void CheckRenderingSpeed()
		{
			Graphics gra  = new Epsitec.Common.Drawing.Graphics ();
			Font     font = Font.GetFont ("Tahoma", "Regular");
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			
			AntiGrain.Interface.NoOp ();
			AntiGrain.Interface.NoOp ();
			AntiGrain.Interface.NoOp ();
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c0 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			AntiGrain.Interface.NoOp ();
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			System.Console.Out.WriteLine ("No-op work: " + c2.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			AntiGrain.Interface.NoOpString (text);
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			System.Console.Out.WriteLine ("No-op work with string: " + c2.ToString ());
			
			gra.SetPixmapSize (1000, 400);
			gra.SolidRenderer.Color = Color.FromBrightness (0);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			gra.AddText (10, 200, text, font, size);
			gra.RenderSolid ();
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
				gra.AddText (10, 200, text, font, size);
				gra.RenderSolid ();
				c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
				
				tot += c2;
			}
			
			long mean = tot / 100;
			
			System.Console.Out.WriteLine ("Mean Rendering : " + mean.ToString () + " -> " + (mean / cpu_speed / text.Length) + "us / char in AGG <=> " + (mean / text.Length) + " cycles / char.");
		}
		
		[Test] public void CheckRenderingSpeedGDIPlus()
		{
			double   size = 10.6;
			string   text = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			
			System.Drawing.Bitmap image = new System.Drawing.Bitmap (1000, 400, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Graphics gra = System.Drawing.Graphics.FromImage (image);
			System.Drawing.Font font = new System.Drawing.Font ("Tahoma", (float) size);
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c0 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			gra.DrawString (text, font, System.Drawing.Brushes.Black, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
				gra.DrawString (text, font, System.Drawing.Brushes.Black, 10, 200);
				c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
				
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
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c0 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			System.IntPtr hFont = font.ToHfont ();
			System.IntPtr hDC   = gra.GetHdc ();
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("Setup: " + c2.ToString ());
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			SelectObject(hDC, hFont);
			TextOut (hDC, 10, 200, text, text.Length);
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("First rendering: " + c2.ToString ());
			
			long tot = 0;
			
			for (int i = 0; i < 100; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
				SelectObject(hDC, hFont);
				TextOut (hDC, 10, 200, text, text.Length);
				c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
				
				tot += c2;
			}
			
			gra.ReleaseHdc (hDC);
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / 100).ToString () + " -> " + (tot * 1000 / 100 / cpu_speed / text.Length) + "ns / char in GDI");
		}
		
		[Test] public void CheckFillPixelCache()
		{
			Graphics gra   = new Epsitec.Common.Drawing.Graphics ();
			int      loops = 1;
			double   size  = 10.6;
			string   text  = "The quick brown fox jumps over the lazy dog. Apportez ce vieux whisky au juge blond qui fume !";
			Font     font  = Font.GetFont ("Tahoma", "Regular");
			
#if false
			foreach (Font.FaceInfo face in Font.Faces)
			{
				if (face.Name.StartsWith ("Bi"))
				{
					font = face.GetFonts ()[0];
					System.Console.WriteLine ("Using font : {0}/{1}/{2}", font.FaceName, font.StyleName, font.OpticalName);
					System.Console.WriteLine ("'A' bounds: {0}", font.GetCharBounds ('A').ToString ());
					System.Console.WriteLine ("'B' bounds: {0}", font.GetCharBounds ('B').ToString ());
					System.Console.WriteLine ("'I' bounds: {0}", font.GetCharBounds ('I').ToString ());
					System.Console.WriteLine ("'i' bounds: {0}", font.GetCharBounds ('i').ToString ());
					System.Console.WriteLine ("'O' bounds: {0}", font.GetCharBounds ('O').ToString ());
					System.Console.WriteLine ("'o' bounds: {0}", font.GetCharBounds ('o').ToString ());
					System.Console.WriteLine ("'p' bounds: {0}", font.GetCharBounds ('p').ToString ());
					System.Console.WriteLine ("'q' bounds: {0}", font.GetCharBounds ('q').ToString ());
					break;
				}
			}
#endif

			Color    black = Color.FromBrightness (0);
			
			long cc = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			long c0 = c2 - c1;
			
			System.Console.Out.WriteLine ("Timer overhead: " + (c0) + " cycles -> " + (c0 / cpu_speed) + "us");
			
			gra.SetPixmapSize (1000, 400);
			gra.SolidRenderer.Color = Color.FromBrightness (0);
			
			font.FillPixelCache ("", size, 10, 200);
			font.PaintPixelCache (gra.Pixmap, "", size, 10, 200, black);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			font.FillPixelCache ("", size, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("Calling FillPixelCache, no work done: " + (c2) + " cycles -> " + (c2 * 1000 / cpu_speed) + "ns");
			
			c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			font.FillPixelCache (text, size, 10, 200);
			c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
			
			System.Console.Out.WriteLine ("Filling the cache: " + (c2) + " cycles -> " + (c2 * 1000 / cpu_speed / text.Length) + "ns / char");
			
			
			
			long tot;
			
			tot = 0;
			
			for (int i = 0; i < loops; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
				font.PaintPixelCache (gra.Pixmap, text, size, 10, 200, black);
				c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
				
				tot += c2;
			}
			
			System.Console.Out.WriteLine ("Mean Rendering : " + (tot / loops).ToString () + " cycles -> " + (tot * 10 / cpu_speed / text.Length) + "ns / char in Cached AGG");
			
			gra.SolidRenderer.Color = black;
			
			tot = 0;
			
			for (int i = 0; i < loops; i++)
			{
				c1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
				gra.PaintText (10, 180, text, font, size);
				c2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta - c0;
				
				tot += c2;
			}
			
			System.Console.Out.WriteLine ("Mean Painting : " + (tot / loops).ToString () + " cycles -> " + (tot * 10 / cpu_speed / text.Length) + "ns / char in Cached AGG");
			
			
			
			font.FillPixelCache (text, size, 10, 240);
			font.PaintPixelCache (gra.Pixmap, text, size, 10, 240, black);
			
			gra.AddText (10, 220, text, font, size);
			gra.RenderSolid ();
			
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (1000, 400);
			form.Text = "CheckFillPixelCache";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint4);
			
			this.global_pixmap_4 = gra.Pixmap;
			
			form.Show ();
		}
		
#if false
		[Test] public void CheckDebugTrapZeroPointer()
		{
			try
			{
				Agg.Library.TrapZeroPointer ();
			}
			catch (System.Exception ex)
			{
				System.Console.Out.WriteLine (ex.ToString ());
			}
		}
#endif
		
		[Test] public void CheckFontFaceBalloon()
		{
			int n = Font.Count;
			
			for (int i = 0; i < n; i++)
			{
				Font font = Font.GetFont (i);
				
				if (font.FaceName.StartsWith ("Bal"))
				{
					System.Console.Out.WriteLine (font.FaceName + " / " + font.StyleName + " / " + font.OpticalName);
					System.Console.Out.WriteLine ("  " + font.FullName + " (" + font.LocalStyleName + ") / e=" + font.GetGlyphIndex ('e').ToString ());
					System.Console.Out.WriteLine ("  A=" + font.Ascender + " D=" + font.Descender + " H=" + (font.LineHeight-font.Ascender+font.Descender) + " w=" + font.GetCharAdvance ('e'));
					
					Font find = Font.GetFont (font.FaceName, font.StyleName, font.OpticalName);
					
					Assert.IsNotNull (find);
					Assert.AreEqual (font.Handle, find.Handle);
				}
			}
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

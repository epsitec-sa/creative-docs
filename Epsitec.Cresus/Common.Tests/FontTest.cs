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
				Assertion.Assert (find.Handle == font.Handle);
			}
			
			Assertion.AssertNull (Font.GetFont (n+1));
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
		
		[Test] public void CheckTextBreak()
		{
			Font   font = Font.GetFont ("Times New Roman", "Regular");
			string text = "The quick brown fox jumps over the lazy dog. Whatever, we just need a piece of long text to break apart.";
			double width = 100;
			TextBreak tb = new TextBreak (font, text, 12.0);
			
			string break_text;
			double break_width;
			
			int line_count = 0;
			string[] chunk = new string[10];
			
			while (tb.GetNextBreak (width, out break_text, out break_width))
			{
				Assertion.Assert (break_width <= width);
				Assertion.Assert (break_text.Length > 0);
				
				chunk[line_count++] = break_text;
			}
			
			Assertion.Assert (line_count == 6);
			Assertion.Assert (break_text == "");
			Assertion.Assert (break_width == 0);
			
			Assertion.Assert (chunk[5] == "apart.");
			
			Assertion.Assert (tb.GetNextBreak (width, out break_text, out break_width) == false);
			
			tb.Dispose ();
			
			Assertion.Assert (tb.GetNextBreak (width, out break_text, out break_width) == false);
		}
		
		[Test] public void CheckGlyphPaint()
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			
			form.ClientSize = new System.Drawing.Size (460, 320);
			form.Text = "CheckGlyphPaint";
			form.Paint +=new System.Windows.Forms.PaintEventHandler(form_Paint);
			
			this.global_pixmap = new Pixmap ();
			this.global_pixmap.Size = form.ClientSize;
			this.global_pixmap.Clear ();
			
			Epsitec.Common.Drawing.Renderer.Solid renderer = new Common.Drawing.Renderer.Solid ();
			renderer.Pixmap = this.global_pixmap;
			renderer.Clear (Color.FromBrightness (1));
			
			Rasterizer rasterizer = new Rasterizer ();
			rasterizer.Gamma = 1.2;
			
			Font font = Font.GetFont ("Arial", "Regular");
			double x = 20;
			
			rasterizer.FillMode = FillMode.NonZero;
			rasterizer.AddGlyph (font, font.GetGlyphIndex ('t'), x, 20, 400);	x += font.GetGlyphAdvance (font.GetGlyphIndex ('t')) * 400;
			rasterizer.AddGlyph (font, font.GetGlyphIndex ('m'), x, 20, 400);
			renderer.Color = Color.FromBrightness (0);
			rasterizer.Render (renderer);
			
			form.Show ();
		}

		private void form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.global_pixmap.Paint (e.Graphics, e.ClipRectangle);
		}
		
		private Pixmap					global_pixmap;
	}
}

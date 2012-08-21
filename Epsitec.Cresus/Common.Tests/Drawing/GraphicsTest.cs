using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets.Helpers;


namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class GraphicsTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckTextOutput()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (300, 640);
			window.Text = "CheckTextOutput";
			window.Root.PaintForeground += Text_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckTextOutputStabilityCheck()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (800, 120);
			window.Text = "CheckTextOutputStabilityCheck";
			window.Root.PaintForeground += TextStabilityCheck_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckTextOutputWingdings()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (300, 640);
			window.Text = "CheckTextOutputWingdings";
			window.Root.PaintForeground += TextWingdings_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckGammaOutput()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (370, 210);
			window.Text = "CheckGammaOutput";
			window.Root.PaintForeground += Gamma_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckSmooth()
		{
			Window window = new Window ();
			
			window.Text = "CheckSmooth";
			window.Root.PaintForeground += Smooth_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckPixmapRawData()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (40*8, 40*8);
			window.Text = "CheckPixmapRawData";
			window.Root.PaintForeground += CheckPixmapRawData_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckCurve()
		{
			Window window = new Window ();
			
			window.Text = "CheckCurve";
			window.Root.PaintForeground += Curve_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckMultiPath()
		{
			Window window = new Window ();
			
			window.Text = "CheckMultiPath";
			window.Root.PaintForeground += MultiPath_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckAlphaMask()
		{
			Window window = new Window ();
			
			window.Text = "CheckAlphaMask";
			window.Root.PaintForeground += AlphaMask_PaintForeground;
			window.Root.Invalidate ();
			window.ClientSize = new Size (300, 250);
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckEvenOddFill()
		{
			Window window = new Window ();
			
			window.Text = "CheckEvenOddFill";
			window.Root.PaintForeground += EvenOddFill_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckNonZeroFill()
		{
			Window window = new Window ();
			
			window.Text = "CheckNonZeroFill";
			window.Root.PaintForeground += NonZeroFill_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckPathAccumulationRasterizer()
		{
			Window window = new Window ();
			
			window.Text = "PathAccumulationRasterizer";
			window.Root.PaintForeground += PathAccumulationRasterizer_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckPathAppend()
		{
			Window window = new Window ();
			window.WindowSize = new Size (800, 1500);
			
			window.Text = "CheckPathAppend";
			window.Root.PaintForeground += PathAppend_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckFontPreviewer()
		{
			FontPreviewer.Initialize ();
			
			Window window = new Window ();
			window.WindowSize = new Size (800, 1500);
			
			window.Text = "CheckFontPreviewer";
			window.Root.PaintForeground += FontPreviewer_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckClipping()
		{
			Window window = new Window ();
			
			window.Text = "CheckClipping";
			window.Root.PaintForeground += Clipping_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckTransform()
		{
			Window window = new Window ();
			
			window.Text = "CheckTransform";
			window.Root.PaintForeground += Transform_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckLineCaps()
		{
			Window window = new Window ();
			
			window.Text = "CheckLineCaps";
			window.Root.PaintForeground += LineCaps_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckGradient1()
		{
			Window window = new Window ();
			
			window.Text = "CheckGradient1";
			window.Root.PaintForeground += Gradient1_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckGradient2()
		{
			Window window = new Window ();
			
			window.Text = "CheckGradient2";
			window.Root.PaintForeground += Gradient2_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckGradient3()
		{
			Window window = new Window ();
			
			window.Text = "CheckGradient3";
			window.Root.PaintForeground += Gradient3_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckImage()
		{
			Window window = new Window ();
			
			window.Text = "CheckImage";
			window.Root.PaintForeground += Image_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckImageRect()
		{
			Window window = new Window ();
			
			window.Text = "CheckImageRect";
			window.Root.PaintForeground += ImageRect_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckImageAlpha()
		{
			Window window = new Window ();

			window.ClientSize = new Size (300, 300);
			window.Text = "CheckImageAlpha";
			window.Root.PaintForeground += ImageAlpha_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckImageRectTIFF()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (200, 200);
			window.Text = "CheckImageRectTIFF";
			window.Root.PaintForeground += ImageRectTIFF_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckImage100dpi200dpi()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (400, 200);
			window.Text = "CheckImage100dpi200dpi";
			window.Root.PaintForeground += Image100dpi200dpi_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckSpecial4Fill()
		{
			Window window = new Window ();
			
			window.Text = "CheckSpecial4Fill";
			window.Root.PaintForeground += Special4Fill_PaintForeground;
			window.Root.Invalidate ();
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		
		private void Text_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double x = 10;
			double y = root.Client.Size.Height - 10;
			double width  = root.Client.Size.Width  - 20;
			double height = 25;
			
			string text = "This is a test string using ";
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 10.6;
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.LineWidth = 0.2;
			ContentAlignment[] modes = new ContentAlignment[]
				{
					ContentAlignment.TopLeft,    ContentAlignment.TopCenter,    ContentAlignment.TopRight,
					ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter, ContentAlignment.MiddleRight,
					ContentAlignment.BottomLeft, ContentAlignment.BottomCenter, ContentAlignment.BottomRight
				};
			
			foreach (ContentAlignment mode in modes)
			{
				y -= height + 5;
				e.Graphics.AddRectangle (x, y, width, height);
				e.Graphics.AddText (x, y, width, height, text + mode.ToString (), font, size, mode);
			}
			
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			font = Font.GetFont ("Times New Roman", "Italic");
			
			e.Graphics.SolidRenderer.Color = Color.FromRgb (0, 0, 0);
			e.Graphics.PaintText (10, 60, "Hello, world. 14pt", font, 14.0);
			e.Graphics.PaintText (10, 50, "Hello, world. 13pt", font, 13.0);
			e.Graphics.PaintText (10, 40, "Hello, world. 12pt", font, 12.0);
			e.Graphics.PaintText (10, 30, "Hello, world. 11pt", font, 11.0);
			e.Graphics.PaintText (10, 20, "Hello, world. 10pt", font, 10.0);
			e.Graphics.PaintText (10, 10, "Hello, world. 9pt",  font,  9.0);
			
			text = "Quelle idée\u00A0! Un fjord finlandais...";
			y    = 80;
			
			double font_size  = 14;
			double line_width = font.GetTextAdvance (text) * font_size - 10;
			
			while (line_width < width)
			{
				FontClassInfo[] text_infos;
				double           text_width;
				double           text_elasticity;
				
				font.GetTextClassInfos (text, out text_infos, out text_width, out text_elasticity);
				
				text_width      *= font_size;
				text_elasticity *= font_size;
				
				double delta = line_width - text_width;
				
				if ((text_elasticity > 0) && (delta != 0))
				{
					for (int i = 0; i < text_infos.Length; i++)
					{
						double width_ratio = text_infos[i].Width / delta;
						double elast_ratio = text_infos[i].Elasticity / text_elasticity;
						text_infos[i].Scale = 1.00 + elast_ratio / width_ratio;
					}
				}
				
				e.Graphics.SolidRenderer.Color = Color.FromRgb (0, 0, 0);
				e.Graphics.PaintText (10, y, text, font, font_size, text_infos);
				e.Graphics.LineWidth = 0.8;
				e.Graphics.AddLine (10, y, 10, y+10);
				e.Graphics.AddLine (10 + text_width, y, 10 + text_width, y+10);
				e.Graphics.AddLine (10 + line_width, y, 10 + line_width, y+10);
				e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
				
				y += font_size;
				line_width += 5;
			}
		}
		
		private void TextStabilityCheck_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			string text = "abcdefghijklmnopqrstuvwxyz 0123456789 +@#*%&/()= ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 10.6;
			Graphics g = e.Graphics;
			
			for (double x = 0.0; x < 1.0; x += 0.08)
			{
				for (double y = 0.0; y < 2.0; y += 0.39)
				{
					g.PaintText (x+20, y+20, text, font, size);
					g.RenderSolid (Color.FromBrightness (0));
				}
			}
			
			size = 10.2;
			
			for (double x = 0.0; x < 1.0; x += 0.08)
			{
				for (double y = 0.0; y < 2.0; y += 0.39)
				{
					g.PaintText (x+20, y+35, text, font, size);
					g.RenderSolid (Color.FromBrightness (0));
				}
			}
			
			size = 10.8;
			
			for (double x = 0.0; x < 1.0; x += 0.08)
			{
				for (double y = 0.0; y < 2.0; y += 0.39)
				{
					g.PaintText (x+20, y+50, text, font, size);
					g.RenderSolid (Color.FromBrightness (0));
				}
			}
			
			font = Font.GetFont ("Arial", "Regular");
			size = 10.8;
			
			for (double x = 0.0; x < 1.0; x += 0.08)
			{
				for (double y = 0.0; y < 2.0; y += 0.39)
				{
					g.PaintText (x+20, y+65, text, font, size);
					g.RenderSolid (Color.FromBrightness (0));
				}
			}
		}
		
		private void TextWingdings_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double x = 10;
			double y = root.Client.Size.Height - 10;
			
			Font   font = Font.GetFont ("Wingdings", "Regular");
			double size = 48;
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.LineWidth = 0.2;
			
			y -= size;
			
			for (int i = 4; i < 64; i += 4)
			{
				Path path = new Path ();
				Path copy = new Path ();
				
				path.Append (font, i, x, y, size);
				
				PathElement[] elements;
				Point[]       points;
			
				path.GetElements (out elements, out points);
				copy.SetElements (elements, points);
				
				e.Graphics.Rasterizer.AddSurface (copy);
//				e.Graphics.Rasterizer.AddGlyph (font, i+0, x + 0, y, size);
				e.Graphics.Rasterizer.AddGlyph (font, i+1, x + 64, y, size);
				e.Graphics.Rasterizer.AddGlyph (font, i+2, x + 128, y, size);
				e.Graphics.Rasterizer.AddGlyph (font, i+3, x + 192, y, size);
				
				y -= size;
				
				path.Dispose ();
				copy.Dispose ();
			}
			
			e.Graphics.RenderSolid (Color.FromBrightness (0));
		}
		
		private void Gamma_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 10.6;
			
			double y = root.Client.Size.Height - 5;
			
			for (int i = 0; i < 5; i++)
			{
				switch (i)
				{
					case 0: e.Graphics.Rasterizer.Gamma = 0.80;	break;
					case 1: e.Graphics.Rasterizer.Gamma = 0.90;	break;
					case 2: e.Graphics.Rasterizer.Gamma = 1.00;	break;
					case 3: e.Graphics.Rasterizer.Gamma = 1.10;	break;
					case 4: e.Graphics.Rasterizer.Gamma = 1.20;	break;
				}
				
				y -= 40;
				
				e.Graphics.AddFilledRectangle (5, y, 360, 36);
				e.Graphics.RenderSolid (Color.FromBrightness (1));
				
				e.Graphics.AddText (10, y+5+13, "A theme is a background plus a set of sounds, icons, and other elements", font, size);
				e.Graphics.AddText (10, y+5,    "to help you personalize your computer with one click. gamma=" + e.Graphics.Rasterizer.Gamma.ToString (), font, size);
				e.Graphics.RenderSolid (Color.FromBrightness (0));
			}
		}
		
		private void EvenOddFill_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			//	Jouons un peu avec les arcs d'ellipse... D'abord les non jointifs.
			
			path1.MoveTo (50, 200);
			path1.ArcDeg (50, 200, 30, 20, 30, 135, true);
			
			Point pend = path1.CurrentPoint;
			path1.ArcDeg (pend, 3, 3, 0, 360, true);
			path1.Close ();
			
			path1.MoveTo (50, 190);
			path1.ArcDeg (50, 190, 30, 20, 30, 135, false);
			
			//	...et maintenant les arcs jointifs.
			
			path1.MoveTo (200, 200);
			path1.ArcToDeg (200, 200, 50, 50, 30, 60, true);
			path1.Close ();
			
			path1.MoveTo (200, 200);
			path1.ArcToDeg (200, 200, 40, 40, 145, 20, true);
			path1.Close ();
			
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			e.Graphics.FillMode = FillMode.EvenOdd;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
		}
		
		private void CheckPixmapRawData_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			Graphics gfx = new Graphics ();
			gfx.SetPixmapSize (40, 40)	;
			gfx.SolidRenderer.Color = Color.FromRgb (1, 0.2, 0);
			gfx.PaintText (2, 8, "@", Font.DefaultFont, 40);
			
			using (Pixmap.RawData src = new Pixmap.RawData (gfx.Pixmap))
			{
				for (int y = 0; y < src.Height; y++)
				{
					for (int x = 0; x < src.Width; x++)
					{
						Color pixel = src[x, y];
						e.Graphics.AddFilledRectangle (x * 8, y * 8, 7, 7);
						e.Graphics.RenderSolid (pixel);
					}
				}
			}
		}
		
		private void Smooth_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			e.Graphics.ScaleTransform (10, 10, 0, 0);
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			
			path1.MoveTo (1, 1);
			path1.LineTo (1, 11);
			path1.LineTo (11, 6);
			path1.Close ();
			
			path2.MoveTo (5, 5);
			path2.LineTo (20, 5);
			path2.LineTo (20, 15);
			path2.CurveTo (20, 20, 5, 20, 5, 15);
			
			DashedPath path3 = new DashedPath ();
			DashedPath path4 = new DashedPath ();
			
			path3.DefaultZoom = 10;
			path4.DefaultZoom = 10;
			
			path3.MoveTo (1, 1);
			path3.LineTo (1, 11);
			path3.LineTo (11, 6);
			path3.Close ();
			path3.AddDash (1, 0.2);
			path3.AddDash (0.4, 0.2);
			path3.DashOffset = 0.5;
			
			path4.Append (path2);
			path4.AddDash (0.5, 0.5);
			
			e.Graphics.SmoothRenderer.Color = Color.FromRgb (0, 0, 1);
			e.Graphics.SmoothRenderer.SetParameters (1, 1);
			e.Graphics.SmoothRenderer.AddPath (path1);
			e.Graphics.SmoothRenderer.AddPath (path2);
			
			using (Path dashed = path3.GenerateDashedPath ())
			{
				e.Graphics.Rasterizer.AddOutline (dashed, 0.2);
			}
			
			using (Path dashed = path4.GenerateDashedPath ())
			{
				e.Graphics.Rasterizer.AddOutline (dashed, 0.2);
			}
			
			e.Graphics.RenderSolid (Color.FromRgb (0, 1, 0));
			
			
			e.Graphics.RotateTransformDeg (15, cx/10, cy/10);
			e.Graphics.TranslateTransform (5, 2);
			
			e.Graphics.SmoothRenderer.Color = Color.FromAlphaRgb (0.5, 1, 0, 0);
			e.Graphics.SmoothRenderer.SetParameters (1, 1);
			e.Graphics.SmoothRenderer.AddPath (path1);
			e.Graphics.SmoothRenderer.AddPath (path2);
			
		}
		
		private void AlphaMask_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			e.Graphics.ScaleTransform (root.Client.Size.Width / 300.0, root.Client.Size.Height / 250.0, 0, 0);
			
			Graphics mask = e.Graphics.CreateAlphaMask ();
			
			for (int i = 0; i <= 200; i++)
			{
				Path path = new Path ();
				
				path.MoveTo ( 10, 10+i);
				path.LineTo (250, 10+i);
				path.LineTo (250, 10+i+2);
				path.LineTo ( 10, 10+i+2);
				path.Close ();
				
				mask.Rasterizer.AddSurface (path);
				mask.RenderSolid (Color.FromRgb (i/200.0, 0, 0));
			}
			
			e.Graphics.AddText (10, 10, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.AddText (10, 50, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.AddText (10, 90, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.AddText (10,130, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.AddText (10,170, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.AddText (10,210, "AAAAAAAAAAAA", Font.GetFont ("Tahoma", "Regular"), 40);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0.5, 0));
			e.Graphics.SolidRenderer.SetAlphaMask (mask.Pixmap, MaskComponent.R);
			e.Graphics.AddText (30, 30, "A!!", Font.GetFont ("Tahoma", "Regular"), 250);
			e.Graphics.RenderSolid (Color.FromAlphaRgb (1.0, 0, 0, 1));
			e.Graphics.SolidRenderer.SetAlphaMask (null, MaskComponent.None);
			e.Graphics.AddText (-30, 50, "|", Font.GetFont ("Tahoma", "Regular"), 250);
			e.Graphics.RenderSolid (Color.FromAlphaRgb (1.0, 0, 1, 0));
			
			mask.Dispose ();
		}
		
		private void Curve_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			e.Graphics.ScaleTransform (10, 10, 0, 0);
			
			Path path2 = new Path ();
			Path path3 = new Path ();
			
			path2.MoveTo (5, 5);
			path2.LineTo (20, 5);
			path2.LineTo (20, 15);
			path2.CurveTo (20, 20, 5, 20, 5, 15);
			
			e.Graphics.RotateTransformDeg (15, cx/10, cy/10);
			
			e.Graphics.Rasterizer.AddOutline (path2, 1.8, CapStyle.Round, JoinStyle.Miter, 5.0);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
			
			e.Graphics.TranslateTransform (20, 2);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.RenderSolid (Color.FromRgb (1, 1, 0));
			
			path3.Append (path2, 10, -2);
			e.Graphics.Rasterizer.AddSurface (path3);
			e.Graphics.RenderSolid (Color.FromAlphaRgb (0.5, 0, 1, 0));
		}
		
		private void MultiPath_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			
			path2.MoveTo (10, 10);
			path2.LineTo (110, 10);
			path2.LineTo (110, 40);
			path2.Close ();
			
			path1.Append (path2);
			
			path2.Clear ();
			path2.MoveTo (10,  50);
			path2.LineTo (110, 50);
			path2.LineTo (110, 90);
			path2.Close ();
			
			path1.Append (path2);
			
			e.Graphics.Rasterizer.AddOutline (path1, 2, CapStyle.Round, JoinStyle.Miter, 5.0);
			e.Graphics.RenderSolid (Color.FromRgb (0, 0, 0));
		}
		
		private void NonZeroFill_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			Transform t = e.Graphics.Transform;
			
			e.Graphics.RotateTransformDeg (15, cx, cy);
			e.Graphics.ScaleTransform (1.2, 1.2, 0, 0);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
			
			e.Graphics.Transform = t;
		}
		
		private void PathAccumulationRasterizer_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			PathAccumulationRasterizer r = new PathAccumulationRasterizer ();
			Graphics g = new Graphics ();
			g.ReplaceRasterizer (r);
			
			g.RotateTransformDeg (15, cx, cy);
			g.ScaleTransform (1.2, 1.2, 0, 0);
			
			g.AddLine (cx, cy-5, cx, cy+5);
			g.AddLine (cx-5, cy, cx+5, cy);
			g.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			g.FillMode = FillMode.NonZero;
			g.Rasterizer.AddSurface (path1);
			g.Rasterizer.AddSurface (path2);
			g.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			g.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			g.RenderSolid (Color.FromRgb (1, 0, 0));
			
			foreach (Path path in r.GetPaths ())
			{
				e.Graphics.Rasterizer.AddSurface (path);
				System.Console.WriteLine (path.ToString ());
			}
			
			e.Graphics.RenderSolid (Color.FromRgb (0, 0, 1));
		}
		
		private void PathAppend_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			Path path = new Path ();
			path.MoveTo ( 990 - 600,  927 - 800);
			path.LineTo ( 760 - 600,  923 - 800);
			path.LineTo (1027 - 600, 1512 - 800);
			path.Close ();
			
			e.Graphics.Color = Color.FromName ("Yellow");
			e.Graphics.PaintSurface (path);
			
			e.Graphics.Color = Color.FromName ("Black");
			e.Graphics.LineWidth = 100;
			e.Graphics.LineMiterLimit = 5;
			e.Graphics.LineJoin = JoinStyle.MiterRevert;
			e.Graphics.LineCap = CapStyle.Round;
			
			e.Graphics.PaintOutline (path);
		}
		
		private void FontPreviewer_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			OpenType.FontCollection collection = OpenType.FontCollection.Default;
			collection.RefreshCache ();
			
			double ox = 10;
			double oy = 20;
			double size = 100;
			
			string[] fonts = new string[] { "Avant Garde Book BT", "Tahoma", "Palatino Linotype Bold", "Wingdings", "Webdings", "Symbol" };
			
			foreach (string font in fonts)
			{
				OpenType.FontIdentity fid = collection[font];
				
				if (fid != null)
				{
					Path path = FontPreviewer.GetPath (fid, ox, oy, size);
					
					e.Graphics.Color = Color.FromName ("Black");
					e.Graphics.PaintSurface (path);
					
					path.Dispose ();
					
					path = FontPreviewer.GetPathAbc (fid, ox, oy + size*fonts.Length, size);
					
					e.Graphics.Color = Color.FromName ("Black");
					e.Graphics.PaintSurface (path);
					
					oy += size;
					
					path.Dispose ();
				}
			}
		}
		
		private void Clipping_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			Transform t = e.Graphics.Transform;
			
			e.Graphics.RotateTransformDeg (15, cx, cy);
			e.Graphics.ScaleTransform (1.2, 1.2, 0, 0);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.SetClippingRectangle (new Rectangle (50, 50, root.Client.Size.Width - 100, 50));
			
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
			e.Graphics.ResetClippingRectangle ();
			e.Graphics.Transform = t;
			
			Rectangle r1 = new Rectangle (10, root.Client.Size.Height - 20, 100, 10);
			Rectangle r2 = new Rectangle (10, root.Client.Size.Height - 32, 100,  5);
			Rectangle r3 = new Rectangle (10, root.Client.Size.Height - 41, 100,  5);
			
			e.Graphics.AddFilledRectangle (r1);
			e.Graphics.AddFilledRectangle (r2);
			e.Graphics.AddFilledRectangle (r3);
			e.Graphics.RenderSolid (Color.FromRgb (0, 1, 0));
			
			e.Graphics.SetClippingRectangles (new Rectangle[] { r2, r1, r3 });
			e.Graphics.AddText (0, root.Client.Size.Height - 50, "AAAAAAAAAAAAAAAAAAA", font, 70);
			e.Graphics.RenderSolid (Color.FromRgb (0, 0, 1));
		}
		
		private void Transform_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.RotateTransformDeg (15, cx, cy);
			e.Graphics.ScaleTransform (1.2, 1.2, 0, 0);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRgb (1, 0, 0));
			
			e.Graphics.AddText (0, cy + 10, root.Client.Size.Width, 20, "The quick brown fox jumps over the lazy dog !", font, 12, ContentAlignment.BottomCenter);
			e.Graphics.RenderSolid (Color.FromRgb (0, 0, 0.4));
		}
		
		private void LineCaps_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double dx = root.Client.Size.Width;
			double dy = root.Client.Size.Height;
			
			double y;
			
			y = 25;
			
			e.Graphics.LineCap = CapStyle.Round;
			e.Graphics.LineWidth = 40;
			
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			e.Graphics.LineCap = CapStyle.Round;
			e.Graphics.LineWidth = 2;
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (1));
			
			y = 75;
			
			e.Graphics.LineCap = CapStyle.Square;
			e.Graphics.LineWidth = 40;
			
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			e.Graphics.LineCap = CapStyle.Round;
			e.Graphics.LineWidth = 2;
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (1));
			
			y = 125;
			
			e.Graphics.LineCap = CapStyle.Butt;
			e.Graphics.LineWidth = 40;
			
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			e.Graphics.LineCap = CapStyle.Round;
			e.Graphics.LineWidth = 2;
			e.Graphics.AddLine (25, y, dx - 25, y);
			e.Graphics.RenderSolid (Color.FromBrightness (1));
		}
		
		private void Gradient1_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.RotateTransformDeg (0, cx, cy);
			e.Graphics.ScaleTransform (0.8, 0.8, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Path path1 = new Path ();
			Path path2 = new Path ();
			Font font  = Font.GetFont ("Times New Roman", "Regular");
			
			path1.MoveTo (10, 10);
			path1.LineTo (10, 110);
			path1.LineTo (110, 60);
			path1.Close ();
			path1.MoveTo (20, 25);
			path1.LineTo (95, 60);
			path1.LineTo (20, 95);
			path1.Close ();
			
			path2.MoveTo (210, 10);
			path2.LineTo (210, 110);
			path2.LineTo (310, 60);
			path2.Close ();
			path2.MoveTo (220, 25);
			path2.LineTo (220, 95);
			path2.LineTo (295, 60);
			path2.Close ();
			
			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];
			
			for (int i = 0; i < 256; i++)
			{
				double sin1 = System.Math.Sin (i*System.Math.PI/102.4);
				double sin2 = System.Math.Sin (i*System.Math.PI/102.4 + 0.2);
				r[i] = 0.5 + sin1*sin1 * 0.3;
				g[i] = 0.5 + sin2*sin2 * 0.3;
				b[i] = 1.0;
				Color.ConvertHsvToRgb (i/256.0*360.0, 1.0, 1.0, out r[i], out g[i], out b[i]);
				a[i] = 1.0;
			}
			
			//	Pour voir les extrémités :
			
			r[0]   = g[0]   = b[0]   = 1.0;
			r[255] = g[255] = b[255] = 1.0;
			
			e.Graphics.FillMode = FillMode.NonZero;
//			e.Graphics.AddFilledRectangle (0, cy+50, root.Client.Size.Width, 10);
//			e.Graphics.Rasterizer.AddSurface (path1);
//			e.Graphics.Rasterizer.AddSurface (path2);
//			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
//			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.AddFilledRectangle (0, 0, 2*cx, 2*cy);
			e.Graphics.GradientRenderer.Fill = Epsitec.Common.Drawing.GradientFill.X;
//			e.Graphics.GradientRenderer.SetParameters (0, root.Client.Size.Width);
			e.Graphics.GradientRenderer.SetParameters (0, 100);
			e.Graphics.GradientRenderer.SetColors (r, g, b, a);

			Transform t = Transform.Identity;
			t = t.Translate (-50, 0);
			t = t.RotateDeg (45, 0, 0);
			t = t.Translate (cx, cy);
			e.Graphics.GradientRenderer.Transform = t;
			
			e.Graphics.RenderGradient ();
			
			e.Graphics.AddRectangle (cx - 50, cy - 30, 100, 60);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
		}
		
		private void Gradient2_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			//e.Graphics.RotateTransform (0, cx, cy);
			//e.Graphics.ScaleTransform (1, 1, cx, cy);

			Path path1 = new Path ();
			path1.MoveTo (0, 0);
			path1.LineTo (0, root.Client.Size.Height);
			path1.LineTo (root.Client.Size.Width, root.Client.Size.Height);
			path1.LineTo (root.Client.Size.Width, 0);
			path1.Close ();
			
			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];
			
			for (int i = 0 ; i < 256 ; i++)
			{
				Color.ConvertHsvToRgb (i/256.0*360.0, 1.0, 1.0, out r[i], out g[i], out b[i]);
				a[i] = 1.0;
			}
			
			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.GradientRenderer.Fill = Epsitec.Common.Drawing.GradientFill.Conic;
			e.Graphics.GradientRenderer.SetParameters (0, 200);
			e.Graphics.GradientRenderer.SetColors (r, g, b, a);
			//e.Graphics.GradientRenderer.SetColors (Color.FromRgb(1,0,0), Color.FromRgb(0,0,1));

			Transform t = Transform.Identity;
			t = t.Translate (cx, cy);
			//t.Rotate (30, cx, cy);
			e.Graphics.GradientRenderer.Transform = t;
			
			e.Graphics.RenderGradient ();

			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
		}

		private void Gradient3_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;

			Rectangle rect = new Rectangle(50, 100, 150, 50);

			Path path = new Path();
			path.MoveTo(rect.Left, rect.Bottom);
			path.LineTo(rect.Left, rect.Top);
			path.LineTo(rect.Right, rect.Top);
			path.LineTo(rect.Right, rect.Bottom);
			
			e.Graphics.Rasterizer.AddOutline(path, 1);
			//e.Graphics.RenderSolid(Color.FromBrightness(0));

			e.Graphics.FillMode = FillMode.NonZero;
			e.Graphics.GradientRenderer.SetColors(Color.FromRgb(1,0,0), Color.FromRgb(0,0,1));

			Transform ot = e.Graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;

			e.Graphics.GradientRenderer.Fill = GradientFill.Y;
			Point center = rect.Center;
			e.Graphics.GradientRenderer.SetParameters(-100, 100);
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			t = t.RotateDeg (0, center);

			e.Graphics.GradientRenderer.Transform = t;
			e.Graphics.RenderGradient();
			e.Graphics.GradientRenderer.Transform = ot;
		}
		
		private void Image_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.RotateTransformDeg (-30, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Font   font = Font.GetFont ("Arial Black", "Regular");
			double size = 50;
			
			e.Graphics.FillMode = FillMode.NonZero;
			
			Image bitmapImage = ImageProvider.Instance.GetImage (@"file:Images/test.png", Resources.DefaultManager);

			Assert.IsNotNull (bitmapImage);

			Bitmap bitmap = bitmapImage.BitmapImage;

			Assert.IsNotNull (bitmap);

			string text = "TOMATE";
			
			double max_width = 0;
			
			foreach (char c in text)
			{
				max_width = System.Math.Max (max_width, font.GetCharAdvance (c) * size);
			}
			
			double x = cx - max_width * text.Length / 2;
			double y = cy - size * 1.3;
			
			double width  = max_width;
			double height = font.LineHeight * size;
			
			foreach (char c in text)
			{
				e.Graphics.AddText (x, y, width, height, new string (c, 1), font, size,  ContentAlignment.MiddleCenter);
				e.Graphics.ImageRenderer.BitmapImage = bitmap;

				Transform t = Transform.Identity;
				t = t.Scale (width / bitmap.Width, height / bitmap.Height);
				t = t.Translate (x, y);
				
				e.Graphics.ImageRenderer.Transform = t;
				e.Graphics.RenderImage ();
				
				x += width;
			}
			
			e.Graphics.Color = Color.FromRgb (1.0, 1.0, 1.0);
			e.Graphics.PaintSurface (Path.FromRectangle (cx-120, cy+20, 120, 40));
			e.Graphics.Color = Color.FromRgb (0.5, 0.5, 0.5);
			e.Graphics.PaintSurface (Path.FromRectangle (cx+0, cy+20, 120, 40));
			
			Graphics draft_graphics = new Graphics ();
			
			draft_graphics.SetPixmapSize (40, 40);
			
			Pixmap draft_pixmap = draft_graphics.Pixmap;
			Image  draft_bitmap = Bitmap.FromPixmap (draft_pixmap);
			
			draft_graphics.SolidRenderer.ClearAlphaRgb (0.0, 1.0, 1.0, 1.0);
			draft_graphics.Color     = Color.FromRgb (0, 0, 0.6);
			draft_graphics.LineWidth = 3.0;
			draft_graphics.PaintOutline (Path.FromCircle (20, 20, 15, 10));
			
			e.Graphics.PaintImage (draft_bitmap, cx-120, cy+20, 40, 40, 0, 0, 40, 40);
			e.Graphics.PaintImage (draft_bitmap, cx+0,  cy+20, 40, 40, 0, 0, 40, 40);
			
			draft_graphics.Color     = Color.FromRgb (1.0, 1.0, 1.0);
			draft_graphics.PaintSurface (Path.FromCircle (20, 20, 5, 5));
			
			e.Graphics.PaintImage (draft_bitmap, cx-80, cy+20, 40, 40, 0, 0, 40, 40);
			e.Graphics.PaintImage (draft_bitmap, cx+40, cy+20, 40, 40, 0, 0, 40, 40);
			
			draft_graphics.Color     = Color.FromAlphaRgb (0.5, 1.0, 0, 0);
			draft_graphics.PaintSurface (Path.FromCircle (20, 20, 10, 15));
			
			e.Graphics.PaintImage (draft_bitmap, cx-40, cy+20, 40, 40, 0, 0, 40, 40);
			e.Graphics.PaintImage (draft_bitmap, cx+80, cy+20, 40, 40, 0, 0, 40, 40);
		}
		
		private void ImageRect_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.RotateTransformDeg (0, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));

			Bitmap bitmap = ImageProvider.Instance.GetImage (@"file:Images/test.png", Resources.DefaultManager).BitmapImage;
			
			//	L'image fait 115 x 102 pixels
			
			e.Graphics.PaintImage (bitmap,  10,  10, 64, 48);					//	stretch, toute l'image
			e.Graphics.PaintImage (bitmap,  75,  10, 64, 48, 60, 40);			//	clip, origine dans image [60;40]
			e.Graphics.PaintImage (bitmap, 150,  10, 64, 48, 60, 40, 32, 48);	//	stretch, origine dans image [60;40], taille source [32;48]
			
			e.Graphics.PaintImage (bitmap,  10,  60, 200, 20);					//	stretch
			e.Graphics.PaintImage (bitmap,  10,  82, 200, 20, 0, 0);			//	clip
			e.Graphics.PaintImage (bitmap,  10, 104, 200, 20, 0, 0, 125, 20);	//	clip + stretch
			e.Graphics.PaintImage (bitmap,  10, 126, 200, 20, 0, 10, 200, 80);	//	clip + stretch
			e.Graphics.PaintImage (bitmap,  10, 148, 200, 20, 100, 10, 200, 80);//	clip + stretch (déborde de l'image, 185 pixels en dehors à droite)
			e.Graphics.PaintImage (bitmap,  10, 148, 200, 20, 120, 10, 200, 80);//	clip + stretch (plus rien)
		}
		
		private void ImageAlpha_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = 100;
			double cy = 100;

			e.Graphics.ScaleTransform (1.5, 1.5, 0, 0);
			e.Graphics.RotateTransformDeg (0, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));

			for (int y = 0; y < 200; y += 8)
			{
				e.Graphics.AddFilledRectangle (0, y, 200, 4);
				e.Graphics.RenderSolid (Color.FromBrightness (1));
				e.Graphics.AddFilledRectangle (0, y+4, 200, 4);
				e.Graphics.RenderSolid (Color.FromBrightness (0.5));
			}

			Bitmap bitmap = ImageProvider.Instance.GetImage (@"file:Images/4x4-alpha.png", Resources.DefaultManager).BitmapImage;
			
			//	L'image fait 2 x 2 pixels

			e.Graphics.ImageFilter = new ImageFilter (ImageFilteringMode.None);
			e.Graphics.PaintImage (bitmap,  10,  10, 64, 64);

			e.Graphics.ImageFilter = new ImageFilter (ImageFilteringMode.Bilinear);
			e.Graphics.PaintImage (bitmap, 110,  10, 64, 64);

			e.Graphics.ImageFilter = new ImageFilter (ImageFilteringMode.Bicubic);
			e.Graphics.PaintImage (bitmap,  10, 110, 64, 64);
			
			e.Graphics.ImageFilter = new ImageFilter (ImageFilteringMode.ResamplingBilinear);
			e.Graphics.PaintImage (bitmap, 110, 110, 64, 64);
		}
		
		private void ImageRectTIFF_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Size.Width / 2;
			double cy = root.Client.Size.Height / 2;
			
			e.Graphics.ScaleTransform (2, 2, 0, 0);
			e.Graphics.RotateTransformDeg (0, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Bitmap bitmap = ImageProvider.Instance.GetImage (@"file:Images/image1.tif", Resources.DefaultManager).BitmapImage;
			
			//	L'image fait 96 x 96 pixels
			
			e.Graphics.PaintImage (bitmap, new Rectangle ( 10,  10,  40,  40), new Rectangle (32, 32, 32, 32));
			e.Graphics.PaintImage (bitmap, new Rectangle ( 60,  10,  64,  64), new Rectangle (32, 32, 32, 32));
			e.Graphics.PaintImage (bitmap, new Rectangle ( 10,  60, 100, 100), new Rectangle (32, 32, 32, 32));
			
			e.Graphics.ScaleTransform (0.5, 0.5, 0, 0);
			
			e.Graphics.PaintImage (bitmap, new Rectangle (  1,   1,  2, 2), new Rectangle (0, 1, 2, 1));
			e.Graphics.PaintImage (bitmap, new Rectangle (  1,   5,  4, 4), new Rectangle (0, 1, 2, 1));
			e.Graphics.PaintImage (bitmap, new Rectangle (  1,  10,  8, 8), new Rectangle (0, 1, 2, 1));
		}
		
		private void Image100dpi200dpi_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			//	L'image fait 200 x 200 pixels
			
			Bitmap bitmap_1 = Bitmap.FromFile ("..\\..\\Images\\100-dpi.tif").BitmapImage;
			Bitmap bitmap_2 = Bitmap.FromFile ("..\\..\\Images\\200-dpi.tif").BitmapImage;
			
			Font font = Font.GetFont ("Tahoma", "Regular");
			
			e.Graphics.PaintImage (bitmap_1, new Rectangle (0, 0, 200, 200), new Rectangle (0, 0, 200, 200));
			e.Graphics.PaintImage (bitmap_2, new Rectangle (200, 0, 200, 200), new Rectangle (0, 0, 200, 200));
			
			e.Graphics.SolidRenderer.Color = Color.FromBrightness (0);
			e.Graphics.PaintText ( 10, 10, string.Format ("{0} dpi", bitmap_1.DpiX), font, 12);
			e.Graphics.PaintText (210, 10, string.Format ("{0} dpi", bitmap_2.DpiX), font, 12);
		}
		
		private void Special4Fill_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			int dx = (int) root.Client.Size.Width;
			int dy = (int) root.Client.Size.Height;
			
			e.Graphics.SolidRenderer.Clear4Colors (10, 10, dx - 20, dy - 20,
				Color.FromRgb (0.75, 0, 0),
				Color.FromRgb (0.75, 0, 1),
				Color.FromRgb (0.75, 1, 1),
				Color.FromRgb (0.75, 1, 0));
		}
		
	}
}

using System;
using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class GraphicsTest
	{
		[Test] public void CheckTextOutput()
		{
			WindowFrame window = new WindowFrame ();
			
			window.ClientSize = new System.Drawing.Size (300, 290);
			window.Text = "CheckTextOutput";
			window.Root.PaintForeground += new PaintEventHandler(Text_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckGammaOutput()
		{
			WindowFrame window = new WindowFrame ();
			
			window.ClientSize = new System.Drawing.Size (370, 210);
			window.Text = "CheckGammaOutput";
			window.Root.PaintForeground += new PaintEventHandler(Gamma_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}
		
		[Test] public void CheckEvenOddFill()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckEvenOddFill";
			window.Root.PaintForeground += new PaintEventHandler(EvenOddFill_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}
		
		[Test] public void CheckNonZeroFill()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckNonZeroFill";
			window.Root.PaintForeground += new PaintEventHandler(NonZeroFill_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckClipping()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckClipping";
			window.Root.PaintForeground += new PaintEventHandler(Clipping_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckTransform()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckTransform";
			window.Root.PaintForeground += new PaintEventHandler(Transform_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckGradient()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckGradient";
			window.Root.PaintForeground += new PaintEventHandler(Gradient_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckImage()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckImage";
			window.Root.PaintForeground += new PaintEventHandler(Image_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckImageRect()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckImageRect";
			window.Root.PaintForeground += new PaintEventHandler(ImageRect_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		[Test] public void CheckImageRectTIFF()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Text = "CheckImageRect";
			window.Root.PaintForeground += new PaintEventHandler(ImageRectTIFF_PaintForeground);
			window.Root.Invalidate ();
			window.Show ();
		}

		private void Text_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double x = 10;
			double y = root.Client.Height - 10;
			double width  = root.Client.Width  - 20;
			double height = 25;
			
			string text = "This is a test string using ";
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 10.6;
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
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
		}
		
		private void Gamma_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 10.6;
			
			double y = root.Client.Height - 5;
			
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
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
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
			
			e.Graphics.Rasterizer.FillMode = FillMode.EvenOdd;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRGB (1, 0, 0));
		}
		
		private void NonZeroFill_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
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
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRGB (1, 0, 0));
		}
		
		private void Clipping_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (15, cx, cy);
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
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
			e.Graphics.SetClippingRectangle (50, 50, root.Client.Width - 100, 50);
			
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRGB (1, 0, 0));
			e.Graphics.ResetClippingRectangle ();
		}
		
		private void Transform_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (15, cx, cy);
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
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.RenderSolid (Color.FromRGB (1, 0, 0));
			
			e.Graphics.AddText (0, cy + 10, root.Client.Width, 20, "The quick brown fox jumps over the lazy dog !", font, 12, ContentAlignment.BottomCenter);
			e.Graphics.RenderSolid (Color.FromRGB (0, 0, 0.4));
		}
		
		private void Gradient_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (0, cx, cy);
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
				a[i] = 1.0;
			}
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
			e.Graphics.AddFilledRectangle (0, cy+50, root.Client.Width, 10);
			e.Graphics.Rasterizer.AddSurface (path1);
			e.Graphics.Rasterizer.AddSurface (path2);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'),  30, 60, 100);
			e.Graphics.Rasterizer.AddGlyph (font, font.GetGlyphIndex ('A'), 230, 60, 100);
			e.Graphics.GradientRenderer.Fill = Epsitec.Common.Drawing.GradientFill.X;
			e.Graphics.GradientRenderer.SetParameters (0, root.Client.Width);
			e.Graphics.GradientRenderer.SetColors (r, g, b, a);
			
			Transform t = new Transform ();
			t.Rotate (30, cx, cy);
			e.Graphics.GradientRenderer.Transform = t;
			
			e.Graphics.RenderGradient ();
		}
		
		private void Image_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (-30, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Font   font = Font.GetFont ("Arial Black", "Regular");
			double size = 50;
			
			e.Graphics.Rasterizer.FillMode = FillMode.NonZero;
			Bitmap bitmap = Support.ImageProvider.Default.GetImage (@"file:..\..\test.png").BitmapImage;
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
				e.Graphics.ImageRenderer.Bitmap = bitmap;
				
				Transform t = new Transform ();
				t.Scale (width / bitmap.Width, height / bitmap.Height);
				t.Translate (x, y);
				
				e.Graphics.ImageRenderer.Transform = t;
				e.Graphics.RenderImage ();
				
				x += width;
			}
		}
		
		private void ImageRect_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (0, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Bitmap bitmap = Support.ImageProvider.Default.GetImage (@"file:..\..\test.png").BitmapImage;
			
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
		
		private void ImageRectTIFF_PaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			
			double cx = root.Client.Width / 2;
			double cy = root.Client.Height / 2;
			
			e.Graphics.RotateTransform (0, cx, cy);
			
			e.Graphics.AddLine (cx, cy-5, cx, cy+5);
			e.Graphics.AddLine (cx-5, cy, cx+5, cy);
			e.Graphics.RenderSolid (Color.FromBrightness (0));
			
			Bitmap bitmap = Support.ImageProvider.Default.GetImage (@"file:..\..\image1.tif").BitmapImage;
			
			//	L'image fait 96 x 96 pixels
			
			e.Graphics.PaintImage (bitmap, new Rectangle ( 10,  10,  40,  40), new Rectangle (32, 32, 32, 32));
			e.Graphics.PaintImage (bitmap, new Rectangle ( 60,  10,  64,  64), new Rectangle (32, 32, 32, 32));
			e.Graphics.PaintImage (bitmap, new Rectangle ( 10,  60, 100, 100), new Rectangle (32, 32, 32, 32));
		}
	}
}

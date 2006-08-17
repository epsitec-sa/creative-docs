using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class PixmapTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckPixmapNew()
		{
			Pixmap pixmap = new Pixmap ();
			
			pixmap.Size = new System.Drawing.Size (200, 100);
			
			int width;
			int height;
			int stride;
			
			System.Drawing.Imaging.PixelFormat format;
			System.IntPtr scan0;
			
			pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
			
			Assert.AreEqual (200, width);
			Assert.AreEqual (100, height);
			Assert.AreEqual (800, stride);
			Assert.AreEqual (System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
			Assert.IsFalse (scan0 == System.IntPtr.Zero);
			Assert.IsTrue (pixmap.GetMemoryBitmapHandle () == System.IntPtr.Zero);
			
			pixmap.Dispose ();
		}
		
		[Test] public void CheckAllocatePixmap()
		{
			Pixmap pixmap = new Pixmap ();
			System.IntPtr dc = System.IntPtr.Zero;
			
			pixmap.AllocatePixmap (new System.Drawing.Size (200, 100));
			
			int width;
			int height;
			int stride;
			
			System.Drawing.Imaging.PixelFormat format;
			System.IntPtr scan0;
			
			pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
			
			Assert.AreEqual (200, width);
			Assert.AreEqual (100, height);
			Assert.AreEqual (800, stride);
			Assert.AreEqual (System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
			Assert.IsFalse (scan0 == System.IntPtr.Zero);
			Assert.IsFalse (pixmap.GetMemoryBitmapHandle () == System.IntPtr.Zero);
			
			pixmap.Dispose ();
		}
		
		[Test] public void CheckPixmapPaintInOSBitmap()
		{
			Graphics graphics = new Graphics ();
			Pixmap   pixmap   = graphics.Pixmap;
			
			graphics.AllocatePixmap ();
			graphics.SetPixmapSize (200, 100);
			
			int width;
			int height;
			int stride;
			
			System.Drawing.Imaging.PixelFormat format;
			System.IntPtr scan0;
			
			pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
			
			Assert.AreEqual (200, width);
			Assert.AreEqual (100, height);
			Assert.AreEqual (800, stride);
			Assert.AreEqual (System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
			Assert.IsFalse (scan0 == System.IntPtr.Zero);
			Assert.IsFalse (pixmap.GetMemoryBitmapHandle () == System.IntPtr.Zero);
			
			//	Bitmap sélectionné dans le DC : on peut peindre ici...
			
			OpenType.FontCollection font_collection = new OpenType.FontCollection ();
			font_collection.Initialize ();
			
			OpenType.Font font = font_collection.CreateFont ("Palatino Linotype", "Regular");
			double        size = 24.0;
			
			font.SelectFontManager (OpenType.FontManagerType.System);
			font.SelectFeatures ("liga", "dlig");
			
			System.IntPtr hfont  = font.GetFontHandle (size);
			ushort[]      glyphs = font.GenerateGlyphs ("Quelle affiche!");
			
//-			graphics.AddText (10, 10, "Quelle affiche!", Font.GetFont (font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName), size);
//-			graphics.RenderSolid (Color.FromName ("Red"));
			
			int x  = 10;
			int y1 = (100-10);
			int y2 = (100-40);
			
			int[]    deltas = new int[glyphs.Length];
			double[] x_pos  = new double[glyphs.Length];
			
			font.GetPositions (glyphs, size, 0.0, x_pos);
			
			for (int i = 0; i < deltas.Length-1; i++)
			{
				deltas[i] = (int) (x_pos[i+1] - x_pos[i+0]);
			}
			
			AntiGrain.Buffer.DrawGlyphs (pixmap.Handle, hfont, x, y1 - (int) font.GetAscender (size), glyphs, null, glyphs.Length, 0x000000);
			AntiGrain.Buffer.DrawGlyphs (pixmap.Handle, hfont, x, y2 - (int) font.GetAscender (size), glyphs, deltas, glyphs.Length, 0xFF0000);
			
			Bitmap image = Bitmap.FromPixmap (pixmap) as Bitmap;
			byte[] data  = image.Save (ImageFormat.Bmp, 32, 1, ImageCompression.None);
			
			using (System.IO.FileStream stream = new System.IO.FileStream ("os-bitmap-32.bmp", System.IO.FileMode.OpenOrCreate))
			{
				stream.Write (data, 0, data.Length);
			}
			
			pixmap.Dispose ();
		}
		
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] static extern int SetBkMode(System.IntPtr hdc, int mode);
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] static extern bool ExtTextOut(System.IntPtr hdc, int x, int y, int options, System.IntPtr rect, [System.Runtime.InteropServices.In] ushort[] text, int count, [System.Runtime.InteropServices.In] int[] dx);
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] extern static System.IntPtr CreateCompatibleDC(System.IntPtr hdc);
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] extern static bool DeleteDC(System.IntPtr hdc);
		[System.Runtime.InteropServices.DllImport ("GDI32.dll")] extern static System.IntPtr SelectObject(System.IntPtr hdc, System.IntPtr gdi_object);
		
		[Test] public void CheckPixmapCopy()
		{
			Graphics cache = new Graphics ();
			Pixmap[] stack = new Pixmap[3];
			
			cache.SetPixmapSize (400, 400);
			
			cache.SolidRenderer.ClearAlphaRgb (1, 1, 1, 1);
			
			cache.Color = Color.FromAlphaRgb (0.5, 1, 0, 0);
			cache.PaintSurface (Path.FromCircle (40, 40, 20));
			cache.Color = Color.FromAlphaRgb (0.5, 0, 1, 0);
			cache.PaintSurface (Path.FromCircle (52, 32, 16));
			cache.Color = Color.FromAlphaRgb (0.5, 0, 0, 1);
			cache.PaintSurface (Path.FromCircle (52, 48, 12));
			
			stack[0] = new Pixmap ();
			stack[0].Size = new System.Drawing.Size (400, 400);
			stack[0].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			cache.SolidRenderer.ClearAlphaRgb (0, 0, 0, 0);
			cache.Color = Color.FromAlphaRgb (0.5, 1, 0, 0);
			cache.PaintSurface (Path.FromCircle (200, 200, 100));
			
			Color c1 = cache.Pixmap.GetPixel (50, 50);
			Color c2 = cache.Pixmap.GetPixel (200, 200);
			
			System.Console.Out.WriteLine ("Background: {0}", c1);
			System.Console.Out.WriteLine ("Red 50%:    {0}", c2);
			
			stack[1] = new Pixmap ();
			stack[1].Size = new System.Drawing.Size (400, 400);
			stack[1].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			cache.SolidRenderer.ClearAlphaRgb (0, 0, 0, 0);
			cache.Color = Color.FromAlphaRgb (0.5, 0, 1, 0);
			cache.PaintSurface (Path.FromCircle (260, 160, 80));
			cache.Color = Color.FromAlphaRgb (0.5, 0, 0, 1);
			cache.PaintSurface (Path.FromCircle (260, 240, 60));
			
			c1 = cache.Pixmap.GetPixel (50, 50);
			c2 = cache.Pixmap.GetPixel (260, 160);
			
			System.Console.Out.WriteLine ("Background: {0}", c1);
			System.Console.Out.WriteLine ("Green 50%:  {0}", c2);
			
			stack[2] = new Pixmap ();
			stack[2].Size = new System.Drawing.Size (400, 400);
			stack[2].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			Widgets.Window window = new Widgets.Window ();
			window.ClientSize = new Size (400, 400);
			Widgets.Widget widget = new TestWidget (stack);
			
			widget.Dock   = Widgets.DockStyle.Fill;
			widget.SetParent (window.Root);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		protected class TestWidget : Widgets.Widget
		{
			public TestWidget(Pixmap[] stack)
			{
				this.stack = stack;
			}
			
			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				int dx = (int) (this.Client.Size.Width);
				int dy = (int) (this.Client.Size.Height);
				
				for (int i = 0; i < this.stack.Length; i++)
				{
					if (i == 0)
					{
						graphics.Pixmap.Copy (0, 0, this.stack[i], 0, 0, dx, dy);
					}
					else
					{
						graphics.Pixmap.Compose (0, 0, this.stack[i], 0, 0, dx, dy);
					}
				}
			}
			
			Pixmap[]				stack;
		}
	}
}

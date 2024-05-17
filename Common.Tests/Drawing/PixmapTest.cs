using Epsitec.Common.Drawing;
using Epsitec.Common.Drawing.Platform;
using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class PixmapTest
    {
        [SetUp]
        public void Initialization()
        {
            Epsitec.Common.Drawing.Font.Initialize();
            Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
        }

        [Test]
        [Ignore("Disabled (does not build)")]
        public void CheckPixmapNew()
        {
            /*
            DrawingBitmap pixmap = new DrawingBitmap();

            pixmap.Size = new System.Drawing.Size(200, 100);

            int width;
            int height;
            int stride;

            System.Drawing.Imaging.PixelFormat format;
            System.IntPtr scan0;

            pixmap.GetMemoryLayout(out width, out height, out stride, out format, out scan0);

            Assert.AreEqual(200, width);
            Assert.AreEqual(100, height);
            Assert.AreEqual(800, stride);
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
            Assert.IsFalse(scan0 == System.IntPtr.Zero);
            Assert.IsFalse(pixmap.GetMemoryBitmapHandle() == System.IntPtr.Zero);

            pixmap.Dispose();
            */
        }

        [Test]
        [Ignore("Reported broken by Marc Bettex")]
        public void CheckAllocatePixmapFromImageClient()
        {
            /*
            string path = @"..\..\..\..\Images\picture.png";

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            DrawingBitmap pixmap0;
            DrawingBitmap pixmap1;
            DrawingBitmap pixmap2;

            watch.Start();
            pixmap0 = PixmapTest.CreatePixmapUsingImageClient(path, -1, true);
            watch.Stop();
            watch.Reset();

            watch.Start();
            pixmap1 = PixmapTest.CreatePixmapUsingImageClient(path, -1, false);
            watch.Stop();
            System.Console.Out.WriteLine(
                "Loading of '{0}' took {1} ms",
                path,
                watch.ElapsedMilliseconds
            );
            watch.Reset();

            path = @"..\..\..\..\Images\photo.jpg";

            watch.Start();
            pixmap2 = PixmapTest.CreatePixmapUsingImageClient(path, 200, false);
            watch.Stop();
            System.Console.Out.WriteLine(
                "Loading of '{0}' took {1} ms",
                path,
                watch.ElapsedMilliseconds
            );
            watch.Reset();

            Assert.IsNull(pixmap0.AssociatedImage);
            Assert.IsNotNull(pixmap1.AssociatedImage);
            Assert.IsNull(pixmap2.AssociatedImage);

            PlatformWindow window = new PlatformWindow();
            window.ClientSize = new Size(640, 220);
            window.Text = "CheckAllocatePixmapFromImageClient";
            window.Root.PaintForeground += delegate(object sender, PaintEventArgs e)
            {
                Bitmap bitmap0 = Bitmap.FromPixmap(pixmap0).BitmapImage;
                Bitmap bitmap1 = Bitmap.FromPixmap(pixmap1).BitmapImage;
                Bitmap bitmap2 = Bitmap.FromPixmap(pixmap2).BitmapImage;

                e.Graphics.PaintImage(
                    bitmap0,
                    new Rectangle(10, 10, 200, 200),
                    new Rectangle(0, 0, 200, 200)
                );
                e.Graphics.PaintImage(
                    bitmap1,
                    new Rectangle(220, 10, 200, 200),
                    new Rectangle(0, 0, 200, 200)
                );
                e.Graphics.PaintImage(
                    bitmap2,
                    new Rectangle(430, 10, 200, 200),
                    new Rectangle(0, 0, 200, 200)
                );
            };

            window.Root.Invalidate();
            window.Show();
            PlatformWindow.RunInTestEnvironment(window);

            pixmap1.AssociatedImage.Dispose();

            pixmap0.Dispose();
            pixmap1.Dispose();
            pixmap2.Dispose();
            */
            throw new System.NotImplementedException();
        }

        private static DrawingBitmap CreatePixmapUsingImageClient(
            string path,
            int size,
            bool copyBits
        )
        {
            DrawingBitmap pixmap;
            NativeBitmap image = null;

            try
            {
                if (size > 0)
                {
                    image = NativeBitmap.Load(path);
                    image = image.MakeThumbnail(size);
                }
                else
                {
                    image = NativeBitmap.Load(path);
                }

                pixmap = new DrawingBitmap((uint)image.Width, (uint)image.Height);
                pixmap.AllocatePixmap(image);
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }

            return pixmap;
        }

        [Test]
        [Ignore("Disabled (does not build)")]
        public void CheckAllocatePixmap()
        {
            /*
            DrawingBitmap pixmap = new DrawingBitmap();
            System.IntPtr dc = System.IntPtr.Zero;

            pixmap.AllocatePixmap(new System.Drawing.Size(200, 100));

            int width;
            int height;
            int stride;

            System.Drawing.Imaging.PixelFormat format;
            System.IntPtr scan0;

            pixmap.GetMemoryLayout(out width, out height, out stride, out format, out scan0);

            Assert.AreEqual(200, width);
            Assert.AreEqual(100, height);
            Assert.AreEqual(800, stride);
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
            Assert.IsFalse(scan0 == System.IntPtr.Zero);
            Assert.IsFalse(pixmap.GetMemoryBitmapHandle() == System.IntPtr.Zero);

            pixmap.Dispose();
            */
        }

        [Test]
        [Ignore("Disabled (does not build)")]
        public void CheckPixmapPaintInOSBitmap()
        {
            /*
            Graphics graphics = new Graphics();
            DrawingBitmap pixmap = graphics.DrawingBitmap;

            graphics.AllocatePixmap();
            graphics.SetPixmapSize(200, 100);

            int width;
            int height;
            int stride;

            System.Drawing.Imaging.PixelFormat format;
            System.IntPtr scan0;

            pixmap.GetMemoryLayout(out width, out height, out stride, out format, out scan0);

            Assert.AreEqual(200, width);
            Assert.AreEqual(100, height);
            Assert.AreEqual(800, stride);
            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format32bppPArgb, format);
            Assert.IsFalse(scan0 == System.IntPtr.Zero);
            Assert.IsFalse(pixmap.GetMemoryBitmapHandle() == System.IntPtr.Zero);

            //	Bitmap sélectionné dans le DC : on peut peindre ici...

            OpenType.FontCollection font_collection = new OpenType.FontCollection();
            font_collection.Initialize();

            OpenType.Font font = font_collection.CreateFont("Palatino Linotype", "Regular");
            double size = 24.0;

            font.SelectFontManager(OpenType.FontManagerType.System);
            font.SelectFeatures("liga", "dlig");

            System.IntPtr hfont = font.GetFontHandle(size);
            ushort[] glyphs = font.GenerateGlyphsWithMask("Quelle affiche!");

            //-			graphics.AddText (10, 10, "Quelle affiche!", Font.GetFont (font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName), size);
            //-			graphics.RenderSolid (Color.FromName ("Red"));

            int x = 10;
            int y1 = (100 - 10);
            int y2 = (100 - 40);

            int[] deltas = new int[glyphs.Length];
            double[] x_pos = new double[glyphs.Length];

            font.GetPositions(glyphs, size, 0.0, x_pos);

            for (int i = 0; i < deltas.Length - 1; i++)
            {
                deltas[i] = (int)(x_pos[i + 1] - x_pos[i + 0]);
            }

            AntigrainCPP.Buffer.DrawGlyphs(
                pixmap.Handle,
                hfont,
                x,
                y1 - (int)font.GetAscender(size),
                glyphs,
                null,
                glyphs.Length,
                0x000000
            );
            AntigrainCPP.Buffer.DrawGlyphs(
                pixmap.Handle,
                hfont,
                x,
                y2 - (int)font.GetAscender(size),
                glyphs,
                deltas,
                glyphs.Length,
                0xFF0000
            );

            Bitmap image = Bitmap.FromPixmap(pixmap) as Bitmap;
            byte[] data = image.Save(ImageFormat.Bmp, 32, 1, ImageCompression.None);

            using (
                System.IO.FileStream stream = new System.IO.FileStream(
                    "os-bitmap-32.bmp",
                    System.IO.FileMode.OpenOrCreate
                )
            )
            {
                stream.Write(data, 0, data.Length);
            }

            pixmap.Dispose();
            */
        }

        [Test]
        [Ignore("Pixmap copy not implemented")]
        public void CheckPixmapCopy()
        {
            /*
            Graphics cache = new Graphics();
            DrawingBitmap[] stack = new DrawingBitmap[3];

            cache.SetPixmapSize(400, 400);

            cache.SolidRenderer.ClearAlphaRgb(1, 1, 1, 1);

            cache.Color = Color.FromAlphaRgb(0.5, 1, 0, 0);
            cache.PaintSurface(Path.FromCircle(40, 40, 20));
            cache.Color = Color.FromAlphaRgb(0.5, 0, 1, 0);
            cache.PaintSurface(Path.FromCircle(52, 32, 16));
            cache.Color = Color.FromAlphaRgb(0.5, 0, 0, 1);
            cache.PaintSurface(Path.FromCircle(52, 48, 12));

            stack[0] = new DrawingBitmap();
            stack[0].Size = new System.Drawing.Size(400, 400);
            stack[0].Copy(0, 0, cache.Pixmap, 0, 0, 400, 400);

            cache.SolidRenderer.ClearAlphaRgb(0, 0, 0, 0);
            cache.Color = Color.FromAlphaRgb(0.5, 1, 0, 0);
            cache.PaintSurface(Path.FromCircle(200, 200, 100));

            Color c1 = cache.Pixmap.GetPixel(50, 50);
            Color c2 = cache.Pixmap.GetPixel(200, 200);

            System.Console.Out.WriteLine("Background: {0}", c1);
            System.Console.Out.WriteLine("Red 50%:    {0}", c2);

            stack[1] = new DrawingBitmap();
            stack[1].Size = new System.Drawing.Size(400, 400);
            stack[1].Copy(0, 0, cache.Pixmap, 0, 0, 400, 400);

            cache.SolidRenderer.ClearAlphaRgb(0, 0, 0, 0);
            cache.Color = Color.FromAlphaRgb(0.5, 0, 1, 0);
            cache.PaintSurface(Path.FromCircle(260, 160, 80));
            cache.Color = Color.FromAlphaRgb(0.5, 0, 0, 1);
            cache.PaintSurface(Path.FromCircle(260, 240, 60));

            c1 = cache.Pixmap.GetPixel(50, 50);
            c2 = cache.Pixmap.GetPixel(260, 160);

            System.Console.Out.WriteLine("Background: {0}", c1);
            System.Console.Out.WriteLine("Green 50%:  {0}", c2);

            stack[2] = new DrawingBitmap();
            stack[2].Size = new System.Drawing.Size(400, 400);
            stack[2].Copy(0, 0, cache.Pixmap, 0, 0, 400, 400);

            PlatformWindow window = new PlatformWindow();
            window.ClientSize = new Size(400, 400);
            Widget widget = new TestWidget(stack);

            widget.Dock = DockStyle.Fill;
            widget.SetParent(window.Root);

            window.Show();
            PlatformWindow.RunInTestEnvironment(window);
            */
            throw new System.NotImplementedException();
        }

        /*
        protected class TestWidget : Widget
        {
            public TestWidget(DrawingBitmap[] stack)
            {
                this.stack = stack;
            }

            protected override void PaintBackgroundImplementation(
                Graphics graphics,
                Rectangle clip_rect
            )
            {
                int dx = (int)(this.Client.Size.Width);
                int dy = (int)(this.Client.Size.Height);

                for (int i = 0; i < this.stack.Length; i++)
                {
                    if (i == 0)
                    {
                        graphics.Pixmap.Copy(0, 0, this.stack[i], 0, 0, dx, dy);
                    }
                    else
                    {
                        graphics.Pixmap.Compose(0, 0, this.stack[i], 0, 0, dx, dy);
                    }
                }
            }

            DrawingBitmap[] stack;
        }
        */
    }
}

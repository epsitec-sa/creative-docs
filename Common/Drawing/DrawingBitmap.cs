//	Copyright © 2003-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Platform;
using Epsitec.Common.Text.Properties;

namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// DrawingBitmap represents a bitmap in which we can draw
    /// This implementation is using antigrain buffers
    /// </summary>
    public class DrawingBitmap : Bitmap
    {
        public DrawingBitmap(uint width, uint height)
        {
            int stride = -(int)width * 4; // 4 bytes per pixel, one byte per color channel
            this.buffer = new AntigrainSharp.GraphicBuffer(width, height, stride, Font.FontManager);
        }

        public AntigrainSharp.GraphicContext GraphicContext
        {
            get { return this.buffer.GraphicContext; }
        }

        public override Size Size
        {
            get { return new Size(this.buffer.Height, this.buffer.Width); }
        }

        public override int Stride
        {
            get { return (int)this.buffer.Stride; }
        }

        public override int PixelWidth
        {
            get { return (int)this.buffer.Width; }
        }
        public override int PixelHeight
        {
            get { return (int)this.buffer.Height; }
        }

        public override byte[] GetPixelBuffer()
        {
            return this.buffer.GetBufferData();
        }

        #region NotImplemented

        /// <summary>
        /// Allocates a pixmap based on a FreeImage image instance.
        /// </summary>
        /// <param name="image">The FreeImage image instance.</param>
        /// <param name="copyImageBits">Specifies if the image bits must be copied.</param>
        /// <returns><c>true</c> if the image bits were inherited directly, without any copy (the
        /// image must stay alive as long as the pixmap in that case).</returns>
        public bool AllocatePixmap(NativeBitmap image)
        {
            /*
            if ((this.size.IsEmpty) && (this.aggBuffer == System.IntPtr.Zero))
            {
                NativeBitmap temp = null;

                if (image.BitsPerPixel < 24)
                {
                    temp = image.ConvertToPremultipliedArgb32();
                    image = temp;
                }

                int bitsPerPixel = 32;
                int width = image.Width;
                int height = image.Height;
                int pitch = width * 4;
                int bufferSize = pitch * height;
                var bufferMemory = System.Runtime.InteropServices.Marshal.AllocHGlobal(bufferSize);

                image.CopyPixelsToBuffer(bufferMemory, bufferSize, pitch);

                this.aggBuffer = AntigrainCPP.Buffer.NewFrom(
                    width,
                    height,
                    bitsPerPixel,
                    pitch,
                    bufferMemory,
                    copyBits: true
                );

                System.Runtime.InteropServices.Marshal.FreeHGlobal(bufferMemory);

                this.size = new System.Drawing.Size(width, height);
                this.isOsBitmap = true;

                if (temp != null)
                {
                    temp.Dispose();
                }

                return false;
            }
            else
            {
                throw new System.InvalidOperationException("Cannot re-allocate pixmap.");
            }
            */
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            /*
            AntigrainCPP.Buffer.Clear(this.aggBuffer);
            */
            throw new System.NotImplementedException();
        }

        public void Paint(System.IntPtr hdc, System.Drawing.Rectangle clip)
        {
            /*
            AntigrainCPP.Buffer.Paint(
                this.aggBuffer,
                hdc,
                clip.Left,
                clip.Bottom,
                clip.Right,
                clip.Top
            );
            */
            throw new System.NotImplementedException();
        }

        public void Paint(
            System.IntPtr hdc,
            System.Drawing.Point offset,
            System.Drawing.Rectangle clip
        )
        {
            /*
            AntigrainCPP.Buffer.PaintOffset(
                this.aggBuffer,
                hdc,
                offset.X,
                offset.Y,
                clip.Left,
                clip.Bottom,
                clip.Right,
                clip.Top
            );
            */
            throw new System.NotImplementedException();
        }

        public void Blend(
            System.IntPtr hdc,
            System.Drawing.Point offset,
            System.Drawing.Rectangle clip
        )
        {
            /*
            AntigrainCPP.Buffer.BlendOffset(
                this.aggBuffer,
                hdc,
                offset.X,
                offset.Y,
                clip.Left,
                clip.Bottom,
                clip.Right,
                clip.Top
            );
            */
            throw new System.NotImplementedException();
        }

        public void Compose(
            int x,
            int y,
            DrawingBitmap source,
            int xSource,
            int ySource,
            int width,
            int height
        )
        {
            /*
            AntigrainCPP.Buffer.ComposeBuffer(
                this.aggBuffer,
                x,
                y,
                source.aggBuffer,
                xSource,
                ySource,
                width,
                height
            );
            */
            throw new System.NotImplementedException();
        }

        public void Copy(
            int x,
            int y,
            DrawingBitmap source,
            int xSource,
            int ySource,
            int width,
            int height
        )
        {
            /*
            AntigrainCPP.Buffer.BltBuffer(
                this.aggBuffer,
                x,
                y,
                source.aggBuffer,
                xSource,
                ySource,
                width,
                height
            );
            */
            throw new System.NotImplementedException();
        }

        public void Erase(System.Drawing.Rectangle clip)
        {
            /*
            AntigrainCPP.Buffer.ClearRect(
                this.aggBuffer,
                clip.Left,
                clip.Top,
                clip.Right,
                clip.Bottom
            );
            */
            throw new System.NotImplementedException();
        }

        public void GetMemoryLayout(
            out int width,
            out int height,
            out int stride,
            //out System.Drawing.Imaging.PixelFormat format,
            out System.IntPtr scan0
        )
        {
            /*
            //format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            scan0 = AntigrainCPP.Buffer.GetMemoryLayout(
                this.aggBuffer,
                out width,
                out height,
                out stride
            );
            */
            throw new System.NotImplementedException();
        }

        public System.IntPtr GetMemoryBitmapHandle()
        {
            /*
            return AntigrainCPP.Buffer.GetMemoryBitmapHandle(this.aggBuffer);
            */
            throw new System.NotImplementedException();
        }

        public void InfiniteClipping()
        {
            /*
            AntigrainCPP.Buffer.InfiniteClipping(this.aggBuffer);
            */
            throw new System.NotImplementedException();
        }

        public void EmptyClipping()
        {
            /*
            AntigrainCPP.Buffer.EmptyClipping(this.aggBuffer);
            */
            throw new System.NotImplementedException();
        }

        public void AddClipBox(double x1, double y1, double x2, double y2)
        {
            /*
            int cx1 = (int)(x1);
            int cy1 = (int)(y1);
            int cx2 = (int)(x2 + 0.9999);
            int cy2 = (int)(y2 + 0.9999);
            AntigrainCPP.Buffer.AddClipBox(this.aggBuffer, cx1, cy1, cx2 - 1, cy2 - 1);
            */
            throw new System.NotImplementedException();
        }

        public Color GetPixel(int x, int y)
        {
            if ((x < 0) || (x >= this.Size.Width) || (y < 0) || (y >= this.Size.Height))
            {
                return Color.Empty;
            }

            using (DrawingBitmap.RawData src = new DrawingBitmap.RawData(this))
            {
                return src[x, y];
            }
        }

        public void PremultiplyAlpha()
        {
            int pixWidth;
            int pixHeight;
            int pixStride;

            //System.Drawing.Imaging.PixelFormat pixFormat;
            System.IntPtr pixScan0;

            this.GetMemoryLayout(
                out pixWidth,
                out pixHeight,
                out pixStride,
                //out pixFormat,
                out pixScan0
            );

            if (pixScan0 == System.IntPtr.Zero)
            {
                return;
            }
            /*
            if (
                (pixFormat == PixelFormat.Format32bppArgb)
                || (pixFormat == PixelFormat.Format32bppPArgb)
            ) */
            unsafe
            {
                byte* pixData = (byte*)pixScan0.ToPointer();

                for (int y = 0; y < pixHeight; y++)
                {
                    byte* row = pixData + pixStride * y;

                    for (int x = 0; x < pixWidth; x++)
                    {
                        int a = row[3];
                        int r = row[2];
                        int g = row[1];
                        int b = row[0];

                        if ((a != 0) && (a != 255))
                        {
                            r = r * a / 255;
                            g = g * a / 255;
                            b = b * a / 255;

                            row[2] = (byte)r;
                            row[1] = (byte)g;
                            row[0] = (byte)b;
                        }

                        row += 4;
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members
        public override void Dispose()
        {
            this.buffer.Dispose();
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            /*
            if (disposing)
            {
                //	Nothing 'managed' to dispose here
            }

            if (this.aggBuffer != System.IntPtr.Zero)
            {
                AntigrainCPP.Buffer.Delete(this.aggBuffer);
                this.aggBuffer = System.IntPtr.Zero;
            }
            */
        }

        #region RawData Class
        public class RawData : System.IDisposable
        {
            public RawData(DrawingBitmap pixmap)
            {
                /*
                pixmap.GetMemoryLayout(
                    out this.dx,
                    out this.dy,
                    out this.stride,
                    out this.format,
                    out this.pixels
                );
                */
            }

            /*            public RawData(System.Drawing.Bitmap bitmap)
                            : this(bitmap, System.Drawing.Imaging.PixelFormat.Format32bppArgb) { }
            */
            /*            public RawData(System.Drawing.Bitmap bitmap, System.Drawing.Imaging.PixelFormat format)
                        {
                            System.Drawing.Rectangle clip = new System.Drawing.Rectangle(
                                0,
                                0,
                                bitmap.Width,
                                bitmap.Height
                            );
                            System.Drawing.Imaging.ImageLockMode mode = System
                                .Drawing
                                .Imaging
                                .ImageLockMode
                                .ReadWrite;
            
                            this.bm = bitmap;
                            this.bmData = bitmap.LockBits(clip, mode, format);
            
                            this.stride = this.bmData.Stride;
                            this.pixels = this.bmData.Scan0;
                            this.dx = this.bmData.Width;
                            this.dy = this.bmData.Height;
                            this.format = format;
                        }
            */

            /*            public RawData(Image image)
                            : this(image.BitmapImage.NativeBitmap) { }
            */
            public RawData(Image image)
            {
                throw new System.NotImplementedException();
            }

            /*            public PixelFormat PixelFormat
                        {
                            get { return this.format; }
                        }
            */
            public int Stride
            {
                get { return this.stride; }
            }

            public System.IntPtr Pixels
            {
                get { return this.pixels; }
            }

            public int Width
            {
                get { return this.dx; }
            }

            public int Height
            {
                get { return this.dy; }
            }

            public bool IsBottomUp
            {
                //get { return this.bm == null; }
                get { return false; }
            }

            public Color this[int x, int y]
            {
                get
                {
                    /*
                    byte r,
                        g,
                        b,
                        a;

                    switch (this.format)
                    {
                        case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                        case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                        case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                            break;
                        default:
                            throw new System.InvalidOperationException(
                                string.Format("Cannot access pixel of format {0}.", this.format)
                            );
                    }

                    unsafe
                    {
                        byte* ptr = x * 4 + y * this.stride + (byte*)this.Pixels.ToPointer();
                        a = ptr[3];
                        r = ptr[2];
                        g = ptr[1];
                        b = ptr[0];
                    }

                    return Color.FromAlphaRgb(a / 255.0, r / 255.0, g / 255.0, b / 255.0);
                    */
                    return Color.Empty;
                }
                set
                {
                    /*
                    byte r = (byte)(value.R * 255.0 + .5);
                    byte g = (byte)(value.G * 255.0 + .5);
                    byte b = (byte)(value.B * 255.0 + .5);
                    byte a = (byte)(value.A * 255.0 + .5);

                    switch (this.format)
                    {
                        case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                        case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                        case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                            break;
                        default:
                            throw new System.InvalidOperationException(
                                string.Format("Cannot access pixel of format {0}.", this.format)
                            );
                    }

                    unsafe
                    {
                        byte* ptr = x * 4 + y * this.stride + (byte*)this.Pixels.ToPointer();
                        ptr[3] = a;
                        ptr[2] = r;
                        ptr[1] = g;
                        ptr[0] = b;
                    }
                    */
                }
            }

            #region IDisposable Members
            public void Dispose()
            {
                this.Dispose(true);
                System.GC.SuppressFinalize(this);
            }
            #endregion

            public void CopyFrom(RawData that)
            {
                /*
                if (this.format != that.format)
                {
                    throw new System.InvalidOperationException(
                        string.Format("Cannot copy formats {0} to {1}.", this.format, that.format)
                    );
                }

                int bpp = 0;

                switch (this.format)
                {
                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                        bpp = 4;
                        break;
                }

                if (bpp == 0)
                {
                    throw new System.InvalidOperationException(
                        string.Format("Copy does not support format {0}.", this.format)
                    );
                }

                unsafe
                {
                    System.IntPtr srcscan0 = that.Pixels;
                    System.IntPtr dstscan0 = this.Pixels;
                    int srcstride = that.Stride;
                    int dststride = this.Stride;

                    int dx = System.Math.Min(this.Width, that.Width);
                    int dy = System.Math.Min(this.Height, that.Height);

                    int* srcdata = (int*)srcscan0.ToPointer();
                    int* dstdata = (int*)dstscan0.ToPointer();
                    int srcymul = srcstride / 4;
                    int dstymul = dststride / 4;

                    if (that.IsBottomUp == false)
                    {
                        srcdata += that.Height * srcymul;
                        srcdata -= srcymul;
                        srcymul = -srcymul;
                    }
                    if (this.IsBottomUp == false)
                    {
                        dstdata += this.Height * dstymul;
                        dstdata -= dstymul;
                        dstymul = -dstymul;
                    }

                    for (int y = 0; y < dy; y++)
                    {
                        for (int x = 0; x < dx; x++)
                        {
                            dstdata[x] = srcdata[x];
                        }

                        srcdata += srcymul;
                        dstdata += dstymul;
                    }
                }
                */
            }

            public void CopyTo(RawData raw)
            {
                raw.CopyFrom(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                /*
                if (disposing)
                {
                    if (this.bm != null)
                    {
                        this.bm.UnlockBits(this.bmData);

                        this.bm = null;
                        this.bmData = null;
                    }
                }
                */
            }

            protected int stride;
            protected System.IntPtr pixels;
            protected int dx,
                dy;
            //protected PixelFormat format;

            //protected System.Drawing.Bitmap bm;
            //protected BitmapData bmData;
        }
        #endregion

        private AntigrainSharp.GraphicBuffer buffer;
    }
}

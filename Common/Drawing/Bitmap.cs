/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Runtime.InteropServices;
using Epsitec.Common.Drawing.Platform;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// La classe Bitmap encapsule une image de type bitmap.
    /// </summary>
    public abstract class Bitmap : Image
    {
        public override Bitmap BitmapImage
        {
            get { return this; }
        }

        public abstract int Stride { get; }

        public abstract byte[] GetPixelBuffer();

        public override void DefineZoom(double zoom) { }

        public override void DefineColor(Drawing.Color color) { }

        public override void DefineAdorner(object adorner) { }

        public abstract int PixelWidth { get; }

        public abstract int PixelHeight { get; }

        #region NotImplemented
        public byte[] GetRawBitmapBytes()
        {
            /*            this.LockBits();
            
                        try
                        {
                            if (this.bitmapData != null)
                            {
                                System.IntPtr memory = this.Scan0;
                                int size = this.bitmapData.Height * this.bitmapData.Stride;
            
                                if ((memory != System.IntPtr.Zero) && (size > 0))
                                {
                                    byte[] data = new byte[size];
                                    Marshal.Copy(memory, data, 0, size);
                                    return data;
                                }
                            }
                        }
                        finally
                        {
                            this.UnlockBits();
                        }
            */
            throw new System.NotImplementedException();
        }

        public static void Merge(
            Bitmap bitmapBlack,
            Bitmap bitmapWhite,
            bool alphaCorrect,
            bool alphaPremultiplied
        )
        {
            /*
            //	A partir de la même image, une fois sur fond noir et une fois sur
            //	fond blanc, on calcule les composantes rgb ainsi que la transparence,
            //	directement dans l'image blanche (pour éviter l'emploi d'un 3ème
            //	buffer).
            //	Ceci corrige un bug dans AntiGrain qui génère des images transparentes
            //	toutes fauses (les dégradés transparents ont une teinte blanche incorrecte).
            bitmapBlack.LockBits();
            bitmapWhite.LockBits();

            var sBlack = bitmapBlack.Scan0;
            var sWhite = bitmapWhite.Scan0;

            unsafe
            {
                int* iBlack = (int*)(bitmapBlack.Scan0.ToPointer());
                int* iWhite = (int*)(bitmapWhite.Scan0.ToPointer());

                for (int i = 0; i < bitmapBlack.Width * bitmapBlack.Height; i++)
                {
                    int bb = iBlack[i] & 0xff;
                    int gb = (iBlack[i] >> 8) & 0xff;
                    int rb = (iBlack[i] >> 16) & 0xff;
                    int ab = (iBlack[i] >> 24) & 0xff;

                    int bw = iWhite[i] & 0xff;
                    int gw = (iWhite[i] >> 8) & 0xff;
                    int rw = (iWhite[i] >> 16) & 0xff;
                    int aw = (iWhite[i] >> 24) & 0xff;

                    if (alphaCorrect)
                    {
                        Bitmap.MergePixel(rb, gb, bb, ref rw, ref gw, ref bw, ref aw);
                    }

                    if (alphaPremultiplied)
                    {
                        Bitmap.MultiplyAlpha(ref rw, ref gw, ref bw, ref aw);
                    }

                    iWhite[i] = bw | (gw << 8) | (rw << 16) | (aw << 24);
                }
            }

            bitmapBlack.UnlockBits();
            bitmapWhite.UnlockBits();
            */
            throw new System.NotImplementedException();
        }

        private static void MergePixel(
            int rb,
            int gb,
            int bb,
            ref int rw,
            ref int gw,
            ref int bw,
            ref int aw
        )
        {
            /*
            int ar = 255 - (rw - rb);
            int ag = 255 - (gw - gb);
            int ab = 255 - (bw - bb);

            if (ar == 0)
            {
                rw = 0;
            }
            else if (ar < 128)
            {
                rw = 255 - (((255 - rw) * 255) / ar);
            }
            else
            {
                rw = (rb * 255) / ar;
            }

            if (ag == 0)
            {
                gw = 0;
            }
            else if (ag < 128)
            {
                gw = 255 - (((255 - gw) * 255) / ag);
            }
            else
            {
                gw = (gb * 255) / ag;
            }

            if (ab == 0)
            {
                bw = 0;
            }
            else if (ab < 128)
            {
                bw = 255 - (((255 - bw) * 255) / ab);
            }
            else
            {
                bw = (bb * 255) / ab;
            }

            aw = (ar + ag + ab) / 3;
            */
            throw new System.NotImplementedException();
        }

        private static void MultiplyAlpha(ref int r, ref int g, ref int b, ref int a)
        {
            /*
            r = (r * a) / 256;
            g = (g * a) / 256;
            b = (b * a) / 256;
            */
            throw new System.NotImplementedException();
        }

        public void FlipY()
        {
            /*            try
                        {
                            this.LockBits();
            
                            int nx = this.bitmapDx / 2;
                            int ny = this.bitmapDy / 2;
                            int my = this.bitmapDy - 1;
            
                            unsafe
                            {
                                for (int y = 0; y < ny; y++)
                                {
                                    int y1 = y;
                                    int y2 = my - y;
                                    int* s1 =
                                        (int*)(this.bitmapData.Scan0.ToPointer())
                                        + y1 * this.bitmapData.Stride / 4;
                                    int* s2 =
                                        (int*)(this.bitmapData.Scan0.ToPointer())
                                        + y2 * this.bitmapData.Stride / 4;
            
                                    for (int x = 0; x < this.bitmapDx; x++)
                                    {
                                        int v1 = s1[x];
                                        int v2 = s2[x];
                                        s1[x] = v2;
                                        s2[x] = v1;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            this.UnlockBits();
                        }
            */
            throw new System.NotImplementedException();
        }

        public byte[] Save(ImageFormat format)
        {
            /*
            return this.Save(format, 0);
            */
            throw new System.NotImplementedException();
        }

        public byte[] Save(ImageFormat format, int depth)
        {
            /*
            int quality = 0;
            ImageCompression compression = ImageCompression.None;

            switch (format)
            {
                case ImageFormat.Bmp:
                case ImageFormat.Png:
                    if (depth == 0)
                    {
                        depth = 32;
                    }
                    break;

                case ImageFormat.Gif:
                    break;

                case ImageFormat.Tiff:
                    if (depth == 0)
                    {
                        depth = 32;
                    }
                    compression = ImageCompression.Lzw;
                    break;

                case ImageFormat.Jpeg:
                    depth = 24;
                    quality = 75;
                    break;

                case ImageFormat.WindowsIcon:
                    break;

                default:
                    throw new System.ArgumentException("Invalid format specified", "format");
            }

            return this.Save(format, depth, quality, compression);
            */
            throw new System.NotImplementedException();
        }

        public byte[] Save(ImageFormat format, int depth, int quality, ImageCompression compression)
        {
            //return this.Save(format, depth, quality, compression, 72.0);
            //return null;
            throw new System.NotImplementedException();
        }

        private byte[] SaveIcon(ImageFormat format)
        {
            /*
            byte[] imageBytes = this.GetRawBitmapBytes();

            switch (format)
            {
                case ImageFormat.WindowsIcon:
                    return NativeIcon.CreateIcon(
                        imageBytes,
                        this.Stride,
                        this.PixelWidth,
                        this.PixelHeight
                    );

                case ImageFormat.WindowsPngIcon:
                    return NativeIcon.CreatePngIcon(
                        imageBytes,
                        this.Stride,
                        this.PixelWidth,
                        this.PixelHeight
                    );

                default:
                    return null;
            }
            */
            throw new System.NotImplementedException();
        }

        public override Bitmap GetImageForPaintStyle(GlyphPaintStyle style)
        {
            /*
            switch (style)
            {
                case GlyphPaintStyle.Normal:
                    return this;
                case GlyphPaintStyle.Disabled:
                    return Bitmap.FromImageDisabled(this, Color.FromName("Control"));
            }

            return null;
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromImage(NativeBitmap image)
        {
            /*
            if ((image == null) || (image.IsValid == false))
            {
                return null;
            }

            try
            {
                DrawingBitmap pixmap = new DrawingBitmap();
                pixmap.AllocatePixmap(image);

                return Bitmap.FromPixmap(pixmap);
            }
            catch (System.NullReferenceException)
            {
                return null;
            }
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromPixmap(DrawingBitmap pixmap)
        {
            /*
            Bitmap bitmap = new Bitmap();

            bitmap.pixmap = pixmap;
            //bitmap.bitmap = null;
            bitmap.bitmapDx = pixmap.Size.Width;
            bitmap.bitmapDy = pixmap.Size.Height;
            bitmap.size = new Size(bitmap.bitmapDx, bitmap.bitmapDy);
            bitmap.origin = new Point(0, 0);

            //	Prétend que le bitmap est verrouillé, puisqu'on a de toute façons déjà accès aux
            //	pixels (c'est d'ailleurs bien la seule chose qu'on a) :

            //bitmap.bitmapLockCount = 1;
            bitmap.isOriginDefined = true;

            /*            int dx,
                            dy,
                            stride;
                        System.IntPtr pixels;
                        System.Drawing.Imaging.PixelFormat format;
            
                        pixmap.GetMemoryLayout(out dx, out dy, out stride, out format, out pixels);
            
                        bitmap.bitmapData = new BitmapData();
            
                        bitmap.bitmapData.Width = dx;
                        bitmap.bitmapData.Height = dy;
                        bitmap.bitmapData.PixelFormat = format;
                        bitmap.bitmapData.Scan0 = pixels;
                        bitmap.bitmapData.Stride = stride;
            //
            return bitmap;
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromNativeBitmap(int dx, int dy)
        {
            //return Bitmap.FromNativeBitmap(new System.Drawing.Bitmap(dx, dy));
            //return null;
            throw new System.NotImplementedException();
        }

        public static Bitmap FromNativeBitmap(byte[] data)
        {
            /*            var bitmap = Bitmap.DecompressBitmap(data);
                        return Bitmap.FromNativeBitmap(bitmap);
            */
            //  return null;
            throw new System.NotImplementedException();
        }

        public static Bitmap FromNativeIcon(string path, int dx, int dy)
        {
            //return Bitmap.FromNativeIcon(IconLoader.LoadIcon(path, dx, dy));
            //return null;
            throw new System.NotImplementedException();
        }

        /*        public static System.Drawing.Icon LoadNativeIcon(string path, int dx, int dy)
                {
                    return IconLoader.LoadIcon(path, dx, dy);
                }
        */
        public static int GetIconWidth(IconSize iconSize)
        {
            /*
            switch (iconSize)
            {
                case IconSize.Default:
                case IconSize.Normal:
                    return Win32Api.GetSystemMetrics(Win32Const.SM_CXICON);
                case IconSize.Small:
                    return Win32Api.GetSystemMetrics(Win32Const.SM_CXSMICON);
                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", iconSize.GetQualifiedName())
                    );
            }
            */
            throw new System.NotImplementedException();
        }

        public static int GetIconHeight(IconSize iconSize)
        {
            /*
            switch (iconSize)
            {
                case IconSize.Default:
                case IconSize.Normal:
                    return Win32Api.GetSystemMetrics(Win32Const.SM_CYICON);
                case IconSize.Small:
                    return Win32Api.GetSystemMetrics(Win32Const.SM_CYSMICON);
                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", iconSize.GetQualifiedName())
                    );
            }
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromNativeIcon(PlatformSystemIcon systemIcon)
        {
            /*            System.Drawing.Icon icon = null;
            
                        switch (systemIcon)
                        {
                            case PlatformSystemIcon.Application:
                                icon = System.Drawing.SystemIcons.Application;
                                break;
            
                            case PlatformSystemIcon.Asterisk:
                                icon = System.Drawing.SystemIcons.Asterisk;
                                break;
            
                            case PlatformSystemIcon.Error:
                                icon = System.Drawing.SystemIcons.Error;
                                break;
            
                            case PlatformSystemIcon.Exclamation:
                                icon = System.Drawing.SystemIcons.Exclamation;
                                break;
            
                            case PlatformSystemIcon.Hand:
                                icon = System.Drawing.SystemIcons.Hand;
                                break;
            
                            case PlatformSystemIcon.Information:
                                icon = System.Drawing.SystemIcons.Information;
                                break;
            
                            case PlatformSystemIcon.Question:
                                icon = System.Drawing.SystemIcons.Question;
                                break;
            
                            case PlatformSystemIcon.Shield:
                                icon = System.Drawing.SystemIcons.Shield;
                                break;
            
                            case PlatformSystemIcon.Warning:
                                icon = System.Drawing.SystemIcons.Warning;
                                break;
            
                            case PlatformSystemIcon.WinLogo:
                                icon = System.Drawing.SystemIcons.WinLogo;
                                break;
                        }
            
                        if (icon != null)
                        {
                            return Bitmap.FromNativeIcon(icon);
                        }
                        else
                        {
                            return null;
                        }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private static void OnOutOfMemoryEncountered()
        {
            /*
            var handler = Bitmap.OutOfMemoryEncountered;

            if (handler != null)
            {
                handler(null);
            }
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromImageDisabled(Image image, Color background)
        {
            /*            System.Diagnostics.Debug.Assert(image != null);
            
                        int r = (int)(background.R * 255.5);
                        int g = (int)(background.G * 255.5);
                        int b = (int)(background.B * 255.5);
            
                        ImageSeed seed = new ImageSeed(r, g, b, image.UniqueId);
            
                        lock (Bitmap.disabledImages)
                        {
                            if (Bitmap.disabledImages.Contains(seed))
                            {
                                return Bitmap.disabledImages[seed] as Bitmap;
                            }
            
                            System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);
            
                            System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
                            System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap(
                                srcBitmap.Width,
                                srcBitmap.Height
                            );
            
                            Platform.ImageDisabler.Paint(srcBitmap, dstBitmap, color);
            
                            Bitmap bitmap = new Bitmap();
            
                            bitmap.bitmap = dstBitmap;
                            bitmap.bitmapDx = dstBitmap.Width;
                            bitmap.bitmapDy = dstBitmap.Height;
                            bitmap.size = image.Size;
                            bitmap.origin = image.Origin;
                            bitmap.isOriginDefined = image.IsOriginDefined;
            
                            Bitmap.disabledImages[seed] = bitmap;
            
                            return bitmap;
                        }
            */
            //  return null;
            throw new System.NotImplementedException();
        }

        public static Bitmap CopyImage(Image image)
        {
            /*            if (image == null)
                        {
                            return null;
                        }
            
                        System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
                        System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap(
                            srcBitmap.Width,
                            srcBitmap.Height
                        );
            
                        double dpiX = srcBitmap.HorizontalResolution;
                        double dpiY = srcBitmap.VerticalResolution;
            
                        srcBitmap.SetResolution(dstBitmap.HorizontalResolution, dstBitmap.VerticalResolution);
            
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(dstBitmap))
                        {
                            graphics.DrawImageUnscaled(srcBitmap, 0, 0, srcBitmap.Width, srcBitmap.Height);
                        }
            
                        Bitmap bitmap = new Bitmap();
            
                        bitmap.bitmap = dstBitmap;
                        bitmap.bitmapDx = dstBitmap.Width;
                        bitmap.bitmapDy = dstBitmap.Height;
                        bitmap.size = image.Size;
                        bitmap.origin = image.Origin;
                        bitmap.isOriginDefined = image.IsOriginDefined;
            
                        bitmap.dpiX = dpiX;
                        bitmap.dpiY = dpiY;
            
                        return bitmap;
            */
            //  return null;
            throw new System.NotImplementedException();
        }

        public static Bitmap FromLargerImage(Image image, Rectangle clip)
        {
            /*
            Bitmap bitmap = Bitmap.FromLargerImage(image, clip, null);
            return bitmap;
            */
            throw new System.NotImplementedException();
        }

        public static Bitmap FromLargerImage(Image image, Rectangle clip, Point? origin)
        {
            /*            if (image == null)
                        {
                            return null;
                        }
            
                        System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
            
                        int dx = (int)(clip.Width + 0.5);
                        int dy = (int)(clip.Height + 0.5);
                        int x = (int)(clip.Left);
                        int y = (int)(clip.Bottom);
                        int yy = srcBitmap.Height - dy - y;
            
                        System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap(dx, dy);
            
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(dstBitmap))
                        {
                            graphics.DrawImage(
                                srcBitmap,
                                0,
                                0,
                                new System.Drawing.Rectangle(x, yy, dx, dy),
                                System.Drawing.GraphicsUnit.Pixel
                            );
                        }
            
                        Bitmap bitmap = new Bitmap();
            
                        double sx = image.Width / srcBitmap.Width;
                        double sy = image.Height / srcBitmap.Height;
            
                        bitmap.bitmap = dstBitmap;
                        bitmap.bitmapDx = dstBitmap.Width;
                        bitmap.bitmapDy = dstBitmap.Height;
                        bitmap.size = new Size(sx * dx, sy * dy);
                        bitmap.origin = origin;
                        bitmap.isOriginDefined = true;
            
                        return bitmap;
            */
            //  return null;
            throw new System.NotImplementedException();
        }

        public static string[] GetFilenameExtensions(ImageFormat format)
        {
            /*            System.Drawing.Imaging.ImageCodecInfo info = Bitmap.GetCodecInfo(format);
            
                        if (info != null)
                        {
                            return info.FilenameExtension.Split(';');
                        }
            */
            //return null;
            throw new System.NotImplementedException();
        }
        #endregion NotImplemented

        public abstract override void Dispose();
    }
}

//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;

namespace Epsitec.Common.Drawing.Platform
{
    /// <summary>
    /// NativeBitmap represents a bitmap that we can read/write to a file
    /// This implementation is using ImageMagick for that purpose
    /// </summary>
    public sealed class NativeBitmap : Bitmap
    {
        internal NativeBitmap(string source)
        {
            this.magicImage = new ImageMagick.MagickImage(source);
        }

        internal NativeBitmap(byte[] buffer)
        {
            this.magicImage = new ImageMagick.MagickImage(buffer);
        }

        public override int Stride
        {
            get
            {
                // divide by 8 to get the stride in byte
                return this.magicImage.Width * this.BitsPerPixel / 8;
            }
        }

        public int BitsPerPixel
        {
            get { return this.magicImage.Depth * this.magicImage.ChannelCount; }
        }

        public override Size Size
        {
            get { return new Size(this.magicImage.Width, this.magicImage.Height); }
        }

        public override int PixelWidth
        {
            get { return this.magicImage.BaseWidth; }
        }

        public override int PixelHeight
        {
            get { return this.magicImage.BaseHeight; }
        }

        public bool IsTransparent
        {
            /*            get
                        {
                            if (this.bitmapSource == null)
                            {
                                return false;
                            }
                            else
                            {
                                var format = this.bitmapSource.Format;
            
                                return NativeBitmap.GetRgbAlphaPixelFormats().Any(x => x == format);
                            }
                        }
            */
            //  get { return false; }
            get { throw new System.NotImplementedException(); }
        }

        public BitmapFileFormat FileFormat
        {
            get { throw new System.NotImplementedException(); }
            //get { return this.fileFormat; }
        }

        public int MemorySize
        {
            /*            get
                        {
                            return this.bitmapSource == null ? 0 : this.bitmapSource.PixelHeight * this.Pitch;
                        }
            */
            //  get { return 0; }
            get { throw new System.NotImplementedException(); }
        }

        public int Pitch
        {
            /*            get
                        {
                            if (this.bitmapSource == null)
                            {
                                return 0;
                            }
            
                            int bytesPerPixel = (this.bitmapSource.Format.BitsPerPixel + 7) / 8;
                            return (this.bitmapSource.PixelWidth * bytesPerPixel + 3) & 0x0ffffffc;
                        }
            */
            //get { return 0; }
            get { throw new System.NotImplementedException(); }
        }

        public string Information { get; set; }

        public bool IsValid
        {
            //get { return this.bitmapSource != null; }
            // get { return true; }
            get { throw new System.NotImplementedException(); }
        }

        public BitmapColorType ColorType
        {
            /*            get
                        {
                            if (this.colorType.HasValue)
                            {
                                return this.colorType.Value;
                            }
                            var format = this.bitmapSource.Format;
            
                            if (NativeBitmap.GetRgbAlphaPixelFormats().Any(x => x == format))
                            {
                                return BitmapColorType.RgbAlpha;
                            }
                            if (NativeBitmap.GetGrayscalePixelFormats().Any(x => x == format))
                            {
                                return BitmapColorType.MinIsBlack;
                            }
                            if (NativeBitmap.GetRgbPixelFormats().Any(x => x == format))
                            {
                                return BitmapColorType.Rgb;
                            }
                            if (NativeBitmap.GetIndexedPixelFormats().Any(x => x == format))
                            {
                                return BitmapColorType.Palette;
                            }
                            if (NativeBitmap.GetCmykPixelFormats().Any(x => x == format))
                            {
                                return BitmapColorType.Cmyk;
                            }
            
                            try
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    string.Format(
                                        "ColorType: Format {0} could not be mapped to any type -- {1} bpp / Masks={2}",
                                        format.ToString(),
                                        format.BitsPerPixel,
                                        string.Join(
                                            "-",
                                            format
                                                .Masks.SelectMany(x => x.Mask)
                                                .Select(x => x.ToString("x"))
                                                .ToArray()
                                        )
                                    )
                                );
                            }
                            catch (System.Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    string.Format("ColorType: Exception {0}", ex.Message)
                                );
                            }
            
                            return BitmapColorType.Unsupported;
                        }
            */
            //get { return BitmapColorType.Unsupported; }
            get { throw new System.NotImplementedException(); }
        }

        public long ByteCount
        {
            //get { return this.Pitch * (long)this.Height; }
            get { throw new System.NotImplementedException(); }
        }

        public double DpiX
        {
            //get { return this.bitmapSource == null ? 72 : this.bitmapSource.DpiX; }
            //get { return 72; }
            get { throw new System.NotImplementedException(); }
        }

        public double DpiY
        {
            //get { return this.bitmapSource == null ? 72 : this.bitmapSource.DpiY; }
            //get { return 72; }
            get { throw new System.NotImplementedException(); }
        }

        public override byte[] GetPixelBuffer()
        {
            // In the antigrain backend, we use the BGRA format for pixels, so we use the same format here
            // See the definition in AggUI/aggcpp/pixelfmt.h
            if (this.pixelBuffer == null)
            {
                this.pixelBuffer = this
                    .magicImage.GetPixels()
                    .ToByteArray(ImageMagick.PixelMapping.BGRA);
            }
            return this.pixelBuffer;
        }

        #region IDisposable Members

        public override void Dispose()
        {
            if (this.magicImage == null)
            {
                return;
            }
            this.magicImage.Dispose();
            this.magicImage = null;
        }

        #endregion

        public NativeBitmap ConvertToPremultipliedArgb32()
        {
            /*
            FormatConvertedBitmap converter = new FormatConvertedBitmap();
            converter.BeginInit();
            converter.Source = this.bitmapSource;
            converter.DestinationFormat = System.Windows.Media.PixelFormats.Pbgra32;
            converter.EndInit();

            return new NativeBitmap(converter, this.fileFormat);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap ConvertToRgb24()
        {
            /*            FormatConvertedBitmap converter = new FormatConvertedBitmap();
                        converter.BeginInit();
                        converter.Source = this.bitmapSource;
                        converter.DestinationFormat = System.Windows.Media.PixelFormats.Bgr24;
                        converter.EndInit();

                        return new NativeBitmap(converter, this.fileFormat);*/
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap ConvertToGrayscale()
        {
            /*
            FormatConvertedBitmap bitmap = new FormatConvertedBitmap();

            bitmap.BeginInit();
            bitmap.Source = this.bitmapSource;
            bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
            bitmap.EndInit();

            return new NativeBitmap(bitmap, this.fileFormat, BitmapColorType.MinIsBlack);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap ConvertToArgb32()
        {
            /*
            FormatConvertedBitmap bitmap = new FormatConvertedBitmap();

            bitmap.BeginInit();
            bitmap.Source = this.bitmapSource;
            bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            bitmap.EndInit();

            return new NativeBitmap(bitmap, this.fileFormat);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap ConvertToCmyk32()
        {
            /*
            FormatConvertedBitmap bitmap = new FormatConvertedBitmap();

            bitmap.BeginInit();
            bitmap.Source = this.bitmapSource;
            bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Cmyk32;
            bitmap.EndInit();

            return new NativeBitmap(bitmap, this.fileFormat);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap GetChannel(BitmapColorChannel channel)
        {
            switch (channel)
            {
                case BitmapColorChannel.Alpha:
                    return this.GetChannelAlpha();

                case BitmapColorChannel.Black:
                case BitmapColorChannel.Cyan:
                case BitmapColorChannel.Magenta:
                case BitmapColorChannel.Yellow:
                    return this.GetChannelCmyk(channel);

                case BitmapColorChannel.Grayscale:
                    return this.GetChannelGrayscale();

                case BitmapColorChannel.Red:
                case BitmapColorChannel.Green:
                case BitmapColorChannel.Blue:
                    return this.GetChannelRgb(channel);

                default:
                    throw new System.InvalidOperationException(
                        "Unsupported color channel specified"
                    );
            }
        }

        public byte[] GetRawImageDataInCompactFormFor8BitImage()
        {
            /*
            if (this.BitsPerPixel != 8)
            {
                throw new System.InvalidOperationException(
                    "BitsPerPixel != 8 : cannot get 8-bit image byte array"
                );
            }

            byte[] buffer = new byte[this.Height * this.Width];

            this.bitmapSource.CopyPixels(buffer, this.Width, 0);

            return buffer;
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap Crop(int x, int y, int dx, int dy)
        {
            /*
            if ((dx > 0) && (dy > 0))
            {
                try
                {
                    CroppedBitmap cropped = new CroppedBitmap(
                        this.bitmapSource,
                        new System.Windows.Int32Rect(x, y, dx, dy)
                    );
                    return new NativeBitmap(cropped, this.fileFormat);
                }
                catch { }
            }

            return NativeBitmap.CreateEmpty();
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public static NativeBitmap CreateEmpty()
        {
            byte[] data = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            return NativeBitmap.CreateFromPremultipliedArgb32(data, 4, 1, 1);
        }

        public byte[] SaveToMemory(
            BitmapFileType fileType,
            int quality = 80,
            TiffCompressionOption tiffCompressionOption = TiffCompressionOption.Lzw
        )
        {
            return this.SaveToMemory(
                new BitmapFileFormat()
                {
                    Type = fileType,
                    Quality = quality,
                    TiffCompression = tiffCompressionOption
                }
            );
        }

        public byte[] SaveToMemory(BitmapFileFormat fileFormat)
        {
            var stream = new System.IO.MemoryStream();

            switch (fileFormat.Type)
            {
                case BitmapFileType.Jpeg:
                    this.SaveToMemoryJpeg(fileFormat, stream);
                    break;

                case BitmapFileType.Png:
                    this.SaveToMemoryPng(fileFormat, stream);
                    break;

                case BitmapFileType.Tiff:
                    this.SaveToMemoryTiff(fileFormat, stream);
                    break;

                case BitmapFileType.Bmp:
                    this.SaveToMemoryBmp(fileFormat, stream);
                    break;

                case BitmapFileType.Gif:
                    this.SaveToMemoryGif(fileFormat, stream);
                    break;

                default:
                    System.Diagnostics.Debug.Fail("Unsupported format : " + fileFormat.Type);
                    break;
            }

            return stream.ToArray();
        }

        public NativeBitmap Rescale(int dx, int dy)
        {
            /*
            double scaleX = ((double)dx) / this.Width;
            double scaleY = ((double)dy) / this.Height;

            var transform = new System.Windows.Media.ScaleTransform(scaleX, scaleY);
            var bitmap = new TransformedBitmap(this.bitmapSource, transform);

            return new NativeBitmap(bitmap, this.fileFormat);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public NativeBitmap MakeThumbnail(int size)
        {
            /*
            double scaleX;
            double scaleY;

            if (this.Width > this.Height)
            {
                scaleX = ((double)size) / this.Width;
                scaleY = scaleX;
            }
            else
            {
                scaleY = ((double)size) / this.Height;
                scaleX = scaleY;
            }

            if ((scaleX >= 1) && (scaleY >= 1))
            {
                //	If the thumbnail is larger than the original image, then simply
                //	use the original image instead:

                return this;
            }

            var transform = new System.Windows.Media.ScaleTransform(scaleX, scaleY);
            var bitmap = new TransformedBitmap(this.bitmapSource, transform);

            return new NativeBitmap(bitmap, this.fileFormat);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        public static NativeBitmap Load(string path)
        {
            path = System.IO.Path.GetFullPath(path);

            return new NativeBitmap(path);
        }

        public static NativeBitmap Load(byte[] buffer, string path = null)
        {
            if (path != null)
            {
                return NativeBitmap.Load(path);
            }
            return new NativeBitmap(buffer);
        }

        public override string ToString()
        {
            return string.Format(
                "ColorType={0} DpiX={1} DpiY={2} Size={3}x{4} Info={5} Bpp={6} MemorySize={7}",
                this.ColorType,
                this.DpiX,
                this.DpiY,
                this.Width,
                this.Height,
                this.Information,
                this.BitsPerPixel,
                this.MemorySize
            );
        }

        public static NativeBitmap FromData(byte[] data)
        {
            return new NativeBitmap(data);
        }

        public static NativeBitmap FromFile(string filename)
        {
            return new NativeBitmap(filename);
        }

        public static NativeBitmap CreateFromPremultipliedArgb32(
            byte[] imageBytes,
            int pitch,
            int dx,
            int dy
        )
        {
            /*
            WriteableBitmap bitmap = new WriteableBitmap(
                dx,
                dy,
                72,
                72,
                PixelFormats.Pbgra32,
                null
            );
            bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, dx, dy), imageBytes, pitch, 0);

            return new NativeBitmap(bitmap, colorType: BitmapColorType.RgbAlpha);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private void SaveToMemoryJpeg(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
        {
            /*
            var encoder = new JpegBitmapEncoder();

            encoder.QualityLevel = fileFormat.Quality;
            encoder.Frames.Add(BitmapFrame.Create(this.bitmapSource));
            encoder.Save(stream);
            */
            throw new System.NotImplementedException();
        }

        private void SaveToMemoryPng(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
        {
            /*            var encoder = new PngBitmapEncoder();
            
                        encoder.Frames.Add(BitmapFrame.Create(this.bitmapSource));
                        encoder.Save(stream);
            */
            throw new System.NotImplementedException();
        }

        private void SaveToMemoryTiff(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
        {
            /*            if ((this.ColorType != Platform.BitmapColorType.Cmyk) && (fileFormat.TiffCmyk))
                        {
                            this.ConvertToCmyk32().SaveToMemoryTiff(fileFormat, stream);
                        }
                        else
                        {
                            var encoder = new TiffBitmapEncoder();
            
                            encoder.Compression = (System.Windows.Media.Imaging.TiffCompressOption)
                                (int)fileFormat.TiffCompression;
                            encoder.Frames.Add(BitmapFrame.Create(this.bitmapSource));
                            encoder.Save(stream);
                        }
            */
            throw new System.NotImplementedException();
        }

        private void SaveToMemoryBmp(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
        {
            /*            var encoder = new BmpBitmapEncoder();
            
                        encoder.Frames.Add(BitmapFrame.Create(this.bitmapSource));
                        encoder.Save(stream);
            */
        }

        private void SaveToMemoryGif(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
        {
            /*            var encoder = new GifBitmapEncoder();
            
                        encoder.Frames.Add(BitmapFrame.Create(this.bitmapSource));
                        encoder.Save(stream);
            */
            throw new System.NotImplementedException();
        }

        private static BitmapFileFormat GuessFileFormat(string path)
        {
            BitmapFileFormat fileFormat = new BitmapFileFormat();

            if (string.IsNullOrEmpty(path))
            {
                return fileFormat;
            }

            string ext = System.IO.Path.GetExtension(path).ToLowerInvariant();

            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    fileFormat.Type = BitmapFileType.Jpeg;
                    fileFormat.Quality = 80;
                    break;

                case ".png":
                    fileFormat.Type = BitmapFileType.Png;
                    break;

                case ".tif":
                case ".tiff":
                    fileFormat.Type = BitmapFileType.Tiff;
                    break;

                case ".bmp":
                    fileFormat.Type = BitmapFileType.Bmp;
                    break;
            }
            return fileFormat;
        }

        /*        private static IEnumerable<PixelFormat> GetCmykPixelFormats()
                {
                    yield return PixelFormats.Cmyk32;
                }
        
                private static IEnumerable<PixelFormat> GetRgbAlphaPixelFormats()
                {
                    yield return PixelFormats.Bgra32;
        
                    yield return PixelFormats.Pbgra32;
                    yield return PixelFormats.Prgba128Float;
                    yield return PixelFormats.Prgba64;
        
                    yield return PixelFormats.Rgba128Float;
                    yield return PixelFormats.Rgba64;
                }
        
                private static IEnumerable<PixelFormat> GetRgbPixelFormats()
                {
                    yield return PixelFormats.Bgr101010;
                    yield return PixelFormats.Bgr24;
                    yield return PixelFormats.Bgr32;
                    yield return PixelFormats.Bgr555;
                    yield return PixelFormats.Bgr565;
        
                    yield return PixelFormats.BlackWhite;
                    yield return PixelFormats.Gray16;
                    yield return PixelFormats.Gray2;
                    yield return PixelFormats.Gray32Float;
                    yield return PixelFormats.Gray4;
                    yield return PixelFormats.Gray8;
        
                    yield return PixelFormats.Indexed1;
                    yield return PixelFormats.Indexed2;
                    yield return PixelFormats.Indexed4;
                    yield return PixelFormats.Indexed8;
        
                    yield return PixelFormats.Rgb128Float;
                    yield return PixelFormats.Rgb24;
                    yield return PixelFormats.Rgb48;
                }
        
                private static IEnumerable<PixelFormat> GetGrayscalePixelFormats()
                {
                    yield return PixelFormats.BlackWhite;
                    yield return PixelFormats.Gray16;
                    yield return PixelFormats.Gray2;
                    yield return PixelFormats.Gray32Float;
                    yield return PixelFormats.Gray4;
                    yield return PixelFormats.Gray8;
                }
        
                private static IEnumerable<PixelFormat> GetIndexedPixelFormats()
                {
                    yield return PixelFormats.Indexed1;
                    yield return PixelFormats.Indexed2;
                    yield return PixelFormats.Indexed4;
                    yield return PixelFormats.Indexed8;
                }
        */
        private NativeBitmap GetChannelAlpha()
        {
            /*
            if (this.bitmapSource.Format == PixelFormats.Bgra32)
            {
                return this.ExtractChannelFrom32BitSource(offset: 3);
            }
            else
            {
                return this.ConvertToArgb32().GetChannelAlpha();
            }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private NativeBitmap GetChannelGrayscale()
        {
            /*
            if (this.bitmapSource.Format == PixelFormats.Gray8)
            {
                return this;
            }
            else
            {
                return this.ConvertToGrayscale();
            }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private NativeBitmap GetChannelCmyk(BitmapColorChannel channel)
        {
            /*
            if (this.bitmapSource.Format == PixelFormats.Cmyk32)
            {
                switch (channel)
                {
                    case BitmapColorChannel.Cyan:
                        return this.ExtractChannelFrom32BitSource(offset: 0);
                    case BitmapColorChannel.Magenta:
                        return this.ExtractChannelFrom32BitSource(offset: 1);
                    case BitmapColorChannel.Yellow:
                        return this.ExtractChannelFrom32BitSource(offset: 2);
                    case BitmapColorChannel.Black:
                        return this.ExtractChannelFrom32BitSource(offset: 3);
                }

                throw new System.InvalidOperationException();
            }
            else
            {
                return this.ConvertToCmyk32().GetChannelCmyk(channel);
            }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private NativeBitmap GetChannelRgb(BitmapColorChannel channel)
        {
            /*
            if (
                (this.bitmapSource.Format == PixelFormats.Bgra32)
                || (this.bitmapSource.Format == PixelFormats.Bgr32)
            )
            {
                switch (channel)
                {
                    case BitmapColorChannel.Blue:
                        return this.ExtractChannelFrom32BitSource(offset: 0);
                    case BitmapColorChannel.Green:
                        return this.ExtractChannelFrom32BitSource(offset: 1);
                    case BitmapColorChannel.Red:
                        return this.ExtractChannelFrom32BitSource(offset: 2);
                }

                throw new System.InvalidOperationException();
            }
            else
            {
                return this.ConvertToArgb32().GetChannelRgb(channel);
            }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private NativeBitmap ExtractChannelFrom32BitSource(int offset)
        {
            /*
            byte[] fullImage = new byte[this.Height * this.Pitch];
            this.bitmapSource.CopyPixels(fullImage, this.Pitch, 0);

            return NativeBitmap.ExtractChannelFrom32BitSource(
                fullImage,
                this.Width,
                this.Height,
                this.Pitch,
                offset
            );
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private static NativeBitmap ExtractChannelFrom32BitSource(
            byte[] rgb32Pixels,
            int dx,
            int dy,
            int pitch,
            int byteOffset
        )
        {
            /*
            byte[] monoPixels = new byte[dx * dy];

            int monoIndex = 0;

            for (int y = 0; y < dy; y++)
            {
                int rgb32Index = y * pitch + byteOffset;

                for (int x = 0; x < dx; x++)
                {
                    monoPixels[monoIndex] = rgb32Pixels[rgb32Index];

                    monoIndex += 1;
                    rgb32Index += 4;
                }
            }

            WriteableBitmap bitmap = new WriteableBitmap(dx, dy, 72, 72, PixelFormats.Gray8, null);
            bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, dx, dy), monoPixels, dx, 0);

            return new NativeBitmap(bitmap, colorType: BitmapColorType.MinIsBlack);
            */
            //return null;
            throw new System.NotImplementedException();
        }

        internal byte[] GetScanline(int y)
        {
            /*
            byte[] scanline = new byte[this.Pitch];
            this.bitmapSource.CopyPixels(
                new System.Windows.Int32Rect(0, y, this.Width, 1),
                scanline,
                this.Pitch,
                0
            );
            return scanline;
            */
            //return null;
            throw new System.NotImplementedException();
        }

        internal NativeBitmap Palettize(IEnumerable<Color> palette)
        {
            /*
            return new NativeBitmap(
                new FormatConvertedBitmap(
                    this.bitmapSource,
                    PixelFormats.Indexed8,
                    new BitmapPalette(palette.ToList()),
                    0.5
                ),
                colorType: BitmapColorType.Palette
            );
            */
            //return null;
            throw new System.NotImplementedException();
        }

        internal NativeBitmap Quantize(int maxColors, int maxBitsPerPixel)
        {
            /*
            if (this.BitsPerPixel != 32)
            {
                return this.ConvertToPremultipliedArgb32().Quantize(maxColors, maxBitsPerPixel);
            }
            else
            {
                return new NativeBitmap(
                    OctreeQuantizer.Quantize(this.bitmapSource, maxColors, maxBitsPerPixel),
                    colorType: BitmapColorType.Palette
                );
            }
            */
            //return null;
            throw new System.NotImplementedException();
        }

        internal IEnumerable<Color> GetPalette()
        {
            /*
            return this.bitmapSource.Palette.Colors;
            */
            //return null;
            throw new System.NotImplementedException();
        }

        private ImageMagick.MagickImage magicImage;
        private byte[] pixelBuffer;
    }
}

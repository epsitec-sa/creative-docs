//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Epsitec.Common.Drawing.Platform
{
	using Color=System.Windows.Media.Color;

	public sealed class NativeBitmap : System.IDisposable
	{
		internal NativeBitmap(BitmapSource bitmapSource, BitmapFileFormat fileFormat = null, BitmapColorType? colorType = null)
		{
			//	We really must create a copy of the bitmap source, and not just a
			//	CachedBitmap, but really a WriteableBitmap, or else WPF will keep
			//	alive the full bitmap chain, which rapidly leads to memory errors.

			BitmapSource bitmapCopy = null;


			for (int attempt = 0; ; attempt++)
			{
				try
				{
					bitmapCopy = new WriteableBitmap (bitmapSource);
					break;
				}
				catch (System.OutOfMemoryException)
				{
					//	There is no memory available for System.Windows.Media.Imaging to operate
					//	properly.

					if (attempt >= 5)
					{
						//	No need to try more; the situation is really catastrophic and nothing
						//	we can do will help.
						throw;
					}
					
					var handler = NativeBitmap.outOfMemoryHandler;

					if (handler == null)
					{
						break;
					}
					
					handler ();
				}
			}

			this.bitmapSource = bitmapCopy;
			this.fileFormat   = fileFormat;
			this.colorType    = colorType;

			if (this.bitmapSource.CanFreeze)
			{
				this.bitmapSource.Freeze ();
			}
		}

		
		public int								BitsPerPixel
		{
			get
			{
				return this.bitmapSource == null ? 0 : this.bitmapSource.Format.BitsPerPixel;
			}
		}

		public int								Width
		{
			get
			{
				return this.bitmapSource == null ? 0 : this.bitmapSource.PixelWidth;
			}
		}

		public int								Height
		{
			get
			{
				return this.bitmapSource == null ? 0 : this.bitmapSource.PixelHeight;
			}
		}

		public bool								IsTransparent
		{
			get
			{
				if (this.bitmapSource == null)
				{
					return false;
				}
				else
				{
					var format = this.bitmapSource.Format;

					return NativeBitmap.GetRgbAlphaPixelFormats ().Any (x => x == format);
				}
			}
		}

		public BitmapFileFormat					FileFormat
		{
			get
			{
				return this.fileFormat;
			}
		}

		public int								MemorySize
		{
			get
			{
				return this.bitmapSource == null ? 0 : this.bitmapSource.PixelHeight * this.Pitch;
			}
		}

		public int								Pitch
		{
			get
			{
				if (this.bitmapSource == null)
                {
					return 0;
                }

				int bytesPerPixel = (this.bitmapSource.Format.BitsPerPixel + 7) / 8;
				return (this.bitmapSource.PixelWidth * bytesPerPixel + 3) & 0x0ffffffc;
			}
		}

		public string							Information
		{
			get;
			set;
		}
		
		public bool								IsValid
		{
			get
			{
				return this.bitmapSource != null;
			}
		}

		public BitmapColorType					ColorType
		{
			get
			{
				if (this.colorType.HasValue)
				{
					return this.colorType.Value;
				}
				var format = this.bitmapSource.Format;

				if (NativeBitmap.GetRgbAlphaPixelFormats ().Any (x => x == format))
				{
					return BitmapColorType.RgbAlpha;
				}
				if (NativeBitmap.GetGrayscalePixelFormats ().Any (x => x == format))
				{
					return BitmapColorType.MinIsBlack;
				}
				if (NativeBitmap.GetRgbPixelFormats ().Any (x => x == format))
				{
					return BitmapColorType.Rgb;
				}
				if (NativeBitmap.GetIndexedPixelFormats ().Any (x => x == format))
				{
					return BitmapColorType.Palette;
				}
				if (NativeBitmap.GetCmykPixelFormats ().Any (x => x == format))
				{
					return BitmapColorType.Cmyk;
				}

				return BitmapColorType.Unsupported;
			}
		}

		public long								ByteCount
		{
			get
			{
				return this.Pitch * (long)this.Height;
			}
		}

		public double							DpiX
		{
			get
			{
				return this.bitmapSource == null ? 72 : this.bitmapSource.DpiX;
			}
		}

		public double							DpiY
		{
			get
			{
				return this.bitmapSource == null ? 72 : this.bitmapSource.DpiY;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.bitmapSource = null;
		}

		#endregion

		public static void SetOutOfMemoryHandler(System.Action callback)
		{
			NativeBitmap.outOfMemoryHandler = callback;
		}

		public NativeBitmap ConvertToPremultipliedArgb32()
		{
			FormatConvertedBitmap converter = new FormatConvertedBitmap ();
			converter.BeginInit ();
			converter.Source = this.bitmapSource;
			converter.DestinationFormat = System.Windows.Media.PixelFormats.Pbgra32;
			converter.EndInit ();

			return new NativeBitmap (converter, this.fileFormat);
		}

		public NativeBitmap ConvertToRgb24()
		{
			FormatConvertedBitmap converter = new FormatConvertedBitmap ();
			converter.BeginInit ();
			converter.Source = this.bitmapSource;
			converter.DestinationFormat = System.Windows.Media.PixelFormats.Bgr24;
			converter.EndInit ();

			return new NativeBitmap (converter, this.fileFormat);
		}

		public NativeBitmap ConvertToGrayscale()
		{
			FormatConvertedBitmap bitmap = new FormatConvertedBitmap ();

			bitmap.BeginInit ();
			bitmap.Source = this.bitmapSource;
			bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
			bitmap.EndInit ();

			return new NativeBitmap (bitmap, this.fileFormat, BitmapColorType.MinIsBlack);
		}

		public NativeBitmap ConvertToArgb32()
		{
			FormatConvertedBitmap bitmap = new FormatConvertedBitmap ();

			bitmap.BeginInit ();
			bitmap.Source = this.bitmapSource;
			bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
			bitmap.EndInit ();

			return new NativeBitmap (bitmap, this.fileFormat);
		}

		public NativeBitmap ConvertToCmyk32()
		{
			FormatConvertedBitmap bitmap = new FormatConvertedBitmap ();

			bitmap.BeginInit ();
			bitmap.Source = this.bitmapSource;
			bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Cmyk32;
			bitmap.EndInit ();

			return new NativeBitmap (bitmap, this.fileFormat);
		}

		public NativeBitmap GetChannel(BitmapColorChannel channel)
		{
			switch (channel)
			{
				case BitmapColorChannel.Alpha:
					return this.GetChannelAlpha ();

				case BitmapColorChannel.Black:
				case BitmapColorChannel.Cyan:
				case BitmapColorChannel.Magenta:
				case BitmapColorChannel.Yellow:
					return this.GetChannelCmyk (channel);

				case BitmapColorChannel.Grayscale:
					return this.GetChannelGrayscale ();

				case BitmapColorChannel.Red:
				case BitmapColorChannel.Green:
				case BitmapColorChannel.Blue:
					return this.GetChannelRgb (channel);

				default:
					throw new System.InvalidOperationException ("Unsupported color channel specified");
			}
		}

		public void CopyPixelsToBuffer(System.IntPtr bufferMemory, int bufferSize, int pitch)
		{
			System.Windows.Int32Rect sourceRect = System.Windows.Int32Rect.Empty;

			var temp = new TransformedBitmap ();
			temp.BeginInit ();
			temp.Source = this.bitmapSource;
			temp.Transform = new System.Windows.Media.ScaleTransform (1, -1);
			temp.EndInit ();

			temp.CopyPixels (sourceRect, bufferMemory, bufferSize, pitch);
		}

		public byte[] GetRawImageDataInCompactFormFor8BitImage()
		{
			if (this.BitsPerPixel != 8)
			{
				throw new System.InvalidOperationException ("BitsPerPixel != 8 : cannot get 8-bit image byte array");
			}

			byte[] buffer = new byte[this.Height * this.Width];

			this.bitmapSource.CopyPixels (buffer, this.Width, 0);

			return buffer;
		}

		public NativeBitmap Crop(int x, int y, int dx, int dy)
		{
			if ((dx > 0) &&
				(dy > 0))
			{
				try
				{
					CroppedBitmap cropped = new CroppedBitmap (this.bitmapSource, new System.Windows.Int32Rect (x, y, dx, dy));
					return new NativeBitmap (cropped, this.fileFormat);
				}
				catch
				{
				}
			}

			return NativeBitmap.CreateEmpty ();
		}

		public static NativeBitmap CreateEmpty()
		{
			byte[] data = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
			return NativeBitmap.CreateFromPremultipliedArgb32 (data, 4, 1, 1);
		}

		public byte[] SaveToMemory(BitmapFileType fileType, int quality = 80, TiffCompressionOption tiffCompressionOption = TiffCompressionOption.Lzw)
		{
			return this.SaveToMemory (new BitmapFileFormat () { Type = fileType, Quality = quality, TiffCompression = tiffCompressionOption });
		}

		public byte[] SaveToMemory(BitmapFileFormat fileFormat)
		{
			var stream = new System.IO.MemoryStream ();

			switch (fileFormat.Type)
			{
				case BitmapFileType.Jpeg:
					this.SaveToMemoryJpeg (fileFormat, stream);
					break;

				case BitmapFileType.Png:
					this.SaveToMemoryPng (fileFormat, stream);
					break;

				case BitmapFileType.Tiff:
					this.SaveToMemoryTiff (fileFormat, stream);
					break;

				case BitmapFileType.Bmp:
					this.SaveToMemoryBmp (fileFormat, stream);
					break;

				case BitmapFileType.Gif:
					this.SaveToMemoryGif (fileFormat, stream);
					break;
				
				default:
					System.Diagnostics.Debug.Fail ("Unsupported format : " + fileFormat.Type);
					break;
			}

			return stream.ToArray ();
		}

		public NativeBitmap Rescale(int dx, int dy)
		{
			double scaleX = ((double) dx) / this.Width;
			double scaleY = ((double) dy) / this.Height;

			var transform = new System.Windows.Media.ScaleTransform (scaleX, scaleY);
			var bitmap = new TransformedBitmap (this.bitmapSource, transform);

			return new NativeBitmap (bitmap, this.fileFormat);
		}

		public NativeBitmap MakeThumbnail(int size)
		{
			double scaleX;
			double scaleY;

			if (this.Width > this.Height)
			{
				scaleX = ((double) size) / this.Width;
				scaleY = scaleX;
			}
			else
			{
				scaleY = ((double) size) / this.Height;
				scaleX = scaleY;
			}

			if ((scaleX >= 1) &&
				(scaleY >= 1))
			{
				//	If the thumbnail is larger than the original image, then simply
				//	use the original image instead:

				return this;
			}

			var transform = new System.Windows.Media.ScaleTransform (scaleX, scaleY);
			var bitmap = new TransformedBitmap (this.bitmapSource, transform);

			return new NativeBitmap (bitmap, this.fileFormat);
		}

		
		public static NativeBitmap Load(string path)
		{
			path = System.IO.Path.GetFullPath (path);
			
			return NativeBitmap.Load (System.IO.File.ReadAllBytes (path), path);
		}
		
		public static NativeBitmap Load(byte[] buffer, string path = null)
		{
			if (buffer == null)
			{
				return null;
			}

			try
			{
				using (var imageStreamSource = new System.IO.MemoryStream (buffer))
				{
					return NativeBitmap.LoadFromMemoryStream (path, imageStreamSource);
				}
			}
			catch
			{
				try
				{
					System.Drawing.Image drawingImage;

					using (var imageStreamSource = new System.IO.MemoryStream (buffer))
					{
						drawingImage = System.Drawing.Image.FromStream (imageStreamSource);

						using (var memoryStream = new System.IO.MemoryStream ())
						{
							drawingImage.Save (memoryStream, System.Drawing.Imaging.ImageFormat.Png);
							drawingImage.Dispose ();

							memoryStream.Seek (0, System.IO.SeekOrigin.Begin);

							return NativeBitmap.LoadFromMemoryStream (path, memoryStream);
						}
					}
				}
				catch
				{
					return null;
				}
			}
		}

		private static NativeBitmap LoadFromMemoryStream(string path, System.IO.MemoryStream imageStreamSource)
		{
			BitmapImage image = new BitmapImage ();

			image.BeginInit ();
			image.StreamSource  = imageStreamSource;
//-			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption   = BitmapCacheOption.OnLoad;
			image.EndInit ();

			BitmapFileFormat fileFormat = NativeBitmap.GuessFileFormat (path);
			return new NativeBitmap (image, fileFormat);
		}

		public static NativeBitmap Create(System.Drawing.Bitmap bitmap)
		{
			switch (bitmap.PixelFormat)
			{
				case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
					return SystemDrawingBitmapHelper.Create24bppImage (bitmap);
				case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
					return SystemDrawingBitmapHelper.Create32bppImage (bitmap);
				case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
					return SystemDrawingBitmapHelper.Create8bppImage (bitmap);
				default:
					return null;
			}
		}

		internal static NativeBitmap CreateFromPremultipliedArgb32(byte[] imageBytes, int pitch, int dx, int dy)
		{
			WriteableBitmap bitmap = new WriteableBitmap (dx, dy, 72, 72, PixelFormats.Pbgra32, null);
			bitmap.WritePixels (new System.Windows.Int32Rect (0, 0, dx, dy), imageBytes, pitch, 0);

			return new NativeBitmap (bitmap, colorType: BitmapColorType.RgbAlpha);
			
		}


		private void SaveToMemoryJpeg(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new JpegBitmapEncoder ();

			encoder.QualityLevel = fileFormat.Quality;
			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryPng(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new PngBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryTiff(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
		{
			if ((this.ColorType != Platform.BitmapColorType.Cmyk) &&
				(fileFormat.TiffCmyk))
			{
				this.ConvertToCmyk32 ().SaveToMemoryTiff (fileFormat, stream);
			}
			else
			{
				var encoder = new TiffBitmapEncoder ();

				encoder.Compression = (System.Windows.Media.Imaging.TiffCompressOption) (int) fileFormat.TiffCompression;
				encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
				encoder.Save (stream);
			}
		}

		private void SaveToMemoryBmp(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new BmpBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryGif(BitmapFileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new GifBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		
		private static BitmapFileFormat GuessFileFormat(string path)
		{
			BitmapFileFormat fileFormat = new BitmapFileFormat ();

			if (string.IsNullOrEmpty (path))
			{
				return fileFormat;
			}

			string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

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
		
		private static IEnumerable<PixelFormat> GetCmykPixelFormats()
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


		private NativeBitmap GetChannelAlpha()
		{
			if (this.bitmapSource.Format == PixelFormats.Bgra32)
			{
				return this.ExtractChannelFrom32BitSource (offset: 3);
			}
			else
			{
				return this.ConvertToArgb32 ().GetChannelAlpha ();
			}
		}

		private NativeBitmap GetChannelGrayscale()
		{
			if (this.bitmapSource.Format == PixelFormats.Gray8)
			{
				return this;
			}
			else
			{
				return this.ConvertToGrayscale ();
			}
		}

		private NativeBitmap GetChannelCmyk(BitmapColorChannel channel)
		{
			if (this.bitmapSource.Format == PixelFormats.Cmyk32)
			{
				switch (channel)
				{
					case BitmapColorChannel.Cyan:
						return this.ExtractChannelFrom32BitSource (offset: 0);
					case BitmapColorChannel.Magenta:
						return this.ExtractChannelFrom32BitSource (offset: 1);
					case BitmapColorChannel.Yellow:
						return this.ExtractChannelFrom32BitSource (offset: 2);
					case BitmapColorChannel.Black:
						return this.ExtractChannelFrom32BitSource (offset: 3);
				}

				throw new System.InvalidOperationException ();
			}
			else
			{
				return this.ConvertToCmyk32 ().GetChannelCmyk (channel);
			}
		}

		private NativeBitmap GetChannelRgb(BitmapColorChannel channel)
		{
			if ((this.bitmapSource.Format == PixelFormats.Bgra32) ||
				(this.bitmapSource.Format == PixelFormats.Bgr32))
			{
				switch (channel)
				{
					case BitmapColorChannel.Blue:
						return this.ExtractChannelFrom32BitSource (offset: 0);
					case BitmapColorChannel.Green:
						return this.ExtractChannelFrom32BitSource (offset: 1);
					case BitmapColorChannel.Red:
						return this.ExtractChannelFrom32BitSource (offset: 2);
				}

				throw new System.InvalidOperationException ();
			}
			else
			{
				return this.ConvertToArgb32 ().GetChannelRgb (channel);
			}
		}


		private NativeBitmap ExtractChannelFrom32BitSource(int offset)
		{
			byte[] fullImage = new byte[this.Height * this.Pitch];
			this.bitmapSource.CopyPixels (fullImage, this.Pitch, 0);
			
			return NativeBitmap.ExtractChannelFrom32BitSource (fullImage, this.Width, this.Height, this.Pitch, offset);
		}

		private static NativeBitmap ExtractChannelFrom32BitSource(byte[] rgb32Pixels, int dx, int dy, int pitch, int byteOffset)
		{
			byte[] monoPixels = new byte[dx*dy];

			int monoIndex = 0;

			for (int y = 0; y < dy; y++)
			{
				int rgb32Index = y * pitch + byteOffset;

				for (int x = 0; x < dx; x++)
				{
					monoPixels[monoIndex] = rgb32Pixels[rgb32Index];

					monoIndex  += 1;
					rgb32Index += 4;
				}
			}

			WriteableBitmap bitmap = new WriteableBitmap (dx, dy, 72, 72, PixelFormats.Gray8, null);
			bitmap.WritePixels (new System.Windows.Int32Rect (0, 0, dx, dy), monoPixels, dx, 0);

			return new NativeBitmap (bitmap, colorType: BitmapColorType.MinIsBlack);
		}

		
		internal byte[] GetScanline(int y)
		{
			byte[] scanline = new byte[this.Pitch];
			this.bitmapSource.CopyPixels (new System.Windows.Int32Rect (0, y, this.Width, 1), scanline, this.Pitch, 0);
			return scanline;
		}

		internal NativeBitmap Palettize(IEnumerable<Color> palette)
		{
			return new NativeBitmap (new FormatConvertedBitmap (this.bitmapSource, PixelFormats.Indexed8, new BitmapPalette (palette.ToList ()), 0.5), colorType: BitmapColorType.Palette);
		}

		internal NativeBitmap Quantize(int maxColors, int maxBitsPerPixel)
		{
			if (this.BitsPerPixel != 32)
			{
				return this.ConvertToPremultipliedArgb32 ().Quantize (maxColors, maxBitsPerPixel);
			}
			else
			{
				return new NativeBitmap (OctreeQuantizer.Quantize (this.bitmapSource, maxColors, maxBitsPerPixel), colorType: BitmapColorType.Palette);
			}
		}

		internal IEnumerable<Color> GetPalette()
		{
			return this.bitmapSource.Palette.Colors;
		}


		private static System.Action			outOfMemoryHandler;

		private BitmapSource					bitmapSource;
		private BitmapFileFormat				fileFormat;
		private readonly BitmapColorType?		colorType;
	}
}

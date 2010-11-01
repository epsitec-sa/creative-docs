//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Epsitec.Common.Drawing.Platform
{
	public class NativeBitmap : System.IDisposable
	{
		public NativeBitmap()
		{
		}

		private NativeBitmap(BitmapSource bitmapSource, FileFormat fileFormat = null, ColorType? colorType = null)
		{
			this.bitmapSource = bitmapSource;
			this.fileFormat   = fileFormat;
			this.colorType    = colorType;

			int height = this.Height;
			int pitch  = this.Pitch;

			byte[] pixels = new byte[pitch * height];

			this.bitmapSource.CopyPixels (pixels, pitch, 0);
			this.bitmapSource.Freeze ();
		}

		public int BitsPerPixel
		{
			get
			{
				return this.GetBitsPerPixel ();
			}
		}

		public int Width
		{
			get
			{
				return this.bitmapSource.PixelWidth;
			}
		}

		public int Height
		{
			get
			{
				return this.bitmapSource.PixelHeight;
			}
		}

		int GetBitsPerPixel()
		{
			return this.bitmapSource == null ? 0 : this.bitmapSource.Format.BitsPerPixel;
		}

		public FileFormat FileFormat
		{
			get
			{
				return this.fileFormat;
			}
		}

		BitmapSource bitmapSource;

		#region IDisposable Members

		public void Dispose()
		{
			this.bitmapSource = null;
		}

		#endregion

		public void ExtractPixels(System.IntPtr bufferMemory, int bufferSize, int pitch)
		{
			System.Windows.Int32Rect sourceRect = System.Windows.Int32Rect.Empty;

			var temp = new TransformedBitmap ();
			temp.BeginInit ();
			temp.Source = this.bitmapSource;
			temp.Transform = new System.Windows.Media.ScaleTransform (1, -1);
			temp.EndInit ();

			temp.CopyPixels (sourceRect, bufferMemory, bufferSize, pitch);
		}

		public NativeBitmap ConvertTo32Bits()
		{
			FormatConvertedBitmap converter = new FormatConvertedBitmap ();
			converter.BeginInit ();
			converter.Source = this.bitmapSource;
			converter.DestinationFormat = System.Windows.Media.PixelFormats.Pbgra32;
			converter.EndInit ();

			return new NativeBitmap (converter, this.fileFormat);
		}

		public NativeBitmap ConvertTo24Bits()
		{
			FormatConvertedBitmap converter = new FormatConvertedBitmap ();
			converter.BeginInit ();
			converter.Source = this.bitmapSource;
			converter.DestinationFormat = System.Windows.Media.PixelFormats.Bgr24;
			converter.EndInit ();

			return new NativeBitmap (converter, this.fileFormat);
		}
		
		public int MemorySize
		{
			get
			{
				return this.bitmapSource.PixelHeight * this.Pitch;
			}
		}

		public int Pitch
		{
			get
			{
				int bytesPerPixel = (this.bitmapSource.Format.BitsPerPixel + 7) / 8;
				return (this.bitmapSource.PixelWidth * bytesPerPixel + 3) & 0x0ffffffc;
			}
		}

		public string Information
		{
			get;
			set;
		}

		public static NativeBitmap Load(byte[] buffer, string path = null)
		{
#if false
			using (var imageStreamSource = new System.IO.MemoryStream (buffer))
			{
				PngBitmapDecoder decoder = new PngBitmapDecoder (imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				return new ImageClient (decoder.Frames[0]);
			}
#else
			using (var imageStreamSource = new System.IO.MemoryStream (buffer))
			{
				BitmapImage image = new BitmapImage ();
				image.BeginInit ();
				image.StreamSource = imageStreamSource;
				image.EndInit ();
				FileFormat fileFormat = NativeBitmap.GuessFileFormat (path);
				return new NativeBitmap (image, fileFormat);
			}
#endif
		}

		public byte[] SaveToMemory(FileFormat fileFormat)
		{
			var stream = new System.IO.MemoryStream ();

			switch (fileFormat.Type)
			{
				case FileFormatType.Jpeg:
					this.SaveToMemoryJpeg (fileFormat, stream);
					break;

				case FileFormatType.Png:
					this.SaveToMemoryPng (fileFormat, stream);
					break;

				case FileFormatType.Tiff:
					this.SaveToMemoryTiff (fileFormat, stream);
					break;

				case FileFormatType.Bmp:
					this.SaveToMemoryBmp (fileFormat, stream);
					break;

				case FileFormatType.Gif:
					this.SaveToMemoryGif (fileFormat, stream);
					break;
				
				default:
					System.Diagnostics.Debug.Fail ("Unsupported format : " + fileFormat.Type);
					break;
			}

			return stream.ToArray ();
		}

		private void SaveToMemoryJpeg(FileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new JpegBitmapEncoder ();

			encoder.QualityLevel = fileFormat.Quality;
			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryPng(FileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new PngBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryTiff(FileFormat fileFormat, System.IO.MemoryStream stream)
		{
			if ((this.ColorType != Platform.ColorType.Cmyk) &&
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

		private void SaveToMemoryBmp(FileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new BmpBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		private void SaveToMemoryGif(FileFormat fileFormat, System.IO.MemoryStream stream)
		{
			var encoder = new GifBitmapEncoder ();

			encoder.Frames.Add (BitmapFrame.Create (this.bitmapSource));
			encoder.Save (stream);
		}

		
		public bool IsValid
		{
			get
			{
				return this.bitmapSource != null;
			}
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

			var transform = new System.Windows.Media.ScaleTransform (scaleX, scaleY);
			var bitmap = new TransformedBitmap (this.bitmapSource, transform);

			return new NativeBitmap (bitmap, this.fileFormat);
		}

		public static NativeBitmap Load(string path)
		{
			path = System.IO.Path.GetFullPath (path);

			try
			{
				BitmapImage bitmap = new BitmapImage ();

				bitmap.BeginInit ();
				bitmap.UriSource = new System.Uri (path);
				bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				bitmap.EndInit ();

				FileFormat fileFormat = NativeBitmap.GuessFileFormat (path);
				return new NativeBitmap (bitmap, fileFormat);
			}
			catch
			{
				return null;
			}
		}

		private static FileFormat GuessFileFormat(string path)
		{
			FileFormat fileFormat = new FileFormat ();

			if (string.IsNullOrEmpty (path))
			{
				return fileFormat;
			}

			string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

			switch (ext)
			{
				case ".jpg":
				case ".jpeg":
					fileFormat.Type = FileFormatType.Jpeg;
					fileFormat.Quality = 80;
					break;

				case ".png":
					fileFormat.Type = FileFormatType.Png;
					break;

				case ".tif":
				case ".tiff":
					fileFormat.Type = FileFormatType.Tiff;
					break;

				case ".bmp":
					fileFormat.Type = FileFormatType.Bmp;
					break;
			}
			return fileFormat;
		}
		
		private FileFormat fileFormat;
		private ColorType? colorType;
		
		public ColorType ColorType
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
					return ColorType.RgbAlpha;
                }
				if (NativeBitmap.GetRgbPixelFormats ().Any (x => x == format))
                {
					return ColorType.Rgb;
                }
				if (NativeBitmap.GetGrayscalePixelFormats ().Any (x => x == format))
                {
					return ColorType.MinIsBlack;
                }
				if (NativeBitmap.GetIndexedPixelFormats ().Any (x => x == format))
                {
					return ColorType.Palette;
                }
				if (NativeBitmap.GetCmykPixelFormats ().Any (x => x == format))
                {
					return ColorType.Cmyk;
                }

				return ColorType.Unsupported;
			}
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

		public NativeBitmap GetChannel(ColorChannel channel)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return this.GetChannelAlpha ();

				case ColorChannel.Black:
				case ColorChannel.Cyan:
				case ColorChannel.Magenta:
				case ColorChannel.Yellow:
					return this.GetChannelCmyk (channel);

				case ColorChannel.Grayscale:
					return this.GetChannelGrayscale ();

				case ColorChannel.Red:
				case ColorChannel.Green:
				case ColorChannel.Blue:
					return this.GetChannelRgb (channel);

				default:
					throw new System.InvalidOperationException ("Unsupported color channel specified");
			}
		}

		private NativeBitmap GetChannelAlpha()
		{
			if (this.bitmapSource.Format == PixelFormats.Bgra32)
			{
				return this.ExtractChannelFrom32BitSource (offset: 3);
			}
			else
			{
				return this.ConvertToRgb32 ().GetChannelAlpha ();
			}
		}

		private NativeBitmap ExtractChannelFrom32BitSource(int offset)
		{
			byte[] fullImage = new byte[this.Height * this.Pitch];
			this.bitmapSource.CopyPixels (fullImage, this.Pitch, 0);
			return NativeBitmap.CreateGrayscaleFrom32BitPixels (fullImage, this.Width, this.Height, this.Pitch, offset);
		}

		private static NativeBitmap CreateGrayscaleFrom32BitPixels(byte[] rgb32Pixels, int dx, int dy, int pitch, int byteOffset)
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

			return new NativeBitmap (bitmap, colorType: ColorType.MinIsBlack);
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

		private NativeBitmap GetChannelCmyk(ColorChannel channel)
		{
			if (this.bitmapSource.Format == PixelFormats.Cmyk32)
			{
				switch (channel)
				{
					case ColorChannel.Cyan:
						return this.ExtractChannelFrom32BitSource (offset: 0);
					case ColorChannel.Magenta:
						return this.ExtractChannelFrom32BitSource (offset: 1);
					case ColorChannel.Yellow:
						return this.ExtractChannelFrom32BitSource (offset: 2);
					case ColorChannel.Black:
						return this.ExtractChannelFrom32BitSource (offset: 3);
				}

				throw new System.InvalidOperationException ();
			}
			else
			{
				return this.ConvertToCmyk32 ().GetChannelCmyk (channel);
			}
		}

		private NativeBitmap GetChannelRgb(ColorChannel channel)
		{
			if ((this.bitmapSource.Format == PixelFormats.Bgra32) ||
				(this.bitmapSource.Format == PixelFormats.Bgr32))
			{
				switch (channel)
				{
					case ColorChannel.Blue:
						return this.ExtractChannelFrom32BitSource (offset: 0);
					case ColorChannel.Green:
						return this.ExtractChannelFrom32BitSource (offset: 1);
					case ColorChannel.Red:
						return this.ExtractChannelFrom32BitSource (offset: 2);
				}

				throw new System.InvalidOperationException ();
			}
			else
			{
				return this.ConvertToRgb32 ().GetChannelRgb (channel);
			}
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

		public NativeBitmap ConvertToGrayscale()
		{
			FormatConvertedBitmap bitmap = new FormatConvertedBitmap ();

			bitmap.BeginInit ();
			bitmap.Source = this.bitmapSource;
			bitmap.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
			bitmap.EndInit ();

			return new NativeBitmap (bitmap, this.fileFormat, ColorType.MinIsBlack);
		}

		public NativeBitmap ConvertToRgb32()
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

		public NativeBitmap Crop(int x, int y, int dx, int dy)
		{
			CroppedBitmap cropped = new CroppedBitmap(this.bitmapSource, new System.Windows.Int32Rect (x, y, dx, dy));

			//	TODO: check that the rectangle size is correct

			return new NativeBitmap (cropped, this.fileFormat);
		}

		public bool IsTransparent
		{
			get
			{
				var format = this.bitmapSource.Format;
				
				return NativeBitmap.GetRgbAlphaPixelFormats ().Any (x => x == format);
			}
		}
	}

	public enum FileFormatType
	{
		Unknown = -1,
		Bmp = 0,
		Ico = 1,
		Jpeg = 2,
		Jng = 3,
		Koala = 4,
		Iff = 5,
		Lbm = 5,
		Mng = 6,
		Pbm = 7,
		PbmRaw = 8,
		Pcd = 9,
		Pcx = 10,
		Pgm = 11,
		PgmRaw = 12,
		Png = 13,
		Ppm = 14,
		PpmRaw = 15,
		Raw = 16,
		Targa = 17,
		Tiff = 18,
		Wbmp = 19,
		Psd = 20,
		Cut = 21,
		Xbm = 22,
		Xpm = 23,
		Dds = 24,
		Gif = 25,
		Hdr = 26,
		FaxG3 = 27,
	}
	public enum TiffCompressionOption
	{
		// Summary:
		//     The System.Windows.Media.Imaging.TiffBitmapEncoder encoder attempts to save
		//     the bitmap with the best possible compression schema.
		Default = 0,
		//
		// Summary:
		//     The Tagged Image File Format (TIFF) image is not compressed.
		None = 1,
		//
		// Summary:
		//     The CCITT3 compression schema is used.
		Ccitt3 = 2,
		//
		// Summary:
		//     The CCITT4 compression schema is used.
		Ccitt4 = 3,
		//
		// Summary:
		//     The LZW compression schema is used.
		Lzw = 4,
		//
		// Summary:
		//     The RLE compression schema is used.
		Rle = 5,
		//
		// Summary:
		//     Zip compression schema is used.
		Zip = 6,
	}

	public class FileFormat
	{
		public FileFormat()
		{
			this.Type = FileFormatType.Unknown;
		}


		public FileFormatType Type
		{
			get;
			set;
		}

		public TiffCompressionOption TiffCompression
		{
			get;
			set;
		}

		public int Quality
		{
			get;
			set;
		}

		public bool TiffCmyk
		{
			get;
			set;
		}
	}

	public enum ColorChannel
	{
		None,
		Alpha,
		Grayscale,
		Red, Green, Blue,
		Cyan, Magenta, Yellow, Black,
	}

	public enum ColorType
	{
		Unsupported,

		MinIsBlack,
		MinIsWhite,

		Rgb,
		RgbAlpha,
		Palette,

		Cmyk,
	}
}

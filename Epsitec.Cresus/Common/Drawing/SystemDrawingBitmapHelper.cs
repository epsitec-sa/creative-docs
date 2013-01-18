//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Epsitec.Common.Drawing.Platform
{
	using Color=System.Windows.Media.Color;
	
	/// <summary>
	/// The <c>SystemDrawingBitmapHelper</c> class provides conversion methods
	/// to convert native images to <see cref="System.Drawing.Bitmap"/>.
	/// </summary>
	internal static class SystemDrawingBitmapHelper
	{
		/// <summary>
		/// Generates a 1 bit per pixel bitmap mask. Transparent pixels are
		/// represented by a set bit; opaque pixels by a cleared bit.
		/// </summary>
		/// <param name="source">The image source.</param>
		/// <returns>The 1 bit per pixel bitmap mask.</returns>
		public static System.Drawing.Bitmap Generate1BppBitmapMask(NativeBitmap source)
		{
			NativeBitmap source32 = source.ConvertToPremultipliedArgb32 ();

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format1bppIndexed;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			SystemDrawingBitmapHelper.CopyPalette (bitmap, SystemDrawingBitmapHelper.GetMonochromePalette ());

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;

				byte[] bits = new byte[8];

				for (int i = 0; i < 8; i++)
				{
					bits[i] = (byte) (0x80 >> i);
				}

				for (int y = 0; y < dy; y++)
				{
					byte*  rawDst = buffer - y * stride;
					byte[] alpha  = source32.GetScanline (y);

					for (int x = 0; x < dx; x++)
					{
						if (alpha[x*4+3] < 0x80)
						{
							rawDst[x/8] |= bits[x & 0x07];
						}
					}
				}
			}

			bitmap.UnlockBits (raw);

			source32.Dispose ();

			return bitmap;
		}

		/// <summary>
		/// Generates an 8 bit per pixel alpha mask.
		/// </summary>
		/// <param name="source">The image source.</param>
		/// <returns>The 8 bit per pixel alpha mask.</returns>
		public static System.Drawing.Bitmap Generate8BppAlphaMask(NativeBitmap source, bool invertAlpha)
		{
			NativeBitmap source32 = source.ConvertToPremultipliedArgb32 ();

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			SystemDrawingBitmapHelper.CopyPalette (bitmap, SystemDrawingBitmapHelper.Get256GrayPalette ());

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;
				byte  xor    = (byte) (invertAlpha ? 0xff : 0x00);

				for (int y = 0; y < dy; y++)
				{
					byte*  rawDst = buffer - y * stride;
					byte[] alpha  = source32.GetScanline (y);

					for (int x = 0; x < dx; x++)
					{
						*rawDst++ = (byte) (alpha[x*4+3] ^ xor);
					}
				}
			}

			bitmap.UnlockBits (raw);

			source32.Dispose ();

			return bitmap;
		}

		/// <summary>
		/// Generates a 4 bit per pixel bitmap using with a fixed palette. The
		/// palette colors are compatible with Windows 16 color icons.
		/// Transparent pixels are mapped to color index 0.
		/// </summary>
		/// <param name="source">The source image.</param>
		/// <returns>The 4 bit per pixel bitmap.</returns>
		public static System.Drawing.Bitmap Generate4BppBitmapWithFixedPalette(NativeBitmap source)
		{
			System.Drawing.Color[] fixedPalette = SystemDrawingBitmapHelper.GetWindows16ColorPalette ();
			var palette = fixedPalette.Select (x => Color.FromArgb (x.A, x.R, x.G, x.B));

			NativeBitmap source32  = source.ConvertToPremultipliedArgb32 ();
			NativeBitmap source8   = source32.Palettize (palette);

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format4bppIndexed;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			SystemDrawingBitmapHelper.CopyPalette (bitmap, fixedPalette);

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;

				for (int y = 0; y < dy; y++)
				{
					byte[] rawSrc = source8.GetScanline (y);
					byte*  rawDst = buffer - y * stride;
					byte[] alpha  = source32.GetScanline (y);

					for (int x = 0; x < dx; x += 2)
					{
						byte pixel1 = rawSrc[x+0];
						byte pixel2 = rawSrc[x+1];

						int a1 = alpha[x*4+3];
						int a2 = alpha[x*4+7];

						if (a1 < 0x80)
						{
							pixel1 = 0x00;
						}
						if (a2 < 0x80)
						{
							pixel2 = 0x00;
						}

						*rawDst++ = (byte) ((pixel1 << 4) | (pixel2 & 0x0f));
					}
				}
			}

			bitmap.UnlockBits (raw);

			source8.Dispose ();
			source32.Dispose ();

			return bitmap;
		}


		/// <summary>
		/// Generates a 4 bit per pixel bitmap with a 15 color palette; the 16th
		/// color is used to code the transparency.
		/// </summary>
		/// <param name="source">The image source.</param>
		/// <returns>The 4 bit per pixel bitmap.</returns>
		public static System.Drawing.Bitmap Generate4BppBitmapWithTransparentColor(NativeBitmap source)
		{
			NativeBitmap source32  = source.ConvertToPremultipliedArgb32 ();
			NativeBitmap source8   = source32.Quantize (15, 8);

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format4bppIndexed;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			System.Drawing.Color[] palette = source8.GetPalette ().Concat (SystemDrawingBitmapHelper.GetInfiniteTransparentPalette ()).Take (16).Select (x => System.Drawing.Color.FromArgb (x.A, x.R, x.G, x.B)).ToArray ();
			palette[15] = SystemDrawingBitmapHelper.PickUnusedColor (palette);

			SystemDrawingBitmapHelper.CopyPalette (bitmap, palette);

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;

				for (int y = 0; y < dy; y++)
				{
					byte[] rawSrc = source8.GetScanline (y);
					byte*  rawDst = buffer - y * stride;
					byte[] alpha  = source32.GetScanline (y);

					for (int x = 0; x < dx; x += 2)
					{
						byte pixel1 = rawSrc[x+0];
						byte pixel2 = rawSrc[x+1];

						int a1 = alpha[x*4+3];
						int a2 = alpha[x*4+7];

						if (a1 < 0x80)
						{
							pixel1 = 0x0f;
						}
						if (a2 < 0x80)
						{
							pixel2 = 0x0f;
						}

						*rawDst++ = (byte) ((pixel1 << 4) | (pixel2 & 0x0f));
					}
				}
			}

			bitmap.UnlockBits (raw);

			source8.Dispose ();
			source32.Dispose ();

			return bitmap;
		}

		/// <summary>
		/// Generates an 8 bit per pixel bitmap with a 255 color palette; the 256th
		/// color is used to code the transparency.
		/// </summary>
		/// <param name="source">The image source.</param>
		/// <returns>The 8 bit per pixel bitmap.</returns>
		public static System.Drawing.Bitmap Generate8BppBitmapWithTransparentColor(NativeBitmap source)
		{
			NativeBitmap source32  = source.ConvertToPremultipliedArgb32 ();
			NativeBitmap source8   = source32.Quantize (255, 8);

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			System.Drawing.Color[] palette = source8.GetPalette ().Concat (SystemDrawingBitmapHelper.GetInfiniteTransparentPalette ()).Take (256).Select (x => System.Drawing.Color.FromArgb (x.A, x.R, x.G, x.B)).ToArray ();
			palette[255] = SystemDrawingBitmapHelper.PickUnusedColor (palette);

			SystemDrawingBitmapHelper.CopyPalette (bitmap, palette);

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;

				for (int y = 0; y < dy; y++)
				{
					byte[] rawSrc = source8.GetScanline (y);
					byte*  rawDst = buffer - y * stride;
					byte[] alpha  = source32.GetScanline (y);

					for (int x = 0; x < dx; x++)
					{
						int a = alpha[x*4+3];

						if (a < 0x80)
						{
							*rawDst++ = 0xff;
						}
						else
						{
							*rawDst++ = rawSrc[x];
						}
					}
				}
			}

			bitmap.UnlockBits (raw);

			source8.Dispose ();
			source32.Dispose ();

			return bitmap;
		}

		/// <summary>
		/// Generates a 32 bit per pixel bitmap.
		/// </summary>
		/// <param name="source">The image source.</param>
		/// <returns>The 32 bit per pixel bitmap.</returns>
		public static System.Drawing.Bitmap Generate32BppBitmap(NativeBitmap source)
		{
			NativeBitmap source32 = source.ConvertToPremultipliedArgb32 ();

			int dx = source.Width;
			int dy = source.Height;

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
			System.Drawing.Imaging.BitmapData raw;

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int   stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;

				for (int y = 0; y < dy; y++)
				{
					byte[] rawSrc = source32.GetScanline (y);
					byte* rawDst = buffer - y * stride;

					for (int x = 0; x < dx; x++)
					{
						int b = rawSrc[x*4+0];
						int g = rawSrc[x*4+1];
						int r = rawSrc[x*4+2];
						int a = rawSrc[x*4+3];

						if (a == 0)
						{
							r = g = b = 0x00;
						}

						*rawDst++ = (byte) b;
						*rawDst++ = (byte) g;
						*rawDst++ = (byte) r;
						*rawDst++ = (byte) a;
					}
				}
			}

			bitmap.UnlockBits (raw);

			source32.Dispose ();

			return bitmap;
		}

		
		private static void CopyPalette(System.Drawing.Bitmap bitmap, System.Drawing.Color[] colors)
		{
			System.Drawing.Imaging.ColorPalette palette = bitmap.Palette;

			int count = System.Math.Min (colors.Length, palette.Entries.Length);

			for (int i = 0; i < count; i++)
			{
				palette.Entries[i] = colors[i];
			}

			bitmap.Palette = palette;
		}

		private static System.Drawing.Color[] GetMonochromePalette()
		{
			System.Drawing.Color[] palette = new System.Drawing.Color[2];

			palette[0] = System.Drawing.Color.FromArgb (0x00, 0x00, 0x00);
			palette[1] = System.Drawing.Color.FromArgb (0xff, 0xff, 0xff);

			return palette;
		}

		private static System.Drawing.Color[] GetWindows16ColorPalette()
		{
			System.Drawing.Color[] fixedPalette;
			fixedPalette = new System.Drawing.Color[16];

			fixedPalette[0x0] = System.Drawing.Color.FromArgb (0x00, 0x00, 0x00);
			fixedPalette[0x1] = System.Drawing.Color.FromArgb (0x00, 0x00, 0x7f);
			fixedPalette[0x2] = System.Drawing.Color.FromArgb (0x00, 0x7f, 0x00);
			fixedPalette[0x3] = System.Drawing.Color.FromArgb (0x00, 0x7f, 0x7f);
			fixedPalette[0x4] = System.Drawing.Color.FromArgb (0x7f, 0x00, 0x00);
			fixedPalette[0x5] = System.Drawing.Color.FromArgb (0x7f, 0x00, 0x7f);
			fixedPalette[0x6] = System.Drawing.Color.FromArgb (0x7f, 0x7f, 0x00);
			fixedPalette[0x7] = System.Drawing.Color.FromArgb (0xbf, 0xbf, 0xbf);
			fixedPalette[0x8] = System.Drawing.Color.FromArgb (0x7f, 0x7f, 0x7f);
			fixedPalette[0x9] = System.Drawing.Color.FromArgb (0x00, 0x00, 0xff);
			fixedPalette[0xa] = System.Drawing.Color.FromArgb (0x00, 0xff, 0x00);
			fixedPalette[0xb] = System.Drawing.Color.FromArgb (0x00, 0xff, 0xff);
			fixedPalette[0xc] = System.Drawing.Color.FromArgb (0xff, 0x00, 0x00);
			fixedPalette[0xd] = System.Drawing.Color.FromArgb (0xff, 0x00, 0xff);
			fixedPalette[0xe] = System.Drawing.Color.FromArgb (0xff, 0xff, 0x00);
			fixedPalette[0xf] = System.Drawing.Color.FromArgb (0xff, 0xff, 0xff);
			return fixedPalette;
		}

		private static System.Drawing.Color[] Get256GrayPalette()
		{
			System.Drawing.Color[] palette = new System.Drawing.Color[256];

			for (int i = 0; i < 256; i++)
			{
				palette[i] = System.Drawing.Color.FromArgb (i, i, i);
			}

			return palette;
		}

		private static IEnumerable<Color> GetInfiniteTransparentPalette()
		{
			while (true)
			{
				yield return Color.FromArgb (0, 0, 0, 0);
			}
		}

		private static System.Drawing.Color PickUnusedColor(System.Drawing.Color[] palette)
		{
			int n = palette.Length;

			int r = 255;
			int g = 0;
			int b = 255;

			int cycle = 0;

		again:
			for (int i = 0; i < n; i++)
			{
				if ((palette[i].R == r) &&
					(palette[i].G == g) &&
					(palette[i].B == b))
				{
					//	Collision, try again with slightly different colors

					cycle++;

					switch (cycle)
					{
						case 1:
							r--;
							break;
						case 2:
							g++;
							r++;
							break;
						case 3:
							g--;
							b--;
							break;
						case 4:
							g++;
							cycle = 0;
							break;
					}

					goto again;
				}
			}

			return System.Drawing.Color.FromArgb (r, g, b);
		}

		
		internal static NativeBitmap Create24bppImage(System.Drawing.Bitmap bitmap)
		{
			int dx = bitmap.Width;
			int dy = bitmap.Height;

			WriteableBitmap output = new WriteableBitmap (dx, dy, 72, 72, PixelFormats.Bgr24, null);
			
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			System.Drawing.Imaging.BitmapData raw;

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;
				byte[] rawDst = new byte[dx*3];

				for (int y = 0; y < dy; y++)
				{
					byte* rawSrc = buffer - y * stride;
					int rawDstIndex = 0;

					for (int x = 0; x < dx; x++)
					{
						int b = *rawSrc++;
						int g = *rawSrc++;
						int r = *rawSrc++;

						rawDst[rawDstIndex++] = (byte) b;
						rawDst[rawDstIndex++] = (byte) g;
						rawDst[rawDstIndex++] = (byte) r;
					}

					output.WritePixels (new System.Windows.Int32Rect (0, y, dx, 1), rawDst, dx*3, 0);
				}
			}

			bitmap.UnlockBits (raw);

			return new NativeBitmap (output, colorType:	BitmapColorType.Rgb);
		}

		internal static NativeBitmap Create32bppImage(System.Drawing.Bitmap bitmap)
		{
			int dx = bitmap.Width;
			int dy = bitmap.Height;

			WriteableBitmap output = new WriteableBitmap (dx, dy, 72, 72, PixelFormats.Bgra32, null);

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			System.Drawing.Imaging.BitmapData raw;

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;
				byte[] rawDst = new byte[dx*4];
					
				for (int y = 0; y < dy; y++)
				{
					byte* rawSrc = buffer - y * stride;
					int rawDstIndex = 0;
					
					for (int x = 0; x < dx; x++)
					{
						int b = *rawSrc++;
						int g = *rawSrc++;
						int r = *rawSrc++;
						int a = *rawSrc++;

						rawDst[rawDstIndex++] = (byte) b;
						rawDst[rawDstIndex++] = (byte) g;
						rawDst[rawDstIndex++] = (byte) r;
						rawDst[rawDstIndex++] = (byte) a;
					}

					output.WritePixels (new System.Windows.Int32Rect (0, y, dx, 1), rawDst, dx*4, 0);
				}
			}

			bitmap.UnlockBits (raw);

			return new NativeBitmap (output, colorType: BitmapColorType.RgbAlpha);
		}

		internal static NativeBitmap Create8bppImage(System.Drawing.Bitmap bitmap)
		{
			int dx = bitmap.Width;
			int dy = bitmap.Height;

			WriteableBitmap output = new WriteableBitmap (dx, dy, 72, 72, PixelFormats.Bgra32, null);
			
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle (0, 0, dx, dy);
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
			System.Drawing.Imaging.BitmapData raw;
			System.Drawing.Color[] palette = bitmap.Palette.Entries;

			raw = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);

			unsafe
			{
				int stride = raw.Stride;
				byte* buffer = (byte*) raw.Scan0.ToPointer () + (dy-1) * stride;
				byte[] rawDst = new byte[dx*4];
					
				for (int y = 0; y < dy; y++)
				{
					byte* rawSrc = buffer - y * stride;
					int rawDstIndex = 0;

					for (int x = 0; x < dx; x++)
					{
						int index = *rawSrc++;

						int r = palette[index].R;
						int g = palette[index].G;
						int b = palette[index].B;
						int a = 0xff;

						rawDst[rawDstIndex++] = (byte) b;
						rawDst[rawDstIndex++] = (byte) g;
						rawDst[rawDstIndex++] = (byte) r;
						rawDst[rawDstIndex++] = (byte) a;
					}
				}
			}

			bitmap.UnlockBits (raw);

			return new NativeBitmap (output, colorType: BitmapColorType.RgbAlpha);
		}
	}
}

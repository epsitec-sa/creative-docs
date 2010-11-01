//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Epsitec.Common.Drawing.Platform
{
	/// <summary>
	/// The <c>NativeIcon</c> class provides conversion methods to convert
	/// FreeImage images to Windows ICO files.
	/// </summary>
	public static class NativeIcon
	{
		/// <summary>
		/// Creates an icon in the default sizes (16x16, 32x32 and 48x48).
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <returns>The contents of the ICO file.</returns>
		public static byte[] CreateIcon(NativeBitmap image)
		{
			return NativeIcon.CreateIcon (image, 48, 32, 16);
		}

		public static byte[] CreateIcon(byte[] imageBytes, int pitch, int dx, int dy)
		{
			return NativeIcon.CreateIcon (Platform.NativeBitmap.CreateFromPremultipliedArgb32 (imageBytes, pitch, dx, dy));
		}

		/// <summary>
		/// Creates an icon in the default sizes (16x16, 32x32 and 48x48),
		/// including a high resolution version for Windows Vista (256x256).
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <returns>The contents of the ICO file.</returns>
		public static byte[] CreatePngIcon(NativeBitmap image)
		{
			return NativeIcon.CreateIcon (image, 48, 32, 16, 256);
		}

		public static byte[] CreatePngIcon(byte[] imageBytes, int pitch, int dx, int dy)
		{
			return NativeIcon.CreatePngIcon (Platform.NativeBitmap.CreateFromPremultipliedArgb32 (imageBytes, pitch, dx, dy));
		}

		/// <summary>
		/// Creates an icon in the specified sizes. The sizes should be
		/// provided starting with the largest.
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <param name="sizes">The sizes (e.g. 48, 32 and 16).</param>
		/// <returns>The contents of the ICO file.</returns>
		public static byte[] CreateIcon(NativeBitmap image, params int[] sizes)
		{
			NativeBitmap[] images = new NativeBitmap[sizes.Length];

			for (int i = 0; i < sizes.Length; i++)
			{
				images[i] = image.Rescale (sizes[i], -sizes[i]);
			}

			byte[] icon = NativeIcon.CreateIcon (images);

			for (int i = 0; i < sizes.Length; i++)
			{
				images[i].Dispose ();
			}

			return icon;
		}

		/// <summary>
		/// Creates an icon which includes all images downsampled to 16 and
		/// 256 colors, and also the full 32 bpp version. The images should
		/// be sorted by size (largest first).
		/// </summary>
		/// <param name="image">The source images.</param>
		/// <returns>The contents of the ICO file.</returns>
		public static byte[] CreateIcon(IEnumerable<NativeBitmap> images)
		{
			System.Drawing.IconLib.MultiIcon multi = new System.Drawing.IconLib.MultiIcon ();
			System.Drawing.IconLib.SingleIcon icon = multi.Add ("base");

			multi.SelectedIndex = 0;

			foreach (int bpp in new int[] { 4, 8, 32 })
			{
				foreach (NativeBitmap image in images)
				{
					int size = image.Width;

					if (size < 256)
					{
						switch (bpp)
						{
							case 4:
								System.Drawing.Bitmap bitmapMask  = SystemDrawingBitmapHelper.Generate1BppBitmapMask (image);
								System.Drawing.Bitmap bitmap4bpp  = SystemDrawingBitmapHelper.Generate4BppBitmapWithFixedPalette (image);
								icon.Add (bitmap4bpp, bitmapMask);
								bitmapMask.Dispose ();
								bitmap4bpp.Dispose ();
								break;

							case 8:
								System.Drawing.Bitmap bitmap8bpp  = SystemDrawingBitmapHelper.Generate8BppBitmapWithTransparentColor (image);
								icon.Add (bitmap8bpp, bitmap8bpp.Palette.Entries[255]);
								bitmap8bpp.Dispose ();
								break;

							case 32:
								System.Drawing.Bitmap bitmap32bpp = SystemDrawingBitmapHelper.Generate32BppBitmap (image);
								icon.Add (bitmap32bpp, System.Drawing.Color.Transparent);
								bitmap32bpp.Dispose ();
								break;
						}
					}
					else if (bpp == 32)
					{
						System.Drawing.Bitmap bitmap32bpp = SystemDrawingBitmapHelper.Generate32BppBitmap (image);
						System.Drawing.IconLib.IconImage iconImage = icon.Add (bitmap32bpp, System.Drawing.Color.Transparent);

						iconImage.IconImageFormat = System.Drawing.IconLib.IconImageFormat.Png;

						bitmap32bpp.Dispose ();
					}
				}
			}

			System.IO.MemoryStream stream = new System.IO.MemoryStream ();

			multi.Save (stream, System.Drawing.IconLib.MultiIconFormat.Ico);

			return stream.ToArray ();
		}
	}
}

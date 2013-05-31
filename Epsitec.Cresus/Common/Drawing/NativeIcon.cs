//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static byte[] CreateIcon(byte[] imageBytes, int pitch, int dx, int dy)
		{
			return NativeIcon.CreateIcon (Platform.NativeBitmap.CreateFromPremultipliedArgb32 (imageBytes, pitch, dx, dy));
		}

		/// <summary>
		/// Creates an icon in the default sizes (16x16, 32x32 and 48x48).
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <returns>The contents of the ICO file.</returns>
		private static byte[] CreateIcon(NativeBitmap image)
		{
			return NativeIcon.CreateIcon (image, 72, 48, 32, 16);
		}

		public static byte[] CreatePngIcon(byte[] imageBytes, int pitch, int dx, int dy)
		{
			return NativeIcon.CreatePngIcon (Platform.NativeBitmap.CreateFromPremultipliedArgb32 (imageBytes, pitch, dx, dy));
		}

		/// <summary>
		/// Creates an icon in the default sizes (16x16, 32x32 and 48x48),
		/// including a high resolution version for Windows Vista (256x256).
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <returns>The contents of the ICO file.</returns>
		private static byte[] CreatePngIcon(NativeBitmap image)
		{
			return NativeIcon.CreateIcon (image, 72, 48, 32, 16, 256);
		}

		/// <summary>
		/// Creates an icon in the specified sizes. The sizes should be
		/// provided starting with the largest.
		/// </summary>
		/// <param name="image">The source image.</param>
		/// <param name="sizes">The sizes (e.g. 48, 32 and 16).</param>
		/// <returns>The contents of the ICO file.</returns>
		private static byte[] CreateIcon(NativeBitmap image, params int[] sizes)
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

			//	On génère d'abord les images 8 bpp, puis 32 bpp. 4 bpp étant devenu désuet, il
			//	a été supprimé de la liste, bien que le code "case 4" plus bas soit resté, en
			//	cas de besoin futur.

			foreach (int bpp in new int[] { 8, 32 })
			{
				foreach (NativeBitmap image in images)
				{
					//	Une icône consiste en une collection de fichiers BMP non comprimés (4-, 8-,
					//	16- ou 32- bpp) avec une taille qui peut aller techniquement de 1 x 1 à
					//	255 x 255. Microsoft a rajouté bien plus tard le support pour une
					//	représentation sous forme PNG comprimée pour le format 256 x 256,
					//	parce que sinon, une telle icône serait juste monstrueuse (256KB en BMP).
					//	Le format 256 x 256 est donc nécessairement du PNG, et le PNG est
					//	nécessairement en 32 bit/pixel dans la spécification des fichiers .ICO.

					if (image.Width <= 64)
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
					else
					{
						switch (bpp)
						{
							//	Les icônes > 128 n'ont pas de sens en 32 bits (voir remarque ci-dessus).

							case 32:
								System.Drawing.Bitmap bitmap32bpp = SystemDrawingBitmapHelper.Generate32BppBitmap (image);
								var i32 = icon.Add (bitmap32bpp, System.Drawing.Color.Transparent);
								i32.IconImageFormat = System.Drawing.IconLib.IconImageFormat.Png;
								bitmap32bpp.Dispose ();
								break;
						}
					}
				}
			}

			System.IO.MemoryStream stream = new System.IO.MemoryStream ();

			multi.Save (stream, System.Drawing.IconLib.MultiIconFormat.Ico);

			return stream.ToArray ();
		}
	}
}

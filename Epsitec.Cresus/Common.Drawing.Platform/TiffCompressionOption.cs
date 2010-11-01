//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Platform
{
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
}

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

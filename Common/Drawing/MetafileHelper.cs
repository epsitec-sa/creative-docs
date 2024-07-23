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


using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing.Platform
{
    public static class ClipboardMetafileHelper
    {
        /// <summary>
        /// Puts the metafile on the clipboard and dispose the original metafile.
        /// </summary>
        /// <param name="metafile">The metafile.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        static public bool PutMetafileOnClipboardAndDispose(Metafile metafile)
        {
            bool result = false;

            System.IntPtr hEmfOriginal;
            System.IntPtr hEmfClipboard;

            //	Calling GetHenhmetafile makes the metafile object invalid for any
            //	further use !

            hEmfOriginal = metafile.GetHenhmetafile();

            if (hEmfOriginal != System.IntPtr.Zero)
            {
                hEmfClipboard = ClipboardMetafileHelper.CopyEnhMetaFile(
                    hEmfOriginal,
                    System.IntPtr.Zero
                );

                if (hEmfClipboard != System.IntPtr.Zero)
                {
                    if (ClipboardMetafileHelper.OpenClipboard(System.IntPtr.Zero))
                    {
                        if (ClipboardMetafileHelper.EmptyClipboard())
                        {
                            if (
                                ClipboardMetafileHelper.SetClipboardData(
                                    CF_ENHMETAFILE,
                                    hEmfClipboard
                                ) == hEmfClipboard
                            )
                            {
                                result = true;
                            }

                            ClipboardMetafileHelper.CloseClipboard();
                        }
                    }
                }

                ClipboardMetafileHelper.DeleteEnhMetaFile(hEmfOriginal);
            }

            metafile.Dispose();

            return result;
        }

        #region Interop Definitions

        [DllImport("user32.dll")]
        static extern bool OpenClipboard(System.IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        static extern System.IntPtr SetClipboardData(uint uFormat, System.IntPtr hMem);

        [DllImport("user32.dll")]
        static extern bool CloseClipboard();

        [DllImport("gdi32.dll")]
        static extern System.IntPtr CopyEnhMetaFile(System.IntPtr hEmfSrc, System.IntPtr hNull);

        [DllImport("gdi32.dll")]
        static extern bool DeleteEnhMetaFile(System.IntPtr hEmf);

        private const uint CF_ENHMETAFILE = 14;

        #endregion
    }
}

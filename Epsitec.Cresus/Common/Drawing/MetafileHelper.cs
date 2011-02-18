//	Copyright © 2009, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

			hEmfOriginal = metafile.GetHenhmetafile ();
			
			if (hEmfOriginal != System.IntPtr.Zero)
			{
				hEmfClipboard = ClipboardMetafileHelper.CopyEnhMetaFile (hEmfOriginal, System.IntPtr.Zero);
				
				if (hEmfClipboard != System.IntPtr.Zero)
				{
					if (ClipboardMetafileHelper.OpenClipboard (System.IntPtr.Zero))
					{
						if (ClipboardMetafileHelper.EmptyClipboard ())
						{
							if (ClipboardMetafileHelper.SetClipboardData (CF_ENHMETAFILE, hEmfClipboard) == hEmfClipboard)
							{
								result = true;
							}

							ClipboardMetafileHelper.CloseClipboard ();
						}
					}
				}
				
				ClipboardMetafileHelper.DeleteEnhMetaFile (hEmfOriginal);
			}

			metafile.Dispose ();

			return result;
		}
		
		#region Interop Definitions

		[DllImport ("user32.dll")]
		static extern bool OpenClipboard(System.IntPtr hWndNewOwner);
		[DllImport ("user32.dll")]
		static extern bool EmptyClipboard();
		[DllImport ("user32.dll")]
		static extern System.IntPtr SetClipboardData(uint uFormat, System.IntPtr hMem);
		[DllImport ("user32.dll")]
		static extern bool CloseClipboard();
		[DllImport ("gdi32.dll")]
		static extern System.IntPtr CopyEnhMetaFile(System.IntPtr hEmfSrc, System.IntPtr hNull);
		[DllImport ("gdi32.dll")]
		static extern bool DeleteEnhMetaFile(System.IntPtr hEmf);

		private const uint CF_ENHMETAFILE = 14;

		#endregion
	}
}

//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	/// <summary>
	/// The <c>Clipboard</c> class is used to read data in the "HTML Format"
	/// without having .NET cripple the data while doing so.
	/// </summary>
	public static class Clipboard
	{
		public static byte[] ReadHtmlFormat()
		{
			Data data = new Data ();
			int  size = Clipboard.ReadHtmlFromClipboard (ref data);

			if (size < 0)
			{
				return null;
			}
			else
			{
				byte[] buffer = new byte[size];
				
				Clipboard.CopyClipboardData (ref data, buffer, size);
				Clipboard.FreeClipboardData (ref data);

				System.Diagnostics.Debug.Assert (data.DataPtr == System.IntPtr.Zero);
				System.Diagnostics.Debug.Assert (data.Size == 0);
				
				return buffer;
			}
		}

		#region Native Interface to Clipboard.Win32 DLL

		[DllImport ("Clipboard.Win32.dll")]
		private extern static int ReadHtmlFromClipboard(ref Data data);

		[DllImport ("Clipboard.Win32.dll")]
		private extern static int CopyClipboardData(ref Data data, byte[] buffer, int size);
		
		[DllImport ("Clipboard.Win32.dll")]
		private extern static int FreeClipboardData(ref Data data);

		private struct Data
		{
			public System.IntPtr DataPtr;
			public int Size;
		}

		#endregion
	}
}

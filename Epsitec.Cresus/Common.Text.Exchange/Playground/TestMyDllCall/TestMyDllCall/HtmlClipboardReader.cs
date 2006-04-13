using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Text.Exchange
{
	public class NativeHtmlClipboardReader
	{
		[DllImport ("ClipboardDll.dll")]
		private static unsafe extern byte* ReadHtmlFromClipboard();

		[DllImport ("ClipboardDll.dll")]
		public static extern int GetClipboardSize();

		[DllImport ("ClipboardDll.dll")]
		private static extern void FreeClipboard();

		public static byte[] ReadClipBoard()
		{
			int size;
			unsafe
			{
				byte* pBuffer;

				pBuffer = ReadHtmlFromClipboard ();

				if (pBuffer != null)
				{

					size = GetClipboardSize ();

					byte[] managedBuffer = new byte[size];

					for (int i = 0; i < size; i++)
					{
						managedBuffer[i] = pBuffer[i];
					}

					FreeClipboard ();

					return managedBuffer;
				}
			}

			return null;

		}
	}
}

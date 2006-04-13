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
		private static extern int GetClipboardSize();

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

		public static string ReadClipBoardHtml()
		{
			byte [] clipboardBytes = ReadClipBoard() ;
			sbyte[] startbytes = new sbyte[200] ;
			StringBuilder sb = new StringBuilder() ;

			for (int i = 0; i < 200; i++)
			{
				sb.Append((char)clipboardBytes[i]) ;
			}

			string startstring = sb.ToString ();

			const string starthtml = "StartHTML:";
			int startHtmlIndex = startstring.IndexOf (starthtml);
			int index = 0;

			if (startHtmlIndex > 0)
			{
				startHtmlIndex += starthtml.Length;
				string nhtmlindex = startstring.Substring (startHtmlIndex, 9);

				index = Int32.Parse (nhtmlindex);
			}

			string htmlstring = Encoding.UTF8.GetString (clipboardBytes, index, clipboardBytes.Length - index);
			return htmlstring ;

		}
	}
}

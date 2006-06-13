//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ
//  

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Epsitec.Common.Text.Exchange
{
	public class NativeHtmlClipboardReader
	{
		[DllImport ("Clipboard.Win32.dll")]
		private static unsafe extern int ReadHtmlFromClipboard(System.IntPtr handle);

		[DllImport ("Clipboard.Win32.dll")]
		private static extern int ReadClipboardData(byte[] buffer, int size);

		[DllImport ("Clipboard.Win32.dll")]
		private static extern void FreeClipboardData();

		public static byte[] ReadClipBoard()
		{
			int size = NativeHtmlClipboardReader.ReadHtmlFromClipboard (System.IntPtr.Zero);

			if (size > 0)
			{
				byte[] buffer = new byte[size];
				
				NativeHtmlClipboardReader.ReadClipboardData (buffer, size);
				NativeHtmlClipboardReader.FreeClipboardData ();

				return buffer;
			}

			return null;
		}

				
		public static string ReadClipBoardHtml()
		{
#if true
			byte [] clipboardBytes = ReadClipBoard() ;
#else
			FileStream filestream = File.Open ("clipboard.txt", FileMode.Open) ;
			BinaryReader binReader = new BinaryReader (filestream);
			byte[] clipboardBytes = clipboardBytes = binReader.ReadBytes((int)filestream.Length);
			filestream.Close() ;		
#endif

			if (clipboardBytes == null)
			{
				return string.Empty;
			}

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
				string nhtmlindex = startstring.Substring (startHtmlIndex, 15);		// 15 == bonne reserve pour la longueur de l'offset
				index = Misc.ParseInt (nhtmlindex);
			}

			string htmlstring = Encoding.UTF8.GetString (clipboardBytes, index, clipboardBytes.Length - index);
			return htmlstring ;

		}

	}
}

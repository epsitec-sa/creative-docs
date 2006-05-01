//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ
//  
//  [MW:MYSTERE] PROVISOIRE: Le lecture du presse papier avec Clipboard.Win32.dll fait tout déconner avec des plantées des
//  plus bizarres au bout de 2 ou 3 collages. Donc pour l'instant on utilise la lecture buggé du format HTML
//  dans le presse-papiers de .net

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

#if true  
				
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

			sbyte[] startbytes = new sbyte[200] ; // 200 un peu cochon
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
#else
		// code provisoire qui ne déconne pas mais qui foire avec le problème UTF-8, pas grave pour le debug
		public static string ReadClipBoardHtml()
		{
			string s = string.Empty;
			if (System.Windows.Forms.Clipboard.ContainsText (System.Windows.Forms.TextDataFormat.Html))
			{
				s = System.Windows.Forms.Clipboard.GetText (System.Windows.Forms.TextDataFormat.Html);
			}

			if (s.Length == 0)
				return s ;


			string startstring = s.Substring (0, 200); // 200 un peu cochon

			const string starthtml = "StartHTML:";
			int startHtmlIndex = startstring.IndexOf (starthtml);
			int index = 0;

			if (startHtmlIndex > 0)
			{
				startHtmlIndex += starthtml.Length;
				string nhtmlindex = startstring.Substring (startHtmlIndex, 9);

				index = Int32.Parse (nhtmlindex);
			}

			string htmlstring = s.Substring (index);

			return htmlstring;

		}
#endif
	}
}

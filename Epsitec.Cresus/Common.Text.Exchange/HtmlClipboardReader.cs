//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ
//  
//  PROVISOIRE: Le lecture du presse papier avec Clipboard.Win32.dll fait tout déconner avec des plantées des
//  plus bizarres au bout de 2 ou 3 collages. Donc pour l'instant on utilise la lecture buggé du format HTML
//  dans le presse-papiers de .net

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Text.Exchange
{
	public class NativeHtmlClipboardReader
	{
		[DllImport ("Clipboard.Win32.dll")]
		private static unsafe extern byte* ReadHtmlFromClipboard();

		[DllImport ("Clipboard.Win32.dll")]
		private static extern int GetClipboardSize();

		[DllImport ("Clipboard.Win32.dll")]
		private static extern void FreeClipboard();

		public static byte[] ReadClipBoard()
		{
			int size;
			unsafe
			{
				byte* pBuffer;

				pBuffer = ReadHtmlFromClipboard ();

				if (pBuffer != null && (int)pBuffer != 1)
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

#if false  // code qui déconne
		public static string ReadClipBoardHtml()
		{
			byte [] clipboardBytes = ReadClipBoard() ;

			if (clipboardBytes == null)
			{
				return "";
			}

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
#else

		// code provisoire qui ne déconne pas mais qui foire avec le problème UTF-8, pas grave pour le debug
		public static string ReadClipBoardHtml()
		{
			string s = "";
			if (System.Windows.Forms.Clipboard.ContainsText (System.Windows.Forms.TextDataFormat.Html))
			{
				s = System.Windows.Forms.Clipboard.GetText (System.Windows.Forms.TextDataFormat.Html);
			}

			if (s.Length == 0)
				return s ;


			string startstring = s.Substring (0, 200);

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

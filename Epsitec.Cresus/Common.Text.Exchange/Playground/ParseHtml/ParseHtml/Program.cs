using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// place de jeux pour essayer de lire correctement du texte en format "HTML Format"
// depuis le presse-papiers.

namespace ParseHtml
{

	class HtmlClipBoardReader
	{
		[DllImport ("user32.dll", SetLastError=true)]
		static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport ("user32.dll", SetLastError=true)]
		static extern bool CloseClipboard();

		[DllImport ("user32.dll", SetLastError=true)]
		static extern IntPtr GetClipboardData(uint format);

		[DllImport ("kernel32.dll", SetLastError=true)]
		static extern IntPtr GlobalLock(IntPtr h);

		[DllImport ("kernel32.dll", SetLastError=true)]
		static extern bool GlobalUnlock(IntPtr h);

		[DllImport ("kernel32.dll", SetLastError=true)]
		static extern UIntPtr GlobalSize(IntPtr h);

		[DllImport ("user32.dll", SetLastError=true)]
		static extern bool EmptyClipboard();

		[DllImport ("user32.dll", SetLastError=true)]
		static extern uint RegisterClipboardFormat(string format);

		protected int     test ;
		protected int		toto;
		protected double	bla;

		static private uint cf_html = RegisterClipboardFormat ("HTML Format");

		public unsafe string ReadHtmlClipboard()
		{
			bool ok = false;
			int err;

			ok = HtmlClipBoardReader.OpenClipboard ((IntPtr)0);

			// étrange: ok vaut true
			err = Marshal.GetLastWin32Error ();
			// mais err vaut 5 (ERROR_ACCESS_DENIED) !!??

			if (ok)	
			{
				byte *p ;
				int size ;
				IntPtr h ;

				h = GetClipboardData(1) ;

				if (h == IntPtr.Zero)
				{
					err = Marshal.GetLastWin32Error ();
				}

				size = (int)GlobalSize(h) ;

				ok = HtmlClipBoardReader.EmptyClipboard ();

				ok = HtmlClipBoardReader.CloseClipboard ();
			}

			return "";
		}
	}

	class Program
	{
		static string ReadClipboard()
		{
			HtmlClipBoardReader clipboardreader = new HtmlClipBoardReader();

			clipboardreader.ReadHtmlClipboard ();


#if false
			string returnHtmlText =	 Clipboard.GetText (TextDataFormat.Html);
			return returnHtmlText;
#endif

#if false
			string str = HtmlReadClipboard ();

			System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject ();
			data.GetData (System.Windows.Forms.DataFormats.Html, false);
			IDataObject dataobject = System.Windows.Forms.Clipboard.GetDataObject ();


			UTF8Encoding utf8 = new UTF8Encoding ();

			string euro = "@€A";

			byte[] be = utf8.GetBytes(euro) ;


			MemoryStream ms;
			ms = dataobject.GetData (System.Windows.Forms.DataFormats.Html, true) as MemoryStream;

			if (o != null)
			{

				byte[] b = new byte[o.Length] ;

				int i = 0;
				foreach (char c in o)
				{
					b[i++] = (byte)c ;
					if (c == (byte) '@')
					{
						Console.WriteLine ();
					}
				}
				string us = utf8.GetString (b);

				Console.WriteLine (o.GetType ().ToString ());
				Console.Write (o);
				return o;
			}
#endif
			return "";
		}

		
		[STAThread]
		static void Main(string[] args)
		{
			// Create an instance of StreamReader to read from a file.
			// The using statement also closes the StreamReader.

			string s = ReadClipboard ();
#if false
			using (StreamReader sr = new StreamReader ("file.txt"))
			{
				String line;
				// Read and display lines from the file until the end of 
				// the file is reached.
				while ((line = sr.ReadLine ()) != null)
				{
					Console.WriteLine (line);
				}
			}
#endif
		}
	}
}

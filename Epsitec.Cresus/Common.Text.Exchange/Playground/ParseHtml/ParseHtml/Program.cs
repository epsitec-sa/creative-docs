using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Epsitec.Common.Text.Exchange.HtmlParser;

// using System.Runtime.InteropServices;

// place de jeux pour essayer de lire correctement du texte en format "HTML Format"
// depuis le presse-papiers.

namespace Epsitec.Common.Text.Exchange
{
#if false
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
#endif
	

	class Program
	{
#if false
		static string ReadClipboard()
		{

			HtmlClipBoardReader clipboardreader = new HtmlClipBoardReader();

			clipboardreader.ReadHtmlClipboard ();


			string returnHtmlText =	 Clipboard.GetText (TextDataFormat.Html);
			return returnHtmlText;


			
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

			return "";
		}
#endif


		static void ProcessNodes(HtmlNodeCollection nodes)
		{
			bool italique = false;
			bool bold = false;
			bool span = false;
			foreach (HtmlNode node in nodes)
			{
				string s = node.ToString ();
				if (node is HtmlElement)
				{
					HtmlElement element = node as HtmlElement;

					if (element.Name == "span")
					{
						Console.WriteLine (">>> span on");
						span = true;
					}

					if (element.Name == "i")
					{
						Console.WriteLine (">>> italic on");
						italique = true;
					}

					if (element.Name == "b")
					{
						Console.WriteLine (">>> bold on");
						bold = true;
					}

#if false
					Console.WriteLine ("Element: {0}", element.Name) ;
					if (element.Attributes.Count > 0)
					{
						foreach (HtmlAttribute attribute in element.Attributes)
						{
							Console.WriteLine (" Arribute: {0} = {1}", attribute.Name, attribute.Value);
						}
					}
#endif
					ProcessNodes (element.Nodes);

					if (italique)
					{
						Console.WriteLine (">>> italic off");
						italique = false;
					}
					if (span)
					{
						Console.WriteLine (">>> span off");
						span = false;
					}


					if (bold)
					{
						Console.WriteLine (">>> bold off");
						bold = false;
					}



				}
				else
				{
					HtmlText text = node as HtmlText;
					Console.WriteLine ("Text: {0}", text.Text);
				}
			}
		}

		[STAThread]
		static void Main(string[] args)
		{
			// Create an instance of StreamReader to read from a file.
			// The using statement also closes the StreamReader.

			// string s = NativeHtmlClipboardReader.ReadClipBoard ();

//			string s = NativeHtmlClipboardReader.ReadClipBoardHtml ();
#if true
			String line;
			using (StreamReader sr = new StreamReader ("test1.htm"))
			{
				// Read and display lines from the file until the end of 
				// the file is reached.

				line = sr.ReadToEnd ();
			}

			HtmlDocument thehtmldoc = new HtmlDocument (line, false);

			ProcessNodes (thehtmldoc.Nodes);

#endif
		}
	}
}

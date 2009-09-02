//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	using IDataObject=System.Windows.Forms.IDataObject;

	/// <summary>
	/// The <c>ClipboardWriteData</c> class contains a collection of values which
	/// represent the data to put into the clipboard.
	/// </summary>
	public sealed class ClipboardWriteData
	{
		public ClipboardWriteData()
		{
			this.data = new System.Windows.Forms.DataObject ();
		}


		internal IDataObject Data
		{
			get
			{
				return this.data;
			}
		}
		

		public void WriteObject(string format, object value)
		{
			this.data.SetData(format, false, value);
		}

		public void WriteText(string value)
		{
			value = value.Replace ("\r\n", "\n");
			value = value.Replace ("\n", "\r\n");
			
			this.data.SetData ("UnicodeText", true, value);
		}
		
		public void WriteHtmlFragment(string value)
		{
			//	Quand on place un texte HTML dans le presse-papier, il faut aussi en placer une
			//	version textuelle :
			
			string text = value.Replace ("<br />", "\r\n");
			this.WriteText (Support.Utilities.XmlBreakToText (text));
			
			System.Text.StringBuilder html = new System.Text.StringBuilder ();
			
			//	Le malheur, c'est que ni FrontPage 2000, ni Word 2000 ne comprennent l'entité &apos;
			//	et du coup, on ne peut pas l'utiliser !
			
			string source = value.Replace ("&apos;", "&#39;");
			string utf8   = Clipboard.ConvertStringToBrokenUtf8 (Clipboard.ConvertSimpleXmlToHtml (source));
			
			html.Append ("Version:1.0\n");
			html.Append ("StartHTML:00000000\n");		int idxStartHtml = html.Length - 9;
			html.Append ("EndHTML:00000000\n");			int idxEndHtml   = html.Length - 9;
			html.Append ("StartFragment:00000000\n");	int idxStartFrag = html.Length - 9;
			html.Append ("EndFragment:00000000\n");		int idxEndFrag   = html.Length - 9;
			html.Append ("\n");							int idxHtmlBegin = html.Length;
			html.Append ("<html>\n");
			html.Append ("<body>\n");					int idxFragBegin = html.Length;
			html.Append ("<!--StartFragment-->\n");
			html.Append (utf8);
			html.Append ("<!--EndFragment-->\n");		int idxFragEnd   = html.Length;
			html.Append ("</body>\n");
			html.Append ("</html>\n");					int idxHtmlEnd   = html.Length;
			
			this.PatchString (html, idxStartHtml, string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00000000}", idxHtmlBegin));
			this.PatchString (html, idxEndHtml,   string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00000000}", idxHtmlEnd));
			this.PatchString (html, idxStartFrag, string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00000000}", idxFragBegin));
			this.PatchString (html, idxEndFrag,   string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00000000}", idxFragEnd));
			
			
			byte[] blob = new byte[html.Length];

			for (int i = 0; i < html.Length; i++)
			{
				blob[i] = (byte) html[i];
			}

			System.IO.MemoryStream stream = new System.IO.MemoryStream (blob);
			
			this.data.SetData (Clipboard.Formats.Hmtl, true, stream);
		}

		public void WriteTextLayout(string value)
		{
			this.data.SetData (Clipboard.Formats.TextLayoutV1, false, value);
		}


		private void PatchString(System.Text.StringBuilder buffer, int pos, string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				buffer[pos+i] = text[i];
			}
		}


		private readonly IDataObject			data;
	}
}

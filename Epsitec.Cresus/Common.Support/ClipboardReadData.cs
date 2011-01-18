//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using IDataObject=System.Windows.Forms.IDataObject;

	/// <summary>
	/// The <c>ClipboardReadData</c> class contains a snapshot of the clipboard contents.
	/// </summary>
	public sealed class ClipboardReadData
	{
		internal ClipboardReadData(IDataObject data)
		{
			if (data == null)
			{
				this.data = System.Windows.Forms.Clipboard.GetDataObject ();
				this.html = Epsitec.Common.Support.Platform.Win32.Clipboard.ReadHtmlFormat ();
			}
			else
			{
				this.data = data;
				this.html = null;
			}
		}
		
		
		public string[]						NativeFormats
		{
			get
			{
				return this.data.GetFormats (false);
			}
		}
		
		public string[]						AllPossibleFormats
		{
			get
			{
				return this.data.GetFormats (true);
			}
		}


		
		public object Read(string format)
		{
			if ((format == Clipboard.Formats.Hmtl) &&
				(this.html != null))
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (this.html.Length);

				for (int i = 0; i < this.html.Length; i++)
				{
					buffer.Append ((char) this.html[i]);
				}

				return buffer.ToString ();
			}
			else
			{
				return this.data.GetData (format, true);
			}
		}
		
		public string ReadAsString(string format)
		{
			return this.Read (format) as string;
		}
		
		public string[] ReadAsStringArray(string format)
		{
			return this.Read (format) as string[];
		}
		
		public string ReadText()
		{
			return this.data.GetData (typeof (string)) as string;
		}
		
		public string ReadHtmlFragment()
		{
			string rawHtml = this.ReadAsString (Clipboard.Formats.Hmtl);
			
			if (rawHtml == null)
			{
				return null;
			}
			
			//	Vérifie qu'il y a bien tous les tags dans le fragment HTML :
			
			int idxVersion    = rawHtml.IndexOf ("Version:");
			int idxStartHtml  = rawHtml.IndexOf ("StartHTML:");
			int idxEndHtml    = rawHtml.IndexOf ("EndHTML:");
			int idxStartFrag  = rawHtml.IndexOf ("StartFragment:");
			int idxEndFrag    = rawHtml.IndexOf ("EndFragment:");
			int idxStart      = rawHtml.IndexOf ("<!--StartFragment");
			int idxEnd        = rawHtml.IndexOf ("<!--EndFragment");
			int idxBegin      = idxStart < 0 ? -1 : rawHtml.IndexOf (">", idxStart) + 1;
			
			if ((idxStart      < idxVersion) ||
				(idxEnd        < idxStart) ||
				(idxBegin      < 1) ||
				(idxVersion    < 0) ||
				(idxStartHtml < idxVersion) ||
				(idxEndHtml   < idxStartHtml) ||
				(idxStartFrag < idxVersion) ||
				(idxEndFrag   < idxStartFrag))
			{
				return null;
			}
			
			return Clipboard.ConvertBrokenUtf8ToString (rawHtml.Substring (idxBegin, idxEnd - idxBegin));
		}
		
		public string ReadHtmlDocument()
		{
			string rawHtml = this.ReadAsString (Clipboard.Formats.Hmtl);
			
			if (rawHtml == null)
			{
				return null;
			}
			
			//	Vérifie qu'il y a bien tous les tags dans le fragment HTML :
			
			int idxVersion    = rawHtml.IndexOf ("Version:");
			int idxStartHtml  = rawHtml.IndexOf ("StartHTML:");
			int idxEndHtml    = rawHtml.IndexOf ("EndHTML:");
			int idxStartFrag  = rawHtml.IndexOf ("StartFragment:");
			int idxEndFrag    = rawHtml.IndexOf ("EndFragment:");
			int idxStart      = rawHtml.IndexOf ("<!--StartFragment");
			int idxEnd        = rawHtml.IndexOf ("<!--EndFragment");
			int idxBegin      = idxStart < 0 ? -1 : rawHtml.IndexOf (">", idxStart) + 1;
			
			if ((idxStart      < idxVersion) ||
				(idxEnd        < idxStart) ||
				(idxBegin      < 1) ||
				(idxVersion    < 0) ||
				(idxStartHtml < idxVersion) ||
				(idxEndHtml   < idxStartHtml) ||
				(idxStartFrag < idxVersion) ||
				(idxEndFrag   < idxStartFrag))
			{
				return null;
			}
			
			idxBegin = System.Int32.Parse (this.ExtractDigits (rawHtml, idxStartHtml + 10));
			idxEnd   = System.Int32.Parse (this.ExtractDigits (rawHtml, idxEndHtml + 8));
			
			return Clipboard.ConvertBrokenUtf8ToString (rawHtml.Substring (idxBegin, idxEnd - idxBegin));
		}
		
		public string ReadTextLayout()
		{
			return this.ReadAsString (Clipboard.Formats.TextLayoutV1);
		}
		
		
		public bool IsCompatible(ClipboardDataFormat format)
		{
			switch (format)
			{
				case ClipboardDataFormat.Text:
					return this.data.GetDataPresent (Clipboard.Formats.String, true);
				
				case ClipboardDataFormat.Image:
					return this.data.GetDataPresent (Clipboard.Formats.Bitmap, true);
				
				case ClipboardDataFormat.MicrosoftHtml:
					return this.data.GetDataPresent (Clipboard.Formats.Hmtl, false);
			}
			
			return false;
		}

		public bool IsCompatible(string format)
		{
			return this.data.GetDataPresent (format, true);
		}
		
		
		private string ExtractDigits(string text, int pos)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = pos; i < text.Length; i++)
			{
				char c = text[i];
				
				if ((c < '0') || (c > '9'))
				{
					break;
				}
				
				buffer.Append (c);
			}
			
			return buffer.ToString ();
		}
		
		
		private IDataObject					data;
		private byte[]						html;
	}
}

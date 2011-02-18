//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// La classe MSClipboardHtmlHeader gère la création de l'en-tête pour le format html du presse-papiers
	/// </summary>
	public class MSClipboardHtmlHeader
	{
		public MSClipboardHtmlHeader()
		{
			this.text.AppendLine ("Version:1.0");
			this.text.AppendLine ("StartHTML:<STARTHT>");
			this.text.AppendLine ("EndHTML:<ENDHTML>");
			this.text.AppendLine ("StartFragment:<STARTFR>");
			this.text.AppendLine ("EndFragment:<ENDFRAG>");
			this.text.AppendLine ("StartSelection:<STARTSE>");
			this.text.AppendLine ("EndSelection:<ENDSELE>");
			this.text.AppendLine ("SourceURL:mhtml:mid://00000002/");
			this.text.AppendLine ("");
			this.offsetHtml = this.text.Length;
			this.UpdateOffset ("<STARTHT>", offsetHtml);
		}

		public void AddHtmlText(ref string htmltext)
		{
			this.text.Append (htmltext);
		}

		public void AddHtmlText(ref ExchangeStringBuilder htmltext)
		{
			this.text.Append (htmltext.ToString ());
		}

		public void UpdateStartFragment(int offset)
		{
			this.UpdateOffset ("<STARTFR>", offset + this.offsetHtml);
			this.UpdateOffset ("<STARTSE>", offset + this.offsetHtml);
		}

		public void UpdateEndFragment(int offset)
		{
			UpdateOffset ("<ENDFRAG>", offset + this.offsetHtml);
			UpdateOffset ("<ENDSELE>", offset + this.offsetHtml);
		}

		public void UpdateStartHtml(int offset)
		{
			UpdateOffset ("<STARTHTM>", offset + this.offsetHtml);
		}

		public void UpdateEndHtml(int offset)
		{
			UpdateOffset ("<ENDHTML>", offset + this.offsetHtml);
		}

		public override string ToString()
		{
			return this.text.ToString ();
		}

		public byte[] GetBytes()
		{
			return System.Text.Encoding.UTF8.GetBytes (this.text.ToString ());
		}

		private void UpdateOffset(string offsetname, int offset)
		{
			this.text.Replace (offsetname, string.Format ("{0,9:d9}", offset));
		}

		private StringBuilder text = new StringBuilder ();
		private int offsetHtml;
	}
}

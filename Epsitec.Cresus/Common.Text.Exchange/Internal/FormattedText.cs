//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using System.Collections.Generic;

namespace Epsitec.Common.Text.Exchange.Internal
{
	/// <summary>
	/// La classe <c>FormattedText</c> ne sert qu'à pouvoir stocker le format
	/// presse-papier interne (qui existe sous forme de string) dans le presse-
	/// papier Windows. C'est nécessaire à cause de la gestion particulière du
	/// presse-papier avec .NET
	/// </summary>
	[System.Serializable]
	public class FormattedText
	{
		public FormattedText()
		{
			this.encodedText = "";
		}

		public FormattedText(string encodedText)
		{
			this.encodedText = encodedText;
		}

		public string EncodedText
		{
			get
			{
				return this.encodedText;
			}
			set
			{
				this.encodedText = value;
			}
		}

		public static System.Windows.Forms.DataFormats.Format ClipboardFormat
		{
			get
			{
				return FormattedText.format;
			}
		}

		public override string ToString()
		{
			return this.EncodedText;
		}

		static FormattedText()
		{
			// Registers a new data format with the windows Clipboard
			format = System.Windows.Forms.DataFormats.GetFormat ("Epsitec.FormattedText");
		}

		private static System.Windows.Forms.DataFormats.Format format;
		private string encodedText;
	}
}

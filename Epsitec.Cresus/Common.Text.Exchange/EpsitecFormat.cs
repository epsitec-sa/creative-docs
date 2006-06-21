using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

// La classe EpsitecFormat ne sert qu'� pouvoir stocker le format presse-papier interne (qui existe sous forme de string)
// dans le presse papier Windows. C'est n�cessaire � cause de la gestion particuli�re du presse-papier avec .Net

// Responsable: Michael Walz


namespace Epsitec.Common.Text.Exchange
{
	[Serializable ()]
	public class EpsitecFormat
	{

		private static DataFormats.Format format;
		static EpsitecFormat()
		{
			// Registers a new data format with the windows Clipboard
			format = DataFormats.GetFormat("EpsitecFormattedText") ;
		}
		
		public static DataFormats.Format Format
		{
			get
			{
				return format;
			}
		}

		private string stringData;

		public EpsitecFormat()
		{
			stringData = "";
		}

		public EpsitecFormat(string str)
		{
			stringData = str;
		}

		public string String
		{
			get
			{
				return stringData;
			}
			set
			{
				stringData = value;
			}
		}


		public override string ToString()
		{
			return String;
		}
	}

}

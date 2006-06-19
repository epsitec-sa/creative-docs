using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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

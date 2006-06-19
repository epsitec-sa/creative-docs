
#define USE_SPAN

using System;
using System.Collections.Generic;
using System.Text;


namespace Epsitec.Common.Text.Exchange
{
	public class NativeTextOut
	{
		StringBuilder output = new StringBuilder() ;

		public NativeTextOut()
		{
			this.Initialize() ;
		}


		private void Initialize()
		{
		}

		public void SetItalic(bool italic)
		{
		}

		public void SetBold(bool bold)
		{
		}

		public void SetUnderlined(bool underlined)
		{
		}

		public void SetStrikeout(bool strikeout)
		{
		}

		public void SetFont(double fontSize)
		{
		}


		public void AppendText(string text)
		{
			output.Append (text);
		}

		public string ToString()
		{
			return output.ToString ();
		}
		
	}

}

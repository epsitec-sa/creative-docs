
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

		public void AppendText(string text)
		{
			output.Append (text);
		}

		public void AppendTextLine(string text)
		{
			output.AppendLine (text);
		}

		public string ToString()
		{
			return output.ToString ();
		}

	}
}

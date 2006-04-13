using System;
using System.Collections.Generic;
using System.Text;
using Epsitec.Common.Text.Exchange;

namespace Epsitec.Common.Text.Exchange
{
	class Program
	{
		static void Main(string[] args)
		{
			byte[] p = NativeHtmlClipboardReader.ReadClipBoard ();
			string str = Encoding.UTF8.GetString (p);

			Console.ReadLine ();



		}
	}
}

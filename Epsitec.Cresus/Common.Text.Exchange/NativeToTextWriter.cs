using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Epsitec.Common.Text.Exchange
{
	class NativeToTextWriter
	{
		public NativeToTextWriter(string nativeClipboard, Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, TextNavigator navigator)
		{
			this.nativeClipboard = nativeClipboard;
			this.navigator = navigator;
			this.paraWrapper = paraWrapper;
			this.textWrapper = textWrapper;
		}

		public void ProcessIt()
		{
			StringReader theReader = new StringReader (nativeClipboard);
			string formatline;
			string contentline;

			while (true)
			{
				formatline = theReader.ReadLine ();
				if (formatline == null)
					break;

				contentline = theReader.ReadLine ();
				if (contentline == null)
					break;

				bool paragraphSep = false ;
				NativeConverter.SetDefined (textWrapper, formatline, ref paragraphSep);

				this.navigator.Insert (contentline);

			}

		}

		Wrappers.TextWrapper textWrapper ;
		Wrappers.ParagraphWrapper paraWrapper ;
		TextNavigator navigator;
		string nativeClipboard ;

	}
}

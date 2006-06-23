using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Epsitec.Common.Text.Exchange
{
	class NativeToTextWriter
	{
		public NativeToTextWriter(string nativeClipboard, Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, TextNavigator navigator, TextStory story, PasteMode pastemode)
		{
			this.nativeConverter = new NativeConverter (textWrapper, paraWrapper, navigator, story);
			this.nativeClipboard = nativeClipboard;
			this.navigator = navigator;
			this.paraWrapper = paraWrapper;
			this.textWrapper = textWrapper;
			this.story = story;
			this.pasteMode = pastemode;
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

				bool paragraphSep;
				this.nativeConverter.SetDefined (formatline, out paragraphSep);

				if (paragraphSep)
				{
					contentline = new string((char) Epsitec.Common.Text.Unicode.Code.ParagraphSeparator, 1) ;
				}
				else
				{
					contentline = theReader.ReadLine ();
				}

				if (contentline == null)
					break;

				this.navigator.InsertWithTabs (contentline);

				if (paragraphSep)
				{
					this.paraWrapper.Defined.ClearDefinedProperties ();
					paragraphSep = false;
				}
			}
		}

		private Wrappers.TextWrapper textWrapper ;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private TextStory story;
		private string nativeClipboard;
		private PasteMode pasteMode;
		private NativeConverter nativeConverter;

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Epsitec.Common.Text.Exchange
{
	class NativeToTextWriter
	{
		public NativeToTextWriter(string nativeClipboard, CopyPasteContext cpContext, PasteMode pastemode)
		{
			this.nativeConverter = new NativeConverter (cpContext, pastemode);
			this.nativeClipboard = nativeClipboard;
			this.cpContext = cpContext;
			this.pasteMode = pastemode;
			this.theReader = new StringReader (nativeClipboard);
		}

		public void ProcessIt()
		{
			string formatline;
			string contentline;

			while (true)
			{
				formatline = theReader.ReadLine ();
				if (formatline == null)
					break;

				if (formatline == "{")
				{
					this.ProcessStyles ();
					continue;
				}

				if (this.pasteMode == PasteMode.KeepTextOnly)
				{
					if (NativeConverter.IsParagraphSeparator (formatline))
					{
						contentline = new string ((char) Epsitec.Common.Text.Unicode.Code.ParagraphSeparator, 1);
					}
					else
					{
						contentline = theReader.ReadLine ();
						if (contentline == null)
							break;
					}

					this.cpContext.Navigator.InsertWithTabs (contentline);
				}
				else
				{
					bool paragraphSep;
					this.nativeConverter.SetDefined (formatline, out paragraphSep);

					if (paragraphSep)
					{
						contentline = new string ((char) Unicode.Code.ParagraphSeparator, 1);
					}
					else
					{
						contentline = theReader.ReadLine ();
					}

					if (contentline == null)
						break;

					this.cpContext.Navigator.InsertWithTabs (contentline);

					if (paragraphSep)
					{
#if true
						this.nativeConverter.ResetParagraph ();
#else
						this.cpContext.ParaWrapper.Defined.ClearDefinedProperties ();
#endif
						paragraphSep = false;
					}
				}
			}
		}

		private void ProcessStyles()
		{
			string line;
			TextContext context = this.cpContext.Story.TextContext;

			while (true)
			{
				line = theReader.ReadLine ();
				if (line == null)
					break;

				if (line == "}")
					break ;

#if false
				int indexendcaption = line.IndexOf ('\\');
				string stylecaption = line.Substring(0, indexendcaption) ;

				line = line.Substring (indexendcaption + 1);

				indexendcaption = line.IndexOf ('\\') ;
				string strnbbasestyles = line.Substring (0, indexendcaption);

				int nbbasestyles = Misc.ParseInt (strnbbasestyles);
#else
				string stylecaption = Misc.NextElement(ref line, '\\') ;
				string strnbbasestyles = Misc.NextElement(ref line, '\\') ;
				int nbbasestyles = Misc.ParseInt (strnbbasestyles);

				TextStyle style = context.StyleList.StyleMap.GetTextStyle (stylecaption);

				if (style == null)
				{
					for (int i = 0; i < nbbasestyles; i++)
					{
//						TextStyle style = context.StyleList.StyleMap.GetTextStyle (stylecaption);

						string basestylecaption = Misc.NextElement (ref line, '\\');
					}

				}

#endif
			}
		}

		StringReader theReader;
		private string nativeClipboard;
		private PasteMode pasteMode;
		private NativeConverter nativeConverter;
		private CopyPasteContext cpContext;

	}
}

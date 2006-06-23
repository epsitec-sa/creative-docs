using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Epsitec.Common.Text.Exchange
{
	class NativeToTextWriter
	{
		public NativeToTextWriter(string nativeClipboard, CopyPasteContect cpContext, PasteMode pastemode)
		{
			this.nativeConverter = new NativeConverter (cpContext);
			this.nativeClipboard = nativeClipboard;
			this.cpContext = cpContext;
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

				if (this.pasteMode == PasteMode.InsertRaw)
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
						this.cpContext.ParaWrapper.Defined.ClearDefinedProperties ();
						paragraphSep = false;
					}
				}
			}
		}

		private string nativeClipboard;
		private PasteMode pasteMode;
		private NativeConverter nativeConverter;
		private CopyPasteContect cpContext;

	}
}

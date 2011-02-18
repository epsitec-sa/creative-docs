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
					this.nativeConverter.StyleDefinitions = this.ProcessStyles ();
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
					this.nativeConverter.SetDefinedString (formatline, out paragraphSep);

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

		private Dictionary<int, StyleDefinition> ProcessStyles()
		{
			string line;
			TextContext context = this.cpContext.Story.TextContext;

			Dictionary<int, StyleDefinition> styleDefinitions = new Dictionary<int, StyleDefinition> ();

			while (true)
			{
				line = theReader.ReadLine ();
				if (line == null)
					break;

				if (line == "}")
					break ;

				string stylecaption =  SerializerSupport.DeserializeString (Misc.NextElement (ref line, '/'));

				string strstyleident = Misc.NextElement (ref line, '/');
				int styleident = Misc.ParseInt (strstyleident);

				string styledefault = Misc.NextElement (ref line, '/');
				bool isDefault = Misc.ParseBool (styledefault);

				string strstyleclass = Misc.NextElement (ref line, '/');
				TextStyleClass styleclass = (TextStyleClass) Misc.ParseByte (strstyleclass);

				string strnbbasestyles = Misc.NextElement(ref line, '/') ;
				int nbbasestyles = Misc.ParseInt (strnbbasestyles);

				TextStyle style = context.StyleList.StyleMap.GetTextStyle (stylecaption);

				//if (style == null)
				{
					string[] baseStyleCaptions = new string[nbbasestyles] ;

					for (int i = 0; i < nbbasestyles; i++)
					{
						string basestylecaption = SerializerSupport.DeserializeString(Misc.NextElement (ref line, '/'));
						baseStyleCaptions[i] = basestylecaption ;
					}

					StyleDefinition styleDefinition = new StyleDefinition (stylecaption, styleident, styleclass, baseStyleCaptions, line, isDefault);
					styleDefinitions.Add (styleident, styleDefinition);
				}
			}

			return styleDefinitions;
		}

		StringReader theReader;
		private string nativeClipboard;
		private PasteMode pasteMode;
		private NativeConverter nativeConverter;
		private CopyPasteContext cpContext;

	}
}

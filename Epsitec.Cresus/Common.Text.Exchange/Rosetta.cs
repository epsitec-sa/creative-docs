//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

using System.Text;

namespace Epsitec.Common.Text.Exchange
{


	public class MSHtmlText
	{
		public MSHtmlText()
		{
			this.text.AppendLine ("Version:1.0");
			this.text.AppendLine ("StartHTML:<STARTHT>");
			this.text.AppendLine ("EndHTML:<ENDHTML>");
			this.text.AppendLine ("StartFragment:<STARTFR>");
			this.text.AppendLine ("EndFragment:<ENDFRAG>");
			this.text.AppendLine ("StartSelection:<STARTSE>");
			this.text.AppendLine ("EndSelection:<ENDSELE>");
			this.text.AppendLine ("SourceURL:mhtml:mid://00000002/");
			this.text.AppendLine ("");
			offsetHtml = this.text.Length;
			this.UpdateOffset ("<STARTHT>", offsetHtml);
		}

		public void AddHtmlText(ref string htmltext)
		{
			this.text.Append (htmltext);
		}

		public void AddHtmlText(ref ExchangeStringBuilder htmltext)
		{
			this.text.Append (htmltext.ToString());
		}

		public void UpdateStartFragment(int offset)
		{
			UpdateOffset ("<STARTFR>", offset + offsetHtml);
			UpdateOffset ("<STARTSE>", offset + offsetHtml);
		}

		public void UpdateEndFragment(int offset)
		{
			UpdateOffset ("<ENDFRAG>", offset + offsetHtml);
			UpdateOffset ("<ENDSELE>", offset + offsetHtml);
		}

		public void UpdateStartHtml(int offset)
		{
			UpdateOffset ("<STARTHTM>", offset + offsetHtml);
		}

		public void UpdateEndHtml(int offset)
		{
			UpdateOffset ("<ENDHTML>", offset + offsetHtml);
		}

		public string ToString()
		{
			return this.text.ToString() ;
		}

		private void UpdateOffset(string offsetname, int offset)
		{
			this.text.Replace (offsetname, string.Format ("{0,9:d9}", offset));
		}

		private ExchangeStringBuilder text = new ExchangeStringBuilder() ;
		private int offsetHtml;
	}


	public class ExchangeStringBuilder
	{
		public ExchangeStringBuilder()
		{
			theBuilder = new StringBuilder ();
		}

		public void Append(string str)
		{
			this.length += str.Length ;
			theBuilder.Append (str);
		}

		public void AppendLine(string str)
		{
			this.length += str.Length + 2 ;
			theBuilder.AppendLine(str) ;
		}

		public void Replace(string oldstring, string newstring)
		{
			theBuilder.Replace (oldstring, newstring);
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public new string ToString()
		{
			return theBuilder.ToString() ;
		}

		private StringBuilder theBuilder;
		private int length ;
	}


	public class HtmlText
	{
		public HtmlText()
		{
			this.mshtml = new MSHtmlText ();
			this.output.AppendLine ("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
			this.output.AppendLine ("<HTML><HEAD>") ;
			this.output.AppendLine ("<STYLE></STYLE>") ;
			this.output.AppendLine ("</HEAD>");
			this.output.AppendLine ("");
			this.output.AppendLine ("<BODY>");
			this.mshtml.UpdateStartFragment (this.output.Length);
		}

		public void Terminate()
		{
			this.CloseOpenTagsOnStack (true);
			mshtml.UpdateEndFragment (this.output.Length);
			this.output.AppendLine ("<BODY>");
			this.output.AppendLine ("<HTML>");
			mshtml.UpdateEndHtml (this.output.Length);
			mshtml.AddHtmlText (ref this.output);
		}

		public override string ToString()
		{
			return this.output.ToString ();
		}

		public void AppendText(string thestring)
		{
			if (precedIsItalic && !isItalic)
			{
				this.CloseTag (HtmlAttribute.Italic);
			}

			if (precedIsBold && !isBold)
			{
				this.CloseTag (HtmlAttribute.Bold);
			}

			if (precedIsUnderlined && !isUnderlined)
			{
				this.CloseTag (HtmlAttribute.Underlined);
			}

			if (precedIsStrikeout && !isStrikeout)
			{
				this.CloseTag (HtmlAttribute.Strikeout);
			}


			if (!precedIsItalic && isItalic)
			{
				this.OpenTag (HtmlAttribute.Italic);
			}


			if (!precedIsBold && isBold)
			{
				this.OpenTag (HtmlAttribute.Bold);
			}


			if (!precedIsUnderlined && isUnderlined)
			{
				this.OpenTag (HtmlAttribute.Underlined);
			}

			if (!precedIsStrikeout && isStrikeout)
			{
				this.OpenTag (HtmlAttribute.Strikeout);
			}

			if (this.precedFontFace != this.fontFace)
			{
				if (this.precedFontFace.Length != 0)
				{
					this.CloseTag (HtmlAttribute.Font);
				}

				if (this.fontFace.Length != 0)
				{
					this.OpenTag (HtmlAttribute.Font, "face", this.fontFace);
				}
			}

			if (this.precedfontSize != this.fontSize)
			{
				if (this.precedfontSize != 0)
				{
					this.CloseTag (HtmlAttribute.Font);
				}

				if (this.fontSize != 0)
				{
					this.OpenTag (HtmlAttribute.Font, "size", this.fontSize.ToString ());
				}
			}

			if (this.precedfontColor != this.fontColor)
			{
				if (this.precedfontColor.Length != 0)
				{
					this.CloseTag (HtmlAttribute.Font);
				}

				if (this.fontColor.Length != 0)
				{
					this.OpenTag (HtmlAttribute.Font, "color", this.fontColor);
				}
			}



			this.precedIsItalic = this.isItalic;
			this.precedIsBold = this.isBold;
			this.precedIsUnderlined = this.isUnderlined;
			this.precedIsStrikeout = this.isStrikeout;

			this.precedFontFace = this.fontFace;
			this.precedfontSize = this.fontSize;
			this.precedfontColor = this.fontColor;

			this.precedParagraphMode = this.paragraphMode;

			this.AppendTagsToClose (false);
			this.AppendPendingTags ();
			this.AppendTagsToOpen ();

			this.TransformSpecialHtmlChars (ref thestring);
			this.TransformToUTF8 (ref thestring);
			this.output.Append (thestring);
		}

		public void SetItalic(bool italic)
		{
			this.isItalic = italic;
		}

		public void SetBold(bool bold)
		{
			this.isBold = bold;
		}

		public void SetUnderlined(bool underlined)
		{
			this.isUnderlined = underlined;
		}

		public void SetStrikeout(bool underlined)
		{
			this.isStrikeout = underlined;
		}

		public void SetFontSize(double fontSize)
		{
			this.fontSize = (int) (fontSize / (254.0 / 72.0));
		}

		static private int[] htmlfontsizes =
			{
				8,10,12,14,18,24,36
			};

		static public int PointFontSizeToHtmlFontSize(int pointsize)
		{
			int htmlsize = 1;

			int i;

			for (i = 0; i <= HtmlText.htmlfontsizes.Length; i++)
			{
				if (pointsize <= htmlfontsizes[i])
					break;
				htmlsize++;
			}

			return htmlsize;
		}


		public void SetFontFace(string fontFace)
		{
			if (fontFace != null)
				this.fontFace = fontFace;
			else
				this.fontFace = "";
		}

		public void SetFontColor(string fontcolor)
		{
			if (fontcolor != null)
			{
				Epsitec.Common.Drawing.RichColor richcolor = Epsitec.Common.Drawing.RichColor.Parse (fontcolor);

				int r = (int) (richcolor.R * 255);
				int g = (int) (richcolor.G * 255);
				int b = (int) (richcolor.B * 255);

				this.fontColor = string.Format ("#{0,2:x2}{1,2:x2}{2,2:x2}", r, g, b);
			}
			else
			{
				this.fontColor = "";
			}
		}

		public void SetParagraph(Wrappers.JustificationMode JustificationMode)
		{
			this.paragraphMode = JustificationMode;
		}

		public string JustificationModeToHtml(Wrappers.JustificationMode JustificationMode)
		{
			string retval = null;

			switch (JustificationMode)
			{
				case Wrappers.JustificationMode.AlignLeft:
					retval = null;
					break;
				case Wrappers.JustificationMode.JustifyAlignLeft:
				case Wrappers.JustificationMode.JustifyCenter:
				case Wrappers.JustificationMode.JustifyAlignRight:
					retval = "justify";
					break;
				case Wrappers.JustificationMode.Center:
					retval = "center";
					break;
				case Wrappers.JustificationMode.AlignRight:
					retval = "right";
					break;
			}

			return retval;
		}

		public void NewParagraph(Wrappers.JustificationMode JustificationMode)
		{
			string parametername = null, parametervalue = null;

			if (JustificationMode != Wrappers.JustificationMode.Unknown)
			{
				parametername = "align";

				switch (JustificationMode)
				{
					case Wrappers.JustificationMode.AlignLeft:
						parametername = null;
						parametervalue = null;
						break;
					case Wrappers.JustificationMode.JustifyAlignLeft:
					case Wrappers.JustificationMode.JustifyCenter:
					case Wrappers.JustificationMode.JustifyAlignRight:
						parametervalue = "justify";
						break;
					case Wrappers.JustificationMode.Center:
						parametervalue = "center";
						break;
					case Wrappers.JustificationMode.AlignRight:
						parametervalue = "right";
						break;
				}
			}
			else
			{

			}

			HtmlAttributeWithParam attr = new HtmlAttributeWithParam (HtmlAttribute.Paragraph, parametername, parametervalue);

			this.output.Append (attr.AttributeToString (HtmlTagMode.Open));
			this.openTagsStack.Push (attr);
		}

		public void CloseParagraph(Wrappers.JustificationMode JustificationMode)
		{
			this.CloseOpenTagsOnStack (true);
			this.ClearTags ();
			this.output.Append ("\r\n");


			this.NewParagraph (JustificationMode);
		}

		static private string GetHtmlTag(string attribute, HtmlTagMode tagMode)
		{
			return GetHtmlTag (attribute, tagMode, null, null);
		}

		static private string GetHtmlTag(string attribute, HtmlTagMode tagMode, string parametername, string parametervalue)
		{
			string retval = "";
			string tagcontent;

			if (parametername != null)
			{
				tagcontent = string.Format ("{0} {1}=\"{2}\"", attribute, parametername, parametervalue);
			}
			else
			{
				tagcontent = attribute;
			}

			switch (tagMode)
			{
				case HtmlTagMode.Open:
					retval = string.Concat ("<", tagcontent, ">");
					break;
				case HtmlTagMode.Close:
					retval = string.Concat ("</", attribute, ">");
					break;
				case HtmlTagMode.StartOnly:
					break;
			}
			return retval;
		}

		private void ClearTags()
		{
			this.paragraphMode = Wrappers.JustificationMode.Unknown;
			this.precedParagraphMode = Wrappers.JustificationMode.Unknown;
			this.paragraphNotModeSet = true;

			this.isItalic = false;
			this.precedIsItalic = false;

			this.isBold = false;
			this.precedIsBold = false;

			this.isUnderlined = false;
			this.precedIsUnderlined = false;

			this.isStrikeout = false;
			this.precedIsStrikeout = false;

			this.fontColor = "";
			this.precedfontColor = "";

			this.fontSize = 0;
			this.precedfontSize = 0;

			this.fontFace = "";
			this.precedFontFace = "";
		}


		private void TransformToUTF8(ref string line)
		{
			UTF8Encoding utf8 = new UTF8Encoding ();
			byte[] encodedBytes = utf8.GetBytes (line);
			char[] chararray = new char[encodedBytes.Length] ;

			for (int i = 0; i < encodedBytes.Length; i++)
			{
				chararray[i] = (char) encodedBytes[i];
			}

			line = new string (chararray);
		}

		private void TransformSpecialHtmlChars(ref string line)
		{
			StringBuilder output = new StringBuilder (line);
			string oldchar;

			output.Replace ("<", "&lt;");
			output.Replace (">", "&gt;");
			oldchar = new string ((char) Epsitec.Common.Text.Unicode.Code.LineSeparator, 1);
			output.Replace (oldchar, "<br />\r\n");

			for (int i = 0; i < output.Length - 1; i++)
			{
				if (output[i] == ' ' && output[i + 1] == ' ')
				{
					output[i] = (char)0xA0 ;	// non breaking space
				}
			}

			line = output.ToString ();
		}

		private void CloseTag(HtmlAttribute closetag)
		{
			HtmlAttributeWithParam attr = new HtmlAttributeWithParam (closetag, null, null);
			this.tagsToClose.Add (attr);
		}

		private void OpenTag(HtmlAttribute opentag, string parametername, string parametervalue)
		{
			HtmlAttributeWithParam attr = new HtmlAttributeWithParam (opentag, parametername, parametervalue);
			this.tagsToOpen.Add (attr);
		}


		private void OpenTag(HtmlAttribute opentag)
		{
			this.OpenTag (opentag, null, null);
		}

		private void CloseTagsIfPossible(bool closeAlsoParagraphs)
		{
			// ferme tous les tags dans this.tagsToClose si possible

			bool found = true;

			while (found)
			{
				found = false;
				for (int i = 0; i < this.tagsToClose.Count; i++)
				{
					System.Diagnostics.Debug.Assert (this.openTagsStack.Count > 0);

					HtmlAttributeWithParam attr = this.tagsToClose[i];
					HtmlAttributeWithParam attronstack = this.openTagsStack.Peek ();

					if ((closeAlsoParagraphs || attr.Attribute () != HtmlAttribute.Paragraph) && attronstack.IsSameTag (attr))
					{
						this.output.Append (attr.AttributeToString (HtmlTagMode.Close));
						this.tagsToClose.RemoveAt (i);
						i--;
						this.openTagsStack.Pop ();
						found = true;
					}
				}
			}
		}

		private void AppendTagsToClose(bool closeAlsoParagraphs)
		{
			this.pendingAttributes.Clear ();
			while (this.tagsToClose.Count > 0)
			{
				CloseTagsIfPossible (closeAlsoParagraphs);

				if (this.tagsToClose.Count > 0)
				{
					HtmlAttributeWithParam topattribute = this.openTagsStack.Pop ();
					pendingAttributes.Push (topattribute);
					this.output.Append (topattribute.AttributeToString (HtmlTagMode.Close));
				}
			}

			this.tagsToClose.Clear ();
		}

		public void AppendPendingTags()
		{
			while (this.pendingAttributes.Count > 0)
			{
				HtmlAttributeWithParam attr;
				attr = this.pendingAttributes.Pop ();
				this.output.Append (attr.AttributeToString (HtmlTagMode.Open));
				this.openTagsStack.Push (attr);
			}
			this.pendingAttributes.Clear ();
		}

		private void CloseOpenTagsOnStack(bool closeAlsoParagraphs)
		{
			// ferme les tags rest�s ouverts
			while (this.openTagsStack.Count > 0)
			{
				HtmlAttributeWithParam attr;
				attr = openTagsStack.Pop ();
				this.output.Append (attr.AttributeToString (HtmlTagMode.Close));
			}
		}



		private void AppendTagsToOpen()
		{
			foreach (HtmlAttributeWithParam attr in this.tagsToOpen)
			{
				string tag = attr.AttributeToString (HtmlTagMode.Open);
				this.output.Append (tag);
				this.openTagsStack.Push (attr);
			}

			this.tagsToOpen.Clear ();
		}


		enum HtmlAttribute
		{
			Bold,
			Italic,
			Underlined,
			Strikeout,
			Font,
			Paragraph
		}

		enum HtmlTagMode
		{
			Open,
			Close,
			StartOnly
		}

		class HtmlAttributeWithParam
		{
			public HtmlAttributeWithParam(HtmlAttribute attr, string parametername, string parametervalue)
			{
				this.htmlattribute = attr;
				this.parametername = parametername;
				this.parametervalue = parametervalue;
			}


			public string AttributeToString(HtmlTagMode tagmode)
			{
				string retval = "";

				switch (this.htmlattribute)
				{
					case HtmlAttribute.Italic:
						retval = HtmlText.GetHtmlTag ("i", tagmode);
						break;
					case HtmlAttribute.Bold:
						retval = HtmlText.GetHtmlTag ("b", tagmode);
						break;
					case HtmlAttribute.Underlined:
						retval = HtmlText.GetHtmlTag ("u", tagmode);
						break;
					case HtmlAttribute.Strikeout:
						retval = HtmlText.GetHtmlTag ("s", tagmode);
						break;
					case HtmlAttribute.Paragraph:
						retval = HtmlText.GetHtmlTag ("p", tagmode, this.parametername, this.parametervalue);
						break;
					case HtmlAttribute.Font:
						if (parametername == "size")
							parametervalue = HtmlText.PointFontSizeToHtmlFontSize (System.Int32.Parse(this.parametervalue)).ToString();
						retval = HtmlText.GetHtmlTag ("font", tagmode, this.parametername, this.parametervalue);
						break;
				}

				return retval;
			}

			public bool IsSameTag(HtmlAttributeWithParam tag)
			{
				// si les deux tags sont de m�me type
				return this.htmlattribute == tag.htmlattribute;
			}

			public HtmlAttribute Attribute()
			{
				return htmlattribute;
			}


			private HtmlAttribute htmlattribute;
			private string parametername;
			private string parametervalue;
		}

		private Wrappers.JustificationMode paragraphMode = Wrappers.JustificationMode.Unknown;
		private Wrappers.JustificationMode precedParagraphMode = Wrappers.JustificationMode.Unknown;
		private bool paragraphNotModeSet = true;

		private bool isItalic = false;
		private bool precedIsItalic = false;

		private bool isBold = false;
		private bool precedIsBold = false;

		private bool isUnderlined = false;
		private bool precedIsUnderlined = false;

		private bool isStrikeout = false;
		private bool precedIsStrikeout = false;

		private string fontColor = "";
		private string precedfontColor = "";

		private int fontSize = 0;
		private int precedfontSize = 0;

		private string fontFace = "";
		private string precedFontFace = "";


		private ExchangeStringBuilder output = new ExchangeStringBuilder ();

		private System.Collections.Generic.Stack<HtmlAttributeWithParam> openTagsStack = new System.Collections.Generic.Stack<HtmlAttributeWithParam> ();
		private System.Collections.Generic.List<HtmlAttributeWithParam> tagsToClose = new System.Collections.Generic.List<HtmlAttributeWithParam> ();
		private System.Collections.Generic.List<HtmlAttributeWithParam> tagsToOpen = new System.Collections.Generic.List<HtmlAttributeWithParam> ();
		private System.Collections.Generic.Stack<HtmlAttributeWithParam> pendingAttributes = new System.Collections.Generic.Stack<HtmlAttributeWithParam> ();


		MSHtmlText mshtml;

		public string msHtml
		{
			get
			{
				return mshtml.ToString() ;
			}
		}
	}

	/// <summary>
	/// La classe Rosetta joue le r�le de plate-forme centrale pour la conversion
	/// de formats de texte (la "pierre de Rosette").
	/// </summary>
	public class Rosetta
	{
		public Rosetta()
		{
		}

		public string ConvertCtmlToHtml(string ctml)
		{
			//	M�thode bidon juste pour v�rifier si les tests compilent.

			return "TODO";
		}

		public static void TestCode(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			StringBuilder output = new StringBuilder ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			System.Windows.Forms.Clipboard clipboard;

			HtmlText htmlText = new HtmlText ();
			bool newParagraph = true;

			navigator.MoveTo (0, 0);

			htmlText.NewParagraph (paraWrapper.Active.JustificationMode);

			while (true)
			{
				string runText;
				int runLength = navigator.GetRunLength (100000);

				if (runLength == 0)
				{
					break;
				}

				runText = navigator.ReadText (runLength);

				// navigator.TextStyles[1].StyleProperties[1]. ;
				// navigator.TextContext.StyleList.StyleMap.GetCaption ();
				// navigator.TextContext.StyleList.

				bool finishParagraph = false;
				if (runLength == 1 && runText[0] == (char) Epsitec.Common.Text.Unicode.Code.ParagraphSeparator)
				{
					finishParagraph = true;
					// on est tomb� sur un s�parateur de paragraphe
				}
				else
				{
					htmlText.SetItalic (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic);
					htmlText.SetBold (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold);
					htmlText.SetUnderlined (textWrapper.Active.IsUnderlineDefined);
					htmlText.SetStrikeout (textWrapper.Active.IsStrikeoutDefined);

					htmlText.SetFontFace (textWrapper.Active.FontFace);
					htmlText.SetFontSize (textWrapper.Active.IsFontSizeDefined ? textWrapper.Active.FontSize : 0);
					htmlText.SetFontColor (textWrapper.Active.Color);

					htmlText.AppendText (runText);
				}

				// avance au run suivant
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caract�re afin de se trouver v�ritablement dans
				// le contexte du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1);

				if (navigator.GetRunLength (1000000) == 0)
					break; // arr�te si on est � la fin

				// recule au d�but du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 1);

				if (finishParagraph)
				{
					htmlText.CloseParagraph (paraWrapper.Active.JustificationMode);
					finishParagraph = false;
				}
			}

			htmlText.Terminate ();

			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, htmlText.msHtml);
			System.Diagnostics.Debug.WriteLine ("Code de test 1 appel�.");
		}


		#region TestCode1
		public static void TestCode1(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			System.Windows.Forms.Clipboard clipboard;
			System.Text.StringBuilder output = new System.Text.StringBuilder ();

			while (true)
			{
				int runLength = navigator.GetRunLength (1000000);

				if (runLength == 0)
				{
					break;
				}

				string runText       = navigator.ReadText (runLength);

				navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);

				Property[] runProperties = navigator.AccumulatedTextProperties;
				TextStyle[] runStyles     = navigator.TextStyles;

				System.Console.Out.WriteLine ("Run: >>{0}<< with {1} properties", runText, runProperties.Length);

				output.AppendFormat ("Run: \"{0}\"\r\n", runText);

				string text;

				foreach (Property p in runProperties)
				{

					text = p.ToString ();

					output.AppendFormat ("  {0}  {1}\r\n", p.WellKnownType, text);

					if (p.WellKnownType == Properties.WellKnownType.Font)
					{
						Properties.FontProperty fontProperty = p as Properties.FontProperty;

						string fontFace = fontProperty.FaceName;
						string fontStyle = fontProperty.StyleName;

						fontStyle = OpenType.FontCollection.GetStyleHash (fontStyle);

						System.Console.Out.WriteLine ("- Font: {0} {1}", fontFace, fontStyle);
					}
					else
					{
						//-						System.Console.Out.WriteLine ("- {0}", p.WellKnownType);
					}
				}

				//	La fa�on "haut niveau" de faire :

				if (textWrapper.Active.IsFontFaceDefined)
				{
					System.Console.Out.WriteLine ("- Font Face: {0}", textWrapper.Active.FontFace, textWrapper.Active.FontStyle, textWrapper.Active.InvertItalic ? "(italic)" : "");
				}
				if (textWrapper.Active.IsFontStyleDefined)
				{
					System.Console.Out.WriteLine ("- Font Style: {0}", textWrapper.Active.FontStyle);
				}
				if (textWrapper.Active.IsInvertItalicDefined)
				{
					System.Console.Out.WriteLine ("- Invert Italic: {0}", textWrapper.Active.InvertItalic);
				}
				if (textWrapper.Active.IsInvertBoldDefined)
				{
					System.Console.Out.WriteLine ("- Invert Bold: {0}", textWrapper.Active.InvertBold);
				}
			}

			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, output);

			System.Diagnostics.Debug.WriteLine ("Code de test 1 appel�.");
		}
		#endregion

	}
}

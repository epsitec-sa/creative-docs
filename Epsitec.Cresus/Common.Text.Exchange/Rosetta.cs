//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

namespace Epsitec.Common.Text.Exchange
{

	/// <summary>
	/// Cette classe représente un texte formaté en HTML compatible "Microsoft".
	/// </summary>
	/// 


	public class HtmlText
	{
		public HtmlText()
		{
		}

		public void Terminate()
		{
			this.CloseOpenTagsOnStack ();
		}

		public override string ToString()
		{
			return this.output.ToString ();
		}

		public void NewPara()
		{
			if ((precedParagraphMode != paragraphMode) || paragraphNotModeSet)
			{
				if (!paragraphNotModeSet)
					this.CloseTag (HtmlAttribute.Paragraph);

				this.paragraphNotModeSet = false;

				string mode = JustificationModeToHtml (paragraphMode);
				string parametername = null;

				if (mode != null)
					parametername = "align";

				this.OpenTag (HtmlAttribute.Paragraph, parametername, mode);
			}

			this.AppendTagsToOpen ();
			this.AppendPendingTags ();
		}

		public void AppendText(string thestring)
		{
			if ((precedParagraphMode != paragraphMode) || paragraphNotModeSet)
			{
				if (!paragraphNotModeSet)
					this.CloseTag (HtmlAttribute.Paragraph);

				this.paragraphNotModeSet = false ;

				string mode = JustificationModeToHtml (paragraphMode);
				string parametername = null;

				if (mode != null)
					parametername = "align";

				this.OpenTag (HtmlAttribute.Paragraph, parametername, mode);
			}

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
					this.OpenTag (HtmlAttribute.Font, "size", this.fontSize.ToString());
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

			this.TransformLineSeparators (ref thestring);
			// this.TransformParagraphSeparators (ref thestring);
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
			this.fontSize = (int)(fontSize / (254.0 / 72.0));
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
			string retval = null ;

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
			string parametername = null, parametervalue = null ;

			if (JustificationMode != Wrappers.JustificationMode.Unknown)
			{
				parametername = "align" ;

				switch (JustificationMode)
				{
					case Wrappers.JustificationMode.AlignLeft:
						parametername = null ;
						parametervalue = null ;
						break;
					case Wrappers.JustificationMode.JustifyAlignLeft:
					case Wrappers.JustificationMode.JustifyCenter:
					case Wrappers.JustificationMode.JustifyAlignRight:
						parametervalue = "justify" ;
						break;
					case Wrappers.JustificationMode.Center:
						parametervalue = "center" ;
						break ;
					case Wrappers.JustificationMode.AlignRight:
						parametervalue = "right" ;
						break ;
				}
			}
			else
			{

			}

			HtmlAttributeWithParam attr = new HtmlAttributeWithParam (HtmlAttribute.Paragraph, parametername, parametervalue);

			this.output.Append (attr.AttributeToString(HtmlTagMode.Open)) ;
			this.openTagsStack.Push (attr);
		}

		public void CloseParagraph()
		{
			this.CloseTag (HtmlAttribute.Paragraph);
			this.AppendTagsToClose (true);
			this.AppendTagsToOpen ();
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


		private void ReplaceSpecialCodes(ref string line, Epsitec.Common.Text.Unicode.Code specialCode, string htmlTag)
		{
#if false			

			// line.Remove() n'a aucun effet ??? très bizarre, bon pour l'instant on s'en fiche
			// car on fait autrement

			int index ; 
			while ((index = line.IndexOf((char)Epsitec.Common.Text.Unicode.Code.LineSeparator)) != -1)
			{
				line.Remove (index, 1);
				line.Insert (index, "<br />");
			}
#else
			System.Text.StringBuilder outputstringbuilder = new System.Text.StringBuilder ();

			string[] split = line.Split (new char[] { (char) specialCode });

			int index;
			int max = split.GetLength (0);

			for (index = 0; index < max; index++)
			{
				outputstringbuilder.Append (split[index]);
				if (index + 1 < max)
					outputstringbuilder.Append (htmlTag);
			}

			line = outputstringbuilder.ToString ();
#endif
		}

		private void TransformLineSeparators(ref string line)
		{
			// transforme tous les caractères LineSeparator en <br>
			this.ReplaceSpecialCodes (ref line, Epsitec.Common.Text.Unicode.Code.LineSeparator, "<br />\r\n");
		}


		private void TransformParagraphSeparators(ref string line)
		{
			// transforme tous les caractères LineSeparator en <br>
			this.ReplaceSpecialCodes (ref line, Epsitec.Common.Text.Unicode.Code.ParagraphSeparator, "</p>\r\n\r\n<p>");
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

		private void CloseOpenTagsOnStack()
		{
			// ferme les tags restés ouverts
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
						retval = HtmlText.GetHtmlTag ("p", tagmode, parametername, parametervalue);
						break;
					case HtmlAttribute.Font:
						retval = HtmlText.GetHtmlTag ("font", tagmode, parametername, parametervalue);
						break;
				}

				return retval;
			}

			public bool IsSameTag(HtmlAttributeWithParam tag)
			{
				// si les deux tags sont de même type
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


		private System.Text.StringBuilder output = new System.Text.StringBuilder ();

		private System.Collections.Generic.Stack<HtmlAttributeWithParam> openTagsStack = new System.Collections.Generic.Stack<HtmlAttributeWithParam> ();
		private System.Collections.Generic.List<HtmlAttributeWithParam> tagsToClose = new System.Collections.Generic.List<HtmlAttributeWithParam> ();
		private System.Collections.Generic.List<HtmlAttributeWithParam> tagsToOpen = new System.Collections.Generic.List<HtmlAttributeWithParam> ();
		private System.Collections.Generic.Stack<HtmlAttributeWithParam> pendingAttributes = new System.Collections.Generic.Stack<HtmlAttributeWithParam> ();

	}

	/// <summary>
	/// La classe Rosetta joue le rôle de plate-forme centrale pour la conversion
	/// de formats de texte (la "pierre de Rosette").
	/// </summary>
	public class Rosetta
	{
		public Rosetta()
		{
		}

		public string ConvertCtmlToHtml(string ctml)
		{
			//	Méthode bidon juste pour vérifier si les tests compilent.

			return "TODO";
		}

		public static void TestCode(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			System.Text.StringBuilder output = new System.Text.StringBuilder ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			System.Windows.Forms.Clipboard clipboard;

			HtmlText htmlText = new HtmlText ();
			bool newParagraph = true;

			navigator.MoveTo (0, 0);
			
			htmlText.SetParagraph (paraWrapper.Defined.JustificationMode);
			htmlText.AppendText ("");

			while (true)
			{
				string runText ;
				int runLength = navigator.GetRunLength (1000000);

				if (runLength == 0)
				{
					break;
				}

				runText = navigator.ReadText (runLength);

				bool finishParagraph = false;
				if (runLength == 1 && runText[0] == (char) Epsitec.Common.Text.Unicode.Code.ParagraphSeparator)
				{
					//		htmlText.SetParagraph (paraWrapper.Defined.JustificationMode);
					// on est tombé sur un séparateur de paragraphe
					htmlText.CloseParagraph ();
					finishParagraph = true;
				}
				else
				{
					htmlText.SetItalic (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic);
					htmlText.SetBold (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold);
					htmlText.SetUnderlined (textWrapper.Defined.IsUnderlineDefined);
					htmlText.SetStrikeout (textWrapper.Defined.IsStrikeoutDefined);

					htmlText.SetFontFace (textWrapper.Defined.FontFace);
					htmlText.SetFontSize (textWrapper.Defined.IsFontSizeDefined ? textWrapper.Defined.FontSize : 0);
					htmlText.SetFontColor (textWrapper.Defined.Color);

					htmlText.AppendText (runText);
				}

#if false

				{
					// on vient de lire un caractère "fin de paragraphe"
					runLength = navigator.GetRunLength (1000000);
					runText = navigator.ReadText (runLength);
					if (newParagraph)
					{
						Wrappers.JustificationMode JustificationMode = paraWrapper.Defined.JustificationMode;
						// = paraWrapper.Defined.IsJustificationModeDefined && 

						htmlText.NewParagraph (paraWrapper.Defined.JustificationMode);
					}
				}
				else
				{
					htmlText.AppendText (runText);
				}
#endif

			

				// avance au run suivant
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte su run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1);

				if (navigator.GetRunLength (1000000) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 1);

				if (finishParagraph)
				{
					htmlText.SetParagraph (paraWrapper.Defined.JustificationMode);
					htmlText.NewPara ();
				}
			}

			htmlText.Terminate ();

			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Text, htmlText);
			System.Diagnostics.Debug.WriteLine ("Code de test 1 appelé.");
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

				//	La façon "haut niveau" de faire :

				if (textWrapper.Defined.IsFontFaceDefined)
				{
					System.Console.Out.WriteLine ("- Font Face: {0}", textWrapper.Defined.FontFace, textWrapper.Defined.FontStyle, textWrapper.Defined.InvertItalic ? "(italic)" : "");
				}
				if (textWrapper.Defined.IsFontStyleDefined)
				{
					System.Console.Out.WriteLine ("- Font Style: {0}", textWrapper.Defined.FontStyle);
				}
				if (textWrapper.Defined.IsInvertItalicDefined)
				{
					System.Console.Out.WriteLine ("- Invert Italic: {0}", textWrapper.Defined.InvertItalic);
				}
				if (textWrapper.Defined.IsInvertBoldDefined)
				{
					System.Console.Out.WriteLine ("- Invert Bold: {0}", textWrapper.Defined.InvertBold);
				}
			}

			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Text, output);

			System.Diagnostics.Debug.WriteLine ("Code de test 1 appelé.");
		}
		#endregion

	}
}

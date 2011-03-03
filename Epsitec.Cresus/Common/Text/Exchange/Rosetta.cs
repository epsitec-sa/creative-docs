//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

#define USE_SPAN
#define SIMPLECOPYPASTE

using System.Text;
using System.Collections.Generic;
using Epsitec.Common.Text.Exchange.HtmlParser;


// Il faudrait trouver comment accéder à Epsitec.Common.Document.Modifier.FontSizeScale (254.0 / 72.0) 
//


namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// La classe Rosetta joue le rôle de plate-forme centrale pour la conversion
	/// de formats de texte (la "pierre de Rosette").
	/// </summary>
	public class Rosetta
	{
		public Rosetta()
		{
		}

		public static PasteMode pasteMode = PasteMode.KeepSource;

		public static void TogglePasteMode()
		{

			if (pasteMode == PasteMode.KeepTextOnly)
				Rosetta.pasteMode = PasteMode.Unknown;

			Rosetta.pasteMode++;

			string text = string.Format("Mode paste: {0}", Rosetta.pasteMode) ;
			System.Windows.Forms.MessageBox.Show(text, "Debug: Mode pour paste");
		}


		public static void PasteHtmlText(TextStory story, TextNavigator navigator)
		{
			string s = NativeHtmlClipboardReader.Read ();

			if (s.Length == 0)
				return;

			HtmlDocument thehtmldoc = new HtmlDocument (s, true);

			using (CopyPasteContext cpContext = new CopyPasteContext (story, navigator))
			{
				HtmlToTextWriter theWriter = new HtmlToTextWriter (thehtmldoc, cpContext, Rosetta.pasteMode);
				theWriter.ProcessIt ();
			}
		}

		public static void PasteNativeText(TextStory story, TextNavigator navigator, ClipboardData clipboard)
		{
			string text = clipboard.GetFormattedText ();

			if (string.IsNullOrEmpty (text))
			{
				return;
			}

			using (CopyPasteContext cpContext = new CopyPasteContext (story, navigator))
			{
				NativeToTextWriter theWriter = new NativeToTextWriter (text, cpContext, Rosetta.pasteMode);
				theWriter.ProcessIt ();
			}
		}


		public static void CopyText(TextStory story, TextNavigator usernavigator, ClipboardData clipboard)
		{
			HtmlTextOut htmlText = new HtmlTextOut ();
			NativeTextOut nativeText = new NativeTextOut ();
			string rawText = string.Empty ;

			using (CopyPasteContext cpContext = new CopyPasteContext (story))
			{
				Rosetta.CopyHtmlText (story, usernavigator, htmlText, cpContext);
				Rosetta.CopyNativeText (story, usernavigator, nativeText, cpContext);
				
				rawText = Rosetta.CopyRawText (story, usernavigator);

				clipboard.Add (System.Windows.Forms.DataFormats.Text, rawText);
				clipboard.Add (System.Windows.Forms.DataFormats.Html, htmlText.HtmlStream);
				clipboard.Add (Internal.FormattedText.ClipboardFormat.Name, nativeText.FormattedText);
			}
		}

		private static void CopyHtmlText(TextStory story, TextNavigator usernavigator, HtmlTextOut htmlText, CopyPasteContext cpContext)
		{
			if (!usernavigator.HasSelection)
				return;

			int[] selectedPositions = usernavigator.GetAdjustedSelectionCursorPositions ();

			// ne gère pas les sélections disjointes pour le moment
			System.Diagnostics.Debug.Assert (selectedPositions.Length == 2);

			int selectionLength = selectedPositions[1] - selectedPositions[0];
			int selectionStart = selectedPositions[0];
			int selectionEnd = selectedPositions[1];
			int currentPosition = selectionStart;

			cpContext.Navigator.MoveTo (selectionStart, 0);

			htmlText.NewParagraph (cpContext.ParaWrapper.Active.JustificationMode);

			while (true)
			{
				string runText;
				int runLength = cpContext.Navigator.GetRunLength (selectionLength);

				if (currentPosition + runLength > selectionEnd)
					runLength = selectionEnd - currentPosition ;

				if (runLength <= 0)
				{
					break;
				}

				runText = cpContext.Navigator.ReadText (runLength);
				currentPosition += runLength;

				// navigator.TextStyles[1].StyleProperties[1]. ;
				// navigator.TextContext.StyleList.StyleMap.GetCaption ();
				// navigator.TextContext.StyleList.

				bool finishParagraph = false;
				if (runLength == 1 && runText[0] == (char) Unicode.Code.ParagraphSeparator)
				{
					finishParagraph = true;
					// on est tombé sur un séparateur de paragraphe
				}
				else
				{
#if false	// n'utilise plus les "invertxxx" mais utilise ce qui est réellement affiché
					htmlText.SetItalic (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic);
					htmlText.SetBold (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold);
#else
					htmlText.SetItalic (Rosetta.IsItalic (cpContext.Navigator));
					htmlText.SetBold (Rosetta.IsBold (cpContext.Navigator));
#endif
					htmlText.SetUnderline (cpContext.TextWrapper.Active.IsUnderlineDefined);
					htmlText.SetStrikeout (cpContext.TextWrapper.Active.IsStrikeoutDefined);

					SimpleXScript xscript = GetSimpleXScript (cpContext.TextWrapper);
					htmlText.SetSimpleXScript (xscript);

					htmlText.SetFontFace (cpContext.TextWrapper.Active.FontFace);
					htmlText.SetFontSize (cpContext.TextWrapper.Active.IsFontSizeDefined ? cpContext.TextWrapper.Active.FontSize : 0);
					htmlText.SetFontColor (cpContext.TextWrapper.Active.Color);

					htmlText.AppendText (runText);
				}

				// avance au run suivant
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, 1);

				if (cpContext.Navigator.GetRunLength (1) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);

				if (finishParagraph)
				{
					htmlText.CloseParagraph (cpContext.ParaWrapper.Active.JustificationMode);
					finishParagraph = false;
				}
			}

			htmlText.Terminate ();
		}


		private static void CopyNativeText(TextStory story, TextNavigator usernavigator, NativeTextOut nativeText, CopyPasteContext cpContext)
		{
			if (!usernavigator.HasSelection)
				return;

			int[] selectedPositions = usernavigator.GetAdjustedSelectionCursorPositions ();

			// ne gère pas les sélections disjointes pour le moment
			System.Diagnostics.Debug.Assert (selectedPositions.Length == 2);

			// ATTENTION: BUG: lorsqu'on sélectionne une ligne avec des puces
			// la longueur (selectionLength) est trop grande
			int selectionLength = selectedPositions[1] - selectedPositions[0];
			int selectionStart = selectedPositions[0];
			int selectionEnd = selectedPositions[1];
			int currentPosition = selectionStart;
			cpContext.Navigator.MoveTo (selectionStart, 0);

			NativeConverter converter = new NativeConverter (cpContext, PasteMode.KeepTextOnly);
			bool isOnNewParagraph = true;

			while (true)
			{
				string runText;
				int runLength = cpContext.Navigator.GetRunLength (selectionLength);

				if (currentPosition + runLength > selectionEnd)
					runLength = selectionEnd - currentPosition ;

				if (runLength <= 0 && !isOnNewParagraph)
				{
					break;
				}

				runText =  cpContext.Navigator.ReadText (runLength);
				currentPosition += runLength;

				bool paragraphSep = false;
				if (runLength == 1 && runText[0] == (char) Unicode.Code.ParagraphSeparator)
				{
					// on est tombé sur un séparateur de paragraphe
					paragraphSep = true;
				}

				//NativeConverter converter = new NativeConverter (cpContext, PasteMode.KeepTextOnly);
				string runattributes = converter.GetDefinedString (isOnNewParagraph, paragraphSep);

				isOnNewParagraph = paragraphSep;

				nativeText.AppendTextLine (runattributes);
				
				if (!paragraphSep)
					nativeText.AppendTextLine (runText);
			

				// avance au run suivant
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);

				if (cpContext.Navigator.GetRunLength (1) == 0)
					break; // arrête si on est à la fin

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, 1);

				// recule au début du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			}

			List<string> textStyles = converter.GetStyleStrings ();

			nativeText.AppendStyleLine ("{");

			foreach (string stylestring in textStyles)
			{
				nativeText.AppendStyleLine (stylestring);
			}

			nativeText.AppendStyleLine ("}");

		}

		private static string CopyRawText(TextStory story, TextNavigator usernavigator)
		{
			string[] texts = usernavigator.GetSelectedTexts ();

			if (texts == null || texts.Length == 0)
				return "" ;// false;

			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			foreach (string part in texts)
			{
				builder.Append (part);
			}

			string text = builder.ToString ();

			text = text.Replace ("\u2029", "\r\n");
			text = text.Replace ("\u2028", "\r\n");

			return text;
		}




		#region SimpleXscript
		public enum SimpleXScript
		{
			Superscript,
			Subscript,
			Normalscript
		} ;

		static private SimpleXScript GetSimpleXScript(Wrappers.TextWrapper wrapper)
		{

			if (wrapper.Active.IsXscriptDefined)
			{
				if (wrapper.Active.Xscript.Offset > 0.0)
					return SimpleXScript.Superscript;
				if (wrapper.Active.Xscript.Offset < 0.0)
					return SimpleXScript.Subscript;
				else
					return SimpleXScript.Normalscript;
			}
			else
			{
				return SimpleXScript.Normalscript;
			}
		}
		#endregion		

		#region Fonctions de test provisoire

		// parcours du text
		public static void TestCode(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			System.Text.StringBuilder output = new System.Text.StringBuilder ();

			story.DisableOpletQueue ();

			while (true)
			{
				int runLength = navigator.GetRunLength (1000000);

				if (runLength == 0)
				{
					break;
				}

				string runText       = navigator.ReadText (runLength);

				navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);


				//	La façon "haut niveau" de faire :

				TextContext context = story.TextContext;

				foreach (TextStyle style in navigator.TextStyles)
				{
					if (style.TextStyleClass == TextStyleClass.Paragraph || style.TextStyleClass == TextStyleClass.Text)
					{
						string s = context.StyleList.StyleMap.GetCaption (style);
					}
				}

				bool b = textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic;
				b = textWrapper.Active.IsInvertItalicDefined && textWrapper.Active.InvertItalic;

				Property[] props;
				Property[] fonts;

				props = navigator.AccumulatedTextProperties;
				fonts = Property.Filter (props, Properties.WellKnownType.Font);

				if (fonts.Length > 0)
				{
					System.Diagnostics.Debug.Assert (fonts.Length == 1);
					Properties.FontProperty fontProperty;
					string fontStyle;
					fontProperty = fonts[0] as Properties.FontProperty;
					fontStyle = OpenType.FontCollection.GetStyleHash (fontProperty.StyleName);
				}

				// navigator.SetTextStyles(
			}

			story.EnableOpletQueue ();
		}


		// test de création de styles

		public static void TestCode6(TextStory story, TextNavigator navigator)
		{
			Epsitec.Common.Text.TextContext context = story.TextContext;
			TextStyle[] styles = context.StyleList.StyleMap.GetSortedStyles ();

			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			System.Collections.ArrayList parents = new System.Collections.ArrayList ();
			parents.Add (context.DefaultParagraphStyle);

			// this.document.Modifier.OpletQueueBeginAction ((this.category == StyleCategory.Paragraph) ? Res.Strings.Action.AggregateNewParagraph : Res.Strings.Action.AggregateNewCharacter);

			Text.TextStyle style = context.StyleList.NewTextStyle (/*this.document.Modifier.OpletQueue*/ null, null, Common.Text.TextStyleClass.Paragraph, properties, parents);

			// obtient le nombre de styles de paragraphe
#if false
			int rank = styles.Length + 1;	// ne fonctionne pas car il y a des "trous" !??
#else
			int rank = 0;
			foreach (TextStyle thestyle in context.StyleList.StyleMap.GetSortedStyles ())
			{
				string s = thestyle.Name;
				s = context.StyleList.StyleMap.GetCaption (thestyle);
				if (thestyle.TextStyleClass == TextStyleClass.Paragraph)
					rank++;
			}

#endif
			context.StyleList.StyleMap.SetCaption (null /*this.document.Modifier.OpletQueue*/, style, "Style créé par MW");
			context.StyleList.StyleMap.SetRank (null, style, rank);

			//this.document.SetSelectedTextStyle (this.category, rank);

			//this.document.Modifier.OpletQueueValidateAction ();
			//this.document.Notifier.NotifyTextStyleListChanged ();
			//this.document.IsDirtySerialize = true;

			//this.oneShootSelectName = true;
			//this.SetDirtyContent ();
			return;
		}


		// Test pour parcourir tous les styles existants

		public static void TestCode4(TextStory story, TextNavigator navigator)
		{
			Epsitec.Common.Text.TextContext context = story.TextContext;

			TextStyle[] styles = context.StyleList.StyleMap.GetSortedStyles ();

			TextStyle thenewstyle = null ;
			foreach (TextStyle thestyle in styles)
			{
				string s = thestyle.Name;
				s = context.StyleList.StyleMap.GetCaption (thestyle);

				bool b = false;

				if (b)
				{
					thenewstyle = thestyle ;
					break;
				}
			}

			TextStyle[] newstyles = new TextStyle[1] ;

			newstyles[0] = thenewstyle ;
			navigator.SetTextStyles(newstyles) ;

			navigator.Insert ("abc");

			return;
		}


		private static Properties.FontProperty GetFontProperty(TextNavigator navigator)
		{
			Property[] props;
			Property[] fonts;
			Properties.FontProperty fontProperty = null;

			props = navigator.AccumulatedTextProperties;
			fonts = Property.Filter (props, Properties.WellKnownType.Font);

			if (fonts.Length > 0)
			{
				System.Diagnostics.Debug.Assert (fonts.Length == 1);
				fontProperty = fonts[0] as Properties.FontProperty;
			}

			return fontProperty;
		}

		private static string GetFontStyleString(TextNavigator navigator)
		{
			string fontStyle = null;

			Properties.FontProperty fontProperty = Rosetta.GetFontProperty (navigator);

			if (fontProperty != null)
				fontStyle = OpenType.FontCollection.GetStyleHash (fontProperty.StyleName);

			return fontStyle;
		}

		private static bool IsItalic(TextNavigator navigator)
		{
			string fontStyle = GetFontStyleString(navigator) ;

			return fontStyle != null && fontStyle == "Italic";
		}


		private static bool IsBold(TextNavigator navigator)
		{
			string fontStyle = GetFontStyleString (navigator);

			return fontStyle != null && fontStyle == "Bold";
		}

		#endregion

	}
}

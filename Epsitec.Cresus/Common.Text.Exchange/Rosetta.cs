//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

#define USE_SPAN

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

		public static void PasteHtmlText(TextStory story, TextNavigator navigator)
		{
			string s = NativeHtmlClipboardReader.ReadClipBoardHtml ();

			if (s.Length == 0)
				return;

			HtmlDocument thehtmldoc = new HtmlDocument (s, true);

			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			HtmlToTextWriter theWriter = new HtmlToTextWriter (thehtmldoc, textWrapper, paraWrapper, navigator, PasteMode.InsertRaw);
			theWriter.ProcessIt ();
		}

		public static void PasteNativeText(TextStory story, TextNavigator navigator)
		{
			System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject ();
			EpsitecFormat efmt = ido.GetData (Common.Text.Exchange.EpsitecFormat.Format.Name, false) as EpsitecFormat;

			if (efmt.String.Length == 0)
				return;

#if false
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);
#else
			CopyPasteContect cpContext = new CopyPasteContect (story, navigator);
#endif

			NativeToTextWriter theWriter = new NativeToTextWriter (efmt.String, cpContext, PasteMode.InsertAll);
			theWriter.ProcessIt ();
			
		}


		public static void CopyText(TextStory story, TextNavigator usernavigator)
		{
			HtmlTextOut htmlText = new HtmlTextOut ();
			NativeTextOut nativeText = new NativeTextOut ();
			string rawText = string.Empty ;

			Rosetta.CopyHtmlText (story, usernavigator, htmlText);
			Rosetta.CopyNativeText (story, usernavigator, nativeText);
			rawText = Rosetta.CopyRawText(story, usernavigator) ;

			System.Windows.Forms.IDataObject od = new System.Windows.Forms.DataObject ();
			od.SetData (System.Windows.Forms.DataFormats.Text, true, rawText);
			od.SetData (System.Windows.Forms.DataFormats.Html, true, htmlText.HtmlStream);

			EpsitecFormat efmt = new EpsitecFormat (nativeText.ToString());
			od.SetData (EpsitecFormat.Format.Name, true, efmt);

			System.Windows.Forms.Clipboard.SetDataObject (od, true);
		}

		private static void CopyHtmlText(TextStory story, TextNavigator usernavigator, HtmlTextOut htmlText)
		{
			if (!usernavigator.IsSelectionActive)
				return;

			TextNavigator navigator = new TextNavigator (story);

			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			System.Windows.Forms.Clipboard clipboard;

			bool newParagraph = true;

			int[] selectedPositions = usernavigator.GetAdjustedSelectionCursorPositions ();

			// ne gère pas les sélections disjointes pour le moment
			System.Diagnostics.Debug.Assert (selectedPositions.Length == 2);

			int selectionLength = selectedPositions[1] - selectedPositions[0];
			int selectionStart = selectedPositions[0];
			int selectionEnd = selectedPositions[1];
			int currentPosition = selectionStart;
			navigator.MoveTo (selectionStart, 0);

			htmlText.NewParagraph (paraWrapper.Active.JustificationMode);

			while (true)
			{
				string runText;
				int runLength = navigator.GetRunLength (selectionLength);

				if (currentPosition + runLength > selectionEnd)
					runLength = selectionEnd - currentPosition ;

				if (runLength <= 0)
				{
					break;
				}

				runText = navigator.ReadText (runLength);
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
					htmlText.SetItalic (Rosetta.IsItalic(navigator));
					htmlText.SetBold (Rosetta.IsBold (navigator));
#endif
					htmlText.SetUnderlined (textWrapper.Active.IsUnderlineDefined);
					htmlText.SetStrikeout (textWrapper.Active.IsStrikeoutDefined);

					SimpleXScript xscript = GetSimpleXScript (textWrapper);
					htmlText.SetSimpleXScript (xscript);

					htmlText.SetFontFace (textWrapper.Active.FontFace);
					htmlText.SetFontSize (textWrapper.Active.IsFontSizeDefined ? textWrapper.Active.FontSize : 0);
					htmlText.SetFontColor (textWrapper.Active.Color);

					htmlText.AppendText (runText);
				}

				// avance au run suivant
				navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte du run
				navigator.MoveTo (TextNavigator.Target.CharacterNext, 1);

				if (navigator.GetRunLength (1) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);

				if (finishParagraph)
				{
					htmlText.CloseParagraph (paraWrapper.Active.JustificationMode);
					finishParagraph = false;
				}
			}

			htmlText.Terminate ();
		}


		public static void CopyNativeText(TextStory story, TextNavigator usernavigator, NativeTextOut nativeText)
		{
			if (!usernavigator.IsSelectionActive)
				return;

			CopyPasteContect cpContext = new CopyPasteContect (story);

			int[] selectedPositions = usernavigator.GetAdjustedSelectionCursorPositions ();

			// ne gère pas les sélections disjointes pour le moment
			System.Diagnostics.Debug.Assert (selectedPositions.Length == 2);

			// ATTENTION: BUG: lorsqu'on sélectionne une ligne avec des puces
			// les la longueur (selectionLength) est trop grande
			int selectionLength = selectedPositions[1] - selectedPositions[0];
			int selectionStart = selectedPositions[0];
			int selectionEnd = selectedPositions[1];
			int currentPosition = selectionStart;
			cpContext.Navigator.MoveTo (selectionStart, 0);

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

				runText =  cpContext.Navigator.ReadText (runLength);
				currentPosition += runLength;

				bool paragraphSep = false;
				if (runLength == 1 && runText[0] == (char) Unicode.Code.ParagraphSeparator)
				{
					// on est tombé sur un séparateur de paragraphe
					paragraphSep = true;
				}

				NativeConverter converter = new NativeConverter (cpContext);
				string runattributes = converter.GetDefinedString (paragraphSep);

				nativeText.AppendTextLine (runattributes);
				
				if (!paragraphSep)
					nativeText.AppendTextLine (runText);
			

				// avance au run suivant
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterNext, 1);

				if (cpContext.Navigator.GetRunLength (1) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				cpContext.Navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			}

		}

		public static string CopyRawText(TextStory story, TextNavigator usernavigator)
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


		public string ConvertCtmlToHtml(string ctml)
		{
			//	Méthode bidon juste pour vérifier si les tests compilent.

			return "TODO";
		}

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

				bool b = textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic ;
				b = textWrapper.Active.IsInvertItalicDefined && textWrapper.Active.InvertItalic ;

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
					fontStyle = OpenType.FontCollection.GetStyleHash(fontProperty.StyleName);
				}

					// navigator.SetTextStyles(
			}

			story.EnableOpletQueue ();
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

			int i;

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

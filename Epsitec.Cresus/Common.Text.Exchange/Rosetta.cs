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

		public string ConvertCtmlToHtml(string ctml)
		{
			//	Méthode bidon juste pour vérifier si les tests compilent.

			return "TODO";
		}

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



		public static void TestCode2(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			StringBuilder output = new StringBuilder ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			System.Windows.Forms.Clipboard clipboard;

			HtmlTextOut htmlText = new HtmlTextOut ();
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
					// on est tombé sur un séparateur de paragraphe
				}
				else
				{
					htmlText.SetItalic (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic);
					htmlText.SetBold (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold);
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
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1);

				if (navigator.GetRunLength (1000000) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 1);

				if (finishParagraph)
				{
					htmlText.CloseParagraph (paraWrapper.Active.JustificationMode);
					finishParagraph = false;
				}
			}

			htmlText.Terminate ();

			System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject ();
			data.SetData (System.Windows.Forms.DataFormats.Text, true, htmlText.rawText);
			data.SetData (System.Windows.Forms.DataFormats.Html, true, htmlText.HtmlStream);
			System.Windows.Forms.Clipboard.SetDataObject (data, true);

			System.Diagnostics.Debug.WriteLine ("Code de test 1 appelé.");
		}
		
		
		public static void TestCode1(TextStory story, TextNavigator navigator)
		{
			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

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

				if (textWrapper.Active.IsFontFaceDefined)
				{
					System.Console.Out.WriteLine ("- Font Face: {0}", textWrapper.Active.FontFace, textWrapper.Active.FontStyle, textWrapper.Active.InvertItalic ? "(italic)" : string.Empty);
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

			story.EnableOpletQueue ();
			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, output);

			System.Diagnostics.Debug.WriteLine ("Code de test 1 appelé.");
		}
		

		public static void TestCode(TextStory story, TextNavigator navigator)
		{
			string s = NativeHtmlClipboardReader.ReadClipBoardHtml ();

			if (s.Length == 0)
				return;

			HtmlDocument thehtmldoc = new HtmlDocument (s, true);

			Wrappers.TextWrapper textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			HtmlToTextWriter theWriter = new HtmlToTextWriter (thehtmldoc, textWrapper, paraWrapper, navigator);
			theWriter.ProcessIt ();

#if false
			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.FontFace = "Times New Roman";
			textWrapper.Defined.FontStyle = "Regular";
			textWrapper.Defined.FontSize = 12 * HtmlText.FontSizeFactor;
			textWrapper.Defined.Units = Properties.SizeUnits.Points;
			textWrapper.ResumeSynchronizations ();

			navigator.Insert ("Hello you world !");
			navigator.Insert (Unicode.Code.ParagraphSeparator);
			navigator.Insert ("The End.");
			navigator.MoveTo (TextNavigator.Target.LineStart, 0);
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			navigator.MoveTo (TextNavigator.Target.WordStart, 2);

			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.InvertItalic = true;
			textWrapper.ResumeSynchronizations ();

			navigator.Insert ("wonderful ");

			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.ClearInvertItalic ();
			textWrapper.ResumeSynchronizations ();

			navigator.Insert ("and beautiful ");

			navigator.MoveTo (0, 0);
#endif

		}

	}
}

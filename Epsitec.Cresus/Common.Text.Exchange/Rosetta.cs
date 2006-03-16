//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

namespace Epsitec.Common.Text.Exchange
{

	/// <summary>
	/// Cette classe représente un texte formaté en HTML compatible "Microsoft".
	/// </summary>
	/// 

	public class HtmlRun
	{
		private string runText;
		private bool italic;
		private bool bold;

		public HtmlRun()
		{
		}


	}

	public class HtmlText
	{
		public HtmlText()
		{
			this.output.Append ("<p>");
			this.openTagsStack.Push (HtmlAttribute.Paragraph);
		}

		public void Terminate()
		{
			CloseOpenTagsOnStack() ;
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

			precedIsItalic = isItalic;
			precedIsBold = isBold;
			precedIsUnderlined = isUnderlined;

			this.AppendTagsToClose ();
			this.AppendTagsToOpen ();

			this.TransformLineSeparators (ref thestring);
			this.TransformParagraphSeparators (ref thestring);
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

		public void SetFont(string fontName, int fontSize)
		{
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
			System.Text.StringBuilder output = new System.Text.StringBuilder ();

			string[] split = line.Split (new char[] { (char) specialCode });

			int index;
			int start = 0;
			int max = split.GetLength (0);

			for (index = 0; index < max; index++)
			{
				output.Append (split[index]);
				if (index + 1 < max)
					output.Append (htmlTag);
			}

			line = output.ToString ();
#endif
		}

		private void TransformLineSeparators(ref string line)
		{
			// transforme tous les caractères LineSeparator en <br>
			ReplaceSpecialCodes (ref line, Epsitec.Common.Text.Unicode.Code.LineSeparator, "<br />");
		}


		private void TransformParagraphSeparators(ref string line)
		{
			// transforme tous les caractères LineSeparator en <br>
			ReplaceSpecialCodes (ref line, Epsitec.Common.Text.Unicode.Code.ParagraphSeparator, "</p>\r\n<p>");
		}


		private string GetHtmlTag(string attribute, HtmlTagMode tagMode)
		{
			string retval = "";
			switch (tagMode)
			{
				case HtmlTagMode.Open:
					retval = "<" + attribute + ">";
					break;
				case HtmlTagMode.Close:
					retval = "</" + attribute + ">";
					break;
				case HtmlTagMode.StartOnly:
					break;
			}
			return retval;
		}

		private string AttributeToString(HtmlAttribute attribute, HtmlTagMode tagmode, string parameter)
		{
			string retval = "";

			switch (attribute)
			{
				case HtmlAttribute.Italic:
					retval = this.GetHtmlTag ("i", tagmode);
					break;
				case HtmlAttribute.Bold:
					retval = this.GetHtmlTag ("b", tagmode);
					break;
				case HtmlAttribute.Underlined:
					retval = this.GetHtmlTag ("u", tagmode);
					break;

			}

			return retval;
		}

		private void CloseTag(HtmlAttribute closetag)
		{
			this.tagsToClose.Add (closetag);
		}

		private void OpenTag(HtmlAttribute opentag)
		{
			this.tagsToOpen.Add (opentag);
		}


		private void CloseTagsIfPossible()
		{
			// ferme tous les tags dans this.tagsToClose si possible

			bool found = true;

			while (found)
			{
				found = false;
				for (int i = 0; i < this.tagsToClose.Count; i++)
				{
					HtmlAttribute attr = (HtmlAttribute) this.tagsToClose[i];
				//	System.Diagnostics.Debug.Assert (this.openTagsStack.Count > 0);
					if ((HtmlAttribute) this.openTagsStack.Peek () == attr)
					{
						this.output.Append (this.AttributeToString (attr, HtmlTagMode.Close, null));
						this.tagsToClose.RemoveAt (i);
						i--;
						this.openTagsStack.Pop ();
						found = true;
					}
				}
			}
		}

		private void AppendTagsToClose()
		{
			System.Collections.Stack tmpattributes = new System.Collections.Stack() ;

			while (this.tagsToClose.Count > 0)
			{
				CloseTagsIfPossible ();

				if (this.tagsToClose.Count > 0)
				{
					HtmlAttribute topattribute = (HtmlAttribute) this.openTagsStack.Pop ();
					tmpattributes.Push (topattribute);
					this.output.Append (this.AttributeToString (topattribute, HtmlTagMode.Close, null));
				}
			}

			while (tmpattributes.Count > 0)
			{
				HtmlAttribute attr;
				attr = (HtmlAttribute) tmpattributes.Pop();
				this.output.Append (this.AttributeToString (attr, HtmlTagMode.Open, null));
				this.openTagsStack.Push (attr);
			}

			this.tagsToClose.Clear ();
		}

		private void CloseOpenTagsOnStack()
		{
			// ferme les tags restés ouverts
			while (this.openTagsStack.Count > 0)
			{
				HtmlAttribute attr;
				attr = (HtmlAttribute) openTagsStack.Pop ();
				this.output.Append (this.AttributeToString (attr, HtmlTagMode.Close, null));
			}
		}

		private void AppendTagsToOpen()
		{
			foreach (HtmlAttribute attr in this.tagsToOpen)
			{
				string tag = AttributeToString (attr, HtmlTagMode.Open, null);
				this.output.Append (tag);
				//this.openTagsStack.Push (attr);
			}

			this.tagsToOpen.Clear ();
		}


		enum HtmlAttribute
		{
			Bold,
			Italic,
			Underlined,
			Font,
			Paragraph
		}

		enum HtmlTagMode
		{
			Open,
			Close,
			StartOnly
		}

		private bool isItalic = false;
		private bool isBold = false;
		private bool isUnderlined = false;

		private bool precedIsItalic = false;
		private bool precedIsBold = false;
		private bool precedIsUnderlined = false;

		private HtmlAttribute lastAttributeSet;

		private System.Collections.ArrayList runList;

		private System.Text.StringBuilder output = new System.Text.StringBuilder ();

		private System.Collections.Stack openTagsStack = new System.Collections.Stack ();
		private System.Collections.ArrayList tagsToClose = new System.Collections.ArrayList ();
		private System.Collections.ArrayList tagsToOpen = new System.Collections.ArrayList ();

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

			navigator.MoveTo (0, 0);

			while (true)
			{
				int runLength = navigator.GetRunLength (1000000);

				if (runLength == 0)
				{
					break;
				}

				if (textWrapper.Defined.IsFontFaceDefined)
				{
					System.Console.Out.WriteLine ("- Font Face: {0}", textWrapper.Defined.FontFace, textWrapper.Defined.FontStyle, textWrapper.Defined.InvertItalic ? "(italic)" : "");
				}
				if (textWrapper.Defined.IsFontStyleDefined)
				{
					System.Console.Out.WriteLine ("- Font Style: {0}", textWrapper.Defined.FontStyle);
				}


				htmlText.SetItalic (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic);
				htmlText.SetBold (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold);
				htmlText.SetUnderlined (textWrapper.Defined.IsUnderlineDefined);//  && textWrapper.Defined.InvertBold);
				

				int i;

				//output.Append(navigator.ReadText (runLength) );
				htmlText.AppendText (navigator.ReadText (runLength));

				// avance au run suivant
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, runLength);

				// avance encore d'un seul caractère afin de se trouver véritablement dans
				// le contexte su run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1);

				if (navigator.GetRunLength (1000000) == 0)
					break; // arrête si on est à la fin

				// recule au début du run
				navigator.MoveTo (Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 1);

			}

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

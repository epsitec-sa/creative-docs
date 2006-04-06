//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

#define USE_SPAN

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

		public byte[] GetBytes()
		{
			return System.Text.Encoding.UTF8.GetBytes (this.text.ToString ());
		}

		private void UpdateOffset(string offsetname, int offset)
		{
			this.text.Replace (offsetname, string.Format ("{0,9:d9}", offset));
		}

		private StringBuilder text = new StringBuilder() ;
		private int offsetHtml;
	}


	public class ExchangeStringBuilder
	{
		public ExchangeStringBuilder()
		{
			theBuilder = new System.Collections.Generic.List<byte> ();
		}

		public void Append(string str)
		{
			UTF8Encoding utf8 = new UTF8Encoding ();
			byte[] encodedBytes = utf8.GetBytes (str);

			for (int i = 0; i < encodedBytes.Length; i++)
			{
				this.theBuilder.Add(encodedBytes[i]);
			}
		}

		public void AppendLine(string str)
		{
			this.length += str.Length + 2 ;
			this.Append(str) ;
			this.theBuilder.Add ((byte) '\r');
			this.theBuilder.Add ((byte) '\n');
		}


		public int Length
		{
			get
			{
				return this.theBuilder.Count;
			}
		}

		public byte this[int index]
		{
			get
			{
				return this.theBuilder[index];
			}			
		}

		public new string ToString()
		{
			StringBuilder tmpBuilder = new StringBuilder ();

			for (int i = 0; i < theBuilder.Count; i++)
			{
				tmpBuilder.Append ((char) theBuilder[i]);
			}

			return tmpBuilder.ToString ();
		}

		private System.Collections.Generic.List<byte> theBuilder;
		private int length ;
	}


	public class HtmlText
	{
		public HtmlText()
		{
			tagStorageList.Add (HtmlAttribute.Bold, new TagStoreage (this, HtmlAttribute.Bold));
			tagStorageList.Add (HtmlAttribute.Italic, new TagStoreage (this, HtmlAttribute.Italic));
			tagStorageList.Add (HtmlAttribute.Strikeout, new TagStoreage (this, HtmlAttribute.Strikeout));
			tagStorageList.Add (HtmlAttribute.Subscript, new TagStoreage (this, HtmlAttribute.Subscript));
			tagStorageList.Add (HtmlAttribute.Superscript, new TagStoreage (this, HtmlAttribute.Superscript));
			tagStorageList.Add (HtmlAttribute.Underlined, new TagStoreage (this, HtmlAttribute.Underlined));

			this.mshtml = new MSHtmlText ();
			this.output.AppendLine ("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
			this.output.AppendLine ("<HTML><HEAD>") ;
			this.output.AppendLine ("<STYLE></STYLE>") ;
			this.output.AppendLine ("</HEAD>");
			this.output.AppendLine ("");
			this.output.AppendLine ("<BODY>");
			this.output.AppendLine ("<!--StartFragment-->");
			this.mshtml.UpdateStartFragment (this.output.Length);
		}

		public void Terminate()
		{

			this.CloseOpenTagsOnStack (true);

			mshtml.UpdateEndFragment (this.output.Length);
			this.output.AppendLine ("<!--EndFragment-->");
			this.output.AppendLine ("<BODY>");
			this.output.Append ("<HTML>");
			mshtml.UpdateEndHtml (this.output.Length);
			this.output.AppendLine ("");
			
			byte[] headerbytes = mshtml.GetBytes ();
			int length = headerbytes.Length + this.output.Length;

			byte[] blob = new byte[length];

			int i, j ;
			for (i = 0; i < headerbytes.Length; i++)
			{
				blob[i] = headerbytes[i]; 
			}

			for (j = 0; j < this.output.Length; j++,i++)
			{
				blob[i] = this.output[j];
			}

			this.memoryStream = new System.IO.MemoryStream (blob);

			char[] chararray = new char[length];

			for (i = 0; i < length; i++)
			{
				chararray[i] = (char) blob[i];
			}
			string test = new string (chararray);

		}

		public override string ToString()
		{
			return this.output.ToString ();
		}

		public void AppendText(string thestring)
		{
			//ProcessAttribute (ref precedIsItalic, ref isItalic, HtmlAttribute.Italic);
			//ProcessAttribute (ref precedIsBold, ref isBold, HtmlAttribute.Bold);

			foreach (System.Collections.Generic.KeyValuePair<HtmlAttribute, TagStoreage> kv in this.tagStorageList)
			{
				kv.Value.ProcessIt ();
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
			this.precedFontFace = this.fontFace;


			if (this.precedfontSize != this.fontSize)
			{
				if (this.precedfontSize != 0)
				{
					this.CloseTag (HtmlAttribute.Font);
				}

				if (this.fontSize != 0)
				{
					int htmlfontsize = HtmlText.PointFontSizeToHtmlFontSize (this.fontSize);

					this.OpenTag (HtmlAttribute.Font, "size", htmlfontsize.ToString ());
				}
			}
			this.precedfontSize = this.fontSize;

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
			this.precedfontColor = this.fontColor;			
			

			this.precedParagraphMode = this.paragraphMode;

			this.AppendTagsToClose (false);
			this.AppendPendingTags ();
			this.AppendTagsToOpen ();

			this.rawtextBuilder.Append (thestring);

			this.TransformSpecialHtmlChars (ref thestring);
			this.output.Append (thestring);
		}

		public void SetSimpleXScript(Rosetta.SimpleXScript xscript)
		{
			this.tagStorageList[HtmlAttribute.Superscript].Set (xscript == Rosetta.SimpleXScript.Superscript);
			this.tagStorageList[HtmlAttribute.Subscript].Set (xscript == Rosetta.SimpleXScript.Subscript);
		}

		public void SetItalic(bool italic)
		{
			this.tagStorageList[HtmlAttribute.Italic].Set(italic);
		}

		public void SetBold(bool bold)
		{
			this.tagStorageList[HtmlAttribute.Bold].Set (bold);
		}

		public void SetUnderlined(bool underlined)
		{
			this.tagStorageList[HtmlAttribute.Underlined].Set (underlined);
		}

		public void SetStrikeout(bool strikeout)
		{
			this.tagStorageList[HtmlAttribute.Strikeout].Set (strikeout);
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

		public string rawText
		{
			get
			{
				return this.rawtextBuilder.ToString (); 
			}
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

		public void ProcessAttribute(ref bool precedent, ref bool current, HtmlAttribute attr)
		{
			if (precedent && !current)
			{
				this.CloseTag (attr);
			}
			if (!precedent && current)
			{
				this.OpenTag (attr);
			}
			precedent = current;
		}

		private class TagStoreage
		{
			public TagStoreage(HtmlText htmltext, HtmlAttribute attr)
			{
				this.attr = attr;
				this.htmltext = htmltext;
			}

			public void ProcessIt()
			{
				this.htmltext.ProcessAttribute (ref this.precedingstate, ref this.currentstate, this.attr);
			}
			public void Set(bool setit)
			{
				currentstate = setit;
			}

			private bool precedingstate;
			private bool currentstate;
			private HtmlAttribute attr;
			private HtmlText htmltext;
		}

		private System.Collections.Generic.Dictionary<HtmlAttribute, TagStoreage> tagStorageList = 
			new System.Collections.Generic.Dictionary<HtmlAttribute, TagStoreage> ();

		private void ClearTags()
		{
			this.paragraphMode = Wrappers.JustificationMode.Unknown;
			this.precedParagraphMode = Wrappers.JustificationMode.Unknown;
			this.paragraphNotModeSet = true;

			this.fontColor = "";
			this.precedfontColor = "";

			this.fontSize = 0;
			this.precedfontSize = 0;

			this.fontFace = "";
			this.precedFontFace = "";
		}


		private void TransformToUTF8(ref string line)
		{
#if  false
			UTF8Encoding utf8 = new UTF8Encoding ();
			byte[] encodedBytes = utf8.GetBytes (line);
			
			char[] chararray = new char[encodedBytes.Length] ;

			for (int i = 0; i < encodedBytes.Length; i++)
			{
				chararray[i] = (char) encodedBytes[i];
			}

			line = new string (chararray);
#endif
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


		public enum HtmlAttribute
		{
			Bold,
			Italic,
			Underlined,
			Strikeout,
			Font,
			Paragraph,
			Superscript,
			Subscript
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
						retval = HtmlText.GetHtmlTag ("font", tagmode, this.parametername, this.parametervalue);
						break;
					case HtmlAttribute.Subscript:
						retval = HtmlText.GetHtmlTag ("sub", tagmode);
						break;
					case HtmlAttribute.Superscript:
						retval = HtmlText.GetHtmlTag ("sup", tagmode);
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

		private string fontColor = "";
		private string precedfontColor = "";

		private int fontSize = 0;
		private int precedfontSize = 0;

		private string fontFace = "";
		private string precedFontFace = "";


		private ExchangeStringBuilder output = new ExchangeStringBuilder ();
		private StringBuilder rawtextBuilder = new StringBuilder ();

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

		private System.IO.MemoryStream memoryStream;

		public System.IO.MemoryStream HtmlStream
		{
			get
			{
				return memoryStream;
			}
		}
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

		public enum SimpleXScript
		{
			Superscript,
			Subscript,
			Normalscript
		} ;

		static SimpleXScript GetSimpleXScript(Wrappers.TextWrapper wrapper)
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
					// on est tombé sur un séparateur de paragraphe
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

					SimpleXScript xscript = GetSimpleXScript (textWrapper);
					htmlText.SetSimpleXScript (xscript);

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

#if true
			System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject ();
			data.SetData (System.Windows.Forms.DataFormats.Text, true, htmlText.rawText);
			data.SetData (System.Windows.Forms.DataFormats.Html, true, htmlText.HtmlStream);
			System.Windows.Forms.Clipboard.SetDataObject (data, true);
#else
			//System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, htmlText.MemoryStream);
			System.Windows.Forms.Clipboard.Clear ();
			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Text, "Test");
			System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, htmlText.MemoryStream);
			// System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Text, "Test");
#endif

			// System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, s);
			//System.Windows.Forms.Clipboard.SetData (System.Windows.Forms.DataFormats.Html, htmlText.msHtml);
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

			System.Diagnostics.Debug.WriteLine ("Code de test 1 appelé.");
		}
		#endregion

	}
}

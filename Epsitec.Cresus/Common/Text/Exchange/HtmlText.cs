
#define USE_SPAN

using System;
using System.Collections.Generic;
using System.Text;


namespace Epsitec.Common.Text.Exchange
{
	public class HtmlTextOut
	{

		public HtmlTextOut()
		{
			this.Initialize("") ;
		}

		public HtmlTextOut(string comment)
		{
			this.Initialize(comment) ;
		}

		private void Initialize(string comment)
		{
			this.tagStorageList.Add (HtmlTagType.Bold, new TagStoreage (this, HtmlTagType.Bold));
			this.tagStorageList.Add (HtmlTagType.Italic, new TagStoreage (this, HtmlTagType.Italic));
			this.tagStorageList.Add (HtmlTagType.Strikeout, new TagStoreage (this, HtmlTagType.Strikeout));
			this.tagStorageList.Add (HtmlTagType.Subscript, new TagStoreage (this, HtmlTagType.Subscript));
			this.tagStorageList.Add (HtmlTagType.Superscript, new TagStoreage (this, HtmlTagType.Superscript));
			this.tagStorageList.Add (HtmlTagType.Underline, new TagStoreage (this, HtmlTagType.Underline));
#if USE_SPAN
			this.tagStorageList.Add (HtmlTagType.Span, new TagStoreage (this, HtmlTagType.Span));
#endif
			this.mshtml = new MSClipboardHtmlHeader ();
			this.output.AppendLine ("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
			this.output.AppendLine ("<HTML><HEAD>");

			this.output.AppendLine ("<!-- EpsitecFormattedText");
			this.output.AppendLine (comment);
			this.output.AppendLine ("-->");

			this.output.AppendLine ("<STYLE></STYLE>");
			this.output.AppendLine ("</HEAD>");
			this.output.AppendLine (string.Empty);
			this.output.AppendLine ("<BODY>");
			this.output.AppendLine ("<!--StartFragment-->");
			this.mshtml.UpdateStartFragment (this.output.Length);
		}

		public System.IO.MemoryStream HtmlStream
		{
			get
			{
				return this.memoryStream;
			}
		}

		public void Terminate()
		{

			this.CloseOpenTagsOnStack (true);

			this.mshtml.UpdateEndFragment (this.output.Length);
			this.output.AppendLine ("<!--EndFragment-->");
			this.output.AppendLine ("</BODY>");
			this.output.Append ("</HTML>");
			this.mshtml.UpdateEndHtml (this.output.Length);
			this.output.AppendLine (string.Empty);

			byte[] headerbytes = mshtml.GetBytes ();
			int length = headerbytes.Length + this.output.Length;

			byte[] blob = new byte[length];

			int i, j;
			for (i = 0; i < headerbytes.Length; i++)
			{
				blob[i] = headerbytes[i];
			}

			for (j = 0; j < this.output.Length; j++, i++)
			{
				blob[i] = this.output[j];
			}

			this.memoryStream = new System.IO.MemoryStream (blob);

#if false
			char[] chararray = new char[length];

			for (i = 0; i < length; i++)
			{
				chararray[i] = (char) blob[i];
			}
			string test = new string (chararray);
#endif
		}

		public override string ToString()
		{
			return this.output.ToString ();
		}

		public void AppendText(string thestring)
		{
#if USE_SPAN
			TagStoreage ts = this.tagStorageList[HtmlTagType.Span];
			ts.SetSpanStyle (new SpanStyle (this.fontFace, this.fontSize, this.fontColor));
#endif

			foreach (System.Collections.Generic.KeyValuePair<HtmlTagType, TagStoreage> kv in this.tagStorageList)
			{
				kv.Value.ProcessIt ();
			}

#if USE_SPAN
#else
			if (this.precedFontFace != this.fontFace)
			{
				if (this.precedFontFace.Length != 0)
				{
					this.CloseTag (HtmlTagType.Font);
				}

				if (this.fontFace.Length != 0)
				{
					this.OpenTag (HtmlTagType.Font, "face", this.fontFace);
				}
			}
			this.precedFontFace = this.fontFace;


			if (this.precedfontSize != this.fontSize)
			{
				if (this.precedfontSize != 0)
				{
					this.CloseTag (HtmlTagType.Font);
				}

				if (this.fontSize != 0)
				{
					int htmlfontsize = HtmlText.PointFontSizeToHtmlFontSize (this.fontSize);

					this.OpenTag (HtmlTagType.Font, "size", htmlfontsize.ToString ());
				}
			}
			this.precedfontSize = this.fontSize;

			if (this.precedfontColor != this.fontColor)
			{
				if (this.precedfontColor.Length != 0)
				{
					this.CloseTag (HtmlTagType.Font);
				}

				if (this.fontColor.Length != 0)
				{
					this.OpenTag (HtmlTagType.Font, "color", this.fontColor);
				}
			}
			this.precedfontColor = this.fontColor;
#endif

			this.precedParagraphMode = this.paragraphMode;

			this.AppendTagsToClose (false);
			this.AppendPendingTags ();
			this.AppendTagsToOpen ();

			this.TransformSpecialHtmlChars (ref thestring);
			this.output.Append (thestring);
			this.paragraphStarted = false;
		}

		public void SetSimpleXScript(Rosetta.SimpleXScript xscript)
		{
			this.tagStorageList[HtmlTagType.Superscript].Set (xscript == Rosetta.SimpleXScript.Superscript);
			this.tagStorageList[HtmlTagType.Subscript].Set (xscript == Rosetta.SimpleXScript.Subscript);
		}

		public void SetItalic(bool italic)
		{
			this.tagStorageList[HtmlTagType.Italic].Set (italic);
		}

		public void SetBold(bool bold)
		{
			this.tagStorageList[HtmlTagType.Bold].Set (bold);
		}

		public void SetUnderline(bool underline)
		{
			this.tagStorageList[HtmlTagType.Underline].Set (underline);
		}

		public void SetStrikeout(bool strikeout)
		{
			this.tagStorageList[HtmlTagType.Strikeout].Set (strikeout);
		}

		public void SetFontSize(double fontSize)
		{
			this.fontSize = fontSize / /* Epsitec.Common.Document.Modifier.FontSizeScale*/ FontSizeFactor;
		}

		static private int[] htmlfontsizes =
			{
				8,10,12,14,18,24,36
			};

		static public int PointFontSizeToHtmlFontSize(int pointsize)
		{
			int htmlsize = 1;

			int i;

			for (i = 0; i <= HtmlTextOut.htmlfontsizes.Length; i++)
			{
				if (pointsize <= htmlfontsizes[i])
					break;
				htmlsize++;
			}

			return htmlsize;
		}

		static public int HtmlFontSizeToPointFontSize(int htmlsize)
		{
			return HtmlTextOut.htmlfontsizes[htmlsize - 1];
		}

		public void SetFontFace(string fontFace)
		{
			if (fontFace != null)
				this.fontFace = fontFace;
			else
				this.fontFace = string.Empty;
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
				this.fontColor = string.Empty;
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
			string attributeName = null, attributeValue = null;

			if (JustificationMode != Wrappers.JustificationMode.Unknown)
			{
				attributeName = "align";

				switch (JustificationMode)
				{
					case Wrappers.JustificationMode.AlignLeft:
						attributeName = null;
						attributeValue = null;
						break;
					case Wrappers.JustificationMode.JustifyAlignLeft:
					case Wrappers.JustificationMode.JustifyCenter:
					case Wrappers.JustificationMode.JustifyAlignRight:
						attributeValue = "justify";
						break;
					case Wrappers.JustificationMode.Center:
						attributeValue = "center";
						break;
					case Wrappers.JustificationMode.AlignRight:
						attributeValue = "right";
						break;
				}
			}
			else
			{

			}

			HtmlTagWithParam tag = new HtmlTagWithParam (HtmlTagType.Paragraph, attributeName, attributeValue);

			this.output.Append (tag.TagToString (HtmlTagMode.Open));
			this.paragraphStarted = true;
			this.IncOpenTags (tag.GetHtmlTagType ());
			this.openTagsStack.Push (tag);
		}

		public void CloseParagraph(Wrappers.JustificationMode JustificationMode)
		{
			if (this.paragraphStarted)
			{
				// on est au début d'un paragraphe, il faut ajouter un NBSP, sinon le paragraphe vide est ignéroi
				this.output.Append (new string ((char) 0xA0, 1));
				//this.output.Append ("xxxx");
			}
			this.CloseOpenTagsOnStack (true);
			this.ClearTags ();
			this.output.Append ("\r\n");


			this.NewParagraph (JustificationMode);
		}

		public const double FontSizeFactor = 254.0 / 72.0;


		static private string GetHtmlTag(string tagstring, HtmlTagMode tagMode)
		{
			return GetHtmlTag (tagstring, tagMode, null, null, false);
		}

		static private string GetHtmlTag(string tagstring, HtmlTagMode tagMode, string parametername, string parametervalue, bool useapostroph)
		{
			string retval = string.Empty;
			string tagcontent;

			if (parametername != null)
			{
				if (useapostroph)
				{
					tagcontent = string.Format ("{0} {1}='{2}'", tagstring, parametername, parametervalue);
				}
				else
				{
					tagcontent = string.Format ("{0} {1}=\"{2}\"", tagstring, parametername, parametervalue);
				}
			}
			else
			{
				tagcontent = tagstring;
			}

			switch (tagMode)
			{
				case HtmlTagMode.Open:
					retval = string.Concat ("<", tagcontent, ">");
					break;
				case HtmlTagMode.Close:
					retval = string.Concat ("</", tagstring, ">");
					break;
			}
			return retval;
		}

		private void ProcessTag(ref bool precedent, ref bool current, ref SpanStyle precdingStyle, ref SpanStyle currentStyle, HtmlTagType tag)
		{
			if (precdingStyle == null && currentStyle == null)
			{
				if (precedent && !current)
				{
					this.CloseTag (tag);
				}
				if (!precedent && current)
				{
					this.OpenTag (tag);
				}
				precedent = current;
			}
			else
			{
				if (precdingStyle != currentStyle)
				{
					if (this.TagIsOnStack (tag))
					{
						this.CloseTag (tag);
					}
					this.OpenTag (tag, "style", currentStyle.ToString ());
				}

				precdingStyle = currentStyle;
			}
		}

		private class TagStoreage
		{
			public TagStoreage(HtmlTextOut htmltext, HtmlTagType tag)
			{
				this.tag = tag;
				this.htmltext = htmltext;
			}

			public void ProcessIt()
			{
				this.htmltext.ProcessTag (ref this.precedingstate, ref this.currentstate,
										  ref this.precedingspanstyle, ref this.currentspanstyle, this.tag);
			}

			public void SetSpanStyle(SpanStyle st)
			{
				this.currentspanstyle = st;
			}

			public void Clear()
			{
#if false
				this.precedingstate = this.currentstate = false ;
				if (this.precedingspanstyle != null)
				{
					this.precedingspanstyle.Clear ();
				}

				if (this.currentspanstyle != null)
				{
					this.currentspanstyle.Clear ();
				}
#else
				this.precedingspanstyle = null;
				this.currentspanstyle = null;
				this.currentstate = false;
				this.precedingstate = false;
#endif
			}

			public void Set(bool setit)
			{
				currentstate = setit;
			}


			public int nbopen;
			private bool precedingstate;
			private bool currentstate;
			private SpanStyle precedingspanstyle;
			private SpanStyle currentspanstyle;
			private HtmlTagType tag;
			private HtmlTextOut htmltext;

			//private System.Collections.Generic.Dictionary 
		}

		private void IncOpenTags(HtmlTagType tag)
		{
			TagStoreage ts;

			if (this.tagStorageList.TryGetValue (tag, out ts))
			{
				this.tagStorageList[tag].nbopen++;
			}
		}

		private void DecOpenTags(HtmlTagType tag)
		{
			TagStoreage ts;

			if (this.tagStorageList.TryGetValue (tag, out ts))
			{
				ts.nbopen--;
			}
		}

		private int NbOfOpenTags(HtmlTagType tag)
		{
			TagStoreage ts;

			if (this.tagStorageList.TryGetValue (tag, out ts))
			{
				return ts.nbopen;
			}

			return 0;
		}


		private System.Collections.Generic.Dictionary<HtmlTagType, TagStoreage> tagStorageList = 
			new System.Collections.Generic.Dictionary<HtmlTagType, TagStoreage> ();

		private void ClearTags()
		{
			this.paragraphMode = Wrappers.JustificationMode.Unknown;
			this.precedParagraphMode = Wrappers.JustificationMode.Unknown;

			this.fontColor = string.Empty;
			this.precedfontColor = string.Empty;

			this.fontSize = 0;

			this.fontFace = string.Empty;
			this.precedFontFace = string.Empty;

			foreach (System.Collections.Generic.KeyValuePair<HtmlTagType, TagStoreage> kv in this.tagStorageList)
			{
				kv.Value.Clear ();
			}
		}

		private void TransformSpecialHtmlChars(ref string line)
		{
			StringBuilder output = new StringBuilder (line);
			string oldchar;

			output.Replace ("<", "&lt;");
			output.Replace (">", "&gt;");
			oldchar = new string ((char) Unicode.Code.LineSeparator, 1);
			output.Replace (oldchar, "<br />\r\n");

			for (int i = 0; i < output.Length - 1; i++)
			{
				if (output[i] == ' ' && output[i + 1] == ' ')
				{
					output[i] = (char) 0xA0;	// non breaking space
				}
			}

			line = output.ToString ();
		}

		private bool TagIsOnStack(HtmlTagType tagtype)
		{
			foreach (HtmlTagWithParam t in this.openTagsStack)
			{
				if (t.GetHtmlTagType () == tagtype)
					return true;
			}
			return false;
		}

		private void CloseTag(HtmlTagType tagtoclose)
		{
			HtmlTagWithParam tag = new HtmlTagWithParam (tagtoclose, null, null);
			this.tagsToClose.Add (tag);
		}

		private void OpenTag(HtmlTagType tagtoopen, string parametername, string parametervalue)
		{
			HtmlTagWithParam tag = new HtmlTagWithParam (tagtoopen, parametername, parametervalue);
			this.tagsToOpen.Add (tag);
		}


		private void OpenTag(HtmlTagType tagtoopen)
		{
			this.OpenTag (tagtoopen, null, null);
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

					HtmlTagWithParam tag = this.tagsToClose[i];
					HtmlTagWithParam tagonstack = this.openTagsStack.Peek ();

					if ((closeAlsoParagraphs || tag.GetHtmlTagType () != HtmlTagType.Paragraph) && tagonstack.IsSameTag (tag))
					{
						this.output.Append (tag.TagToString (HtmlTagMode.Close));
						this.DecOpenTags (tag.GetHtmlTagType ());
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
			this.pendingTags.Clear ();
			while (this.tagsToClose.Count > 0)
			{
				this.CloseTagsIfPossible (closeAlsoParagraphs);

				if (this.tagsToClose.Count > 0)
				{
					HtmlTagWithParam topTag = this.openTagsStack.Pop ();
					pendingTags.Push (topTag);
					this.output.Append (topTag.TagToString (HtmlTagMode.Close));
					this.DecOpenTags (topTag.GetHtmlTagType ());
				}
			}

			this.tagsToClose.Clear ();
		}

		private void AppendPendingTags()
		{
			while (this.pendingTags.Count > 0)
			{
				HtmlTagWithParam tag;
				tag = this.pendingTags.Pop ();
				this.output.Append (tag.TagToString (HtmlTagMode.Open));
				this.IncOpenTags (tag.GetHtmlTagType ());
				this.openTagsStack.Push (tag);
			}
			this.pendingTags.Clear ();
		}

		private void CloseOpenTagsOnStack(bool closeAlsoParagraphs)
		{
			// ferme les tags restés ouverts
			while (this.openTagsStack.Count > 0)
			{
				HtmlTagWithParam tag;
				tag = openTagsStack.Pop ();
				this.output.Append (tag.TagToString (HtmlTagMode.Close));
				this.DecOpenTags (tag.GetHtmlTagType ());
			}
		}



		private void AppendTagsToOpen()
		{
			foreach (HtmlTagWithParam tag in this.tagsToOpen)
			{
				string tagString = tag.TagToString (HtmlTagMode.Open);
				this.output.Append (tagString);
				this.IncOpenTags (tag.GetHtmlTagType ());
				this.openTagsStack.Push (tag);
			}

			this.tagsToOpen.Clear ();
		}


		private enum HtmlTagType
		{
			Bold,
			Italic,
			Underline,
			Strikeout,
			Font,
			Paragraph,
			Superscript,
			Subscript
#if USE_SPAN
				,
			Span
#endif
		}

		private enum HtmlTagMode
		{
			Open,
			Close
		}

		private class HtmlTagWithParam
		{
			public HtmlTagWithParam(HtmlTagType tag, string parametername, string parametervalue)
			{
				this.htmltag = tag;
				this.parametername = parametername;
				this.parametervalue = parametervalue;
			}

			public string TagToString(HtmlTagMode tagmode)
			{
				string retval = string.Empty;

				switch (this.htmltag)
				{
					case HtmlTagType.Italic:
						retval = HtmlTextOut.GetHtmlTag ("i", tagmode);
						break;
					case HtmlTagType.Bold:
						retval = HtmlTextOut.GetHtmlTag ("b", tagmode);
						break;
					case HtmlTagType.Underline:
						retval = HtmlTextOut.GetHtmlTag ("u", tagmode);
						break;
					case HtmlTagType.Strikeout:
						retval = HtmlTextOut.GetHtmlTag ("s", tagmode);
						break;
					case HtmlTagType.Paragraph:
						retval = HtmlTextOut.GetHtmlTag ("p", tagmode, this.parametername, this.parametervalue, false);
						break;
					case HtmlTagType.Font:
						retval = HtmlTextOut.GetHtmlTag ("font", tagmode, this.parametername, this.parametervalue, false);
						break;
					case HtmlTagType.Subscript:
						retval = HtmlTextOut.GetHtmlTag ("sub", tagmode);
						break;
					case HtmlTagType.Superscript:
						retval = HtmlTextOut.GetHtmlTag ("sup", tagmode);
						break;
					case HtmlTagType.Span:
						retval = HtmlTextOut.GetHtmlTag ("span", tagmode, this.parametername, this.parametervalue, true);
						break;
				}

				return retval;
			}

			public bool IsSameTag(HtmlTagWithParam tag)
			{
				// si les deux tags sont de même type
				return this.htmltag == tag.htmltag;
			}

			public HtmlTagType GetHtmlTagType()
			{
				return htmltag;
			}


			private HtmlTagType htmltag;
			private string parametername;
			private string parametervalue;
		}

		private Wrappers.JustificationMode paragraphMode = Wrappers.JustificationMode.Unknown;
		private Wrappers.JustificationMode precedParagraphMode = Wrappers.JustificationMode.Unknown;
		private bool paragraphStarted = false;

		private string fontColor = string.Empty;
		private string precedfontColor = string.Empty;

		private double fontSize = 0;
		
		private string fontFace = string.Empty;
		private string precedFontFace = string.Empty;


		private ExchangeStringBuilder output = new ExchangeStringBuilder ();

		private System.Collections.Generic.Stack<HtmlTagWithParam> openTagsStack = new System.Collections.Generic.Stack<HtmlTagWithParam> ();
		private System.Collections.Generic.List<HtmlTagWithParam> tagsToClose = new System.Collections.Generic.List<HtmlTagWithParam> ();
		private System.Collections.Generic.List<HtmlTagWithParam> tagsToOpen = new System.Collections.Generic.List<HtmlTagWithParam> ();
		private System.Collections.Generic.Stack<HtmlTagWithParam> pendingTags = new System.Collections.Generic.Stack<HtmlTagWithParam> ();


		MSClipboardHtmlHeader mshtml;
#if false
		public string msHtml
		{
			get
			{
				return mshtml.ToString() ;
			}
		}
#endif
		private System.IO.MemoryStream memoryStream;

	}

}

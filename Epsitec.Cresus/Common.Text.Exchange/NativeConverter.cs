using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

// Conversion entre format presse-papier natif et TextWrapper
// Responsable: Michael Walz

// TODO:
// - gérer les propriétés suivantes : TextMarker, TextBox, UserTags, Conditions
// - propriété link: gèrer les / et autres signes cabalistiques dans les hyperliens
//

namespace Epsitec.Common.Text.Exchange
{
	class NativeConverter
	{

		public NativeConverter(CopyPasteContext cpContext, PasteMode pastemode)
		{
			this.textWrapper = cpContext.TextWrapper;
			this.paraWrapper = cpContext.ParaWrapper;
			this.navigator = cpContext.Navigator;
			this.story = cpContext.Story;
			this.pasteMode = pastemode;
		}

		/// <summary>
		/// Obtient un array de strings contenant chacun une définition de style
		/// </summary>
		/// <returns></returns>
		public List<string> GetStyleStrings()
		{
			TextContext context = this.story.TextContext;
			TextStyle[] styles = context.StyleList.StyleMap.GetSortedStyles ();
			List<string> stringstyles = new List<string>();
			Hashtable processedStyleCaptions = new Hashtable ();

			foreach (TextStyle thestyle in styles)
			{

				if (thestyle.TextStyleClass == TextStyleClass.Paragraph || thestyle.TextStyleClass == TextStyleClass.Text)
				{
					string stylecaption = context.StyleList.StyleMap.GetCaption (thestyle);

					AddStyle (context,  thestyle, stringstyles, false, processedStyleCaptions);
				}
			}

			return stringstyles;
		}

		private void AddStyle(TextContext context, TextStyle thestyle, List<string> stringstyles, bool isbasestyle, Hashtable processed)
		{
			//  Format d'une ligne de description d'un style dans le presse-papier :
			//  caption/styleident/isdefaultstyle/TextStyleClass/nbofparentstyles/parentstylename_1/.../parentstylename_n/serialized_style

			string stylecaption = context.StyleList.StyleMap.GetCaption (thestyle);
			StringBuilder output = new StringBuilder ();

			if (! processed.ContainsKey(stylecaption))
			{
				if (this.usedTextStyles.ContainsKey (stylecaption) || isbasestyle)
				{
					StringBuilder basestylenames = new StringBuilder (string.Format ("{0}/", thestyle.ParentStyles.Length));

					foreach (TextStyle basestyle in thestyle.ParentStyles)
					{
						string basestylename = context.StyleList.StyleMap.GetCaption (basestyle);
						basestylenames.AppendFormat ("{0}/", SerializerSupport.SerializeString(basestylename));

						this.AddStyle (context, basestyle, stringstyles, true, processed);
					}

					bool isDefault = (thestyle == context.DefaultTextStyle) || (thestyle == context.DefaultParagraphStyle);
					string props = Property.SerializeProperties (thestyle.StyleProperties);

					int styleident = this.GetStyleIdent (stylecaption);
					output.AppendFormat ("{0}/{1}/{2}/{3}/{4}{5}", SerializerSupport.SerializeString(stylecaption), styleident ,Misc.BoolToByte(isDefault),(byte) thestyle.TextStyleClass, basestylenames.ToString (), props);
					processed.Add (stylecaption, null);
					stringstyles.Add (output.ToString ());
				}
			}
		}


		private int GetStyleIdent(string caption)
		{
			int ident ;

			if (!this.usedTextStyles.TryGetValue (caption, out ident))
			{
				this.usedTextStyles.Add (caption, ++this.styleident);
				ident = this.styleident;
			}

			return ident;
		}

		/// <summary>
		/// Convertit les attributs d'un textwrapper en format presse-papier natif
		/// </summary>
		/// <param name="textwrapper"></param>
		/// <returns></returns>
		public string GetDefinedString(bool paragraphSep)
		{
			StringBuilder output = new StringBuilder();

			output.Append ('[');

			Text.TextStyle[] styles = this.navigator.TextStyles;

			foreach (TextStyle style in styles)
			{
				if (style.TextStyleClass == TextStyleClass.Paragraph && paragraphSep)
				{
					string caption = story.TextContext.StyleList.StyleMap.GetCaption (style);
					int styleident = GetStyleIdent (caption);
					output.AppendFormat ("pstyle:{0}/", styleident);
				}

				if (style.TextStyleClass == TextStyleClass.Text)
				{
					string caption = story.TextContext.StyleList.StyleMap.GetCaption (style);
					int styleident = GetStyleIdent (caption);
					output.AppendFormat ("cstyle:{0}/", styleident);
				}
			}

			if (paragraphSep)
			{
				output.Append("par/") ;
				string definedPar = this.GetDefinedParString() ;

				if (definedPar.Length != 0)
				{
					output.AppendFormat("pardef:{0}/", definedPar) ;
				}
			}

			if (this.textWrapper.Defined.IsInvertItalicDefined && this.textWrapper.Defined.InvertItalic)
			{
				output.Append ("i/");
			}

			if (this.textWrapper.Defined.IsInvertBoldDefined && this.textWrapper.Defined.InvertBold)
			{
				output.Append ("b/");
			}

			if (this.textWrapper.Defined.IsColorDefined)
			{
				output.AppendFormat ("c:{0}/", this.textWrapper.Defined.Color);
			}

			if (this.textWrapper.Defined.IsFontFaceDefined)
			{
				output.AppendFormat ("fface:{0}/", this.textWrapper.Defined.FontFace);
			}

			if (this.textWrapper.Defined.IsFontStyleDefined)
			{
				output.AppendFormat ("fstyle:{0}/", this.textWrapper.Defined.FontStyle);
			}

			if (this.textWrapper.Defined.IsFontSizeDefined)
			{
				if (this.textWrapper.Defined.IsUnitsDefined)
				{
					Properties.SizeUnits units = this.textWrapper.Defined.Units;
					output.AppendFormat ("funits:{0}/", (byte)this.textWrapper.Defined.Units);
				}

				output.AppendFormat ("fsize:{0}/", this.textWrapper.Defined.FontSize);
			}

			if (this.textWrapper.Defined.IsFontFeaturesDefined)
			{
				string [] features = this.textWrapper.Defined.FontFeatures ;

				StringBuilder conacatfeatures = new StringBuilder ();

				foreach (string feature in features)
				{
					conacatfeatures.AppendFormat ("{0}\\", feature);
				}

				output.AppendFormat ("ffeat:{0}/", conacatfeatures.ToString());
			}


			if (this.textWrapper.Defined.IsUnderlineDefined)
			{
				string xlinedef = XlineDefinitionToString (this.textWrapper.Defined.Underline);
				output.AppendFormat ("u:{0}/",xlinedef);
			}

			if (this.textWrapper.Defined.IsOverlineDefined)
			{
				string xlinedef = XlineDefinitionToString (this.textWrapper.Defined.Overline);
				output.AppendFormat ("o:{0}/", xlinedef);
			}

			if (this.textWrapper.Defined.IsStrikeoutDefined)
			{
				string xlinedef = XlineDefinitionToString (this.textWrapper.Defined.Strikeout);
				output.AppendFormat ("s:{0}/", xlinedef);
			}

			if (this.textWrapper.Defined.IsXscriptDefined)
			{
				string xscriptdef = XScriptDefinitionToString (this.textWrapper.Defined.Xscript);
				output.AppendFormat ("x:{0}/", xscriptdef);
			}

			if (this.textWrapper.Defined.IsFontGlueDefined)
			{
				output.AppendFormat ("fontglue:{0}/", this.textWrapper.Defined.FontGlue);
			}

			if (this.textWrapper.Defined.IsLanguageHyphenationDefined)
			{
				output.AppendFormat("langhyph:{0}/", this.textWrapper.Defined.LanguageHyphenation) ;
			}

			if (this.textWrapper.Defined.IsLanguageLocaleDefined)
			{
				output.AppendFormat ("langloc:{0}/", this.textWrapper.Defined.LanguageLocale);
			}

			if (this.textWrapper.Defined.IsLinkDefined)
			{
				output.AppendFormat ("link:{0}/", this.textWrapper.Defined.Link);
			}

			output.Append (']');
			return output.ToString() ;
		}

		public static bool IsParagraphSeparator(string input)
		{
			char[] separators = new char[] { '/' };
			string[] elements = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string elem in elements)
			{
				if (elem == "par")
					return true ;
			}

			return false;
		}

		public void SetDefined(string input, out bool paragrpahSep)
		{
			char[] separators = new char[] {'/'};

			paragrpahSep = false;

			bool invertItalic = false;
			bool invertBold = false;
			bool underline = false;
			bool color = false;
			bool fontFace = false ;
			bool fontStyle = false ;
			bool fontSize = false ;
			bool fontglue = false;
			bool languageHyphenation = false;
			bool languageLocale = false;
			bool units = false;
			bool features = false;
			bool overline = false;
			bool strikeout = false;
			bool xscript = false;
			bool link = false;

			this.textWrapper.SuspendSynchronizations ();

			System.Diagnostics.Debug.Assert (input[0] == '[' && input[input.Length-1] == ']');
			input = input.Substring(1, input.Length - 2) ;

			string[] elements = input.Split (separators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string elem in elements)
			{
				string[] subelements = elem.Split (':');

				bool notHandled = false;

				switch (subelements[0])
				{
					case "i":
						this.textWrapper.Defined.InvertItalic = true;
						invertItalic = true;
						break;
					case "b":
						this.textWrapper.Defined.InvertBold = true;
						invertBold = true;
						break;
					case "u":
						StringToXlineDefinition (subelements[1], this.textWrapper.Defined.Underline);
						underline = true;
						break;
					case "o":
						StringToXlineDefinition (subelements[1], this.textWrapper.Defined.Overline);
						overline = true;
						break;
					case "s":
						StringToXlineDefinition (subelements[1], this.textWrapper.Defined.Strikeout);
						strikeout = true;
						break;
					case "x":
						StringToXScriptDefinition (subelements[1], this.textWrapper.Defined.Xscript);
						xscript = true;
						break;
					case "c":
						this.textWrapper.Defined.Color = subelements[1];
						color = true;
						break;
					default:
						notHandled = true;
						break;
						
				}

				if (notHandled && pasteMode == PasteMode.MatchDestination)
				{
					if (subelements[0] == "par")
					{
						paragrpahSep = true;
					}
				}
				else if (notHandled && pasteMode == PasteMode.KeepSource)
				{
					switch (subelements[0])
					{
						case "cstyle":
							string styleident = subelements[1];

							//TextStyle thestyle = StyleFromCaption (stylecaption);
							TextStyle thestyle = this.GetTextStyleToApply (styleident);

							if (thestyle != null)
							{
								this.textWrapper.ResumeSynchronizations ();
								this.navigator.SetTextStyles (thestyle);
								this.textWrapper.SuspendSynchronizations ();
							}
							break;
						case "pstyle":
							styleident = subelements[1];
							//thestyle = StyleFromCaption (stylecaption);
							thestyle = this.GetTextStyleToApply (styleident);

							if (thestyle != null)
							{
								this.textWrapper.ResumeSynchronizations ();
								this.navigator.SetParagraphStyles (thestyle);
								this.textWrapper.SuspendSynchronizations ();
							}
							break;
						case "par":
							paragrpahSep = true;
							break;
						case "pardef":
							this.SetParagraph (subelements[1]);
							break;
						case "fface":
							this.textWrapper.Defined.FontFace = subelements[1];
							fontFace = true;
							break;
						case "fstyle":
							this.textWrapper.Defined.FontStyle = subelements[1];
							fontStyle = true;
							break;
						case "fsize":
							double size = Misc.ParseDouble (subelements[1]);
							this.textWrapper.Defined.FontSize = size;
							fontSize = true;
							break;
						case "funits":
							byte theunits = Misc.ParseByte (subelements[1]);
							this.textWrapper.Defined.Units = (Properties.SizeUnits) theunits;
							units = true;
							break;
						case "ffeat":
							char[] splitchars = { '\\' };
							string[] thefeatures = subelements[1].Split (splitchars, StringSplitOptions.RemoveEmptyEntries);
							this.textWrapper.Defined.FontFeatures = thefeatures;
							features = true;
							break;
						case "fontglue":
							this.textWrapper.Defined.FontGlue = Misc.ParseDouble (subelements[1]);
							fontglue = true;
							break;
						case "langhyph":
							this.textWrapper.Defined.LanguageHyphenation = Misc.ParseDouble (subelements[1]);
							languageHyphenation = true;
							break;
						case "langloc":
							this.textWrapper.Defined.LanguageLocale = subelements[1];
							languageLocale = true;
							break;
						case "link":
							this.textWrapper.Defined.Link = subelements[1];
							link = true;
							break;
					}
				}
			}

			if (!invertItalic)
				this.textWrapper.Defined.ClearInvertItalic ();

			if (!invertBold)
				this.textWrapper.Defined.ClearInvertBold ();

			if (!underline)
				this.textWrapper.Defined.ClearUnderline ();

			if (!overline)
				this.textWrapper.Defined.ClearOverline ();

			if (!strikeout)
				this.textWrapper.Defined.ClearStrikeout ();

			if (!xscript)
				this.textWrapper.Defined.ClearXscript ();

			if (!color)
				this.textWrapper.Defined.ClearColor ();

			if (pasteMode == PasteMode.KeepSource)
			{

				if (!fontFace)
					this.textWrapper.Defined.ClearFontFace ();

				if (!fontStyle)
					this.textWrapper.Defined.ClearFontStyle ();

				if (!fontSize)
					this.textWrapper.Defined.ClearFontSize ();

				if (!fontglue)
					this.textWrapper.Defined.ClearFontGlue ();

				if (!languageHyphenation)
					this.textWrapper.Defined.ClearLanguageHyphenation ();

				if (!languageLocale)
					this.textWrapper.Defined.ClearLanguageLocale ();

				if (!link)
					this.textWrapper.Defined.ClearLink ();

				if (!units)
					this.textWrapper.Defined.ClearUnits ();

				if (!features)
					this.textWrapper.Defined.ClearFontFeatures ();
			}

			this.textWrapper.ResumeSynchronizations ();
		}

		public void ResetParagraph()
		{
			this.paraWrapper.Defined.RestoreInternalState (this.savedDefinedParagraph);
		}

		private static string XScriptDefinitionToString(Wrappers.TextWrapper.XscriptDefinition xscriptdef)
		{
			StringBuilder output = new StringBuilder ();

			output.Append (Misc.BoolToByte (xscriptdef.IsDisabled));
			output.Append (';');
			output.Append (Misc.BoolToByte (xscriptdef.IsEmpty));
			output.Append (';');
			output.Append (xscriptdef.Offset);
			output.Append (';');
			output.Append (xscriptdef.Scale);
			output.Append (';');

			return output.ToString ();
		}

		private static void StringToXScriptDefinition(string strxline, Wrappers.TextWrapper.XscriptDefinition xscriptdef)
		{
			char[] sep = { ';' };
			string[] elements = strxline.Split (sep, StringSplitOptions.None);

			xscriptdef.IsDisabled = Misc.ParseBool (elements[0]);
			//	xscriptdef.IsEmpty = Misc.ParseBool (elements[1]);;
			xscriptdef.Offset = Misc.ParseDouble (elements[2]);
			xscriptdef.Scale = Misc.ParseDouble (elements[3]);
		}


		private static string XlineDefinitionToString(Wrappers.TextWrapper.XlineDefinition xlinedef)
		{
			StringBuilder output = new StringBuilder ();

			output.Append (Misc.BoolToByte (xlinedef.IsDisabled));
			output.Append (';');
			output.Append (Misc.BoolToByte (xlinedef.IsEmpty));
			output.Append (';');
			output.Append (xlinedef.Position);
			output.Append (';');
			output.Append ((byte) xlinedef.PositionUnits);
			output.Append (';');
			output.Append (xlinedef.Thickness);
			output.Append (';');
			output.Append ((byte) xlinedef.ThicknessUnits);
			output.Append (';');
			output.Append (SerializerSupport.SerializeString (xlinedef.DrawClass));
			output.Append (';');
			output.Append (SerializerSupport.SerializeString (xlinedef.DrawStyle));
			output.Append (';');

			return output.ToString ();
		}

		private static void StringToXlineDefinition(string strxline, Wrappers.TextWrapper.XlineDefinition xlinedef)
		{
			char[] sep = { ';' };
			string[] elements = strxline.Split (sep, StringSplitOptions.None);

			xlinedef.IsDisabled = Misc.ParseBool (elements[0]);
			//xlinedef.IsEmpty = ParseBool (elements[1])

			xlinedef.Position = Misc.ParseDouble (elements[2]);

			xlinedef.PositionUnits = (Properties.SizeUnits) Misc.ParseByte (elements[3]);

			xlinedef.Thickness = Misc.ParseDouble (elements[4]);

			xlinedef.ThicknessUnits = (Properties.SizeUnits) Misc.ParseByte (elements[5]);

			xlinedef.DrawClass = SerializerSupport.DeserializeString (elements[6]);

			xlinedef.DrawStyle = SerializerSupport.DeserializeString (elements[7]);
		}

		private void SetParagraph(string parastring)
		{
			char[] seps = { '\\' };
			char[] innerseps = { '|' };
			string[] elements = parastring.Split (seps, StringSplitOptions.RemoveEmptyEntries);

			bool indentationlevel = false;
			bool alignmode = false;
			bool justificationmode = false;
			bool leading = false;
			bool leadingunits = false;
			bool indentationlevelattribute = false;
			bool leftmarginbody = false;
			bool rightmarginbody = false;
			bool leftmarginfirst = false;
			bool rightmarginfirst = false;
			bool marginunits = false;
			bool pagargaphstartmode = false;
			bool spaceafter = false;
			bool spaceafterunits = false;
			bool spacebefore = false;
			bool spacebeforeunits = false;
			bool breakfenceafter = false;
			bool breakfencebefore = false;
			bool keependlines = false;
			bool keepstartlines = false;
			bool keepwithnextparagraph = false;
			bool keepwithpreviousparagraph = false;
			bool hyphenation = false;


			this.paraWrapper.SuspendSynchronizations ();

			this.savedDefinedParagraph = this.paraWrapper.Defined.SaveInternalState ();

			foreach (string element in elements)
			{
				string[] el = element.Split (innerseps, StringSplitOptions.RemoveEmptyEntries);

				switch (el[0])
				{
					case "align":
						this.paraWrapper.Defined.AlignMode = (Properties.AlignMode) Misc.ParseByte (el[1]);
						alignmode = true;
						break;
					case "just":
						this.paraWrapper.Defined.JustificationMode = (Wrappers.JustificationMode) Misc.ParseByte (el[1]);
						justificationmode = true;
						break;
					case "indleva":
						this.paraWrapper.Defined.IndentationLevelAttribute = SerializerSupport.DeserializeString (el[1]);
						indentationlevelattribute = true;
						break;
					case "indlev":
						this.paraWrapper.Defined.IndentationLevel = Misc.ParseInt (el[1]);
						indentationlevel = true;
						break;
					case "leading":
						this.paraWrapper.Defined.Leading = Misc.ParseDouble (el[1]);
						leading = true;
						break;
					case "leadingunit":
						this.paraWrapper.Defined.LeadingUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						leadingunits = true;
						break;
					case "leftmbody":
						this.paraWrapper.Defined.LeftMarginBody = Misc.ParseDouble (el[1]);
						leftmarginbody = true;
						break;
					case "rightmbody":
						this.paraWrapper.Defined.RightMarginBody = Misc.ParseDouble (el[1]);
						rightmarginbody = true;
						break;
					case "leftmfirst":
						this.paraWrapper.Defined.LeftMarginFirst = Misc.ParseDouble (el[1]);
						leftmarginfirst = true;
						break;
					case "rightmfirst":
						this.paraWrapper.Defined.RightMarginFirst = Misc.ParseDouble (el[1]);
						rightmarginfirst = true;
						break;
					case "marginunits":
						this.paraWrapper.Defined.MarginUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						marginunits = true;
						break;
					case "parstartmode":
						this.paraWrapper.Defined.ParagraphStartMode = (Properties.ParagraphStartMode) Misc.ParseByte (el[1]);
						pagargaphstartmode = true;
						break;
					case "spaceafter":
						this.paraWrapper.Defined.SpaceAfter = Misc.ParseDouble(el[1]) ;
						spaceafter = true ;
						break;
					case "spaceafteru":
						this.paraWrapper.Defined.SpaceAfterUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						spaceafterunits = true;
						break;
					case "spacebefore":
						this.paraWrapper.Defined.SpaceBefore = Misc.ParseDouble (el[1]);
						spacebefore = true;
						break;
					case "spacebeforeu":
						this.paraWrapper.Defined.SpaceBeforeUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						spacebeforeunits = true;
						break;
					case "bfa":
						this.paraWrapper.Defined.BreakFenceAfter = Misc.ParseDouble (el[1]);
						breakfenceafter = true;
						break;
					case "bfb":
						this.paraWrapper.Defined.BreakFenceBefore = Misc.ParseDouble (el[1]);
						breakfencebefore = true;
						break;
					case "keepel":
						this.paraWrapper.Defined.KeepEndLines = Misc.ParseInt (el[1]);
						keependlines = true;
						break;
					case "keepsl":
						this.paraWrapper.Defined.KeepStartLines = Misc.ParseInt (el[1]);
						keepstartlines = true;
						break;
					case "keepwithnext":
						this.paraWrapper.Defined.KeepWithNextParagraph = Misc.ParseBool (el[1]);
						keepwithnextparagraph = true;
						break;
					case "keepwithprev":
						this.paraWrapper.Defined.KeepWithPreviousParagraph = Misc.ParseBool (el[1]);
						keepwithpreviousparagraph = true;
						break;
					case "hyphen":
						this.paraWrapper.Defined.Hyphenation = Misc.ParseBool (el[1]);
						hyphenation = true;
						break;

				}
			}

			if (!hyphenation)
				this.paraWrapper.Defined.ClearHyphenation ();

			if (!keepwithnextparagraph)
				this.paraWrapper.Defined.ClearKeepWithNextParagraph ();

			if (!keepwithpreviousparagraph)
				this.paraWrapper.Defined.ClearKeepWithPreviousParagraph();

			if (!keependlines)
				this.paraWrapper.Defined.ClearKeepEndLines ();

			if (!keepstartlines)
				this.paraWrapper.Defined.ClearKeepStartLines ();

			if (!breakfenceafter)
				this.paraWrapper.Defined.ClearBreakFenceAfter ();

			if (!breakfencebefore)
				this.paraWrapper.Defined.ClearBreakFenceBefore ();

			if (!spaceafter)
				this.paraWrapper.Defined.ClearSpaceAfter() ;

			if (!spaceafterunits)
				this.paraWrapper.Defined.ClearSpaceAfterUnits ();

			if (!spacebefore)
				this.paraWrapper.Defined.ClearSpaceBefore ();

			if (!spacebeforeunits)
				this.paraWrapper.Defined.ClearSpaceBeforeUnits ();

			if (!pagargaphstartmode)
				this.paraWrapper.Defined.ClearParagraphStartMode ();

			if (!indentationlevel)
				this.paraWrapper.Defined.ClearIndentationLevel ();

			if (!indentationlevelattribute)
				this.paraWrapper.Defined.ClearIndentationLevelAttribute ();

			if (!alignmode)
				this.paraWrapper.Defined.ClearAlignMode ();

			if (!justificationmode)
				this.paraWrapper.Defined.ClearJustificationMode ();

			if (!leading)
				this.paraWrapper.Defined.ClearLeading ();

			if (!leadingunits)
				this.paraWrapper.Defined.ClearLeadingUnits ();

			if (!leftmarginbody)
				this.paraWrapper.Defined.ClearLeftMarginBody ();

			if (!rightmarginbody)
				this.paraWrapper.Defined.ClearRightMarginBody ();

			if (!leftmarginfirst)
				this.paraWrapper.Defined.ClearLeftMarginFirst ();

			if (!rightmarginfirst)
				this.paraWrapper.Defined.ClearRightMarginFirst ();

			if (!marginunits)
				this.paraWrapper.Defined.ClearMarginUnits ();

			this.paraWrapper.ResumeSynchronizations ();
		}


		private string GetDefinedParString()
		{
			StringBuilder output = new StringBuilder ();

			if (this.paraWrapper.Defined.IsAlignModeDefined)
			{
				output.AppendFormat ("align|{0}\\", (byte) this.paraWrapper.Defined.AlignMode);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsJustificationModeDefined)
			{
				output.AppendFormat ("just|{0}", (byte) this.paraWrapper.Defined.JustificationMode);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsIndentationLevelAttributeDefined)
			{
				output.AppendFormat ("indleva|{0}", SerializerSupport.SerializeString (this.paraWrapper.Defined.IndentationLevelAttribute));
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsIndentationLevelDefined)
			{
				output.AppendFormat ("indlev|{0}", this.paraWrapper.Defined.IndentationLevel);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsLeadingDefined)
			{
				output.AppendFormat ("leading|{0}", this.paraWrapper.Defined.Leading);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsLeadingUnitsDefined)
			{
				output.AppendFormat ("leadingunit|{0}", (byte) this.paraWrapper.Defined.LeadingUnits);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsLeftMarginBodyDefined)
			{
				output.AppendFormat ("leftmbody|{0}", this.paraWrapper.Defined.LeftMarginBody);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsRightMarginBodyDefined)
			{
				output.AppendFormat ("rightmbody|{0}", this.paraWrapper.Defined.RightMarginBody);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsLeftMarginFirstDefined)
			{
				output.AppendFormat ("leftmfirst|{0}", this.paraWrapper.Defined.LeftMarginFirst);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsRightMarginFirstDefined)
			{
				output.AppendFormat ("rightmfirst|{0}", this.paraWrapper.Defined.RightMarginFirst);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsMarginUnitsDefined)
			{
				output.AppendFormat ("marginunits|{0}", (byte) this.paraWrapper.Defined.MarginUnits);
				output.Append ('\\');
			}

#if false
			if (this.paraWrapper.Defined.IsManagedParagraphDefined)
			{
				output.AppendFormat ("managedp|{0}", this.paraWrapper.Defined.ManagedParagraph);
			}
#endif

			if (this.paraWrapper.Defined.IsParagraphStartModeDefined)
			{
				output.AppendFormat ("parstartmode|{0}", (byte) this.paraWrapper.Defined.ParagraphStartMode);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsSpaceAfterDefined)
			{
				output.AppendFormat ("spaceafter|{0}", (byte) this.paraWrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat ("spaceafteru|{0}", (byte) this.paraWrapper.Defined.SpaceAfterUnits);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsSpaceBeforeDefined)
			{
				output.AppendFormat ("spacebefore|{0}", (byte) this.paraWrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat ("spacebeforeu|{0}", (byte) this.paraWrapper.Defined.SpaceBeforeUnits);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsBreakFenceAfterDefined)
			{
				output.AppendFormat ("bfa|{0}", this.paraWrapper.Defined.BreakFenceAfter);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsBreakFenceBeforeDefined)
			{
				output.AppendFormat ("bfb|{0}", this.paraWrapper.Defined.BreakFenceBefore);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsHyphenationDefined)
			{
				output.AppendFormat ("hyphen|{0}", Misc.BoolToByte (this.paraWrapper.Defined.Hyphenation));
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsItemListInfoDefined)
			{
				output.AppendFormat ("itemli|{0}", this.paraWrapper.Defined.ItemListInfo);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsKeepEndLinesDefined)
			{
				output.AppendFormat ("keepel|{0}", this.paraWrapper.Defined.KeepEndLines);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsKeepStartLinesDefined)
			{
				output.AppendFormat ("keepsl|{0}", this.paraWrapper.Defined.KeepStartLines);
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsKeepWithNextParagraphDefined)
			{
				output.AppendFormat ("keepwithnext|{0}", Misc.BoolToByte (this.paraWrapper.Defined.KeepWithNextParagraph));
				output.Append ('\\');
			}

			if (this.paraWrapper.Defined.IsKeepWithPreviousParagraphDefined)
			{
				output.AppendFormat ("keepwithprev|{0}", Misc.BoolToByte (this.paraWrapper.Defined.KeepWithPreviousParagraph));
				output.Append ('\\');
			}

			return output.ToString ();
		}


		private TextStyle StyleFromCaption(string caption)
		{
			TextStyle[] styles = this.story.TextContext.StyleList.StyleMap.GetSortedStyles ();

			foreach (TextStyle thestyle in styles)
			{
				if (this.story.TextContext.StyleList.StyleMap.GetCaption (thestyle) == caption)
				{
					return thestyle;
				}
			}

			return null;
		}

		private StyleDefinition GetStyleDefinition(int styleident)
		{
			return this.styleDefinitions[styleident];
		}

		 
		private StyleDefinition GetStyleDefinition(string caption)
		{
			foreach (KeyValuePair<int, StyleDefinition> kv in this.styleDefinitions)
			{
				if (kv.Value.Caption == caption)
					return kv.Value;
			}

			return null;
		}

		private TextStyle GetTextStyleToApply (string styleident)
		{
			int ident = Misc.ParseInt(styleident) ;
			string stylecaption = this.styleDefinitions[ident].Caption;

			TextStyle thestyle = StyleFromCaption (stylecaption);

			if (thestyle == null)
			{
				StyleDefinition styledef = this.GetStyleDefinition (stylecaption);
				System.Diagnostics.Debug.Assert (styledef != null);

				thestyle = NewStyle (styledef);
			}

			return thestyle ;
		}


		private TextStyle NewStyle(StyleDefinition styledef)
		{
			TextContext context = this.story.TextContext;

			foreach (string basecaption in styledef.BaseStyleCaptions)
			{
				if (context.StyleList.StyleMap.GetTextStyle (basecaption) == null)
				{
					StyleDefinition basestyledef = GetStyleDefinition (basecaption);
					
					if (!basestyledef.IsDefaultStyle)
						this.NewStyle (basestyledef);
				}
			}

			Property[] properties = Property.DeserializeProperties (context, styledef.Serialized);

			ArrayList parents = new System.Collections.ArrayList();

			foreach (string basecaption in styledef.BaseStyleCaptions)
			{
				StyleDefinition basestyledef = GetStyleDefinition (basecaption);
				if (basestyledef.IsDefaultStyle)
				{
					if (basestyledef.TextStyleClass == TextStyleClass.Paragraph)
					{
						parents.Add (context.DefaultParagraphStyle);
					}
					else if (basestyledef.TextStyleClass == TextStyleClass.Text)
					{
						parents.Add (context.DefaultTextStyle);
					}
					else
					{
						System.Diagnostics.Debug.Assert (false, "Incorrect TextStyleClass in clipboard");
						return null;
					}
				}
				else
				{
					parents.Add (context.StyleList.StyleMap.GetTextStyle (basecaption));
				}
			}

			if (styledef.IsDefaultStyle)
			{
				if (styledef.TextStyleClass == TextStyleClass.Paragraph)
				{
					return context.DefaultParagraphStyle;
				}
				else if (styledef.TextStyleClass == TextStyleClass.Text)
				{
					return context.DefaultTextStyle;
				}
				else
				{
					System.Diagnostics.Debug.Assert (false, "Incorrect TextStyleClass in clipboard");
					return null;
				}
			}
			else
			{
				return this.CreateStyle (styledef.TextStyleClass, styledef.Caption, properties, parents);
			}
		}


		private TextStyle CreateStyle(TextStyleClass textStyleClass, string caption, Property[] properties, ArrayList parents)
		{
			TextContext context = this.story.TextContext;

			TextStyle style = context.StyleList.NewTextStyle (null, null, textStyleClass, properties, parents);

			context.StyleList.StyleMap.SetCaption (null, style, caption);

			// cherche le dernier rang
			int rank = 0;
			foreach (TextStyle thestyle in context.StyleList.StyleMap.GetSortedStyles ())
			{
				string s = thestyle.Name;
				s = context.StyleList.StyleMap.GetCaption (thestyle);
				if (thestyle.TextStyleClass == TextStyleClass.Paragraph)
					rank++;
			}

			context.StyleList.StyleMap.SetRank (null, style, rank);

			return style;
		}

		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private TextStory story;
		private PasteMode pasteMode;
		private Dictionary<string, int> usedTextStyles = new Dictionary<string, int> ();
		private int styleident = 0;

		private Dictionary<int,StyleDefinition> styleDefinitions;

		public Dictionary<int, StyleDefinition> StyleDefinitions
		{
			set
			{
				styleDefinitions = value;
			}
		}

		private object savedDefinedParagraph;

	}

}
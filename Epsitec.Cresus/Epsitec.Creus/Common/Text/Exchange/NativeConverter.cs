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
	public class NativeConverter
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
						basestylenames.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}/", SerializerSupport.SerializeString(basestylename));

						this.AddStyle (context, basestyle, stringstyles, true, processed);
					}

					bool isDefault = (thestyle == context.DefaultTextStyle) || (thestyle == context.DefaultParagraphStyle);
					string props = Property.SerializeProperties (thestyle.StyleProperties);

					int styleident = this.GetStyleIdent (stylecaption);
					output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}{5}", SerializerSupport.SerializeString (stylecaption), styleident, Misc.BoolToByte (isDefault), (byte) thestyle.TextStyleClass, basestylenames.ToString (), props);
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
		public string GetDefinedString(bool emitParagraphDefinitions, bool paragraphSep)
		{
			StringBuilder output = new StringBuilder ();

			output.Append ('[');

			Text.TextStyle[] styles = this.navigator.TextStyles;

			foreach (TextStyle style in styles)
			{
				if (style.TextStyleClass == TextStyleClass.Paragraph && emitParagraphDefinitions)
				{
					string caption = story.TextContext.StyleList.StyleMap.GetCaption (style);
					int styleident = GetStyleIdent (caption);
					output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "pstyle:{0}/", styleident);
				}

				if (style.TextStyleClass == TextStyleClass.Text)
				{
					string caption = story.TextContext.StyleList.StyleMap.GetCaption (style);
					int styleident = GetStyleIdent (caption);
					output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "cstyle:{0}/", styleident);
				}
			}

			if (paragraphSep)
			{
				output.Append ("par/");
			}

			if (emitParagraphDefinitions)
			{
				string definedPar = NativeConverter.GetParagraphDefinitions (this.paraWrapper);

				if (definedPar.Length != 0)
				{
					System.Diagnostics.Debug.Assert (definedPar.Contains ("/") == false);
					
					output.Append ("pardef:");
					output.Append (definedPar);
					output.Append ("/");
				}
			}

			NativeConverter.GetTextDefinitions (this.textWrapper, output);

			output.Append (']');
			return output.ToString ();
		}

		public static string GetTextDefinitions(Wrappers.TextWrapper wrapper)
		{
			System.Text.StringBuilder buffer = new StringBuilder ();
			buffer.Append ("[");
			NativeConverter.GetTextDefinitions (wrapper, buffer);
			buffer.Append ("]");
			return buffer.ToString ();
		}

		private static void GetTextDefinitions(Wrappers.TextWrapper wrapper, StringBuilder output)
		{
			if (wrapper.Defined.IsInvertItalicDefined && wrapper.Defined.InvertItalic)
			{
				output.Append ("i/");
			}

			if (wrapper.Defined.IsInvertBoldDefined && wrapper.Defined.InvertBold)
			{
				output.Append ("b/");
			}

			if (wrapper.Defined.IsColorDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "c:{0}/", wrapper.Defined.Color);
			}

			if (wrapper.Defined.IsFontFaceDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "fface:{0}/", wrapper.Defined.FontFace);
			}

			if (wrapper.Defined.IsFontStyleDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "fstyle:{0}/", wrapper.Defined.FontStyle);
			}

			if (wrapper.Defined.IsFontSizeDefined)
			{
				if (wrapper.Defined.IsUnitsDefined)
				{
					Properties.SizeUnits units = wrapper.Defined.Units;
					output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "funits:{0}/", (byte) wrapper.Defined.Units);
				}

				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "fsize:{0}/", wrapper.Defined.FontSize);
			}

			if (wrapper.Defined.IsFontFeaturesDefined)
			{
				string[] features = wrapper.Defined.FontFeatures;

				StringBuilder concatFeatures = new StringBuilder ();

				foreach (string feature in features)
				{
					concatFeatures.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}\\", feature);
				}

				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "ffeat:{0}/", concatFeatures.ToString ());
			}


			if (wrapper.Defined.IsUnderlineDefined)
			{
				string xlinedef = XlineDefinitionToString (wrapper.Defined.Underline);
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "u:{0}/", xlinedef);
			}

			if (wrapper.Defined.IsOverlineDefined)
			{
				string xlinedef = XlineDefinitionToString (wrapper.Defined.Overline);
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "o:{0}/", xlinedef);
			}

			if (wrapper.Defined.IsStrikeoutDefined)
			{
				string xlinedef = XlineDefinitionToString (wrapper.Defined.Strikeout);
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "s:{0}/", xlinedef);
			}

			if (wrapper.Defined.IsXscriptDefined)
			{
				string xscriptdef = XScriptDefinitionToString (wrapper.Defined.Xscript);
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "x:{0}/", xscriptdef);
			}

			if (wrapper.Defined.IsFontGlueDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "fontglue:{0}/", wrapper.Defined.FontGlue);
			}

			if (wrapper.Defined.IsLanguageHyphenationDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "langhyph:{0}/", wrapper.Defined.LanguageHyphenation);
			}

			if (wrapper.Defined.IsLanguageLocaleDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "langloc:{0}/", wrapper.Defined.LanguageLocale);
			}

			if (wrapper.Defined.IsLinkDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "link:{0}/", wrapper.Defined.Link);
			}
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

		public void SetDefinedString(string input, out bool paragraphSep)
		{
			paragraphSep = NativeConverter.SetDefininitions (this, this.textWrapper, this.paraWrapper, this.pasteMode, input);
		}

		public static bool SetTextDefinitions(Wrappers.TextWrapper textWrapper, string input)
		{
			if (string.IsNullOrEmpty (input))
			{
				return false;
			}
			else
			{
				return NativeConverter.SetDefininitions (null, textWrapper, null, PasteMode.KeepSource, input);
			}
		}

		private static bool SetDefininitions(NativeConverter that, Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, PasteMode pasteMode, string input)
		{
			char[] separators = new char[] {'/'};

			bool paragraphSep = false;

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

			textWrapper.SuspendSynchronizations ();

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
						textWrapper.Defined.InvertItalic = true;
						invertItalic = true;
						break;
					case "b":
						textWrapper.Defined.InvertBold = true;
						invertBold = true;
						break;
					case "u":
						StringToXlineDefinition (subelements[1], textWrapper.Defined.Underline);
						underline = true;
						break;
					case "o":
						StringToXlineDefinition (subelements[1], textWrapper.Defined.Overline);
						overline = true;
						break;
					case "s":
						StringToXlineDefinition (subelements[1], textWrapper.Defined.Strikeout);
						strikeout = true;
						break;
					case "x":
						StringToXScriptDefinition (subelements[1], textWrapper.Defined.Xscript);
						xscript = true;
						break;
					case "c":
						textWrapper.Defined.Color = subelements[1];
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
						paragraphSep = true;
					}
				}
				else if (notHandled && pasteMode == PasteMode.KeepSource)
				{
					switch (subelements[0])
					{
						case "cstyle":
							string styleident = subelements[1];

							//TextStyle thestyle = StyleFromCaption (stylecaption);
							TextStyle thestyle = that == null ? null : that.GetTextStyleToApply (styleident);

							if (thestyle != null)
							{
								textWrapper.ResumeSynchronizations ();
								that.navigator.SetTextStyles (thestyle);
								textWrapper.SuspendSynchronizations ();
							}
							break;
						case "pstyle":
							styleident = subelements[1];
							//thestyle = StyleFromCaption (stylecaption);
							thestyle = that == null ? null : that.GetTextStyleToApply (styleident);

							if (thestyle != null)
							{
								textWrapper.ResumeSynchronizations ();
								that.navigator.SetParagraphStyles (thestyle);
								textWrapper.SuspendSynchronizations ();
							}
							break;
						case "par":
							paragraphSep = true;
							break;
						case "pardef":
							if (paraWrapper != null)
							{
								if (that == null)
								{
									NativeConverter.SetParagraph (paraWrapper, subelements[1]);
								}
								else
								{
									that.savedDefinedParagraph = NativeConverter.SetParagraph (paraWrapper, subelements[1]);
								}
							}
							break;
						case "fface":
							textWrapper.Defined.FontFace = subelements[1];
							fontFace = true;
							break;
						case "fstyle":
							textWrapper.Defined.FontStyle = subelements[1];
							fontStyle = true;
							break;
						case "fsize":
							double size = Misc.ParseDouble (subelements[1]);
							textWrapper.Defined.FontSize = size;
							fontSize = true;
							break;
						case "funits":
							byte theunits = Misc.ParseByte (subelements[1]);
							textWrapper.Defined.Units = (Properties.SizeUnits) theunits;
							units = true;
							break;
						case "ffeat":
							char[] splitchars = { '\\' };
							string[] thefeatures = subelements[1].Split (splitchars, StringSplitOptions.RemoveEmptyEntries);
							textWrapper.Defined.FontFeatures = thefeatures;
							features = true;
							break;
						case "fontglue":
							textWrapper.Defined.FontGlue = Misc.ParseDouble (subelements[1]);
							fontglue = true;
							break;
						case "langhyph":
							textWrapper.Defined.LanguageHyphenation = Misc.ParseDouble (subelements[1]);
							languageHyphenation = true;
							break;
						case "langloc":
							textWrapper.Defined.LanguageLocale = subelements[1];
							languageLocale = true;
							break;
						case "link":
							textWrapper.Defined.Link = subelements[1];
							link = true;
							break;
					}
				}
			}

			if (!invertItalic)
				textWrapper.Defined.ClearInvertItalic ();

			if (!invertBold)
				textWrapper.Defined.ClearInvertBold ();

			if (!underline)
				textWrapper.Defined.ClearUnderline ();

			if (!overline)
				textWrapper.Defined.ClearOverline ();

			if (!strikeout)
				textWrapper.Defined.ClearStrikeout ();

			if (!xscript)
				textWrapper.Defined.ClearXscript ();

			if (!color)
				textWrapper.Defined.ClearColor ();

			if (pasteMode == PasteMode.KeepSource)
			{

				if (!fontFace)
					textWrapper.Defined.ClearFontFace ();

				if (!fontStyle)
					textWrapper.Defined.ClearFontStyle ();

				if (!fontSize)
					textWrapper.Defined.ClearFontSize ();

				if (!fontglue)
					textWrapper.Defined.ClearFontGlue ();

				if (!languageHyphenation)
					textWrapper.Defined.ClearLanguageHyphenation ();

				if (!languageLocale)
					textWrapper.Defined.ClearLanguageLocale ();

				if (!link)
					textWrapper.Defined.ClearLink ();

				if (!units)
					textWrapper.Defined.ClearUnits ();

				if (!features)
					textWrapper.Defined.ClearFontFeatures ();
			}

			textWrapper.ResumeSynchronizations ();

			return paragraphSep;
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
			//xlinedef.IsEmpty = Misc.ParseBool (elements[1])

			xlinedef.Position = Misc.ParseDouble (elements[2]);

			xlinedef.PositionUnits = (Properties.SizeUnits) Misc.ParseByte (elements[3]);

			xlinedef.Thickness = Misc.ParseDouble (elements[4]);

			xlinedef.ThicknessUnits = (Properties.SizeUnits) Misc.ParseByte (elements[5]);

			xlinedef.DrawClass = SerializerSupport.DeserializeString (elements[6]);

			xlinedef.DrawStyle = SerializerSupport.DeserializeString (elements[7]);
		}

		private static object SetParagraph(Wrappers.ParagraphWrapper paraWrapper, string parastring)
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


			paraWrapper.SuspendSynchronizations ();

			object savedDefinedParagraph = paraWrapper.Defined.SaveInternalState ();

			foreach (string element in elements)
			{
				string[] el = element.Split (innerseps, StringSplitOptions.RemoveEmptyEntries);

				switch (el[0])
				{
					case "align":
						paraWrapper.Defined.AlignMode = (Properties.AlignMode) Misc.ParseByte (el[1]);
						alignmode = true;
						break;
					case "just":
						paraWrapper.Defined.JustificationMode = (Wrappers.JustificationMode) Misc.ParseByte (el[1]);
						justificationmode = true;
						break;
					case "indleva":
						paraWrapper.Defined.IndentationLevelAttribute = SerializerSupport.DeserializeString (el[1]);
						indentationlevelattribute = true;
						break;
					case "indlev":
						paraWrapper.Defined.IndentationLevel = Misc.ParseInt (el[1]);
						indentationlevel = true;
						break;
					case "leading":
						paraWrapper.Defined.Leading = Misc.ParseDouble (el[1]);
						leading = true;
						break;
					case "leadingunit":
						paraWrapper.Defined.LeadingUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						leadingunits = true;
						break;
					case "leftmbody":
						paraWrapper.Defined.LeftMarginBody = Misc.ParseDouble (el[1]);
						leftmarginbody = true;
						break;
					case "rightmbody":
						paraWrapper.Defined.RightMarginBody = Misc.ParseDouble (el[1]);
						rightmarginbody = true;
						break;
					case "leftmfirst":
						paraWrapper.Defined.LeftMarginFirst = Misc.ParseDouble (el[1]);
						leftmarginfirst = true;
						break;
					case "rightmfirst":
						paraWrapper.Defined.RightMarginFirst = Misc.ParseDouble (el[1]);
						rightmarginfirst = true;
						break;
					case "marginunits":
						paraWrapper.Defined.MarginUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						marginunits = true;
						break;
					case "parstartmode":
						paraWrapper.Defined.ParagraphStartMode = (Properties.ParagraphStartMode) Misc.ParseByte (el[1]);
						pagargaphstartmode = true;
						break;
					case "spaceafter":
						paraWrapper.Defined.SpaceAfter = Misc.ParseDouble(el[1]) ;
						spaceafter = true ;
						break;
					case "spaceafteru":
						paraWrapper.Defined.SpaceAfterUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						spaceafterunits = true;
						break;
					case "spacebefore":
						paraWrapper.Defined.SpaceBefore = Misc.ParseDouble (el[1]);
						spacebefore = true;
						break;
					case "spacebeforeu":
						paraWrapper.Defined.SpaceBeforeUnits = (Properties.SizeUnits) Misc.ParseByte (el[1]);
						spacebeforeunits = true;
						break;
					case "bfa":
						paraWrapper.Defined.BreakFenceAfter = Misc.ParseDouble (el[1]);
						breakfenceafter = true;
						break;
					case "bfb":
						paraWrapper.Defined.BreakFenceBefore = Misc.ParseDouble (el[1]);
						breakfencebefore = true;
						break;
					case "keepel":
						paraWrapper.Defined.KeepEndLines = Misc.ParseInt (el[1]);
						keependlines = true;
						break;
					case "keepsl":
						paraWrapper.Defined.KeepStartLines = Misc.ParseInt (el[1]);
						keepstartlines = true;
						break;
					case "keepwithnext":
						paraWrapper.Defined.KeepWithNextParagraph = Misc.ParseBool (el[1]);
						keepwithnextparagraph = true;
						break;
					case "keepwithprev":
						paraWrapper.Defined.KeepWithPreviousParagraph = Misc.ParseBool (el[1]);
						keepwithpreviousparagraph = true;
						break;
					case "hyphen":
						paraWrapper.Defined.Hyphenation = Misc.ParseBool (el[1]);
						hyphenation = true;
						break;

				}
			}

			if (!hyphenation)
				paraWrapper.Defined.ClearHyphenation ();

			if (!keepwithnextparagraph)
				paraWrapper.Defined.ClearKeepWithNextParagraph ();

			if (!keepwithpreviousparagraph)
				paraWrapper.Defined.ClearKeepWithPreviousParagraph();

			if (!keependlines)
				paraWrapper.Defined.ClearKeepEndLines ();

			if (!keepstartlines)
				paraWrapper.Defined.ClearKeepStartLines ();

			if (!breakfenceafter)
				paraWrapper.Defined.ClearBreakFenceAfter ();

			if (!breakfencebefore)
				paraWrapper.Defined.ClearBreakFenceBefore ();

			if (!spaceafter)
				paraWrapper.Defined.ClearSpaceAfter() ;

			if (!spaceafterunits)
				paraWrapper.Defined.ClearSpaceAfterUnits ();

			if (!spacebefore)
				paraWrapper.Defined.ClearSpaceBefore ();

			if (!spacebeforeunits)
				paraWrapper.Defined.ClearSpaceBeforeUnits ();

			if (!pagargaphstartmode)
				paraWrapper.Defined.ClearParagraphStartMode ();

			if (!indentationlevel)
				paraWrapper.Defined.ClearIndentationLevel ();

			if (!indentationlevelattribute)
				paraWrapper.Defined.ClearIndentationLevelAttribute ();

			if (!alignmode)
				paraWrapper.Defined.ClearAlignMode ();

			if (!justificationmode)
				paraWrapper.Defined.ClearJustificationMode ();

			if (!leading)
				paraWrapper.Defined.ClearLeading ();

			if (!leadingunits)
				paraWrapper.Defined.ClearLeadingUnits ();

			if (!leftmarginbody)
				paraWrapper.Defined.ClearLeftMarginBody ();

			if (!rightmarginbody)
				paraWrapper.Defined.ClearRightMarginBody ();

			if (!leftmarginfirst)
				paraWrapper.Defined.ClearLeftMarginFirst ();

			if (!rightmarginfirst)
				paraWrapper.Defined.ClearRightMarginFirst ();

			if (!marginunits)
				paraWrapper.Defined.ClearMarginUnits ();

			paraWrapper.ResumeSynchronizations ();

			return savedDefinedParagraph;
		}


		public static string GetParagraphDefinitions(Wrappers.ParagraphWrapper wrapper)
		{
			StringBuilder output = new StringBuilder ();

			if (wrapper.Defined.IsAlignModeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "align|{0}\\", (byte) wrapper.Defined.AlignMode);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsJustificationModeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "just|{0}", (byte) wrapper.Defined.JustificationMode);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsIndentationLevelAttributeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "indleva|{0}", SerializerSupport.SerializeString (wrapper.Defined.IndentationLevelAttribute));
				output.Append ('\\');
			}

			if (wrapper.Defined.IsIndentationLevelDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "indlev|{0}", wrapper.Defined.IndentationLevel);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsLeadingDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "leading|{0}", wrapper.Defined.Leading);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsLeadingUnitsDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "leadingunit|{0}", (byte) wrapper.Defined.LeadingUnits);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsLeftMarginBodyDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "leftmbody|{0}", wrapper.Defined.LeftMarginBody);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsRightMarginBodyDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "rightmbody|{0}", wrapper.Defined.RightMarginBody);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsLeftMarginFirstDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "leftmfirst|{0}", wrapper.Defined.LeftMarginFirst);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsRightMarginFirstDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "rightmfirst|{0}", wrapper.Defined.RightMarginFirst);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsMarginUnitsDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "marginunits|{0}", (byte) wrapper.Defined.MarginUnits);
				output.Append ('\\');
			}

#if false
			if (wrapper.Defined.IsManagedParagraphDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "managedp|{0}", wrapper.Defined.ManagedParagraph);
			}
#endif

			if (wrapper.Defined.IsParagraphStartModeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "parstartmode|{0}", (byte) wrapper.Defined.ParagraphStartMode);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsSpaceAfterDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "spaceafter|{0}", (byte) wrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "spaceafteru|{0}", (byte) wrapper.Defined.SpaceAfterUnits);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsSpaceBeforeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "spacebefore|{0}", (byte) wrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "spacebeforeu|{0}", (byte) wrapper.Defined.SpaceBeforeUnits);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsBreakFenceAfterDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "bfa|{0}", wrapper.Defined.BreakFenceAfter);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsBreakFenceBeforeDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "bfb|{0}", wrapper.Defined.BreakFenceBefore);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsHyphenationDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "hyphen|{0}", Misc.BoolToByte (wrapper.Defined.Hyphenation));
				output.Append ('\\');
			}

			if (wrapper.Defined.IsItemListInfoDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "itemli|{0}", wrapper.Defined.ItemListInfo);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsKeepEndLinesDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "keepel|{0}", wrapper.Defined.KeepEndLines);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsKeepStartLinesDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "keepsl|{0}", wrapper.Defined.KeepStartLines);
				output.Append ('\\');
			}

			if (wrapper.Defined.IsKeepWithNextParagraphDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "keepwithnext|{0}", Misc.BoolToByte (wrapper.Defined.KeepWithNextParagraph));
				output.Append ('\\');
			}

			if (wrapper.Defined.IsKeepWithPreviousParagraphDefined)
			{
				output.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "keepwithprev|{0}", Misc.BoolToByte (wrapper.Defined.KeepWithPreviousParagraph));
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

			TextStyle style = context.StyleList.NewTextStyle (this.story.OpletQueue, null, textStyleClass, properties, parents);

			context.StyleList.StyleMap.SetCaption (this.story.OpletQueue, style, caption);

			// cherche le dernier rang
			int rank = 0;
			foreach (TextStyle thestyle in context.StyleList.StyleMap.GetSortedStyles ())
			{
				string s = thestyle.Name;
				s = context.StyleList.StyleMap.GetCaption (thestyle);
				if (thestyle.TextStyleClass == TextStyleClass.Paragraph)
					rank++;
			}

			context.StyleList.StyleMap.SetRank (this.story.OpletQueue, style, rank);

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

		internal Dictionary<int, StyleDefinition> StyleDefinitions
		{
			set
			{
				styleDefinitions = value;
			}
		}

		private object savedDefinedParagraph;

	}

}
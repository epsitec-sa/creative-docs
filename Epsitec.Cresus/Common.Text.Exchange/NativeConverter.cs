using System;
using System.Collections.Generic;
using System.Text;

// Conversion entre format presse-papier natif et TextWrapper
// Responsable: Michael Walz

// Choses qui restent à faire:
// - gérer les propriétés suivantes : TextMarker, TextBox, UserTags, Conditions
// - propriété link: gèrer les / et autres signes cabalistiques dans les hyperliens
//
// - copier/coller interdocuments
//   - copier les définitions de styles 
//   - gérer la création des styles s'ils n'existent pas
//
// - gestion des paragraphes modifiés "à la main"
//

namespace Epsitec.Common.Text.Exchange
{
	class NativeConverter
	{

		private static TextStyle StyleFromCaption(string caption, TextStory story)
		{
			TextStyle[] styles = story.TextContext.StyleList.StyleMap.GetSortedStyles ();

			TextStyle thenewstyle = null;
			foreach (TextStyle thestyle in styles)
			{
				if (story.TextContext.StyleList.StyleMap.GetCaption (thestyle) == caption)
				{
					return thestyle ;
				}
			}

			return null ;
		}

		private static string XScriptDefinitionToString(Wrappers.TextWrapper.XscriptDefinition xscriptdef)
		{
			StringBuilder output = new StringBuilder ();

			output.Append (Misc.BoolToByte (xscriptdef.IsDisabled));
			output.Append ('\\');
			output.Append (Misc.BoolToByte (xscriptdef.IsEmpty));
			output.Append ('\\');
			output.Append (xscriptdef.Offset);
			output.Append ('\\');
			output.Append (xscriptdef.Scale);
			output.Append ('\\');

			return output.ToString ();
		}

		static void StringToXScriptDefinition(string strxline, Wrappers.TextWrapper.XscriptDefinition xscriptdef)
		{
			char[] sep = { '\\' };
			string[] elements = strxline.Split (sep, StringSplitOptions.None);

			byte b;
			double d;

			b = byte.Parse (elements[0]);
			xscriptdef.IsDisabled = Misc.ByteToBool (b);

			b = byte.Parse (elements[1]);
			//	xscriptdef.IsEmpty = Misc.byteTobool (b);

			xscriptdef.Offset = Misc.ParseDouble (elements[2]);

			xscriptdef.Scale = Misc.ParseDouble (elements[3]);
		}


		private static string XlineDefinitionToString(Wrappers.TextWrapper.XlineDefinition xlinedef)
		{
			StringBuilder output = new StringBuilder() ;

			output.Append (Misc.BoolToByte (xlinedef.IsDisabled));
			output.Append ('\\');
			output.Append (Misc.BoolToByte (xlinedef.IsEmpty));
			output.Append ('\\');
			output.Append (xlinedef.Position);
			output.Append ('\\');
			output.Append ((byte) xlinedef.PositionUnits);
			output.Append ('\\');
			output.Append (xlinedef.Thickness);
			output.Append ('\\');
			output.Append ((byte) xlinedef.ThicknessUnits);
			output.Append ('\\');
			output.Append (Misc.StringNull(xlinedef.DrawClass));
			output.Append ('\\');
			output.Append (Misc.StringNull(xlinedef.DrawStyle));
			output.Append ('\\');

			return output.ToString();
		}

		private static void StringToXlineDefinition(string strxline, Wrappers.TextWrapper.XlineDefinition xlinedef)
		{
			char[] sep = { '\\' };
			string [] elements = strxline.Split (sep, StringSplitOptions.None);

			byte b ;
			double d;

			b = byte.Parse(elements[0]) ;
			xlinedef.IsDisabled = Misc.ByteToBool (b);

			b = byte.Parse (elements[1]);
		//	xlinedef.IsEmpty = Misc.byteTobool (b);

			xlinedef.Position = Misc.ParseDouble (elements[2]);

			b = byte.Parse(elements[3]) ;
			xlinedef.PositionUnits = (Properties.SizeUnits) b;

			xlinedef.Thickness = Misc.ParseDouble (elements[4]);

			b = byte.Parse (elements[5]);
			xlinedef.ThicknessUnits = (Properties.SizeUnits) b;

			xlinedef.DrawClass = Misc.NullString(elements[6]);

			xlinedef.DrawStyle = Misc.NullString(elements[7]);
		}

		private static void SetParagraph(Wrappers.ParagraphWrapper parawrapper, string parastring)
		{
			char[] seps = { '\\' };
			char[] innerseps = {'|'};
			string []elements = parastring.Split(seps, StringSplitOptions.RemoveEmptyEntries) ;

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

			parawrapper.SuspendSynchronizations ();

			foreach (string element in elements)
			{
				string [] el = element.Split(innerseps, StringSplitOptions.RemoveEmptyEntries) ;

				switch (el[0])
				{
					case "indlev":
						parawrapper.Defined.IndentationLevel = Misc.ParseInt (el[1]);
						indentationlevel = true;
						break;
					case "indleva":
						parawrapper.Defined.IndentationLevelAttribute = Misc.NullString (el[1]);
						indentationlevelattribute = true;
						break;
					case "align":
						parawrapper.Defined.AlignMode = (Properties.AlignMode)byte.Parse (el[1]);
						alignmode = true;
						break;
					case "just":
						parawrapper.Defined.JustificationMode = (Wrappers.JustificationMode) byte.Parse (el[1]);
						justificationmode = true;
						break;
					case "leading":
						parawrapper.Defined.Leading = Misc.ParseDouble (el[1]);
						leading = true;
						break;
					case "leadingunit":
						parawrapper.Defined.LeadingUnits = (Properties.SizeUnits) byte.Parse (el[1]);
						leadingunits = true;
						break;
					case "leftmbody":
						parawrapper.Defined.LeftMarginBody = Misc.ParseDouble (el[1]);
						leftmarginbody = true;
						break;
					case "rightmbody":
						parawrapper.Defined.RightMarginBody = Misc.ParseDouble (el[1]);
						rightmarginbody = true;
						break;
					case "leftmfirst":
						parawrapper.Defined.LeftMarginFirst = Misc.ParseDouble (el[1]);
						leftmarginfirst = true;
						break;
					case "rightmfirst":
						parawrapper.Defined.RightMarginFirst = Misc.ParseDouble (el[1]);
						rightmarginfirst = true;
						break;
					case "marginunits":
						parawrapper.Defined.MarginUnits = (Properties.SizeUnits) byte.Parse (el[1]);
						marginunits = true;
						break;
						
				}
			}

			if (!indentationlevel)
				parawrapper.Defined.ClearIndentationLevel ();

			if (!indentationlevelattribute)
				parawrapper.Defined.ClearIndentationLevelAttribute ();

			if (!alignmode)
				parawrapper.Defined.ClearAlignMode ();

			if (!justificationmode)
				parawrapper.Defined.ClearJustificationMode ();

			if (!leading)
				parawrapper.Defined.ClearLeading ();

			if (!leadingunits)
				parawrapper.Defined.ClearLeadingUnits ();

			if (!leftmarginbody)
				parawrapper.Defined.ClearLeftMarginBody();

			if (!rightmarginbody)
				parawrapper.Defined.ClearRightMarginBody ();

			if (!leftmarginfirst)
				parawrapper.Defined.ClearLeftMarginFirst ();

			if (!rightmarginfirst)
				parawrapper.Defined.ClearRightMarginFirst ();

			if (!marginunits)
				parawrapper.Defined.ClearMarginUnits();

			parawrapper.ResumeSynchronizations ();
		}


		private static string GetDefinedParString(Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, TextNavigator navigator, TextStory story)
		{
			StringBuilder output = new StringBuilder ();

			if (paraWrapper.Defined.IsAlignModeDefined)
			{
				output.AppendFormat ("align|{0}\\", (byte) paraWrapper.Defined.AlignMode);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsJustificationModeDefined)
			{
				output.AppendFormat ("just|{0}", (byte) paraWrapper.Defined.JustificationMode);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsIndentationLevelAttributeDefined)
			{
				output.AppendFormat ("indleva|{0}", Misc.StringNull(paraWrapper.Defined.IndentationLevelAttribute));
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsIndentationLevelDefined)
			{
				output.AppendFormat ("indlev|{0}", paraWrapper.Defined.IndentationLevel);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsLeadingDefined)
			{
				output.AppendFormat ("leading|{0}", paraWrapper.Defined.Leading);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsLeadingUnitsDefined)
			{
				output.AppendFormat ("leadingunit|{0}", (byte) paraWrapper.Defined.LeadingUnits);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsLeftMarginBodyDefined)
			{
				output.AppendFormat ("leftmbody|{0}", paraWrapper.Defined.LeftMarginBody);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsRightMarginBodyDefined)
			{
				output.AppendFormat ("rightmbody|{0}", paraWrapper.Defined.RightMarginBody);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsLeftMarginFirstDefined)
			{
				output.AppendFormat ("leftmfirst|{0}", paraWrapper.Defined.LeftMarginFirst);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsRightMarginFirstDefined)
			{
				output.AppendFormat ("rightmfirst|{0}", paraWrapper.Defined.RightMarginFirst);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsMarginUnitsDefined)
			{
				output.AppendFormat ("marginunits|{0}", (byte) paraWrapper.Defined.MarginUnits);
				output.Append ('\\');
			}

#if false
			if (paraWrapper.Defined.IsManagedParagraphDefined)
			{
				output.AppendFormat ("managedp|{0}", paraWrapper.Defined.ManagedParagraph);
			}
#endif

			if (paraWrapper.Defined.IsParagraphStartModeDefined)
			{
				output.AppendFormat ("parstartmode|{0}", (byte) paraWrapper.Defined.ParagraphStartMode);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsSpaceAfterDefined)
			{
				output.AppendFormat ("spaceafter|{0}", (byte) paraWrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat ("spaceafteru|{0}", (byte) paraWrapper.Defined.SpaceAfterUnits);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsSpaceBeforeDefined)
			{
				output.AppendFormat ("spacebefore|{0}", (byte) paraWrapper.Defined.SpaceAfter);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsSpaceAfterUnitsDefined)
			{
				output.AppendFormat ("spacebeforeu|{0}", (byte) paraWrapper.Defined.SpaceBeforeUnits);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsBreakFenceAfterDefined)
			{
				output.AppendFormat ("bfa|{0}", paraWrapper.Defined.BreakFenceAfter);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsHyphenationDefined)
			{
				output.AppendFormat ("hy|{0}", Misc.BoolToByte (paraWrapper.Defined.Hyphenation));
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsBreakFenceBeforeDefined)
			{
				output.AppendFormat ("bfb|{0}", paraWrapper.Defined.BreakFenceBefore);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsItemListInfoDefined)
			{
				output.AppendFormat ("itemli|{0}", paraWrapper.Defined.ItemListInfo);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsKeepEndLinesDefined)
			{
				output.AppendFormat ("keepel|{0}", paraWrapper.Defined.KeepEndLines);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsKeepStartLinesDefined)
			{
				output.AppendFormat ("keepsl|{0}", paraWrapper.Defined.KeepStartLines);
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsKeepWithNextParagraphDefined)
			{
				output.AppendFormat ("keepwnp|{0}", Misc.BoolToByte (paraWrapper.Defined.KeepWithNextParagraph));
				output.Append ('\\');
			}

			if (paraWrapper.Defined.IsKeepWithPreviousParagraphDefined)
			{
				output.AppendFormat ("keepwpp|{0}", Misc.BoolToByte (paraWrapper.Defined.KeepWithPreviousParagraph));
				output.Append ('\\');
			}

			return output.ToString ();
		}


		/// <summary>
		/// Convertit les attributs d'un textwrapper en format presse-papier natif
		/// </summary>
		/// <param name="textwrapper"></param>
		/// <returns></returns>
		public static string GetDefinedString(Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, TextNavigator navigator, TextStory story, bool paragraphSep)
		{
			StringBuilder output = new StringBuilder();

			output.Append ('[');

			Text.TextStyle[] styles = navigator.TextStyles;

			foreach (TextStyle style in styles)
			{
				string caption = story.TextContext.StyleList.StyleMap.GetCaption (style);

				if (style.TextStyleClass == TextStyleClass.Paragraph && paragraphSep)
				{
					output.AppendFormat ("pstyle:{0}/", caption);
				}

				if (style.TextStyleClass == TextStyleClass.Text)
				{
					output.AppendFormat ("cstyle:{0}/", caption);
				}
			}

			if (paragraphSep)
			{
				output.Append("par/") ;
				string definedPar = GetDefinedParString(textWrapper, paraWrapper, navigator, story) ;

				if (definedPar.Length != 0)
				{
					output.AppendFormat("pardef:{0}/", definedPar) ;
				}
			}

			if (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic)
			{
				output.Append ("i/");
			}

			if (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold)
			{
				output.Append ("b/");
			}

			if (textWrapper.Defined.IsColorDefined)
			{
				output.AppendFormat ("c:{0}/", textWrapper.Defined.Color);
			}

			if (textWrapper.Defined.IsFontFaceDefined)
			{
				output.AppendFormat ("fface:{0}/", textWrapper.Defined.FontFace);
			}

			if (textWrapper.Defined.IsFontStyleDefined)
			{
				output.AppendFormat ("fstyle:{0}/", textWrapper.Defined.FontStyle);
			}

			if (textWrapper.Defined.IsFontSizeDefined)
			{
				if (textWrapper.Defined.IsUnitsDefined)
				{
					Properties.SizeUnits units = textWrapper.Defined.Units;
					output.AppendFormat ("funits:{0}/", (byte)textWrapper.Defined.Units);
				}

				output.AppendFormat ("fsize:{0}/", textWrapper.Defined.FontSize);
			}

			if (textWrapper.Defined.IsFontFeaturesDefined)
			{
				string [] features = textWrapper.Defined.FontFeatures ;

				StringBuilder conacatfeatures = new StringBuilder ();

				foreach (string feature in features)
				{
					conacatfeatures.AppendFormat ("{0}\\", feature);
				}

				output.AppendFormat ("ffeat:{0}/", conacatfeatures.ToString());
			}


			if (textWrapper.Defined.IsUnderlineDefined)
			{
				string xlinedef = XlineDefinitionToString (textWrapper.Defined.Underline);
				output.AppendFormat ("u:{0}/",xlinedef);
			}

			if (textWrapper.Defined.IsOverlineDefined)
			{
				string xlinedef = XlineDefinitionToString (textWrapper.Defined.Overline);
				output.AppendFormat ("o:{0}/", xlinedef);
			}

			if (textWrapper.Defined.IsStrikeoutDefined)
			{
				string xlinedef = XlineDefinitionToString (textWrapper.Defined.Strikeout);
				output.AppendFormat ("s:{0}/", xlinedef);
			}

			if (textWrapper.Defined.IsXscriptDefined)
			{
				string xscriptdef = XScriptDefinitionToString (textWrapper.Defined.Xscript);
				output.AppendFormat ("x:{0}/", xscriptdef);
			}

			if (textWrapper.Defined.IsFontGlueDefined)
			{
				output.AppendFormat ("fontglue:{0}/", textWrapper.Defined.FontGlue);
			}

			if (textWrapper.Defined.IsLanguageHyphenationDefined)
			{
				output.AppendFormat("langhyph:{0}/", textWrapper.Defined.LanguageHyphenation) ;
			}

			if (textWrapper.Defined.IsLanguageLocaleDefined)
			{
				output.AppendFormat ("langloc:{0}/", textWrapper.Defined.LanguageLocale);
			}

			if (textWrapper.Defined.IsLinkDefined)
			{
				output.AppendFormat ("link:{0}/", textWrapper.Defined.Link);
			}

			output.Append (']');
			return output.ToString() ;
		}

		public static void SetDefined(Wrappers.TextWrapper textwrapper, Wrappers.ParagraphWrapper parawrapper, TextNavigator navigator, TextStory story, string input, out bool paragrpahSep)
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

			textwrapper.SuspendSynchronizations ();

			System.Diagnostics.Debug.Assert (input[0] == '[' && input[input.Length-1] == ']');
			input = input.Substring(1, input.Length - 2) ;

			string[] elements = input.Split (separators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string elem in elements)
			{
				string[] subelements = elem.Split (':');
				switch (subelements[0])
				{
					case "cstyle":
						string stylecaption = subelements[1];
						TextStyle thestyle = StyleFromCaption (stylecaption, story);

						if (thestyle != null)
						{
							navigator.SetTextStyles (thestyle);
						}
						break;
					case "pstyle":
						stylecaption = subelements[1];
						thestyle = StyleFromCaption (stylecaption, story);

						if (thestyle != null)
						{
							navigator.SetParagraphStyles (thestyle);
						}
						break;
					case "par":
						paragrpahSep = true;
						break;
					case "pardef":
						SetParagraph (parawrapper, subelements[1]);
						break;
					case "i":
						textwrapper.Defined.InvertItalic = true;
						invertItalic = true;
						break;
					case "b":
						textwrapper.Defined.InvertBold = true;
						invertBold = true;
						break;
					case "u":
						StringToXlineDefinition (subelements[1], textwrapper.Defined.Underline);
						underline = true;
						break;
					case "o":
						StringToXlineDefinition (subelements[1], textwrapper.Defined.Overline);
						overline = true;
						break;
					case "s":
						StringToXlineDefinition (subelements[1], textwrapper.Defined.Strikeout);
						strikeout = true;
						break;
					case "x":
						StringToXScriptDefinition(subelements[1], textwrapper.Defined.Xscript);
						xscript = true;
						break;
					case "c":
						textwrapper.Defined.Color = subelements[1];
						color = true;
						break;
					case "fface":
						textwrapper.Defined.FontFace = subelements[1];
						fontFace = true;
						break;
					case "fstyle":
						textwrapper.Defined.FontStyle = subelements[1];
						fontStyle = true;
						break;
					case "fsize":
						double size = double.Parse(subelements[1],System.Globalization.NumberStyles.Float) ;
						textwrapper.Defined.FontSize = size;
						fontSize = true;
						break;
					case "funits":
						byte theunits = byte.Parse (subelements[1]);
						textwrapper.Defined.Units = (Properties.SizeUnits) theunits;
						units = true;
						break;
					case "ffeat":
						char[] splitchars = {'\\'};
						string [] thefeatures = subelements[1].Split(splitchars, StringSplitOptions.RemoveEmptyEntries) ;
						textwrapper.Defined.FontFeatures = thefeatures;
						features = true;
						break;
					case "fontglue":
						textwrapper.Defined.FontGlue = double.Parse (subelements[1], System.Globalization.NumberStyles.Float);
						fontglue = true;
						break;
					case "langhyph":
						textwrapper.Defined.LanguageHyphenation = double.Parse (subelements[1], System.Globalization.NumberStyles.Float);
						languageHyphenation = true;
						break;
					case "langloc":
						textwrapper.Defined.LanguageLocale = subelements[1];
						languageLocale = true;
						break;
					case "link":
						textwrapper.Defined.Link = subelements[1];
						link = true;
						break;
				}
			}

			if (!invertItalic)
				textwrapper.Defined.ClearInvertItalic ();

			if (!invertBold)
				textwrapper.Defined.ClearInvertBold ();

			if (!underline)
				textwrapper.Defined.ClearUnderline ();

			if (!overline)
				textwrapper.Defined.ClearOverline ();

			if (!strikeout)
				textwrapper.Defined.ClearStrikeout ();

			if (!xscript)
				textwrapper.Defined.ClearXscript();

			if (!color)
				textwrapper.Defined.ClearColor ();

			if (!fontFace)
				textwrapper.Defined.ClearFontFace ();

			if (!fontStyle)
				textwrapper.Defined.ClearFontStyle ();

			if (!fontSize)
				textwrapper.Defined.ClearFontSize ();

			if (!fontglue)
				textwrapper.Defined.ClearFontGlue ();

			if (!languageHyphenation)
				textwrapper.Defined.ClearLanguageHyphenation ();

			if (!languageLocale)
				textwrapper.Defined.ClearLanguageLocale ();

			if (!link)
				textwrapper.Defined.ClearLink ();

			if (!units)
				textwrapper.Defined.ClearUnits ();

			if (!features)
				textwrapper.Defined.ClearFontFeatures ();

			textwrapper.ResumeSynchronizations ();
		}
	}

}
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Wrappers.
	/// </summary>
	public class Wrappers
	{
		public Wrappers(Document document)
		{
			this.document = document;

			this.textWrapper      = new Text.Wrappers.TextWrapper();
			this.paragraphWrapper = new Text.Wrappers.ParagraphWrapper();

			this.styleTextWrapper      = new Text.Wrappers.TextWrapper();
			this.styleParagraphWrapper = new Text.Wrappers.ParagraphWrapper();

			this.textWrapper.Active.Changed  += new EventHandler(this.HandleTextWrapperChanged);
			this.textWrapper.Defined.Changed += new EventHandler(this.HandleTextWrapperChanged);

			this.paragraphWrapper.Active.Changed  += new EventHandler(this.HandleParagraphWrapperChanged);
			this.paragraphWrapper.Defined.Changed += new EventHandler(this.HandleParagraphWrapperChanged);

			this.document.TextContext.StyleList.StyleRedefined +=  new EventHandler(this.HandleStyleWrapperChanged);
		}


		public Text.Wrappers.TextWrapper TextWrapper
		{
			//	Wrapper pour la fonte.
			get { return this.textWrapper; }
		}

		public Text.Wrappers.ParagraphWrapper ParagraphWrapper
		{
			//	Wrapper pour le paragraphe.
			get { return this.paragraphWrapper; }
		}


		public Text.Wrappers.TextWrapper StyleTextWrapper
		{
			//	Wrapper pour les styles de caractère.
			get { return this.styleTextWrapper; }
		}

		public Text.Wrappers.ParagraphWrapper StyleParagraphWrapper
		{
			//	Wrapper pour les styles de paragraphe.
			get { return this.styleParagraphWrapper; }
		}


		public bool IsWrappersAttached
		{
			//	Indique si les wrappers sont attachés.
			get
			{
				return this.textWrapper.IsAttached;
			}
		}

		public void WrappersAttach(TextFlow textFlow)
		{
			//	Attache tous les wrappers à un texte.
			this.textFlow = textFlow;
			this.textWrapper.Attach(textFlow.TextNavigator);
			this.paragraphWrapper.Attach(textFlow.TextNavigator);
		}

		public void WrappersDetach()
		{
			//	Détache tous les wrappers.
			this.textFlow = null;
			this.textWrapper.Detach();
			this.paragraphWrapper.Detach();
		}

		public TextFlow TextFlow
		{
			//	Donne le TextFlow en cours d'édition.
			get
			{
				return this.textFlow;
			}
		}


		public void UpdateCommands()
		{
			//	Met à jour toutes les commandes.
			this.HandleTextWrapperChanged(null);
			this.HandleParagraphWrapperChanged(null);
		}


		public void UpdateQuickFonts()
		{
			//	Met à jour toutes les polices rapides, lorsqu'un changement dans les réglages a été fait.
			//	Appelé par DocumentEditor lorsque la notification FontsSettingsChanged est reçue.
			System.Collections.ArrayList quickFonts = this.document.Settings.QuickFonts;
			if ( quickFonts.Count == 0 )
			{
				this.quickFonts = null;
			}
			else
			{
				int quickCount;
				this.quickFonts = Misc.MergeFontList(Misc.GetFontList(false), quickFonts, true, null, out quickCount);
			}
		}

		public OpenType.FontIdentity GetQuickFonts(int rank)
		{
			//	Donne une police rapide.
			if ( this.quickFonts == null )  return null;
			if ( rank >= this.quickFonts.Count )  return null;
			return this.quickFonts[rank] as OpenType.FontIdentity;
		}


		public void UpdateQuickButtons()
		{
			//	Met à jour toutes les commandes pour les polices rapides.
			this.UpdateQuickButton(0);
			this.UpdateQuickButton(1);
			this.UpdateQuickButton(2);
			this.UpdateQuickButton(3);
		}

		public void UpdateQuickButton(int i)
		{
			//	Met à jour une commande pour les polices rapides.
			//	Commandes "FontQuick1" à "FontQuick4":
			if ( this.document.CommandDispatcher == null )  return;
			string cmd = string.Format("FontQuick{0}", (i+1).ToString(System.Globalization.CultureInfo.InvariantCulture));
			CommandState cs = this.document.CommandDispatcher.GetCommandState(cmd);
			if ( cs == null )  return;

			if ( this.IsWrappersAttached )
			{
				OpenType.FontIdentity id = this.GetQuickFonts(i);
				if ( id == null )
				{
					cs.Enable = false;  // pas de police rapide définie pour ce bouton
					cs.ActiveState = ActiveState.No;
				}
				else
				{
					cs.Enable = true;

					string face = this.textWrapper.Active.FontFace;
					cs.ActiveState = (face == id.InvariantFaceName) ? ActiveState.Yes : ActiveState.No;
				}
			}
			else
			{
				cs.Enable = false;  // pas de texte en édition
				cs.ActiveState = ActiveState.No;
			}
		}

		protected void HandleTextWrapperChanged(object sender)
		{
			//	Le wrapper du texte a changé.
			bool enabled = this.IsWrappersAttached;
			bool bold        = false;
			bool italic      = false;
			bool underlined  = false;
			bool overlined   = false;
			bool strikeout   = false;
			bool subscript   = false;
			bool superscript = false;

			if ( enabled )
			{
				bold        = this.BoldActiveState;
				italic      = this.ItalicActiveState;
				underlined  = this.UnderlinedActiveState;
				overlined   = this.OverlinedActiveState;
				strikeout   = this.StrikeoutActiveState;
				subscript   = this.SubscriptActiveState;
				superscript = this.SuperscriptActiveState;
			}

			this.CommandActiveState("Paste", enabled);
			this.CommandActiveState("Glyphs", enabled);
			this.CommandActiveState("GlyphsInsert", enabled);
			this.CommandActiveState("TextEditing", enabled);
			this.CommandActiveState("TextShowControlCharacters", enabled);
			this.CommandActiveState("TextInsertQuad", enabled);
			this.CommandActiveState("TextInsertNewFrame", enabled);
			this.CommandActiveState("TextInsertNewPage", enabled);

			this.CommandActiveState("FontBold",        enabled, bold       );
			this.CommandActiveState("FontItalic",      enabled, italic     );
			this.CommandActiveState("FontUnderlined",  enabled, underlined );
			this.CommandActiveState("FontOverlined",   enabled, overlined  );
			this.CommandActiveState("FontStrikeout",   enabled, strikeout  );
			this.CommandActiveState("FontSubscript",   enabled, subscript  );
			this.CommandActiveState("FontSuperscript", enabled, superscript);

			this.CommandActiveState("FontSizePlus",  enabled);
			this.CommandActiveState("FontSizeMinus", enabled);
			this.CommandActiveState("FontClear",     enabled);

			this.UpdateQuickButtons();
		}

		protected void HandleParagraphWrapperChanged(object sender)
		{
			//	Le wrapper des paragraphes a changé.
			bool enabled = this.IsWrappersAttached;
			double leading = 0.0;
			Common.Text.Wrappers.JustificationMode justif = Text.Wrappers.JustificationMode.Unknown;

			if ( enabled )
			{
				if ( this.paragraphWrapper.Active.LeadingUnits == Text.Properties.SizeUnits.Percent )
				{
					leading = this.paragraphWrapper.Active.Leading;
				}

				justif = this.paragraphWrapper.Active.JustificationMode;
			}

			this.CommandActiveState("ParagraphLeading08", enabled, (leading == 0.8));
			this.CommandActiveState("ParagraphLeading10", enabled, (leading == 1.0));
			this.CommandActiveState("ParagraphLeading12", enabled, (leading == 1.2));
			this.CommandActiveState("ParagraphLeading15", enabled, (leading == 1.5));
			this.CommandActiveState("ParagraphLeading20", enabled, (leading == 2.0));
			this.CommandActiveState("ParagraphLeading30", enabled, (leading == 3.0));
			this.CommandActiveState("ParagraphLeadingPlus",  enabled);
			this.CommandActiveState("ParagraphLeadingMinus", enabled);

			this.CommandActiveState("JustifHLeft",   enabled, (justif == Text.Wrappers.JustificationMode.AlignLeft));
			this.CommandActiveState("JustifHCenter", enabled, (justif == Text.Wrappers.JustificationMode.Center));
			this.CommandActiveState("JustifHRight",  enabled, (justif == Text.Wrappers.JustificationMode.AlignRight));
			this.CommandActiveState("JustifHJustif", enabled, (justif == Text.Wrappers.JustificationMode.JustifyAlignLeft));
			this.CommandActiveState("JustifHAll",    enabled, (justif == Text.Wrappers.JustificationMode.JustifyJustfy));

			this.CommandActiveState("ParagraphIndentPlus",  enabled);
			this.CommandActiveState("ParagraphIndentMinus", enabled);
			this.CommandActiveState("ParagraphClear",       enabled);
		}

		protected void HandleStyleWrapperChanged(object sender)
		{
			//	Un style de paragraphe ou de caractère a changé.
			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle style in styles )
			{
				this.document.Notifier.NotifyTextStyleChanged(style);
			}

			this.document.Notifier.NotifyArea();
		}


		#region Style Check
		public bool IsFreeName(Text.TextStyle style, string name)
		{
			//	Vérifie si un nom est possible pour un style donné.
			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle existing in styles )
			{
				if ( existing == style )  continue;
				if ( existing.TextStyleClass != style.TextStyleClass )  continue;

				if ( name == this.document.TextContext.StyleList.StyleMap.GetCaption(existing) )
				{
					return false;
				}
			}
			return true;
		}

		public void StyleArrangeAll()
		{
			//	Passe en revue tous les styles pour réarranger l'ordre des parents.
			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle style in styles )
			{
				System.Collections.ArrayList parents = new System.Collections.ArrayList();
				parents.AddRange(style.ParentStyles);
				parents = this.ArrangeParentStyles(parents);
				this.document.TextContext.StyleList.RedefineTextStyle(this.document.Modifier.OpletQueue, style, style.StyleProperties, parents);
			}
		}

		public void StyleCheckAllDefaultParent()
		{
			//	Passe en revue tous les styles de paragraphe pour vérifier s'ils font
			//	référence au style de base.
			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle style in styles )
			{
				if ( !this.IsStyleAsDefaultParent(style) )
				{
					System.Collections.ArrayList parents = new System.Collections.ArrayList();
					parents.Add(this.document.TextContext.DefaultParagraphStyle);
					parents.AddRange(style.ParentStyles);

					this.document.TextContext.StyleList.RedefineTextStyle(this.document.Modifier.OpletQueue, style, style.StyleProperties, parents);
				}
			}
		}

		public System.Collections.ArrayList ArrangeParentStyles(System.Collections.ArrayList parents)
		{
			//	Réarrange la liste des parents d'un style pour utiliser le même ordre
			//	que la liste (selon StyleMap.GetRank donc).
			System.Collections.ArrayList arrangedParents = new System.Collections.ArrayList();

			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle style in styles )
			{
				if ( style.TextStyleClass == Text.TextStyleClass.Paragraph )
				{
					if ( parents.Contains(style) )
					{
						arrangedParents.Add(style);
					}
				}
			}
			foreach ( Text.TextStyle style in styles )
			{
				if ( style.TextStyleClass == Text.TextStyleClass.Text )
				{
					if ( parents.Contains(style) )
					{
						arrangedParents.Add(style);
					}
				}
			}

			return arrangedParents;
		}

		public bool IsStyleAsDefaultParent(Text.TextStyle style)
		{
			//	Vérifie si un style de paragraphe fait référence au style de base dans sa
			//	parenté directe ou indirecte. Il faut savoir qu'un style de paragraphe doit
			//	obligatoirement avoir une référence au style de base quelque part dans sa
			//	parenté complexe !
			//	Par exemple, ceci est correct:
			//		b -> a -> base
			//	Le style 'b' n'a pas le style de base comme parent direct. Il a seulement le
			//	style 'a' comme parent. Mais comme 'a' à le style de base comme parent direct,
			//	tout est juste.
			if ( style.TextStyleClass != Text.TextStyleClass.Paragraph )  return true;

			if ( this.document.TextContext.StyleList.IsDefaultParagraphTextStyle(style) )
			{
				return true;
			}

			Text.TextStyle[] parents = style.ParentStyles;
			foreach ( Text.TextStyle parent in parents )
			{
				if ( this.document.TextContext.StyleList.IsDefaultParagraphTextStyle(parent) )
				{
					return true;
				}

				if ( this.IsStyleAsDefaultParent(parent) )
				{
					return true;
				}
			}
			
			return false;
		}

		public bool IsStyleAsCircularRef(Text.TextStyle reference, Text.TextStyle style)
		{
			//	Vérifie s'il existe une référence circulaire dans un style.
			Text.TextStyle[] parents = style.ParentStyles;
			foreach ( Text.TextStyle parent in parents )
			{
				if ( parent == reference )
				{
					return true;
				}

				if ( this.IsStyleAsCircularRef(reference, parent) )
				{
					return true;
				}
			}
			
			return false;
		}
		#endregion


		#region GetStyleTextInfo
		public void GetStyleTextInfo(Text.TextStyle style, out string info, out int lines)
		{
			//	Donne un texte d'information sur un style quelconque.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("<font size=\"80%\">");
			lines = 0;

			if ( this.styleParagraphWrapper.Defined.IsJustificationModeDefined ||
				 this.styleParagraphWrapper.Defined.IsHyphenationDefined       )
			{
				builder.Append(Misc.Image("TextJustif"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsJustificationModeDefined )
				{
					Text.Wrappers.JustificationMode justif = this.styleParagraphWrapper.Defined.JustificationMode;
					switch ( justif )
					{
						case Text.Wrappers.JustificationMode.AlignLeft:         builder.Append(Res.Strings.Action.ParagraphAlignLeft);    break;
						case Text.Wrappers.JustificationMode.Center:            builder.Append(Res.Strings.Action.ParagraphAlignCenter);  break;
						case Text.Wrappers.JustificationMode.AlignRight:        builder.Append(Res.Strings.Action.ParagraphAlignRight);   break;
						case Text.Wrappers.JustificationMode.JustifyAlignLeft:  builder.Append(Res.Strings.Action.ParagraphAlignJustif);  break;
						case Text.Wrappers.JustificationMode.JustifyJustfy:     builder.Append(Res.Strings.Action.ParagraphAlignAll);     break;
					}
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsHyphenationDefined )
				{
					bool isHyphen = this.styleParagraphWrapper.Defined.IsHyphenationDefined;
					if ( isHyphen )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.Action.ParagraphHyphen);
						first = false;
					}
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleParagraphWrapper.Defined.IsLeadingDefined   ||
				 this.styleParagraphWrapper.Defined.IsAlignModeDefined )
			{
				builder.Append(Misc.Image("TextLeading"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsLeadingDefined )
				{
					double leading = this.styleParagraphWrapper.Defined.Leading;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.LeadingUnits;
					builder.Append(this.GetInfoValue(leading, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsAlignModeDefined )
				{
					bool alignFirst = (this.styleParagraphWrapper.Defined.AlignMode == Common.Text.Properties.AlignMode.First);
					bool alignAll   = (this.styleParagraphWrapper.Defined.AlignMode == Common.Text.Properties.AlignMode.All);
					if ( alignFirst )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.TextPanel.Leading.Tooltip.AlignFirst);
						first = false;
					}
					if ( alignAll )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.TextPanel.Leading.Tooltip.AlignAll);
						first = false;
					}
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleParagraphWrapper.Defined.IsLeftMarginFirstDefined ||
				 this.styleParagraphWrapper.Defined.IsLeftMarginBodyDefined  ||
				 this.styleParagraphWrapper.Defined.IsRightMarginBodyDefined )
			{
				builder.Append(Misc.Image("TextMargins"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsLeftMarginFirstDefined )
				{
					if ( !first )  builder.Append(", ");
					double margin = this.styleParagraphWrapper.Defined.LeftMarginFirst;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetInfoValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsLeftMarginBodyDefined )
				{
					if ( !first )  builder.Append(", ");
					double margin = this.styleParagraphWrapper.Defined.LeftMarginBody;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetInfoValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsRightMarginBodyDefined )
				{
					if ( !first )  builder.Append(", ");
					double margin = this.styleParagraphWrapper.Defined.RightMarginBody;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetInfoValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleParagraphWrapper.Defined.IsSpaceBeforeDefined ||
				 this.styleParagraphWrapper.Defined.IsSpaceAfterDefined  )
			{
				builder.Append(Misc.Image("TextSpaces"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsSpaceBeforeDefined )
				{
					if ( !first )  builder.Append(", ");
					double margin = this.styleParagraphWrapper.Defined.SpaceBefore;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.SpaceBeforeUnits;
					builder.Append(this.GetInfoValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsSpaceAfterDefined )
				{
					if ( !first )  builder.Append(", ");
					double margin = this.styleParagraphWrapper.Defined.SpaceAfter;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.SpaceAfterUnits;
					builder.Append(this.GetInfoValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleParagraphWrapper.Defined.IsKeepWithNextParagraphDefined     ||
				 this.styleParagraphWrapper.Defined.IsKeepWithPreviousParagraphDefined ||
				 this.styleParagraphWrapper.Defined.IsKeepStartLinesDefined            ||
				 this.styleParagraphWrapper.Defined.IsKeepEndLinesDefined              ||
				 this.styleParagraphWrapper.Defined.IsParagraphStartModeDefined        )
			{
				builder.Append(Misc.Image("TextKeep"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsKeepWithNextParagraphDefined )
				{
					bool keep = this.styleParagraphWrapper.Defined.KeepWithNextParagraph;
					if ( keep )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.TextPanel.Keep.Tooltip.KeepNext);
						first = false;
					}
				}

				if ( this.styleParagraphWrapper.Defined.IsKeepWithPreviousParagraphDefined )
				{
					bool keep = this.styleParagraphWrapper.Defined.KeepWithPreviousParagraph;
					if ( keep )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.TextPanel.Keep.Tooltip.KeepPrev);
						first = false;
					}
				}

				if ( this.styleParagraphWrapper.Defined.IsKeepStartLinesDefined )
				{
					if ( !first )  builder.Append(", ");
					int keep = this.styleParagraphWrapper.Defined.KeepStartLines;
					builder.Append(keep.ToString());
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsKeepEndLinesDefined )
				{
					if ( !first )  builder.Append(", ");
					int keep = this.styleParagraphWrapper.Defined.KeepEndLines;
					builder.Append(keep.ToString());
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsParagraphStartModeDefined )
				{
					if ( !first )  builder.Append(", ");
					Text.Properties.ParagraphStartMode mode = this.styleParagraphWrapper.Defined.ParagraphStartMode;
					builder.Append(TextPanels.Keep.ModeToString(mode));
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleTextWrapper.Defined.IsFontFaceDefined  ||
				 this.styleTextWrapper.Defined.IsFontStyleDefined ||
				 this.styleTextWrapper.Defined.IsFontSizeDefined  ||
				 this.styleTextWrapper.Defined.IsColorDefined     ||
				 this.styleTextWrapper.Defined.IsFontGlueDefined  )
			{
				builder.Append(Misc.Image("TextFont"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleTextWrapper.Defined.IsFontFaceDefined )
				{
					builder.Append(this.styleTextWrapper.Defined.FontFace);
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsFontStyleDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(this.styleTextWrapper.Defined.FontStyle);
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsFontSizeDefined )
				{
					if ( !first )  builder.Append(", ");
					double size = this.styleTextWrapper.Defined.FontSize;
					Text.Properties.SizeUnits units = this.styleTextWrapper.Defined.Units;
					builder.Append(this.GetInfoValue(size, units, Modifier.FontSizeScale));
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsColorDefined )
				{
					if ( !first )  builder.Append(", ");
					RichColor color = RichColor.Parse(this.styleTextWrapper.Defined.Color);
					builder.Append(Misc.GetColorNiceName(color));
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsFontGlueDefined )
				{
					if ( !first )  builder.Append(", ");
					double size = this.styleTextWrapper.Defined.FontGlue;
					builder.Append(this.GetInfoValue(size, Text.Properties.SizeUnits.Percent, this.document.Modifier.RealScale));
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleTextWrapper.Defined.IsUnderlineDefined ||
				 this.styleTextWrapper.Defined.IsOverlineDefined  ||
				 this.styleTextWrapper.Defined.IsStrikeoutDefined )
			{
				builder.Append(Misc.Image("TextXline"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleTextWrapper.Defined.IsUnderlineDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontUnderlined);
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsOverlineDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontOverlined);
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsStrikeoutDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontStrikeout);
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleTextWrapper.Defined.IsXscriptDefined )
			{
				builder.Append(Misc.Image("TextXscript"));
				builder.Append("  ");
				bool first = true;

				double offset = this.styleTextWrapper.Defined.Xscript.Offset;

				if ( offset > 0 )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontSuperscript);
					first = false;
				}

				if ( offset < 0 )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontSubscript);
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleTextWrapper.Defined.IsLanguageHyphenationDefined ||
				 this.styleTextWrapper.Defined.IsLanguageLocaleDefined      )
			{
				builder.Append(Misc.Image("TextLanguage"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleTextWrapper.Defined.IsLanguageLocaleDefined )
				{
					if ( !first )  builder.Append(", ");
					string language = this.styleTextWrapper.Defined.LanguageLocale;
					builder.Append(TextPanels.Language.LanguageShortToLong(language));
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsLanguageHyphenationDefined )
				{
					bool hyphen = (this.styleTextWrapper.Defined.LanguageHyphenation == 1.0);
					if ( hyphen )
					{
						if ( !first )  builder.Append(", ");
						builder.Append(Res.Strings.Action.ParagraphHyphen);
						first = false;
					}
				}

				builder.Append("<br/>");
				lines ++;
			}

			builder.Append("</font>");
			info = builder.ToString();
		}

		protected string GetInfoValue(double value, Text.Properties.SizeUnits units, double scale)
		{
			if ( units == Text.Properties.SizeUnits.Percent )
			{
				value *= 100.0;
				value = System.Math.Floor(value+0.5);  // zéro décimales
				return string.Format("{0}%", value.ToString());
			}
			else
			{
				value /= scale;
				value *= 1000.0;  // 3 décimales
				value = System.Math.Floor(value+0.5);
				value /= 1000.0;
				return value.ToString();
			}
		}
		#endregion


		public void ExecuteCommand(string name)
		{
			//	Exécute une commande.
			switch ( name )
			{
				case "FontQuick1":      this.ChangeQuick(0, name);   break;
				case "FontQuick2":      this.ChangeQuick(1, name);   break;
				case "FontQuick3":      this.ChangeQuick(2, name);   break;
				case "FontQuick4":      this.ChangeQuick(3, name);   break;
				case "FontBold":        this.ChangeBold();           break;
				case "FontItalic":      this.ChangeItalic();         break;
				case "FontUnderlined":  this.ChangeUnderlined();     break;
				case "FontOverlined":   this.ChangeOverlined();      break;
				case "FontStrikeout":   this.ChangeStrikeout();      break;
				case "FontSubscript":   this.ChangeSubscript();      break;
				case "FontSuperscript": this.ChangeSuperscript();    break;
				case "FontSizePlus":    this.IncrementFontSize(1);   break;
				case "FontSizeMinus":   this.IncrementFontSize(-1);  break;

				case "ParagraphLeading08":     this.ChangeParagraphLeading(0.8);    break;
				case "ParagraphLeading10":     this.ChangeParagraphLeading(1.0);    break;
				case "ParagraphLeading12":     this.ChangeParagraphLeading(1.2);    break;
				case "ParagraphLeading15":     this.ChangeParagraphLeading(1.5);    break;
				case "ParagraphLeading20":     this.ChangeParagraphLeading(2.0);    break;
				case "ParagraphLeading30":     this.ChangeParagraphLeading(3.0);    break;
				case "ParagraphLeadingPlus":   this.IncrementParagraphLeading(1);   break;
				case "ParagraphLeadingMinus":  this.IncrementParagraphLeading(-1);  break;
				case "ParagraphIndentPlus":    this.IncrementParagraphIndent(1);    break;
				case "ParagraphIndentMinus":   this.IncrementParagraphIndent(-1);   break;

				case "JustifHLeft":    this.Justif(Text.Wrappers.JustificationMode.AlignLeft);         break;
				case "JustifHCenter":  this.Justif(Text.Wrappers.JustificationMode.Center);            break;
				case "JustifHRight":   this.Justif(Text.Wrappers.JustificationMode.AlignRight);        break;
				case "JustifHJustif":  this.Justif(Text.Wrappers.JustificationMode.JustifyAlignLeft);  break;
				case "JustifHAll":     this.Justif(Text.Wrappers.JustificationMode.JustifyJustfy);     break;

				case "FontClear":       this.FontClear();       break;
				case "ParagraphClear":  this.ParagraphClear();  break;
			}
		}

		protected void ChangeQuick(int i, string name)
		{
			//	La commande pour une police rapide a été actionnée.
			OpenType.FontIdentity id = this.GetQuickFonts(i);
			if ( id == null )  return;
			string face = id.InvariantFaceName;

			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.FontFace  = face;
			this.textWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
			this.textWrapper.DefineOperationName(name, face);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeBold()
		{
			//	La commande 'gras' a été actionnée.
			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.InvertBold = !this.textWrapper.Defined.InvertBold;
			this.textWrapper.DefineOperationName("FontBold", Res.Strings.Action.FontBold);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeItalic()
		{
			//	La commande 'italique' a été actionnée.
			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.InvertItalic = !this.textWrapper.Defined.InvertItalic;
			this.textWrapper.DefineOperationName("FontItalic", Res.Strings.Action.FontItalic);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeUnderlined()
		{
			//	La commande 'souligné' a été actionnée.
			this.textWrapper.SuspendSynchronizations();

			if ( this.textWrapper.Active.IsUnderlineDefined )
			{
				this.textWrapper.Defined.ClearUnderline();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Underline;
				this.FillUnderlineDefinition(xline);
			}

			this.textWrapper.DefineOperationName("FontUnderlined", Res.Strings.Action.FontUnderlined);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeOverlined()
		{
			//	La commande 'surligné' a été actionnée.
			this.textWrapper.SuspendSynchronizations();

			if ( this.textWrapper.Active.IsOverlineDefined )
			{
				this.textWrapper.Defined.ClearOverline();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Overline;
				this.FillOverlineDefinition(xline);
			}

			this.textWrapper.DefineOperationName("FontOverlined", Res.Strings.Action.FontOverlined);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeStrikeout()
		{
			//	La commande 'biffé' a été actionnée.
			this.textWrapper.SuspendSynchronizations();

			if ( this.textWrapper.Active.IsStrikeoutDefined )
			{
				this.textWrapper.Defined.ClearStrikeout();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Strikeout;
				this.FillStrikeoutDefinition(xline);
			}

			this.textWrapper.DefineOperationName("FontStrikeout", Res.Strings.Action.FontStrikeout);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeSubscript()
		{
			//	La commande 'indice' a été actionnée.
			this.textWrapper.SuspendSynchronizations();

			if ( this.SubscriptActiveState )
			{
				this.textWrapper.Defined.ClearXscript();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.textWrapper.Defined.Xscript;
				this.FillSubscriptDefinition(xscript);
			}

			this.textWrapper.DefineOperationName("FontSubscript", Res.Strings.Action.FontSubscript);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeSuperscript()
		{
			//	La commande 'exposant' a été actionnée.
			this.textWrapper.SuspendSynchronizations();

			if ( this.SuperscriptActiveState )
			{
				this.textWrapper.Defined.ClearXscript();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.textWrapper.Defined.Xscript;
				this.FillSuperscriptDefinition(xscript);
			}

			this.textWrapper.DefineOperationName("FontSuperscript", Res.Strings.Action.FontSuperscript);
			this.textWrapper.ResumeSynchronizations();
		}

		public void IncrementFontSize(double delta)
		{
			//	La commande pour changer de taille a été actionnée.
			double size = this.textWrapper.Defined.FontSize;
			Text.Properties.SizeUnits units = this.textWrapper.Defined.Units;
			if ( double.IsNaN(size) )
			{
				size = this.textWrapper.Active.FontSize;
				units = this.textWrapper.Active.Units;
			}

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				double[] list = {0.25, 0.4, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0};
				size = Wrappers.SearchNextValue(size, list, delta);
			}
			else
			{
				size /= Modifier.FontSizeScale;

				if ( this.document.Type == DocumentType.Pictogram )
				{
					double[] list = {0.8,0.9,1.0,1.1,1.2,1.4,1.6,1.8,2.0,2.2,2.4,2.6,2.8,3.6,4.8,7.2,9.6,12.0,14.4,19.2,24.0};
					size = Wrappers.SearchNextValue(size, list, delta);
				}
				else
				{
					double[] list = {8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72,96,120,144,192,240};
					size = Wrappers.SearchNextValue(size, list, delta);
				}

				size *= Modifier.FontSizeScale;
			}

			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.FontSize = size;
			this.textWrapper.Defined.Units = units;
			this.textWrapper.DefineOperationName("FontFontSize", Res.Strings.Action.FontSize);
			this.textWrapper.ResumeSynchronizations();
		}


		protected void ChangeParagraphLeading(double value)
		{
			//	La commande pour changer d'interligne a été actionnée.
			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.Leading = value;
			this.paragraphWrapper.Defined.LeadingUnits = Text.Properties.SizeUnits.Percent;
			this.paragraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		public void IncrementParagraphLeading(double delta)
		{
			//	La commande modifier l'interligne a été actionnée.
			if ( !this.paragraphWrapper.IsAttached )  return;

			double leading = this.paragraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.paragraphWrapper.Active.LeadingUnits;

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				double[] list = {0.5, 0.8, 0.9, 1.0, 1.2, 1.5, 2.0, 2.5, 3.0};
				leading = Wrappers.SearchNextValue(leading, list, delta);
			}
			else
			{
				if ( this.document.Modifier.RealUnitDimension == RealUnitType.DimensionInch )
				{
					leading += delta*12.7;  // 0.05in
					leading = System.Math.Max(leading, 12.7);
				}
				else
				{
					leading += delta*10.0;  // 1mm
					leading = System.Math.Max(leading, 10.0);
				}
			}

			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.Leading = leading;
			this.paragraphWrapper.Defined.LeadingUnits = units;
			this.paragraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		protected void IncrementParagraphIndent(int delta)
		{
			//	La commande pour modifier l'indentation a été actionnée.
			if ( !this.paragraphWrapper.IsAttached )  return;

			int level = 0;
			
			if ( this.paragraphWrapper.Active.IsIndentationLevelDefined )
			{
				level = this.paragraphWrapper.Active.IndentationLevel;
			}
			
			int oldLevel = level;
			int newLevel = System.Math.Max(level+delta, 0);
			
#if false
			if ( this.paragraphWrapper.Attachment == Text.Wrappers.Attachment.Text )
			{
				Text.TextNavigator navigator = this.paragraphWrapper.AttachedTextNavigator;
				Text.TextStyle[] styles = Text.TextStyle.FilterStyles(navigator.TextStyles, Text.TextStyleClass.Paragraph);
				
				if ( styles.Length == 1 )
				{
					string oldLevelSuffix = string.Format(" {0}", oldLevel+1);
					string newLevelSuffix = string.Format(" {0}", newLevel+1);
					string styleCaption = this.document.TextContext.StyleList.StyleMap.GetCaption(styles[0]);
					
					if ( styleCaption.EndsWith(oldLevelSuffix) )
					{
						//	Le style courant se termine avec un numéro qui correspond à notre
						//	niveau d'indentation; s'il existe un style avec le nom de la nou-
						//	velle indentation, on va simplement appliquer ce style-là au texte
						//	plutôt que de changer des réglages.
						
						styleCaption = styleCaption+"¬";
						styleCaption = styleCaption.Replace(oldLevelSuffix+"¬", newLevelSuffix);
						
						Text.TextStyle newStyle = this.document.TextContext.StyleList.StyleMap.GetTextStyle(styleCaption);
						
						if ( newStyle != null && newStyle.TextStyleClass == Text.TextStyleClass.Paragraph )
						{
							navigator.SetParagraphStyles(newStyle);
							return;
						}
					}
				}
			}
#endif
			
			double marginFirst = this.paragraphWrapper.Active.LeftMarginFirst;
			double marginBody  = this.paragraphWrapper.Active.LeftMarginBody;
			double marginBase  = this.paragraphWrapper.GetUnderlyingMarginsState().LeftMarginBody;
			double marginFirstRight = this.paragraphWrapper.Active.RightMarginFirst;
			double marginBodyRight  = this.paragraphWrapper.Active.RightMarginBody;

			double distance;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				distance = 100.0;  // 10.0mm
			}
			else
			{
				distance = 127.0;  // 0.5in
			}

			double offset = newLevel*distance;
			
			if ( this.paragraphWrapper.Active.IsIndentationLevelAttributeDefined &&
				 this.paragraphWrapper.Active.IndentationLevelAttribute != null )
			{
				string attribute = this.paragraphWrapper.Active.IndentationLevelAttribute;
				double fontSize = this.textWrapper.Active.FontSize;
				fontSize /= Modifier.FontSizeScale;
				offset = 0;
				offset += Text.TabList.GetLevelOffset(fontSize, newLevel, attribute);
				offset += Text.TabList.GetRelativeOffset(fontSize, attribute);
				offset *= Modifier.FontSizeScale;
			}

			double diff = marginFirst-marginBody;
			marginBody  = marginBase+offset;
			marginFirst = marginBody+diff;
			marginBody  = System.Math.Max(marginBody,  0);
			marginFirst = System.Math.Max(marginFirst, 0);

			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.IndentationLevel = newLevel;
			this.paragraphWrapper.Defined.LeftMarginFirst = marginFirst;
			this.paragraphWrapper.Defined.RightMarginFirst = marginFirstRight;
			this.paragraphWrapper.Defined.LeftMarginBody  = marginBody;
			this.paragraphWrapper.Defined.RightMarginBody = marginBodyRight;
			this.paragraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
			this.paragraphWrapper.DefineOperationName("ParagraphIndent", Res.Strings.Action.ParagraphIndent);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		protected void Justif(Common.Text.Wrappers.JustificationMode justif)
		{
			//	La commande pour changer de mode de justification a été actionnée.
			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.JustificationMode = justif;
			this.paragraphWrapper.DefineOperationName("ParagraphJustif", Res.Strings.Action.ParagraphJustif);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		protected void FontClear()
		{
			//	La commande pour effacer les définitions de caractère a été actionnée.
			if ( !this.textWrapper.IsAttached )  return;

			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.ClearFontFace();
			this.textWrapper.Defined.ClearFontStyle();
			this.textWrapper.Defined.ClearFontSize();
			this.textWrapper.Defined.ClearFontGlue();
			this.textWrapper.Defined.ClearFontFeatures();
			this.textWrapper.Defined.ClearUnits();
			this.textWrapper.Defined.ClearInvertBold();
			this.textWrapper.Defined.ClearInvertItalic();
			this.textWrapper.Defined.ClearColor();
			this.textWrapper.Defined.ClearLanguageLocale();
			this.textWrapper.Defined.ClearLanguageHyphenation();
			this.textWrapper.Defined.ClearXscript();
			this.textWrapper.Defined.ClearUnderline();
			this.textWrapper.Defined.ClearStrikeout();
			this.textWrapper.Defined.ClearOverline();
			this.textWrapper.Defined.ClearTextBox();
			this.textWrapper.Defined.ClearTextMarker();
			this.textWrapper.Defined.ClearLink();
			this.textWrapper.Defined.ClearConditions();
			this.textWrapper.Defined.ClearUserTags();
			this.textWrapper.DefineOperationName("FontClear", Res.Strings.Action.FontClear);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ParagraphClear()
		{
			//	La commande pour effacer les définitions de paragraphe a été actionnée.
			if ( !this.paragraphWrapper.IsAttached )  return;

			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.ClearJustificationMode();
			this.paragraphWrapper.Defined.ClearHyphenation();
			this.paragraphWrapper.Defined.ClearLeftMarginFirst();
			this.paragraphWrapper.Defined.ClearLeftMarginBody();
			this.paragraphWrapper.Defined.ClearRightMarginFirst();
			this.paragraphWrapper.Defined.ClearRightMarginBody();
			this.paragraphWrapper.Defined.ClearMarginUnits();
			this.paragraphWrapper.Defined.ClearIndentationLevel();
			this.paragraphWrapper.Defined.ClearLeading();
			this.paragraphWrapper.Defined.ClearLeadingUnits();
			this.paragraphWrapper.Defined.ClearSpaceBefore();
			this.paragraphWrapper.Defined.ClearSpaceBeforeUnits();
			this.paragraphWrapper.Defined.ClearSpaceAfter();
			this.paragraphWrapper.Defined.ClearSpaceAfterUnits();
			this.paragraphWrapper.Defined.ClearAlignMode();
			this.paragraphWrapper.Defined.ClearKeepStartLines();
			this.paragraphWrapper.Defined.ClearKeepEndLines();
			this.paragraphWrapper.Defined.ClearKeepWithNextParagraph();
			this.paragraphWrapper.Defined.ClearKeepWithPreviousParagraph();
			this.paragraphWrapper.Defined.ClearParagraphStartMode();
			this.paragraphWrapper.DefineOperationName("ParagraphClear", Res.Strings.Action.ParagraphClear);
			this.paragraphWrapper.ResumeSynchronizations();
		}


		public void FillUnderlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness = 1.0;  // 0.1mm
				position  = 5.0;  // 0.5mm
			}
			else
			{
				thickness = 1.27;  // 0.005in
				position  = 5.08;  // 0.02in
			}

			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = null;
		}
        
		public void FillOverlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness = 1.0;  // 0.1mm
				position  = 2.0;  // 0.2mm
			}
			else
			{
				thickness = 1.27;  // 0.005in
				position  = 2.54;  // 0.01in
			}

			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = null;
		}
        
		public void FillStrikeoutDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness =  2.0;  // 0.2mm
				position  = 12.0;  // 1.2mm
			}
			else
			{
				thickness =  2.54;  // 0.01in
				position  = 12.70;  // 0.5in
			}

			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = null;
		}
		
		public void FillSubscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  =  0.6;
			xscript.Offset = -0.15;
		}
        
		public void FillSuperscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  = 0.6;
			xscript.Offset = 0.25;
		}


		protected bool BoldActiveState
		{
			//	Donne l'état de la commande 'gras'.
			get
			{
				string face  = this.textWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.textWrapper.Active.FontFace;
				}

				string style = this.textWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.textWrapper.Active.FontStyle;
				}

				OpenType.FontWeight weight = OpenType.FontWeight.Medium;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					weight = font.FontIdentity.FontWeight;
				}

				bool state = ((int)weight > (int)OpenType.FontWeight.Medium);
				state ^= this.textWrapper.Defined.InvertBold;

				return state;
			}
		}

		protected bool ItalicActiveState
		{
			//	Donne l'état de la commande 'italique'.
			get
			{
				string face  = this.textWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.textWrapper.Active.FontFace;
				}

				string style = this.textWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.textWrapper.Active.FontStyle;
				}

				OpenType.FontWeight weight = OpenType.FontWeight.Medium;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					weight = font.FontIdentity.FontWeight;
				}

				OpenType.FontStyle italic = OpenType.FontStyle.Normal;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					italic = font.FontIdentity.FontStyle;
				}

				bool state = italic != OpenType.FontStyle.Normal;
				state ^= this.textWrapper.Defined.InvertItalic;

				return state;
			}
		}

		protected bool UnderlinedActiveState
		{
			//	Donne l'état de la commande 'souligné'.
			get
			{
				return this.textWrapper.Active.IsUnderlineDefined;
			}
		}

		protected bool OverlinedActiveState
		{
			//	Donne l'état de la commande 'surligné'.
			get
			{
				return this.textWrapper.Active.IsOverlineDefined;
			}
		}

		protected bool StrikeoutActiveState
		{
			//	Donne l'état de la commande 'biffé'.
			get
			{
				return this.textWrapper.Active.IsStrikeoutDefined;
			}
		}

		protected bool SubscriptActiveState
		{
			//	Donne l'état de la commande 'indice'.
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset < 0.0;
			}
		}

		protected bool SuperscriptActiveState
		{
			//	Donne l'état de la commande 'exposant'.
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset > 0.0;
			}
		}


		protected void CommandActiveState(string name, bool enabled)
		{
			//	Modifie l'état d'une commande.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
		}

		protected void CommandActiveState(string name, bool enabled, bool state)
		{
			//	Modifie l'état d'une commande.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}


		protected static double SearchNextValue(double value, double[] list, double delta)
		{
			//	Cherche la valeur suivante ou précédente dans une liste.
			value += delta*0.0000001;

			if ( delta > 0 )
			{
				for ( int i=0 ; i<list.Length ; i++ )
				{
					if ( value < list[i] )
					{
						return list[i];
					}
				}
			}

			if ( delta < 0 )
			{
				for ( int i=list.Length-1 ; i>=0 ; i-- )
				{
					if ( value > list[i] )
					{
						return list[i];
					}
				}
			}

			return value;
		}

		
		protected Document								document;
		protected Text.Wrappers.TextWrapper				textWrapper;
		protected Text.Wrappers.ParagraphWrapper		paragraphWrapper;
		protected Text.Wrappers.TextWrapper				styleTextWrapper;
		protected Text.Wrappers.ParagraphWrapper		styleParagraphWrapper;
		protected TextFlow								textFlow;
		protected System.Collections.ArrayList			quickFonts;
	}
}

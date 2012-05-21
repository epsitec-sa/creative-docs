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

			this.textWrapper.Active.Changed  += this.HandleTextWrapperChanged;
			this.textWrapper.Defined.Changed += this.HandleTextWrapperChanged;

			this.paragraphWrapper.Active.Changed  += this.HandleParagraphWrapperChanged;
			this.paragraphWrapper.Defined.Changed += this.HandleParagraphWrapperChanged;

			this.document.TextContext.StyleList.StyleRedefined += this.HandleStyleWrapperChanged;

			if ( this.document.Mode == DocumentMode.Modify )
			{
				string tag;
				Drawing.DynamicImage image;

				tag = string.Concat(this.document.UniqueName, ".TextFontBrief");
				image = new Drawing.DynamicImage(new Drawing.Size(140, 16), new Drawing.DynamicImagePaintCallback(this.DrawDynamicImageFontBrief));
				image.IsCacheEnabled = true;
				Epsitec.Common.Support.ImageProvider.Default.AddDynamicImage(tag, image);

				tag = string.Concat(this.document.UniqueName, ".TextFontMenu");
				image = new Drawing.DynamicImage(new Drawing.Size(200, 30), new Drawing.DynamicImagePaintCallback(this.DrawDynamicImageFontMenu));
				image.IsCacheEnabled = true;
				Epsitec.Common.Support.ImageProvider.Default.AddDynamicImage(tag, image);

				tag = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				image = new Drawing.DynamicImage(new Drawing.Size(53, 45), new Drawing.DynamicImagePaintCallback(this.DrawDynamicImageStyleBrief));
				image.IsCacheEnabled = true;
				Epsitec.Common.Support.ImageProvider.Default.AddDynamicImage(tag, image);

				tag = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				image = new Drawing.DynamicImage(new Drawing.Size(200, 32), new Drawing.DynamicImagePaintCallback(this.DrawDynamicImageStyleMenu));
				image.IsCacheEnabled = true;
				Epsitec.Common.Support.ImageProvider.Default.AddDynamicImage(tag, image);
			}
		}

		public void Dispose()
		{
			if ( this.document.Mode == DocumentMode.Modify )
			{
				string tag;
				
				tag = string.Concat(this.document.UniqueName, ".TextFontBrief");
				Epsitec.Common.Support.ImageProvider.Default.RemoveDynamicImage(tag);
				
				tag = string.Concat(this.document.UniqueName, ".TextFontMenu");
				Epsitec.Common.Support.ImageProvider.Default.RemoveDynamicImage(tag);
				
				tag = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				Epsitec.Common.Support.ImageProvider.Default.RemoveDynamicImage(tag);
				
				tag = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				Epsitec.Common.Support.ImageProvider.Default.RemoveDynamicImage(tag);
			}

			this.textWrapper.Active.Changed  -= this.HandleTextWrapperChanged;
			this.textWrapper.Defined.Changed -= this.HandleTextWrapperChanged;

			this.paragraphWrapper.Active.Changed  -= this.HandleParagraphWrapperChanged;
			this.paragraphWrapper.Defined.Changed -= this.HandleParagraphWrapperChanged;

			this.document.TextContext.StyleList.StyleRedefined -= this.HandleStyleWrapperChanged;
		}

		public void ClearDynamicImageStyle()
		{
			//	Efface le cache des images dynamiques des styles de texte.
			string tag;

			tag = string.Concat(this.document.UniqueName, ".TextStyleBrief");
			Epsitec.Common.Support.ImageProvider.Default.ClearDynamicImageCache(tag);

			tag = string.Concat(this.document.UniqueName, ".TextStyleMenu");
			Epsitec.Common.Support.ImageProvider.Default.ClearDynamicImageCache(tag);
		}

		public void TextContextChangedDisconnect()
		{
			this.document.TextContext.StyleList.StyleRedefined -= this.HandleStyleWrapperChanged;
		}

		public void TextContextChangedConnect()
		{
			this.document.TextContext.StyleList.StyleRedefined += this.HandleStyleWrapperChanged;
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


		protected void HandleTextWrapperChanged(object sender)
		{
			//	Le wrapper du texte a changé.
			bool enabled = this.IsWrappersAttached;
			bool bold        = false;
			bool italic      = false;
			bool underline   = false;
			bool overline    = false;
			bool strikeout   = false;
			bool subscript   = false;
			bool superscript = false;

			if ( enabled )
			{
				bold        = this.BoldActiveState;
				italic      = this.ItalicActiveState;
				underline   = this.UnderlineActiveState;
				overline    = this.OverlineActiveState;
				strikeout   = this.StrikeoutActiveState;
				subscript   = this.SubscriptActiveState;
				superscript = this.SuperscriptActiveState;
			}

			this.CommandActiveState("Paste", enabled);
			this.CommandActiveState("Glyphs", enabled);
			this.CommandActiveState("GlyphsInsert", enabled);
			this.CommandActiveState("TextEditing", enabled);
			this.CommandActiveState(Commands.TextShowControlCharacters, enabled);
			this.CommandActiveState("TextInsertQuad", enabled);
			this.CommandActiveState("TextInsertNewFrame", enabled);
			this.CommandActiveState("TextInsertNewPage", enabled);

			this.CommandActiveState(Res.Commands.FontBold.CommandId,        enabled, bold       );
			this.CommandActiveState(Res.Commands.FontItalic.CommandId,      enabled, italic     );
			this.CommandActiveState(Res.Commands.FontUnderline.CommandId,   enabled, underline  );
			this.CommandActiveState(Commands.FontOverline,    enabled, overline   );
			this.CommandActiveState(Commands.FontStrikeout,   enabled, strikeout  );
			this.CommandActiveState(Commands.FontSubscript,   enabled, subscript  );
			this.CommandActiveState(Commands.FontSuperscript, enabled, superscript);

			this.CommandActiveState(Commands.FontSizePlus,  enabled);
			this.CommandActiveState(Commands.FontSizeMinus, enabled);
			this.CommandActiveState(Commands.FontClear,     enabled);
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

			string leadingState = "Leading?";
			if ( leading == 0.8 )  leadingState = "ParagraphLeading08";
			if ( leading == 1.0 )  leadingState = "ParagraphLeading10";
			if ( leading == 1.2 )  leadingState = "ParagraphLeading12";
			if ( leading == 1.5 )  leadingState = "ParagraphLeading15";
			if ( leading == 2.0 )  leadingState = "ParagraphLeading20";
			if ( leading == 3.0 )  leadingState = "ParagraphLeading30";

			string justifState = "Justif?";
			switch ( justif )
			{
				case Text.Wrappers.JustificationMode.AlignLeft:         justifState = "ParagraphJustifLeft";    break;
				case Text.Wrappers.JustificationMode.Center:            justifState = "ParagraphJustifCenter";  break;
				case Text.Wrappers.JustificationMode.AlignRight:        justifState = "ParagraphJustifRight";   break;
				case Text.Wrappers.JustificationMode.JustifyAlignLeft:  justifState = "ParagraphJustifJustif";  break;
				case Text.Wrappers.JustificationMode.JustifyJustfy:     justifState = "ParagraphJustifAll";     break;
			}

			this.CommandActiveState("ParagraphLeading", enabled, leadingState);
			this.CommandActiveState("ParagraphJustif",  enabled, justifState);

			this.CommandActiveState("ParagraphLeadingPlus",  enabled);
			this.CommandActiveState("ParagraphLeadingMinus", enabled);
			this.CommandActiveState("ParagraphIndentPlus",   enabled);
			this.CommandActiveState("ParagraphIndentMinus",  enabled);
			this.CommandActiveState("ParagraphClear",        enabled);
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


		#region GetStyleBrief
		public void GetStyleBrief(Text.TextStyle style, out string brief, out int lines)
		{
			//	Donne un texte résumé sur un style quelconque.
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
					builder.Append(Res.Strings.TextPanel.Leading.Short.Leading);
					builder.Append("=");
					double leading = this.styleParagraphWrapper.Defined.Leading;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.LeadingUnits;
					builder.Append(this.GetBriefValue(leading, units, this.document.Modifier.RealScale));
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

			if ( this.styleParagraphWrapper.Defined.IsLeftMarginFirstDefined  ||
				 this.styleParagraphWrapper.Defined.IsLeftMarginBodyDefined   ||
				 this.styleParagraphWrapper.Defined.IsRightMarginBodyDefined  ||
				 this.styleParagraphWrapper.Defined.IsIndentationLevelDefined )
			{
				builder.Append(Misc.Image("TextMargins"));
				builder.Append("  ");
				bool first = true;

				if ( this.styleParagraphWrapper.Defined.IsLeftMarginFirstDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Margins.Short.LeftFirst);
					builder.Append("=");
					double margin = this.styleParagraphWrapper.Defined.LeftMarginFirst;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetBriefValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsLeftMarginBodyDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Margins.Short.LeftBody);
					builder.Append("=");
					double margin = this.styleParagraphWrapper.Defined.LeftMarginBody;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetBriefValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsRightMarginBodyDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Margins.Short.Right);
					builder.Append("=");
					double margin = this.styleParagraphWrapper.Defined.RightMarginBody;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.MarginUnits;
					builder.Append(this.GetBriefValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsIndentationLevelDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Margins.Short.Level);
					builder.Append("=");
					int level = this.styleParagraphWrapper.Defined.IndentationLevel;
					builder.Append(level.ToString());
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
					builder.Append(Res.Strings.TextPanel.Spaces.Short.Before);
					builder.Append("=");
					double margin = this.styleParagraphWrapper.Defined.SpaceBefore;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.SpaceBeforeUnits;
					builder.Append(this.GetBriefValue(margin, units, this.document.Modifier.RealScale));
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsSpaceAfterDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Spaces.Short.After);
					builder.Append("=");
					double margin = this.styleParagraphWrapper.Defined.SpaceAfter;
					Text.Properties.SizeUnits units = this.styleParagraphWrapper.Defined.SpaceAfterUnits;
					builder.Append(this.GetBriefValue(margin, units, this.document.Modifier.RealScale));
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
					builder.Append(Res.Strings.TextPanel.Keep.Short.KeepStart);
					builder.Append("=");
					int keep = this.styleParagraphWrapper.Defined.KeepStartLines;
					builder.Append(keep.ToString());
					first = false;
				}

				if ( this.styleParagraphWrapper.Defined.IsKeepEndLinesDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.TextPanel.Keep.Short.KeepEnd);
					builder.Append("=");
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

			if ( this.styleParagraphWrapper.Defined.IsTabsDefined )
			{
				builder.Append(Misc.Image("TextTabs"));
				builder.Append("  ");
				bool first = true;

				string[] tabs = this.styleParagraphWrapper.Defined.Tabs;
				this.document.TextContext.TabList.SortTabs(tabs);
				builder.Append(string.Format("{0}x: ", tabs.Length));

				foreach ( string tab in tabs )
				{
					double pos;
					TextTabType type;
					Objects.AbstractText.GetTextTab(this.document, tab, out pos, out type);

					if ( !first )  builder.Append(", ");
					builder.Append(this.GetBriefValue(pos, Text.Properties.SizeUnits.Points, this.document.Modifier.RealScale));
					first = false;
				}

				builder.Append("<br/>");
				lines ++;
			}

			if ( this.styleParagraphWrapper.Defined.IsManagedParagraphDefined )
			{
				builder.Append(Misc.Image("TextGenerator"));
				builder.Append("  ");
				bool first = true;

				Text.ParagraphManagers.ItemListManager.Parameters p = this.styleParagraphWrapper.Defined.ItemListParameters;
				builder.Append(p.Generator.GlobalPrefix);
				for ( int i=0 ; i<p.Generator.Count ; i++ )
				{
					Text.Generator.Sequence sequence = p.Generator[i];
					if ( sequence.SuppressBefore && !first )
					{
						builder.Append(", ");
					}
					builder.Append(sequence.Prefix);
					builder.Append(TextPanels.Generator.ConvSequenceToShort(sequence));
					builder.Append(sequence.Suffix);
					first = false;
				}
				builder.Append(p.Generator.GlobalSuffix);

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
					builder.Append(Res.Strings.TextPanel.Font.Short.Size);
					builder.Append("=");
					double size = this.styleTextWrapper.Defined.FontSize;
					Text.Properties.SizeUnits units = this.styleTextWrapper.Defined.Units;
					builder.Append(this.GetBriefValue(size, units, Modifier.FontSizeScale));
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
					builder.Append(Res.Strings.TextPanel.Font.Short.Glue);
					builder.Append("=");
					double size = this.styleTextWrapper.Defined.FontGlue;
					builder.Append(this.GetBriefValue(size, Text.Properties.SizeUnits.Percent, this.document.Modifier.RealScale));
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
					builder.Append(Res.Strings.Action.FontUnderline);
					first = false;
				}

				if ( this.styleTextWrapper.Defined.IsOverlineDefined )
				{
					if ( !first )  builder.Append(", ");
					builder.Append(Res.Strings.Action.FontOverline);
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
			brief = builder.ToString();
		}

		protected string GetBriefValue(double value, Text.Properties.SizeUnits units, double scale)
		{
			//	Donne la chaîne pour représenter une valeur numérique.
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


		public void ExecuteCommand(string name, string advanceState)
		{
			//	Exécute une commande.

			if ( Druid.IsValidResourceId(name) )
			{
				//	La commande a été spécifiée au moyen d'un DRUID. On préfère travailler
				//	avec les noms des commandes ci-après, alors on remplace le DRUID de la
				//	commande par son nom :
				
				Command command = Command.Find(name);
				name = command.Name;
			}

			switch ( name )
			{
				case Commands.FontBold:        this.ChangeBold();           break;
				case Commands.FontItalic:      this.ChangeItalic();         break;
				case Commands.FontUnderline:   this.ChangeUnderline();      break;
				case Commands.FontOverline:    this.ChangeOverline();       break;
				case Commands.FontStrikeout:   this.ChangeStrikeout();      break;
				case Commands.FontSubscript:   this.ChangeSubscript();      break;
				case Commands.FontSuperscript: this.ChangeSuperscript();    break;
				case Commands.FontSizePlus:    this.IncrementFontSize(1);   break;
				case Commands.FontSizeMinus:   this.IncrementFontSize(-1);  break;

				case "ParagraphLeading":  this.ChangeParagraphLeading(advanceState);  break;
				case "ParagraphJustif":   this.ChangeParagraphJustif(advanceState);   break;

				case "ParagraphLeadingPlus":   this.IncrementParagraphLeading(1);   break;
				case "ParagraphLeadingMinus":  this.IncrementParagraphLeading(-1);  break;
				case "ParagraphIndentPlus":    this.IncrementParagraphIndent(1);    break;
				case "ParagraphIndentMinus":   this.IncrementParagraphIndent(-1);   break;

				case Commands.FontClear:       this.FontClear();       break;
				case "ParagraphClear":  this.ParagraphClear();  break;
			}
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

		protected void ChangeUnderline()
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

			this.textWrapper.DefineOperationName("FontUnderline", Res.Strings.Action.FontUnderline);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeOverline()
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

			this.textWrapper.DefineOperationName("FontOverline", Res.Strings.Action.FontOverline);
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


		protected void ChangeParagraphLeading(string advanceState)
		{
			//	La commande pour changer d'interligne a été actionnée.
			double value = 1.0;
			switch ( advanceState )
			{
				case "ParagraphLeading08":  value = 0.8;  break;
				case "ParagraphLeading10":  value = 1.0;  break;
				case "ParagraphLeading12":  value = 1.2;  break;
				case "ParagraphLeading15":  value = 1.5;  break;
				case "ParagraphLeading20":  value = 2.0;  break;
				case "ParagraphLeading30":  value = 3.0;  break;
			}

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

		protected void ChangeParagraphJustif(string advanceState)
		{
			//	La commande pour changer de mode de justification a été actionnée.
			Text.Wrappers.JustificationMode mode = Text.Wrappers.JustificationMode.AlignLeft;
			switch ( advanceState )
			{
				case "ParagraphJustifLeft":    mode = Text.Wrappers.JustificationMode.AlignLeft;         break;
				case "ParagraphJustifCenter":  mode = Text.Wrappers.JustificationMode.Center;            break;
				case "ParagraphJustifRight":   mode = Text.Wrappers.JustificationMode.AlignRight;        break;
				case "ParagraphJustifJustif":  mode = Text.Wrappers.JustificationMode.JustifyAlignLeft;  break;
				case "ParagraphJustifAll":     mode = Text.Wrappers.JustificationMode.JustifyJustfy;     break;
			}

			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.JustificationMode = mode;
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
					
					if (font != null)
					{
						weight = font.FontIdentity.FontWeight;
					}
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

					if (font != null)
					{
						weight = font.FontIdentity.FontWeight;
					}
				}

				OpenType.FontStyle italic = OpenType.FontStyle.Normal;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);

					if (font != null)
					{
						italic = font.FontIdentity.FontStyle;
					}
				}

				bool state = italic != OpenType.FontStyle.Normal;
				state ^= this.textWrapper.Defined.InvertItalic;

				return state;
			}
		}

		protected bool UnderlineActiveState
		{
			//	Donne l'état de la commande 'souligné'.
			get
			{
				return this.textWrapper.Active.IsUnderlineDefined;
			}
		}

		protected bool OverlineActiveState
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
			CommandState cs = this.document.GetCommandState (name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
		}

		protected void CommandActiveState(string name, bool enabled, bool state)
		{
			//	Modifie l'état d'une commande.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.GetCommandState (name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected void CommandActiveState(string name, bool enabled, string advanceState)
		{
			//	Modifie l'état d'une commande avancée.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.GetCommandState (name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.AdvancedState = advanceState;
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


		#region DynamicDrawing
		public void FontFaceComboUpdate(IconButtonsCombo combo)
		{
			//	Le combo pour les polices va être ouvert.
			bool quickOnly = true;
			string selectedFontFace = null;
			int quickCount;
			System.Collections.ArrayList fontList = Misc.MergeFontList(Misc.GetFontList(false), this.document.Settings.QuickFonts, quickOnly, selectedFontFace, out quickCount);

			combo.Items.Clear();
			int i = 0;
			foreach ( OpenType.FontIdentity id in fontList )
			{
				string parameter = string.Concat(id.InvariantFaceName, '\t', id.InvariantStyleName);
				string briefIcon   = Misc.IconDyn (string.Concat(this.document.UniqueName, ".TextFontBrief"), parameter);
				string regularText = Misc.ImageDyn(string.Concat(this.document.UniqueName, ".TextFontMenu"),  parameter);
				string tooltip     = string.Format(Res.Strings.Action.Apply.Tooltip.Font, TextLayout.ConvertToTaggedText(id.InvariantFaceName));

				combo.Items.Add(new IconButtonsCombo.Item(id.InvariantFaceName, briefIcon, regularText, regularText, tooltip));
				i ++;
			}
		}

		protected bool DrawDynamicImageFontBrief(Graphics graphics, Size size, string argument, GlyphPaintStyle style, Color color, object xAdorner)
		{
			//	Dessine une police pour l'icône résumée, pour une image dynamique.
			if (style != GlyphPaintStyle.Normal)
			{
				return false;
			}
			if (graphics != null)
			{
				IAdorner adorner = xAdorner as IAdorner;

				string[] arguments = argument.Split ('\t');
				System.Diagnostics.Debug.Assert (arguments.Length == 2);
				string fontFace  = arguments[0];
				string fontStyle = arguments[1];

				Rectangle rect = new Rectangle (0, 1.5, size.Width, size.Height-5);

				OpenType.Font font = TextContext.GetFont (fontFace, fontStyle);

				if (font == null)
				{
					return false;
				}


				OpenType.FontIdentity id = font.FontIdentity;
				double ox = rect.Left;
				double oy = rect.Bottom + rect.Height*0.25;

				//	Dessine l'échantillon "Abc".
				double fontSize = rect.Height*0.85;
				Color c = adorner.ColorText (WidgetPaintState.Enabled);
				Path path = Common.Widgets.Helpers.FontPreviewer.GetPathAbc (id, ox, oy, fontSize);

				if (path != null)
				{
					Rectangle bounds = path.ComputeBounds ();
					double sx = (rect.Width-bounds.Width)/2-ox;
					graphics.TranslateTransform (sx, 0);

					graphics.Color = c;
					graphics.PaintSurface (path);
					path.Dispose ();

					graphics.TranslateTransform (-sx, 0);
				}
			}
			
			return true;
		}

		protected bool DrawDynamicImageFontMenu(Graphics graphics, Size size, string argument, GlyphPaintStyle style, Color color, object xAdorner)
		{
			//	Dessine une police pour un menu, pour une image dynamique.
			if (style != GlyphPaintStyle.Normal)
			{
				return false;
			}
			
			if (graphics != null)
			{
				IAdorner adorner = xAdorner as IAdorner;

				string[] arguments = argument.Split ('\t');
				System.Diagnostics.Debug.Assert (arguments.Length == 2);
				string fontFace  = arguments[0];
				string fontStyle = arguments[1];

				OpenType.Font font = TextContext.GetFont (fontFace, fontStyle);
				
				if (font == null)
				{
					return false;
				}

				OpenType.FontIdentity id = font.FontIdentity;
				double oy = size.Height*0.25;

				//	Dessine le nom de la police.
				Rectangle r = new Rectangle (3, 0, 140-3, size.Height);
				Color c = adorner.ColorText (WidgetPaintState.Enabled);
				TextLayout layout = new TextLayout ();
				layout.Text = TextLayout.ConvertToTaggedText (fontFace);
				layout.Alignment = ContentAlignment.MiddleLeft;
				layout.LayoutSize = r.Size;
				layout.Paint (r.BottomLeft, graphics, r, c, GlyphPaintStyle.Normal);

				//	Dessine l'échantillon "Abc".
				double fontSize = size.Height*0.85;
				Path path = Common.Widgets.Helpers.FontPreviewer.GetPathAbc (id, 140+5, oy, fontSize);

				if (path != null)
				{
					graphics.Color = c;
					graphics.PaintSurface (path);
					path.Dispose ();
				}

				//	Dessine les traits verticaux de séparation.
				graphics.AddLine (140+0.5, 0, 140+0.5, size.Height);
				graphics.RenderSolid (adorner.ColorTextFieldBorder (true));
			}

			return true;
		}

		protected bool DrawDynamicImageStyleBrief(Graphics graphics, Size size, string argument, GlyphPaintStyle style, Color color, object xAdorner)
		{
			//	Dessine un style pour l'icône résumée, pour une image dynamique.
			if (style != GlyphPaintStyle.Normal)
			{
				return false;
			}
			
			if (graphics != null)
			{
				IAdorner adorner = xAdorner as IAdorner;

				string[] arguments = argument.Split ('\t');
				System.Diagnostics.Debug.Assert (arguments.Length == 2);
				string styleName = arguments[0];
				Text.TextStyleClass styleClass = Text.TextStyleClass.Paragraph;
				if (arguments[1] == "Character")
					styleClass = Text.TextStyleClass.Text;
				Text.TextStyle textStyle = this.document.TextContext.StyleList.GetTextStyle (styleName, styleClass);

				//	Plus la hauteur est petite, plus il faut de place pour le nom, pour qu'il reste lisible.
				//	Avec h=45, la hauteur pour le nom est de 14, soit environ un tier.
				//	Avec h=22, la hauteur pour le nom est de 11, soit la moitié.
				double factor = System.Math.Min (0.68 - size.Height*0.36/45, 0.5);
				double limit = System.Math.Floor (size.Height*factor);

				Rectangle rect = new Rectangle (3, limit, size.Width-6, size.Height-limit-3);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromBrightness (1));  // fond blanc
				this.DrawStyle (graphics, rect, textStyle);

				rect = new Rectangle (3, 0, size.Width-3, limit);
				string text = Misc.UserTextStyleName (this.document.TextContext.StyleList.StyleMap.GetCaption (textStyle));
				Color c = adorner.ColorText (WidgetPaintState.Enabled);
				this.DrawDynamicText (graphics, rect, text, limit*10/14, c, ContentAlignment.MiddleLeft);
			}
			
			return true;
		}

		protected bool DrawDynamicImageStyleMenu(Graphics graphics, Size size, string argument, GlyphPaintStyle style, Color color, object xAdorner)
		{
			//	Dessine un style pour un menu, pour une image dynamique.
			if (style != GlyphPaintStyle.Normal)
			{
				return false;
			}
			
			if (graphics != null)
			{
				IAdorner adorner = xAdorner as IAdorner;

				string[] arguments = argument.Split ('\t');
				System.Diagnostics.Debug.Assert (arguments.Length == 2);
				string styleName = arguments[0];
				Text.TextStyleClass styleClass = Text.TextStyleClass.Paragraph;
				if (arguments[1] == "Character")
					styleClass = Text.TextStyleClass.Text;
				Text.TextStyle textStyle = this.document.TextContext.StyleList.GetTextStyle (styleName, styleClass);

				double limit = System.Math.Floor (size.Width*0.5);

				Rectangle r = new Rectangle (3, 0, limit-3, size.Height);
				string text = Misc.UserTextStyleName (this.document.TextContext.StyleList.StyleMap.GetCaption (textStyle));
				Color c = adorner.ColorText (WidgetPaintState.Enabled);
				this.DrawDynamicText (graphics, r, text, 0, c, ContentAlignment.MiddleLeft);

				Rectangle rect = new Rectangle (limit, 0, size.Width-limit, size.Height);
				this.DrawStyle (graphics, rect, textStyle);

				graphics.AddLine (limit-0.5, 0, limit-0.5, size.Height);  // séparateur vertical
				graphics.RenderSolid (adorner.ColorBorder);
			}
			
			return true;
		}

		protected void DrawStyle(Graphics graphics, Rectangle rect, Text.TextStyle textStyle)
		{
			//	Dessine un échantillon de style dans un rectangle.
			Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(rect);

			double h = rect.Height;
			rect.Deflate(rect.Height*0.05);
			rect.Bottom -= rect.Height*10;  // hauteur presque infinie

			double scale = 1.0/7.0;
			Transform initial = graphics.Transform;
			graphics.ScaleTransform(scale, scale, 0.0, 0.0);
			rect.Scale(1.0/scale);
			h *= 1.0/scale;

			Document document = this.document.DocumentForSamples;
			document.Modifier.OpletQueueEnable = false;

			if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
			{
				Objects.TextBox2 obj = this.document.ObjectForSamplesParagraph;
				obj.RectangleToSample(rect);
				obj.SampleDefineStyle(textStyle);

				Shape[] shapes = obj.ShapesBuild(graphics, null, false);

				Drawer drawer = new Drawer(document);
				drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.OnlyText, shapes);
			}

			if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Text )
			{
				Point p1 = rect.TopLeft;
				Point p2 = rect.TopRight;
				p1.Y -= h*0.7;
				p2.Y -= h*0.7;

				double r = 12*Modifier.FontSizeScale;
				graphics.LineWidth = 1.0;
				graphics.AddLine(p1.X-10, p1.Y, p2.X+10, p2.Y);
				graphics.AddLine(p1.X-10, p1.Y+r, p2.X+10, p2.Y+r);
				graphics.RenderSolid(Color.FromRgb(1,0,0));  // rouge

				Objects.TextLine2 obj = this.document.ObjectForSamplesCharacter;
				obj.RectangleToSample(p1, p2);
				obj.SampleDefineStyle(textStyle);

				Shape[] shapes = obj.ShapesBuild(graphics, null, false);

				Drawer drawer = new Drawer(document);
				drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.All, shapes);
			}

			graphics.Transform = initial;
			graphics.RestoreClippingRectangle(iClip);
		}

		protected void DrawDynamicText(Graphics graphics, Rectangle rect, string text, double fontSize, Color color, ContentAlignment alignment)
		{
			//	Dessine un texte simple (sans tags html) inclu dans un rectangle.
			Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(rect);

			text = TextLayout.ConvertToSimpleText(text);

			if ( fontSize == 0 )
			{
				fontSize = Font.DefaultFontSize;
			}

			Font font = Misc.GetFont("Tahoma");
			graphics.AddText(rect.Left, rect.Bottom, rect.Width, rect.Height, text, font, fontSize, alignment);
			graphics.RenderSolid(color);
			
			graphics.RestoreClippingRectangle(iClip);
		}
		#endregion

		
		#region RibbonMemorize
		//	C'est ici que sont mémorisés les états des IconButtonsCombo dans les rubans.

		public int RibbonParagraphStyleFirst
		{
			get { return this.ribbonParagraphStyleFirst; }
			set { this.ribbonParagraphStyleFirst = value; }
		}

		public int RibbonCharacterStyleFirst
		{
			get { return this.ribbonCharacterStyleFirst; }
			set { this.ribbonCharacterStyleFirst = value; }
		}

		public int RibbonFontFirst
		{
			get { return this.ribbonFontFirst; }
			set { this.ribbonFontFirst = value; }
		}
		#endregion


		protected Document								document;
		protected Text.Wrappers.TextWrapper				textWrapper;
		protected Text.Wrappers.ParagraphWrapper		paragraphWrapper;
		protected Text.Wrappers.TextWrapper				styleTextWrapper;
		protected Text.Wrappers.ParagraphWrapper		styleParagraphWrapper;
		protected TextFlow								textFlow;
		protected int									ribbonParagraphStyleFirst;
		protected int									ribbonCharacterStyleFirst;
		protected int									ribbonFontFirst;
	}
}

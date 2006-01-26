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

			this.styleTextWrapper.Active.Changed      += new EventHandler(this.HandleStyleWrapperChanged);
			this.styleParagraphWrapper.Active.Changed += new EventHandler(this.HandleStyleWrapperChanged);
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
			//	Wrapper pour les styles de caract�re.
			get { return this.styleTextWrapper; }
		}

		public Text.Wrappers.ParagraphWrapper StyleParagraphWrapper
		{
			//	Wrapper pour les styles de paragraphe.
			get { return this.styleParagraphWrapper; }
		}


		public bool IsWrappersAttached
		{
			//	Indique si les wrappers sont attach�s.
			get
			{
				return this.textWrapper.IsAttached;
			}
		}

		public void WrappersAttach(TextFlow textFlow)
		{
			//	Attache tous les wrappers � un texte.
			this.textFlow = textFlow;
			this.textWrapper.Attach(textFlow.TextNavigator);
			this.paragraphWrapper.Attach(textFlow.TextNavigator);
		}

		public void WrappersDetach()
		{
			//	D�tache tous les wrappers.
			this.textFlow = null;
			this.textWrapper.Detach();
			this.paragraphWrapper.Detach();
		}

		public TextFlow TextFlow
		{
			//	Donne le TextFlow en cours d'�dition.
			get
			{
				return this.textFlow;
			}
		}


		public void UpdateCommands()
		{
			//	Met � jour toutes les commandes.
			this.HandleTextWrapperChanged(null);
			this.HandleParagraphWrapperChanged(null);
		}


		public void UpdateQuickFonts()
		{
			//	Met � jour toutes les polices rapides, lorsqu'un changement dans les r�glages a �t� fait.
			//	Appel� par DocumentEditor lorsque la notification FontsSettingsChanged est re�ue.
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
			//	Met � jour toutes les commandes pour les polices rapides.
			this.UpdateQuickButton(0);
			this.UpdateQuickButton(1);
			this.UpdateQuickButton(2);
			this.UpdateQuickButton(3);
		}

		public void UpdateQuickButton(int i)
		{
			//	Met � jour une commande pour les polices rapides.
			//	Commandes "FontQuick1" � "FontQuick4":
			if ( this.document.CommandDispatcher == null )  return;
			string cmd = string.Format("FontQuick{0}", (i+1).ToString(System.Globalization.CultureInfo.InvariantCulture));
			CommandState cs = this.document.CommandDispatcher.GetCommandState(cmd);
			if ( cs == null )  return;

			if ( this.IsWrappersAttached )
			{
				OpenType.FontIdentity id = this.GetQuickFonts(i);
				if ( id == null )
				{
					cs.Enable = false;  // pas de police rapide d�finie pour ce bouton
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
				cs.Enable = false;  // pas de texte en �dition
				cs.ActiveState = ActiveState.No;
			}
		}

		protected void HandleTextWrapperChanged(object sender)
		{
			//	Le wrapper du texte a chang�.
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
			//	Le wrapper des paragraphes a chang�.
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
			//	Le wrapper des styles de paragraphe ou de caract�re a chang�.
			System.Diagnostics.Debug.WriteLine("HandleStyleWrapperChanged "+(Wrappers.cc++).ToString());
			Text.TextStyle attachedStyle = this.styleParagraphWrapper.AttachedStyle;
			if ( attachedStyle == null )  return;

			Text.TextStyle[] styles = this.document.TextContext.StyleList.StyleMap.GetSortedStyles();
			foreach ( Text.TextStyle style in styles )
			{
				Text.TextStyle[] parents = style.ParentStyles;
				foreach ( Text.TextStyle parent in parents )
				{
					if ( parent == attachedStyle )
					{
						this.document.Notifier.NotifyTextStyleChanged(style);
					}
				}
			}

			this.document.Notifier.NotifyTextStyleChanged(attachedStyle);
		}

		protected static int cc=0;


		public bool IsStyleAsDefaultParent(Text.TextStyle style)
		{
			//	V�rifie si un style de paragraphe fait r�f�rence au style de base dans dans
			//	sa parent� directe ou indirecte. Il faut savoir qu'un style de paragraphe doit
			//	obligatoirement avoir une r�f�rence au style de base quelque part dans sa
			//	parent� complexe !
			//	Par exemple, ceci est correct:
			//		b -> a -> base
			//	Le style 'b' n'a pas le style de base comme parent direct. Il a seulement le
			//	style 'a' comme parent. Mais comme 'a' � le style de base comme parent direct,
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


		public void ExecuteCommand(string name)
		{
			//	Ex�cute une commande.
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
			//	La commande pour une police rapide a �t� actionn�e.
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
			//	La commande 'gras' a �t� actionn�e.
			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.InvertBold = !this.textWrapper.Defined.InvertBold;
			this.textWrapper.DefineOperationName("FontBold", Res.Strings.Action.FontBold);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeItalic()
		{
			//	La commande 'italique' a �t� actionn�e.
			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.InvertItalic = !this.textWrapper.Defined.InvertItalic;
			this.textWrapper.DefineOperationName("FontItalic", Res.Strings.Action.FontItalic);
			this.textWrapper.ResumeSynchronizations();
		}

		protected void ChangeUnderlined()
		{
			//	La commande 'soulign�' a �t� actionn�e.
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
			//	La commande 'surlign�' a �t� actionn�e.
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
			//	La commande 'biff�' a �t� actionn�e.
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
			//	La commande 'indice' a �t� actionn�e.
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
			//	La commande 'exposant' a �t� actionn�e.
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
			//	La commande pour changer de taille a �t� actionn�e.
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
				size /= Modifier.fontSizeScale;

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

				size *= Modifier.fontSizeScale;
			}

			this.textWrapper.SuspendSynchronizations();
			this.textWrapper.Defined.FontSize = size;
			this.textWrapper.Defined.Units = units;
			this.textWrapper.DefineOperationName("FontFontSize", Res.Strings.Action.FontSize);
			this.textWrapper.ResumeSynchronizations();
		}


		protected void ChangeParagraphLeading(double value)
		{
			//	La commande pour changer d'interligne a �t� actionn�e.
			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.Leading = value;
			this.paragraphWrapper.Defined.LeadingUnits = Text.Properties.SizeUnits.Percent;
			this.paragraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		public void IncrementParagraphLeading(double delta)
		{
			//	La commande modifier l'interligne a �t� actionn�e.
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
			//	La commande pour modifier l'indentation a �t� actionn�e.
			if ( !this.paragraphWrapper.IsAttached )  return;

			int level = 0;
			if ( this.paragraphWrapper.Active.IsIndentationLevelDefined )
			{
				level = this.paragraphWrapper.Active.IndentationLevel;
			}
			double marginFirst = this.paragraphWrapper.Active.LeftMarginFirst;
			double marginBody  = this.paragraphWrapper.Active.LeftMarginBody;
			double marginBase  = this.paragraphWrapper.GetUnderlyingMarginsState().LeftMarginBody;

			double distance;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				distance = 100.0;  // 10.0mm
			}
			else
			{
				distance = 127.0;  // 0.5in
			}

			level += delta;
			level = System.Math.Max(level, 0);

			double diff = marginFirst-marginBody;
			marginBody  = marginBase+level*distance;
			marginFirst = marginBody+diff;
			marginBody  = System.Math.Max(marginBody,  0);
			marginFirst = System.Math.Max(marginFirst, 0);

			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.IndentationLevel = level;
			this.paragraphWrapper.Defined.LeftMarginFirst = marginFirst;
			this.paragraphWrapper.Defined.LeftMarginBody  = marginBody;
			this.paragraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
			this.paragraphWrapper.DefineOperationName("ParagraphIndent", Res.Strings.Action.ParagraphIndent);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		protected void Justif(Common.Text.Wrappers.JustificationMode justif)
		{
			//	La commande pour changer de mode de justification a �t� actionn�e.
			this.paragraphWrapper.SuspendSynchronizations();
			this.paragraphWrapper.Defined.JustificationMode = justif;
			this.paragraphWrapper.DefineOperationName("ParagraphJustif", Res.Strings.Action.ParagraphJustif);
			this.paragraphWrapper.ResumeSynchronizations();
		}

		protected void FontClear()
		{
			//	La commande pour effacer les d�finitions de caract�re a �t� actionn�e.
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
			//	La commande pour effacer les d�finitions de paragraphe a �t� actionn�e.
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
			//	Donne l'�tat de la commande 'gras'.
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
			//	Donne l'�tat de la commande 'italique'.
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
			//	Donne l'�tat de la commande 'soulign�'.
			get
			{
				return this.textWrapper.Active.IsUnderlineDefined;
			}
		}

		protected bool OverlinedActiveState
		{
			//	Donne l'�tat de la commande 'surlign�'.
			get
			{
				return this.textWrapper.Active.IsOverlineDefined;
			}
		}

		protected bool StrikeoutActiveState
		{
			//	Donne l'�tat de la commande 'biff�'.
			get
			{
				return this.textWrapper.Active.IsStrikeoutDefined;
			}
		}

		protected bool SubscriptActiveState
		{
			//	Donne l'�tat de la commande 'indice'.
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset < 0.0;
			}
		}

		protected bool SuperscriptActiveState
		{
			//	Donne l'�tat de la commande 'exposant'.
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset > 0.0;
			}
		}


		protected void CommandActiveState(string name, bool enabled)
		{
			//	Modifie l'�tat d'une commande.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
		}

		protected void CommandActiveState(string name, bool enabled, bool state)
		{
			//	Modifie l'�tat d'une commande.
			if ( this.document.CommandDispatcher == null )  return;
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}


		protected static double SearchNextValue(double value, double[] list, double delta)
		{
			//	Cherche la valeur suivante ou pr�c�dente dans une liste.
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

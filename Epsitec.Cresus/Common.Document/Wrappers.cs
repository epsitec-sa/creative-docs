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

			this.textWrapper.Active.Changed  += new EventHandler(this.HandleTextWrapperChanged);
			this.textWrapper.Defined.Changed += new EventHandler(this.HandleTextWrapperChanged);

			this.paragraphWrapper.Active.Changed  += new EventHandler(this.HandleParagraphWrapperChanged);
			this.paragraphWrapper.Defined.Changed += new EventHandler(this.HandleParagraphWrapperChanged);
		}


		// Wrapper pour la fonte.
		public Text.Wrappers.TextWrapper TextWrapper
		{
			get { return this.textWrapper; }
		}

		// Wrapper pour le paragraphe.
		public Text.Wrappers.ParagraphWrapper ParagraphWrapper
		{
			get { return this.paragraphWrapper; }
		}


		// Indique si les wrappers sont attachés.
		public bool IsWrappersAttached
		{
			get
			{
				return this.textWrapper.IsAttached;
			}
		}

		// Attache tous les wrappers à un texte.
		public void WrappersAttach(TextFlow textFlow)
		{
			this.textWrapper.Attach(textFlow.TextNavigator);
			this.paragraphWrapper.Attach(textFlow.TextNavigator);
		}

		// Détache tous les wrappers.
		public void WrappersDetach()
		{
			this.textWrapper.Detach();
			this.paragraphWrapper.Detach();
		}


		// Met à jour toutes les commandes.
		public void UpdateCommands()
		{
			this.HandleTextWrapperChanged(null);
			this.HandleParagraphWrapperChanged(null);
		}

		// Met à jour toutes les polices rapides.
		public void UpdateQuickFonts()
		{
			int quickCount;
			this.quickFonts = Misc.MergeFontList(Misc.GetFontList(false), this.document.Settings.QuickFonts, true, null, out quickCount);
		}

		// Donne une police rapide.
		public OpenType.FontIdentity GetQuickFonts(int rank)
		{
			if ( this.quickFonts == null )  return null;
			if ( rank >= this.quickFonts.Count )  return null;
			return this.quickFonts[rank] as OpenType.FontIdentity;
		}


		// Met à jour toutes les commandes pour les polices rapides.
		public void UpdateQuickButtons()
		{
			this.UpdateQuickButton(0);
			this.UpdateQuickButton(1);
			this.UpdateQuickButton(2);
			this.UpdateQuickButton(3);
		}

		// Met à jour une commande pour les polices rapides.
		public void UpdateQuickButton(int i)
		{
			string cmd = string.Format("FontQuick{0}", (i+1).ToString(System.Globalization.CultureInfo.InvariantCulture));
			CommandState cs = this.document.CommandDispatcher.GetCommandState(cmd);
			if ( cs == null )  return;

			if ( this.IsWrappersAttached )
			{
				OpenType.FontIdentity id = this.GetQuickFonts(i);
				if ( id == null )
				{
					cs.Enable = false;
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
				cs.Enable = false;
			}
		}

		// Le wrapper du texte a changé.
		protected void HandleTextWrapperChanged(object sender)
		{
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

		// Le wrapper des paragraphes a changé.
		protected void HandleParagraphWrapperChanged(object sender)
		{
			bool enabled = this.IsWrappersAttached;
			double leading = 0.0;
			Common.Text.Wrappers.JustificationMode justif = Text.Wrappers.JustificationMode.Unknown;

			if ( enabled )
			{
				if ( this.paragraphWrapper.Active.LeadingUnits == Text.Properties.SizeUnits.Percent )
				{
					leading = this.paragraphWrapper.Active.Leading;
				}

				justif = this.document.ParagraphWrapper.Active.JustificationMode;
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


		// Exécute une commande.
		public void ExecuteCommand(string name)
		{
			switch ( name )
			{
				case "FontQuick1":      this.ChangeQuick(0);         break;
				case "FontQuick2":      this.ChangeQuick(1);         break;
				case "FontQuick3":      this.ChangeQuick(2);         break;
				case "FontQuick4":      this.ChangeQuick(3);         break;
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

		protected void ChangeQuick(int i)
		{
			OpenType.FontIdentity id = this.GetQuickFonts(i);
			if ( id == null )  return;
			string face = id.InvariantFaceName;

			this.textWrapper.SuspendSynchronisations();
			this.textWrapper.Defined.FontFace  = face;
			this.textWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
			this.textWrapper.ResumeSynchronisations();
		}

		protected void ChangeBold()
		{
			this.textWrapper.Defined.InvertBold = !this.textWrapper.Defined.InvertBold;
		}

		protected void ChangeItalic()
		{
			this.textWrapper.Defined.InvertItalic = !this.textWrapper.Defined.InvertItalic;
		}

		protected void ChangeUnderlined()
		{
			this.textWrapper.SuspendSynchronisations();

			if ( this.textWrapper.Active.IsUnderlineDefined )
			{
				this.textWrapper.Defined.ClearUnderline();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Underline;
				this.FillUnderlineDefinition(xline);
			}

			this.textWrapper.ResumeSynchronisations();
		}

		protected void ChangeOverlined()
		{
			this.textWrapper.SuspendSynchronisations();

			if ( this.textWrapper.Active.IsOverlineDefined )
			{
				this.textWrapper.Defined.ClearOverline();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Overline;
				this.FillOverlineDefinition(xline);
			}

			this.textWrapper.ResumeSynchronisations();
		}

		protected void ChangeStrikeout()
		{
			this.textWrapper.SuspendSynchronisations();

			if ( this.textWrapper.Active.IsStrikeoutDefined )
			{
				this.textWrapper.Defined.ClearStrikeout();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.textWrapper.Defined.Strikeout;
				this.FillStrikeoutDefinition(xline);
			}

			this.textWrapper.ResumeSynchronisations();
		}

		protected void ChangeSubscript()
		{
			this.textWrapper.SuspendSynchronisations();

			if ( this.SubscriptActiveState )
			{
				this.textWrapper.Defined.ClearXscript();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.textWrapper.Defined.Xscript;
				this.FillSubscriptDefinition(xscript);
			}

			this.textWrapper.ResumeSynchronisations();
		}

		protected void ChangeSuperscript()
		{
			this.textWrapper.SuspendSynchronisations();

			if ( this.SuperscriptActiveState )
			{
				this.textWrapper.Defined.ClearXscript();
			}
			else
			{
				Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.textWrapper.Defined.Xscript;
				this.FillSuperscriptDefinition(xscript);
			}

			this.textWrapper.ResumeSynchronisations();
		}

		public void IncrementFontSize(double delta)
		{
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

				double[] list = {8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72};
				size = Wrappers.SearchNextValue(size, list, delta);

				size *= Modifier.fontSizeScale;
			}

			this.textWrapper.SuspendSynchronisations();
			this.textWrapper.Defined.FontSize = size;
			this.textWrapper.Defined.Units = units;
			this.textWrapper.ResumeSynchronisations();
		}


		protected void ChangeParagraphLeading(double value)
		{
			this.paragraphWrapper.SuspendSynchronisations();
			this.paragraphWrapper.Defined.Leading = value;
			this.paragraphWrapper.Defined.LeadingUnits = Text.Properties.SizeUnits.Percent;
			this.paragraphWrapper.ResumeSynchronisations();
		}

		public void IncrementParagraphLeading(double delta)
		{
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

			this.paragraphWrapper.SuspendSynchronisations();
			this.paragraphWrapper.Defined.Leading = leading;
			this.paragraphWrapper.Defined.LeadingUnits = units;
			this.paragraphWrapper.ResumeSynchronisations();
		}

		protected void IncrementParagraphIndent(int delta)
		{
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

			this.paragraphWrapper.SuspendSynchronisations();
			this.paragraphWrapper.Defined.IndentationLevel = level;
			this.paragraphWrapper.Defined.LeftMarginFirst = marginFirst;
			this.paragraphWrapper.Defined.LeftMarginBody  = marginBody;
			this.paragraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
			this.paragraphWrapper.ResumeSynchronisations();
		}

		protected void Justif(Common.Text.Wrappers.JustificationMode justif)
		{
			this.document.ParagraphWrapper.Defined.JustificationMode = justif;
		}

		protected void FontClear()
		{
			if ( !this.textWrapper.IsAttached )  return;

			this.textWrapper.SuspendSynchronisations();
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
			this.textWrapper.ResumeSynchronisations();
		}

		protected void ParagraphClear()
		{
			if ( !this.paragraphWrapper.IsAttached )  return;

			this.paragraphWrapper.SuspendSynchronisations();
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
			this.paragraphWrapper.ResumeSynchronisations();
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
			get
			{
				return this.textWrapper.Active.IsUnderlineDefined;
			}
		}

		protected bool OverlinedActiveState
		{
			get
			{
				return this.textWrapper.Active.IsOverlineDefined;
			}
		}

		protected bool StrikeoutActiveState
		{
			get
			{
				return this.textWrapper.Active.IsStrikeoutDefined;
			}
		}

		protected bool SubscriptActiveState
		{
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset < 0.0;
			}
		}

		protected bool SuperscriptActiveState
		{
			get
			{
				return this.textWrapper.Defined.IsXscriptDefined && this.textWrapper.Defined.Xscript.Offset > 0.0;
			}
		}


		// Modifie l'état d'une commande.
		protected void CommandActiveState(string name, bool enabled)
		{
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
		}

		// Modifie l'état d'une commande.
		protected void CommandActiveState(string name, bool enabled, bool state)
		{
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}


		protected static double SearchNextValue(double value, double[] list, double delta)
		{
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
		protected System.Collections.ArrayList			quickFonts;
	}
}

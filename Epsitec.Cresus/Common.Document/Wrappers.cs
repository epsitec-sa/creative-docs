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

		// Le wrapper du texte a changé.
		protected void HandleTextWrapperChanged(object sender)
		{
			bool enabled = this.IsWrappersAttached;
			bool bold       = false;
			bool italic     = false;
			bool underlined = false;
			bool overlined  = false;
			bool strikeout  = false;

			if ( enabled )
			{
				bold       = this.BoldActiveState;
				italic     = this.ItalicActiveState;
				underlined = this.UnderlinedActiveState;
				overlined  = this.OverlinedActiveState;
				strikeout  = this.StrikeoutActiveState;
			}

			this.CommandActiveState("FontBold",       enabled, bold      );
			this.CommandActiveState("FontItalic",     enabled, italic    );
			this.CommandActiveState("FontUnderlined", enabled, underlined);
			this.CommandActiveState("FontOverlined",  enabled, overlined );
			this.CommandActiveState("FontStrikeout",  enabled, strikeout );
		}

		// Le wrapper des paragraphes a changé.
		protected void HandleParagraphWrapperChanged(object sender)
		{
		}


		// Exécute une commande.
		public void ExecuteCommand(string name)
		{
			switch ( name )
			{
				case "FontBold":        this.ChangeBold();        break;
				case "FontItalic":      this.ChangeItalic();      break;
				case "FontUnderlined":  this.ChangeUnderlined();  break;
				case "FontOverlined":   this.ChangeOverlined();   break;
				case "FontStrikeout":   this.ChangeStrikeout();   break;
			}
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


		// Modifie l'état d'une commande.
		protected void CommandActiveState(string name, bool enabled, bool state)
		{
			CommandState cs = this.document.CommandDispatcher.GetCommandState(name);
			System.Diagnostics.Debug.Assert(cs != null);
			cs.Enable = enabled;
			cs.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		
		protected Document								document;
		protected Text.Wrappers.TextWrapper				textWrapper;
		protected Text.Wrappers.ParagraphWrapper		paragraphWrapper;
	}
}

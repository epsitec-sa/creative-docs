using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Strike permet de choisir les soulignements.
	/// </summary>
	[SuppressBundleSupport]
	public class Strike : Abstract
	{
		public Strike(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Strike.Title;

			this.fixIcon.Text = Misc.Image("TextStrike");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Strike.Title);

			this.buttonUnderlined = this.CreateIconButton(Misc.Icon("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined, new MessageEventHandler(this.HandleButtonUnderlineClicked));
			this.buttonStrikeout  = this.CreateIconButton(Misc.Icon("FontStrikeout"),  Res.Strings.Action.Text.Font.Strikeout,  new MessageEventHandler(this.HandleButtonClicked));
			this.buttonOverlined  = this.CreateIconButton(Misc.Icon("FontOverlined"),  Res.Strings.Action.Text.Font.Overlined,  new MessageEventHandler(this.HandleButtonClicked));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.document.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Indique si ce panneau est visible pour un filtre donné.
		public override bool IsFilterShow(string filter)
		{
			return ( filter == "All" || filter == "Frequently" || filter == "Character" );
		}


		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 105;
					}
					else	// étendu/compact ?
					{
						h += 80;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			bool underlined   = this.document.TextWrapper.Active.IsUnderlineDefined;
			bool isUnderlined = this.document.TextWrapper.Defined.IsUnderlineDefined;

			bool strikeout    = this.document.TextWrapper.Active.IsStrikeoutDefined;
			bool isStrikeout  = this.document.TextWrapper.Defined.IsStrikeoutDefined;

			bool overlined    = this.document.TextWrapper.Active.IsOverlineDefined;
			bool isOverlined  = this.document.TextWrapper.Defined.IsOverlineDefined;

			this.ignoreChanged = true;
			
			this.ActiveIconButton(this.buttonUnderlined, underlined, isUnderlined);
			this.ActiveIconButton(this.buttonStrikeout,  strikeout,  isStrikeout );
			this.ActiveIconButton(this.buttonOverlined,  overlined,  isOverlined );

			this.ignoreChanged = false;
		}

		
		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonUnderlined == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderlined.Bounds = r;
					r.Offset(20, 0);
					this.buttonStrikeout.Bounds = r;
					r.Offset(20, 0);
					this.buttonOverlined.Bounds = r;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderlined.Bounds = r;
					r.Offset(20, 0);
					this.buttonStrikeout.Bounds = r;
					r.Offset(20, 0);
					this.buttonOverlined.Bounds = r;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonUnderlined.Bounds = r;
				r.Offset(20, 0);
				this.buttonStrikeout.Bounds = r;
				r.Offset(20, 0);
				this.buttonOverlined.Bounds = r;
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		private void HandleButtonUnderlineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			// Cycle entre divers états:
			//
			// (A1) Soulignement hérité du style actif
			// (A2) Pas de soulignement (forcé par un disable local)
			// (A3) Soulignement défini localement
			//
			// ou si aucun soulignement n'est défini dans le style actif:
			//
			// (B1) Pas de soulignement
			// (B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition underline = this.document.TextWrapper.Defined.Underline;
			
			if ( this.document.TextWrapper.Active.IsUnderlineDefined )
			{
				if ( this.document.TextWrapper.Active.Underline.IsDisabled &&
					 this.document.TextWrapper.Active.Underline.IsEmpty == false )
				{
					// (A2)
					this.FillUnderlineDefinition(underline);  // --> (A3)
					
					if ( underline.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Underline) )
					{
						// L'état défini par notre souligné local est identique à celui hérité
						// par le style actif; utilise celui du style dans ce cas.
						this.document.TextWrapper.Defined.ClearUnderline();  // --> (A1)
					}
				}
				else if ( this.document.TextWrapper.Defined.IsUnderlineDefined )
				{
					// (A3) ou (B2)
					this.document.TextWrapper.Defined.ClearUnderline();  // --> (A1) ou (B1)
				}
				else
				{
					// (A1)
					underline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				// (B1)
				this.FillUnderlineDefinition(underline);  // --> (B2)
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}
		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.ClearUnderline();
			this.document.TextWrapper.Defined.ClearStrikeout();
			this.document.TextWrapper.Defined.ClearOverline();
			this.document.TextWrapper.ResumeSynchronisations();
		}

		
		private void FillUnderlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition underline)
		{
			underline.IsDisabled = false;
			
			underline.Thickness      = 1.0;
			underline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			underline.Position       = -5.0;
			underline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			underline.DrawClass      = "underline";
			underline.DrawStyle      = RichColor.ToString(RichColor.FromBrightness(0));
		}
        

		protected IconButton				buttonUnderlined;
		protected IconButton				buttonStrikeout;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonClear;
	}
}

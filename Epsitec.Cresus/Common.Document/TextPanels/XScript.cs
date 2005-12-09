using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe XScript permet de définir les indices/exposants.
	/// </summary>
	[SuppressBundleSupport]
	public class XScript : Abstract
	{
		public XScript(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.XScript.Title;

			this.fixIcon.Text = Misc.Image("TextXScript");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.XScript.Title);

			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),   Res.Strings.Action.Text.Font.Subscript,   new MessageEventHandler(this.HandleButtonSubscriptClicked));
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"), Res.Strings.Action.Text.Font.Superscript, new MessageEventHandler(this.HandleButtonSuperscriptClicked));

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

			bool subscript   = false;
			bool superscript = false;
			bool isXScript   = this.document.TextWrapper.Defined.IsXscriptDefined;

			if ( isXScript )
			{
				subscript   = (this.document.TextWrapper.Defined.Xscript.Offset < 0.0);
				superscript = (this.document.TextWrapper.Defined.Xscript.Offset > 0.0);
			}

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonSubscript,   subscript,   isXScript);
			this.ActiveIconButton(this.buttonSuperscript, superscript, isXScript);
			
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

			if ( this.buttonSubscript == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonSubscript.Bounds = r;
					r.Offset(20, 0);
					this.buttonSuperscript.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonSubscript.Bounds = r;
					r.Offset(20, 0);
					this.buttonSuperscript.Bounds = r;

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
				this.buttonSubscript.Bounds = r;
				r.Offset(20, 0);
				this.buttonSuperscript.Bounds = r;
			
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		private void HandleButtonSubscriptClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.document.TextWrapper.Defined.Xscript;
			
			if ( this.document.TextWrapper.Active.IsXscriptDefined )
			{
				if ( this.document.TextWrapper.Active.Xscript.IsDisabled &&
					 this.document.TextWrapper.Active.Xscript.IsEmpty == false )
				{
					this.FillSubscriptDefinition(xscript);
					
					if ( xscript.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Xscript) )
					{
						this.document.TextWrapper.Defined.ClearXscript();
					}
				}
				else if ( this.document.TextWrapper.Defined.IsXscriptDefined )
				{
					if ( this.document.TextWrapper.Defined.Xscript.Offset < 0 )
					{
						this.document.TextWrapper.Defined.ClearXscript();
					}
					else
					{
						this.FillSubscriptDefinition(xscript);
					}
				}
				else
				{
					xscript.IsDisabled = true;
				}
			}
			else
			{
				this.FillSubscriptDefinition(xscript);
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleButtonSuperscriptClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.document.TextWrapper.Defined.Xscript;
			
			if ( this.document.TextWrapper.Active.IsXscriptDefined )
			{
				if ( this.document.TextWrapper.Active.Xscript.IsDisabled &&
					 this.document.TextWrapper.Active.Xscript.IsEmpty == false )
				{
					this.FillSuperscriptDefinition(xscript);
					
					if ( xscript.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Xscript) )
					{
						this.document.TextWrapper.Defined.ClearXscript();
					}
				}
				else if ( this.document.TextWrapper.Defined.IsXscriptDefined )
				{
					if ( this.document.TextWrapper.Defined.Xscript.Offset > 0 )
					{
						this.document.TextWrapper.Defined.ClearXscript();
					}
					else
					{
						this.FillSuperscriptDefinition(xscript);
					}
				}
				else
				{
					xscript.IsDisabled = true;
				}
			}
			else
			{
				this.FillSuperscriptDefinition(xscript);
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.ClearXscript();
			this.document.TextWrapper.ResumeSynchronisations();
		}

		
		private void FillSubscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  = 0.6;
			xscript.Offset = -0.15;
		}
        
		private void FillSuperscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  = 0.6;
			xscript.Offset = 0.25;
		}
        

		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected IconButton				buttonClear;
	}
}

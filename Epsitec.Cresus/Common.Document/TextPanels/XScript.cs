using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Xscript permet de définir les indices/exposants.
	/// </summary>
	[SuppressBundleSupport]
	public class Xscript : Abstract
	{
		public Xscript(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Xscript.Title;

			this.fixIcon.Text = Misc.Image("TextXscript");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xscript.Title);

			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),   Res.Strings.Action.Text.Font.Subscript,   new MessageEventHandler(this.HandleButtonSubscriptClicked));
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"), Res.Strings.Action.Text.Font.Superscript, new MessageEventHandler(this.HandleButtonSuperscriptClicked));

			this.fieldScale  = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Scale,  Res.Strings.TextPanel.Xscript.Short.Scale,  Res.Strings.TextPanel.Xscript.Long.Scale,  25.0, 100.0, 5.0, new EventHandler(this.HandleScaleOffsetChanged));
			this.fieldOffset = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Offset, Res.Strings.TextPanel.Xscript.Short.Offset, Res.Strings.TextPanel.Xscript.Long.Offset, 10.0, 100.0, 5.0, new EventHandler(this.HandleScaleOffsetChanged));

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
						h += 80;
					}
					else	// étendu/compact ?
					{
						h += 30;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
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

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right-25;
					this.fieldScale.Bounds = r;
					r.Offset(0, -25);
					this.fieldOffset.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonSubscript.Bounds = r;
					r.Offset(20, 0);
					this.buttonSuperscript.Bounds = r;
					r.Offset(20, 0);
					r.Width = 60;
					this.fieldScale.Bounds = r;
					r.Offset(60, 0);
					this.fieldOffset.Bounds = r;

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
				r.Offset(20, 0);
				r.Width = 60;
				this.fieldScale.Bounds = r;
				r.Offset(60, 0);
				this.fieldOffset.Bounds = r;
			
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			bool subscript   = false;
			bool superscript = false;
			bool isXscript   = this.document.TextWrapper.Defined.IsXscriptDefined;
			double scale     = 0.0;
			double offset    = 0.0;

			if ( isXscript )
			{
				subscript   = (this.document.TextWrapper.Defined.Xscript.Offset < 0.0);
				superscript = (this.document.TextWrapper.Defined.Xscript.Offset > 0.0);
				scale       =  this.document.TextWrapper.Defined.Xscript.Scale;
				offset      = System.Math.Abs(this.document.TextWrapper.Defined.Xscript.Offset);
			}

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonSubscript,   subscript,   isXscript);
			this.ActiveIconButton(this.buttonSuperscript, superscript, isXscript);

			this.SetTextFieldRealPercent(this.fieldScale.TextFieldReal,  scale,  isXscript, true);
			this.SetTextFieldRealPercent(this.fieldOffset.TextFieldReal, offset, isXscript, true);
			
			this.ignoreChanged = false;
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
					this.FillSubscriptDefinition(xscript, false);
					
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
						this.FillSubscriptDefinition(xscript, true);
					}
				}
				else
				{
					xscript.IsDisabled = true;
				}
			}
			else
			{
				this.FillSubscriptDefinition(xscript, true);
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
					this.FillSuperscriptDefinition(xscript, false);
					
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
						this.FillSuperscriptDefinition(xscript, true);
					}
				}
				else
				{
					xscript.IsDisabled = true;
				}
			}
			else
			{
				this.FillSuperscriptDefinition(xscript, true);
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleScaleOffsetChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.document.TextWrapper.Defined.Xscript;
			if ( xscript.Offset < 0 )  this.FillSubscriptDefinition(xscript, false);
			if ( xscript.Offset > 0 )  this.FillSuperscriptDefinition(xscript, false);
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

		
		private void FillSubscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript, bool def)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  = def ? 0 : (double)  this.fieldScale.TextFieldReal.InternalValue;
			xscript.Offset = def ? 0 : (double) -this.fieldOffset.TextFieldReal.InternalValue;

			if ( xscript.Scale  == 0 )  xscript.Scale  =  0.6;
			if ( xscript.Offset == 0 )  xscript.Offset = -0.15;
		}
        
		private void FillSuperscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript, bool def)
		{
			xscript.IsDisabled = false;
			
			xscript.Scale  = def ? 0 : (double) this.fieldScale.TextFieldReal.InternalValue;
			xscript.Offset = def ? 0 : (double) this.fieldOffset.TextFieldReal.InternalValue;

			if ( xscript.Scale  == 0 )  xscript.Scale  = 0.6;
			if ( xscript.Offset == 0 )  xscript.Offset = 0.25;
		}
        

		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected Widgets.TextFieldLabel	fieldScale;
		protected Widgets.TextFieldLabel	fieldOffset;
		protected IconButton				buttonClear;
	}
}

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Xscript permet de définir les indices/exposants.
	/// </summary>
	public class Xscript : Abstract
	{
		public Xscript(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Xscript.Title;

			this.fixIcon.Text = Misc.Image("TextXscript");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xscript.Title);

			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),   Res.Strings.Action.FontSubscript,   this.HandleButtonSubscriptClicked);
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"), Res.Strings.Action.FontSuperscript, this.HandleButtonSuperscriptClicked);

			this.fieldScale  = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Scale,  Res.Strings.TextPanel.Xscript.Short.Scale,  Res.Strings.TextPanel.Xscript.Long.Scale,  25.0, 100.0, 50.0, 5.0, this.HandleScaleOffsetChanged);
			this.fieldOffset = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Offset, Res.Strings.TextPanel.Xscript.Short.Offset, Res.Strings.TextPanel.Xscript.Long.Offset, 10.0, 100.0, 50.0, 5.0, this.HandleScaleOffsetChanged);

			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);

			this.TextWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.TextWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.TextWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.TextWrapper.Defined.Changed -= this.HandleWrapperChanged;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.buttonClear.Visibility = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
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

		
		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonSubscript == null )  return;

			Rectangle rect = this.UsefulZone;

			this.fieldScale.Visibility = true;
			this.fieldOffset.Visibility = true;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonSubscript.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonSuperscript.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right-25;
					this.fieldScale.SetManualBounds(r);
					r.Offset(0, -25);
					this.fieldOffset.SetManualBounds(r);
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonSubscript.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonSuperscript.SetManualBounds(r);
					r.Offset(20, 0);
					r.Width = 60;
					this.fieldScale.SetManualBounds(r);
					r.Offset(60, 0);
					this.fieldOffset.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonSubscript.SetManualBounds(r);
				r.Offset(20, 0);
				this.buttonSuperscript.SetManualBounds(r);
				r.Offset(20, 0);
				r.Width = 60;
				this.fieldScale.SetManualBounds(r);
				r.Offset(60, 0);
				this.fieldOffset.SetManualBounds(r);

				if (r.Right>rect.Right-20)
				{
					this.fieldOffset.Visibility = false;
				}
				if (r.Right-60>rect.Right-20)
				{
					this.fieldScale.Visibility = false;
				}
			
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);
			}
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.TextWrapper.IsAttached == false )  return;
			
			bool subscript   = false;
			bool superscript = false;
			bool isXscript   = this.TextWrapper.Defined.IsXscriptDefined;
			double scale     = 0.0;
			double offset    = 0.0;

			if ( isXscript )
			{
				subscript   = (this.TextWrapper.Defined.Xscript.Offset < 0.0);
				superscript = (this.TextWrapper.Defined.Xscript.Offset > 0.0);
				scale       =  this.TextWrapper.Defined.Xscript.Scale;
				offset      = System.Math.Abs(this.TextWrapper.Defined.Xscript.Offset);
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
			if ( !this.TextWrapper.IsAttached )  return;
			
			this.TextWrapper.SuspendSynchronizations();
			
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.TextWrapper.Defined.Xscript;
			
			if ( this.TextWrapper.Active.IsXscriptDefined )
			{
				if ( this.TextWrapper.Active.Xscript.IsDisabled &&
					 this.TextWrapper.Active.Xscript.IsEmpty == false )
				{
					this.FillSubscriptDefinition(xscript, false);
					
					if ( xscript.EqualsIgnoringIsDisabled(this.TextWrapper.Active.Xscript) )
					{
						this.TextWrapper.Defined.ClearXscript();
					}
				}
				else if ( this.TextWrapper.Defined.IsXscriptDefined )
				{
					if ( this.TextWrapper.Defined.Xscript.Offset < 0 )
					{
						this.TextWrapper.Defined.ClearXscript();
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
			
			this.TextWrapper.DefineOperationName("FontSubscript", Res.Strings.Action.FontSubscript);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleButtonSuperscriptClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			this.TextWrapper.SuspendSynchronizations();
			
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.TextWrapper.Defined.Xscript;
			
			if ( this.TextWrapper.Active.IsXscriptDefined )
			{
				if ( this.TextWrapper.Active.Xscript.IsDisabled &&
					 this.TextWrapper.Active.Xscript.IsEmpty == false )
				{
					this.FillSuperscriptDefinition(xscript, false);
					
					if ( xscript.EqualsIgnoringIsDisabled(this.TextWrapper.Active.Xscript) )
					{
						this.TextWrapper.Defined.ClearXscript();
					}
				}
				else if ( this.TextWrapper.Defined.IsXscriptDefined )
				{
					if ( this.TextWrapper.Defined.Xscript.Offset > 0 )
					{
						this.TextWrapper.Defined.ClearXscript();
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
			
			this.TextWrapper.DefineOperationName("FontSuperscript", Res.Strings.Action.FontSuperscript);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleScaleOffsetChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript = this.TextWrapper.Defined.Xscript;
			if ( xscript.Offset < 0 )  this.FillSubscriptDefinition(xscript, false);
			if ( xscript.Offset > 0 )  this.FillSuperscriptDefinition(xscript, false);
			this.TextWrapper.DefineOperationName("FontXscript", Res.Strings.TextPanel.Xscript.Title);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.ClearXscript();
			this.TextWrapper.DefineOperationName("FontXscriptClear", Res.Strings.TextPanel.Clear);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		private void FillSubscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript, bool def)
		{
			this.document.Wrappers.FillSubscriptDefinition(xscript);

			if ( !def )
			{
				xscript.Scale  = (double)  this.fieldScale.TextFieldReal.InternalValue;
				xscript.Offset = (double) -this.fieldOffset.TextFieldReal.InternalValue;
			}
		}
        
		private void FillSuperscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript, bool def)
		{
			this.document.Wrappers.FillSuperscriptDefinition(xscript);

			if ( !def )
			{
				xscript.Scale  = (double) this.fieldScale.TextFieldReal.InternalValue;
				xscript.Offset = (double) this.fieldOffset.TextFieldReal.InternalValue;
			}
		}
        

		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected Widgets.TextFieldLabel	fieldScale;
		protected Widgets.TextFieldLabel	fieldOffset;
		protected IconButton				buttonClear;
	}
}

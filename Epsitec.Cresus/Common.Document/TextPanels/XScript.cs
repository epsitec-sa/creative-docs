using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Xscript permet de d�finir les indices/exposants.
	/// </summary>
	[SuppressBundleSupport]
	public class Xscript : Abstract
	{
		public Xscript(Document document, bool isStyle) : base(document, isStyle)
		{
			this.label.Text = Res.Strings.TextPanel.Xscript.Title;

			this.fixIcon.Text = Misc.Image("TextXscript");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xscript.Title);

			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),   Res.Strings.Action.FontSubscript,   new MessageEventHandler(this.HandleButtonSubscriptClicked));
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"), Res.Strings.Action.FontSuperscript, new MessageEventHandler(this.HandleButtonSuperscriptClicked));

			this.fieldScale  = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Scale,  Res.Strings.TextPanel.Xscript.Short.Scale,  Res.Strings.TextPanel.Xscript.Long.Scale,  25.0, 100.0, 5.0, new EventHandler(this.HandleScaleOffsetChanged));
			this.fieldOffset = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Xscript.Tooltip.Offset, Res.Strings.TextPanel.Xscript.Short.Offset, Res.Strings.TextPanel.Xscript.Long.Offset, 10.0, 100.0, 5.0, new EventHandler(this.HandleScaleOffsetChanged));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
			this.buttonClear.Visibility = !this.TextWrapper.IsAttachedToDefaultStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau �tendu ?
				{
					if ( this.IsLabelProperties )  // �tendu/d�tails ?
					{
						h += 80;
					}
					else	// �tendu/compact ?
					{
						h += 30;
					}
				}
				else	// panneau r�duit ?
				{
					h += 30;
				}

				return h;
			}
		}

		
		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associ� a chang�.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
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


		protected override void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
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
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.ClearXscript();
			this.TextWrapper.DefineOperationName("FontXscriptClear", Res.Strings.TextPanel.Clear);
			this.TextWrapper.ResumeSynchronizations();
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

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Margins permet de choisir les marges horizontales.
	/// </summary>
	[SuppressBundleSupport]
	public class Margins : Abstract
	{
		public Margins(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Margins.Title;

			this.fixIcon.Text = Misc.Image("TextMargins");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Margins.Title);

			this.fieldLeftMarginFirst = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftFirst, Res.Strings.TextPanel.Margins.Short.LeftFirst, Res.Strings.TextPanel.Margins.Long.LeftFirst, 0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));
			this.fieldLeftMarginBody  = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftBody,  Res.Strings.TextPanel.Margins.Short.LeftBody,  Res.Strings.TextPanel.Margins.Long.LeftBody,  0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));
			this.fieldRightMargin     = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleRight,     Res.Strings.TextPanel.Margins.Short.Right,     Res.Strings.TextPanel.Margins.Long.Right,     0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.document.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
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
						h += 55;
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

			double leftFirst = this.document.ParagraphWrapper.Active.LeftMarginFirst;
			double leftBody  = this.document.ParagraphWrapper.Active.LeftMarginBody;
			double right     = this.document.ParagraphWrapper.Active.RightMarginBody;
			bool isLeftFirst = this.document.ParagraphWrapper.Defined.IsLeftMarginFirstDefined;
			bool isLeftBody  = this.document.ParagraphWrapper.Defined.IsLeftMarginBodyDefined;
			bool isRight     = this.document.ParagraphWrapper.Defined.IsRightMarginBodyDefined;

			this.ignoreChanged = true;

			this.fieldLeftMarginFirst.TextFieldReal.InternalValue = (decimal) leftFirst;
			this.fieldLeftMarginBody.TextFieldReal.InternalValue  = (decimal) leftBody;
			this.fieldRightMargin.TextFieldReal.InternalValue     = (decimal) right;
			this.ProposalTextFieldLabel(this.fieldLeftMarginFirst, !isLeftFirst);
			this.ProposalTextFieldLabel(this.fieldLeftMarginBody,  !isLeftBody );
			this.ProposalTextFieldLabel(this.fieldRightMargin,     !isRight    );
			
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

			if ( this.fieldLeftMarginFirst == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Right = rect.Right-25;
					this.fieldLeftMarginFirst.Bounds = r;
					r.Offset(0, -25);
					this.fieldLeftMarginBody.Bounds = r;
					r.Offset(0, -25);
					this.fieldRightMargin.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
					this.buttonClear.Visibility = true;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 60;
					this.fieldLeftMarginFirst.Bounds = r;
					r.Offset(60, 0);
					this.fieldLeftMarginBody.Bounds = r;
					r.Offset(60, 0);
					this.fieldRightMargin.Bounds = r;

					r.Offset(0, -25);
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
					this.buttonClear.Visibility = true;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 60;
				this.fieldLeftMarginFirst.Bounds = r;
				r.Offset(60, 0);
				this.fieldLeftMarginBody.Bounds = r;
				r.Offset(60, 0);
				this.fieldRightMargin.Bounds = r;

				this.buttonClear.Visibility = false;
			}
		}


		private void HandleMarginChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;

			if ( field == this.fieldLeftMarginFirst.TextFieldReal )
			{
				this.document.ParagraphWrapper.SuspendSynchronisations();
				this.document.ParagraphWrapper.Defined.LeftMarginFirst = value;
				this.document.ParagraphWrapper.Defined.MarginUnits     = Common.Text.Properties.SizeUnits.Points;
				this.document.ParagraphWrapper.ResumeSynchronisations();
			}

			if ( field == this.fieldLeftMarginBody.TextFieldReal )
			{
				this.document.ParagraphWrapper.SuspendSynchronisations();
				this.document.ParagraphWrapper.Defined.LeftMarginBody = value;
				this.document.ParagraphWrapper.Defined.MarginUnits    = Common.Text.Properties.SizeUnits.Points;
				this.document.ParagraphWrapper.ResumeSynchronisations();
			}

			if ( field == this.fieldRightMargin.TextFieldReal )
			{
				this.document.ParagraphWrapper.SuspendSynchronisations();
				this.document.ParagraphWrapper.Defined.RightMarginFirst = value;
				this.document.ParagraphWrapper.Defined.RightMarginBody  = value;
				this.document.ParagraphWrapper.Defined.MarginUnits      = Common.Text.Properties.SizeUnits.Points;
				this.document.ParagraphWrapper.ResumeSynchronisations();
			}
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.ClearLeftMarginFirst();
			this.document.ParagraphWrapper.Defined.ClearLeftMarginBody();
			this.document.ParagraphWrapper.Defined.ClearRightMarginFirst();
			this.document.ParagraphWrapper.Defined.ClearRightMarginBody();
			this.document.ParagraphWrapper.Defined.ClearMarginUnits();
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}


		protected Widgets.TextFieldLabel	fieldLeftMarginFirst;
		protected Widgets.TextFieldLabel	fieldLeftMarginBody;
		protected Widgets.TextFieldLabel	fieldRightMargin;
		protected IconButton				buttonClear;
	}
}

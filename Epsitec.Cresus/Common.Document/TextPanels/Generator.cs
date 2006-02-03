using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Generator permet de choisir les puces et les numérotations.
	/// </summary>
	[SuppressBundleSupport]
	public class Generator : Abstract
	{
		public Generator(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Generator.Title;

			this.fixIcon.Text = Misc.Image("TextGenerator");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Generator.Title);

			this.fieldLeftMarginFirst = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftFirst, Res.Strings.TextPanel.Margins.Short.LeftFirst, Res.Strings.TextPanel.Margins.Long.LeftFirst, 0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleMarginChanged));
			this.fieldLeftMarginBody  = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftBody,  Res.Strings.TextPanel.Margins.Short.LeftBody,  Res.Strings.TextPanel.Margins.Long.LeftBody,  0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleMarginChanged));
			this.fieldRightMargin     = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleRight,     Res.Strings.TextPanel.Margins.Short.Right,     Res.Strings.TextPanel.Margins.Long.Right,     0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleMarginChanged));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.UpdateButtonClear();
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


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
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
			}

			this.UpdateButtonClear();
		}

		protected void UpdateButtonClear()
		{
			if ( this.isExtendedSize )
			{
				this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
			}
			else
			{
				this.buttonClear.Visibility = false;
			}
		}

		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			double leftFirst = this.ParagraphWrapper.Active.LeftMarginFirst;
			double leftBody  = this.ParagraphWrapper.Active.LeftMarginBody;
			double right     = this.ParagraphWrapper.Active.RightMarginBody;
			bool isLeftFirst = this.ParagraphWrapper.Defined.IsLeftMarginFirstDefined;
			bool isLeftBody  = this.ParagraphWrapper.Defined.IsLeftMarginBodyDefined;
			bool isRight     = this.ParagraphWrapper.Defined.IsRightMarginBodyDefined;

			this.ignoreChanged = true;

			this.SetTextFieldRealValue(this.fieldLeftMarginFirst.TextFieldReal, leftFirst, Common.Text.Properties.SizeUnits.Points, isLeftFirst, false);
			this.SetTextFieldRealValue(this.fieldLeftMarginBody.TextFieldReal,  leftBody,  Common.Text.Properties.SizeUnits.Points, isLeftBody,  false);
			this.SetTextFieldRealValue(this.fieldRightMargin.TextFieldReal,     right,     Common.Text.Properties.SizeUnits.Points, isRight,     false);

			this.ignoreChanged = false;
		}


		private void HandleMarginChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value;
			Common.Text.Properties.SizeUnits units;
			bool isDefined;
			this.GetTextFieldRealValue(field, out value, out units, out isDefined);

			this.ParagraphWrapper.SuspendSynchronizations();

			if ( field == this.fieldLeftMarginFirst.TextFieldReal )
			{
				if ( isDefined )
				{
					this.ParagraphWrapper.Defined.LeftMarginFirst = value;
					this.ParagraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
				}
				else
				{
					this.ParagraphWrapper.Defined.ClearLeftMarginFirst();
				}
			}

			if ( field == this.fieldLeftMarginBody.TextFieldReal )
			{
				if ( isDefined )
				{
					this.ParagraphWrapper.Defined.LeftMarginBody = value;
					this.ParagraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
				}
				else
				{
					this.ParagraphWrapper.Defined.ClearLeftMarginBody();
				}
			}

			if ( field == this.fieldRightMargin.TextFieldReal )
			{
				if ( isDefined )
				{
					this.ParagraphWrapper.Defined.RightMarginFirst = value;
					this.ParagraphWrapper.Defined.RightMarginBody  = value;
					this.ParagraphWrapper.Defined.MarginUnits = Common.Text.Properties.SizeUnits.Points;
				}
				else
				{
					this.ParagraphWrapper.Defined.ClearRightMarginFirst();
					this.ParagraphWrapper.Defined.ClearRightMarginBody();
				}
			}

			if ( !this.ParagraphWrapper.Defined.IsLeftMarginFirstDefined  &&
				 !this.ParagraphWrapper.Defined.IsLeftMarginBodyDefined   &&
				 !this.ParagraphWrapper.Defined.IsRightMarginFirstDefined &&
				 !this.ParagraphWrapper.Defined.IsRightMarginBodyDefined  )
			{
				this.ParagraphWrapper.Defined.ClearMarginUnits();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphMargins", Res.Strings.TextPanel.Margins.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearLeftMarginFirst();
			this.ParagraphWrapper.Defined.ClearLeftMarginBody();
			this.ParagraphWrapper.Defined.ClearRightMarginFirst();
			this.ParagraphWrapper.Defined.ClearRightMarginBody();
			this.ParagraphWrapper.Defined.ClearMarginUnits();
			this.ParagraphWrapper.Defined.ClearIndentationLevel();
			this.ParagraphWrapper.Defined.ClearIndentationLevelAttribute();
			this.ParagraphWrapper.DefineOperationName("ParagraphMarginsClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}


		protected Widgets.TextFieldLabel	fieldLeftMarginFirst;
		protected Widgets.TextFieldLabel	fieldLeftMarginBody;
		protected Widgets.TextFieldLabel	fieldRightMargin;
		protected IconButton				buttonClear;
	}
}

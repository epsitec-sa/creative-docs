using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Spaces permet de choisir les marges verticales.
	/// </summary>
	public class Spaces : Abstract
	{
		public Spaces(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Spaces.Title;

			this.fixIcon.Text = Misc.Image("TextSpaces");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Spaces.Title);

			this.fieldBefore = this.CreateTextFieldLabel(Res.Strings.TextPanel.Spaces.Tooltip.Before, Res.Strings.TextPanel.Spaces.Short.Before, Res.Strings.TextPanel.Spaces.Long.Before, 0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleBeforeChanged);
			this.fieldAfter  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Spaces.Tooltip.After,  Res.Strings.TextPanel.Spaces.Short.After,  Res.Strings.TextPanel.Spaces.Long.After,  0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleAfterChanged);

			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);

			this.ParagraphWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.ParagraphWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.ParagraphWrapper.Defined.Changed -= this.HandleWrapperChanged;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
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
						h += 55;
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

			if ( this.fieldBefore == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Right = rect.Right-25;
					this.fieldBefore.SetManualBounds(r);
					r.Offset(0, -25);
					this.fieldAfter.SetManualBounds(r);
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 60;
					this.fieldBefore.SetManualBounds(r);
					r.Offset(60, 0);
					this.fieldAfter.SetManualBounds(r);
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
				r.Width = 60;
				this.fieldBefore.SetManualBounds(r);
				r.Offset(60, 0);
				this.fieldAfter.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);
			}
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();

			if ( this.ParagraphWrapper.IsAttached == false )  return;
			
			double before = this.ParagraphWrapper.Active.SpaceBefore;
			double after  = this.ParagraphWrapper.Active.SpaceAfter;
			bool isBefore = this.ParagraphWrapper.Defined.IsSpaceBeforeDefined;
			bool isAfter  = this.ParagraphWrapper.Defined.IsSpaceAfterDefined;

			this.ignoreChanged = true;

			this.SetTextFieldRealValue(this.fieldBefore.TextFieldReal, before, Common.Text.Properties.SizeUnits.Points, isBefore, false);
			this.SetTextFieldRealValue(this.fieldAfter.TextFieldReal,  after,  Common.Text.Properties.SizeUnits.Points, isAfter,  false);

			this.ignoreChanged = false;
		}

		private void HandleBeforeChanged(object sender)
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

			if ( isDefined )
			{
				this.ParagraphWrapper.Defined.SpaceBefore = value;
				this.ParagraphWrapper.Defined.SpaceBeforeUnits = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				this.ParagraphWrapper.Defined.ClearSpaceBefore();
				this.ParagraphWrapper.Defined.ClearSpaceBeforeUnits();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphSpaces", Res.Strings.TextPanel.Spaces.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		
		private void HandleAfterChanged(object sender)
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

			if ( isDefined )
			{
				this.ParagraphWrapper.Defined.SpaceAfter = value;
				this.ParagraphWrapper.Defined.SpaceAfterUnits = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				this.ParagraphWrapper.Defined.ClearSpaceAfter();
				this.ParagraphWrapper.Defined.ClearSpaceAfterUnits();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphSpaces", Res.Strings.TextPanel.Spaces.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		
		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearSpaceBefore();
			this.ParagraphWrapper.Defined.ClearSpaceBeforeUnits();
			this.ParagraphWrapper.Defined.ClearSpaceAfter();
			this.ParagraphWrapper.Defined.ClearSpaceAfterUnits();
			this.ParagraphWrapper.DefineOperationName("ParagraphSpacesClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}


		protected Widgets.TextFieldLabel	fieldBefore;
		protected Widgets.TextFieldLabel	fieldAfter;
		protected IconButton				buttonClear;
	}
}

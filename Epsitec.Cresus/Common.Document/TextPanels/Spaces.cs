using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Spaces permet de choisir les marges verticales.
	/// </summary>
	[SuppressBundleSupport]
	public class Spaces : Abstract
	{
		public Spaces(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Spaces.Title;

			this.fixIcon.Text = Misc.Image("TextSpaces");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Spaces.Title);

			this.fieldBefore = this.CreateTextFieldLabel(Res.Strings.TextPanel.Spaces.Tooltip.Before, Res.Strings.TextPanel.Spaces.Short.Before, Res.Strings.TextPanel.Spaces.Long.Before, 0.0, 0.1, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleBeforeChanged));
			this.fieldAfter  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Spaces.Tooltip.After,  Res.Strings.TextPanel.Spaces.Short.After,  Res.Strings.TextPanel.Spaces.Long.After,  0.0, 0.1, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleAfterChanged));

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
					this.fieldBefore.Bounds = r;
					r.Offset(0, -25);
					this.fieldAfter.Bounds = r;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 60;
					this.fieldBefore.Bounds = r;
					r.Offset(60, 0);
					this.fieldAfter.Bounds = r;
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
				this.fieldBefore.Bounds = r;
				r.Offset(60, 0);
				this.fieldAfter.Bounds = r;
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();

			double before = this.document.ParagraphWrapper.Active.SpaceBefore;
			double after  = this.document.ParagraphWrapper.Active.SpaceAfter;
			bool isBefore = this.document.ParagraphWrapper.Defined.IsSpaceBeforeDefined;
			bool isAfter  = this.document.ParagraphWrapper.Defined.IsSpaceAfterDefined;

			this.ignoreChanged = true;

			this.fieldBefore.TextFieldReal.InternalValue = (decimal) before;
			this.fieldAfter.TextFieldReal.InternalValue  = (decimal) after;
			this.ProposalTextFieldLabel(this.fieldBefore, !isBefore);
			this.ProposalTextFieldLabel(this.fieldAfter,  !isAfter);
			
			this.ignoreChanged = false;
		}

		private void HandleBeforeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";

			this.document.ParagraphWrapper.SuspendSynchronisations();

			if ( isDefined )
			{
				this.document.ParagraphWrapper.Defined.SpaceBefore = value;
				this.document.ParagraphWrapper.Defined.SpaceBeforeUnits = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				this.document.ParagraphWrapper.Defined.ClearSpaceBefore();
				this.document.ParagraphWrapper.Defined.ClearSpaceBeforeUnits();
			}

			this.document.ParagraphWrapper.ResumeSynchronisations();
		}
		
		private void HandleAfterChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";

			this.document.ParagraphWrapper.SuspendSynchronisations();

			if ( isDefined )
			{
				this.document.ParagraphWrapper.Defined.SpaceAfter = value;
				this.document.ParagraphWrapper.Defined.SpaceAfterUnits = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				this.document.ParagraphWrapper.Defined.ClearSpaceAfter();
				this.document.ParagraphWrapper.Defined.ClearSpaceAfterUnits();
			}

			this.document.ParagraphWrapper.ResumeSynchronisations();
		}
		
		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.ClearSpaceBefore();
			this.document.ParagraphWrapper.Defined.ClearSpaceBeforeUnits();
			this.document.ParagraphWrapper.Defined.ClearSpaceAfter();
			this.document.ParagraphWrapper.Defined.ClearSpaceAfterUnits();
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}


		protected Widgets.TextFieldLabel	fieldBefore;
		protected Widgets.TextFieldLabel	fieldAfter;
		protected IconButton				buttonClear;
	}
}

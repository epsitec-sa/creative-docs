using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Leading permet de choisir l'interligne.
	/// </summary>
	[SuppressBundleSupport]
	public class Leading : Abstract
	{
		public Leading(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Leading.Title;

			this.fixIcon.Text = Misc.Image("TextLeading");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Leading.Title);

			this.fieldLeading = this.CreateTextFieldLabel(Res.Strings.TextPanel.Leading.Tooltip.Leading, Res.Strings.TextPanel.Leading.Short.Leading, Res.Strings.TextPanel.Leading.Long.Leading, 0.0, 100.0, 1.0, false, new EventHandler(this.HandleLeadingChanged));
			this.buttonAlign = this.CreateIconButton(Misc.Icon("ParaLeadingAlign"), Res.Strings.TextPanel.Leading.Tooltip.Mode, new MessageEventHandler(this.HandleButtonAlignClicked));
			this.fieldAlign = this.CreateTextFieldLabel(Res.Strings.TextPanel.Leading.Tooltip.Align, Res.Strings.TextPanel.Leading.Short.Align, Res.Strings.TextPanel.Leading.Long.Align, 0.0, 100.0, 1.0, false, new EventHandler(this.HandleAlignChanged));

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

		
		// Indique si ce panneau est visible pour un filtre donné.
		public override bool IsFilterShow(string filter)
		{
			return ( filter == "All" || filter == "Frequently" || filter == "Paragraph" );
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


		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldLeading == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Right = rect.Right-25;
					this.fieldLeading.Bounds = r;
					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlign.Bounds = r;
					r.Left = rect.Left+20;
					r.Right = rect.Right-25;
					this.fieldAlign.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 60;
					this.fieldLeading.Bounds = r;
					r.Offset(65, 0);
					r.Width = 20;
					this.buttonAlign.Bounds = r;
					r.Offset(20, 0);
					r.Width = 60;
					this.fieldAlign.Bounds = r;

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
				this.fieldLeading.Bounds = r;
				r.Offset(65, 0);
				r.Width = 20;
				this.buttonAlign.Bounds = r;
				r.Offset(20, 0);
				r.Width = 60;
				this.fieldAlign.Bounds = r;

				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			double leading = this.document.ParagraphWrapper.Active.Leading;
			bool isLeading = this.document.ParagraphWrapper.Defined.IsLeadingDefined;

			bool align = (this.document.ParagraphWrapper.Active.AlignMode == Common.Text.Properties.AlignMode.All);
			bool isAlign = this.document.ParagraphWrapper.Defined.IsAlignModeDefined;

			this.ignoreChanged = true;

			this.fieldLeading.TextFieldReal.InternalValue = (decimal) leading;
			this.ProposalTextFieldLabel(this.fieldLeading, !isLeading);

			this.ActiveIconButton(this.buttonAlign, align, isAlign);

			this.ignoreChanged = false;
		}


		private void HandleLeadingChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";

			this.document.ParagraphWrapper.SuspendSynchronisations();

			if ( field == this.fieldLeading.TextFieldReal )
			{
				if ( isDefined )
				{
					this.document.ParagraphWrapper.Defined.Leading = value;
					this.document.ParagraphWrapper.Defined.LeadingUnits = Common.Text.Properties.SizeUnits.Points;
				}
				else
				{
					this.document.ParagraphWrapper.Defined.ClearLeading();
					this.document.ParagraphWrapper.Defined.ClearLeadingUnits();
				}
			}

			this.document.ParagraphWrapper.ResumeSynchronisations();
		}

		private void HandleButtonAlignClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool align = (this.buttonAlign.ActiveState == ActiveState.No);
			Common.Text.Properties.AlignMode mode = align ? Common.Text.Properties.AlignMode.All : Common.Text.Properties.AlignMode.None;
			this.document.ParagraphWrapper.Defined.AlignMode = mode;
		}

		private void HandleAlignChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.ClearLeading();
			this.document.ParagraphWrapper.Defined.ClearLeadingUnits();
			this.document.ParagraphWrapper.Defined.ClearAlignMode();
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}

		
		protected Widgets.TextFieldLabel	fieldLeading;
		protected IconButton				buttonAlign;
		protected Widgets.TextFieldLabel	fieldAlign;
		protected IconButton				buttonClear;
	}
}

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
			this.buttonAlignFirst = this.CreateIconButton(Misc.Icon("ParaLeadingAlignFirst"), Res.Strings.TextPanel.Leading.Tooltip.AlignFirst, new MessageEventHandler(this.HandleButtonAlignFirstClicked));
			this.buttonAlignAll = this.CreateIconButton(Misc.Icon("ParaLeadingAlignAll"), Res.Strings.TextPanel.Leading.Tooltip.AlignAll, new MessageEventHandler(this.HandleButtonAlignAllClicked));
			this.buttonSettings = this.CreateIconButton(Misc.Icon("Settings"), Res.Strings.Action.Settings, new MessageEventHandler(this.HandleButtonSettingsClicked), false);

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

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
				return this.LabelHeight+30;
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

			Rectangle r = rect;
			r.Bottom = r.Top-20;

			r.Left = rect.Left;
			r.Width = 60;
			this.fieldLeading.Bounds = r;
			r.Offset(60+12, 0);
			r.Width = 20;
			this.buttonAlignFirst.Bounds = r;
			r.Offset(20, 0);
			this.buttonAlignAll.Bounds = r;
			r.Offset(20+5, 0);
			this.buttonSettings.Bounds = r;

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.Bounds = r;
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			double leading = this.document.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.document.ParagraphWrapper.Active.LeadingUnits;
			bool isLeading = this.document.ParagraphWrapper.Defined.IsLeadingDefined;

			bool alignFirst = (this.document.ParagraphWrapper.Active.AlignMode == Common.Text.Properties.AlignMode.First);
			bool alignAll   = (this.document.ParagraphWrapper.Active.AlignMode == Common.Text.Properties.AlignMode.All);
			bool isAlign = this.document.ParagraphWrapper.Defined.IsAlignModeDefined;

			this.ignoreChanged = true;

			this.SetTextFieldRealValue(this.fieldLeading.TextFieldReal, leading, units, isLeading);
			this.ActiveIconButton(this.buttonAlignFirst, alignFirst, isAlign);
			this.ActiveIconButton(this.buttonAlignAll,   alignAll,   isAlign);

			this.ignoreChanged = false;
		}


		private void HandleLeadingChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value;
			Common.Text.Properties.SizeUnits units;
			bool isDefined;
			this.GetTextFieldRealValue(field, out value, out units, out isDefined);

			this.document.ParagraphWrapper.SuspendSynchronisations();

			if ( isDefined )
			{
				this.document.ParagraphWrapper.Defined.Leading = value;
				this.document.ParagraphWrapper.Defined.LeadingUnits = units;
			}
			else
			{
				this.document.ParagraphWrapper.Defined.ClearLeading();
				this.document.ParagraphWrapper.Defined.ClearLeadingUnits();
			}

			this.document.ParagraphWrapper.ResumeSynchronisations();
		}

		private void HandleButtonAlignFirstClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool align = (this.buttonAlignFirst.ActiveState == ActiveState.No);
			Common.Text.Properties.AlignMode mode = align ? Common.Text.Properties.AlignMode.First : Common.Text.Properties.AlignMode.None;
			this.document.ParagraphWrapper.Defined.AlignMode = mode;
		}

		private void HandleButtonAlignAllClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool align = (this.buttonAlignAll.ActiveState == ActiveState.No);
			Common.Text.Properties.AlignMode mode = align ? Common.Text.Properties.AlignMode.All : Common.Text.Properties.AlignMode.None;
			this.document.ParagraphWrapper.Defined.AlignMode = mode;
		}

		private void HandleButtonSettingsClicked(object sender, MessageEventArgs e)
		{
			this.document.Notifier.NotifySettingsShowPage("BookDocument", "Grid");
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
		protected IconButton				buttonAlignFirst;
		protected IconButton				buttonAlignAll;
		protected IconButton				buttonSettings;
		protected IconButton				buttonClear;
	}
}

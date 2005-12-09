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

			int index = this.tabIndex++;

			this.fieldLeadingAbs = this.CreateTextFieldLabel       (Res.Strings.TextPanel.Leading.Tooltip.LeadingAbs, " ", Res.Strings.TextPanel.Leading.Long.LeadingAbs,  0.0,  0.1,   1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleLeadingAbsChanged));
			this.fieldLeadingRel = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Leading.Tooltip.LeadingRel, " ", Res.Strings.TextPanel.Leading.Long.LeadingRel, 50.0, 300.0, 10.0, new EventHandler(this.HandleLeadingRelChanged));

			this.buttonUnits = new Button(this);
			this.buttonUnits.Width = 20;
			this.buttonUnits.AutoFocus = false;
			this.buttonUnits.Clicked += new MessageEventHandler(this.HandleButtonUnitsClicked);
			this.buttonUnits.TabIndex = index;
			this.buttonUnits.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUnits, Res.Strings.TextPanel.Leading.Tooltip.LeadingUnits);

			this.buttonLeadingMinus = this.CreateIconButton(Misc.Icon("ParaLeadingMinus"),      Res.Strings.TextPanel.Leading.Tooltip.LeadingMinus, new MessageEventHandler(this.HandleButtonLeadingMinusClicked), false);
			this.buttonLeadingPlus  = this.CreateIconButton(Misc.Icon("ParaLeadingPlus"),       Res.Strings.TextPanel.Leading.Tooltip.LeadingPlus,  new MessageEventHandler(this.HandleButtonLeadingPlusClicked), false);
			this.buttonAlignFirst   = this.CreateIconButton(Misc.Icon("ParaLeadingAlignFirst"), Res.Strings.TextPanel.Leading.Tooltip.AlignFirst,   new MessageEventHandler(this.HandleButtonAlignFirstClicked));
			this.buttonAlignAll     = this.CreateIconButton(Misc.Icon("ParaLeadingAlignAll"),   Res.Strings.TextPanel.Leading.Tooltip.AlignAll,     new MessageEventHandler(this.HandleButtonAlignAllClicked));
			this.buttonSettings     = this.CreateIconButton(Misc.Icon("Settings"),              Res.Strings.Action.Settings,                        new MessageEventHandler(this.HandleButtonSettingsClicked), false);

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


		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldLeadingRel == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnits.Bounds = r;
					r.Left = rect.Left+20;
					r.Right = rect.Right;
					this.fieldLeadingAbs.Bounds = r;
					this.fieldLeadingRel.Bounds = r;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonLeadingMinus.Bounds = r;
					r.Offset(20, 0);
					this.buttonLeadingPlus.Bounds = r;
					r.Offset(20+10, 0);
					this.buttonAlignFirst.Bounds = r;
					this.buttonAlignFirst.Visibility = true;
					r.Offset(20, 0);
					this.buttonAlignAll.Bounds = r;
					this.buttonAlignAll.Visibility = true;
					r.Offset(20+5, 0);
					this.buttonSettings.Bounds = r;
					this.buttonSettings.Visibility = true;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnits.Bounds = r;
					r.Offset(20-11, 0);
					r.Width = 60;
					this.fieldLeadingAbs.Bounds = r;
					this.fieldLeadingRel.Bounds = r;
					r.Offset(60, 0);
					r.Width = 20;
					this.buttonLeadingMinus.Bounds = r;
					r.Offset(20, 0);
					this.buttonLeadingPlus.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlignFirst.Bounds = r;
					this.buttonAlignFirst.Visibility = true;
					r.Offset(20, 0);
					this.buttonAlignAll.Bounds = r;
					this.buttonAlignAll.Visibility = true;
					r.Offset(20+5, 0);
					this.buttonSettings.Bounds = r;
					this.buttonSettings.Visibility = true;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonUnits.Bounds = r;
				r.Offset(20-11, 0);
				r.Width = 60;
				this.fieldLeadingAbs.Bounds = r;
				this.fieldLeadingRel.Bounds = r;
				r.Offset(60, 0);
				r.Width = 20;
				this.buttonLeadingMinus.Bounds = r;
				r.Offset(20, 0);
				this.buttonLeadingPlus.Bounds = r;

				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;

				this.buttonAlignFirst.Visibility = false;
				this.buttonAlignAll.Visibility = false;
				this.buttonSettings.Visibility = false;
			}
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

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				this.buttonUnits.Text = Res.Strings.TextPanel.Leading.Short.LeadingRel;
				this.fieldLeadingAbs.Visibility = false;
				this.fieldLeadingRel.Visibility = true;
				this.SetTextFieldRealPercent(this.fieldLeadingRel.TextFieldReal, leading, isLeading, false);
			}
			else
			{
				this.buttonUnits.Text = Res.Strings.TextPanel.Leading.Short.LeadingAbs;
				this.fieldLeadingAbs.Visibility = true;
				this.fieldLeadingRel.Visibility = false;
				this.SetTextFieldRealValue(this.fieldLeadingAbs.TextFieldReal, leading, units, isLeading);
			}

			this.ActiveIconButton(this.buttonAlignFirst, alignFirst, isAlign);
			this.ActiveIconButton(this.buttonAlignAll,   alignAll,   isAlign);

			this.ignoreChanged = false;
		}


		private void HandleButtonUnitsClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			double value;
			Common.Text.Properties.SizeUnits units = this.document.ParagraphWrapper.Active.LeadingUnits;

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				if ( this.document.Modifier.RealUnitDimension == RealUnitType.DimensionInch )
				{
					value = 127.0;  // 0.5in
				}
				else
				{
					value = 100.0;  // 10mm
				}
				units = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				value = 1.2;
				units = Common.Text.Properties.SizeUnits.Percent;
			}

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.Leading = value;
			this.document.ParagraphWrapper.Defined.LeadingUnits = units;
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}

		private void HandleLeadingAbsChanged(object sender)
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

		private void HandleLeadingRelChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value;
			Common.Text.Properties.SizeUnits units;
			bool isDefined;
			this.GetTextFieldRealPercent(field, out value, out isDefined);
			units = Common.Text.Properties.SizeUnits.Percent;

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

		private void HandleButtonLeadingMinusClicked(object sender, MessageEventArgs e)
		{
			this.LeadingIncrement(-1);
		}

		private void HandleButtonLeadingPlusClicked(object sender, MessageEventArgs e)
		{
			this.LeadingIncrement(1);
		}

		protected void LeadingIncrement(double delta)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			double leading = this.document.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.document.ParagraphWrapper.Active.LeadingUnits;

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				if ( delta > 0 )
				{
					leading *= 1.5;
				}
				else
				{
					leading /= 1.5;
				}
				leading = System.Math.Max(leading, 0.5);
			}
			else
			{
				if ( this.document.Modifier.RealUnitDimension == RealUnitType.DimensionInch )
				{
					leading += delta*50.8;  // 0.2in
					leading = System.Math.Max(leading, 12.7);
				}
				else
				{
					leading += delta*50.0;  // 5mm
					leading = System.Math.Max(leading, 10.0);
				}
			}

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.Leading = leading;
			this.document.ParagraphWrapper.Defined.LeadingUnits = units;
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


		protected Button					buttonUnits;
		protected Widgets.TextFieldLabel	fieldLeadingAbs;
		protected Widgets.TextFieldLabel	fieldLeadingRel;
		protected IconButton				buttonLeadingMinus;
		protected IconButton				buttonLeadingPlus;
		protected IconButton				buttonAlignFirst;
		protected IconButton				buttonAlignAll;
		protected IconButton				buttonSettings;
		protected IconButton				buttonClear;
	}
}

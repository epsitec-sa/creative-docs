using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Leading permet de choisir l'interligne.
	/// </summary>
	public class Leading : Abstract
	{
		public Leading(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Leading.Title;

			this.fixIcon.Text = Misc.Image("TextLeading");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Leading.Title);

			this.fieldLeading = this.CreateTextFieldLabel(Res.Strings.TextPanel.Leading.Tooltip.Leading, Res.Strings.TextPanel.Leading.Short.Leading, Res.Strings.TextPanel.Leading.Long.Leading, 0,0,0,0, Widgets.TextFieldLabel.Type.TextFieldUnit, this.HandleLeadingChanged);
			this.fieldLeading.SetRangeDimension(this.document, 0.0, 0.1, 0.0, 1.0);
			this.fieldLeading.SetRangePercents(this.document, 50.0, 300.0, 100.0, 10.0);
			this.fieldLeading.IsUnitPercent = true;
			this.fieldLeading.ButtonUnit.Clicked += this.HandleButtonUnitClicked;

			this.buttonLeadingMenu = this.CreateComboButton(null, Res.Strings.TextPanel.Leading.Tooltip.Leading, this.HandleButtonLeadingMenuClicked);

			this.buttonAlignFirst = this.CreateIconButton(Misc.Icon("ParagraphLeadingAlignFirst"), Res.Strings.TextPanel.Leading.Tooltip.AlignFirst, this.HandleButtonAlignFirstClicked);
			this.buttonAlignAll   = this.CreateIconButton(Misc.Icon("ParagraphLeadingAlignAll"),   Res.Strings.TextPanel.Leading.Tooltip.AlignAll,   this.HandleButtonAlignAllClicked);

			if (Command.IsDefined ("Settings"))
			{
				this.buttonSettings = this.CreateIconButton (Misc.Icon ("Settings"), Res.Strings.Action.Settings, this.HandleButtonSettingsClicked, false);
			}

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
				this.fieldLeading.ButtonUnit.Clicked -= this.HandleButtonUnitClicked;
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

			if ( this.fieldLeading == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 69;
					this.fieldLeading.SetManualBounds(r);
					r.Offset(69, 0);
					r.Width = 20;
					this.buttonLeadingMenu.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlignFirst.SetManualBounds(r);
					this.buttonAlignFirst.Visibility = true;
					r.Offset(20, 0);
					this.buttonAlignAll.SetManualBounds(r);
					this.buttonAlignAll.Visibility = true;
					r.Offset(20+5, 0);
					if (this.buttonSettings != null)
					{
						this.buttonSettings.SetManualBounds (r);
						this.buttonSettings.Visibility = true;
					}
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 69;
					this.fieldLeading.SetManualBounds(r);
					r.Offset(69, 0);
					r.Width = 20;
					this.buttonLeadingMenu.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlignFirst.SetManualBounds(r);
					this.buttonAlignFirst.Visibility = true;
					r.Offset(20, 0);
					this.buttonAlignAll.SetManualBounds(r);
					this.buttonAlignAll.Visibility = true;
					r.Offset(20+5, 0);
					if (this.buttonSettings != null)
					{
						this.buttonSettings.SetManualBounds (r);
						this.buttonSettings.Visibility = true;
					}
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 69;
				this.fieldLeading.SetManualBounds(r);
				r.Offset(69, 0);
				r.Width = 20;
				this.buttonLeadingMenu.SetManualBounds(r);

				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

				this.buttonAlignFirst.Visibility = false;
				this.buttonAlignAll.Visibility = false;
				if (this.buttonSettings != null)
				{
					this.buttonSettings.Visibility = false;
				}
			}
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();

			if ( this.ParagraphWrapper.IsAttached == false )  return;
			
			double leading = this.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.ParagraphWrapper.Active.LeadingUnits;
			bool isLeading = this.ParagraphWrapper.Defined.IsLeadingDefined;

			bool alignFirst = (this.ParagraphWrapper.Active.AlignMode == Common.Text.Properties.AlignMode.First);
			bool alignAll   = (this.ParagraphWrapper.Active.AlignMode == Common.Text.Properties.AlignMode.All);
			bool isAlign = this.ParagraphWrapper.Defined.IsAlignModeDefined;

			this.ignoreChanged = true;

			this.fieldLeading.IsUnitPercent = (units == Common.Text.Properties.SizeUnits.Percent);
			this.SetTextFieldRealValue(this.fieldLeading.TextFieldReal, leading, units, isLeading, false);

			this.ActiveIconButton(this.buttonAlignFirst, alignFirst, isAlign);
			this.ActiveIconButton(this.buttonAlignAll,   alignAll,   isAlign);

			this.ignoreChanged = false;
		}


		private void HandleButtonUnitClicked(object sender, MessageEventArgs e)
		{
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.fieldLeading.IsUnitPercent = !this.fieldLeading.IsUnitPercent;

			double value;
			Common.Text.Properties.SizeUnits units;

			if ( this.fieldLeading.IsUnitPercent )
			{
				value = 1.2;
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
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

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.Leading = value;
			this.ParagraphWrapper.Defined.LeadingUnits = units;
			this.ParagraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleLeadingChanged(object sender)
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
				this.ParagraphWrapper.Defined.Leading = value;
				this.ParagraphWrapper.Defined.LeadingUnits = units;
			}
			else
			{
				this.ParagraphWrapper.Defined.ClearLeading();
				this.ParagraphWrapper.Defined.ClearLeadingUnits();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleButtonLeadingMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			VMenu menu = this.CreateMenu();
			menu.Host = this;
			menu.MinWidth = this.fieldLeading.ActualWidth+button.ActualWidth;
			TextFieldCombo.AdjustComboSize(this.fieldLeading, menu, false);
			menu.ShowAsComboList(this.fieldLeading, Point.Zero, button);
		}

		private void HandleButtonAlignFirstClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			bool align = (this.buttonAlignFirst.ActiveState == ActiveState.No);
			Common.Text.Properties.AlignMode mode = align ? Common.Text.Properties.AlignMode.First : Common.Text.Properties.AlignMode.None;
			this.ParagraphWrapper.Defined.AlignMode = mode;
		}

		private void HandleButtonAlignAllClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			bool align = (this.buttonAlignAll.ActiveState == ActiveState.No);
			Common.Text.Properties.AlignMode mode = align ? Common.Text.Properties.AlignMode.All : Common.Text.Properties.AlignMode.None;
			this.ParagraphWrapper.Defined.AlignMode = mode;
		}

		private void HandleButtonSettingsClicked(object sender, MessageEventArgs e)
		{
			this.document.Notifier.NotifySettingsShowPage("BookDocument", "Grid");
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearLeading();
			this.ParagraphWrapper.Defined.ClearLeadingUnits();
			this.ParagraphWrapper.Defined.ClearAlignMode();
			this.ParagraphWrapper.DefineOperationName("ParagraphLeadingClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}


		#region Menu
		protected VMenu CreateMenu()
		{
			//	Construit le menu pour choisir l'interligne.
			VMenu menu = new VMenu();

			double leading = this.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.ParagraphWrapper.Active.LeadingUnits;
			bool isPercent = (units == Common.Text.Properties.SizeUnits.Percent);
			bool isDefault = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;

			return Menus.LeadingMenu.CreateLeadingMenu(this.document, leading, isPercent?"%":"", isDefault, this.HandleMenuPressed);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string text = item.Name;

			double leading;
			Common.Text.Properties.SizeUnits units;

			if ( text == "" )
			{
				leading = double.NaN;
				units = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
				if ( text.EndsWith("%") )
				{
					text = text.Substring(0, text.Length-1);
					leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
					units = Common.Text.Properties.SizeUnits.Percent;
				}
				else
				{
					leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
					units = Common.Text.Properties.SizeUnits.Points;
				}
			}

			this.ParagraphWrapper.SuspendSynchronizations();

			if ( double.IsNaN(leading) )
			{
				this.ParagraphWrapper.Defined.ClearLeading();
				this.ParagraphWrapper.Defined.ClearLeadingUnits();
			}
			else
			{
				this.ParagraphWrapper.Defined.Leading = leading;
				this.ParagraphWrapper.Defined.LeadingUnits = units;
				this.ParagraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			}

			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		#endregion

		
		protected Button					buttonUnits;
		protected Widgets.TextFieldLabel	fieldLeading;
		protected GlyphButton				buttonLeadingMenu;
		protected IconButton				buttonAlignFirst;
		protected IconButton				buttonAlignAll;
		protected IconButton				buttonSettings;
		protected IconButton				buttonClear;
	}
}

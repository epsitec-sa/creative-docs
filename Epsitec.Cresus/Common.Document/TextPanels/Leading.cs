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

			this.fieldLeading = this.CreateTextFieldLabel(Res.Strings.TextPanel.Leading.Tooltip.Leading, Res.Strings.TextPanel.Leading.Short.Leading, Res.Strings.TextPanel.Leading.Long.Leading, 0,0,0, Widgets.TextFieldLabel.Type.TextFieldUnit, new EventHandler(this.HandleLeadingChanged));
			this.fieldLeading.SetRangeDimension(this.document, 0.0, 0.1, 1.0);
			this.fieldLeading.SetRangePercents(this.document, 50.0, 300.0, 10.0);
			this.fieldLeading.IsUnitPercent = true;
			this.fieldLeading.ButtonUnit.Clicked += new MessageEventHandler(this.HandleButtonUnitClicked);

			this.buttonLeadingMenu = this.CreateComboButton(null, Res.Strings.TextPanel.Leading.Tooltip.Leading, new MessageEventHandler(this.HandleButtonLeadingMenuClicked));

			this.buttonLeadingMinus = this.CreateIconButton(Misc.Icon("ParagraphLeadingMinus"),      Res.Strings.Action.ParagraphLeadingMinus,         new MessageEventHandler(this.HandleButtonLeadingMinusClicked), false);
			this.buttonLeadingPlus  = this.CreateIconButton(Misc.Icon("ParagraphLeadingPlus"),       Res.Strings.Action.ParagraphLeadingPlus,          new MessageEventHandler(this.HandleButtonLeadingPlusClicked), false);
			this.buttonAlignFirst   = this.CreateIconButton(Misc.Icon("ParagraphLeadingAlignFirst"), Res.Strings.TextPanel.Leading.Tooltip.AlignFirst, new MessageEventHandler(this.HandleButtonAlignFirstClicked));
			this.buttonAlignAll     = this.CreateIconButton(Misc.Icon("ParagraphLeadingAlignAll"),   Res.Strings.TextPanel.Leading.Tooltip.AlignAll,   new MessageEventHandler(this.HandleButtonAlignAllClicked));
			this.buttonSettings     = this.CreateIconButton(Misc.Icon("Settings"),                   Res.Strings.Action.Settings,                      new MessageEventHandler(this.HandleButtonSettingsClicked), false);

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
				this.fieldLeading.ButtonUnit.Clicked -= new MessageEventHandler(this.HandleButtonUnitClicked);
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
					this.fieldLeading.Bounds = r;
					r.Offset(69, 0);
					r.Width = 20;
					this.buttonLeadingMenu.Bounds = r;
					r.Offset(20, 0);
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
				else
				{
					r.Left = rect.Left;
					r.Width = 69;
					this.fieldLeading.Bounds = r;
					r.Offset(69, 0);
					r.Width = 20;
					this.buttonLeadingMenu.Bounds = r;
					r.Offset(20, 0);
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
				r.Width = 69;
				this.fieldLeading.Bounds = r;
				r.Offset(69, 0);
				r.Width = 20;
				this.buttonLeadingMenu.Bounds = r;
				r.Offset(20, 0);
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

			this.fieldLeading.IsUnitPercent = (units == Common.Text.Properties.SizeUnits.Percent);
			this.SetTextFieldRealValue(this.fieldLeading.TextFieldReal, leading, units, isLeading, false);

			this.ActiveIconButton(this.buttonAlignFirst, alignFirst, isAlign);
			this.ActiveIconButton(this.buttonAlignAll,   alignAll,   isAlign);

			this.ignoreChanged = false;
		}


		private void HandleButtonUnitClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

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

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.Leading = value;
			this.document.ParagraphWrapper.Defined.LeadingUnits = units;
			this.document.ParagraphWrapper.ResumeSynchronisations();
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

		private void HandleButtonLeadingMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(-48, 0));
			VMenu menu = this.CreateMenu(new MessageEventHandler(this.HandleMenuPressed));
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonLeadingMinusClicked(object sender, MessageEventArgs e)
		{
			this.document.Wrappers.IncrementParagraphLeading(-1);
		}

		private void HandleButtonLeadingPlusClicked(object sender, MessageEventArgs e)
		{
			this.document.Wrappers.IncrementParagraphLeading(1);
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


		#region Menu
		// Construit le menu pour choisir une page.
		protected VMenu CreateMenu(MessageEventHandler message)
		{
			VMenu menu = new VMenu();

			double leading = this.document.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.document.ParagraphWrapper.Active.LeadingUnits;
			bool percent = (units == Common.Text.Properties.SizeUnits.Percent);

			this.AddMenuItem(menu,  "50%", (leading == 0.5 && percent), message);
			this.AddMenuItem(menu,  "80%", (leading == 0.8 && percent), message);
			this.AddMenuItem(menu, "100%", (leading == 1.0 && percent), message);
			this.AddMenuItem(menu, "150%", (leading == 1.5 && percent), message);
			this.AddMenuItem(menu, "200%", (leading == 2.0 && percent), message);
			this.AddMenuItem(menu, "300%", (leading == 3.0 && percent), message);

			menu.Items.Add(new MenuSeparator());

			if ( this.document.Modifier.RealUnitDimension == RealUnitType.DimensionInch )
			{
				this.AddMenuItem(menu, 0.05*254, leading, !percent, message);  // 0.05in
				this.AddMenuItem(menu, 0.10*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.15*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.20*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.30*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.40*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.50*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.60*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.80*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.10*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.15*254, leading, !percent, message);
				this.AddMenuItem(menu, 0.20*254, leading, !percent, message);
			}
			else
			{
				this.AddMenuItem(menu,  10.0, leading, !percent, message);  // 1mm
				this.AddMenuItem(menu,  20.0, leading, !percent, message);
				this.AddMenuItem(menu,  30.0, leading, !percent, message);
				this.AddMenuItem(menu,  40.0, leading, !percent, message);
				this.AddMenuItem(menu,  60.0, leading, !percent, message);
				this.AddMenuItem(menu,  80.0, leading, !percent, message);
				this.AddMenuItem(menu, 100.0, leading, !percent, message);
				this.AddMenuItem(menu, 150.0, leading, !percent, message);
				this.AddMenuItem(menu, 200.0, leading, !percent, message);
				this.AddMenuItem(menu, 300.0, leading, !percent, message);
				this.AddMenuItem(menu, 400.0, leading, !percent, message);
				this.AddMenuItem(menu, 500.0, leading, !percent, message);
			}

			menu.AdjustSize();
			return menu;
		}

		// Ajoute une case au menu.
		protected void AddMenuItem(VMenu menu, double distance, double current, bool active, MessageEventHandler message)
		{
			string text = this.document.Modifier.RealToString(distance);
			string unit = string.Concat(text, " ", this.document.Modifier.ShortNameUnitDimension);

			if ( active )
			{
				active = (distance == current);
			}
			
			this.AddMenuItem(menu, unit, text, active, message);
		}

		// Ajoute une case au menu.
		protected void AddMenuItem(VMenu menu, string text, bool active, MessageEventHandler message)
		{
			this.AddMenuItem(menu, text, text, active, message);
		}

		// Ajoute une case au menu.
		protected void AddMenuItem(VMenu menu, string text, string name, bool active, MessageEventHandler message)
		{
			string icon = Misc.Icon("RadioNo");
			if ( active )
			{
				icon = Misc.Icon("RadioYes");
				text = Misc.Bold(text);
			}

			MenuItem item = new MenuItem("", icon, text, "", name);

			if ( message != null )
			{
				item.Pressed += message;
			}

			menu.Items.Add(item);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string text = item.Name;

			double leading;
			Common.Text.Properties.SizeUnits units;

			if ( text.EndsWith("%") )
			{
				text = text.Substring(0, text.Length-1);
				leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture) / 100;
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture) * this.document.Modifier.RealScale;
				units = Common.Text.Properties.SizeUnits.Points;
			}

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.Leading = leading;
			this.document.ParagraphWrapper.Defined.LeadingUnits = units;
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}
		#endregion

		
		protected Button					buttonUnits;
		protected Widgets.TextFieldLabel	fieldLeading;
		protected GlyphButton				buttonLeadingMenu;
		protected IconButton				buttonLeadingMinus;
		protected IconButton				buttonLeadingPlus;
		protected IconButton				buttonAlignFirst;
		protected IconButton				buttonAlignAll;
		protected IconButton				buttonSettings;
		protected IconButton				buttonClear;
	}
}

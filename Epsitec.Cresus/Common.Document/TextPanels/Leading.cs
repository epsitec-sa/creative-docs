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
		public Leading(Document document, bool isStyle) : base(document, isStyle)
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

			this.buttonLeadingMinus.Visibility = !this.isStyle;
			this.buttonLeadingPlus.Visibility  = !this.isStyle;

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldLeading.ButtonUnit.Clicked -= new MessageEventHandler(this.HandleButtonUnitClicked);
				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultStyle;
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
						h += 55;
					}
					else	// �tendu/compact ?
					{
						h += 55;
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


		protected override void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
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
		}

		private void HandleButtonLeadingMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(button.Width, 0));
			VMenu menu = this.CreateMenu();
			pos.X -= menu.Width;
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
		}


		#region Menu
		protected VMenu CreateMenu()
		{
			//	Construit le menu pour choisir l'interligne.
			VMenu menu = new VMenu();

			double leading = this.ParagraphWrapper.Active.Leading;
			Common.Text.Properties.SizeUnits units = this.ParagraphWrapper.Active.LeadingUnits;
			bool percent = (units == Common.Text.Properties.SizeUnits.Percent);

			return Menus.LeadingMenu.CreateLeadingMenu(this.document, leading, percent?"%":"", new MessageEventHandler(this.HandleMenuPressed));
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
				leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				leading = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
				units = Common.Text.Properties.SizeUnits.Points;
			}

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.Leading = leading;
			this.ParagraphWrapper.Defined.LeadingUnits = units;
			this.ParagraphWrapper.DefineOperationName("ParagraphLeading", Res.Strings.Action.ParagraphLeading);
			this.ParagraphWrapper.ResumeSynchronizations();
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

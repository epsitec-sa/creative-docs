using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Keep permet de choisir les groupements de paragraphes.
	/// </summary>
	public class Keep : Abstract
	{
		public Keep(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Keep.Title;

			this.fixIcon.Text = Misc.Image("TextKeep");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Keep.Title);

			this.buttonKeepNext = this.CreateIconButton(Misc.Icon("ParagraphKeepNext"), Res.Strings.TextPanel.Keep.Tooltip.KeepNext, this.HandleButtonKeepNextClicked);
			this.buttonKeepPrev = this.CreateIconButton(Misc.Icon("ParagraphKeepPrev"), Res.Strings.TextPanel.Keep.Tooltip.KeepPrev, this.HandleButtonKeepPrevClicked);

			this.fieldKeepStart = this.CreateTextFieldLabel(Res.Strings.TextPanel.Keep.Tooltip.KeepStart, Res.Strings.TextPanel.Keep.Short.KeepStart, Res.Strings.TextPanel.Keep.Long.KeepStart, 0.0, 100.0, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleKeepStartChanged);
			this.fieldKeepEnd   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Keep.Tooltip.KeepEnd,   Res.Strings.TextPanel.Keep.Short.KeepEnd,   Res.Strings.TextPanel.Keep.Long.KeepEnd,   0.0, 100.0, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleKeepEndChanged);
			
			this.fieldKeepStart.TextFieldReal.Resolution = 1.0M;
			this.fieldKeepStart.TextFieldReal.Scale      = 1.0M;
			this.fieldKeepStart.TextFieldReal.UnitType   = RealUnitType.Scalar;
			this.fieldKeepStart.TextFieldReal.MinValue   = 1M;
			this.fieldKeepStart.TextFieldReal.MaxValue   = 5M;
			
			this.fieldKeepEnd.TextFieldReal.Resolution = 1.0M;
			this.fieldKeepEnd.TextFieldReal.Scale      = 1.0M;
			this.fieldKeepEnd.TextFieldReal.UnitType   = RealUnitType.Scalar;
			this.fieldKeepEnd.TextFieldReal.MinValue   = 1M;
			this.fieldKeepEnd.TextFieldReal.MaxValue   = 5M;

			this.fieldStartMode = CreateComboStartMode(this.HandleStartModeChanged);

			this.labelNextStyle = new StaticText(this);
			this.labelNextStyle.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelNextStyle.Text = Res.Strings.TextPanel.Keep.Short.NextStyle;

			this.fieldNextStyle = new Widgets.StyleCombo(this);
			this.fieldNextStyle.StyleCategory = StyleCategory.Paragraph;
			this.fieldNextStyle.Document = this.document;
			this.fieldNextStyle.IsNoneLine = true;
			this.fieldNextStyle.IsDeep = true;
			this.fieldNextStyle.IsReadOnly = true;
			this.fieldNextStyle.AutoFocus = false;
			this.fieldNextStyle.TabIndex = this.tabIndex++;
			this.fieldNextStyle.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.fieldNextStyle.ComboClosed += this.HandleStyleComboClosed;
			ToolTip.Default.SetToolTip(this.fieldNextStyle, Res.Strings.TextPanel.Keep.Tooltip.NextStyle);

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
				this.fieldNextStyle.ComboClosed -= this.HandleStyleComboClosed;

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
						h += 105;
					}
					else	// étendu/compact ?
					{
						h += 55;
					}

					if ( this.isStyle )
					{
						h += 25;  // place pour NextStyle
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}


		protected TextFieldCombo CreateComboStartMode(EventHandler handler)
		{
			//	Crée le combo pour le StartMode.
			TextFieldCombo combo = new TextFieldCombo(this);

			combo.PreferredWidth = 180;
			combo.IsReadOnly = true;

			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.Undefined);
			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.Anywhere);
			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.NewFrame);
			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.NewPage);
			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.NewOddPage);
			combo.Items.Add(Res.Strings.TextPanel.Keep.StartMode.NewEvenPage);

			combo.SelectedItemChanged += handler;

			return combo;
		}

		protected static Common.Text.Properties.ParagraphStartMode StringToMode(string text)
		{
			if ( text == Res.Strings.TextPanel.Keep.StartMode.Anywhere    )  return Common.Text.Properties.ParagraphStartMode.Anywhere;
			if ( text == Res.Strings.TextPanel.Keep.StartMode.NewFrame    )  return Common.Text.Properties.ParagraphStartMode.NewFrame;
			if ( text == Res.Strings.TextPanel.Keep.StartMode.NewPage     )  return Common.Text.Properties.ParagraphStartMode.NewPage;
			if ( text == Res.Strings.TextPanel.Keep.StartMode.NewOddPage  )  return Common.Text.Properties.ParagraphStartMode.NewOddPage;
			if ( text == Res.Strings.TextPanel.Keep.StartMode.NewEvenPage )  return Common.Text.Properties.ParagraphStartMode.NewEvenPage;
			return Common.Text.Properties.ParagraphStartMode.Undefined;
		}

		public static string ModeToString(Common.Text.Properties.ParagraphStartMode mode)
		{
			switch ( mode )
			{
				case Common.Text.Properties.ParagraphStartMode.Anywhere:     return Res.Strings.TextPanel.Keep.StartMode.Anywhere;
				case Common.Text.Properties.ParagraphStartMode.NewFrame:     return Res.Strings.TextPanel.Keep.StartMode.NewFrame;
				case Common.Text.Properties.ParagraphStartMode.NewPage:      return Res.Strings.TextPanel.Keep.StartMode.NewPage;
				case Common.Text.Properties.ParagraphStartMode.NewOddPage:   return Res.Strings.TextPanel.Keep.StartMode.NewOddPage;
				case Common.Text.Properties.ParagraphStartMode.NewEvenPage:  return Res.Strings.TextPanel.Keep.StartMode.NewEvenPage;
			}
			return Res.Strings.TextPanel.Keep.StartMode.Undefined;
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

			if ( this.buttonKeepNext == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonKeepNext.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonKeepPrev.SetManualBounds(r);
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldKeepStart.SetManualBounds(r);
					this.fieldKeepStart.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldKeepEnd.SetManualBounds(r);
					this.fieldKeepEnd.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldStartMode.SetManualBounds(r);
					this.fieldStartMode.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Left+45-5;
					this.labelNextStyle.SetManualBounds(r);
					this.labelNextStyle.Visibility = this.isStyle;
					r.Left = rect.Left+45;
					r.Right = rect.Right;
					this.fieldNextStyle.SetManualBounds(r);
					this.fieldNextStyle.Visibility = this.isStyle;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonKeepNext.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonKeepPrev.SetManualBounds(r);
					r.Offset(20, 0);
					r.Width = 60;
					this.fieldKeepStart.SetManualBounds(r);
					r.Offset(60, 0);
					this.fieldKeepEnd.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldStartMode.SetManualBounds(r);
					this.fieldStartMode.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Left+45-5;
					this.labelNextStyle.SetManualBounds(r);
					this.labelNextStyle.Visibility = this.isStyle;
					r.Left = rect.Left+45;
					r.Right = rect.Right;
					this.fieldNextStyle.SetManualBounds(r);
					this.fieldNextStyle.Visibility = this.isStyle;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonKeepNext.SetManualBounds(r);
				r.Offset(20, 0);
				this.buttonKeepPrev.SetManualBounds(r);
				r.Offset(20, 0);
				r.Width = 60;
				this.fieldKeepStart.SetManualBounds(r);
				r.Offset(60, 0);
				this.fieldKeepEnd.SetManualBounds(r);

				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

				this.fieldStartMode.Visibility = false;
				this.labelNextStyle.Visibility = false;
				this.fieldNextStyle.Visibility = false;
			}
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			bool keepNext    = this.ParagraphWrapper.Active.KeepWithNextParagraph;
			bool keepPrev    = this.ParagraphWrapper.Active.KeepWithPreviousParagraph;
			int  keepStart   = this.ParagraphWrapper.Active.KeepStartLines;
			int  keepEnd     = this.ParagraphWrapper.Active.KeepEndLines;
			bool isKeepNext  = this.ParagraphWrapper.Defined.IsKeepWithNextParagraphDefined;
			bool isKeepPrev  = this.ParagraphWrapper.Defined.IsKeepWithPreviousParagraphDefined;
			bool isKeepStart = this.ParagraphWrapper.Defined.IsKeepStartLinesDefined;
			bool isKeepEnd   = this.ParagraphWrapper.Defined.IsKeepEndLinesDefined;

			Common.Text.Properties.ParagraphStartMode mode = this.ParagraphWrapper.Active.ParagraphStartMode;
			bool isMode = this.ParagraphWrapper.Defined.IsParagraphStartModeDefined;

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonKeepNext, keepNext, isKeepNext);
			this.ActiveIconButton(this.buttonKeepPrev, keepPrev, isKeepPrev);

			this.SetTextFieldRealValue(this.fieldKeepStart.TextFieldReal, keepStart, Common.Text.Properties.SizeUnits.Points, isKeepStart, false);
			this.SetTextFieldRealValue(this.fieldKeepEnd.TextFieldReal,   keepEnd,   Common.Text.Properties.SizeUnits.Points, isKeepEnd,   false);

			this.fieldStartMode.Text = Keep.ModeToString(mode);
			this.fieldStartMode.TextDisplayMode = isMode ? TextFieldDisplayMode.OverriddenValue : TextFieldDisplayMode.InheritedValue;

			Text.TextStyle currentStyle = this.ParagraphWrapper.AttachedStyle;
			string nextStyleName = "";
			if ( currentStyle != null )
			{
				Text.TextStyle nextStyle = currentStyle.NextStyle;
				if ( nextStyle != null )
				{
					nextStyleName = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(nextStyle));
				}
			}
			this.fieldNextStyle.Text = nextStyleName;

			int rank = -1;
			Text.TextStyle[] styles = this.document.TextStyles(StyleCategory.Paragraph);
			for ( int i=0 ; i<styles.Length ; i++ )
			{
				Text.TextStyle style = styles[i];
				if ( style == currentStyle )  rank = i;
			}
			this.fieldNextStyle.ExcludeRank = rank;

			this.ignoreChanged = false;
		}



		private void HandleButtonKeepNextClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			bool value = (this.buttonKeepNext.ActiveState == ActiveState.No);

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.KeepWithNextParagraph = value;
			this.ParagraphWrapper.DefineOperationName("ParagraphKeep", Res.Strings.TextPanel.Keep.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleButtonKeepPrevClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			bool value = (this.buttonKeepPrev.ActiveState == ActiveState.No);

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.KeepWithPreviousParagraph = value;
			this.ParagraphWrapper.DefineOperationName("ParagraphKeep", Res.Strings.TextPanel.Keep.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleKeepStartChanged(object sender)
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
				this.ParagraphWrapper.Defined.KeepStartLines = (int) value;
			}
			else
			{
				this.ParagraphWrapper.Defined.ClearKeepStartLines();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphKeep", Res.Strings.TextPanel.Keep.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		
		private void HandleKeepEndChanged(object sender)
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
				this.ParagraphWrapper.Defined.KeepEndLines = (int) value;
			}
			else
			{
				this.ParagraphWrapper.Defined.ClearKeepEndLines();
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphKeep", Res.Strings.TextPanel.Keep.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		
		private void HandleStartModeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			Common.Text.Properties.ParagraphStartMode mode = Keep.StringToMode(this.fieldStartMode.Text);

			this.ParagraphWrapper.SuspendSynchronizations();

			if ( mode == Common.Text.Properties.ParagraphStartMode.Undefined )
			{
				this.ParagraphWrapper.Defined.ClearParagraphStartMode();
			}
			else
			{
				this.ParagraphWrapper.Defined.ParagraphStartMode = mode;
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphKeep", Res.Strings.TextPanel.Keep.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleStyleComboClosed(object sender)
		{
			//	Combo des styles fermé.
			int sel = this.fieldNextStyle.SelectedItemIndex;
			if ( sel == -1 )  return;

			Text.TextStyle currentStyle = this.ParagraphWrapper.AttachedStyle;
			Text.TextStyle nextStyle;

			if ( sel == -2 )  // ligne <aucun> ?
			{
				nextStyle = null;
				this.fieldNextStyle.Text = "";
			}
			else
			{
				Text.TextStyle[] styles = this.document.TextStyles(StyleCategory.Paragraph);
				nextStyle = styles[sel];
				this.fieldNextStyle.Text = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(nextStyle));
			}

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange);
			this.document.TextContext.StyleList.SetNextStyle(this.document.Modifier.OpletQueue, currentStyle, nextStyle);
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearKeepWithNextParagraph();
			this.ParagraphWrapper.Defined.ClearKeepWithPreviousParagraph();
			this.ParagraphWrapper.Defined.ClearKeepStartLines();
			this.ParagraphWrapper.Defined.ClearKeepEndLines();
			this.ParagraphWrapper.Defined.ClearParagraphStartMode();
			this.ParagraphWrapper.DefineOperationName("ParagraphKeepClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		protected IconButton				buttonKeepNext;
		protected IconButton				buttonKeepPrev;
		protected Widgets.TextFieldLabel	fieldKeepStart;
		protected Widgets.TextFieldLabel	fieldKeepEnd;
		protected TextFieldCombo			fieldStartMode;
		protected StaticText				labelNextStyle;
		protected Widgets.StyleCombo		fieldNextStyle;
		protected IconButton				buttonClear;
	}
}

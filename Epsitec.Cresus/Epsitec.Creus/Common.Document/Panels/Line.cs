using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Line permet de choisir un mode de trait.
	/// </summary>
	public class Line : Abstract
	{
		public Line(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += HandleTypeChanged;
			this.grid.TabIndex = 1;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.StandardDashType.Full);
			this.AddRadioIcon(Properties.StandardDashType.Line);
			this.AddRadioIcon(Properties.StandardDashType.LineDense);
			this.AddRadioIcon(Properties.StandardDashType.Dot);

			this.AddRadioIcon(Properties.StandardDashType.LineExpand);
			this.AddRadioIcon(Properties.StandardDashType.LineDot);
			this.AddRadioIcon(Properties.StandardDashType.LineDotDot);
			this.AddRadioIcon(Properties.StandardDashType.Custom);

			this.gridCap = new RadioIconGrid(this);
			this.gridCap.SelectionChanged += HandleTypeChanged;
			this.gridCap.TabIndex = 100;
			this.gridCap.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(CapStyle.Round);
			this.AddRadioIcon(CapStyle.Square);
			this.AddRadioIcon(CapStyle.Butt);

			this.gridJoin = new RadioIconGrid(this);
			this.gridJoin.SelectionChanged += HandleTypeChanged;
			this.gridJoin.TabIndex = 101;
			this.gridJoin.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(JoinStyle.Round);
			this.AddRadioIcon(JoinStyle.Miter);
			this.AddRadioIcon(JoinStyle.Bevel);

			this.nothingButton = new IconButton(this);
			this.nothingButton.Clicked += this.HandleNothingClicked;
			this.nothingButton.TabIndex = 2;
			this.nothingButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.nothingButton.IconUri = Misc.Icon("Nothing");
			ToolTip.Default.SetToolTip(this.nothingButton, Res.Strings.Panel.Line.Tooltip.Nothing);

			this.field = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.field.LabelLongText = Res.Strings.Panel.Line.Long.Width;
			this.field.TextFieldReal.FactorMinRange = 0.0M;
			this.field.TextFieldReal.FactorMaxRange = 0.1M;
			this.field.TextFieldReal.FactorStep = 0.1M;
			this.field.TextFieldReal.LogarithmicDivisor = 3.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.field.TextFieldReal);
			this.field.TextFieldReal.EditionAccepted += this.HandleTextChanged;
			this.field.TabIndex = 2;
			this.field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.Line.Tooltip.Width);

			this.radioDashRank = new RadioButton[Properties.Line.DashMax];
			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i] = new RadioButton(this);
				this.radioDashRank[i].ActiveStateChanged += this.HandleDashRankChanged;
				this.radioDashRank[i].Index = i;
				this.radioDashRank[i].TabIndex = 10+i;
				this.radioDashRank[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.radioDashRank[i].Index = i;
			}
			ToolTip.Default.SetToolTip(this.radioDashRank[0], Res.Strings.Panel.Line.Tooltip.Dash1);
			ToolTip.Default.SetToolTip(this.radioDashRank[1], Res.Strings.Panel.Line.Tooltip.Dash2);
			ToolTip.Default.SetToolTip(this.radioDashRank[2], Res.Strings.Panel.Line.Tooltip.Dash3);
			this.RadioSelected = 0;

			this.fieldStandardLength = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldStandardLength.LabelShortText = Res.Strings.Panel.Line.Short.DashLength;
			this.fieldStandardLength.LabelLongText  = Res.Strings.Panel.Line.Long.DashLength;
			this.fieldStandardLength.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldStandardLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldStandardLength.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldStandardLength.TextFieldReal);
			this.fieldStandardLength.TextFieldReal.EditionAccepted += this.HandleDashChanged;
			this.fieldStandardLength.TabIndex = 20;
			this.fieldStandardLength.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldStandardLength, Res.Strings.Panel.Line.Tooltip.DashLength);

			this.fieldDashPen = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldDashPen.LabelShortText = Res.Strings.Panel.Line.Short.DashPen;
			this.fieldDashPen.LabelLongText  = Res.Strings.Panel.Line.Long.DashPen;
			this.fieldDashPen.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldDashPen.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldDashPen.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashPen.TextFieldReal);
			this.fieldDashPen.TextFieldReal.EditionAccepted += this.HandleDashChanged;
			this.fieldDashPen.TabIndex = 21;
			this.fieldDashPen.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashPen, Res.Strings.Panel.Line.Tooltip.DashPen);

			this.fieldDashGap = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldDashGap.LabelShortText = Res.Strings.Panel.Line.Short.DashGap;
			this.fieldDashGap.LabelLongText  = Res.Strings.Panel.Line.Long.DashGap;
			this.fieldDashGap.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldDashGap.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldDashGap.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashGap.TextFieldReal);
			this.fieldDashGap.TextFieldReal.EditionAccepted += this.HandleDashChanged;
			this.fieldDashGap.TabIndex = 22;
			this.fieldDashGap.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashGap, Res.Strings.Panel.Line.Tooltip.DashGap);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.StandardDashType type)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Line.GetIconText(type)), Properties.Line.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(CapStyle type)
		{
			this.gridCap.AddRadioIcon(Misc.Icon(Properties.Line.GetIconText(type)), Properties.Line.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(JoinStyle type)
		{
			this.gridJoin.AddRadioIcon(Misc.Icon(Properties.Line.GetIconText(type)), Properties.Line.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= HandleTypeChanged;
				this.gridCap.SelectionChanged -= HandleTypeChanged;
				this.gridJoin.SelectionChanged -= HandleTypeChanged;
				this.nothingButton.Clicked -= this.HandleNothingClicked;
				this.field.TextFieldReal.EditionAccepted -= this.HandleTextChanged;

				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveStateChanged -= this.HandleDashRankChanged;
					this.radioDashRank[i] = null;
				}
				this.fieldStandardLength.TextFieldReal.EditionAccepted -= this.HandleDashChanged;
				this.fieldDashPen.TextFieldReal.EditionAccepted -= this.HandleDashChanged;
				this.fieldDashGap.TextFieldReal.EditionAccepted -= this.HandleDashChanged;

				this.grid = null;
				this.field = null;
				this.fieldDashPen = null;
				this.fieldDashGap = null;
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
						h += 83;

						Properties.StandardDashType type = (Properties.StandardDashType) this.grid.SelectedValue;
						if ( type == Properties.StandardDashType.Custom )
						{
							h += 80;
						}
						else if ( type != Properties.StandardDashType.Full )
						{
							h += 25;
						}
					}
					else	// étendu/compact ?
					{
						h += 106;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.field.TextFieldReal.InternalValue = (decimal) p.Width;

			this.gridCap.SelectedValue  = (int) p.Cap;
			this.gridJoin.SelectedValue = (int) p.Join;

			this.DashToWidget();
			this.grid.SelectedValue = (int) p.StandardDash;
			this.fieldStandardLength.TextFieldReal.InternalValue = (decimal) p.StandardLength;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			p.Width = (double) this.field.TextFieldReal.InternalValue;

			p.Cap  = (CapStyle)  this.gridCap.SelectedValue;
			p.Join = (JoinStyle) this.gridJoin.SelectedValue;

			Properties.StandardDashType newType = (Properties.StandardDashType) this.grid.SelectedValue;
			if ( newType == Properties.StandardDashType.Custom )
			{
				p.StandardDash = newType;
				this.WidgetToDash();

				this.ignoreChanged = true;
				this.fieldStandardLength.TextFieldReal.InternalValue = (decimal) p.StandardLength;
				this.ignoreChanged = false;
			}
			else
			{
				p.StandardLength = (double) this.fieldStandardLength.TextFieldReal.InternalValue;
				p.StandardDash = newType;

				this.ignoreChanged = true;
				this.DashToWidget();
				this.ignoreChanged = false;
			}
		}

		protected void DashToWidget()
		{
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			int i = this.RadioSelected;
			this.fieldDashPen.TextFieldReal.InternalValue = (decimal) p.GetDashPen(i);
			this.fieldDashGap.TextFieldReal.InternalValue = (decimal) p.GetDashGap(i);
		}

		protected void WidgetToDash()
		{
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			int i = this.RadioSelected;
			p.SetDashPen(i, (double) this.fieldDashPen.TextFieldReal.InternalValue);
			p.SetDashGap(i, (double) this.fieldDashGap.TextFieldReal.InternalValue);
		}

		protected int RadioSelected
		{
			get
			{
				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					if ( this.radioDashRank[i].ActiveState == ActiveState.Yes )
					{
						return i;
					}
				}
				return 0;
			}

			set
			{
				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveState = (i==value) ? ActiveState.Yes : ActiveState.No;
				}
			}
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool showStandardLength = false;
			bool enableStandardLength = false;
			bool showDash = false;
			bool enableDash = false;
			bool capJoin = false;

			Properties.StandardDashType type = (Properties.StandardDashType) this.grid.SelectedValue;

			if ( this.isExtendedSize )  // panneau étendu ?
			{
				if ( this.IsLabelProperties )  // détails ?
				{
					if ( type != Properties.StandardDashType.Full   &&
						 type != Properties.StandardDashType.Custom )
					{
						showStandardLength = true;
						enableStandardLength = true;
					}
					if ( type == Properties.StandardDashType.Custom )
					{
						showDash = true;
						enableDash = true;
					}
				}
				else	// compact ?
				{
					showStandardLength = true;
					if ( type != Properties.StandardDashType.Full   &&
						 type != Properties.StandardDashType.Custom )
					{
						enableStandardLength = true;
					}
					showDash = true;
					if ( type == Properties.StandardDashType.Custom )
					{
						enableDash = true;
					}
				}

				capJoin = true;
			}

			this.fieldStandardLength.Visibility = (showStandardLength);
			this.fieldStandardLength.Enable = (enableStandardLength);

			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i].Visibility = (showDash);
				this.radioDashRank[i].Enable = (enableDash);
			}

			this.fieldDashPen.Visibility = (showDash);
			this.fieldDashPen.Enable = (enableDash);
			this.fieldDashGap.Visibility = (showDash);
			this.fieldDashGap.Enable = (enableDash);

			this.gridCap.Visibility = (capJoin);
			this.gridCap.Enable = (capJoin);

			this.gridJoin.Visibility = (capJoin);
			this.gridJoin.Enable = (capJoin);
		}

		protected void UpdateShortLongText()
		{
			//	Adapte les textes courts ou longs.
			if ( this.IsLabelProperties )
			{
				Abstract.SetText(this.radioDashRank[0], Res.Strings.Panel.Line.Long.Dash1);
				Abstract.SetText(this.radioDashRank[1], Res.Strings.Panel.Line.Long.Dash2);
				Abstract.SetText(this.radioDashRank[2], Res.Strings.Panel.Line.Long.Dash3);
			}
			else
			{
				Abstract.SetText(this.radioDashRank[0], "");
				Abstract.SetText(this.radioDashRank[1], "");
				Abstract.SetText(this.radioDashRank[2], ":");
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.UpdateShortLongText();
			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;
			Rectangle r = rect;
			double pTop = rect.Top;

			if ( this.isExtendedSize )  // panneau étendu ?
			{
				if ( this.IsLabelProperties )  // étendu/détails ?
				{
					r.Top = pTop;
					r.Bottom = r.Top-22;
					r.Left = rect.Left;
					r.Right = rect.Right;
					r.Inflate(1);
					this.grid.SetManualBounds(r);

					pTop -= 25;
					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Left;
					r.Width = 20;
					this.nothingButton.SetManualBounds(r);
					r.Left = r.Right;
					r.Right = rect.Right;
					this.field.SetManualBounds(r);
					pTop -= 25;

					Properties.StandardDashType type = (Properties.StandardDashType) this.grid.SelectedValue;
					if ( type == Properties.StandardDashType.Custom )
					{
						r.Top = pTop-8;
						r.Bottom = r.Top-16;
						r.Left = rect.Left;
						r.Width = 180/3;
						for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
						{
							this.radioDashRank[i].SetManualBounds(r);
							r.Offset(r.Width, 0);
						}

						r.Top = r.Bottom;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldDashPen.SetManualBounds(r);

						r.Top = r.Bottom-2;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldDashGap.SetManualBounds(r);

						pTop -= 80;
					}
					else if ( type != Properties.StandardDashType.Full )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldStandardLength.SetManualBounds(r);
						pTop -= 25;
					}
				}
				else	// étendu/compact ?
				{
					r.Top = pTop;
					r.Bottom = r.Top-22*2;
					r.Left = rect.Left;
					r.Width = 22*4;
					r.Inflate(1);
					this.grid.SetManualBounds(r);

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-20;
					r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					this.nothingButton.SetManualBounds(r);
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.field.SetManualBounds(r);
					pTop -= 25;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Right = rect.Right;
					this.fieldStandardLength.SetManualBounds(r);
					pTop -= 25;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Left;
					r.Width = 16;
					for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
					{
						if ( i == Properties.Line.DashMax-1 )  r.Width = 32;
						this.radioDashRank[i].SetManualBounds(r);
						r.Offset(r.Width, 0);
					}
					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldDashPen.SetManualBounds(r);
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldDashGap.SetManualBounds(r);
					pTop -= 25;
				}

				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridCap.SetManualBounds(r);
				r.Left = rect.Right-22*3;
				r.Right = rect.Right;
				this.gridJoin.SetManualBounds(r);
			}
			else	// panneau réduit ?
			{
				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 22*4;
				r.Inflate(1);
				this.grid.SetManualBounds(r);

				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-20;
				r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
				this.nothingButton.SetManualBounds(r);
				r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
				r.Right = rect.Right;
				this.field.SetManualBounds(r);
			}
		}

		private void HandleTextChanged(object sender)
		{
			//	Une valeur a été changée.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleNothingClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton "aucun trait" a été cliqué.
			this.field.TextFieldReal.Value = 0.0M;
			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;

			if ( sender == this.grid && this.IsLabelProperties )
			{
				this.HeightChanged();
			}

			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleDashRankChanged(object sender)
		{
			//	Le rang a été changé.
			if ( this.ignoreChanged )  return;
			this.ignoreChanged = true;
			this.DashToWidget();
			this.ignoreChanged = false;
		}

		private void HandleDashChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected RadioIconGrid				grid;
		protected RadioIconGrid				gridCap;
		protected RadioIconGrid				gridJoin;
		protected IconButton				nothingButton;
		protected Widgets.TextFieldLabel	field;
		protected RadioButton[]				radioDashRank;
		protected Widgets.TextFieldLabel	fieldStandardLength;
		protected Widgets.TextFieldLabel	fieldDashPen;
		protected Widgets.TextFieldLabel	fieldDashGap;
	}
}

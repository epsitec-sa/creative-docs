using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Line permet de choisir un mode de trait.
	/// </summary>
	[SuppressBundleSupport]
	public class Line : Abstract
	{
		public Line(Document document) : base(document)
		{
			this.grid = new Widgets.RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 1;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.StandardDashType.Full);
			this.AddRadioIcon(Properties.StandardDashType.Line);
			this.AddRadioIcon(Properties.StandardDashType.LineDense);
			this.AddRadioIcon(Properties.StandardDashType.Dot);

			this.AddRadioIcon(Properties.StandardDashType.LineExpand);
			this.AddRadioIcon(Properties.StandardDashType.LineDot);
			this.AddRadioIcon(Properties.StandardDashType.LineDotDot);
			this.AddRadioIcon(Properties.StandardDashType.Custom);

			this.gridCap = new Widgets.RadioIconGrid(this);
			this.gridCap.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridCap.TabIndex = 100;
			this.gridCap.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(CapStyle.Round);
			this.AddRadioIcon(CapStyle.Square);
			this.AddRadioIcon(CapStyle.Butt);

			this.gridJoin = new Widgets.RadioIconGrid(this);
			this.gridJoin.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridJoin.TabIndex = 101;
			this.gridJoin.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(JoinStyle.Round);
			this.AddRadioIcon(JoinStyle.Miter);
			this.AddRadioIcon(JoinStyle.Bevel);

			this.nothingButton = new IconButton(this);
			this.nothingButton.Clicked += new MessageEventHandler(this.HandleNothingClicked);
			this.nothingButton.TabIndex = 2;
			this.nothingButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.nothingButton.IconName = Misc.Icon("Nothing");
			ToolTip.Default.SetToolTip(this.nothingButton, Res.Strings.Panel.Line.Tooltip.Nothing);

			this.field = new Widgets.TextFieldLabel(this, false);
			this.field.LabelLongText = Res.Strings.Panel.Line.Long.Width;
			this.field.TextFieldReal.FactorMinRange = 0.0M;
			this.field.TextFieldReal.FactorMaxRange = 0.1M;
			this.field.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.field.TextFieldReal);
			this.field.TextFieldReal.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 2;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.Line.Tooltip.Width);

			this.radioDashRank = new RadioButton[Properties.Line.DashMax];
			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i] = new RadioButton(this);
				this.radioDashRank[i].ActiveStateChanged += new EventHandler(this.HandleDashRankChanged);
				this.radioDashRank[i].TabIndex = 20+i;
				this.radioDashRank[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}
			ToolTip.Default.SetToolTip(this.radioDashRank[0], Res.Strings.Panel.Line.Tooltip.Dash1);
			ToolTip.Default.SetToolTip(this.radioDashRank[1], Res.Strings.Panel.Line.Tooltip.Dash2);
			ToolTip.Default.SetToolTip(this.radioDashRank[2], Res.Strings.Panel.Line.Tooltip.Dash3);
			this.RadioSelected = 0;

			this.fieldStandardLength = new Widgets.TextFieldLabel(this, false);
			this.fieldStandardLength.LabelShortText = Res.Strings.Panel.Line.Short.DashLength;
			this.fieldStandardLength.LabelLongText  = Res.Strings.Panel.Line.Long.DashLength;
			this.fieldStandardLength.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldStandardLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldStandardLength.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldStandardLength.TextFieldReal);
			this.fieldStandardLength.TextFieldReal.ValueChanged += new EventHandler(this.HandleDashChanged);
			this.fieldStandardLength.TabIndex = 20;
			this.fieldStandardLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldStandardLength, Res.Strings.Panel.Line.Tooltip.DashLength);

			this.fieldDashPen = new Widgets.TextFieldLabel(this, false);
			this.fieldDashPen.LabelShortText = Res.Strings.Panel.Line.Short.DashPen;
			this.fieldDashPen.LabelLongText  = Res.Strings.Panel.Line.Long.DashPen;
			this.fieldDashPen.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldDashPen.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldDashPen.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashPen.TextFieldReal);
			this.fieldDashPen.TextFieldReal.ValueChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashPen.TabIndex = 21;
			this.fieldDashPen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashPen, Res.Strings.Panel.Line.Tooltip.DashPen);

			this.fieldDashGap = new Widgets.TextFieldLabel(this, false);
			this.fieldDashGap.LabelShortText = Res.Strings.Panel.Line.Short.DashGap;
			this.fieldDashGap.LabelLongText  = Res.Strings.Panel.Line.Long.DashGap;
			this.fieldDashGap.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldDashGap.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldDashGap.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashGap.TextFieldReal);
			this.fieldDashGap.TextFieldReal.ValueChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashGap.TabIndex = 22;
			this.fieldDashGap.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashGap, Res.Strings.Panel.Line.Tooltip.DashGap);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.StandardDashType type)
		{
			this.grid.AddRadioIcon(Properties.Line.GetIconText(type), Properties.Line.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(CapStyle type)
		{
			this.gridCap.AddRadioIcon(Properties.Line.GetIconText(type), Properties.Line.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(JoinStyle type)
		{
			this.gridJoin.AddRadioIcon(Properties.Line.GetIconText(type), Properties.Line.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.gridCap.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.gridJoin.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.nothingButton.Clicked -= new MessageEventHandler(this.HandleNothingClicked);
				this.field.TextFieldReal.TextChanged -= new EventHandler(this.HandleTextChanged);

				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveStateChanged -= new EventHandler(this.HandleDashRankChanged);
					this.radioDashRank[i] = null;
				}
				this.fieldStandardLength.TextFieldReal.ValueChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashPen.TextFieldReal.ValueChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashGap.TextFieldReal.ValueChanged -= new EventHandler(this.HandleDashChanged);

				this.grid = null;
				this.field = null;
				this.fieldDashPen = null;
				this.fieldDashGap = null;
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

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
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

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
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
					if ( this.radioDashRank[i].ActiveState == WidgetState.ActiveYes )
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
					this.radioDashRank[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
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

			this.fieldStandardLength.SetVisible(showStandardLength);
			this.fieldStandardLength.SetEnabled(enableStandardLength);

			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i].SetVisible(showDash);
				this.radioDashRank[i].SetEnabled(enableDash);
			}

			this.fieldDashPen.SetVisible(showDash);
			this.fieldDashPen.SetEnabled(enableDash);
			this.fieldDashGap.SetVisible(showDash);
			this.fieldDashGap.SetEnabled(enableDash);

			this.gridCap.SetVisible(capJoin);
			this.gridCap.SetEnabled(capJoin);

			this.gridJoin.SetVisible(capJoin);
			this.gridJoin.SetEnabled(capJoin);
		}

		// Adapte les textes courts ou longs.
		protected void UpdateShortLongText()
		{
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

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
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
					this.grid.Bounds = r;

					pTop -= 25;
					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Left;
					r.Width = 20;
					this.nothingButton.Bounds = r;
					r.Left = r.Right;
					r.Right = rect.Right;
					this.field.Bounds = r;
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
							this.radioDashRank[i].Bounds = r;
							r.Offset(r.Width, 0);
						}

						r.Top = r.Bottom;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldDashPen.Bounds = r;

						r.Top = r.Bottom-2;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldDashGap.Bounds = r;

						pTop -= 80;
					}
					else if ( type != Properties.StandardDashType.Full )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldStandardLength.Bounds = r;
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
					this.grid.Bounds = r;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-20;
					r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					this.nothingButton.Bounds = r;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.field.Bounds = r;
					pTop -= 25;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Right = rect.Right;
					this.fieldStandardLength.Bounds = r;
					pTop -= 25;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Left;
					r.Width = 16;
					for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
					{
						if ( i == Properties.Line.DashMax-1 )  r.Width = 32;
						this.radioDashRank[i].Bounds = r;
						r.Offset(r.Width, 0);
					}
					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldDashPen.Bounds = r;
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldDashGap.Bounds = r;
					pTop -= 25;
				}

				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridCap.Bounds = r;
				r.Left = rect.Right-22*3;
				r.Right = rect.Right;
				this.gridJoin.Bounds = r;
			}
			else	// panneau réduit ?
			{
				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 22*4;
				r.Inflate(1);
				this.grid.Bounds = r;

				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-20;
				r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
				this.nothingButton.Bounds = r;
				r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
				r.Right = rect.Right;
				this.field.Bounds = r;
			}
		}

		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le bouton "aucun trait" a été cliqué.
		private void HandleNothingClicked(object sender, MessageEventArgs e)
		{
			this.field.TextFieldReal.Value = 0.0M;
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			if ( sender == this.grid && this.IsLabelProperties )
			{
				this.HeightChanged();
			}

			this.EnableWidgets();
			this.OnChanged();
		}

		// Le rang a été changé.
		private void HandleDashRankChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.ignoreChanged = true;
			this.DashToWidget();
			this.ignoreChanged = false;
		}

		// Un champ a été changé.
		private void HandleDashChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		grid;
		protected Widgets.RadioIconGrid		gridCap;
		protected Widgets.RadioIconGrid		gridJoin;
		protected IconButton				nothingButton;
		protected Widgets.TextFieldLabel	field;
		protected RadioButton[]				radioDashRank;
		protected Widgets.TextFieldLabel	fieldStandardLength;
		protected Widgets.TextFieldLabel	fieldDashPen;
		protected Widgets.TextFieldLabel	fieldDashGap;
	}
}

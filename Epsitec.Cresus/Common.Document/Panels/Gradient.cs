using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Gradient permet de choisir un d�grad� de couleurs.
	/// </summary>
	public class Gradient : Abstract
	{
		public Gradient(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 1;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.GradientFillType.None, false);
			this.AddRadioIcon(Properties.GradientFillType.Linear, false);
			this.AddRadioIcon(Properties.GradientFillType.Circle, false);
			this.AddRadioIcon(Properties.GradientFillType.Diamond, false);

			this.AddRadioIcon(Properties.GradientFillType.Conic, false);
			this.AddRadioIcon(Properties.GradientFillType.Hatch, false);
			this.AddRadioIcon(Properties.GradientFillType.Dots, false);
			this.AddRadioIcon(Properties.GradientFillType.Squares, false);

			this.nothingButton = new IconButton(this);
			this.nothingButton.Clicked += new MessageEventHandler(this.HandleNothingClicked);
			this.nothingButton.TabIndex = 2;
			this.nothingButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.nothingButton.IconName = Misc.Icon("Nothing");
			ToolTip.Default.SetToolTip(this.nothingButton, Res.Strings.Panel.Gradient.Tooltip.Nothing);

			this.fieldColor1 = new ColorSample(this);
			this.fieldColor1.PossibleSource = true;
			this.fieldColor1.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor1.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fieldColor1.TabIndex = 3;
			this.fieldColor1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor1, Res.Strings.Panel.Gradient.Tooltip.Color1);

			this.fieldColor2 = new ColorSample(this);
			this.fieldColor2.PossibleSource = true;
			this.fieldColor2.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor2.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fieldColor2.TabIndex = 4;
			this.fieldColor2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor2, Res.Strings.Panel.Gradient.Tooltip.Color2);

			this.reset = new Button(this);
			this.reset.Text = Res.Strings.Panel.Gradient.Button.Reset;
			this.reset.TabIndex = 5;
			this.reset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += new MessageEventHandler(this.HandleReset);
			ToolTip.Default.SetToolTip(this.reset, Res.Strings.Panel.Gradient.Tooltip.Reset);

			this.fieldAngle = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldAngle.LabelShortText = Res.Strings.Panel.Gradient.Short.Angle;
			this.fieldAngle.LabelLongText  = Res.Strings.Panel.Gradient.Long.Angle;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldAngle.TextFieldReal);
			this.fieldAngle.TextFieldReal.InternalMinValue = -360.0M;
			this.fieldAngle.TextFieldReal.InternalMaxValue =  360.0M;
			this.fieldAngle.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldAngle.TabIndex = 6;
			this.fieldAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldAngle, Res.Strings.Panel.Gradient.Tooltip.Angle);

			this.radioHatchRank = new RadioButton[Properties.Gradient.HatchMax];
			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				this.radioHatchRank[i] = new RadioButton(this);
				this.radioHatchRank[i].ActiveStateChanged += new EventHandler(this.HandleHatchRankChanged);
				this.radioHatchRank[i].TabIndex = 10+i;
				this.radioHatchRank[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.radioHatchRank[i].Index = i;
			}
			ToolTip.Default.SetToolTip(this.radioHatchRank[0], Res.Strings.Panel.Gradient.Tooltip.Hatch1);
			ToolTip.Default.SetToolTip(this.radioHatchRank[1], Res.Strings.Panel.Gradient.Tooltip.Hatch2);
			this.RadioSelected = 0;

			this.fieldRepeat = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldRepeat.LabelShortText = Res.Strings.Panel.Gradient.Short.Repeat;
			this.fieldRepeat.LabelLongText  = Res.Strings.Panel.Gradient.Long.Repeat;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldRepeat.TextFieldReal);
			this.fieldRepeat.TextFieldReal.InternalMinValue = 1;
			this.fieldRepeat.TextFieldReal.InternalMaxValue = 8;
			this.fieldRepeat.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldRepeat.TabIndex = 20;
			this.fieldRepeat.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRepeat, Res.Strings.Panel.Gradient.Tooltip.Repeat);

			this.fieldMiddle = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMiddle.LabelShortText = Res.Strings.Panel.Gradient.Short.Middle;
			this.fieldMiddle.LabelLongText  = Res.Strings.Panel.Gradient.Long.Middle;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldMiddle.TextFieldReal);
			this.fieldMiddle.TextFieldReal.InternalMinValue = -500;
			this.fieldMiddle.TextFieldReal.InternalMaxValue =  500;
			this.fieldMiddle.TextFieldReal.Step = 10;
			this.fieldMiddle.TextFieldReal.TextSuffix = "%";
			this.fieldMiddle.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldMiddle.TabIndex = 21;
			this.fieldMiddle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMiddle, Res.Strings.Panel.Gradient.Tooltip.Middle);

			this.fieldSmooth = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldSmooth.LabelShortText = Res.Strings.Panel.Gradient.Short.Smooth;
			this.fieldSmooth.LabelLongText  = Res.Strings.Panel.Gradient.Long.Smooth;
			this.fieldSmooth.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldSmooth.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldSmooth.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldSmooth.TextFieldReal);
			this.fieldSmooth.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldSmooth.TabIndex = 22;
			this.fieldSmooth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSmooth, Res.Strings.Panel.Gradient.Tooltip.Smooth);

			this.fieldHatchAngle = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldHatchAngle.LabelShortText = Res.Strings.Panel.Gradient.Short.HatchAngle;
			this.fieldHatchAngle.LabelLongText  = Res.Strings.Panel.Gradient.Long.HatchAngle;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldHatchAngle.TextFieldReal);
			this.fieldHatchAngle.TextFieldReal.InternalMinValue = -360.0M;
			this.fieldHatchAngle.TextFieldReal.InternalMaxValue =  360.0M;
			this.fieldHatchAngle.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldHatchAngle.TabIndex = 23;
			this.fieldHatchAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchAngle, Res.Strings.Panel.Gradient.Tooltip.HatchAngle);

			this.fieldHatchWidth = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldHatchWidth.LabelShortText = Res.Strings.Panel.Gradient.Short.HatchWidth;
			this.fieldHatchWidth.LabelLongText  = Res.Strings.Panel.Gradient.Long.HatchWidth;
			this.fieldHatchWidth.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldHatchWidth.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldHatchWidth.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldHatchWidth.TextFieldReal);
			this.fieldHatchWidth.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldHatchWidth.TabIndex = 24;
			this.fieldHatchWidth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchWidth, Res.Strings.Panel.Gradient.Tooltip.HatchWidth);

			this.fieldHatchDistance = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldHatchDistance.LabelShortText = Res.Strings.Panel.Gradient.Short.HatchDistance;
			this.fieldHatchDistance.LabelLongText  = Res.Strings.Panel.Gradient.Long.HatchDistance;
			this.fieldHatchDistance.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldHatchDistance.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldHatchDistance.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldHatchDistance.TextFieldReal);
			this.fieldHatchDistance.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
			this.fieldHatchDistance.TabIndex = 25;
			this.fieldHatchDistance.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchDistance, Res.Strings.Panel.Gradient.Tooltip.HatchDistance);
			
			this.swapColor = new IconButton(this);
			this.swapColor.IconName = Misc.Icon("SwapDataV");
			this.swapColor.Clicked += new MessageEventHandler(this.HandleSwapColorClicked);
			ToolTip.Default.SetToolTip(this.swapColor, Res.Strings.Panel.Gradient.Tooltip.Swap);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.GradientFillType type, bool endOfLine)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Gradient.GetIconText(type)), Properties.Gradient.GetName(type), (int)type, endOfLine);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.nothingButton.Clicked -= new MessageEventHandler(this.HandleNothingClicked);
				this.reset.Clicked -= new MessageEventHandler(this.HandleReset);
				this.fieldColor1.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldColor1.Changed -= new EventHandler(this.HandleFieldColorChanged);
				this.fieldColor2.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldColor2.Changed -= new EventHandler(this.HandleFieldColorChanged);
				this.fieldAngle.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleValueChanged);
				this.fieldRepeat.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleValueChanged);
				this.fieldMiddle.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleValueChanged);
				this.fieldSmooth.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleValueChanged);
				this.fieldHatchAngle.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
				this.fieldHatchWidth.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
				this.fieldHatchDistance.TextFieldReal.EditionAccepted += new EventHandler(this.HandleValueChanged);
				this.swapColor.Clicked -= new MessageEventHandler(this.HandleSwapColorClicked);

				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					this.radioHatchRank[i].ActiveStateChanged -= new EventHandler(this.HandleHatchRankChanged);
					this.radioHatchRank[i] = null;
				}

				this.grid = null;
				this.reset = null;
				this.fieldColor1 = null;
				this.fieldColor2 = null;
				this.swapColor = null;
				this.fieldAngle = null;
				this.fieldRepeat = null;
				this.fieldMiddle = null;
				this.fieldSmooth = null;
				this.originFieldColor = null;
			}

			base.Dispose(disposing);
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

						Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;
						if ( type == Properties.GradientFillType.None )
						{
							h += 25;
						}
						else if ( type == Properties.GradientFillType.Circle  ||
								  type == Properties.GradientFillType.Diamond )
						{
							h += 75;
						}
						else
						{
							h += 100;
						}
					}
					else	// �tendu/compact ?
					{
						h += 80;
					}
				}
				else	// panneau r�duit ?
				{
					h += 30;
				}

				return h;
			}
		}


		protected override void PropertyToWidgets()
		{
			//	Propri�t� -> widgets.
			base.PropertyToWidgets();

			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.FillType;

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldAngle.TextFieldReal.InternalValue = (decimal) this.GetAngle();
			this.fieldRepeat.TextFieldReal.InternalValue = (decimal) p.Repeat;
			this.fieldMiddle.TextFieldReal.InternalValue = (decimal) p.Middle*100;
			this.fieldSmooth.TextFieldReal.InternalValue = (decimal) p.Smooth;
			this.HatchToWidget();

			this.cx = p.Cx;
			this.cy = p.Cy;
			this.sx = p.Sx;
			this.sy = p.Sy;

			this.UpdateClientGeometry();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propri�t�.
			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			p.FillType = (Properties.GradientFillType) this.grid.SelectedValue;

			p.Color1 = this.fieldColor1.Color;
			p.Color2 = this.fieldColor2.Color;
			this.SetAngle((double) this.fieldAngle.TextFieldReal.InternalValue);
			p.Repeat = (int)    this.fieldRepeat.TextFieldReal.InternalValue;
			p.Middle = (double) this.fieldMiddle.TextFieldReal.InternalValue/100;
			p.Smooth = (double) this.fieldSmooth.TextFieldReal.InternalValue;
			this.WidgetToHatch();

			p.Cx = this.cx;
			p.Cy = this.cy;
			p.Sx = this.sx;
			p.Sy = this.sy;
		}

		protected void HatchToWidget()
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			int i = this.RadioSelected;
			this.fieldHatchAngle.TextFieldReal.InternalValue = (decimal) p.GetHatchAngle(i);
			this.fieldHatchWidth.TextFieldReal.InternalValue = (decimal) p.GetHatchWidth(i);
			this.fieldHatchDistance.TextFieldReal.InternalValue = (decimal) p.GetHatchDistance(i);
		}

		protected void WidgetToHatch()
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			int i = this.RadioSelected;
			p.SetHatchAngle(i, (double) this.fieldHatchAngle.TextFieldReal.InternalValue);
			p.SetHatchWidth(i, (double) this.fieldHatchWidth.TextFieldReal.InternalValue);
			p.SetHatchDistance(i, (double) this.fieldHatchDistance.TextFieldReal.InternalValue);
		}

		protected int RadioSelected
		{
			get
			{
				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					if ( this.radioHatchRank[i].ActiveState == ActiveState.Yes )
					{
						return i;
					}
				}
				return 0;
			}

			set
			{
				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					this.radioHatchRank[i].ActiveState = (i==value) ? ActiveState.Yes : ActiveState.No;
				}
			}
		}

		protected double GetAngle()
		{
			//	Donne l'angle.
			Properties.Gradient p = this.property as Properties.Gradient;
			Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;

			if ( type == Properties.GradientFillType.Linear )  // lin�aire ?
			{
				return Point.ComputeAngleDeg(p.Sx, p.Sy)-90;
			}
			else
			{
				return p.Angle;
			}
		}

		protected void SetAngle(double angle)
		{
			//	Change l'angle.
			Properties.Gradient p = this.property as Properties.Gradient;
			Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;

			if ( type == Properties.GradientFillType.Linear )  // lin�aire ?
			{
				double d = System.Math.Sqrt(this.sx*this.sx + this.sy*this.sy);
				Point s = Transform.RotatePointDeg(angle, new Point(0,d));
				this.sx = s.X;
				this.sy = s.Y;
			}
			else
			{
				p.Angle = angle;
			}
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets n�cessaires.
			bool color2 = false;
			bool showReset = false;
			bool enableReset = false;
			bool showAngle = false;
			bool enableAngle = false;
			bool showRepeat = false;
			bool enableRepeat = false;
			bool showHatch = false;
			bool showSmooth = false;
			bool enableSmooth = false;

			Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;

			if ( type != Properties.GradientFillType.None )
			{
				color2 = true;
			}

			if ( this.isExtendedSize )  // panneau �tendu ?
			{
				if ( this.IsLabelProperties )  // �tendu/d�tails ?
				{
					if ( type == Properties.GradientFillType.None )
					{
						showSmooth = true;
						enableSmooth = true;
					}
					else if ( type == Properties.GradientFillType.Linear ||
							  type == Properties.GradientFillType.Conic  )
					{
						showReset = true;
						enableReset = true;
						showAngle = true;
						enableAngle = true;
						showRepeat = true;
						enableRepeat = true;
						showSmooth = true;
						enableSmooth = true;
					}
					else if ( type == Properties.GradientFillType.Circle  ||
							  type == Properties.GradientFillType.Diamond )
					{
						showReset = true;
						enableReset = true;
						showRepeat = true;
						enableRepeat = true;
						showSmooth = true;
						enableSmooth = true;
					}
					else
					{
						showReset = true;
						enableReset = true;
						showHatch = true;
					}
				}
				else	// �tendu/compact ?
				{
					if ( type == Properties.GradientFillType.None )
					{
						showReset = true;
						showAngle = true;
						showRepeat = true;
						showSmooth = true;
						enableSmooth = true;
					}
					else if ( type == Properties.GradientFillType.Linear ||
							  type == Properties.GradientFillType.Conic  )
					{
						showReset = true;
						enableReset = true;
						showAngle = true;
						enableAngle = true;
						showRepeat = true;
						enableRepeat = true;
						showSmooth = true;
						enableSmooth = true;
					}
					else if ( type == Properties.GradientFillType.Circle  ||
							  type == Properties.GradientFillType.Diamond )
					{
						showReset = true;
						enableReset = true;
						showAngle = true;
						showRepeat = true;
						enableRepeat = true;
						showSmooth = true;
						enableSmooth = true;
					}
					else
					{
						showReset = true;
						enableReset = true;
						showHatch = true;
					}
				}
			}

			this.fieldColor2.Visibility = (color2);
			this.swapColor.Visibility = (color2);

			this.reset.Visibility = (showReset);
			this.reset.Enable = (enableReset);

			this.fieldAngle.Visibility = (showAngle);
			this.fieldAngle.Enable = (enableAngle);

			this.fieldRepeat.Visibility = (showRepeat);
			this.fieldRepeat.Enable = (enableRepeat);

			this.fieldMiddle.Visibility = (showRepeat);
			this.fieldMiddle.Enable = (enableRepeat);

			this.fieldSmooth.Visibility = (showSmooth);
			this.fieldSmooth.Enable = (enableSmooth);

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				this.radioHatchRank[i].Visibility = (showHatch);
			}

			this.fieldHatchAngle.Visibility = (showHatch);
			this.fieldHatchWidth.Visibility = (showHatch);
			this.fieldHatchDistance.Visibility = (showHatch);
		}


		public override void OriginColorDeselect()
		{
			//	D�s�lectionne toutes les origines de couleurs possibles.
			this.fieldColor1.ActiveState = ActiveState.No;
			this.fieldColor2.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	S�lectionne l'origine de couleur.
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.fieldColor1;
				else              this.originFieldColor = this.fieldColor2;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = ActiveState.Yes;
		}

		public override int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return this.originFieldRank;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.OnChanged();
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		
		protected void UpdateShortLongText()
		{
			//	Adapte les textes courts ou longs.
			if ( this.IsLabelProperties )
			{
				Abstract.SetText(this.radioHatchRank[0], Res.Strings.Panel.Gradient.Long.Hatch1);
				Abstract.SetText(this.radioHatchRank[1], Res.Strings.Panel.Gradient.Long.Hatch2);
			}
			else
			{
				Abstract.SetText(this.radioHatchRank[0], "");
				Abstract.SetText(this.radioHatchRank[1], ":");
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.fieldColor1 == null )  return;

			this.UpdateShortLongText();
			this.EnableWidgets();
			Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;

			Rectangle rect = this.UsefulZone;
			Rectangle r = rect;
			double pTop = rect.Top;

			r.Top = pTop;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-20;
			r.Width = 20;
			this.nothingButton.SetManualBounds(r);

			if ( type == Properties.GradientFillType.None )
			{
				r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
				r.Right = rect.Right;
				this.fieldColor1.SetManualBounds(r);
			}
			else
			{
				r.Left = rect.Right-20-8-20;
				r.Right = rect.Right-20-8;
				this.fieldColor1.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				this.fieldColor2.SetManualBounds(r);

				r.Left = rect.Right-20-8;
				r.Right = rect.Right-20;
				this.swapColor.SetManualBounds(r);
			}

			if ( this.isExtendedSize )  // panneau �tendu ?
			{
				r.Top = pTop;
				r.Bottom = r.Top-22*2;
				r.Left = rect.Left;
				r.Width = 22*4;
				r.Inflate(1);
				this.grid.SetManualBounds(r);
				pTop -= 25;

				if ( this.IsLabelProperties )  // �tendu/d�tails ?
				{
					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.reset.SetManualBounds(r);
					pTop -= 25;

					if ( type == Properties.GradientFillType.Linear ||
						 type == Properties.GradientFillType.Conic  )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldAngle.SetManualBounds(r);
						pTop -= 25;
					}

					if ( type == Properties.GradientFillType.Linear  ||
						 type == Properties.GradientFillType.Circle  ||
						 type == Properties.GradientFillType.Diamond ||
						 type == Properties.GradientFillType.Conic   )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldRepeat.SetManualBounds(r);
						pTop -= 25;

						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldMiddle.SetManualBounds(r);
						pTop -= 25;
					}

					if ( type == Properties.GradientFillType.None    ||
						 type == Properties.GradientFillType.Linear  ||
						 type == Properties.GradientFillType.Circle  ||
						 type == Properties.GradientFillType.Diamond ||
						 type == Properties.GradientFillType.Conic   )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldSmooth.SetManualBounds(r);
						pTop -= 25;
					}

					if ( type == Properties.GradientFillType.Hatch   ||
						 type == Properties.GradientFillType.Dots    ||
						 type == Properties.GradientFillType.Squares )
					{
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Width = 80;
						for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
						{
							this.radioHatchRank[i].SetManualBounds(r);
							r.Offset(r.Width, 0);
						}
						pTop -= 25;

						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldRepeat.SetManualBounds(r);
						this.fieldHatchAngle.SetManualBounds(r);
						pTop -= 25;

						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldMiddle.SetManualBounds(r);
						this.fieldHatchWidth.SetManualBounds(r);
						pTop -= 25;
					
						r.Top = pTop;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldSmooth.SetManualBounds(r);
						this.fieldHatchDistance.SetManualBounds(r);
						pTop -= 25;
					}
				}
				else	// �tendu/compact ?
				{
					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth-20;
					r.Width = 20;
					this.reset.SetManualBounds(r);

					r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldAngle.SetManualBounds(r);

					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Width = 16;
					for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
					{
						if ( i == Properties.Gradient.HatchMax-1 )  r.Width = 32;
						this.radioHatchRank[i].SetManualBounds(r);
						r.Offset(r.Width, 0);
					}
					pTop -= 25;

					r.Top = pTop;
					r.Bottom = r.Top-20;
					r.Left = rect.Left;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldRepeat.SetManualBounds(r);
					this.fieldHatchAngle.SetManualBounds(r);
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldMiddle.SetManualBounds(r);
					this.fieldHatchWidth.SetManualBounds(r);
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldSmooth.SetManualBounds(r);
					this.fieldHatchDistance.SetManualBounds(r);
				}
			}
			else	// panneau r�duit ?
			{
				r.Top = pTop;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 22*4;
				r.Inflate(1);
				this.grid.SetManualBounds(r);
			}
		}
		

		private void HandleReset(object sender, MessageEventArgs e)
		{
			Properties.GradientFillType type = (Properties.GradientFillType) this.grid.SelectedValue;

			if ( type == Properties.GradientFillType.Linear )  // lin�aire ?
			{
				this.fieldAngle.TextFieldReal.Value = 0.0M;
				this.cx = 0.5;
				this.cy = 0.5;
				this.sx = 0.0;
				this.sy = 0.5;
			}
			else
			{
				this.fieldAngle.TextFieldReal.Value = 0.0M;
				this.cx = 0.5;
				this.cy = 0.5;
				this.sx = 0.5;
				this.sy = 0.5;
			}

			this.OnChanged();
		}

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.originFieldColor = sender as ColorSample;

			this.originFieldRank = -1;
			if ( this.originFieldColor == this.fieldColor1 )  this.originFieldRank = 0;
			if ( this.originFieldColor == this.fieldColor2 )  this.originFieldRank = 1;

			this.OnOriginColorChanged();
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();
			}

			this.OnChanged();
		}

		private void HandleSwapColorClicked(object sender, MessageEventArgs e)
		{
			Drawing.RichColor temp = this.fieldColor1.Color;
			this.fieldColor1.Color = this.fieldColor2.Color;
			this.fieldColor2.Color = temp;

			if ( this.originFieldRank != -1 )
			{
				if ( this.originFieldRank == 0 )
				{
					this.originFieldRank = 1;
					this.originFieldColor = this.fieldColor2;
				}
				else
				{
					this.originFieldRank = 0;
					this.originFieldColor = this.fieldColor1;
				}
				this.OnOriginColorChanged();
			}

			this.OnChanged();
		}

		private void HandleNothingClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton "aucune couleur" a �t� cliqu�.
			this.grid.SelectedValue = (int) Properties.GradientFillType.None;
			this.fieldColor1.Color = Drawing.RichColor.FromAlphaRgb(0, 1,1,1);
			this.OnChanged();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Le type a �t� chang�.
			if ( this.ignoreChanged )  return;

			this.HandleReset(null, null);

			if ( this.IsLabelProperties )
			{
				this.HeightChanged();
			}

			this.UpdateClientGeometry();
			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleHatchRankChanged(object sender)
		{
			//	Le rang a �t� chang�.
			if ( this.ignoreChanged )  return;
			this.ignoreChanged = true;
			this.HatchToWidget();
			this.ignoreChanged = false;
		}


		protected RadioIconGrid				grid;
		protected IconButton				nothingButton;
		protected Button					reset;
		protected ColorSample				fieldColor1;
		protected ColorSample				fieldColor2;
		protected IconButton				swapColor;
		protected Widgets.TextFieldLabel	fieldAngle;
		protected Widgets.TextFieldLabel	fieldRepeat;
		protected Widgets.TextFieldLabel	fieldMiddle;
		protected Widgets.TextFieldLabel	fieldSmooth;
		protected Widgets.TextFieldLabel	fieldHatchAngle;
		protected Widgets.TextFieldLabel	fieldHatchWidth;
		protected Widgets.TextFieldLabel	fieldHatchDistance;
		protected RadioButton[]				radioHatchRank;
		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
		protected double					cx, cy;
		protected double					sx, sy;
	}
}

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Gradient permet de choisir un dégradé de couleurs.
	/// </summary>
	[SuppressBundleSupport]
	public class Gradient : Abstract
	{
		public Gradient(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.nothingButton = new IconButton(this);
			this.nothingButton.Clicked += new MessageEventHandler(this.HandleNothingClicked);
			this.nothingButton.TabIndex = 1;
			this.nothingButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.nothingButton.IconName = "manifest:Epsitec.App.DocumentEditor.Images.Nothing.icon";
			ToolTip.Default.SetToolTip(this.nothingButton, Res.Strings.Panel.Gradient.Tooltip.Nothing);

			this.fieldColor1 = new ColorSample(this);
			this.fieldColor1.PossibleSource = true;
			this.fieldColor1.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor1.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fieldColor1.TabIndex = 2;
			this.fieldColor1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor1, Res.Strings.Panel.Gradient.Tooltip.Color1);

			this.fieldColor2 = new ColorSample(this);
			this.fieldColor2.PossibleSource = true;
			this.fieldColor2.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor2.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fieldColor2.TabIndex = 3;
			this.fieldColor2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor2, Res.Strings.Panel.Gradient.Tooltip.Color2);

			this.listFill = new TextFieldCombo(this);
			this.listFill.IsReadOnly = true;
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.None);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Linear);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Circle);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Diamond);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Conic);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Hatch);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Dots);
			this.listFill.Items.Add(Res.Strings.Panel.Gradient.Combo.Squares);
			this.listFill.SelectedIndexChanged += new EventHandler(this.HandleListChanged);
			this.listFill.TabIndex = 4;
			this.listFill.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.listFill, Res.Strings.Panel.Gradient.Tooltip.Type);

			this.reset = new Button(this);
			this.reset.Text = Res.Strings.Panel.Gradient.Button.Reset;
			this.reset.TabIndex = 5;
			this.reset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += new MessageEventHandler(this.HandleReset);
			ToolTip.Default.SetToolTip(this.reset, Res.Strings.Panel.Gradient.Tooltip.Reset);

			this.fieldAngle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldAngle);
			this.fieldAngle.InternalMinValue = -360.0M;
			this.fieldAngle.InternalMaxValue =  360.0M;
			this.fieldAngle.ValueChanged += new EventHandler(this.HandleValueChanged);
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
			}
			this.radioHatchRank[Properties.Gradient.HatchMax-1].Text = ":";
			ToolTip.Default.SetToolTip(this.radioHatchRank[0], Res.Strings.Panel.Gradient.Tooltip.Hatch1);
			ToolTip.Default.SetToolTip(this.radioHatchRank[1], Res.Strings.Panel.Gradient.Tooltip.Hatch2);
			this.RadioSelected = 0;

			this.fieldRepeat = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldRepeat);
			this.fieldRepeat.InternalMinValue = 1;
			this.fieldRepeat.InternalMaxValue = 8;
			this.fieldRepeat.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldRepeat.TabIndex = 20;
			this.fieldRepeat.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRepeat, Res.Strings.Panel.Gradient.Tooltip.Repeat);

			this.fieldMiddle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldMiddle);
			this.fieldMiddle.InternalMinValue = -500;
			this.fieldMiddle.InternalMaxValue =  500;
			this.fieldMiddle.Step = 10;
			this.fieldMiddle.TextSuffix = "%";
			this.fieldMiddle.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldMiddle.TabIndex = 21;
			this.fieldMiddle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMiddle, Res.Strings.Panel.Gradient.Tooltip.Middle);

			this.fieldSmooth = new TextFieldReal(this);
			this.fieldSmooth.FactorMinRange = 0.0M;
			this.fieldSmooth.FactorMaxRange = 0.1M;
			this.fieldSmooth.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldSmooth);
			this.fieldSmooth.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldSmooth.TabIndex = 22;
			this.fieldSmooth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSmooth, Res.Strings.Panel.Gradient.Tooltip.Smooth);

			this.fieldHatchAngle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldHatchAngle);
			this.fieldHatchAngle.InternalMinValue = -360.0M;
			this.fieldHatchAngle.InternalMaxValue =  360.0M;
			this.fieldHatchAngle.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldHatchAngle.TabIndex = 23;
			this.fieldHatchAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchAngle, Res.Strings.Panel.Gradient.Tooltip.HatchAngle);

			this.fieldHatchWidth = new TextFieldReal(this);
			this.fieldHatchWidth.FactorMinRange = 0.0M;
			this.fieldHatchWidth.FactorMaxRange = 0.1M;
			this.fieldHatchWidth.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldHatchWidth);
			this.fieldHatchWidth.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldHatchWidth.TabIndex = 24;
			this.fieldHatchWidth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchWidth, Res.Strings.Panel.Gradient.Tooltip.HatchWidth);

			this.fieldHatchDistance = new TextFieldReal(this);
			this.fieldHatchDistance.FactorMinRange = 0.0M;
			this.fieldHatchDistance.FactorMaxRange = 0.1M;
			this.fieldHatchDistance.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldHatchDistance);
			this.fieldHatchDistance.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.fieldHatchDistance.TabIndex = 25;
			this.fieldHatchDistance.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHatchDistance, Res.Strings.Panel.Gradient.Tooltip.HatchDistance);
			
			this.labelAngle = new StaticText(this);
			this.labelAngle.Text = Res.Strings.Panel.Gradient.Label.Angle;
			this.labelAngle.Alignment = ContentAlignment.MiddleCenter;

			this.labelRepeat = new StaticText(this);
			this.labelRepeat.Text = Res.Strings.Panel.Gradient.Label.Repeat;
			this.labelRepeat.Alignment = ContentAlignment.MiddleCenter;

			this.labelMiddle = new StaticText(this);
			this.labelMiddle.Text = Res.Strings.Panel.Gradient.Label.Middle;
			this.labelMiddle.Alignment = ContentAlignment.MiddleCenter;

			this.labelSmooth = new StaticText(this);
			this.labelSmooth.Text = Res.Strings.Panel.Gradient.Label.Smooth;
			this.labelSmooth.Alignment = ContentAlignment.MiddleCenter;

			this.labelHatchAngle = new StaticText(this);
			this.labelHatchAngle.Text = Res.Strings.Panel.Gradient.Label.HatchAngle;
			this.labelHatchAngle.Alignment = ContentAlignment.MiddleCenter;

			this.labelHatchWidth = new StaticText(this);
			this.labelHatchWidth.Text = Res.Strings.Panel.Gradient.Label.HatchWidth;
			this.labelHatchWidth.Alignment = ContentAlignment.MiddleCenter;

			this.labelHatchDistance = new StaticText(this);
			this.labelHatchDistance.Text = Res.Strings.Panel.Gradient.Label.HatchDistance;
			this.labelHatchDistance.Alignment = ContentAlignment.MiddleCenter;

			this.swapColor = new IconButton(this);
			this.swapColor.IconName = "manifest:Epsitec.App.DocumentEditor.Images.SwapDataV.icon";
			this.swapColor.Clicked += new MessageEventHandler(this.HandleSwapColorClicked);
			ToolTip.Default.SetToolTip(this.swapColor, Res.Strings.Panel.Gradient.Tooltip.Swap);

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.nothingButton.Clicked -= new MessageEventHandler(this.HandleNothingClicked);
				this.listFill.SelectedIndexChanged -= new EventHandler(this.HandleListChanged);
				this.reset.Clicked -= new MessageEventHandler(this.HandleReset);
				this.fieldColor1.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldColor1.Changed -= new EventHandler(this.HandleFieldColorChanged);
				this.fieldColor2.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldColor2.Changed -= new EventHandler(this.HandleFieldColorChanged);
				this.fieldAngle.ValueChanged -= new EventHandler(this.HandleValueChanged);
				this.fieldRepeat.ValueChanged -= new EventHandler(this.HandleValueChanged);
				this.fieldMiddle.ValueChanged -= new EventHandler(this.HandleValueChanged);
				this.fieldSmooth.ValueChanged -= new EventHandler(this.HandleValueChanged);
				this.swapColor.Clicked -= new MessageEventHandler(this.HandleSwapColorClicked);

				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					this.radioHatchRank[i].ActiveStateChanged -= new EventHandler(this.HandleHatchRankChanged);
					this.radioHatchRank[i] = null;
				}

				this.label = null;
				this.listFill = null;
				this.reset = null;
				this.fieldColor1 = null;
				this.fieldColor2 = null;
				this.swapColor = null;
				this.fieldAngle = null;
				this.fieldRepeat = null;
				this.fieldMiddle = null;
				this.fieldSmooth = null;
				this.labelAngle = null;
				this.labelRepeat = null;
				this.labelMiddle = null;
				this.labelSmooth = null;
				this.originFieldColor = null;
			}

			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 83 : 30 );
			}
		}


		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			int sel = -1;
			switch ( p.FillType )
			{
				case Properties.GradientFillType.None:     sel = 0;  break;
				case Properties.GradientFillType.Linear:   sel = 1;  break;
				case Properties.GradientFillType.Circle:   sel = 2;  break;
				case Properties.GradientFillType.Diamond:  sel = 3;  break;
				case Properties.GradientFillType.Conic:    sel = 4;  break;
				case Properties.GradientFillType.Hatch:    sel = 5;  break;
				case Properties.GradientFillType.Dots:     sel = 6;  break;
				case Properties.GradientFillType.Squares:  sel = 7;  break;
			}
			this.listFill.SelectedIndex = sel;

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldAngle.InternalValue = (decimal) this.GetAngle();
			this.fieldRepeat.InternalValue = (decimal) p.Repeat;
			this.fieldMiddle.InternalValue = (decimal) p.Middle*100;
			this.fieldSmooth.InternalValue = (decimal) p.Smooth;
			this.HatchToWidget();

			this.cx = p.Cx;
			this.cy = p.Cy;
			this.sx = p.Sx;
			this.sy = p.Sy;

			this.UpdateClientGeometry();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			p.FillType = Properties.GradientFillType.None;
			switch ( this.listFill.SelectedIndex )
			{
				case 0:  p.FillType = Properties.GradientFillType.None;     break;
				case 1:  p.FillType = Properties.GradientFillType.Linear;   break;
				case 2:  p.FillType = Properties.GradientFillType.Circle;   break;
				case 3:  p.FillType = Properties.GradientFillType.Diamond;  break;
				case 4:  p.FillType = Properties.GradientFillType.Conic;    break;
				case 5:  p.FillType = Properties.GradientFillType.Hatch;    break;
				case 6:  p.FillType = Properties.GradientFillType.Dots;     break;
				case 7:  p.FillType = Properties.GradientFillType.Squares;  break;
			}

			p.Color1 = this.fieldColor1.Color;
			p.Color2 = this.fieldColor2.Color;
			this.SetAngle((double) this.fieldAngle.InternalValue);
			p.Repeat = (int)    this.fieldRepeat.InternalValue;
			p.Middle = (double) this.fieldMiddle.InternalValue/100;
			p.Smooth = (double) this.fieldSmooth.InternalValue;
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
			this.fieldHatchAngle.InternalValue = (decimal) p.GetHatchAngle(i);
			this.fieldHatchWidth.InternalValue = (decimal) p.GetHatchWidth(i);
			this.fieldHatchDistance.InternalValue = (decimal) p.GetHatchDistance(i);
		}

		protected void WidgetToHatch()
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			if ( p == null )  return;

			int i = this.RadioSelected;
			p.SetHatchAngle(i, (double) this.fieldHatchAngle.InternalValue);
			p.SetHatchWidth(i, (double) this.fieldHatchWidth.InternalValue);
			p.SetHatchDistance(i, (double) this.fieldHatchDistance.InternalValue);
		}

		protected int RadioSelected
		{
			get
			{
				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					if ( this.radioHatchRank[i].ActiveState == WidgetState.ActiveYes )
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
					this.radioHatchRank[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}

		// Donne l'angle.
		protected double GetAngle()
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			int sel = this.listFill.SelectedIndex;

			if ( sel == 1 )  // linéaire ?
			{
				return Point.ComputeAngleDeg(p.Sx, p.Sy)-90;
			}
			else
			{
				return p.Angle;
			}
		}

		// Change l'angle.
		protected void SetAngle(double angle)
		{
			Properties.Gradient p = this.property as Properties.Gradient;
			int sel = this.listFill.SelectedIndex;

			if ( sel == 1 )  // linéaire ?
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

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			int sel = this.listFill.SelectedIndex;
			bool hatch = (sel == 5 || sel == 6 || sel == 7);  // hachures, points ou carrés ?

			this.label.SetVisible(true);
			this.nothingButton.SetVisible(true);
			this.listFill.SetVisible(this.isExtendedSize);
			this.reset.SetVisible(this.isExtendedSize);
			this.fieldColor1.SetVisible(true);
			this.fieldColor2.SetVisible(sel > 0);
			this.fieldAngle.SetVisible(this.isExtendedSize && !hatch);
			this.fieldRepeat.SetVisible(this.isExtendedSize && !hatch);
			this.fieldMiddle.SetVisible(this.isExtendedSize && !hatch);
			this.fieldSmooth.SetVisible(this.isExtendedSize && !hatch);
			this.fieldHatchAngle.SetVisible(this.isExtendedSize && hatch);
			this.fieldHatchWidth.SetVisible(this.isExtendedSize && hatch);
			this.fieldHatchDistance.SetVisible(this.isExtendedSize && hatch);
			this.labelAngle.SetVisible(this.isExtendedSize && !hatch);
			this.labelRepeat.SetVisible(this.isExtendedSize && !hatch);
			this.labelMiddle.SetVisible(this.isExtendedSize && !hatch);
			this.labelSmooth.SetVisible(this.isExtendedSize && !hatch);
			this.labelHatchAngle.SetVisible(this.isExtendedSize && hatch);
			this.labelHatchWidth.SetVisible(this.isExtendedSize && hatch);
			this.labelHatchDistance.SetVisible(this.isExtendedSize && hatch);
			this.swapColor.SetVisible(sel > 0);

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				this.radioHatchRank[i].SetVisible(this.isExtendedSize && hatch);
				this.radioHatchRank[i].SetEnabled(hatch);
			}

			if ( sel > 0 )
			{
				this.reset.SetEnabled(this.isExtendedSize);
				this.fieldRepeat.SetEnabled(this.isExtendedSize);
				this.fieldMiddle.SetEnabled(this.isExtendedSize);
				this.labelRepeat.SetEnabled(this.isExtendedSize);
				this.labelMiddle.SetEnabled(this.isExtendedSize);
			}
			else
			{
				this.reset.SetEnabled(false);
				this.fieldRepeat.SetEnabled(false);
				this.fieldMiddle.SetEnabled(false);
				this.labelRepeat.SetEnabled(false);
				this.labelMiddle.SetEnabled(false);
			}

			if ( sel == 1 || sel == 4 )  // linéaire ou cônique ?
			{
				this.fieldAngle.SetEnabled(this.isExtendedSize);
				this.labelAngle.SetEnabled(this.isExtendedSize);
			}
			else
			{
				this.fieldAngle.SetEnabled(false);
				this.labelAngle.SetEnabled(false);
			}

			if ( hatch )  // hachures ?
			{
				this.fieldHatchAngle.SetEnabled(this.isExtendedSize);
				this.fieldHatchWidth.SetEnabled(this.isExtendedSize);
				this.fieldHatchDistance.SetEnabled(this.isExtendedSize);
			}
			else
			{
				this.fieldHatchAngle.SetEnabled(false);
				this.fieldHatchWidth.SetEnabled(false);
				this.fieldHatchDistance.SetEnabled(false);
			}

			this.fieldSmooth.SetEnabled(this.isExtendedSize);
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fieldColor1.ActiveState = WidgetState.ActiveNo;
			this.fieldColor2.ActiveState = WidgetState.ActiveNo;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.fieldColor1;
				else              this.originFieldColor = this.fieldColor2;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = WidgetState.ActiveYes;
		}

		// Retourne le rang de la couleur d'origine.
		public override int OriginColorRank()
		{
			return this.originFieldRank;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.Color color)
		{
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.OnChanged();
			}
		}

		// Donne la couleur d'origine.
		public override Drawing.Color OriginColorGet()
		{
			if ( this.originFieldColor == null )  return Drawing.Color.FromBrightness(0);
			return this.originFieldColor.Color;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldColor1 == null )  return;

			this.EnableWidgets();
			int sel = this.listFill.SelectedIndex;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			rect.Bottom = rect.Top-20;
			Rectangle r = rect;

			r.Left = rect.Left;
			r.Right = rect.Right-70;
			this.label.Bounds = r;
			r.Left = rect.Right-70;
			r.Right = rect.Right-50;
			this.nothingButton.Bounds = r;

			if ( sel == 0 )
			{
				r.Left = rect.Right-50;
				r.Right = rect.Right;
				this.fieldColor1.Bounds = r;
			}
			else
			{
				r.Left = rect.Right-50;
				r.Right = rect.Right-30;
				this.fieldColor1.Bounds = r;
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				this.fieldColor2.Bounds = r;

				r.Left = rect.Right-30;
				r.Right = rect.Right-20;
				this.swapColor.Bounds = r;
			}

			if ( this.isExtendedSize )
			{
				rect.Offset(0, -25);
				r = rect;

				r.Left = rect.Left;
				r.Right = rect.Left+85;
				this.listFill.Bounds = r;
				r.Left = rect.Left+90;
				r.Right = rect.Left+114;
				this.reset.Bounds = r;
				r.Left = rect.Right-62;
				r.Width = 12;
				this.labelAngle.Bounds = r;
				r.Left = rect.Right-50;
				r.Width = 50;
				this.fieldAngle.Bounds = r;

				r.Width = 16;
				for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
				{
					if ( i == Properties.Gradient.HatchMax-1 )  r.Width = 32;
					this.radioHatchRank[i].Bounds = r;
					r.Offset(r.Width, 0);
				}

				rect.Offset(0, -25);
				r = rect;

				r.Left = rect.Left;
				r.Width = 12;
				this.labelRepeat.Bounds = r;
				this.labelHatchAngle.Bounds = r;
				r.Left = r.Right;
				r.Width = 44;
				this.fieldRepeat.Bounds = r;
				this.fieldHatchAngle.Bounds = r;
				r.Left = r.Right;
				r.Width = 12;
				this.labelMiddle.Bounds = r;
				this.labelHatchWidth.Bounds = r;
				r.Left = r.Right;
				r.Width = 45;
				this.fieldMiddle.Bounds = r;
				this.fieldHatchWidth.Bounds = r;
				r.Left = r.Right;
				r.Width = 12;
				this.labelSmooth.Bounds = r;
				this.labelHatchDistance.Bounds = r;
				r.Left = r.Right;
				r.Width = 50;
				this.fieldSmooth.Bounds = r;
				this.fieldHatchDistance.Bounds = r;
			}
		}
		

		private void HandleReset(object sender, MessageEventArgs e)
		{
			int sel = this.listFill.SelectedIndex;

			if ( sel == 1 )  // linéaire ?
			{
				this.fieldAngle.Value = 0.0M;
				this.cx = 0.5;
				this.cy = 0.5;
				this.sx = 0.0;
				this.sy = 0.5;
			}
			else
			{
				this.fieldAngle.Value = 0.0M;
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
			if ( cs.ActiveState == WidgetState.ActiveYes )
			{
				this.OnOriginColorChanged();
			}

			this.OnChanged();
		}

		private void HandleSwapColorClicked(object sender, MessageEventArgs e)
		{
			Drawing.Color temp     = this.fieldColor1.Color;
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

		// Le bouton "aucune couleur" a été cliqué.
		private void HandleNothingClicked(object sender, MessageEventArgs e)
		{
			this.listFill.SelectedIndex = 0;
			this.fieldColor1.Color = Drawing.Color.FromARGB(0, 1,1,1);
			this.OnChanged();
		}

		private void HandleListChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.UpdateClientGeometry();
			this.HandleReset(null, null);
		}

		private void HandleValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le rang a été changé.
		private void HandleHatchRankChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.ignoreChanged = true;
			this.HatchToWidget();
			this.ignoreChanged = false;
		}


		protected StaticText				label;
		protected IconButton				nothingButton;
		protected TextFieldCombo			listFill;
		protected Button					reset;
		protected ColorSample				fieldColor1;
		protected ColorSample				fieldColor2;
		protected IconButton				swapColor;
		protected TextFieldReal				fieldAngle;
		protected TextFieldReal				fieldRepeat;
		protected TextFieldReal				fieldMiddle;
		protected TextFieldReal				fieldSmooth;
		protected TextFieldReal				fieldHatchAngle;
		protected TextFieldReal				fieldHatchWidth;
		protected TextFieldReal				fieldHatchDistance;
		protected RadioButton[]				radioHatchRank;
		protected StaticText				labelAngle;
		protected StaticText				labelRepeat;
		protected StaticText				labelMiddle;
		protected StaticText				labelSmooth;
		protected StaticText				labelHatchAngle;
		protected StaticText				labelHatchWidth;
		protected StaticText				labelHatchDistance;
		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
		protected double					cx, cy;
		protected double					sx, sy;
	}
}

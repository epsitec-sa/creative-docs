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
			ToolTip.Default.SetToolTip(this.nothingButton, "Aucune couleur");

			this.fieldColor1 = new ColorSample(this);
			this.fieldColor1.PossibleOrigin = true;
			this.fieldColor1.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor1.TabIndex = 2;
			this.fieldColor1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor1, "Couleur 1");

			this.fieldColor2 = new ColorSample(this);
			this.fieldColor2.PossibleOrigin = true;
			this.fieldColor2.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor2.TabIndex = 3;
			this.fieldColor2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor2, "Couleur 2");

			this.listFill = new TextFieldCombo(this);
			this.listFill.IsReadOnly = true;
			this.listFill.Items.Add("Uniforme");
			this.listFill.Items.Add("Linéaire");
			this.listFill.Items.Add("Circulaire");
			this.listFill.Items.Add("Diamant");
			this.listFill.Items.Add("Cônique");
			this.listFill.SelectedIndexChanged += new EventHandler(this.HandleListChanged);
			this.listFill.TabIndex = 4;
			this.listFill.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.reset = new Button(this);
			this.reset.Text = "R";
			this.reset.TabIndex = 5;
			this.reset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += new MessageEventHandler(this.HandleReset);
			ToolTip.Default.SetToolTip(this.reset, "Reset (valeurs standards)");

			this.fieldAngle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldAngle);
			this.fieldAngle.InternalMinValue = -360.0M;
			this.fieldAngle.InternalMaxValue =  360.0M;
			this.fieldAngle.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldAngle.TabIndex = 6;
			this.fieldAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldAngle, "Angle");

			this.fieldRepeat = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldRepeat);
			this.fieldRepeat.InternalMinValue = 1;
			this.fieldRepeat.InternalMaxValue = 8;
			this.fieldRepeat.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldRepeat.TabIndex = 7;
			this.fieldRepeat.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRepeat, "Nombre de répétitions");

			this.fieldMiddle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldMiddle);
			this.fieldMiddle.InternalMinValue = -500;
			this.fieldMiddle.InternalMaxValue =  500;
			this.fieldMiddle.Step = 10;
			this.fieldMiddle.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldMiddle.TabIndex = 8;
			this.fieldMiddle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMiddle, "Couleur médiane");

			this.fieldSmooth = new TextFieldReal(this);
			this.fieldSmooth.FactorMinRange = 0.0M;
			this.fieldSmooth.FactorMaxRange = 0.1M;
			this.fieldSmooth.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldSmooth);
			this.fieldSmooth.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldSmooth.TabIndex = 9;
			this.fieldSmooth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSmooth, "Flou");

			this.labelAngle = new StaticText(this);
			this.labelAngle.Text = "a";
			this.labelAngle.Alignment = ContentAlignment.MiddleCenter;

			this.labelRepeat = new StaticText(this);
			this.labelRepeat.Text = "n";
			this.labelRepeat.Alignment = ContentAlignment.MiddleCenter;

			this.labelMiddle = new StaticText(this);
			this.labelMiddle.Text = "M";
			this.labelMiddle.Alignment = ContentAlignment.MiddleCenter;

			this.labelSmooth = new StaticText(this);
			this.labelSmooth.Text = "F";
			this.labelSmooth.Alignment = ContentAlignment.MiddleCenter;

			this.swapColor = new IconButton(this);
			this.swapColor.IconName = "manifest:Epsitec.App.DocumentEditor.Images.SwapDataV.icon";
			this.swapColor.Clicked += new MessageEventHandler(this.HandleSwapColorClicked);
			ToolTip.Default.SetToolTip(this.swapColor, "Permute les 2 couleurs");

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
				this.fieldColor2.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldAngle.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldRepeat.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldMiddle.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldSmooth.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.swapColor.Clicked -= new MessageEventHandler(this.HandleSwapColorClicked);

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
			}
			this.listFill.SelectedIndex = sel;

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldAngle.InternalValue = (decimal) p.Angle;
			this.fieldRepeat.InternalValue = (decimal) p.Repeat;
			this.fieldMiddle.InternalValue = (decimal) p.Middle*100;
			this.fieldSmooth.InternalValue = (decimal) p.Smooth;

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
			}

			p.Color1 = this.fieldColor1.Color;
			p.Color2 = this.fieldColor2.Color;
			p.Angle  = (double) this.fieldAngle.InternalValue;
			p.Repeat = (int)    this.fieldRepeat.InternalValue;
			p.Middle = (double) this.fieldMiddle.InternalValue/100;
			p.Smooth = (double) this.fieldSmooth.InternalValue;

			p.Cx = this.cx;
			p.Cy = this.cy;
			p.Sx = this.sx;
			p.Sy = this.sy;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			int sel = this.listFill.SelectedIndex;

			this.label.SetVisible(true);
			this.nothingButton.SetVisible(true);
			this.listFill.SetVisible(this.isExtendedSize);
			this.reset.SetVisible(this.isExtendedSize);
			this.fieldColor1.SetVisible(true);
			this.fieldColor2.SetVisible(sel > 0);
			this.fieldAngle.SetVisible(this.isExtendedSize);
			this.fieldRepeat.SetVisible(this.isExtendedSize);
			this.fieldMiddle.SetVisible(this.isExtendedSize);
			this.fieldSmooth.SetVisible(this.isExtendedSize);
			this.labelAngle.SetVisible(this.isExtendedSize);
			this.labelRepeat.SetVisible(this.isExtendedSize);
			this.labelMiddle.SetVisible(this.isExtendedSize);
			this.labelSmooth.SetVisible(this.isExtendedSize);
			this.swapColor.SetVisible(sel > 0);

			if ( sel > 0 )
			{
				this.reset.SetEnabled(this.isExtendedSize);
				this.fieldAngle.SetEnabled(this.isExtendedSize);
				this.fieldRepeat.SetEnabled(this.isExtendedSize);
				this.fieldMiddle.SetEnabled(this.isExtendedSize);
			}
			else
			{
				this.reset.SetEnabled(false);
				this.fieldAngle.SetEnabled(false);
				this.fieldRepeat.SetEnabled(false);
				this.fieldMiddle.SetEnabled(false);
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

				rect.Offset(0, -25);
				r = rect;

				r.Left = rect.Left;
				r.Width = 12;
				this.labelRepeat.Bounds = r;
				r.Left = r.Right;
				r.Width = 44;
				this.fieldRepeat.Bounds = r;
				r.Left = r.Right;
				r.Width = 12;
				this.labelMiddle.Bounds = r;
				r.Left = r.Right;
				r.Width = 45;
				this.fieldMiddle.Bounds = r;
				r.Left = r.Right;
				r.Width = 12;
				this.labelSmooth.Bounds = r;
				r.Left = r.Right;
				r.Width = 50;
				this.fieldSmooth.Bounds = r;
			}
		}
		

		private void HandleReset(object sender, MessageEventArgs e)
		{
			this.fieldAngle.Value = 0.0M;
			this.cx = 0.5;
			this.cy = 0.5;
			this.sx = 1.0;
			this.sy = 1.0;

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
			this.OnChanged();
		}

		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
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
		protected StaticText				labelAngle;
		protected StaticText				labelRepeat;
		protected StaticText				labelMiddle;
		protected StaticText				labelSmooth;
		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
		protected double					cx, cy;
		protected double					sx, sy;
	}
}

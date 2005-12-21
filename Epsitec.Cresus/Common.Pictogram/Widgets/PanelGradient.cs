using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelGradient permet de choisir un d�grad� de couleurs.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelGradient : AbstractPanel
	{
		public PanelGradient(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.listFill = new ScrollList(this);
			this.listFill.Items.Add("Uniforme");
			this.listFill.Items.Add("Lin�aire");
			this.listFill.Items.Add("Circulaire");
			this.listFill.Items.Add("Diamant");
			this.listFill.Items.Add("C�nique");
			this.listFill.SelectedIndexChanged += new EventHandler(this.ListChanged);
			this.listFill.TabIndex = 1;
			this.listFill.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.sample = new GradientSample(this);
			this.sample.Clicked += new MessageEventHandler(this.SampleClicked);
			ToolTip.Default.SetToolTip(this.sample, "Echantillon (cliquez pour remettre les valeurs standards)");

			this.fieldColor1 = new ColorSample(this);
			this.fieldColor1.PossibleSource = true;
			this.fieldColor1.Clicked += new MessageEventHandler(this.FieldColorClicked);
			this.fieldColor1.TabIndex = 2;
			this.fieldColor1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor1, "Couleur 1");

			this.fieldColor2 = new ColorSample(this);
			this.fieldColor2.PossibleSource = true;
			this.fieldColor2.Clicked += new MessageEventHandler(this.FieldColorClicked);
			this.fieldColor2.TabIndex = 3;
			this.fieldColor2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldColor2, "Couleur 2");

			this.fieldRepeat = new TextFieldSlider(this);
			this.fieldRepeat.MinValue = 1;
			this.fieldRepeat.MaxValue = 8;
			this.fieldRepeat.Step = 1;
			this.fieldRepeat.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldRepeat.TabIndex = 4;
			this.fieldRepeat.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRepeat, "Nombre de r�p�titions");

			this.fieldMiddle = new TextFieldSlider(this);
			this.fieldMiddle.MinValue = -100;
			this.fieldMiddle.MaxValue =  100;
			this.fieldMiddle.Step = 10;
			this.fieldMiddle.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldMiddle.TabIndex = 5;
			this.fieldMiddle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMiddle, "Couleur m�diane");

			this.fieldSmooth = new TextFieldSlider(this);
			this.fieldSmooth.MinValue =  0;
			this.fieldSmooth.MaxValue = 10;
			this.fieldSmooth.Step = 0.2M;
			this.fieldSmooth.Resolution = 0.1M;
			this.fieldSmooth.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldSmooth.TabIndex = 6;
			this.fieldSmooth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSmooth, "Flou");

			this.labelRepeat = new StaticText(this);
			this.labelRepeat.Text = "n";
			this.labelRepeat.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelMiddle = new StaticText(this);
			this.labelMiddle.Text = "M";
			this.labelMiddle.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelSmooth = new StaticText(this);
			this.labelSmooth.Text = "F";
			this.labelSmooth.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.swapColor = new IconButton(this);
			this.swapColor.IconName = @"file:images/swapdata.icon";
			this.swapColor.Clicked += new MessageEventHandler(this.SwapColorClicked);
			ToolTip.Default.SetToolTip(this.swapColor, "Permute les 2 couleurs");

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.listFill.SelectedIndexChanged -= new EventHandler(this.ListChanged);
				this.sample.Clicked -= new MessageEventHandler(this.SampleClicked);
				this.fieldColor1.Clicked -= new MessageEventHandler(this.FieldColorClicked);
				this.fieldColor2.Clicked -= new MessageEventHandler(this.FieldColorClicked);
				this.fieldRepeat.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldMiddle.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldSmooth.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.swapColor.Clicked -= new MessageEventHandler(this.SwapColorClicked);

				this.label = null;
				this.listFill = null;
				this.sample = null;
				this.fieldColor1 = null;
				this.fieldColor2 = null;
				this.swapColor = null;
				this.fieldRepeat = null;
				this.fieldMiddle = null;
				this.fieldSmooth = null;
				this.labelRepeat = null;
				this.labelMiddle = null;
				this.labelSmooth = null;
				this.originFieldColor = null;
			}

			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.extendedSize ? 83 : 30 );
			}
		}


		public override void SetProperty(AbstractProperty property)
		{
			//	Propri�t� -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyGradient p = property as PropertyGradient;
			if ( p == null )  return;

			int sel = -1;
			switch ( p.Fill )
			{
				case GradientFill.None:     sel = 0;  break;
				case GradientFill.Linear:   sel = 1;  break;
				case GradientFill.Circle:   sel = 2;  break;
				case GradientFill.Diamond:  sel = 3;  break;
				case GradientFill.Conic:    sel = 4;  break;
			}
			this.listFill.SelectedIndex = sel;
			this.listFill.ShowSelected(ScrollShowMode.Center);

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldRepeat.Value = (decimal) p.Repeat;
			this.fieldMiddle.Value = (decimal) p.Middle*100;
			this.fieldSmooth.Value = (decimal) p.Smooth;

			this.angle = p.Angle;
			this.cx    = p.Cx;
			this.cy    = p.Cy;
			this.sx    = p.Sx;
			this.sy    = p.Sy;

			this.sample.Gradient = p;

			this.EnableWidgets();
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propri�t�.
			PropertyGradient p = new PropertyGradient();
			base.GetProperty(p);

			p.Fill = GradientFill.None;
			switch ( this.listFill.SelectedIndex )
			{
				case 0:  p.Fill = GradientFill.None;     break;
				case 1:  p.Fill = GradientFill.Linear;   break;
				case 2:  p.Fill = GradientFill.Circle;   break;
				case 3:  p.Fill = GradientFill.Diamond;  break;
				case 4:  p.Fill = GradientFill.Conic;    break;
			}

			p.Color1 = this.fieldColor1.Color;
			p.Color2 = this.fieldColor2.Color;
			p.Repeat = (int)this.fieldRepeat.Value;
			p.Middle = (double) this.fieldMiddle.Value/100;
			p.Smooth = (double) this.fieldSmooth.Value;

			p.Angle = this.angle;
			p.Cx    = this.cx;
			p.Cy    = this.cy;
			p.Sx    = this.sx;
			p.Sy    = this.sy;

			this.sample.Gradient = p;
			return p;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets n�cessaires.
			int sel = this.listFill.SelectedIndex;

			this.label.SetVisible(!this.extendedSize);
			this.listFill.SetVisible(this.extendedSize);
			this.sample.SetVisible(this.extendedSize);
			this.fieldColor1.SetVisible(true);
			this.fieldColor2.SetVisible(this.extendedSize);
			this.fieldRepeat.SetVisible(this.extendedSize);
			this.fieldMiddle.SetVisible(this.extendedSize);
			this.fieldSmooth.SetVisible(this.extendedSize);
			this.labelRepeat.SetVisible(this.extendedSize);
			this.labelMiddle.SetVisible(this.extendedSize);
			this.labelSmooth.SetVisible(this.extendedSize);
			this.swapColor.SetVisible(this.extendedSize);

			if ( sel == 1 || sel == 2 || sel == 3 || sel == 4 )
			{
				this.fieldRepeat.SetEnabled(this.extendedSize);
				this.fieldMiddle.SetEnabled(this.extendedSize);
			}
			else
			{
				this.fieldRepeat.SetEnabled(false);
				this.fieldMiddle.SetEnabled(false);
			}

			this.fieldSmooth.SetEnabled(this.extendedSize);
		}


		public override void OriginColorDeselect()
		{
			//	D�s�lectionne toutes les origines de couleurs possibles.
			this.fieldColor1.ActiveState = WidgetState.ActiveNo;
			this.fieldColor2.ActiveState = WidgetState.ActiveNo;
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
			this.originFieldColor.ActiveState = WidgetState.ActiveYes;
		}

		public override int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return this.originFieldRank;
		}

		public override void OriginColorChange(Drawing.Color color)
		{
			//	Modifie la couleur d'origine.
			if ( this.originFieldColor == null )  return;
			this.originFieldColor.Color = color;
		}

		public override Drawing.Color OriginColorGet()
		{
			//	Donne la couleur d'origine.
			if ( this.originFieldColor == null )  return Drawing.Color.FromBrightness(0);
			return this.originFieldColor.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.fieldColor1 == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			if ( this.extendedSize )
			{
				Drawing.Rectangle r = rect;
				r.Width = 80;
				r.Bottom = r.Top-48;
				this.listFill.Bounds = r;
				this.listFill.ShowSelected(ScrollShowMode.Center);

				r.Left = r.Right+5;
				r.Width = 33;
				this.sample.Bounds = r;

				r = rect;
				r.Left = r.Right-50;
				r.Bottom = r.Top-20;
				this.fieldColor1.Bounds = r;
				r.Offset(0, -28);
				this.fieldColor2.Bounds = r;

				r.Bottom = r.Top-2;
				r.Height = 12;
				r.Left += (r.Width-(r.Height+8))/2;
				r.Width = r.Height+8;
				this.swapColor.Bounds = r;

				r = rect;
				r.Bottom = r.Top-48-25;
				r.Height = 20;
				r.Width = 14;
				this.labelRepeat.Bounds = r;
				r.Left = r.Right;
				r.Width = 45;
				this.fieldRepeat.Bounds = r;
				r.Left = r.Right;
				r.Width = 13;
				this.labelMiddle.Bounds = r;
				r.Left = r.Right;
				r.Width = 45;
				this.fieldMiddle.Bounds = r;
				r.Left = r.Right;
				r.Width = 13;
				this.labelSmooth.Bounds = r;
				r.Left = r.Right;
				r.Width = 45;
				this.fieldSmooth.Bounds = r;
			}
			else
			{
				Drawing.Rectangle r = rect;
				r.Right = rect.Right-50;
				this.label.Bounds = r;

				r = rect;
				r.Left = r.Right-50;
				this.fieldColor1.Bounds = r;
			}
		}
		

		private void SampleClicked(object sender, MessageEventArgs e)
		{
			this.angle = 0.0;
			this.cx    = 0.5;
			this.cy    = 0.5;
			this.sx    = 1.0;
			this.sy    = 1.0;

			this.OnChanged();
		}

		private void FieldColorClicked(object sender, MessageEventArgs e)
		{
			this.originFieldColor = sender as ColorSample;

			this.originFieldRank = -1;
			if ( this.originFieldColor == this.fieldColor1 )  this.originFieldRank = 0;
			if ( this.originFieldColor == this.fieldColor2 )  this.originFieldRank = 1;

			this.OnOriginColorChanged();
		}

		private void SwapColorClicked(object sender, MessageEventArgs e)
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

		private void ListChanged(object sender)
		{
			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				listFill;
		protected GradientSample			sample;
		protected ColorSample				fieldColor1;
		protected ColorSample				fieldColor2;
		protected IconButton				swapColor;
		protected TextFieldSlider			fieldRepeat;
		protected TextFieldSlider			fieldMiddle;
		protected TextFieldSlider			fieldSmooth;
		protected StaticText				labelRepeat;
		protected StaticText				labelMiddle;
		protected StaticText				labelSmooth;
		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
		protected double					angle;
		protected double					cx, cy;
		protected double					sx, sy;
	}
}

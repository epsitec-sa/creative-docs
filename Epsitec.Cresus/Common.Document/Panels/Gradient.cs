using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Gradient permet de choisir un d�grad� de couleurs.
	/// </summary>
	[SuppressBundleSupport]
	public class Gradient : Abstract
	{
		public Gradient(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.listFill = new ScrollList(this);
			this.listFill.Items.Add("Uniforme");
			this.listFill.Items.Add("Lin�aire");
			this.listFill.Items.Add("Circulaire");
			this.listFill.Items.Add("Diamant");
			this.listFill.Items.Add("C�nique");
			this.listFill.SelectedIndexChanged += new EventHandler(this.HandleListChanged);
			this.listFill.TabIndex = 1;
			this.listFill.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.sample = new GradientSample(this);
			this.sample.Clicked += new MessageEventHandler(this.HandleSampleClicked);
			ToolTip.Default.SetToolTip(this.sample, "Echantillon (cliquez pour remettre les valeurs standards)");

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

			this.fieldRepeat = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldRepeat);
			this.fieldRepeat.InternalMinValue = 1;
			this.fieldRepeat.InternalMaxValue = 8;
			this.fieldRepeat.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldRepeat.TabIndex = 4;
			this.fieldRepeat.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRepeat, "Nombre de r�p�titions");

			this.fieldMiddle = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldMiddle);
			this.fieldMiddle.InternalMinValue = -100;
			this.fieldMiddle.InternalMaxValue =  100;
			this.fieldMiddle.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldMiddle.TabIndex = 5;
			this.fieldMiddle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMiddle, "Couleur m�diane");

			this.fieldSmooth = new TextFieldReal(this);
			this.fieldSmooth.FactorMinRange = 0.0M;
			this.fieldSmooth.FactorMaxRange = 0.1M;
			this.fieldSmooth.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldSmooth);
			this.fieldSmooth.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldSmooth.TabIndex = 6;
			this.fieldSmooth.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSmooth, "Flou");

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
			this.swapColor.IconName = "manifest:Epsitec.App.DocumentEditor.Images.SwapData.icon";
			this.swapColor.Clicked += new MessageEventHandler(this.HandleSwapColorClicked);
			ToolTip.Default.SetToolTip(this.swapColor, "Permute les 2 couleurs");

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.listFill.SelectedIndexChanged -= new EventHandler(this.HandleListChanged);
				this.sample.Clicked -= new MessageEventHandler(this.HandleSampleClicked);
				this.fieldColor1.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldColor2.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fieldRepeat.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldMiddle.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldSmooth.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.swapColor.Clicked -= new MessageEventHandler(this.HandleSwapColorClicked);

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

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 83 : 30 );
			}
		}


		// Propri�t� -> widgets.
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
			this.listFill.ShowSelected(ScrollShowMode.Center);

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldRepeat.InternalValue = (decimal) p.Repeat;
			this.fieldMiddle.InternalValue = (decimal) p.Middle*100;
			this.fieldSmooth.InternalValue = (decimal) p.Smooth;

			this.angle = p.Angle;
			this.cx    = p.Cx;
			this.cy    = p.Cy;
			this.sx    = p.Sx;
			this.sy    = p.Sy;

			this.sample.Gradient = p;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propri�t�.
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
			p.Repeat = (int)this.fieldRepeat.InternalValue;
			p.Middle = (double) this.fieldMiddle.InternalValue/100;
			p.Smooth = (double) this.fieldSmooth.InternalValue;

			p.Angle = this.angle;
			p.Cx    = this.cx;
			p.Cy    = this.cy;
			p.Sx    = this.sx;
			p.Sy    = this.sy;

			this.sample.Gradient = p;
		}

		// Grise les widgets n�cessaires.
		protected void EnableWidgets()
		{
			int sel = this.listFill.SelectedIndex;

			this.label.SetVisible(!this.isExtendedSize);
			this.listFill.SetVisible(this.isExtendedSize);
			this.sample.SetVisible(this.isExtendedSize);
			this.fieldColor1.SetVisible(true);
			this.fieldColor2.SetVisible(this.isExtendedSize);
			this.fieldRepeat.SetVisible(this.isExtendedSize);
			this.fieldMiddle.SetVisible(this.isExtendedSize);
			this.fieldSmooth.SetVisible(this.isExtendedSize);
			this.labelRepeat.SetVisible(this.isExtendedSize);
			this.labelMiddle.SetVisible(this.isExtendedSize);
			this.labelSmooth.SetVisible(this.isExtendedSize);
			this.swapColor.SetVisible(this.isExtendedSize);

			if ( sel == 1 || sel == 2 || sel == 3 || sel == 4 )
			{
				this.fieldRepeat.SetEnabled(this.isExtendedSize);
				this.fieldMiddle.SetEnabled(this.isExtendedSize);
			}
			else
			{
				this.fieldRepeat.SetEnabled(false);
				this.fieldMiddle.SetEnabled(false);
			}

			this.fieldSmooth.SetEnabled(this.isExtendedSize);
		}


		// D�s�lectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fieldColor1.ActiveState = WidgetState.ActiveNo;
			this.fieldColor2.ActiveState = WidgetState.ActiveNo;
		}

		// S�lectionne l'origine de couleur.
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

		
		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldColor1 == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
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
				Rectangle r = rect;
				r.Right = rect.Right-50;
				this.label.Bounds = r;

				r = rect;
				r.Left = r.Right-50;
				this.fieldColor1.Bounds = r;
			}
		}
		

		private void HandleSampleClicked(object sender, MessageEventArgs e)
		{
			this.angle = 0.0;
			this.cx    = 0.5;
			this.cy    = 0.5;
			this.sx    = 1.0;
			this.sy    = 1.0;

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

		private void HandleListChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				listFill;
		protected GradientSample			sample;
		protected ColorSample				fieldColor1;
		protected ColorSample				fieldColor2;
		protected IconButton				swapColor;
		protected TextFieldReal				fieldRepeat;
		protected TextFieldReal				fieldMiddle;
		protected TextFieldReal				fieldSmooth;
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

using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelGradient permet de choisir un dégradé de couleurs.
	/// </summary>
	public class PanelGradient : AbstractPanel
	{
		public PanelGradient()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.listFill = new ScrollList(this);
			this.listFill.Items.Add("Uniforme");
			this.listFill.Items.Add("Lineaire");
			this.listFill.Items.Add("Circulaire");
			this.listFill.Items.Add("Diamant");
			this.listFill.Items.Add("Conique");
			this.listFill.SelectedIndexChanged += new EventHandler(this.ListChanged);

			this.sample = new GradientSample(this);
			this.sample.Clicked += new MessageEventHandler(this.SampleClicked);

			this.fieldColor1 = new ColorSample(this);
			this.fieldColor1.PossibleOrigin = true;
			this.fieldColor1.Clicked += new MessageEventHandler(this.FieldColorClicked);

			this.fieldColor2 = new ColorSample(this);
			this.fieldColor2.PossibleOrigin = true;
			this.fieldColor2.Clicked += new MessageEventHandler(this.FieldColorClicked);

			this.fieldRepeat = new TextFieldSlider(this);
			this.fieldRepeat.MinRange = 1;
			this.fieldRepeat.MaxRange = 8;
			this.fieldRepeat.Step = 1;
			this.fieldRepeat.TextChanged += new EventHandler(this.TextChanged);

			this.fieldMiddle = new TextFieldSlider(this);
			this.fieldMiddle.MinRange = -100;
			this.fieldMiddle.MaxRange =  100;
			this.fieldMiddle.Step = 10;
			this.fieldMiddle.TextChanged += new EventHandler(this.TextChanged);

			this.fieldSmooth = new TextFieldSlider(this);
			this.fieldSmooth.MinRange =  0;
			this.fieldSmooth.MaxRange = 10;
			this.fieldSmooth.Step = 1;
			this.fieldSmooth.TextChanged += new EventHandler(this.TextChanged);

			this.labelRepeat = new StaticText(this);
			this.labelRepeat.Text = "n";
			this.labelRepeat.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelMiddle = new StaticText(this);
			this.labelMiddle.Text = "M";
			this.labelMiddle.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelSmooth = new StaticText(this);
			this.labelSmooth.Text = "F";
			this.labelSmooth.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		public PanelGradient(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			this.listFill.SelectedIndexChanged -= new EventHandler(this.ListChanged);
			this.sample.Clicked -= new MessageEventHandler(this.SampleClicked);
			this.fieldColor1.Clicked -= new MessageEventHandler(this.FieldColorClicked);
			this.fieldColor2.Clicked -= new MessageEventHandler(this.FieldColorClicked);
			this.fieldRepeat.TextChanged -= new EventHandler(this.TextChanged);
			this.fieldMiddle.TextChanged -= new EventHandler(this.TextChanged);
			this.fieldSmooth.TextChanged -= new EventHandler(this.TextChanged);

			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.extendedSize ? 83 : 30 );
			}
		}


		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

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
			this.listFill.ShowSelectedLine(ScrollListShow.Middle);

			this.fieldColor1.Color = p.Color1;
			this.fieldColor2.Color = p.Color2;
			this.fieldRepeat.Value = p.Repeat;
			this.fieldMiddle.Value = p.Middle*100;
			this.fieldSmooth.Value = p.Smooth;

			this.angle = p.Angle;
			this.cx    = p.Cx;
			this.cy    = p.Cy;
			this.sx    = p.Sx;
			this.sy    = p.Sy;

			this.sample.Gradient = p;

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
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
			p.Middle = this.fieldMiddle.Value/100;
			p.Smooth = this.fieldSmooth.Value;

			p.Angle = this.angle;
			p.Cx    = this.cx;
			p.Cy    = this.cy;
			p.Sx    = this.sx;
			p.Sy    = this.sy;

			this.sample.Gradient = p;
			return p;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
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

			if ( sel == 1 || sel == 2 || sel == 3 || sel == 4 )
			{
				this.fieldRepeat.SetEnabled(true);
				this.fieldMiddle.SetEnabled(true);
			}
			else
			{
				this.fieldRepeat.SetEnabled(false);
				this.fieldMiddle.SetEnabled(false);
			}

			this.fieldSmooth.SetEnabled(true);
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
			this.originFieldColor.Color = color;
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

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			if ( this.extendedSize )
			{
				Drawing.Rectangle r = rect;
				r.Width = 80;
				r.Bottom = r.Top-48;
				this.listFill.Bounds = r;
				this.listFill.ShowSelectedLine(ScrollListShow.Middle);

				r.Left = r.Right+5;
				r.Width = 33;
				this.sample.Bounds = r;

				r = rect;
				r.Left = r.Right-50;
				r.Bottom = r.Top-20;
				this.fieldColor1.Bounds = r;
				r.Offset(0, -28);
				this.fieldColor2.Bounds = r;

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

		private void ListChanged(object sender)
		{
			this.EnableWidgets();
			this.OnChanged();
		}

		private void TextChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				listFill;
		protected GradientSample			sample;
		protected ColorSample				fieldColor1;
		protected ColorSample				fieldColor2;
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

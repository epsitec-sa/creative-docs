using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelShadow permet de choisir une ombre.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelShadow : AbstractPanel
	{
		public PanelShadow(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldColor = new ColorSample(this);
			this.fieldColor.PossibleSource = true;
			this.fieldColor.Clicked += new MessageEventHandler(this.FieldColorClicked);
			this.fieldColor.TabIndex = 1;
			this.fieldColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldRadius = new TextFieldSlider(this);
			this.fieldRadius.MinValue =  0;
			this.fieldRadius.MaxValue = 10;
			this.fieldRadius.Step = 1;
			this.fieldRadius.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldOx = new TextFieldSlider(this);
			this.fieldOx.MinValue = -10;
			this.fieldOx.MaxValue =  10;
			this.fieldOx.Step = 1;
			this.fieldOx.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldOx.TabIndex = 3;
			this.fieldOx.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldOy = new TextFieldSlider(this);
			this.fieldOy.MinValue = -10;
			this.fieldOy.MaxValue =  10;
			this.fieldOy.Step = 1;
			this.fieldOy.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldOy.TabIndex = 4;
			this.fieldOy.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelRadius = new StaticText(this);
			this.labelRadius.Text = "R";
			this.labelRadius.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelOx = new StaticText(this);
			this.labelOx.Text = "X";
			this.labelOx.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelOy = new StaticText(this);
			this.labelOy.Text = "Y";
			this.labelOy.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldColor.Clicked -= new MessageEventHandler(this.FieldColorClicked);
				this.fieldRadius.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldOx.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldOy.TextChanged -= new EventHandler(this.HandleTextChanged);

				this.label = null;
				this.fieldColor = null;
				this.fieldRadius = null;
				this.fieldOx = null;
				this.fieldOy = null;
				this.labelRadius = null;
				this.labelOx = null;
				this.labelOy = null;
			}

			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.extendedSize ? 55 : 30 );
			}
		}


		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyShadow p = property as PropertyShadow;
			if ( p == null )  return;

			this.fieldColor.Color  = p.Color;
			this.fieldRadius.Value = (decimal) p.Radius;
			this.fieldOx.Value     = (decimal) p.Ox;
			this.fieldOy.Value     = (decimal) p.Oy;
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyShadow p = new PropertyShadow();
			base.GetProperty(p);

			p.Color  = this.fieldColor.Color;
			p.Radius = (double) this.fieldRadius.Value;
			p.Ox     = (double) this.fieldOx.Value;
			p.Oy     = (double) this.fieldOy.Value;

			return p;
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.fieldColor.ActiveState = WidgetState.ActiveNo;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			this.fieldColor.ActiveState = WidgetState.ActiveYes;
		}

		public override void OriginColorChange(Drawing.Color color)
		{
			//	Modifie la couleur d'origine.
			this.fieldColor.Color = color;
		}

		public override Drawing.Color OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return this.fieldColor.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldColor == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldColor.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 14;
			this.labelRadius.Bounds = r;
			r.Left = r.Right;
			r.Width = 45;
			this.fieldRadius.Bounds = r;
			r.Left = r.Right;
			r.Width = 13;
			this.labelOx.Bounds = r;
			r.Left = r.Right;
			r.Width = 45;
			this.fieldOx.Bounds = r;
			r.Left = r.Right;
			r.Width = 13;
			this.labelOy.Bounds = r;
			r.Left = r.Right;
			r.Width = 45;
			this.fieldOy.Bounds = r;
		}
		

		private void FieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected ColorSample				fieldColor;
		protected TextFieldSlider			fieldRadius;
		protected TextFieldSlider			fieldOx;
		protected TextFieldSlider			fieldOy;
		protected StaticText				labelRadius;
		protected StaticText				labelOx;
		protected StaticText				labelOy;
	}
}

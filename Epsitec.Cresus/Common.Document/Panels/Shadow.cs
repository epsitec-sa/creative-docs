using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Shadow permet de choisir une ombre.
	/// </summary>
	[SuppressBundleSupport]
	public class Shadow : Abstract
	{
		public Shadow(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fieldColor = new ColorSample(this);
			this.fieldColor.PossibleSource = true;
			this.fieldColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fieldColor.TabIndex = 1;
			this.fieldColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldRadius = new TextFieldReal(this);
			this.fieldRadius.FactorMinRange = 0.0M;
			this.fieldRadius.FactorMaxRange = 0.1M;
			this.fieldRadius.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldRadius);
			this.fieldRadius.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldOx = new TextFieldReal(this);
			this.fieldOx.FactorMinRange = 0.0M;
			this.fieldOx.FactorMaxRange = 0.1M;
			this.fieldOx.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldOx);
			this.fieldOx.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldOx.TabIndex = 3;
			this.fieldOx.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldOy = new TextFieldReal(this);
			this.fieldOy.FactorMinRange = 0.0M;
			this.fieldOy.FactorMaxRange = 0.1M;
			this.fieldOy.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldOy);
			this.fieldOy.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldOy.TabIndex = 4;
			this.fieldOy.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelRadius = new StaticText(this);
			this.labelRadius.Text = "R";
			this.labelRadius.Alignment = ContentAlignment.MiddleCenter;

			this.labelOx = new StaticText(this);
			this.labelOx.Text = "X";
			this.labelOx.Alignment = ContentAlignment.MiddleCenter;

			this.labelOy = new StaticText(this);
			this.labelOy.Text = "Y";
			this.labelOy.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldColor.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
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

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 55 : 30 );
			}
		}


		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Shadow p = this.property as Properties.Shadow;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.fieldColor.Color = p.Color;
			this.fieldRadius.InternalValue = (decimal) p.Radius;
			this.fieldOx.InternalValue     = (decimal) p.Ox;
			this.fieldOy.InternalValue     = (decimal) p.Oy;

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Shadow p = this.property as Properties.Shadow;
			if ( p == null )  return;

			p.Color  = this.fieldColor.Color;
			p.Radius = (double) this.fieldRadius.InternalValue;
			p.Ox     = (double) this.fieldOx.InternalValue;
			p.Oy     = (double) this.fieldOy.InternalValue;
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fieldColor.ActiveState = WidgetState.ActiveNo;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			this.fieldColor.ActiveState = WidgetState.ActiveYes;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.Color color)
		{
			this.fieldColor.Color = color;
		}

		// Donne la couleur d'origine.
		public override Drawing.Color OriginColorGet()
		{
			return this.fieldColor.Color;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldColor == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
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
		

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected StaticText				label;
		protected ColorSample				fieldColor;
		protected TextFieldReal				fieldRadius;
		protected TextFieldReal				fieldOx;
		protected TextFieldReal				fieldOy;
		protected StaticText				labelRadius;
		protected StaticText				labelOx;
		protected StaticText				labelOy;
	}
}

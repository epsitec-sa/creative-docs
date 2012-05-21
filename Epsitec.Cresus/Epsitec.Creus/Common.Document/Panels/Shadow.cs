using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Shadow permet de choisir une ombre.
	/// </summary>
	public class Shadow : Abstract
	{
		public Shadow(Document document) : base(document)
		{
			this.fieldColor = new ColorSample(this);
			this.fieldColor.DragSourceFrame = true;
			this.fieldColor.Clicked += this.HandleFieldColorClicked;
			this.fieldColor.TabIndex = 1;
			this.fieldColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.fieldRadius = new TextFieldReal(this);
			this.fieldRadius.FactorMinRange = 0.0M;
			this.fieldRadius.FactorMaxRange = 0.1M;
			this.fieldRadius.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldRadius);
			this.fieldRadius.ValueChanged += this.HandleValueChanged;
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.fieldOx = new TextFieldReal(this);
			this.fieldOx.FactorMinRange = 0.0M;
			this.fieldOx.FactorMaxRange = 0.1M;
			this.fieldOx.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldOx);
			this.fieldOx.ValueChanged += this.HandleValueChanged;
			this.fieldOx.TabIndex = 3;
			this.fieldOx.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.fieldOy = new TextFieldReal(this);
			this.fieldOy.FactorMinRange = 0.0M;
			this.fieldOy.FactorMaxRange = 0.1M;
			this.fieldOy.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldOy);
			this.fieldOy.ValueChanged += this.HandleValueChanged;
			this.fieldOy.TabIndex = 4;
			this.fieldOy.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.labelRadius = new StaticText(this);
			this.labelRadius.Text = "R";
			this.labelRadius.ContentAlignment = ContentAlignment.MiddleCenter;

			this.labelOx = new StaticText(this);
			this.labelOx.Text = "X";
			this.labelOx.ContentAlignment = ContentAlignment.MiddleCenter;

			this.labelOy = new StaticText(this);
			this.labelOy.Text = "Y";
			this.labelOy.ContentAlignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldColor.Clicked -= this.HandleFieldColorClicked;
				this.fieldRadius.ValueChanged -= this.HandleValueChanged;
				this.fieldOx.ValueChanged -= this.HandleValueChanged;
				this.fieldOy.ValueChanged -= this.HandleValueChanged;

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
				return ( this.isExtendedSize ? this.LabelHeight+55 : this.LabelHeight+30 );
			}
		}


		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Shadow p = this.property as Properties.Shadow;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldColor.Color = p.Color;
			this.fieldRadius.InternalValue = (decimal) p.Radius;
			this.fieldOx.InternalValue     = (decimal) p.Ox;
			this.fieldOy.InternalValue     = (decimal) p.Oy;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Shadow p = this.property as Properties.Shadow;
			if ( p == null )  return;

			p.Color  = this.fieldColor.Color;
			p.Radius = (double) this.fieldRadius.InternalValue;
			p.Ox     = (double) this.fieldOx.InternalValue;
			p.Oy     = (double) this.fieldOy.InternalValue;
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.fieldColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			this.fieldColor.ActiveState = ActiveState.Yes;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			this.fieldColor.Color = color;
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return this.fieldColor.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldColor == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldColor.SetManualBounds(r);

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 14;
			this.labelRadius.SetManualBounds(r);
			r.Left = r.Right;
			r.Width = 45;
			this.fieldRadius.SetManualBounds(r);
			r.Left = r.Right;
			r.Width = 13;
			this.labelOx.SetManualBounds(r);
			r.Left = r.Right;
			r.Width = 45;
			this.fieldOx.SetManualBounds(r);
			r.Left = r.Right;
			r.Width = 13;
			this.labelOy.SetManualBounds(r);
			r.Left = r.Right;
			r.Width = 45;
			this.fieldOy.SetManualBounds(r);
		}
		

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected ColorSample				fieldColor;
		protected TextFieldReal				fieldRadius;
		protected TextFieldReal				fieldOx;
		protected TextFieldReal				fieldOy;
		protected StaticText				labelRadius;
		protected StaticText				labelOx;
		protected StaticText				labelOy;
	}
}

using Epsitec.Common.NiceIcon;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe PanelColor permet de choisir une couleur.
	/// </summary>
	public class PanelColor : AbstractPanel
	{
		public PanelColor()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new ColorSample(this);
			this.field.PossibleOrigin = true;
			this.field.Clicked += new MessageEventHandler(this.FieldClicked);
		}
		
		public PanelColor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			this.field.Clicked -= new MessageEventHandler(this.FieldClicked);
			base.Dispose(disposing);
		}

		
		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

			PropertyColor p = property as PropertyColor;
			if ( p == null )  return;

			this.field.Color = p.Color;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyColor p = new PropertyColor();
			base.GetProperty(p);

			p.Color = this.field.Color;
			return p;
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.field.ActiveState = WidgetState.ActiveNo;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			this.field.ActiveState = WidgetState.ActiveYes;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.Color color)
		{
			this.field.Color = color;
		}

		// Donne la couleur d'origine.
		public override Drawing.Color OriginColorGet()
		{
			return this.field.Color;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-50;
			this.field.Bounds = r;
		}
		

		private void FieldClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}


		protected StaticText				label;
		protected ColorSample				field;
	}
}

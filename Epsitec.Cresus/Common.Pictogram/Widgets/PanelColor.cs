using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelColor permet de choisir une couleur.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelColor : AbstractPanel
	{
		public PanelColor(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new ColorSample(this);
			this.field.PossibleSource = true;
			this.field.Clicked += new MessageEventHandler(this.FieldClicked);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Couleur");
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.Clicked -= new MessageEventHandler(this.FieldClicked);
				this.field = null;
				this.label = null;
			}

			base.Dispose(disposing);
		}

		
		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyColor p = property as PropertyColor;
			if ( p == null )  return;

			this.field.Color = p.Color;
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyColor p = new PropertyColor();
			base.GetProperty(p);

			p.Color = this.field.Color;
			return p;
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.field.ActiveState = WidgetState.ActiveNo;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			this.field.ActiveState = WidgetState.ActiveYes;
		}

		public override void OriginColorChange(Drawing.Color color)
		{
			//	Modifie la couleur d'origine.
			this.field.Color = color;
		}

		public override Drawing.Color OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return this.field.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

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

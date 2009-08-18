using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Color permet de choisir une couleur.
	/// </summary>
	public class Color : Abstract
	{
		public Color(Document document) : base(document)
		{
			this.field = new ColorSample(this);
			this.field.DragSourceFrame = true;
			this.field.Clicked += this.HandleFieldColorClicked;
			this.field.ColorChanged += this.HandleFieldColorChanged;
			this.field.TabIndex = 1;
			this.field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.Color.Tooltip.Main);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.Clicked -= this.HandleFieldColorClicked;
				this.field.ColorChanged -= this.HandleFieldColorChanged;
				this.field = null;
			}

			base.Dispose(disposing);
		}

		
		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Color p = this.property as Properties.Color;
			if ( p == null )  return;

			this.ignoreChanged = true;
			this.field.Color = p.ColorValue;
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Color p = this.property as Properties.Color;
			if ( p == null )  return;

			p.ColorValue = this.field.Color;
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.field.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			this.field.ActiveState = ActiveState.Yes;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			if ( this.field.Color != color )
			{
				this.field.Color = color;
				this.OnChanged();
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return this.field.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Left = r.Right-50;
			this.field.SetManualBounds(r);
		}
		

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();
			}

			this.OnChanged();
		}


		protected ColorSample				field;
	}
}

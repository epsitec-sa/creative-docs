using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Color permet de choisir une couleur.
	/// </summary>
	[SuppressBundleSupport]
	public class Color : Abstract
	{
		public Color(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.field = new ColorSample(this);
			this.field.PossibleOrigin = true;
			this.field.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.field.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Couleur");
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.field.Changed -= new EventHandler(this.HandleFieldColorChanged);
				this.field = null;
				this.label = null;
			}

			base.Dispose(disposing);
		}

		
		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Color p = this.property as Properties.Color;
			if ( p == null )  return;

			this.ignoreChanged = true;
			this.label.Text = p.TextStyle;
			this.field.Color = p.ColorValue;
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Color p = this.property as Properties.Color;
			if ( p == null )  return;

			p.ColorValue = this.field.Color;
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
			if ( this.field.Color != color )
			{
				this.field.Color = color;
				this.OnChanged();
			}
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

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-50;
			this.field.Bounds = r;
		}
		

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == WidgetState.ActiveYes )
			{
				this.OnOriginColorChanged();
			}

			this.OnChanged();
		}


		protected StaticText				label;
		protected ColorSample				field;
	}
}

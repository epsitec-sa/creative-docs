using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelRegular permet de choisir un type de polygone régulier.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelRegular : AbstractPanel
	{
		public PanelRegular()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldNbFaces = new TextFieldSlider(this);
			this.fieldNbFaces.MinValue = 3;
			this.fieldNbFaces.MaxValue = 24;
			this.fieldNbFaces.Step = 1;
			this.fieldNbFaces.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldNbFaces.TabIndex = 1;
			this.fieldNbFaces.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.checkStar = new CheckButton(this);
			this.checkStar.Text = "Etoile";
			this.checkStar.ActiveStateChanged += new EventHandler(this.HandleCheckChanged);
			this.checkStar.TabIndex = 2;
			this.checkStar.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldDeep = new TextFieldSlider(this);
			this.fieldDeep.MinValue = 0;
			this.fieldDeep.MaxValue = 100;
			this.fieldDeep.Step = 5;
			this.fieldDeep.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 3;
			this.fieldDeep.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelDeep = new StaticText(this);
			this.labelDeep.Text = "Renfoncement";
			this.labelDeep.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		public PanelRegular(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldNbFaces.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.checkStar.ActiveStateChanged -= new EventHandler(this.HandleCheckChanged);
				this.fieldDeep.TextChanged -= new EventHandler(this.HandleFieldChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.extendedSize ? 55 : 30 );
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyRegular p = property as PropertyRegular;
			if ( p == null )  return;

			this.fieldNbFaces.Value = p.NbFaces;
			this.checkStar.ActiveState = p.Star ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.fieldDeep.Value = (decimal) p.Deep*100;

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyRegular p = new PropertyRegular();
			base.GetProperty(p);

			p.NbFaces = (int)this.fieldNbFaces.Value;
			p.Star = ( this.checkStar.ActiveState == WidgetState.ActiveYes );
			p.Deep = (double) this.fieldDeep.Value/100;

			return p;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.checkStar.SetEnabled(this.extendedSize);

			bool star = ( this.checkStar.ActiveState == WidgetState.ActiveYes );
			this.fieldDeep.SetEnabled(this.extendedSize && star);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldNbFaces == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldNbFaces.Bounds = r;

			rect.Top = r.Bottom-5;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Height = 20;
			r.Width = 50;
			this.checkStar.Bounds = r;
			r.Left = r.Right;
			r.Width = 75;
			this.labelDeep.Bounds = r;
			r.Left = r.Right;
			r.Width = 50;
			this.fieldDeep.Bounds = r;
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleCheckChanged(object sender)
		{
			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldSlider			fieldNbFaces;
		protected CheckButton				checkStar;
		protected TextFieldSlider			fieldDeep;
		protected StaticText				labelDeep;
	}
}

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Regular permet de choisir un type de polygone régulier.
	/// </summary>
	[SuppressBundleSupport]
	public class Regular : Abstract
	{
		public Regular(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fieldNbFaces = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldNbFaces);
			this.fieldNbFaces.InternalMinValue = 3;
			this.fieldNbFaces.InternalMaxValue = 24;
			this.fieldNbFaces.Step = 1;
			this.fieldNbFaces.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldNbFaces.TabIndex = 1;
			this.fieldNbFaces.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.checkStar = new CheckButton(this);
			this.checkStar.Text = Res.Strings.Panel.Regular.Label.Star;
			this.checkStar.ActiveStateChanged += new EventHandler(this.HandleCheckChanged);
			this.checkStar.TabIndex = 2;
			this.checkStar.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldDeep = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldDeep);
			this.fieldDeep.InternalMinValue = 0;
			this.fieldDeep.InternalMaxValue = 100;
			this.fieldDeep.Step = 5;
			this.fieldDeep.TextSuffix = "%";
			this.fieldDeep.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 3;
			this.fieldDeep.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelDeep = new StaticText(this);
			this.labelDeep.Text = Res.Strings.Panel.Regular.Label.Deep;
			this.labelDeep.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldNbFaces.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.checkStar.ActiveStateChanged -= new EventHandler(this.HandleCheckChanged);
				this.fieldDeep.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.fieldNbFaces = null;
				this.checkStar = null;
				this.fieldDeep = null;
				this.labelDeep = null;
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

			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.fieldNbFaces.InternalValue = p.NbFaces;
			this.checkStar.ActiveState = p.Star ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.fieldDeep.InternalValue = (decimal) p.Deep*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			p.NbFaces = (int)this.fieldNbFaces.InternalValue;
			p.Star = ( this.checkStar.ActiveState == WidgetState.ActiveYes );
			p.Deep = (double) this.fieldDeep.InternalValue/100;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.checkStar.SetEnabled(this.isExtendedSize);

			bool star = ( this.checkStar.ActiveState == WidgetState.ActiveYes );
			this.fieldDeep.SetEnabled(this.isExtendedSize && star);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldNbFaces == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
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
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleCheckChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldReal				fieldNbFaces;
		protected CheckButton				checkStar;
		protected TextFieldReal				fieldDeep;
		protected StaticText				labelDeep;
	}
}

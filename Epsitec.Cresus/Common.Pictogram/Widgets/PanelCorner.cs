using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelCorner permet de choisir un type de coin.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelCorner : AbstractPanel
	{
		public PanelCorner(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldType = new TextFieldCombo(this);
			this.fieldType.IsReadOnly = true;
			for ( int i=0 ; i<100 ; i++ )
			{
				CornerType type = PropertyCorner.ConvType(i);
				if ( type == CornerType.None )  break;
				this.fieldType.Items.Add(PropertyCorner.GetName(type));
			}
			this.fieldType.SelectedIndexChanged += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TabIndex = 1;
			this.fieldType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldRadius = new TextFieldSlider(this);
			this.fieldRadius.MinValue = 0;
			this.fieldRadius.MaxValue = 10;
			this.fieldRadius.Step = 0.1M;
			this.fieldRadius.Resolution = 0.1M;
			this.fieldRadius.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRadius, "Rayon");

			this.fieldEffect1 = new TextFieldSlider(this);
			this.fieldEffect1.MinValue = -100;
			this.fieldEffect1.MaxValue = 200;
			this.fieldEffect1.Step = 5;
			this.fieldEffect1.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldEffect1.TabIndex = 3;
			this.fieldEffect1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect1, "Paramètre A");

			this.fieldEffect2 = new TextFieldSlider(this);
			this.fieldEffect2.MinValue = -100;
			this.fieldEffect2.MaxValue = 200;
			this.fieldEffect2.Step = 5;
			this.fieldEffect2.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldEffect2.TabIndex = 4;
			this.fieldEffect2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect2, "Paramètre B");

			this.labelRadius = new StaticText(this);
			this.labelRadius.Text = "R";
			this.labelRadius.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelEffect1 = new StaticText(this);
			this.labelEffect1.Text = "A";
			this.labelEffect1.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.labelEffect2 = new StaticText(this);
			this.labelEffect2.Text = "B";
			this.labelEffect2.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldType.SelectedIndexChanged -= new EventHandler(this.HandleTypeChanged);
				this.fieldRadius.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldEffect1.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldEffect2.TextChanged -= new EventHandler(this.HandleFieldChanged);
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

			PropertyCorner p = property as PropertyCorner;
			if ( p == null )  return;

			this.fieldType.SelectedIndex = PropertyCorner.ConvType(p.CornerType);
			this.fieldRadius.Value  = (decimal) p.Radius;
			this.fieldEffect1.Value = (decimal) p.Effect1*100;
			this.fieldEffect2.Value = (decimal) p.Effect2*100;

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyCorner p = new PropertyCorner();
			base.GetProperty(p);

			p.CornerType = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			p.Radius  = (double) this.fieldRadius.Value;
			p.Effect1 = (double) this.fieldEffect1.Value/100;
			p.Effect2 = (double) this.fieldEffect2.Value/100;

			return p;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			// Initialise les min/max en fonction du type choisi.
			CornerType type = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			PropertyCorner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

			this.fieldEffect1.MinValue = (decimal) min1*100;
			this.fieldEffect1.MaxValue = (decimal) max1*100;
			this.fieldEffect2.MinValue = (decimal) min2*100;
			this.fieldEffect2.MaxValue = (decimal) max2*100;

			this.fieldRadius.SetEnabled(this.extendedSize && enableRadius);
			this.fieldEffect1.SetEnabled(this.extendedSize && enable1);
			this.fieldEffect2.SetEnabled(this.extendedSize && enable2);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldType == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-110;
			this.label.Bounds = r;

			r.Left = rect.Right-110;
			r.Right = rect.Right;
			this.fieldType.Bounds = r;

			rect.Top = r.Bottom-5;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Height = 20;
			r.Width = 14;
			this.labelRadius.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldRadius.Bounds = r;
			r.Left = r.Right;
			r.Width = 15;
			this.labelEffect1.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldEffect1.Bounds = r;
			r.Left = r.Right;
			r.Width = 10;
			this.labelEffect2.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldEffect2.Bounds = r;
		}
		
		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			// Met les valeurs par défaut correspondant au type choisi.
			CornerType type = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			PropertyCorner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
			this.fieldEffect1.Value = (decimal) effect1*100;
			this.fieldEffect2.Value = (decimal) effect2*100;

			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo			fieldType;
		protected TextFieldSlider			fieldRadius;
		protected TextFieldSlider			fieldEffect1;
		protected TextFieldSlider			fieldEffect2;
		protected StaticText				labelRadius;
		protected StaticText				labelEffect1;
		protected StaticText				labelEffect2;
	}
}

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelCorner permet de choisir un type de coin.
	/// </summary>
	public class PanelCorner : AbstractPanel
	{
		public PanelCorner()
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
			this.fieldType.TextChanged += new EventHandler(this.HandleTypeChanged);

			this.fieldRadius = new TextFieldSlider(this);
			this.fieldRadius.MinRange = 0;
			this.fieldRadius.MaxRange = 10;
			this.fieldRadius.Step = 1;
			this.fieldRadius.TextChanged += new EventHandler(this.HandleFieldChanged);

			this.fieldEffect1 = new TextFieldSlider(this);
			this.fieldEffect1.MinRange = -100;
			this.fieldEffect1.MaxRange = 200;
			this.fieldEffect1.Step = 5;
			this.fieldEffect1.TextChanged += new EventHandler(this.HandleFieldChanged);

			this.fieldEffect2 = new TextFieldSlider(this);
			this.fieldEffect2.MinRange = -100;
			this.fieldEffect2.MaxRange = 200;
			this.fieldEffect2.Step = 5;
			this.fieldEffect2.TextChanged += new EventHandler(this.HandleFieldChanged);

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
		
		public PanelCorner(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldType.TextChanged -= new EventHandler(this.HandleTypeChanged);
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

		// Propri�t� -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

			PropertyCorner p = property as PropertyCorner;
			if ( p == null )  return;

			this.fieldType.SelectedIndex = PropertyCorner.ConvType(p.CornerType);
			this.fieldRadius.Value  = p.Radius;
			this.fieldEffect1.Value = p.Effect1*100;
			this.fieldEffect2.Value = p.Effect2*100;

			this.EnableWidgets();
		}

		// Widget -> propri�t�.
		public override AbstractProperty GetProperty()
		{
			PropertyCorner p = new PropertyCorner();
			base.GetProperty(p);

			p.CornerType = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			p.Radius  = this.fieldRadius.Value;
			p.Effect1 = this.fieldEffect1.Value/100;
			p.Effect2 = this.fieldEffect2.Value/100;

			return p;
		}

		// Grise les widgets n�cessaires.
		protected void EnableWidgets()
		{
			// Initialise les min/max en fonction du type choisi.
			CornerType type = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			PropertyCorner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

			this.fieldEffect1.MinRange = min1*100;
			this.fieldEffect1.MaxRange = max1*100;
			this.fieldEffect2.MinRange = min2*100;
			this.fieldEffect2.MaxRange = max2*100;

			this.fieldRadius.SetEnabled(enableRadius);
			this.fieldEffect1.SetEnabled(enable1);
			this.fieldEffect2.SetEnabled(enable2);
		}

		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldType == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			r.Left = rect.Right-100;
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
		
		// Le type a �t� chang�.
		private void HandleTypeChanged(object sender)
		{
			// Met les valeurs par d�faut correspondant au type choisi.
			CornerType type = PropertyCorner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			PropertyCorner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
			this.fieldEffect1.Value = effect1*100;
			this.fieldEffect2.Value = effect2*100;

			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a �t� chang�.
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

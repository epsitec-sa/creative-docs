using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Corner permet de choisir un type de coin.
	/// </summary>
	[SuppressBundleSupport]
	public class Corner : Abstract
	{
		public Corner(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fieldType = new TextFieldCombo(this);
			this.fieldType.IsReadOnly = true;
			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.CornerType type = Properties.Corner.ConvType(i);
				if ( type == Properties.CornerType.None )  break;
				this.fieldType.Items.Add(Properties.Corner.GetName(type));
			}
			//?this.fieldType.SelectedIndexChanged += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TextChanged += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TabIndex = 1;
			this.fieldType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldRadius = new TextFieldReal(this);
			this.fieldRadius.FactorMinRange = 0.0M;
			this.fieldRadius.FactorMaxRange = 0.1M;
			this.fieldRadius.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldRadius);
			this.fieldRadius.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRadius, "Rayon");

			this.fieldEffect1 = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect1);
			this.fieldEffect1.InternalMinValue = -100;
			this.fieldEffect1.InternalMaxValue = 200;
			this.fieldEffect1.Step = 5;
			this.fieldEffect1.TextSuffix = "%";
			this.fieldEffect1.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldEffect1.TabIndex = 3;
			this.fieldEffect1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect1, "Param�tre A");

			this.fieldEffect2 = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect2);
			this.fieldEffect2.InternalMinValue = -100;
			this.fieldEffect2.InternalMaxValue = 200;
			this.fieldEffect2.Step = 5;
			this.fieldEffect2.TextSuffix = "%";
			this.fieldEffect2.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldEffect2.TabIndex = 4;
			this.fieldEffect2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect2, "Param�tre B");

			this.labelRadius = new StaticText(this);
			this.labelRadius.Text = "R";
			this.labelRadius.Alignment = ContentAlignment.MiddleCenter;

			this.labelEffect1 = new StaticText(this);
			this.labelEffect1.Text = "A";
			this.labelEffect1.Alignment = ContentAlignment.MiddleRight;

			this.labelEffect2 = new StaticText(this);
			this.labelEffect2.Text = "B";
			this.labelEffect2.Alignment = ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//?this.fieldType.SelectedIndexChanged -= new EventHandler(this.HandleTypeChanged);
				this.fieldType.TextChanged -= new EventHandler(this.HandleTypeChanged);
				this.fieldRadius.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldEffect1.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldEffect2.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.fieldType = null;
				this.fieldRadius = null;
				this.fieldEffect1 = null;
				this.fieldEffect2 = null;
				this.labelRadius = null;
				this.labelEffect1 = null;
				this.labelEffect2 = null;
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

		// Propri�t� -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Corner p = this.property as Properties.Corner;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.fieldType.SelectedIndex = Properties.Corner.ConvType(p.CornerType);
			this.fieldRadius.InternalValue  = (decimal) p.Radius;
			this.fieldEffect1.InternalValue = (decimal) p.Effect1*100;
			this.fieldEffect2.InternalValue = (decimal) p.Effect2*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propri�t�.
		protected override void WidgetsToProperty()
		{
			Properties.Corner p = this.property as Properties.Corner;
			if ( p == null )  return;

			p.CornerType = Properties.Corner.ConvType(this.fieldType.SelectedIndex);
			p.Radius  = (double) this.fieldRadius.InternalValue;
			p.Effect1 = (double) this.fieldEffect1.InternalValue/100;
			p.Effect2 = (double) this.fieldEffect2.InternalValue/100;
		}

		// Grise les widgets n�cessaires.
		protected void EnableWidgets()
		{
			// Initialise les min/max en fonction du type choisi.
			Properties.CornerType type = Properties.Corner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			Properties.Corner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

			this.fieldEffect1.InternalMinValue = (decimal) min1*100;
			this.fieldEffect1.InternalMaxValue = (decimal) max1*100;
			this.fieldEffect2.InternalMinValue = (decimal) min2*100;
			this.fieldEffect2.InternalMaxValue = (decimal) max2*100;

			this.fieldRadius.SetEnabled(this.isExtendedSize && enableRadius);
			this.fieldEffect1.SetEnabled(this.isExtendedSize && enable1);
			this.fieldEffect2.SetEnabled(this.isExtendedSize && enable2);
		}

		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldType == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
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
		
		// Le type a �t� chang�.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			// Met les valeurs par d�faut correspondant au type choisi.
			this.EnableWidgets();

			Properties.CornerType type = Properties.Corner.ConvType(this.fieldType.SelectedIndex);
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			Properties.Corner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
			this.fieldEffect1.InternalValue = (decimal) effect1*100;
			this.fieldEffect2.InternalValue = (decimal) effect2*100;

			this.OnChanged();
		}

		// Un champ a �t� chang�.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo			fieldType;
		protected TextFieldReal				fieldRadius;
		protected TextFieldReal				fieldEffect1;
		protected TextFieldReal				fieldEffect2;
		protected StaticText				labelRadius;
		protected StaticText				labelEffect1;
		protected StaticText				labelEffect2;
	}
}

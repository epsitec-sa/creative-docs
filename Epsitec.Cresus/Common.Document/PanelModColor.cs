using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PanelModColor permet de choisir une modification de couleur.
	/// </summary>
	[SuppressBundleSupport]
	public class PanelModColor : AbstractPanel
	{
		public PanelModColor(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;
			this.label.Text = "Transformation de couleur :";

			this.labelArray = new StaticText[7];
			for ( int i=0 ; i<7 ; i++ )
			{
				this.labelArray[i] = new StaticText(this);
				if ( i == 0 )  this.labelArray[i].Text = "T";
				if ( i == 1 )  this.labelArray[i].Text = "S";
				if ( i == 2 )  this.labelArray[i].Text = "L";
				if ( i == 3 )  this.labelArray[i].Text = "R";
				if ( i == 4 )  this.labelArray[i].Text = "V";
				if ( i == 5 )  this.labelArray[i].Text = "B";
				if ( i == 6 )  this.labelArray[i].Text = "A";
				this.labelArray[i].Alignment = ContentAlignment.MiddleCenter;
			}

			this.fieldArray = new TextFieldSlider[7];
			for ( int i=0 ; i<7 ; i++ )
			{
				this.fieldArray[i] = new TextFieldSlider(this);
				this.fieldArray[i].TextChanged += new EventHandler(this.HandleTextChanged);
				this.fieldArray[i].TabIndex = 1+i;
				this.fieldArray[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				if ( i == 0 )
				{
					this.fieldArray[i].MinValue = 0;
					this.fieldArray[i].MaxValue = 360;
				}
				else
				{
					this.fieldArray[i].MinValue = -100;
					this.fieldArray[i].MaxValue =  100;
				}
				this.fieldArray[i].Step = 5;
			}
			this.fieldArray[0].Color = Color.FromRGB(0,0,0);
			this.fieldArray[0].BackColor = Color.FromRGB(0.5,0.5,0.5);
			this.fieldArray[1].Color = Color.FromRGB(0,0,0);
			this.fieldArray[1].BackColor = Color.FromRGB(1,1,1);
			this.fieldArray[2].Color = Color.FromRGB(1,1,1);
			this.fieldArray[2].BackColor = Color.FromRGB(0,0,0);
			this.fieldArray[3].Color = Color.FromRGB(1,0,0);
			this.fieldArray[4].Color = Color.FromRGB(0,1,0);
			this.fieldArray[5].Color = Color.FromRGB(0,0,1);
			this.fieldArray[6].Color = Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fieldArray[0], "Teinte");
			ToolTip.Default.SetToolTip(this.fieldArray[1], "Saturation");
			ToolTip.Default.SetToolTip(this.fieldArray[2], "Luminosité");
			ToolTip.Default.SetToolTip(this.fieldArray[3], "Rouge");
			ToolTip.Default.SetToolTip(this.fieldArray[4], "Vert");
			ToolTip.Default.SetToolTip(this.fieldArray[5], "Bleu");
			ToolTip.Default.SetToolTip(this.fieldArray[6], "Alpha (transparence)");

			this.negativ = new CheckButton(this);
			this.negativ.Text = "Négatif";
			this.negativ.TabIndex = 10;
			this.negativ.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.negativ.ActiveStateChanged += new EventHandler(this.HandleNegativChanged);
			ToolTip.Default.SetToolTip(this.negativ, "Couleurs inversées");

			this.reset = new Button(this);
			this.reset.Text = "R";
			this.reset.TabIndex = 11;
			this.reset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += new MessageEventHandler(this.HandleReset);
			ToolTip.Default.SetToolTip(this.reset, "Reset (valeurs standards)");

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<7 ; i++ )
				{
					this.fieldArray[i].TextChanged -= new EventHandler(this.HandleTextChanged);
					this.fieldArray[i] = null;
					this.labelArray[i] = null;
				}
				this.negativ.ActiveStateChanged -= new EventHandler(this.HandleNegativChanged);
				this.reset.Clicked -= new MessageEventHandler(this.HandleReset);

				this.label = null;
				this.negativ = null;
				this.reset = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 99 : 30 );
			}
		}


		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			PropertyModColor p = this.property as PropertyModColor;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldArray[0].Value = (decimal) p.H;
			this.fieldArray[1].Value = (decimal) p.S*100;
			this.fieldArray[2].Value = (decimal) p.V*100;
			this.fieldArray[3].Value = (decimal) p.R*100;
			this.fieldArray[4].Value = (decimal) p.G*100;
			this.fieldArray[5].Value = (decimal) p.B*100;
			this.fieldArray[6].Value = (decimal) p.A*100;
			this.negativ.ActiveState = p.N ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			PropertyModColor p = this.property as PropertyModColor;
			if ( p == null )  return;

			p.H = (double) this.fieldArray[0].Value;
			p.S = (double) this.fieldArray[1].Value/100;
			p.V = (double) this.fieldArray[2].Value/100;
			p.R = (double) this.fieldArray[3].Value/100;
			p.G = (double) this.fieldArray[4].Value/100;
			p.B = (double) this.fieldArray[5].Value/100;
			p.A = (double) this.fieldArray[6].Value/100;
			p.N = (this.negativ.ActiveState & WidgetState.ActiveYes) != 0;
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			for ( int i=0 ; i<7 ; i++ )
			{
				this.labelArray[i].SetVisible(this.isExtendedSize);
				this.fieldArray[i].SetVisible(this.isExtendedSize);
			}
			this.negativ.SetVisible(this.isExtendedSize);
			this.reset.SetVisible(this.isExtendedSize);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldArray == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-14;
			this.label.Bounds = r;

			rect.Top -= 20;
			rect.Bottom = rect.Top-20;
			for ( int j=0 ; j<3 ; j++ )
			{
				r = rect;
				r.Left += 1;
				for ( int i=0 ; i<3 ; i++ )
				{
					if ( j*3+i >= 7 )  continue;
					r.Width = 13;
					this.labelArray[j*3+i].Bounds = r;
					r.Left = r.Right;
					r.Width = 45;
					this.fieldArray[j*3+i].Bounds = r;
					r.Left = r.Right;
				}
				rect.Offset(0, -24);
			}

			r.Left = rect.Left+72;
			r.Right = rect.Right-24;
			this.negativ.Bounds = r;

			r.Left = r.Right;
			r.Width = 24;
			this.reset.Bounds = r;
		}
		
		// Couleur -> sliders.
		protected void ColoriseSliders()
		{
			double h = (double) this.fieldArray[0].Value;
			Color saturated = Color.FromHSV(h,1,1);
			
			this.fieldArray[0].Color = saturated;
			this.fieldArray[1].Color = saturated;
			this.fieldArray[2].Color = saturated;
		}

		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.ColoriseSliders();
			this.OnChanged();
		}

		private void HandleNegativChanged(object sender)
		{
			this.OnChanged();
		}

		private void HandleReset(object sender, MessageEventArgs e)
		{
			this.fieldArray[0].Value = 0.0M;
			this.fieldArray[1].Value = 0.0M;
			this.fieldArray[2].Value = 0.0M;
			this.fieldArray[3].Value = 0.0M;
			this.fieldArray[4].Value = 0.0M;
			this.fieldArray[5].Value = 0.0M;
			this.fieldArray[6].Value = 0.0M;
			this.negativ.ActiveState = WidgetState.ActiveNo;
		}

		protected StaticText				label;
		protected StaticText[]				labelArray;
		protected TextFieldSlider[]			fieldArray;
		protected CheckButton				negativ;
		protected Button					reset;
	}
}

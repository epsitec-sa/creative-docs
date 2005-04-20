using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe ModColor permet de choisir une modification de couleur.
	/// </summary>
	[SuppressBundleSupport]
	public class ModColor : Abstract
	{
		public ModColor(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;
			this.label.Text = Res.Strings.Panel.ModColor.Label.Title;

			this.labelArray = new StaticText[7];
			for ( int i=0 ; i<7 ; i++ )
			{
				this.labelArray[i] = new StaticText(this);
				if ( i == 0 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.H;
				if ( i == 1 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.S;
				if ( i == 2 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.V;
				if ( i == 3 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.R;
				if ( i == 4 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.G;
				if ( i == 5 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.B;
				if ( i == 6 )  this.labelArray[i].Text = Res.Strings.Panel.ModColor.Label.A;
				this.labelArray[i].Alignment = ContentAlignment.MiddleCenter;
			}

			this.fieldArray = new TextFieldReal[7];
			for ( int i=0 ; i<7 ; i++ )
			{
				this.fieldArray[i] = new TextFieldReal(this);
				this.fieldArray[i].ValueChanged += new EventHandler(this.HandleValueChanged);
				this.fieldArray[i].TabIndex = 1+i;
				this.fieldArray[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				if ( i == 0 )
				{
					this.document.Modifier.AdaptTextFieldRealAngle(this.fieldArray[i]);
				}
				else
				{
					this.document.Modifier.AdaptTextFieldRealScalar(this.fieldArray[i]);
					this.fieldArray[i].InternalMinValue = -100;
					this.fieldArray[i].InternalMaxValue =  100;
				}
				this.fieldArray[i].Step = 5;
			}
			this.fieldArray[0].Color = Drawing.Color.FromRGB(0,0,0);
			this.fieldArray[0].BackColor = Drawing.Color.FromRGB(0.5,0.5,0.5);
			this.fieldArray[1].Color = Drawing.Color.FromRGB(0,0,0);
			this.fieldArray[1].BackColor = Drawing.Color.FromRGB(1,1,1);
			this.fieldArray[2].Color = Drawing.Color.FromRGB(1,1,1);
			this.fieldArray[2].BackColor = Drawing.Color.FromRGB(0,0,0);
			this.fieldArray[3].Color = Drawing.Color.FromRGB(1,0,0);
			this.fieldArray[4].Color = Drawing.Color.FromRGB(0,1,0);
			this.fieldArray[5].Color = Drawing.Color.FromRGB(0,0,1);
			this.fieldArray[6].Color = Drawing.Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fieldArray[0], Res.Strings.Panel.ModColor.Tooltip.H);
			ToolTip.Default.SetToolTip(this.fieldArray[1], Res.Strings.Panel.ModColor.Tooltip.S);
			ToolTip.Default.SetToolTip(this.fieldArray[2], Res.Strings.Panel.ModColor.Tooltip.V);
			ToolTip.Default.SetToolTip(this.fieldArray[3], Res.Strings.Panel.ModColor.Tooltip.R);
			ToolTip.Default.SetToolTip(this.fieldArray[4], Res.Strings.Panel.ModColor.Tooltip.G);
			ToolTip.Default.SetToolTip(this.fieldArray[5], Res.Strings.Panel.ModColor.Tooltip.B);
			ToolTip.Default.SetToolTip(this.fieldArray[6], Res.Strings.Panel.ModColor.Tooltip.A);

			this.negativ = new CheckButton(this);
			this.negativ.Text = Res.Strings.Panel.ModColor.Label.Negativ;
			this.negativ.TabIndex = 10;
			this.negativ.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.negativ.ActiveStateChanged += new EventHandler(this.HandleNegativChanged);
			ToolTip.Default.SetToolTip(this.negativ, Res.Strings.Panel.ModColor.Tooltip.Negativ);

			this.draft = new Button(this);
			this.draft.Text = Res.Strings.Panel.ModColor.Label.Draft;
			this.draft.TabIndex = 11;
			this.draft.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.draft.Clicked += new MessageEventHandler(this.HandleDraft);
			ToolTip.Default.SetToolTip(this.draft, Res.Strings.Panel.ModColor.Tooltip.Draft);

			this.reset = new Button(this);
			this.reset.Text = Res.Strings.Panel.ModColor.Label.Reset;
			this.reset.TabIndex = 12;
			this.reset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += new MessageEventHandler(this.HandleReset);
			ToolTip.Default.SetToolTip(this.reset, Res.Strings.Panel.ModColor.Tooltip.Reset);

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<7 ; i++ )
				{
					this.fieldArray[i].ValueChanged -= new EventHandler(this.HandleValueChanged);
					this.fieldArray[i] = null;
					this.labelArray[i] = null;
				}
				this.negativ.ActiveStateChanged -= new EventHandler(this.HandleNegativChanged);
				this.draft.Clicked -= new MessageEventHandler(this.HandleDraft);
				this.reset.Clicked -= new MessageEventHandler(this.HandleReset);

				this.label = null;
				this.negativ = null;
				this.draft = null;
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

			Properties.ModColor p = this.property as Properties.ModColor;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldArray[0].InternalValue = (decimal) p.H;
			this.fieldArray[1].InternalValue = (decimal) p.S*100;
			this.fieldArray[2].InternalValue = (decimal) p.V*100;
			this.fieldArray[3].InternalValue = (decimal) p.R*100;
			this.fieldArray[4].InternalValue = (decimal) p.G*100;
			this.fieldArray[5].InternalValue = (decimal) p.B*100;
			this.fieldArray[6].InternalValue = (decimal) p.A*100;
			this.negativ.ActiveState = p.N ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.ModColor p = this.property as Properties.ModColor;
			if ( p == null )  return;

			p.H = (double) this.fieldArray[0].InternalValue;
			p.S = (double) this.fieldArray[1].InternalValue/100;
			p.V = (double) this.fieldArray[2].InternalValue/100;
			p.R = (double) this.fieldArray[3].InternalValue/100;
			p.G = (double) this.fieldArray[4].InternalValue/100;
			p.B = (double) this.fieldArray[5].InternalValue/100;
			p.A = (double) this.fieldArray[6].InternalValue/100;
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
			this.draft.SetVisible(this.isExtendedSize);
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
			r.Right = rect.Right-24-1-24;
			this.negativ.Bounds = r;

			r.Left = r.Right;
			r.Width = 24;
			this.draft.Bounds = r;

			r.Left = r.Right+1;
			r.Width = 24;
			this.reset.Bounds = r;
		}
		
		// Couleur -> sliders.
		protected void ColoriseSliders()
		{
			double h = (double) this.fieldArray[0].InternalValue;
			Drawing.Color saturated = Drawing.Color.FromHSV(h,1,1);
			
			this.fieldArray[0].Color = saturated;
			this.fieldArray[1].Color = saturated;
			this.fieldArray[2].Color = saturated;
		}

		// Une valeur a été changée.
		private void HandleValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.ColoriseSliders();
			this.OnChanged();
		}

		private void HandleNegativChanged(object sender)
		{
			this.OnChanged();
		}

		private void HandleDraft(object sender, MessageEventArgs e)
		{
			this.fieldArray[0].InternalValue =    0.0M;  // T
			this.fieldArray[1].InternalValue = -100.0M;  // S
			this.fieldArray[2].InternalValue =    0.0M;  // L
			this.fieldArray[3].InternalValue =  100.0M;  // R
			this.fieldArray[4].InternalValue =    0.0M;  // V
			this.fieldArray[5].InternalValue =    0.0M;  // B
			this.fieldArray[6].InternalValue =  -75.0M;  // A
			this.negativ.ActiveState = WidgetState.ActiveNo;
		}

		private void HandleReset(object sender, MessageEventArgs e)
		{
			this.fieldArray[0].InternalValue = 0.0M;
			this.fieldArray[1].InternalValue = 0.0M;
			this.fieldArray[2].InternalValue = 0.0M;
			this.fieldArray[3].InternalValue = 0.0M;
			this.fieldArray[4].InternalValue = 0.0M;
			this.fieldArray[5].InternalValue = 0.0M;
			this.fieldArray[6].InternalValue = 0.0M;
			this.negativ.ActiveState = WidgetState.ActiveNo;
		}

		protected StaticText				label;
		protected StaticText[]				labelArray;
		protected TextFieldReal[]			fieldArray;
		protected CheckButton				negativ;
		protected Button					reset;
		protected Button					draft;
	}
}

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe ModColor permet de choisir une modification de couleur.
	/// </summary>
	public class ModColor : Abstract
	{
		public ModColor(Document document) : base(document)
		{
			this.fieldArray = new Widgets.TextFieldLabel[7];
			for ( int i=0 ; i<7 ; i++ )
			{
				this.fieldArray[i] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);

				if ( i == 0 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.H;
				if ( i == 1 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.S;
				if ( i == 2 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.V;
				if ( i == 3 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.R;
				if ( i == 4 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.G;
				if ( i == 5 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.B;
				if ( i == 6 )  this.fieldArray[i].LabelShortText = Res.Strings.Panel.ModColor.Short.A;
				
				this.fieldArray[i].TextFieldReal.EditionAccepted += this.HandleValueChanged;
				this.fieldArray[i].TabIndex = 1+i;
				this.fieldArray[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				if ( i == 0 )
				{
					this.document.Modifier.AdaptTextFieldRealAngle(this.fieldArray[i].TextFieldReal);
				}
				else
				{
					this.document.Modifier.AdaptTextFieldRealScalar(this.fieldArray[i].TextFieldReal);
					this.fieldArray[i].TextFieldReal.InternalMinValue = -100;
					this.fieldArray[i].TextFieldReal.InternalMaxValue =  100;
					this.fieldArray[i].TextFieldReal.TextSuffix = "%";
				}
				this.fieldArray[i].TextFieldReal.Step = 5;
			}
			this.fieldArray[0].TextFieldReal.Color = Drawing.Color.FromRgb(0,0,0);
			this.fieldArray[0].TextFieldReal.BackColor = Drawing.Color.FromRgb(0.5,0.5,0.5);
			this.fieldArray[1].TextFieldReal.Color = Drawing.Color.FromRgb(0,0,0);
			this.fieldArray[1].TextFieldReal.BackColor = Drawing.Color.FromRgb(1,1,1);
			this.fieldArray[2].TextFieldReal.Color = Drawing.Color.FromRgb(1,1,1);
			this.fieldArray[2].TextFieldReal.BackColor = Drawing.Color.FromRgb(0,0,0);
			this.fieldArray[3].TextFieldReal.Color = Drawing.Color.FromRgb(1,0,0);
			this.fieldArray[4].TextFieldReal.Color = Drawing.Color.FromRgb(0,1,0);
			this.fieldArray[5].TextFieldReal.Color = Drawing.Color.FromRgb(0,0,1);
			this.fieldArray[6].TextFieldReal.Color = Drawing.Color.FromRgb(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fieldArray[0], Res.Strings.Panel.ModColor.Tooltip.H);
			ToolTip.Default.SetToolTip(this.fieldArray[1], Res.Strings.Panel.ModColor.Tooltip.S);
			ToolTip.Default.SetToolTip(this.fieldArray[2], Res.Strings.Panel.ModColor.Tooltip.V);
			ToolTip.Default.SetToolTip(this.fieldArray[3], Res.Strings.Panel.ModColor.Tooltip.R);
			ToolTip.Default.SetToolTip(this.fieldArray[4], Res.Strings.Panel.ModColor.Tooltip.G);
			ToolTip.Default.SetToolTip(this.fieldArray[5], Res.Strings.Panel.ModColor.Tooltip.B);
			ToolTip.Default.SetToolTip(this.fieldArray[6], Res.Strings.Panel.ModColor.Tooltip.A);

			this.negativ = new CheckButton(this);
			this.negativ.Text = Res.Strings.Panel.ModColor.Short.Negativ;
			this.negativ.TabIndex = 10;
			this.negativ.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.negativ.ActiveStateChanged += this.HandleNegativChanged;
			ToolTip.Default.SetToolTip(this.negativ, Res.Strings.Panel.ModColor.Tooltip.Negativ);

			this.draft = new Button(this);
			this.draft.Text = Res.Strings.Panel.ModColor.Short.Draft;
			this.draft.TabIndex = 11;
			this.draft.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.draft.Clicked += this.HandleDraft;
			ToolTip.Default.SetToolTip(this.draft, Res.Strings.Panel.ModColor.Tooltip.Draft);

			this.reset = new Button(this);
			this.reset.Text = Res.Strings.Panel.ModColor.Button.Reset;
			this.reset.TabIndex = 12;
			this.reset.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.reset.Clicked += this.HandleReset;
			ToolTip.Default.SetToolTip(this.reset, Res.Strings.Panel.ModColor.Tooltip.Reset);

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<7 ; i++ )
				{
					this.fieldArray[i].TextFieldReal.EditionAccepted -= this.HandleValueChanged;
					this.fieldArray[i] = null;
				}
				this.negativ.ActiveStateChanged -= this.HandleNegativChanged;
				this.draft.Clicked -= this.HandleDraft;
				this.reset.Clicked -= this.HandleReset;

				this.negativ = null;
				this.draft = null;
				this.reset = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+79 : this.LabelHeight+30 );
			}
		}


		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.ModColor p = this.property as Properties.ModColor;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldArray[0].TextFieldReal.InternalValue = (decimal) p.H;
			this.fieldArray[1].TextFieldReal.InternalValue = (decimal) p.S*100;
			this.fieldArray[2].TextFieldReal.InternalValue = (decimal) p.V*100;
			this.fieldArray[3].TextFieldReal.InternalValue = (decimal) p.R*100;
			this.fieldArray[4].TextFieldReal.InternalValue = (decimal) p.G*100;
			this.fieldArray[5].TextFieldReal.InternalValue = (decimal) p.B*100;
			this.fieldArray[6].TextFieldReal.InternalValue = (decimal) p.A*100;
			this.negativ.ActiveState = p.N ? ActiveState.Yes : ActiveState.No;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.ModColor p = this.property as Properties.ModColor;
			if ( p == null )  return;

			p.H = (double) this.fieldArray[0].TextFieldReal.InternalValue;
			p.S = (double) this.fieldArray[1].TextFieldReal.InternalValue/100;
			p.V = (double) this.fieldArray[2].TextFieldReal.InternalValue/100;
			p.R = (double) this.fieldArray[3].TextFieldReal.InternalValue/100;
			p.G = (double) this.fieldArray[4].TextFieldReal.InternalValue/100;
			p.B = (double) this.fieldArray[5].TextFieldReal.InternalValue/100;
			p.A = (double) this.fieldArray[6].TextFieldReal.InternalValue/100;
			p.N = (this.negativ.ActiveState & ActiveState.Yes) != 0;
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			for ( int i=0 ; i<7 ; i++ )
			{
				this.fieldArray[i].Visibility = (this.isExtendedSize);
			}
			this.negativ.Visibility = (this.isExtendedSize);
			this.draft.Visibility = (this.isExtendedSize);
			this.reset.Visibility = (this.isExtendedSize);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldArray == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			rect.Bottom = rect.Top-20;
			for ( int j=0 ; j<3 ; j++ )
			{
				r = rect;
				for ( int i=0 ; i<3 ; i++ )
				{
					if ( j*3+i >= 7 )  continue;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldArray[j*3+i].SetManualBounds(r);
					r.Left = r.Right;
				}
				rect.Offset(0, -24);
			}

			r.Left = rect.Left+Widgets.TextFieldLabel.ShortWidth+Widgets.TextFieldLabel.DefaultLabelWidth+Widgets.TextFieldLabel.DefaultMarginWidth;
			r.Right = rect.Right-24-1-24;
			this.negativ.SetManualBounds(r);

			r.Left = r.Right;
			r.Width = 24;
			this.draft.SetManualBounds(r);

			r.Left = r.Right+1;
			r.Width = 24;
			this.reset.SetManualBounds(r);
		}
		
		protected void ColoriseSliders()
		{
			//	Couleur -> sliders.
			double h = (double) this.fieldArray[0].TextFieldReal.InternalValue;
			Drawing.Color saturated = Drawing.Color.FromHsv(h,1,1);
			
			this.fieldArray[0].TextFieldReal.Color = saturated;
			this.fieldArray[1].TextFieldReal.Color = saturated;
			this.fieldArray[2].TextFieldReal.Color = saturated;
		}

		private void HandleValueChanged(object sender)
		{
			//	Une valeur a été changée.
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
			this.fieldArray[0].TextFieldReal.InternalValue =    0.0M;  // T
			this.fieldArray[1].TextFieldReal.InternalValue = -100.0M;  // S
			this.fieldArray[2].TextFieldReal.InternalValue =    0.0M;  // L
			this.fieldArray[3].TextFieldReal.InternalValue =  100.0M;  // R
			this.fieldArray[4].TextFieldReal.InternalValue =    0.0M;  // V
			this.fieldArray[5].TextFieldReal.InternalValue =    0.0M;  // B
			this.fieldArray[6].TextFieldReal.InternalValue =  -75.0M;  // A
			this.negativ.ActiveState = ActiveState.No;
			this.OnChanged();
		}

		private void HandleReset(object sender, MessageEventArgs e)
		{
			this.fieldArray[0].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[1].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[2].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[3].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[4].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[5].TextFieldReal.InternalValue = 0.0M;
			this.fieldArray[6].TextFieldReal.InternalValue = 0.0M;
			this.negativ.ActiveState = ActiveState.No;
			this.OnChanged();
		}

		protected Widgets.TextFieldLabel[]	fieldArray;
		protected CheckButton				negativ;
		protected Button					reset;
		protected Button					draft;
	}
}

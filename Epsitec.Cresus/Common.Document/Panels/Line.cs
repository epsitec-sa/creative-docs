using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Line permet de choisir un mode de trait.
	/// </summary>
	[SuppressBundleSupport]
	public class Line : Abstract
	{
		public Line(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.field = new TextFieldReal(this);
			this.field.FactorMinRange = 0.0M;
			this.field.FactorMaxRange = 0.1M;
			this.field.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.field);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Epaisseur du trait");

			this.dash = new CheckButton(this);
			this.dash.ActiveStateChanged += new EventHandler(this.HandleDashActiveStateChanged);
			this.dash.Text = "Traitillé :";

			this.buttons = new IconButton[6];
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.PanelLineClicked);
				this.buttons[i].TabIndex = 3+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = @"file:images/capround.icon";
			this.buttons[1].IconName = @"file:images/capsquare.icon";
			this.buttons[2].IconName = @"file:images/capbutt.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], "Extrémité ronde");
			ToolTip.Default.SetToolTip(this.buttons[1], "Extrémité carrée");
			ToolTip.Default.SetToolTip(this.buttons[2], "Extrémité tronquée");

			this.buttons[3].IconName = @"file:images/joinround.icon";
			this.buttons[4].IconName = @"file:images/joinmiter.icon";
			this.buttons[5].IconName = @"file:images/joinbevel.icon";
			ToolTip.Default.SetToolTip(this.buttons[3], "Jointure ronde");
			ToolTip.Default.SetToolTip(this.buttons[4], "Jointure pointue");
			ToolTip.Default.SetToolTip(this.buttons[5], "Jointure tronquée");

			this.radioDashRank = new RadioButton[Properties.Line.DashMax];
			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i] = new RadioButton(this);
				this.radioDashRank[i].ActiveStateChanged += new EventHandler(this.HandleDashRankChanged);
				this.radioDashRank[i].TabIndex = 20+i;
				this.radioDashRank[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}
			this.radioDashRank[Properties.Line.DashMax-1].Text = ":";
			ToolTip.Default.SetToolTip(this.radioDashRank[0], "1er couple trait/trou");
			ToolTip.Default.SetToolTip(this.radioDashRank[1], "2ème couple trait/trou");
			ToolTip.Default.SetToolTip(this.radioDashRank[2], "3ème couple trait/trou");
			this.RadioSelected = 0;

			this.fieldDashPen = new TextFieldReal(this);
			this.fieldDashPen.FactorMinRange = 0.0M;
			this.fieldDashPen.FactorMaxRange = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashPen);
			this.fieldDashPen.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashPen.TabIndex = 32;
			this.fieldDashPen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashPen, "Longueur du trait");

			this.fieldDashGap = new TextFieldReal(this);
			this.fieldDashGap.FactorMinRange = 0.0M;
			this.fieldDashGap.FactorMaxRange = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashGap);
			this.fieldDashGap.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashGap.TabIndex = 22;
			this.fieldDashGap.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashGap, "Longueur du trou");

			this.labelDashPen = new StaticText(this);
			this.labelDashPen.Text = "P";
			this.labelDashPen.Alignment = ContentAlignment.MiddleRight;

			this.labelDashGap = new StaticText(this);
			this.labelDashGap.Text = "V";
			this.labelDashGap.Alignment = ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.dash.ActiveStateChanged -= new EventHandler(this.HandleDashActiveStateChanged);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.PanelLineClicked);
					this.buttons[i] = null;
				}

				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveStateChanged -= new EventHandler(this.HandleDashRankChanged);
					this.radioDashRank[i] = null;
				}
				this.fieldDashPen.TextChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashGap.TextChanged -= new EventHandler(this.HandleDashChanged);

				this.label = null;
				this.field = null;
				this.dash = null;
				this.fieldDashPen = null;
				this.fieldDashGap = null;
				this.labelDashPen = null;
				this.labelDashGap = null;
				this.label = null;
				this.label = null;
				this.label = null;
				this.label = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				if ( this.isExtendedSize )
				{
					return 105;
				}
				else
				{
					return 30;
				}
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;
			this.field.InternalValue = (decimal) p.Width;

			int sel = -1;
			if ( p.Cap == CapStyle.Round  )  sel = 0;
			if ( p.Cap == CapStyle.Square )  sel = 1;
			if ( p.Cap == CapStyle.Butt   )  sel = 2;
			this.SelectButtonCap = sel;

			sel = -1;
			if ( p.Join == JoinStyle.Round )  sel = 0;
			if ( p.Join == JoinStyle.Miter )  sel = 1;
			if ( p.Join == JoinStyle.Bevel )  sel = 2;
			this.SelectButtonJoin = sel;

			this.SelectDash = p.Dash;
			this.DashToWidget();

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			p.Width = (double) this.field.InternalValue;

			int sel = this.SelectButtonCap;
			if ( sel == 0 )  p.Cap = CapStyle.Round;
			if ( sel == 1 )  p.Cap = CapStyle.Square;
			if ( sel == 2 )  p.Cap = CapStyle.Butt;

			sel = this.SelectButtonJoin;
			if ( sel == 0 )  p.Join = JoinStyle.Round;
			if ( sel == 1 )  p.Join = JoinStyle.Miter;
			if ( sel == 2 )  p.Join = JoinStyle.Bevel;

			p.Dash = this.SelectDash;
			this.WidgetToDash();
		}

		protected void DashToWidget()
		{
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			int i = this.RadioSelected;
			this.fieldDashPen.InternalValue = (decimal) p.GetDashPen(i);
			this.fieldDashGap.InternalValue = (decimal) p.GetDashGap(i);
		}

		protected void WidgetToDash()
		{
			Properties.Line p = this.property as Properties.Line;
			if ( p == null )  return;

			int i = this.RadioSelected;
			p.SetDashPen(i, (double) this.fieldDashPen.InternalValue);
			p.SetDashGap(i, (double) this.fieldDashGap.InternalValue);
		}

		protected bool SelectDash
		{
			get
			{
				return ( this.dash.ActiveState == WidgetState.ActiveYes );
			}

			set
			{
				this.dash.ActiveState = value ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}

		protected int RadioSelected
		{
			get
			{
				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					if ( this.radioDashRank[i].ActiveState == WidgetState.ActiveYes )
					{
						return i;
					}
				}
				return 0;
			}

			set
			{
				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}

		protected int SelectButtonCap
		{
			get
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					if ( this.buttons[i].ActiveState == WidgetState.ActiveYes )  return i;
				}
				return -1;
			}

			set
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					this.buttons[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}

		protected int SelectButtonJoin
		{
			get
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					if ( this.buttons[i+3].ActiveState == WidgetState.ActiveYes )  return i;
				}
				return -1;
			}

			set
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					this.buttons[i+3].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			bool dash = (this.dash.ActiveState == WidgetState.ActiveYes) && this.isExtendedSize;

			this.dash.SetVisible(this.isExtendedSize);

			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i].SetVisible(this.isExtendedSize);
				this.buttons[i].SetEnabled(this.isExtendedSize);
			}

			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i].SetVisible(this.isExtendedSize);
				this.radioDashRank[i].SetEnabled(dash);
			}
			this.fieldDashPen.SetVisible(this.isExtendedSize);
			this.fieldDashPen.SetEnabled(dash);
			this.fieldDashGap.SetVisible(this.isExtendedSize);
			this.fieldDashGap.SetEnabled(dash);
			this.labelDashPen.SetVisible(this.isExtendedSize);
			this.labelDashPen.SetEnabled(dash);
			this.labelDashGap.SetVisible(this.isExtendedSize);
			this.labelDashGap.SetEnabled(dash);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttons == null )  return;

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
			this.field.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-(20*6+15);
			r.Width = 20;
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20+((i==2)?15:0), 0);
			}

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.dash.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 16;
			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				if ( i == Properties.Line.DashMax-1 )  r.Width = 32;
				this.radioDashRank[i].Bounds = r;
				r.Offset(r.Width, 0);
			}
			r.Left = rect.Right-116;
			r.Width = 12;
			this.labelDashPen.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldDashPen.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelDashGap.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldDashGap.Bounds = r;
		}

		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le bouton des traitillés a été cliqué.
		private void HandleDashActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void PanelLineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonCap = 0;
			if ( button == this.buttons[1] )  this.SelectButtonCap = 1;
			if ( button == this.buttons[2] )  this.SelectButtonCap = 2;

			if ( button == this.buttons[3] )  this.SelectButtonJoin = 0;
			if ( button == this.buttons[4] )  this.SelectButtonJoin = 1;
			if ( button == this.buttons[5] )  this.SelectButtonJoin = 2;

			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le rang a été changé.
		private void HandleDashRankChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.ignoreChanged = true;
			this.DashToWidget();
			this.ignoreChanged = false;
		}

		// Un champ a été changé.
		private void HandleDashChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldReal				field;
		protected CheckButton				dash;
		protected IconButton[]				buttons;
		protected RadioButton[]				radioDashRank;
		protected TextFieldReal				fieldDashPen;
		protected TextFieldReal				fieldDashGap;
		protected StaticText				labelDashPen;
		protected StaticText				labelDashGap;
	}
}

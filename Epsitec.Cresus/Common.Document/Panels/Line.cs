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

			this.nothingButton = new IconButton(this);
			this.nothingButton.Clicked += new MessageEventHandler(this.HandleNothingClicked);
			this.nothingButton.TabIndex = 1;
			this.nothingButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.nothingButton.IconName = "manifest:Epsitec.App.DocumentEditor.Images.Nothing.icon";
			ToolTip.Default.SetToolTip(this.nothingButton, "Aucun trait");

			this.field = new TextFieldReal(this);
			this.field.FactorMinRange = 0.0M;
			this.field.FactorMaxRange = 0.1M;
			this.field.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.field);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 2;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Epaisseur du trait");

			this.dashType = new TextFieldCombo(this);
			this.dashType.IsReadOnly = true;
			this.dashType.Items.Add("Plein");
			this.dashType.Items.Add("Traitillé");
			this.dashType.Items.Add("Traitillé serré");
			this.dashType.Items.Add("Traitillé étendu");
			this.dashType.Items.Add("Pointillé");
			this.dashType.Items.Add("Trait-pointillé");
			this.dashType.Items.Add("Trait-point-pointillé");
			this.dashType.Items.Add("Sur mesure");
			this.dashType.SelectedIndexChanged += new EventHandler(this.HandleListChanged);
			this.dashType.TabIndex = 3;
			this.dashType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.dashType, "Type du trait");

			this.buttons = new IconButton[6];
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.HandlePanelLineClicked);
				this.buttons[i].TabIndex = 4+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = "manifest:Epsitec.App.DocumentEditor.Images.CapRound.icon";
			this.buttons[1].IconName = "manifest:Epsitec.App.DocumentEditor.Images.CapSquare.icon";
			this.buttons[2].IconName = "manifest:Epsitec.App.DocumentEditor.Images.CapButt.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], "Extrémité ronde");
			ToolTip.Default.SetToolTip(this.buttons[1], "Extrémité carrée");
			ToolTip.Default.SetToolTip(this.buttons[2], "Extrémité tronquée");

			this.buttons[3].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JoinRound.icon";
			this.buttons[4].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JoinMiter.icon";
			this.buttons[5].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JoinBevel.icon";
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

			this.fieldStandardLength = new TextFieldReal(this);
			this.fieldStandardLength.FactorMinRange = 0.0M;
			this.fieldStandardLength.FactorMaxRange = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldStandardLength);
			this.fieldStandardLength.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldStandardLength.TabIndex = 20;
			this.fieldStandardLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldStandardLength, "Longueur du motif");

			this.fieldDashPen = new TextFieldReal(this);
			this.fieldDashPen.FactorMinRange = 0.0M;
			this.fieldDashPen.FactorMaxRange = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldDashPen);
			this.fieldDashPen.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashPen.TabIndex = 21;
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

			this.labelStandardLength = new StaticText(this);
			this.labelStandardLength.Text = "L";
			this.labelStandardLength.Alignment = ContentAlignment.MiddleRight;

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
				this.nothingButton.Clicked -= new MessageEventHandler(this.HandleNothingClicked);
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.dashType.SelectedIndexChanged -= new EventHandler(this.HandleListChanged);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandlePanelLineClicked);
					this.buttons[i] = null;
				}

				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveStateChanged -= new EventHandler(this.HandleDashRankChanged);
					this.radioDashRank[i] = null;
				}
				this.fieldStandardLength.TextChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashPen.TextChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashGap.TextChanged -= new EventHandler(this.HandleDashChanged);

				this.label = null;
				this.field = null;
				this.dashType = null;
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

			this.DashToWidget();
			this.SelectDash = p.StandardDash;
			this.fieldStandardLength.InternalValue = (decimal) p.StandardLength;

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

			this.WidgetToDash();
			p.StandardDash = this.SelectDash;
			p.StandardLength = (double) this.fieldStandardLength.InternalValue;
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

		protected Properties.StandardDashType SelectDash
		{
			get
			{
				int sel = this.dashType.SelectedIndex;
				if ( sel == 0 )  return Properties.StandardDashType.Full;
				if ( sel == 1 )  return Properties.StandardDashType.Line;
				if ( sel == 2 )  return Properties.StandardDashType.LineDense;
				if ( sel == 3 )  return Properties.StandardDashType.LineExpand;
				if ( sel == 4 )  return Properties.StandardDashType.Dot;
				if ( sel == 5 )  return Properties.StandardDashType.LineDot;
				if ( sel == 6 )  return Properties.StandardDashType.LineDotDot;
				if ( sel == 7 )  return Properties.StandardDashType.Custom;
				return Properties.StandardDashType.Full;
			}

			set
			{
				if ( value == Properties.StandardDashType.Full       )  this.dashType.SelectedIndex = 0;
				if ( value == Properties.StandardDashType.Line       )  this.dashType.SelectedIndex = 1;
				if ( value == Properties.StandardDashType.LineDense  )  this.dashType.SelectedIndex = 2;
				if ( value == Properties.StandardDashType.LineExpand )  this.dashType.SelectedIndex = 3;
				if ( value == Properties.StandardDashType.Dot        )  this.dashType.SelectedIndex = 4;
				if ( value == Properties.StandardDashType.LineDot    )  this.dashType.SelectedIndex = 5;
				if ( value == Properties.StandardDashType.LineDotDot )  this.dashType.SelectedIndex = 6;
				if ( value == Properties.StandardDashType.Custom     )  this.dashType.SelectedIndex = 7;
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
			int sel = this.dashType.SelectedIndex;
			bool dash = (sel >= 1 && sel <= 4);
			bool user = (sel == 5);

			this.dashType.SetVisible(this.isExtendedSize);

			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i].SetVisible(this.isExtendedSize);
				this.buttons[i].SetEnabled(this.isExtendedSize);
			}

			for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
			{
				this.radioDashRank[i].SetVisible(this.isExtendedSize);
				this.radioDashRank[i].SetEnabled(user);
			}

			this.fieldStandardLength.SetVisible(this.isExtendedSize);
			this.fieldStandardLength.SetEnabled(dash);
			this.labelStandardLength.SetVisible(this.isExtendedSize);
			this.labelStandardLength.SetEnabled(dash);

			this.fieldDashPen.SetVisible(this.isExtendedSize);
			this.fieldDashPen.SetEnabled(user);
			this.fieldDashGap.SetVisible(this.isExtendedSize);
			this.fieldDashGap.SetEnabled(user);
			this.labelDashPen.SetVisible(this.isExtendedSize);
			this.labelDashPen.SetEnabled(user);
			this.labelDashGap.SetVisible(this.isExtendedSize);
			this.labelDashGap.SetEnabled(user);
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
			r.Right = rect.Right-70;
			this.label.Bounds = r;
			r.Left = rect.Right-70;
			r.Right = rect.Right-50;
			this.nothingButton.Bounds = r;
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
			r.Right = rect.Left+117;
			this.dashType.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelStandardLength.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldStandardLength.Bounds = r;

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

		// Le bouton "aucun trait" a été cliqué.
		private void HandleNothingClicked(object sender, MessageEventArgs e)
		{
			this.field.Value = 0.0M;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandlePanelLineClicked(object sender, MessageEventArgs e)
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

		// Le type dans la liste a changé.
		private void HandleListChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
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
		protected IconButton				nothingButton;
		protected TextFieldReal				field;
		protected TextFieldCombo			dashType;
		protected IconButton[]				buttons;
		protected RadioButton[]				radioDashRank;
		protected TextFieldReal				fieldStandardLength;
		protected TextFieldReal				fieldDashPen;
		protected TextFieldReal				fieldDashGap;
		protected StaticText				labelStandardLength;
		protected StaticText				labelDashPen;
		protected StaticText				labelDashGap;
	}
}

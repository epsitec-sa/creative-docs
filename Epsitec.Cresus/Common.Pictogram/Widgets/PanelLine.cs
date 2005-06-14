using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelLine permet de choisir un mode de trait.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelLine : AbstractPanel
	{
		public PanelLine(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.separators = new Separator[5];
			for ( int i=0 ; i<5 ; i++ )
			{
				this.separators[i] = new Separator(this);
				this.separators[i].Alpha = 0.5;
			}

			this.field = new TextFieldSlider(this);
			this.field.MinValue = 0;
			this.field.MaxValue = 5;
			this.field.Step = 0.1M;
			this.field.Resolution = 0.01M;
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Epaisseur du trait");

			this.patternLabel = new StaticText(this);
			this.patternLabel.Alignment = Drawing.ContentAlignment.MiddleLeft;
			this.patternLabel.Text = "Motif";

			this.patternId = new TextFieldCombo(this);
			this.patternId.IsReadOnly = true;
			this.patternId.SelectedIndexChanged += new EventHandler(this.HandlePatternIdChanged);
			this.patternId.TabIndex = 2;
			this.patternId.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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

			this.rndMove = new CheckButton(this);
			this.rndMove.ActiveStateChanged += new EventHandler(this.HandleRndChanged);
			this.rndMove.TabIndex = 10;
			this.rndMove.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.rndMove, "Valeurs aléatoires");

			this.fieldMove = new TextFieldSlider(this);
			this.fieldMove.MinValue = -75;
			this.fieldMove.MaxValue = 100;
			this.fieldMove.Step = 5.0M;
			this.fieldMove.Resolution = 1.0M;
			this.fieldMove.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldMove.TabIndex = 11;
			this.fieldMove.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMove, "Déplacement le long du trait");

			this.rndShift = new CheckButton(this);
			this.rndShift.ActiveStateChanged += new EventHandler(this.HandleRndChanged);
			this.rndShift.TabIndex = 12;
			this.rndShift.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.rndShift, "Valeurs aléatoires");

			this.fieldShift = new TextFieldSlider(this);
			this.fieldShift.MinValue = -100;
			this.fieldShift.MaxValue =  100;
			this.fieldShift.Step = 5.0M;
			this.fieldShift.Resolution = 1.0M;
			this.fieldShift.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldShift.TabIndex = 13;
			this.fieldShift.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldShift, "Déplacement perpendiculaire au trait");

			this.rndSize = new CheckButton(this);
			this.rndSize.ActiveStateChanged += new EventHandler(this.HandleRndChanged);
			this.rndSize.TabIndex = 14;
			this.rndSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.rndSize, "Valeurs aléatoires");

			this.fieldSize = new TextFieldSlider(this);
			this.fieldSize.MinValue = 100;
			this.fieldSize.MaxValue = 200;
			this.fieldSize.Step = 5.0M;
			this.fieldSize.Resolution = 1.0M;
			this.fieldSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldSize.TabIndex = 15;
			this.fieldSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldSize, "Taille du motif");

			this.rndAngle = new CheckButton(this);
			this.rndAngle.ActiveStateChanged += new EventHandler(this.HandleRndChanged);
			this.rndAngle.TabIndex = 16;
			this.rndAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.rndAngle, "Valeurs aléatoires");

			this.fieldAngle = new TextFieldSlider(this);
			this.fieldAngle.MinValue = -180;
			this.fieldAngle.MaxValue =  180;
			this.fieldAngle.Step = 5.0M;
			this.fieldAngle.Resolution = 1.0M;
			this.fieldAngle.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldAngle.TabIndex = 17;
			this.fieldAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldAngle, "Inclinaison du motif");

			this.labelMove = new StaticText(this);
			this.labelMove.Text = "X";
			this.labelMove.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelSize = new StaticText(this);
			this.labelSize.Text = "Z";
			this.labelSize.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelShift = new StaticText(this);
			this.labelShift.Text = "Y";
			this.labelShift.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelAngle = new StaticText(this);
			this.labelAngle.Text = "A";
			this.labelAngle.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.rndPage = new CheckButton(this);
			this.rndPage.Text = "P";
			this.rndPage.ActiveStateChanged += new EventHandler(this.HandleRndChanged);
			this.rndPage.TabIndex = 18;
			this.rndPage.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.rndPage, "Pages aléatoires");

			this.buttonSeed = new Button(this);
			this.buttonSeed.Text = "H";
			this.buttonSeed.Clicked += new MessageEventHandler(this.HandleSeedClicked);
			this.buttonSeed.TabIndex = 19;
			this.buttonSeed.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonSeed, "Autres valeurs aléatoires");

			this.radioDashRank = new RadioButton[PropertyLine.DashMax];
			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				this.radioDashRank[i] = new RadioButton(this);
				this.radioDashRank[i].ActiveStateChanged += new EventHandler(this.HandleDashRankChanged);
				this.radioDashRank[i].TabIndex = 20+i;
				this.radioDashRank[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}
			this.radioDashRank[PropertyLine.DashMax-1].Text = ":";
			ToolTip.Default.SetToolTip(this.radioDashRank[0], "1er couple trait/trou");
			ToolTip.Default.SetToolTip(this.radioDashRank[1], "2ème couple trait/trou");
			ToolTip.Default.SetToolTip(this.radioDashRank[2], "3ème couple trait/trou");

			this.fieldDashPen = new TextFieldSlider(this);
			this.fieldDashPen.MinValue =  0;
			this.fieldDashPen.MaxValue = 10;
			this.fieldDashPen.Step = 0.1M;
			this.fieldDashPen.Resolution = 0.1M;
			this.fieldDashPen.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashPen.TabIndex = 32;
			this.fieldDashPen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashPen, "Longueur du trait");

			this.fieldDashGap = new TextFieldSlider(this);
			this.fieldDashGap.MinValue =  0;
			this.fieldDashGap.MaxValue = 10;
			this.fieldDashGap.Step = 0.1M;
			this.fieldDashGap.Resolution = 0.1M;
			this.fieldDashGap.TextChanged += new EventHandler(this.HandleDashChanged);
			this.fieldDashGap.TabIndex = 22;
			this.fieldDashGap.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDashGap, "Longueur du trou");

			this.labelDashPen = new StaticText(this);
			this.labelDashPen.Text = "P";
			this.labelDashPen.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.labelDashGap = new StaticText(this);
			this.labelDashGap.Text = "V";
			this.labelDashGap.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.dashPen = new double[PropertyLine.DashMax];
			this.dashGap = new double[PropertyLine.DashMax];
			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				this.dashPen[i] = 0.0;
				this.dashGap[i] = 0.0;
			}

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.patternId.SelectedIndexChanged -= new EventHandler(this.HandlePatternIdChanged);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.PanelLineClicked);
					this.buttons[i] = null;
				}

				this.rndMove.ActiveStateChanged -= new EventHandler(this.HandleRndChanged);
				this.rndSize.ActiveStateChanged -= new EventHandler(this.HandleRndChanged);
				this.rndShift.ActiveStateChanged -= new EventHandler(this.HandleRndChanged);
				this.rndAngle.ActiveStateChanged -= new EventHandler(this.HandleRndChanged);
				this.rndPage.ActiveStateChanged -= new EventHandler(this.HandleRndChanged);
				this.fieldMove.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldSize.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldShift.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldAngle.TextChanged -= new EventHandler(this.HandleFieldChanged);

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveStateChanged -= new EventHandler(this.HandleDashRankChanged);
					this.radioDashRank[i] = null;
				}
				this.fieldDashPen.TextChanged -= new EventHandler(this.HandleDashChanged);
				this.fieldDashGap.TextChanged -= new EventHandler(this.HandleDashChanged);

				for ( int i=0 ; i<5 ; i++ )
				{
					this.separators[i] = null;
				}

				this.label = null;
				this.field = null;
				this.patternLabel = null;
				this.patternId = null;
				this.rndMove = null;
				this.rndSize = null;
				this.rndShift = null;
				this.rndAngle = null;
				this.rndPage = null;
				this.fieldMove = null;
				this.fieldSize = null;
				this.fieldShift = null;
				this.fieldAngle = null;
				this.labelMove = null;
				this.labelSize = null;
				this.labelShift = null;
				this.labelAngle = null;
				this.buttonSeed = null;
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
				if ( this.extendedSize )
				{
					return this.IsPatternPossible() ? 105 : 55;
				}
				else
				{
					return 30;
				}
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyLine p = property as PropertyLine;
			if ( p == null )  return;

			this.field.Value = (decimal) p.Width;
			this.UpdatePatternId();
			this.patternId.SelectedIndex = this.IdToSel(p.PatternId);

			int sel = -1;
			if ( p.Cap == Drawing.CapStyle.Round  )  sel = 0;
			if ( p.Cap == Drawing.CapStyle.Square )  sel = 1;
			if ( p.Cap == Drawing.CapStyle.Butt   )  sel = 2;
			this.SelectButtonCap = sel;

			sel = -1;
			if ( p.Join == Drawing.JoinStyle.Round       )  sel = 0;
			if ( p.Join == Drawing.JoinStyle.MiterRevert )  sel = 1;
			if ( p.Join == Drawing.JoinStyle.Bevel       )  sel = 2;
			this.SelectButtonJoin = sel;

			this.rndMove.ActiveState  = p.RndMove  ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.rndSize.ActiveState  = p.RndSize  ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.rndShift.ActiveState = p.RndShift ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.rndAngle.ActiveState = p.RndAngle ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.rndPage.ActiveState  = p.RndPage  ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.fieldMove.Value  = (decimal) p.PatternMove*100;
			this.fieldSize.Value  = (decimal) p.PatternSize*100;
			this.fieldShift.Value = (decimal) p.PatternShift*100;
			this.fieldAngle.Value = (decimal) p.PatternAngle;
			this.patternSeed = p.PatternSeed;

			this.dashRank = 0;
			this.RadioSelected = this.dashRank;
			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				this.dashPen[i] = p.GetDashPen(i);
				this.dashGap[i] = p.GetDashGap(i);
			}
			this.DashToWidget();

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyLine p = new PropertyLine();
			base.GetProperty(p);

			p.Width = (double) this.field.Value;
			p.PatternId = this.SelToId(this.patternId.SelectedIndex);
			p.PatternBbox = this.drawer.IconObjects.RetPatternBbox(p.PatternId);

			int sel = this.SelectButtonCap;
			if ( sel == 0 )  p.Cap = Drawing.CapStyle.Round;
			if ( sel == 1 )  p.Cap = Drawing.CapStyle.Square;
			if ( sel == 2 )  p.Cap = Drawing.CapStyle.Butt;

			sel = this.SelectButtonJoin;
			if ( sel == 0 )  p.Join = Drawing.JoinStyle.Round;
			if ( sel == 1 )  p.Join = Drawing.JoinStyle.MiterRevert;
			if ( sel == 2 )  p.Join = Drawing.JoinStyle.Bevel;

			p.RndMove  = (this.rndMove.ActiveState  == WidgetState.ActiveYes);
			p.RndSize  = (this.rndSize.ActiveState  == WidgetState.ActiveYes);
			p.RndShift = (this.rndShift.ActiveState == WidgetState.ActiveYes);
			p.RndAngle = (this.rndAngle.ActiveState == WidgetState.ActiveYes);
			p.RndPage  = (this.rndPage.ActiveState  == WidgetState.ActiveYes);
			p.PatternMove  = (double) this.fieldMove.Value/100;
			p.PatternSize  = (double) this.fieldSize.Value/100;
			p.PatternShift = (double) this.fieldShift.Value/100;
			p.PatternAngle = (double) this.fieldAngle.Value;
			p.PatternSeed  = this.patternSeed;

			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				p.SetDashPen(i, this.dashPen[i]);
				p.SetDashGap(i, this.dashGap[i]);
			}

			return p;
		}

		protected int RadioSelected
		{
			get
			{
				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
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
				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					this.radioDashRank[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}

		protected void DashToWidget()
		{
			int i = this.RadioSelected;
			this.ignoreChange = true;
			this.fieldDashPen.Value = (decimal) this.dashPen[i];
			this.fieldDashGap.Value = (decimal) this.dashGap[i];
			this.ignoreChange = false;
		}

		protected void UpdatePatternId()
		{
			this.patternId.Items.Clear();
			this.patternId.Items.Add("Trait simple");
			this.patternId.Items.Add("Traitillé");

			int total = this.drawer.IconObjects.TotalPatterns()-1;
			for ( int i=0 ; i<total ; i++ )
			{
				ObjectPattern pattern = this.drawer.IconObjects.Objects[i+1] as ObjectPattern;
				string name = pattern.Name;
				if ( name == "" )
				{
					name = string.Format("Motif numéro {0}", i+1);
				}
				this.patternId.Items.Add(name);
			}
		}

		protected int IdToSel(int id)
		{
			if ( id == 0 || !this.IsPatternPossible() )  return 0;
			if ( id == -1 )  return 1;  // traitillé ?
			int total = this.drawer.IconObjects.TotalPatterns()-1;
			for ( int i=0 ; i<total ; i++ )
			{
				ObjectPattern pattern = this.drawer.IconObjects.Objects[i+1] as ObjectPattern;
				if ( pattern.Id == id )  return i+2;
			}
			return 0;
		}

		protected int SelToId(int sel)
		{
			if ( sel == 0 || !this.IsPatternPossible() )  return 0;
			if ( sel == 1 )  return -1;  // traitillé ?
			if ( sel < 0 || sel >= this.drawer.IconObjects.TotalPatterns()+1 )  return 0;
			ObjectPattern pattern = this.drawer.IconObjects.Objects[sel-1] as ObjectPattern;
			return pattern.Id;
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
			if ( this.IsPatternPossible() && this.extendedSize )
			{
				bool simply = (this.patternId.SelectedIndex < 2);
				bool dash = (this.patternId.SelectedIndex == 1);
				bool seed = false;
				if ( this.rndMove.ActiveState  == WidgetState.ActiveYes )  seed = true;
				if ( this.rndSize.ActiveState  == WidgetState.ActiveYes )  seed = true;
				if ( this.rndShift.ActiveState == WidgetState.ActiveYes )  seed = true;
				if ( this.rndAngle.ActiveState == WidgetState.ActiveYes )  seed = true;
				if ( this.rndPage.ActiveState  == WidgetState.ActiveYes )  seed = true;

				this.patternId.SetVisible(true);
				this.patternLabel.SetVisible(true);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].SetEnabled(this.extendedSize);
					this.buttons[i].SetVisible(simply);
				}

				this.rndMove.SetVisible(!simply);
				this.rndSize.SetVisible(!simply);
				this.rndShift.SetVisible(!simply);
				this.rndAngle.SetVisible(!simply);
				this.rndPage.SetVisible(!simply);
				this.fieldMove.SetVisible(!simply);
				this.fieldSize.SetVisible(!simply);
				this.fieldShift.SetVisible(!simply);
				this.fieldAngle.SetVisible(!simply);
				this.labelMove.SetVisible(!simply);
				this.labelSize.SetVisible(!simply);
				this.labelShift.SetVisible(!simply);
				this.labelAngle.SetVisible(!simply);

				this.buttonSeed.SetVisible(!simply);
				this.buttonSeed.SetEnabled(seed);

				for ( int i=0 ; i<5 ; i++ )
				{
					this.separators[i].SetVisible(!simply);
				}

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					this.radioDashRank[i].SetVisible(simply);
					this.radioDashRank[i].SetEnabled(dash);
				}
				this.fieldDashPen.SetVisible(simply);
				this.fieldDashPen.SetEnabled(dash);
				this.fieldDashGap.SetVisible(simply);
				this.fieldDashGap.SetEnabled(dash);
				this.labelDashPen.SetVisible(simply);
				this.labelDashPen.SetEnabled(dash);
				this.labelDashGap.SetVisible(simply);
				this.labelDashGap.SetEnabled(dash);
			}
			else
			{
				this.patternId.SetVisible(false);
				this.patternLabel.SetVisible(false);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].SetEnabled(this.extendedSize);
					this.buttons[i].SetVisible(true);
				}

				this.rndMove.SetVisible(false);
				this.rndSize.SetVisible(false);
				this.rndShift.SetVisible(false);
				this.rndAngle.SetVisible(false);
				this.rndPage.SetVisible(false);
				this.fieldMove.SetVisible(false);
				this.fieldSize.SetVisible(false);
				this.fieldShift.SetVisible(false);
				this.fieldAngle.SetVisible(false);
				this.labelMove.SetVisible(false);
				this.labelSize.SetVisible(false);
				this.labelShift.SetVisible(false);
				this.labelAngle.SetVisible(false);
				this.buttonSeed.SetVisible(false);

				for ( int i=0 ; i<5 ; i++ )
				{
					this.separators[i].SetVisible(false);
				}

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					this.radioDashRank[i].SetVisible(false);
				}
				this.fieldDashPen.SetVisible(false);
				this.fieldDashGap.SetVisible(false);
				this.labelDashPen.SetVisible(false);
				this.labelDashGap.SetVisible(false);
			}
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttons == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-50;
			this.label.Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.field.Bounds = r;

			if ( this.IsPatternPossible() )
			{
				r.Top = r.Bottom-5;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Left+65;
				this.patternLabel.Bounds = r;
				r.Left = r.Right;
				r.Right = rect.Right;
				this.patternId.Bounds = r;
			}

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-(20*6+15);
			r.Width = 20;
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20+((i==2)?15:0), 0);
			}

			r.Left = rect.Left;
			r.Width = 12;
			this.labelMove.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldMove.Bounds = r;
			r.Left = r.Right-1;
			r.Width = 16;
			this.rndMove.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelShift.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldShift.Bounds = r;
			r.Left = r.Right-1;
			r.Width = 16;
			this.rndShift.Bounds = r;

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonSeed.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 12;
			this.labelSize.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldSize.Bounds = r;
			r.Left = r.Right-1;
			r.Width = 16;
			this.rndSize.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelAngle.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldAngle.Bounds = r;
			r.Left = r.Right-1;
			r.Width = 16;
			this.rndAngle.Bounds = r;

			r.Left = rect.Right-30;
			r.Width = 30;
			this.rndPage.Bounds = r;

			r.Left = rect.Left;
			r.Width = 16;
			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				if ( i == PropertyLine.DashMax-1 )  r.Width = 32;
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

			r.Left   =  81;
			r.Right  =  82;
			r.Bottom =  16;
			r.Top    =  41;
			this.separators[0].Bounds = r;
			r.Left   = 152;
			r.Right  = 153;
			this.separators[1].Bounds = r;
			r.Left   = 171;
			r.Right  = 172;
			r.Top    =  27;
			this.separators[2].Bounds = r;
			r.Left   = 184;
			r.Right  = 185;
			r.Bottom =  28;
			r.Top    =  41;
			this.separators[3].Bounds = r;
			r.Left   =  81;
			r.Right  = 185;
			r.Bottom =  27;
			r.Top    =  28;
			this.separators[4].Bounds = r;
		}

		// Indique si les traits avec des patterns sont possibles.
		protected bool IsPatternPossible()
		{
			return (this.drawer.IconObjects.CurrentPattern == 0);
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}

		// Le motif a été changé.
		private void HandlePatternIdChanged(object sender)
		{
			this.EnableWidgets();
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void PanelLineClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonCap = 0;
			if ( button == this.buttons[1] )  this.SelectButtonCap = 1;
			if ( button == this.buttons[2] )  this.SelectButtonCap = 2;

			if ( button == this.buttons[3] )  this.SelectButtonJoin = 0;
			if ( button == this.buttons[4] )  this.SelectButtonJoin = 1;
			if ( button == this.buttons[5] )  this.SelectButtonJoin = 2;

			this.OnChanged();
		}

		// Un bouton à cocher a été changé.
		private void HandleRndChanged(object sender)
		{
			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			this.OnChanged();
		}

		// Le rang a été changé.
		private void HandleDashRankChanged(object sender)
		{
			this.dashPen[this.dashRank] = (double) this.fieldDashPen.Value;
			this.dashGap[this.dashRank] = (double) this.fieldDashGap.Value;

			this.dashRank = this.RadioSelected;
			this.DashToWidget();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleDashChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			this.dashPen[this.dashRank] = (double) this.fieldDashPen.Value;
			this.dashGap[this.dashRank] = (double) this.fieldDashGap.Value;

			this.OnChanged();
		}

		private void HandleSeedClicked(object sender, MessageEventArgs e)
		{
			this.patternSeed = (this.patternSeed+1)%10000;
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldSlider			field;
		protected StaticText				patternLabel;
		protected TextFieldCombo			patternId;
		protected IconButton[]				buttons;
		protected CheckButton				rndMove;
		protected CheckButton				rndSize;
		protected CheckButton				rndShift;
		protected CheckButton				rndAngle;
		protected CheckButton				rndPage;
		protected TextFieldSlider			fieldMove;
		protected TextFieldSlider			fieldSize;
		protected TextFieldSlider			fieldShift;
		protected TextFieldSlider			fieldAngle;
		protected StaticText				labelMove;
		protected StaticText				labelSize;
		protected StaticText				labelShift;
		protected StaticText				labelAngle;
		protected Button					buttonSeed;
		protected RadioButton[]				radioDashRank;
		protected TextFieldSlider			fieldDashPen;
		protected TextFieldSlider			fieldDashGap;
		protected StaticText				labelDashPen;
		protected StaticText				labelDashGap;
		protected int						patternSeed;
		protected int						dashRank = 0;
		protected double[]					dashPen;
		protected double[]					dashGap;
		protected bool						ignoreChange = false;
		protected Separator[]				separators;
	}
}

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
		public PanelLine()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextFieldSlider(this);
			this.field.MinValue = 0;
			this.field.MaxValue = 5;
			this.field.Step = 0.1M;
			this.field.Resolution = 0.01M;
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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

			this.buttons[3].IconName = @"file:images/joinround.icon";
			this.buttons[4].IconName = @"file:images/joinmiter.icon";
			this.buttons[5].IconName = @"file:images/joinbevel.icon";

			this.fieldSize = new TextFieldSlider(this);
			this.fieldSize.MinValue =  10;
			this.fieldSize.MaxValue = 200;
			this.fieldSize.Step = 5.0M;
			this.fieldSize.Resolution = 1.0M;
			this.fieldSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldSize.TabIndex = 10;
			this.fieldSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldShift = new TextFieldSlider(this);
			this.fieldShift.MinValue = -100;
			this.fieldShift.MaxValue =  100;
			this.fieldShift.Step = 5.0M;
			this.fieldShift.Resolution = 1.0M;
			this.fieldShift.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldShift.TabIndex = 11;
			this.fieldShift.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldAngle = new TextFieldSlider(this);
			this.fieldAngle.MinValue = -180;
			this.fieldAngle.MaxValue =  180;
			this.fieldAngle.Step = 5.0M;
			this.fieldAngle.Resolution = 1.0M;
			this.fieldAngle.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldAngle.TabIndex = 12;
			this.fieldAngle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelSize = new StaticText(this);
			this.labelSize.Text = "Z";
			this.labelSize.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelShift = new StaticText(this);
			this.labelShift.Text = "D";
			this.labelShift.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.labelAngle = new StaticText(this);
			this.labelAngle.Text = "A";
			this.labelAngle.Alignment = Drawing.ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		public PanelLine(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
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
					this.fieldSize.TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldShift.TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldAngle.TextChanged -= new EventHandler(this.HandleFieldChanged);
				}
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
					return this.IsPatternPossible() ? 80 : 55;
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
			if ( p.Join == Drawing.JoinStyle.Round )  sel = 0;
			if ( p.Join == Drawing.JoinStyle.Miter )  sel = 1;
			if ( p.Join == Drawing.JoinStyle.Bevel )  sel = 2;
			this.SelectButtonJoin = sel;

			this.fieldSize.Value  = (decimal) p.PatternSize*100;
			this.fieldShift.Value = (decimal) p.PatternShift*100;
			this.fieldAngle.Value = (decimal) (p.PatternAngle*180/System.Math.PI);

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyLine p = new PropertyLine();
			base.GetProperty(p);

			p.Width = (double) this.field.Value;
			p.PatternId = this.SelToId(this.patternId.SelectedIndex);

			int sel = this.SelectButtonCap;
			if ( sel == 0 )  p.Cap = Drawing.CapStyle.Round;
			if ( sel == 1 )  p.Cap = Drawing.CapStyle.Square;
			if ( sel == 2 )  p.Cap = Drawing.CapStyle.Butt;

			sel = this.SelectButtonJoin;
			if ( sel == 0 )  p.Join = Drawing.JoinStyle.Round;
			if ( sel == 1 )  p.Join = Drawing.JoinStyle.Miter;
			if ( sel == 2 )  p.Join = Drawing.JoinStyle.Bevel;

			p.PatternSize  = (double) this.fieldSize.Value/100;
			p.PatternShift = (double) this.fieldShift.Value/100;
			p.PatternAngle = (double) this.fieldAngle.Value*System.Math.PI/180;

			return p;
		}

		protected void UpdatePatternId()
		{
			this.patternId.Items.Clear();
			this.patternId.Items.Add("Trait simple");

			int total = this.drawer.IconObjects.TotalPatterns();
			for ( int i=1 ; i<total ; i++ )
			{
				ObjectPattern pattern = this.drawer.IconObjects.Objects[i] as ObjectPattern;
				string name = pattern.Name;
				if ( name == "" )
				{
					name = string.Format("Motif numéro {0}", i);
				}
				this.patternId.Items.Add(name);
			}
		}

		protected int IdToSel(int id)
		{
			if ( id == 0 || !this.IsPatternPossible() )  return 0;
			int total = this.drawer.IconObjects.TotalPatterns();
			for ( int i=1 ; i<total ; i++ )
			{
				ObjectPattern pattern = this.drawer.IconObjects.Objects[i] as ObjectPattern;
				if ( pattern.Id == id )  return i;
			}
			return -1;
		}

		protected int SelToId(int sel)
		{
			if ( sel == 0 || !this.IsPatternPossible() )  return 0;
			if ( sel >= this.drawer.IconObjects.TotalPatterns() )  return 0;
			ObjectPattern pattern = this.drawer.IconObjects.Objects[sel] as ObjectPattern;
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
			if ( this.IsPatternPossible() )
			{
				bool simply = (this.patternId.SelectedIndex == 0);

				this.patternId.SetVisible(true);
				this.patternLabel.SetVisible(true);

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].SetEnabled(this.extendedSize);
					this.buttons[i].SetVisible(simply);
				}

				this.fieldSize.SetVisible(!simply);
				this.fieldShift.SetVisible(!simply);
				this.fieldAngle.SetVisible(!simply);
				this.labelSize.SetVisible(!simply);
				this.labelShift.SetVisible(!simply);
				this.labelAngle.SetVisible(!simply);
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

				this.fieldSize.SetVisible(false);
				this.fieldShift.SetVisible(false);
				this.fieldAngle.SetVisible(false);
				this.labelSize.SetVisible(false);
				this.labelShift.SetVisible(false);
				this.labelAngle.SetVisible(false);
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
			r.Width = 14;
			this.labelSize.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fieldSize.Bounds = r;
			r.Left = r.Right;
			r.Width = 15;
			this.labelShift.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldShift.Bounds = r;
			r.Left = r.Right;
			r.Width = 10;
			this.labelAngle.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldAngle.Bounds = r;
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

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldSlider			field;
		protected StaticText				patternLabel;
		protected TextFieldCombo			patternId;
		protected IconButton[]				buttons;
		protected TextFieldSlider			fieldSize;
		protected TextFieldSlider			fieldShift;
		protected TextFieldSlider			fieldAngle;
		protected StaticText				labelSize;
		protected StaticText				labelShift;
		protected StaticText				labelAngle;
	}
}

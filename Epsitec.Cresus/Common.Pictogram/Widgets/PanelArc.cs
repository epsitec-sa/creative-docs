using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelArc permet de choisir un mode d'arc de cercle ou d'ellipse.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelArc : AbstractPanel
	{
		public PanelArc(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.PanelArcClicked);
				this.buttons[i].TabIndex = 2+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = @"file:images/arcfull.icon";
			this.buttons[1].IconName = @"file:images/arcopen.icon";
			this.buttons[2].IconName = @"file:images/arcclose.icon";
			this.buttons[3].IconName = @"file:images/arcpie.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], "Ellipse complète");
			ToolTip.Default.SetToolTip(this.buttons[1], "Arc ouvert");
			ToolTip.Default.SetToolTip(this.buttons[2], "Arc fermé");
			ToolTip.Default.SetToolTip(this.buttons[3], "Camembert");

			this.fieldStarting = new TextFieldSlider(this);
			this.fieldStarting.MinValue =   0;
			this.fieldStarting.MaxValue = 360;
			this.fieldStarting.Step = 5;
			this.fieldStarting.Resolution = 1;
			this.fieldStarting.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldStarting.TabIndex = 20;
			this.fieldStarting.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldStarting, "Angle initial");

			this.fieldEnding = new TextFieldSlider(this);
			this.fieldEnding.MinValue =   0;
			this.fieldEnding.MaxValue = 360;
			this.fieldEnding.Step = 5;
			this.fieldEnding.Resolution = 1;
			this.fieldEnding.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldEnding.TabIndex = 21;
			this.fieldEnding.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEnding, "Angle final");

			this.labelStarting = new StaticText(this);
			this.labelStarting.Text = "a1";
			this.labelStarting.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelEnding = new StaticText(this);
			this.labelEnding.Text = "a2";
			this.labelEnding.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.PanelArcClicked);
					this.buttons[i] = null;
				}
				this.fieldStarting.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldEnding.TextChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.fieldStarting = null;
				this.fieldEnding = null;
				this.labelStarting = null;
				this.labelEnding = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.extendedSize ? 55 : 30 );
			}
		}

		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyArc p = property as PropertyArc;
			if ( p == null )  return;

			this.SelectButtonType    = p.ArcType;
			this.fieldStarting.Value = (decimal) p.StartingAngle;
			this.fieldEnding.Value   = (decimal) p.EndingAngle;

			this.EnableWidgets();
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyArc p = new PropertyArc();
			base.GetProperty(p);

			p.ArcType       = this.SelectButtonType;
			p.StartingAngle = (double) this.fieldStarting.Value;
			p.EndingAngle   = (double) this.fieldEnding.Value;

			return p;
		}

		protected ArcType SelectButtonType
		{
			get
			{
				if ( this.ButtonActive(0) )  return ArcType.Full;
				if ( this.ButtonActive(1) )  return ArcType.Open;
				if ( this.ButtonActive(2) )  return ArcType.Close;
				if ( this.ButtonActive(3) )  return ArcType.Pie;
				return ArcType.Full;
			}

			set
			{
				this.ButtonActive(0, value == ArcType.Full);
				this.ButtonActive(1, value == ArcType.Open);
				this.ButtonActive(2, value == ArcType.Close);
				this.ButtonActive(3, value == ArcType.Pie);
			}
		}

		protected bool ButtonActive(int i)
		{
			return ( this.buttons[i].ActiveState == WidgetState.ActiveYes );
		}

		protected void ButtonActive(int i, bool active)
		{
			this.buttons[i].ActiveState = active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool enable = !this.ButtonActive(0);
			this.fieldStarting.SetEnabled(this.extendedSize && enable);
			this.fieldEnding.SetEnabled(this.extendedSize && enable);
			this.labelStarting.SetEnabled(this.extendedSize && enable);
			this.labelEnding.SetEnabled(this.extendedSize && enable);

			this.fieldStarting.SetVisible(this.extendedSize);
			this.fieldEnding.SetVisible(this.extendedSize);
			this.labelStarting.SetVisible(this.extendedSize);
			this.labelEnding.SetVisible(this.extendedSize);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttons == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-20*4;
			this.label.Bounds = r;

			r.Left = rect.Right-20*4;
			r.Width = 20;
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20, 0);
			}

			r.Offset(0, -25);
			r.Left = rect.Left+41;
			r.Width = 20;
			this.labelStarting.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldStarting.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 20;
			this.labelEnding.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldEnding.Bounds = r;
		}
		
		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			this.OnChanged();
		}

		private void PanelArcClicked(object sender, MessageEventArgs e)
		{
			//	Une valeur a été changée.
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonType = ArcType.Full;
			if ( button == this.buttons[1] )  this.SelectButtonType = ArcType.Open;
			if ( button == this.buttons[2] )  this.SelectButtonType = ArcType.Close;
			if ( button == this.buttons[3] )  this.SelectButtonType = ArcType.Pie;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected IconButton[]				buttons;
		protected TextFieldSlider			fieldStarting;
		protected TextFieldSlider			fieldEnding;
		protected StaticText				labelStarting;
		protected StaticText				labelEnding;
	}
}

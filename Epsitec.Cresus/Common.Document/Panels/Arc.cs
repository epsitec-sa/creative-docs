using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Arc permet de choisir un mode d'arc de cercle ou d'ellipse.
	/// </summary>
	[SuppressBundleSupport]
	public class Arc : Abstract
	{
		public Arc(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.HandlePanelArcClicked);
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
			this.labelStarting.Alignment = ContentAlignment.MiddleCenter;

			this.labelEnding = new StaticText(this);
			this.labelEnding.Text = "a2";
			this.labelEnding.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandlePanelArcClicked);
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

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 55 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Arc p = this.property as Properties.Arc;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.SelectButtonType    = p.ArcType;
			this.fieldStarting.Value = (decimal) p.StartingAngle;
			this.fieldEnding.Value   = (decimal) p.EndingAngle;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Arc p = this.property as Properties.Arc;
			if ( p == null )  return;

			p.ArcType       = this.SelectButtonType;
			p.StartingAngle = (double) this.fieldStarting.Value;
			p.EndingAngle   = (double) this.fieldEnding.Value;
		}

		protected Properties.ArcType SelectButtonType
		{
			get
			{
				if ( this.ButtonActive(0) )  return Properties.ArcType.Full;
				if ( this.ButtonActive(1) )  return Properties.ArcType.Open;
				if ( this.ButtonActive(2) )  return Properties.ArcType.Close;
				if ( this.ButtonActive(3) )  return Properties.ArcType.Pie;
				return Properties.ArcType.Full;
			}

			set
			{
				this.ButtonActive(0, value == Properties.ArcType.Full);
				this.ButtonActive(1, value == Properties.ArcType.Open);
				this.ButtonActive(2, value == Properties.ArcType.Close);
				this.ButtonActive(3, value == Properties.ArcType.Pie);
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


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			bool enable = !this.ButtonActive(0);
			this.fieldStarting.SetEnabled(this.isExtendedSize && enable);
			this.fieldEnding.SetEnabled(this.isExtendedSize && enable);
			this.labelStarting.SetEnabled(this.isExtendedSize && enable);
			this.labelEnding.SetEnabled(this.isExtendedSize && enable);

			this.fieldStarting.SetVisible(this.isExtendedSize);
			this.fieldEnding.SetVisible(this.isExtendedSize);
			this.labelStarting.SetVisible(this.isExtendedSize);
			this.labelEnding.SetVisible(this.isExtendedSize);
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
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandlePanelArcClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonType = Properties.ArcType.Full;
			if ( button == this.buttons[1] )  this.SelectButtonType = Properties.ArcType.Open;
			if ( button == this.buttons[2] )  this.SelectButtonType = Properties.ArcType.Close;
			if ( button == this.buttons[3] )  this.SelectButtonType = Properties.ArcType.Pie;

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

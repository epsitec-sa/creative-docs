using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelTextLine permet de choisir un mode de justification.
	/// </summary>
	public class PanelTextLine : AbstractPanel
	{
		public PanelTextLine()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.PanelTextLineClicked);
				this.buttons[i].TabIndex = 2+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = @"file:images/justifhleft.icon";
			this.buttons[1].IconName = @"file:images/justifhcenter.icon";
			this.buttons[2].IconName = @"file:images/justifhright.icon";
			this.buttons[3].IconName = @"file:images/justifhstretch.icon";

			this.fieldOffset = new TextFieldSlider(this);
			this.fieldOffset.MinValue =  0;
			this.fieldOffset.MaxValue = 70;
			this.fieldOffset.Step = 5;
			this.fieldOffset.Resolution = 1;
			this.fieldOffset.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldOffset.TabIndex = 22;
			this.fieldOffset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldAdd = new TextFieldSlider(this);
			this.fieldAdd.MinValue = -20;
			this.fieldAdd.MaxValue = 100;
			this.fieldAdd.Step = 1.0M;
			this.fieldAdd.Resolution = 1.0M;
			this.fieldAdd.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldAdd.TabIndex = 20;
			this.fieldAdd.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelOffset = new StaticText(this);
			this.labelOffset.Text = "Oy";
			this.labelOffset.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.labelAdd = new StaticText(this);
			this.labelAdd.Text = "L";
			this.labelAdd.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		public PanelTextLine(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.PanelTextLineClicked);
				}
				this.fieldOffset.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldAdd.TextChanged -= new EventHandler(this.HandleFieldChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.extendedSize ? 55 : 30 );
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyTextLine p = property as PropertyTextLine;
			if ( p == null )  return;

			this.SelectButtonHorizontal = p.Horizontal;
			this.fieldOffset.Value      = (decimal) p.Offset*100;
			this.fieldAdd.Value         = (decimal) p.Add*100;

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyTextLine p = new PropertyTextLine();
			base.GetProperty(p);

			p.Horizontal = this.SelectButtonHorizontal;
			p.Offset     = (double) this.fieldOffset.Value/100;
			p.Add        = (double) this.fieldAdd.Value/100;

			return p;
		}

		protected JustifHorizontal SelectButtonHorizontal
		{
			get
			{
				if ( this.ButtonActive(0) )  return JustifHorizontal.Left;
				if ( this.ButtonActive(1) )  return JustifHorizontal.Center;
				if ( this.ButtonActive(2) )  return JustifHorizontal.Right;
				if ( this.ButtonActive(3) )  return JustifHorizontal.Stretch;
				return JustifHorizontal.None;
			}

			set
			{
				this.ButtonActive(0, value == JustifHorizontal.Left);
				this.ButtonActive(1, value == JustifHorizontal.Center);
				this.ButtonActive(2, value == JustifHorizontal.Right);
				this.ButtonActive(3, value == JustifHorizontal.Stretch);
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
			this.fieldOffset.SetVisible(this.extendedSize);
			this.fieldAdd.SetVisible(this.extendedSize);
			this.labelOffset.SetVisible(this.extendedSize);
			this.labelAdd.SetVisible(this.extendedSize);
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
			this.labelOffset.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldOffset.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 20;
			this.labelAdd.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldAdd.Bounds = r;
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void PanelTextLineClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonHorizontal = JustifHorizontal.Left;
			if ( button == this.buttons[1] )  this.SelectButtonHorizontal = JustifHorizontal.Center;
			if ( button == this.buttons[2] )  this.SelectButtonHorizontal = JustifHorizontal.Right;
			if ( button == this.buttons[3] )  this.SelectButtonHorizontal = JustifHorizontal.Stretch;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected IconButton[]				buttons;
		protected TextFieldSlider			fieldOffset;
		protected TextFieldSlider			fieldAdd;
		protected StaticText				labelOffset;
		protected StaticText				labelAdd;
	}
}

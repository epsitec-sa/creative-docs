using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelLine permet de choisir un mode de trait.
	/// </summary>
	public class PanelLine : AbstractPanel
	{
		public PanelLine()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextFieldSlider(this);
			this.field.MinRange = 0;
			this.field.MaxRange = 5;
			this.field.Step = 0.5;
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);

			this.buttons = new IconButton[6];
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.PanelLineClicked);
			}

			this.buttons[0].IconName = @"file:images/capround1.icon";
			this.buttons[1].IconName = @"file:images/capsquare1.icon";
			this.buttons[2].IconName = @"file:images/capbutt1.icon";

			this.buttons[3].IconName = @"file:images/joinround1.icon";
			this.buttons[4].IconName = @"file:images/joinmiter1.icon";
			this.buttons[5].IconName = @"file:images/joinbevel1.icon";

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

				for ( int i=0 ; i<6 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.PanelLineClicked);
				}
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
			this.label.Text = this.text;

			PropertyLine p = property as PropertyLine;
			if ( p == null )  return;

			this.field.Value = p.Width;

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
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyLine p = new PropertyLine();
			base.GetProperty(p);

			p.Width = this.field.Value;

			int sel = this.SelectButtonCap;
			if ( sel == 0 )  p.Cap = Drawing.CapStyle.Round;
			if ( sel == 1 )  p.Cap = Drawing.CapStyle.Square;
			if ( sel == 2 )  p.Cap = Drawing.CapStyle.Butt;

			sel = this.SelectButtonJoin;
			if ( sel == 0 )  p.Join = Drawing.JoinStyle.Round;
			if ( sel == 1 )  p.Join = Drawing.JoinStyle.Miter;
			if ( sel == 2 )  p.Join = Drawing.JoinStyle.Bevel;

			return p;
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


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttons == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.field.Bounds = r;

			rect.Top = r.Bottom-5;
			rect.Bottom = rect.Top-20;
			rect.Left = rect.Right-(rect.Height*6+5);
			rect.Width = rect.Height;
			for ( int i=0 ; i<6 ; i++ )
			{
				this.buttons[i].Bounds = rect;

				if ( i == 2 )
				{
					rect.Offset(rect.Width+5, 0);
				}
				else
				{
					rect.Offset(rect.Width, 0);
				}
			}

		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
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


		protected StaticText				label;
		protected TextFieldSlider			field;
		protected IconButton[]				buttons;
	}
}

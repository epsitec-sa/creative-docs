using Epsitec.Common.NiceIcon;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ButtonGraphics représente un bouton avec une icône vectorielle.
	/// Cette classe est provosoire TODO: incorporer à Widgets !
	/// </summary>
	public class ButtonGraphics : Button
	{
		public ButtonGraphics()
		{
			this.buttonStyle = ButtonStyle.Normal;

			this.iconObjects = new IconObjects();
			this.iconContext = new IconContext();
		}
		
		public ButtonGraphics(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Retourne la hauteur standard d'un bouton.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight+10;
			}
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			return new Drawing.Rectangle(0, 0, this.Client.Width+1, this.Client.Height);
		}

		public IconObjects IconObjects
		{
			get
			{
				return this.iconObjects;
			}

			set
			{
				if ( this.iconObjects != value )
				{
					this.iconObjects = value;
					this.Invalidate();
				}
			}
		}

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			if ( (state & WidgetState.Enabled) == 0 )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}
			adorner.PaintButtonBackground(graphics, rect, state, dir, this.buttonStyle);

			if ( this.iconObjects != null )
			{
				this.iconContext.IsEnable = this.IsEnabled;

				double initialWidth = graphics.LineWidth;
				Drawing.Transform save = graphics.SaveTransform();
				this.iconContext.ScaleX = (this.Client.Width-2)/100;
				this.iconContext.ScaleY = (this.Client.Height-2)/100;
				graphics.ScaleTransform(this.iconContext.ScaleX, this.iconContext.ScaleY, 0.5, 2);

				this.iconObjects.DrawGeometry(graphics, this.iconContext);

				graphics.Transform = save;
				graphics.LineWidth = initialWidth;
			}
		}
		
		
		protected IconObjects			iconObjects;
		protected IconContext			iconContext;
		protected double				margin = 3;
	}
}

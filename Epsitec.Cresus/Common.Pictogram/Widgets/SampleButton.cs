using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe SampleButton représente un bouton avec une icône vectorielle.
	/// </summary>
	public class SampleButton : Epsitec.Common.Widgets.Button
	{
		public SampleButton()
		{
			this.buttonStyle = ButtonStyle.Normal;

			this.iconObjects = new IconObjects();
			this.iconContext = new IconContext();
		}
		
		public SampleButton(Widget embedder) : this()
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
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

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
				this.iconContext.Adorner = adorner;

				if ( this.IsEnabled )
				{
					this.iconContext.UniqueColor = Drawing.Color.Empty;
				}
				else
				{
					this.iconContext.UniqueColor = Drawing.Color.FromBrightness(0.6);
				}

				double initialWidth = graphics.LineWidth;
				Drawing.Transform save = graphics.SaveTransform();
				this.iconContext.ScaleX = (this.Client.Width-2)/this.iconObjects.Size.Width;
				this.iconContext.ScaleY = (this.Client.Height-2)/this.iconObjects.Size.Height;
				graphics.TranslateTransform(1, 1);
				graphics.ScaleTransform(this.iconContext.ScaleX, this.iconContext.ScaleY, 0, 0);

				this.iconObjects.DrawGeometry(graphics, this.iconContext, Drawing.Color.Empty, adorner);

				graphics.Transform = save;
				graphics.LineWidth = initialWidth;
			}
		}
		
		
		protected IconObjects			iconObjects;
		protected IconContext			iconContext;
		protected double				margin = 3;
	}
}

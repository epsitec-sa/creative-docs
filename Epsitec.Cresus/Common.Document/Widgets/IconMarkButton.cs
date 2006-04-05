using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe IconMarkButton est un IconButton avec une marque 'v' en bas.
	/// </summary>
	public class IconMarkButton : IconButton
	{
		public IconMarkButton()
		{
		}

		public IconMarkButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public double MarkHeight
		{
			//	Hauteur de la marque en bas du bouton.
			get
			{
				return this.markHeight;
			}

			set
			{
				this.markHeight = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			
			rect.Bottom += this.markHeight;
			pos.Y += this.markHeight/2;

			if ( (state & WidgetState.Enabled) == 0 )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}

			if ( this.ActiveState == ActiveState.Yes )  // dessine la marque 'v' en bas du bouton ?
			{
				Drawing.Rectangle rmark = this.Client.Bounds;
				rmark.Top = rmark.Bottom+this.markHeight;
				rmark.Inflate(5);
				rmark.Offset(0, 1);
				rmark.Top += 1;
				if ( (state & WidgetState.Enabled) == 0 )
				{
					Color color = adorner.ColorTextFieldBorder(false);
					adorner.PaintGlyph(graphics, rmark, WidgetState.Enabled, color, GlyphShape.Menu, PaintTextStyle.Button);
				}
				else
				{
					adorner.PaintGlyph(graphics, rmark, WidgetState.Enabled, GlyphShape.Menu, PaintTextStyle.Button);
				}
			}
			
			state &= ~WidgetState.Selected;
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);

			if ( this.innerZoom != 1.0 )
			{
				double zoom = (this.innerZoom-1)/2+1;
				this.TextLayout.LayoutSize = this.Client.Size/this.innerZoom;
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Size.Height*zoom);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
				graphics.Transform = transform;
			}
			else
			{
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
			}
		}


		protected double				markHeight = 8;
	}
}

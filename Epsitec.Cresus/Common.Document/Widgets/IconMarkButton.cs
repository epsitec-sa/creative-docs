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
			
			state &= ~WidgetState.Selected;
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);
			adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);

			if ( this.ActiveState == ActiveState.Yes )  // dessine la marque 'v' en bas du bouton ?
			{
				rect = this.Client.Bounds;
				rect.Top = rect.Bottom+this.markHeight;
				rect.Inflate(5);
				rect.Offset(0, 1);
				adorner.PaintGlyph(graphics, rect, WidgetState.Enabled, GlyphShape.Menu, PaintTextStyle.Button);
			}
		}


		protected double				markHeight = 8;
	}
}

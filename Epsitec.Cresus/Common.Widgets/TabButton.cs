namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class TabButton représente un bouton d'un onglet.
	/// </summary>
	public class TabButton : AbstractButton
	{
		public TabButton()
		{
			this.InternalState &= ~InternalState.Engageable;
			this.InternalState &= ~InternalState.AutoFocus;
		}
		
		public TabButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			return new Drawing.Rectangle(-2, -2, this.Client.Width+2, this.Client.Height+2);
		}

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();

			TabBook tabBook = this.Parent as TabBook;
			Drawing.Rectangle frameRect = tabBook.TabClipRectangle;
			Drawing.Rectangle localClip = tabBook.MapClientToRoot(frameRect);
			Drawing.Rectangle saveClip  = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(localClip);
			
			if ( this.ActiveState == WidgetState.ActiveYes )
			{
				rect.Bottom -= 2;
				adorner.PaintTabAboveBackground(graphics, frameRect, rect, state, Direction.Up);
				pos.Y += 1;
			}
			else
			{
				rect.Top -= 2;
				adorner.PaintTabSunkenBackground(graphics, frameRect, rect, state, Direction.Up);
				pos.Y -= 1;
			}
			
			if (this.Text != "")
			{
				Drawing.Size size = this.Client.Size;
				size.Width -= 4;
				this.TextLayout.LayoutSize = size;
				this.TextLayout.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				pos.X += 2;
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, ButtonStyle.Tab);
			}
			
			graphics.RestoreClippingRectangle(saveClip);
		}
	}
}

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class TabButton représente un bouton d'un onglet.
	/// </summary>
	public class TabButton : AbstractButton
	{
		public TabButton()
		{
			this.internalState &= ~InternalState.Engageable;
		}
		
		public TabButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public override Drawing.Rectangle GetPaintBounds()
		{
			//return new Drawing.Rectangle(0, -2, this.clientInfo.width, this.clientInfo.height+2);
			return new Drawing.Rectangle(-2, -2, this.clientInfo.width+2, this.clientInfo.height+2);
		}

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);

			TabBook tabBook = this.Parent as TabBook;
			Drawing.Rectangle frameRect = tabBook.TabClipRectangle;
			Drawing.Rectangle localClip = tabBook.MapClientToRoot(frameRect);
			Drawing.Rectangle saveClip  = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(localClip);
			
			if ( this.ActiveState == WidgetState.ActiveYes )
			{
				rect.Bottom -= 2;
				adorner.PaintTabAboveBackground(graphics, frameRect, rect, state, dir);
				pos.Y += 1;
			}
			else
			{
				rect.Top -= 2;
				adorner.PaintTabSunkenBackground(graphics, frameRect, rect, state, dir);
				pos.Y -= 1;
			}

			adorner.PaintButtonTextLayout(graphics, pos, this.textLayout, state, dir, ButtonStyle.Normal);

			graphics.RestoreClippingRectangle(saveClip);
		}
	}
}

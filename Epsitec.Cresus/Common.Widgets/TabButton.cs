namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class TabButton représente un bouton d'un onglet.
	/// </summary>
	public class TabButton : AbstractButton
	{
		public TabButton()
		{
			this.internal_state &= ~InternalState.Engageable;
		}
		
		public override Drawing.Rectangle GetPaintBounds()
		{
			return new Drawing.Rectangle(0, -2, this.client_info.width, this.client_info.height+2);
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
			}
			else
			{
				rect.Top -= 2;
				adorner.PaintTabSunkenBackground(graphics, frameRect, rect, state, dir);
			}

			adorner.PaintButtonTextLayout(graphics, pos, this.text_layout, state, dir, ButtonStyle.Normal);

			graphics.RestoreClippingRectangle(saveClip);
		}
	}
}

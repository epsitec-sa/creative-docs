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
			this.AutoFocus = false;
		}
		
		public TabButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public override Drawing.Margins GetShapeMargins()
		{
			return new Drawing.Margins(2, 2, 2, 2);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();

			TabBook tabBook = this.Parent as TabBook;
			Drawing.Rectangle frameRect = tabBook.TabClipRectangle;
			Drawing.Rectangle localClip = tabBook.MapClientToRoot(frameRect);
			Drawing.Rectangle saveClip  = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(localClip);
			
			if ( this.ActiveState == ActiveState.Yes )
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
			
			if ( this.Text != "" )
			{
				Drawing.Size size = this.Client.Size;
				size.Width -= 4;
				this.TextLayout.LayoutSize = size;
				pos.X += 2;
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, ButtonStyle.Tab);
			}
			
			graphics.RestoreClippingRectangle(saveClip);
		}
	}
}

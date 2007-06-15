using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class ConfirmationButton repr�sente un bouton pour le dialogue ConfirmationDialog.
	/// </summary>
	public class ConfirmationButton : Button
	{
		public ConfirmationButton()
		{
			this.ButtonStyle = ButtonStyle.Confirmation;
			this.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
		}
		
		public ConfirmationButton(string text)
		{
			this.Text = text;
		}
		
		public ConfirmationButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override Drawing.Size GetTextLayoutSize()
		{
			Drawing.Size size = this.IsActualGeometryValid ? this.Client.Size : this.PreferredSize;
			size.Width -= ConfirmationButton.marginLeft+ConfirmationButton.marginRight;
			size.Height -= ConfirmationButton.marginY*2;
			return size;
		}

		protected override Drawing.Point GetTextLayoutOffset()
		{
			return new Drawing.Point(ConfirmationButton.marginLeft, ConfirmationButton.marginY);
		}

		protected override void OnSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
		{
			base.OnSizeChanged(oldValue, newValue);

			if (oldValue.Width != newValue.Width)  // largeur chang�e ?
			{
				double h = this.TextLayout.FindTextHeight();
				this.PreferredHeight = h+ConfirmationButton.marginY*2+2;  // TODO: pourquoi +2 ?
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.PaintState;
			Drawing.Point     pos   = this.GetTextLayoutOffset();
			
			if ( (state & WidgetPaintState.Enabled) == 0 )
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement d�fini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'�tat s�lectionn� si on peint nous-m�me le fond du bouton.
				state &= ~WidgetPaintState.Selected;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.ButtonStyle);
			}

			pos.Y += this.GetBaseLineVerticalOffset ();
			adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);

			//	Dessine un petit ">" positionn� de fa�on empyrique.
			rect = new Drawing.Rectangle(rect.Left+2, rect.Top-20-13, 20, 20);
			adorner.PaintGlyph(graphics, rect, state, adorner.ColorCaption, GlyphShape.TriangleRight, PaintTextStyle.StaticText);
		}


		protected static readonly double marginLeft = 30;
		protected static readonly double marginRight = 10;
		protected static readonly double marginY = 10;
	}
}

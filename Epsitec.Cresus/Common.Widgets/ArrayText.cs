namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ArrayText repr�sente du texte non �ditable dans un tableau.
	/// Ce texte peut �tre s�lectionn�.
	/// </summary>
	public class ArrayText : Widget
	{
		public ArrayText()
		{
			this.internal_state |= InternalState.AcceptTaggedText;
		}


		// Retourne l'alignement par d�faut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		// Dessine le texte.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			
			adorner.PaintArrayTextLayout(graphics, rect, this.text_layout, state, dir);
		}
	}
}

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StaticText représente du texte non éditable. Ce texte
	/// peut comprendre des images et des liens hyper-texte.
	/// </summary>
	public class StaticText : Widget
	{
		public StaticText()
		{
			this.BackColor = Drawing.Color.Transparent;
		}
		
		public StaticText(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public override double DefaultHeight
		{
			// Retourne la hauteur standard.
			get
			{
				return this.DefaultFontHeight;
			}
		}

		public override Drawing.ContentAlignment DefaultAlignment
		{
			// Retourne l'alignement par défaut d'un bouton.
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		
		public Drawing.Size RetRequiredSize()
		{
			// Retourne les dimensions minimales pour représenter le texte.
			return this.textLayout.SingleLineSize();
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le texte.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			
			if ( !this.BackColor.IsTransparent )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.BackColor);
			}
			
			adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state, dir);
		}
	}
}

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

		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight;
			}
		}

		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		// Retourne les dimensions minimales pour représenter le texte.
		public Drawing.Size RetRequiredSize()
		{
			return this.textLayout.SingleLineSize();
		}

		// Dessine le texte.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
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

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StatusField repr�sente une case d'une ligne de statuts.
	/// </summary>
	public class StatusField : Widget
	{
		public StatusField()
		{
		}
		
		public StatusField(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		public StatusField(string text) : this()
		{
			this.Text = text;
		}
		
		public override double DefaultWidth
		{
			// Retourne la largeur standard.
			get
			{
				return 120;
			}
		}

		public override double DefaultHeight
		{
			// Retourne la hauteur standard.
			get
			{
				return this.DefaultFontHeight;
			}
		}

		protected override void UpdateTextLayout()
		{
			if ( this.TextLayout != null )
			{
				double dx = this.Client.Width - this.margin*2;
				double dy = this.Client.Height;
				this.TextLayout.Alignment = this.Alignment;
				this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		public override Drawing.ContentAlignment DefaultAlignment
		{
			// Retourne l'alignement par d�faut d'un bouton.
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

#if false	//#fix
		public override Drawing.Size PreferredSize
		{
			// Retourne les dimensions minimales pour repr�senter le texte.
			get
			{
				return this.TextLayout.SingleLineSize;
			}
		}
#endif
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le texte.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			if ( this.TextLayout == null )  return;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();
			
			if ( this.BackColor.IsVisible )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.BackColor);
			}

			adorner.PaintStatusItemBackground(graphics, rect, state);
			pos.X += this.margin;
			pos.Y += 0.5;
			adorner.PaintGeneralTextLayout(graphics, clipRect, pos, this.TextLayout, state, PaintTextStyle.Status, this.BackColor);
		}


		protected double			margin = 8.0;
	}
}

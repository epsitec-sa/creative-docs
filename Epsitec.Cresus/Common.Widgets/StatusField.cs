namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StatusField représente une case d'une ligne de statuts.
	/// </summary>
	public class StatusField : Widget
	{
		public StatusField()
		{
			this.BackColor = Drawing.Color.Transparent;
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

		protected override void UpdateLayoutSize()
		{
			if ( this.textLayout != null )
			{
				double dx = this.Client.Width - this.margin*2;
				double dy = this.Client.Height;
				this.textLayout.Alignment = this.Alignment;
				this.textLayout.LayoutSize = new Drawing.Size(dx, dy);
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

			if ( this.textLayout == null )  return;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			
			if ( !this.BackColor.IsTransparent )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.BackColor);
			}

			adorner.PaintStatusItemBackground(graphics, rect, state);
			pos.X += this.margin;
			pos.Y += 0.5;
			this.textLayout.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
			adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state);
		}


		protected double			margin = 8.0;
	}
}

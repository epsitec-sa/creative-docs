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
			this.paintTextStyle = PaintTextStyle.StaticText;
		}
		
		public StaticText(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		public StaticText(string text) : this()
		{
			this.Text = text;
		}
		
		
		public override double						DefaultHeight
		{
			// Retourne la hauteur standard.
			get
			{
				return this.DefaultFontHeight;
			}
		}

		public override Drawing.ContentAlignment	DefaultAlignment
		{
			// Retourne l'alignement par défaut d'un bouton.
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		public override Drawing.Size				PreferredSize
		{
			// Retourne les dimensions minimales pour représenter le texte.
			get
			{
				return this.TextLayout.SingleLineSize;
			}
		}
		
		public override Drawing.Point				BaseLine
		{
			get
			{
				if (this.TextLayout != null)
				{
					return this.TextLayout.GetLineOrigin (0);
				}
				
				return base.BaseLine;
			}
		}
		
		public PaintTextStyle PaintTextStyle
		{
			get { return this.paintTextStyle; }
			set { this.paintTextStyle = value; }
		}


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
			
			this.TextLayout.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
			adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state, this.paintTextStyle, this.BackColor);
		}


		protected PaintTextStyle		paintTextStyle;
	}
}

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ArrowButton dessine un bouton flèche.
	/// </summary>
	public class ArrowButton : Button
	{
		public ArrowButton()
		{
			this.glyphType = GlyphType.None;
			this.InternalState &= ~InternalState.AutoFocus;
			this.InternalState &= ~InternalState.Focusable;
		}
		
		public ArrowButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public GlyphType GlyphType
		{
			get
			{
				return this.glyphType;
			}

			set
			{
				this.glyphType = value;
			}
		}
		
		public override double DefaultWidth
		{
			get
			{
				return 17;
			}
		}
		
		public override double DefaultHeight
		{
			get
			{
				return 17;
			}
		}
		

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			Direction dir = Direction.None;
			switch ( this.glyphType )
			{
				case GlyphType.ArrowUp:     dir = Direction.Up;     break;
				case GlyphType.ArrowDown:   dir = Direction.Down;   break;
				case GlyphType.ArrowLeft:   dir = Direction.Left;   break;
				case GlyphType.ArrowRight:  dir = Direction.Right;  break;
			}
			adorner.PaintButtonBackground(graphics, rect, this.PaintState, dir, this.buttonStyle);
			adorner.PaintGlyph(graphics, rect, this.PaintState, this.glyphType, PaintTextStyle.Button);
		}

		
		protected GlyphType				glyphType;
	}
}

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// GlyphShape détermine l'aspect d'un "glyph" représenté par la classe
	/// GlyphButton.
	/// </summary>
	public enum GlyphShape
	{
		None,
		ArrowUp,
		ArrowDown,
		ArrowLeft,
		ArrowRight,
		Menu,
		Close,
		Dots,
	}
	
	/// <summary>
	/// La classe GlyphButton dessine un bouton avec une icône simple.
	/// </summary>
	public class GlyphButton : IconButton
	{
		public GlyphButton() : this (null, null)
		{
		}
		
		public GlyphButton(string command) : this (command, null)
		{
		}
		
		public GlyphButton(string command, string name) : base (command, null, name)
		{
			this.shape = GlyphShape.None;
			this.InternalState &= ~InternalState.AutoFocus;
			this.InternalState &= ~InternalState.Focusable;
		}
		
		public GlyphButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public GlyphShape						GlyphShape
		{
			get
			{
				return this.shape;
			}
			set
			{
				if ( this.shape != value )
				{
					this.shape = value;
					this.Invalidate();
				}
			}
		}
		
		public override double					DefaultWidth
		{
			get { return 17; }
		}
		
		public override double					DefaultHeight
		{
			get { return 17; }
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			Direction dir = Direction.None;
			switch ( this.shape )
			{
				case GlyphShape.ArrowUp:     dir = Direction.Up;     break;
				case GlyphShape.ArrowDown:   dir = Direction.Down;   break;
				case GlyphShape.ArrowLeft:   dir = Direction.Left;   break;
				case GlyphShape.ArrowRight:  dir = Direction.Right;  break;
			}
			
			adorner.PaintButtonBackground(graphics, rect, this.PaintState, dir, this.buttonStyle);
			adorner.PaintGlyph(graphics, rect, this.PaintState, this.shape, PaintTextStyle.Button);
		}

		
		private GlyphShape						shape;
	}
}

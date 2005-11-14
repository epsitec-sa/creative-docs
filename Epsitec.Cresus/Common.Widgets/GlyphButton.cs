//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// GlyphShape d�termine l'aspect d'un "glyph" repr�sent� par la classe
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
		Accept,
		Reject,
		TabLeft,
		TabRight,
		TabCenter,
		TabDecimal,
		TabIndent,
		ResizeKnob,
		Plus,
		Minus,
	}
	
	/// <summary>
	/// La classe GlyphButton dessine un bouton avec une ic�ne simple.
	/// </summary>
	public class GlyphButton : Button
	{
		public GlyphButton()
		{
			this.ButtonStyle = ButtonStyle.Icon;
			this.AutoFocus = false;
			this.InternalState &= ~InternalState.Focusable;
		}
		
		public GlyphButton(string command) : this (command, null)
		{
		}
		
		public GlyphButton(string command, string name) : this ()
		{
			this.Command = command;
			this.Name    = name;
		}
		
		public GlyphButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public GlyphButton(Widget embedder, GlyphShape shape) : this (embedder)
		{
			this.GlyphShape  = shape;
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
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryToolShapeBounds);
			return rect;
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
			
			if ( this.buttonStyle != ButtonStyle.None )
			{
				adorner.PaintButtonBackground(graphics, rect, this.PaintState, dir, this.buttonStyle);
			}

			adorner.PaintGlyph(graphics, rect, this.PaintState, this.shape, PaintTextStyle.Button);
		}

		
		private GlyphShape						shape;
	}
}

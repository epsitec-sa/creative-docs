namespace Epsitec.Common.Widgets
{
	public enum HeaderButtonStyle
	{
		Top,			// bouton dans en-t�te sup�rieure
		Left,			// bouton dans en-t�te gauche
	}
	
	public enum SortMode
	{
		Up		= -1,
		None	= 0,
		Down	= 1
	}
	
	/// <summary>
	/// La class HeaderButton repr�sente un bouton d'un en-t�te de tableau.
	/// </summary>
	public class HeaderButton : AbstractButton
	{
		public HeaderButton()
		{
			//this.internal_state &= ~InternalState.Engageable;
			this.InternalState &= ~InternalState.AutoFocus;
			this.InternalState &= ~InternalState.Focusable;
			this.headerButtonStyle = HeaderButtonStyle.Top;
		}
		
		public HeaderButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}		
		
		// Bouton dans en-t�te sup�rieure ou gauche ?
		public HeaderButtonStyle Style
		{
			get
			{
				return this.headerButtonStyle;
			}

			set
			{
				if ( this.headerButtonStyle != value )
				{
					this.headerButtonStyle = value;
					this.Invalidate();
				}
			}
		}

		// Bouton statique ou dynamique ?
		public bool Dynamic
		{
			get
			{
				return this.dynamic;
			}

			set
			{
				this.dynamic = value;
			}
		}

		// Choix pour le triangle du bouton.
		public SortMode						SortMode
		{
			get
			{
				return this.sortMode;
			}

			set
			{
				if ( this.sortMode != value )
				{
					this.sortMode = value;
					this.Invalidate();
				}
			}
		}

		protected override void UpdateTextLayout()
		{
			base.UpdateTextLayout();
			
			if ( this.TextLayout != null )
			{
				double dx = this.Client.Width - HeaderButton.Margin*2;
				double dy = this.Client.Height;
				this.TextLayout.Alignment = this.Alignment;
				this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();

			if ( !this.dynamic )
			{
				state &= ~WidgetState.Engaged;
				state &= ~WidgetState.Entered;
			}
			
			Direction dir = Direction.None;
			if ( this.headerButtonStyle == HeaderButtonStyle.Top )
			{
				dir = Direction.Up;
			}
			if ( this.headerButtonStyle == HeaderButtonStyle.Left )
			{
				dir = Direction.Left;
			}
			adorner.PaintHeaderBackground(graphics, rect, state, dir);

			if ( this.TextLayout != null )
			{
				pos.X += HeaderButton.Margin;
				this.TextLayout.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, ButtonStyle.Flat);
			}

			if ( this.sortMode != 0 )  // triangle ?
			{
				GlyphShape type = GlyphShape.None;
				type = GlyphShape.None;

				if ( this.headerButtonStyle == HeaderButtonStyle.Top )
				{
					rect.Right = rect.Left+rect.Height;
					rect.Width  *= 0.75;
					rect.Height *= 0.75;

					if ( this.sortMode > 0 )
					{
						type = GlyphShape.ArrowDown;
					}
					else
					{
						type = GlyphShape.ArrowUp;
						rect.Offset(0, rect.Height/3);
					}
				}
				if ( this.headerButtonStyle == HeaderButtonStyle.Left )
				{
					double dim = 14;
					rect.Left   = rect.Right-dim;
					rect.Bottom = rect.Height/2-dim/2;
					rect.Top    = rect.Bottom+dim;

					if ( this.sortMode > 0 )
					{
						type = GlyphShape.ArrowRight;
					}
					else
					{
						type = GlyphShape.ArrowLeft;
					}
				}
				adorner.PaintGlyph(graphics, rect, state, type, PaintTextStyle.Header);
			}
		}
		
		protected HeaderButtonStyle			headerButtonStyle;
		protected bool						dynamic = false;
		protected SortMode					sortMode = SortMode.None;
		
		protected const double				Margin = 2;
	}
}

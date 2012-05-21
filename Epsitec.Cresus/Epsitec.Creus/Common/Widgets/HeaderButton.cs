namespace Epsitec.Common.Widgets
{
	public enum HeaderButtonStyle
	{
		Top,			// bouton dans en-tête supérieure
		Left,			// bouton dans en-tête gauche
	}
	
	public enum SortMode
	{
		Up		= -1,
		None	= 0,
		Down	= 1
	}
	
	/// <summary>
	/// La class HeaderButton représente un bouton d'un en-tête de tableau.
	/// </summary>
	public class HeaderButton : AbstractButton
	{
		public HeaderButton()
		{
			//this.internalState &= ~InternalState.Engageable;
			this.AutoFocus = false;
			this.InternalState &= ~WidgetInternalState.Focusable;
			this.headerButtonStyle = HeaderButtonStyle.Top;
		}
		
		public HeaderButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}		
		
		
		public HeaderButtonStyle			Style
		{
			//	Bouton dans en-tête supérieure ou gauche ?
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

		
		public bool							IsDynamic
		{
			//	Bouton statique ou dynamique ? Un clic sur un bouton dynamique permet de changer
			//	l'ordre des tris (c'est géré par ScrollArray, entre autres).
			get
			{
				return this.isDynamic;
			}

			set
			{
				this.isDynamic = value;
			}
		}

		public bool							IsSortable
		{
			get
			{
				return this.isSortable;
			}
			set
			{
				this.isSortable = value;
			}
		}

		public SortMode						SortMode
		{
			//	Choix pour le triangle du bouton.
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


		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size = base.GetBestFitSize ();
			size.Width += HeaderButton.Margin*2;
			
			return size;
		}
		
		protected override void UpdateTextLayout()
		{
			base.UpdateTextLayout();
			
			if ( this.TextLayout != null )
			{
				double dx = this.Client.Size.Width - HeaderButton.Margin*2;
				double dy = this.Client.Size.Height;
				this.TextLayout.Alignment = this.ContentAlignment;
				this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.GetPaintState ();

			if ( !this.isDynamic )
			{
				state &= ~WidgetPaintState.Engaged;
				state &= ~WidgetPaintState.Entered;
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
				Drawing.Point pos = new Drawing.Point(HeaderButton.Margin, 0);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, ButtonStyle.Flat);
			}

			if ( this.sortMode != 0 )  // triangle ?
			{
				GlyphShape type = GlyphShape.None;
				type = GlyphShape.None;

				if ( this.headerButtonStyle == HeaderButtonStyle.Top )
				{
#if false
					rect.Right = rect.Left+rect.Height;
					rect.Width  *= 0.75;
					rect.Height *= 0.75;

					if ( this.sortMode > 0 )
					{
						type = GlyphShape.TriangleDown;
					}
					else
					{
						type = GlyphShape.TriangleUp;
						rect.Offset(0, rect.Height/3);
					}
#else
					double dim = rect.Height*0.5;
					rect.Left = rect.Center.X-dim/2;
					rect.Width = dim;

					if ( this.sortMode > 0 )
					{
						rect.Bottom -= 4;
						rect.Height = dim;  // triangle en bas au milieu
						type = GlyphShape.TriangleDown;
					}
					else
					{
						rect.Bottom = rect.Top-dim+2;
						rect.Height = dim;  // triangle en haut au milieu
						type = GlyphShape.TriangleUp;
					}
#endif
				}
				if ( this.headerButtonStyle == HeaderButtonStyle.Left )
				{
					double dim = 14;
					rect.Left   = rect.Right-dim;
					rect.Bottom = rect.Height/2-dim/2;
					rect.Top    = rect.Bottom+dim;

					if ( this.sortMode > 0 )
					{
						type = GlyphShape.TriangleRight;
					}
					else
					{
						type = GlyphShape.TriangleLeft;
					}
				}
				adorner.PaintGlyph(graphics, rect, state, type, PaintTextStyle.Header);
			}
		}
		
		
		protected HeaderButtonStyle			headerButtonStyle;
		protected bool						isDynamic = false;
		protected bool						isSortable = true;
		protected SortMode					sortMode = SortMode.None;
		
		protected const double				Margin = 6;
	}
}

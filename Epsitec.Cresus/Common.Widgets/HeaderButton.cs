namespace Epsitec.Common.Widgets
{
	public enum HeaderButtonStyle
	{
		Top,			// bouton dans en-tête supérieure
		Left,			// bouton dans en-tête gauche
	}
	
	/// <summary>
	/// La class HeaderButton représente un bouton d'un en-tête de tableau.
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
		
		// Bouton dans en-tête supérieure ou gauche ?
		public HeaderButtonStyle HeaderButtonStyle
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

		// Rang de la ligne ou de la colonne associé au bouton.
		public int Rank
		{
			get
			{
				return this.rank;
			}

			set
			{
				this.rank = value;
			}
		}

		// Choix pour le triangle du bouton.
		public int SortMode
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

		protected override void UpdateLayoutSize()
		{
			base.UpdateLayoutSize();
			
			if ( this.textLayout != null )
			{
				double dx = this.Client.Width - HeaderButton.Margin*2;
				double dy = this.Client.Height;
				this.textLayout.Alignment = this.Alignment;
				this.textLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);

			if ( !this.dynamic )
			{
				state &= ~WidgetState.Engaged;
				state &= ~WidgetState.Entered;
			}
			
			Direction type = Direction.None;
			if ( this.headerButtonStyle == HeaderButtonStyle.Top )
			{
				type = Direction.Up;
			}
			if ( this.headerButtonStyle == HeaderButtonStyle.Left )
			{
				type = Direction.Left;
			}
			adorner.PaintHeaderBackground(graphics, rect, state, dir, type);

			if ( this.textLayout != null )
			{
				pos.X += HeaderButton.Margin;
				this.textLayout.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				adorner.PaintButtonTextLayout(graphics, pos, this.textLayout, state, dir, ButtonStyle.Flat);
			}

			if ( this.sortMode != 0 )  // triangle ?
			{
				type = Direction.None;

				if ( this.headerButtonStyle == HeaderButtonStyle.Top )
				{
					rect.Right = rect.Left+rect.Height;
					rect.Width  *= 0.75;
					rect.Height *= 0.75;

					if ( this.sortMode > 0 )
					{
						type = Direction.Down;
					}
					else
					{
						type = Direction.Up;
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
						type = Direction.Right;
					}
					else
					{
						type = Direction.Left;
					}
				}
				adorner.PaintArrow(graphics, rect, state, dir, type);
			}
		}
		
		protected HeaderButtonStyle			headerButtonStyle;
		protected bool						dynamic = false;
		protected int						sortMode = 0;
		protected int						rank;
		
		protected const double				Margin = 2;
	}
}

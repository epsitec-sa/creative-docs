namespace Epsitec.Common.Widgets
{
	public enum HeaderSliderStyle
	{
		Top,			// bouton dans en-tête supérieure
		Left,			// bouton dans en-tête gauche
	}
	
	/// <summary>
	/// La class HeaderSlider représente un bouton d'un en-tête de tableau,
	/// permettant de modifier une largeur de colonne ou une hauteur de ligne.
	/// </summary>
	public class HeaderSlider : AbstractButton
	{
		public HeaderSlider()
		{
			this.InternalState &= ~InternalState.Engageable;
			this.HeaderSliderStyle = HeaderSliderStyle.Top;
		}
		
		public HeaderSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		// Type du bouton.
		public HeaderSliderStyle HeaderSliderStyle
		{
			get
			{
				return this.headerSliderStyle;
			}

			set
			{
				if ( this.headerSliderStyle != value )
				{
					this.headerSliderStyle = value;
					this.Invalidate();
				}
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

		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.OnDragStarted(new MessageEventArgs(message, pos));
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.OnDragMoved(new MessageEventArgs(message, pos));
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.OnDragEnded(new MessageEventArgs(message, pos));
						this.mouseDown = false;
					}
					break;
			}
			
			message.Consumer = this;
		}

		// Le slider va être déplacé.
		protected virtual void OnDragStarted(MessageEventArgs e)
		{
			if ( this.DragStarted != null )  // qq'un écoute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragStarted(this, e);
			}
		}

		// Le slider est déplacé.
		protected virtual void OnDragMoved(MessageEventArgs e)
		{
			if ( this.DragMoved != null )  // qq'un écoute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragMoved(this, e);
			}
		}

		// Le slider est fini de déplacer.
		protected virtual void OnDragEnded(MessageEventArgs e)
		{
			if ( this.DragEnded != null )  // qq'un écoute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragEnded(this, e);
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
			
#if false
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));
#endif
			if ( (state & WidgetState.Entered) != 0 || this.mouseDown )
			{
				state |= WidgetState.Entered;
				state &= ~WidgetState.Focused;
				adorner.PaintButtonBackground(graphics, rect, state, dir, ButtonStyle.Normal);
			}
		}
		

		public event MessageEventHandler	DragStarted;
		public event MessageEventHandler	DragMoved;
		public event MessageEventHandler	DragEnded;
		
		protected HeaderSliderStyle			headerSliderStyle;
		protected int						rank;
		protected bool						mouseDown = false;
	}
}

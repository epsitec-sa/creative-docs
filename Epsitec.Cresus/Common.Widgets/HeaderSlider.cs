namespace Epsitec.Common.Widgets
{
	public enum HeaderSliderStyle
	{
		Top,			// bouton dans en-t�te sup�rieure
		Left,			// bouton dans en-t�te gauche
	}
	
	/// <summary>
	/// La class HeaderSlider repr�sente un bouton d'un en-t�te de tableau,
	/// permettant de modifier une largeur de colonne ou une hauteur de ligne.
	/// </summary>
	public class HeaderSlider : AbstractButton
	{
		public HeaderSlider()
		{
			this.InternalState &= ~InternalState.Engageable;
			this.headerSliderStyle = HeaderSliderStyle.Top;
		}
		
		public HeaderSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public HeaderSliderStyle Style
		{
			//	Type du bouton.
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

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un �v�nement.
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

		protected virtual void OnDragStarted(MessageEventArgs e)
		{
			//	Le slider va �tre d�plac�.
			if ( this.DragStarted != null )  // qq'un �coute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragStarted(this, e);
			}
		}

		protected virtual void OnDragMoved(MessageEventArgs e)
		{
			//	Le slider est d�plac�.
			if ( this.DragMoved != null )  // qq'un �coute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragMoved(this, e);
			}
		}

		protected virtual void OnDragEnded(MessageEventArgs e)
		{
			//	Le slider est fini de d�placer.
			if ( this.DragEnded != null )  // qq'un �coute ?
			{
				if ( e != null )
				{
					e.Message.Consumer = this;
				}
				
				this.DragEnded(this, e);
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			
			if ( (state & WidgetPaintState.Entered) != 0 || this.mouseDown )
			{
				state |= WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Focused;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Up, ButtonStyle.HeaderSlider);
			}
		}
		

		public event MessageEventHandler	DragStarted;
		public event MessageEventHandler	DragMoved;
		public event MessageEventHandler	DragEnded;
		
		protected HeaderSliderStyle			headerSliderStyle;
		protected bool						mouseDown = false;
	}
}

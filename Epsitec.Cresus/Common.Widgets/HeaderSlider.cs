using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using MessageEventHandler=Support.EventHandler<MessageEventArgs>;

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
			this.MouseCursor = MouseCursor.AsVSplit;
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

					switch (this.headerSliderStyle)
					{
						case HeaderSliderStyle.Left:
							this.MouseCursor = MouseCursor.AsHSplit;
							break;

						case HeaderSliderStyle.Top:
							this.MouseCursor = MouseCursor.AsVSplit;
							break;
					}
					this.Invalidate();
				}
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un �v�nement.
			switch ( message.MessageType )
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
			MessageEventHandler handler = (MessageEventHandler) this.GetUserEventHandler("DragStarted");
			if (handler != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}

				handler(this, e);
			}
		}

		protected virtual void OnDragMoved(MessageEventArgs e)
		{
			//	Le slider est d�plac�.
			MessageEventHandler handler = (MessageEventHandler) this.GetUserEventHandler ("DragMoved");
			if (handler != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}

				handler(this, e);
			}
		}

		protected virtual void OnDragEnded(MessageEventArgs e)
		{
			//	Le slider est fini de d�placer.
			MessageEventHandler handler = (MessageEventHandler) this.GetUserEventHandler ("DragEnded");
			if (handler != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}

				handler(this, e);
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;
			
			if ( (state & WidgetPaintState.Entered) != 0 || this.mouseDown )
			{
				state |= WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Focused;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Up, ButtonStyle.HeaderSlider);
			}
		}
		

		public event MessageEventHandler	DragStarted
		{
			add
			{
				this.AddUserEventHandler("DragStarted", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragStarted", value);
			}
		}

		public event MessageEventHandler	DragMoved
		{
			add
			{
				this.AddUserEventHandler("DragMoved", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragMoved", value);
			}
		}

		public event MessageEventHandler	DragEnded
		{
			add
			{
				this.AddUserEventHandler("DragEnded", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragEnded", value);
			}
		}

		
		protected HeaderSliderStyle			headerSliderStyle;
		protected bool						mouseDown = false;
	}
}

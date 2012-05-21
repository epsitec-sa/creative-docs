using Epsitec.Common.Support;

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
			this.InternalState &= ~WidgetInternalState.Engageable;
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
			//	Gestion d'un événement.
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
			//	Le slider va être déplacé.
			var handler = this.GetUserEventHandler<MessageEventArgs> ("DragStarted");
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
			//	Le slider est déplacé.
			var handler = this.GetUserEventHandler<MessageEventArgs> ("DragMoved");
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
			//	Le slider est fini de déplacer.
			var handler = this.GetUserEventHandler<MessageEventArgs> ("DragEnded");
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
			WidgetPaintState state = this.GetPaintState ();
			
			if ( (state & WidgetPaintState.Entered) != 0 || this.mouseDown )
			{
				state |= WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Focused;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Up, ButtonStyle.HeaderSlider);
			}
		}


		public event Support.EventHandler<MessageEventArgs> DragStarted
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

		public event Support.EventHandler<MessageEventArgs> DragMoved
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

		public event Support.EventHandler<MessageEventArgs> DragEnded
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

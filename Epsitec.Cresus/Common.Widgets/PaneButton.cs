namespace Epsitec.Common.Widgets
{
	public enum PaneButtonStyle
	{
		Vertical,			// bouton |
		Horizontal,			// bouton -
	}
	
	/// <summary>
	/// La class PaneButton représente un bouton pour déplacer une frontière.
	/// </summary>
	public class PaneButton : AbstractButton
	{
		public PaneButton()
		{
			this.InternalState &= ~InternalState.Engageable;
			this.AutoFocus = false;
			this.InternalState &= ~InternalState.Focusable;

			this.color[0] = Drawing.Color.FromName("ControlLightLight");
			this.color[1] = Drawing.Color.FromName("ControlLight");
			this.color[2] = Drawing.Color.FromName("ControlDark");
			this.color[3] = Drawing.Color.FromName("ControlDarkDark");

			this.MouseCursor = MouseCursor.AsVSplit;
		}
		
		public PaneButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public PaneButtonStyle PaneButtonStyle
		{
			//	Bouton dans en-tête supérieure ou gauche ?
			get
			{
				return this.paneButtonStyle;
			}

			set
			{
				if ( this.paneButtonStyle != value )
				{
					this.paneButtonStyle = value;

					if ( this.paneButtonStyle == PaneButtonStyle.Vertical )
					{
						this.MouseCursor = MouseCursor.AsVSplit;
					}
					if ( this.paneButtonStyle == PaneButtonStyle.Horizontal )
					{
						this.MouseCursor = MouseCursor.AsHSplit;
					}

					this.Invalidate();
				}
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un événement.
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
			//	Le slider va être déplacé.
			if ( this.DragStarted != null )  // qq'un écoute ?
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
			//	Le slider est déplacé.
			if ( this.DragMoved != null )  // qq'un écoute ?
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
			//	Le slider est fini de déplacer.
			if ( this.DragEnded != null )  // qq'un écoute ?
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
			WidgetPaintState state = this.PaintState;
			Direction dir = (this.paneButtonStyle == PaneButtonStyle.Vertical) ? Direction.Down : Direction.Right;

			adorner.PaintPaneButtonBackground(graphics, rect, state, dir);
		}
		
		public event MessageEventHandler	DragStarted;
		public event MessageEventHandler	DragMoved;
		public event MessageEventHandler	DragEnded;
		
		protected PaneButtonStyle			paneButtonStyle;
		protected Drawing.Color[]			color = new Drawing.Color[4];
		protected bool						mouseDown = false;
	}
}

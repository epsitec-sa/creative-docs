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
			this.internal_state &= ~InternalState.Engageable;
			this.internal_state &= ~InternalState.AutoFocus;
			this.internal_state &= ~InternalState.Focusable;

			this.color = Drawing.Color.FromName("ControlDark");
		}
		
		// Bouton dans en-tête supérieure ou gauche ?
		public PaneButtonStyle PaneButtonStyle
		{
			get
			{
				return this.paneButtonStyle;
			}

			set
			{
				if ( this.paneButtonStyle != value )
				{
					this.paneButtonStyle = value;
					this.Invalidate();
				}
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

			if ( this.paneButtonStyle == PaneButtonStyle.Vertical )
			{
				rect.Left  += 1;
				rect.Right -= 1;
			}
			else
			{
				rect.Bottom += 1;
				rect.Top    -= 1;
			}
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(color);
		}
		
		public event MessageEventHandler	DragStarted;
		public event MessageEventHandler	DragMoved;
		public event MessageEventHandler	DragEnded;
		
		protected PaneButtonStyle			paneButtonStyle;
		protected Drawing.Color				color;
		protected bool						mouseDown = false;
	}
}

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
			this.internalState &= ~InternalState.Engageable;
			this.internalState &= ~InternalState.AutoFocus;
			this.internalState &= ~InternalState.Focusable;

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

			double x, y;
			if ( this.paneButtonStyle == PaneButtonStyle.Vertical )
			{
				x = rect.Left+0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(color[0]);

				x = rect.Left+1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(color[1]);

				x = rect.Right-1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(color[2]);

				x = rect.Right-0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(color[3]);
			}
			else
			{
				y = rect.Top-0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(color[0]);

				y = rect.Top-1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(color[1]);

				y = rect.Bottom+1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(color[2]);

				y = rect.Bottom+0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(color[3]);
			}
		}
		
		public event MessageEventHandler	DragStarted;
		public event MessageEventHandler	DragMoved;
		public event MessageEventHandler	DragEnded;
		
		protected PaneButtonStyle			paneButtonStyle;
		protected Drawing.Color[]			color = new Drawing.Color[4];
		protected bool						mouseDown = false;
	}
}

using Epsitec.Common.Support;

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
			this.InternalState &= ~WidgetInternalState.Engageable;
			this.AutoFocus = false;
			this.InternalState &= ~WidgetInternalState.Focusable;

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
			Direction dir = (this.paneButtonStyle == PaneButtonStyle.Vertical) ? Direction.Down : Direction.Right;

			adorner.PaintPaneButtonBackground(graphics, rect, state, dir);
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

		
		protected PaneButtonStyle			paneButtonStyle;
		protected Drawing.Color[]			color = new Drawing.Color[4];
		protected bool						mouseDown = false;
	}
}

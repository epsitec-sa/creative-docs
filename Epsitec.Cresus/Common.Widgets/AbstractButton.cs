namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractButton implémente les fonctions de base communes de tous
	/// les boutons (notamment au niveau de la gestion des événements).
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractButton : Widget
	{
		public AbstractButton()
		{
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.AutoEngage;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
		}
		
		public AbstractButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override Drawing.ContentAlignment	DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleCenter;
			}
		}
		
		public override Drawing.Point				BaseLine
		{
			get
			{
				if ( this.TextLayout != null )
				{
					return this.MapClientToParent (this.TextLayout.GetLineOrigin (0)) - this.Location;
				}
				
				return base.BaseLine;
			}
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryButtonShapeBounds);
			return rect;
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseEnter:
				case MessageType.MouseDown:
				case MessageType.MouseUp:
				case MessageType.MouseLeave:
				case MessageType.MouseMove:
					break;
				
				case MessageType.KeyPress:
					if ( message.IsAltPressed || message.IsCtrlPressed )
					{
						return;
					}
					
					if ( Feel.Factory.Active.TestPressButtonKey(message) &&
						 this.isKeyboardPressed == false  )
					{
						this.isKeyboardPressed = true;
						this.OnShortcutPressed();
						break;
					}
					return;
				
				case MessageType.KeyUp:
					if ( this.isKeyboardPressed )
					{
						this.isKeyboardPressed = false;
						break;
					}
					return;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}
		
		protected override void OnShortcutPressed()
		{
			if ( this.AutoToggle )
			{
				this.Toggle();
			}
			else
			{
				this.SimulatePressed();
				this.SimulateReleased();
				this.SimulateClicked();
			}
			
			base.OnShortcutPressed();
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
			this.Invalidate ();
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
			this.Invalidate ();
		}

		
		
		protected bool								isKeyboardPressed;
	}
}

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
				if (this.TextLayout != null)
				{
					return this.TextLayout.GetLineOrigin (0);
				}
				
				return base.BaseLine;
			}
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseEnter:
					break;
				
				case MessageType.MouseDown:
					break;
				
				case MessageType.MouseUp:
					break;
				
				case MessageType.MouseLeave:
					break;
				
				case MessageType.KeyPress:
					if ( message.IsAltPressed || message.IsCtrlPressed )
					{
						return;
					}
					
					if ( message.KeyCode == KeyCode.Space &&
						 this.isKeyboardPressed == false  )
					{
						this.isKeyboardPressed = true;
						this.OnShortcutPressed();
					}
					
					break;
				
				case MessageType.KeyUp:
					this.isKeyboardPressed = false;
					break;
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
				this.Window.EngagedWidget = this;
				this.SimulatePressed();
			}
			
			base.OnShortcutPressed();
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryButtonShapeBounds);
			return rect;
		}


		protected bool								isKeyboardPressed;
	}
}

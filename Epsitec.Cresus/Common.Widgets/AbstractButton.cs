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
			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.AutoEngage;
			this.internalState |= InternalState.Focusable;
			this.internalState |= InternalState.Engageable;
		}
		
		
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleCenter;
			}
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
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
					if (message.IsAltPressed || message.IsCtrlPressed)
					{
						return;
					}
					
					if ((message.KeyCode == KeyCode.Space) &&
						(this.is_keyboard_pressed == false))
					{
						this.is_keyboard_pressed = true;
						this.OnShortcutPressed ();
					}
					
					break;
				
				case MessageType.KeyUp:
					this.is_keyboard_pressed = false;
					break;
			}
			
			message.Consumer = this;
		}
		

		protected override void OnShortcutPressed()
		{
			if (this.AutoToggle)
			{
				this.Toggle ();
			}
			else
			{
				this.Window.EngagedWidget = this;
				this.SimulatePressed ();
			}
			
			base.OnShortcutPressed ();
		}
		
		
		protected bool					is_keyboard_pressed;
	}
}

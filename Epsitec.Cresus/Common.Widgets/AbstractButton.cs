namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractButton implémente les fonctions de base communes de tous
	/// les boutons (notamment au niveau de la gestion des événements).
	/// </summary>
	public abstract class AbstractButton : Widget
	{
		public AbstractButton()
		{
			this.internal_state |= InternalState.AcceptTaggedText;
			this.internal_state |= InternalState.AutoFocus;
			this.internal_state |= InternalState.AutoEngage;
			this.internal_state |= InternalState.Focusable;
			this.internal_state |= InternalState.Engageable;
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
					
					if ((message.KeyCode == ' ') &&
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
				this.WindowFrame.EngagedWidget = this;
			}
			
			base.OnShortcutPressed ();
		}
		
		
		protected bool					is_keyboard_pressed;
	}
}

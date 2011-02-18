namespace Epsitec.Common.Widgets.Feel
{
	/// <summary>
	/// Implémentation du "feel" par défaut.
	/// </summary>
	public class Default : IFeel
	{
		public Default()
		{
		}
		
		
		#region IFeel Members
		public bool TestAcceptKey(Epsitec.Common.Widgets.Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsShiftPressed == false) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				return (message.KeyCode == KeyCode.Return);
			}
			
			return false;
		}
		
		public bool TestSelectItemKey(Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsShiftPressed == false) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				switch (message.KeyCode)
				{
					case KeyCode.Space:
					case KeyCode.Return:
						return true;
				
					default:
						return false;
				}
			}
			
			return false;
		}
		
		public bool TestPressButtonKey(Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsShiftPressed == false) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				return (message.KeyCode == KeyCode.Space);
			}
			
			return false;
		}
		
		public bool TestCancelKey(Epsitec.Common.Widgets.Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsShiftPressed == false) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				return (message.KeyCode == KeyCode.Escape);
			}
			
			return false;
		}
		
		public bool TestNavigationKey(Epsitec.Common.Widgets.Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				return (message.KeyCode == KeyCode.Tab);
			}
			
			return false;
		}
		
		public bool TestComboOpenKey(Message message)
		{
			if (((message.MessageType == MessageType.KeyPress) || (message.MessageType == MessageType.KeyDown)) &&
				(message.IsShiftPressed == false) &&
				(message.IsControlPressed == false) &&
				(message.IsAltPressed == false))
			{
				switch (message.KeyCode)
				{
					case KeyCode.ArrowUp:
					case KeyCode.ArrowDown:
						return true;
				
					default:
						return false;
				}
			}
			
			return false;
		}
		
		
		public Shortcut							AcceptShortcut
		{
			get
			{
				return new Shortcut (KeyCode.Return);
			}
		}
		
		public Shortcut							CancelShortcut
		{
			get
			{
				return new Shortcut(KeyCode.Escape);
			}
		}
		#endregion
	}
}

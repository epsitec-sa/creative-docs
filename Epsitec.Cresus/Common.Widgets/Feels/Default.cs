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
			return (message.KeyCode == KeyCode.Return);
		}
		
		public bool TestSelectItemKey(Message message)
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
		
		public bool TestPressButtonKey(Message message)
		{
			return (message.KeyCode == KeyCode.Space);
		}
		
		public bool TestCancelKey(Epsitec.Common.Widgets.Message message)
		{
			return (message.KeyCode == KeyCode.Escape);
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

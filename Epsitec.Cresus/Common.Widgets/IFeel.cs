namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IFeel donne accès aux fonctions propres au "feel" de l'interface
	/// graphique (IAdorner gère le "look" du "look & feel", IFeel the "feel").
	/// </summary>
	public interface IFeel
	{
		bool TestAcceptKey(Message message);			//	touche RETURN
		bool TestSelectItemKey(Message message);		//	touche SPACE ou RETURN
		bool TestPressButtonKey(Message message);		//	touche SPACE
		bool TestCancelKey(Message message);			//	touche ESCAPE
		bool TestNavigationKey(Message message);		//	touche de navigation dans l'interface
		bool TestComboOpenKey(Message message);			//	touche Arrow Up ou touche Arrow Down
		
		Shortcut		AcceptShortcut		{ get; }
		Shortcut		CancelShortcut		{ get; }
	}
}

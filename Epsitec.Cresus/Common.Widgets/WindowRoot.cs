namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot impl�mente le fond de chaque fen�tre. L'utilisateur obtient
	/// en g�n�ral une instance de WindowRoot en appeland WindowFrame.Root.
	/// </summary>
	public class WindowRoot : Widget
	{
		public WindowRoot()
		{
		}
		
		protected override bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			if (base.ShortcutHandler (shortcut, execute_focused) == false)
			{
				//	Le raccourci clavier n'a pas �t� consomm�. Il faut voir si le raccourci clavier
				//	est attach� � une commande globale.
				
				//	TODO: g�re les commandes globales
				
				return false;
			}
			
			return true;
		}

	}
}

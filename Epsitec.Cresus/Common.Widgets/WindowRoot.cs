namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot implémente le fond de chaque fenêtre. L'utilisateur obtient
	/// en général une instance de WindowRoot en appeland WindowFrame.Root.
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
				//	Le raccourci clavier n'a pas été consommé. Il faut voir si le raccourci clavier
				//	est attaché à une commande globale.
				
				//	TODO: gère les commandes globales
				
				return false;
			}
			
			return true;
		}

	}
}

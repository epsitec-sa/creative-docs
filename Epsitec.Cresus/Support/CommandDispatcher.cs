//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher
	{
		public CommandDispatcher()
		{
		}
		
		public void Dispatch(string command, object source)
		{
			//	Transmet la commande à ceux qui sont intéressés
			
			System.Diagnostics.Debug.WriteLine ("Command: " + command);
			System.Diagnostics.Debug.WriteLine ("Source:  " + source);
			
			if (this.handlers.Contains (command))
			{
				(this.handlers[command] as Slot).Fire (source, new CommandEventArgs (this, command));
			}
		}
		
		public void Register(string command, CommandEventHandler handler)
		{
			if (! this.handlers.Contains (command))
			{
				this.handlers[command] = new Slot ();
			}
			
			(this.handlers[command] as Slot).Command += handler;
		}
		
		public void Unregister(string command, CommandEventHandler handler)
		{
			if (this.handlers.Contains (command))
			{
				(this.handlers[command] as Slot).Command -= handler;
			}
		}
		
		
		protected class Slot
		{
			public Slot()
			{
			}
			
			public void Fire(object sender, CommandEventArgs e)
			{
				if (this.Command != null)
				{
					this.Command (sender, e);
				}
			}
			
			public event CommandEventHandler	Command;
		}
		
		public static CommandDispatcher			Default
		{
			get { return CommandDispatcher.def; }
		}
		
		
		protected System.Collections.Hashtable	handlers = new System.Collections.Hashtable ();
		
		private static CommandDispatcher		def = new CommandDispatcher ();
	}
}

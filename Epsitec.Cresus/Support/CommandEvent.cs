//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandEvent représente une commande, telle que transmise
	/// par le CommandDispatcher.
	/// </summary>
	public class CommandEventArgs : System.EventArgs
	{
		public CommandEventArgs(CommandDispatcher dispatcher, string name)
		{
			this.dispatcher = dispatcher;
			this.name       = name;
		}
		
		public CommandDispatcher		Dispatcher
		{
			get { return this.dispatcher; }
		}
		
		public string					Name
		{
			get { return this.name; }
		}
		
		
		protected CommandDispatcher		dispatcher;
		protected string				name;
	}
	
	public delegate void CommandEventHandler(object sender, CommandEventArgs e);
}

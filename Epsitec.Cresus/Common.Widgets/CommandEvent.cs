//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandEvent représente une commande, telle que transmise
	/// par le CommandDispatcher.
	/// </summary>
	public class CommandEventArgs : System.EventArgs
	{
		public CommandEventArgs(object source, string commandName, string[] args)
		{
			this.source  = source;
			this.command = commandName;
			this.args    = args;
		}
		
		
		public object							Source
		{
			get
			{
				return this.source;
			}
		}
		
		public string							CommandName
		{
			get
			{
				return this.command;
			}
		}
		
		public string[]							CommandArgs
		{
			get
			{
				return this.args;
			}
		}
		
		public bool								Executed
		{
			get
			{
				return this.executed;
			}
			set
			{
				this.executed = value;
			}
		}
		
		
		private object							source;
		private string							command;
		private string[]						args;
		private bool							executed;
	}
	
	/// <summary>
	/// Le delegate CommandEventHandler permet d'exécuter une commande envoyée
	/// par un CommandDispatcher et décrite par l'événement associé.
	/// </summary>
	public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}

//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandEvent représente une commande, telle que transmise
	/// par le CommandDispatcher.
	/// </summary>
	public class CommandEventArgs : System.EventArgs
	{
		public CommandEventArgs(object source, string command)
		{
			this.source  = source;
			this.command = command;
		}
		
		public object					Source
		{
			get { return this.source; }
		}
		
		public string					CommandName
		{
			get { return CommandDispatcher.ExtractCommandName (this.command); }
		}
		
		public string[]					CommandArgs
		{
			get { return CommandDispatcher.ExtractCommandArgs (this.command); }
		}
		
		public string					CommandText
		{
			get { return this.command; }
		}
		
		protected object				source;
		protected string				command;
	}
	
	public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}

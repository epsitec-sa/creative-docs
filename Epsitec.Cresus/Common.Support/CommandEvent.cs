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
		public CommandEventArgs(object source, string name)
		{
			this.source = source;
			this.name   = name;
		}
		
		public object					Source
		{
			get { return this.source; }
		}
		
		public string					Name
		{
			get { return this.name; }
		}
		
		
		protected object				source;
		protected string				name;
	}
	
	public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}

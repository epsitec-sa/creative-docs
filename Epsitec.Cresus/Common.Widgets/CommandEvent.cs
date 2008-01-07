//	Copyright � 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandEvent repr�sente une commande, telle que transmise
	/// par le CommandDispatcher.
	/// </summary>
	public class CommandEventArgs : System.EventArgs
	{
		public CommandEventArgs(object source, Command command, CommandContext context, CommandState state)
		{
			this.source  = source;
			this.command = command;
			this.context = context;
			this.state   = state;
		}
		
		
		public object							Source
		{
			get
			{
				return this.source;
			}
		}
		
		public Command							Command
		{
			get
			{
				return this.command;
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

		public CommandContext					CommandContext
		{
			get
			{
				return this.context;
			}
		}

		public CommandState						CommandState
		{
			get
			{
				return this.state;
			}
		}
		
		
		private object							source;
		private Command							command;
		private CommandContext					context;
		private CommandState					state;
		private bool							executed;
	}
	
	/// <summary>
	/// Le delegate CommandEventHandler permet d'ex�cuter une commande envoy�e
	/// par un CommandDispatcher et d�crite par l'�v�nement associ�.
	/// </summary>
	public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}

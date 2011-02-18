//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandEventArgs</c> class describes a command dispatched by the
	/// <see cref="CommandDispatcher"/>.
	/// </summary>
	public class CommandEventArgs : Support.CancelEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="command">The command.</param>
		/// <param name="contextChain">The context chain.</param>
		/// <param name="context">The context.</param>
		/// <param name="state">The state.</param>
		public CommandEventArgs(object source, Command command, CommandContextChain contextChain, CommandContext context, CommandState state)
		{
			this.source  = source;
			this.command = command;
			this.chain   = contextChain;
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
			get;
			set;
		}

		public bool								Handled
		{
			get;
			set;
		}

		public CommandContext					CommandContext
		{
			get
			{
				return this.context;
			}
		}

		public CommandContextChain				CommandContextChain
		{
			get
			{
				return this.chain;
			}
		}

		public CommandState						CommandState
		{
			get
			{
				return this.state;
			}
		}


		private readonly object					source;
		private readonly Command				command;
		private readonly CommandContextChain	chain;
		private readonly CommandContext			context;
		private readonly CommandState			state;
	}
	
	/// <summary>
	/// Le delegate CommandEventHandler permet d'exécuter une commande envoyée
	/// par un CommandDispatcher et décrite par l'événement associé.
	/// </summary>
	public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}

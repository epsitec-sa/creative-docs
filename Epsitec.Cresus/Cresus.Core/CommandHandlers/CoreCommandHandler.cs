//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class CoreCommandHandler
	{
		public CoreCommandHandler(CoreApplication application)
		{
			this.dispatcher = application.CommandDispatcher;
			this.commandContext = application.CommandContext;
			this.commandHandlers = new Dictionary<Command, CommandHandlerStack> ();

			this.dispatcher.RegisterController (this);
		}


		public void PushHandler(Command command, System.Action handler)
		{
			var stack = this.GetCommandHandlerStack (command);
			stack.Push (handler);
		}

		public void PopHandler(Command command)
		{
			var stack = this.GetCommandHandlerStack (command);
			stack.Pop ();
		}

		[Command (Res.CommandIds.Edition.SaveRecord)]
		public void ProcessSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			CommandHandlerStack stack;

			if ((this.commandHandlers.TryGetValue (e.Command, out stack)) &&
				(stack.ContainsCommandHandlers))
			{
				stack.Execute ();
			}
		}


		private CommandHandlerStack GetCommandHandlerStack(Command command)
		{
			CommandHandlerStack stack;

			if (this.commandHandlers.TryGetValue (command, out stack) == false)
			{
				stack = new CommandHandlerStack (command);
				this.commandHandlers[command] = stack;
			}

			return stack;
		}

		private readonly CommandDispatcher dispatcher;
		private readonly CommandContext commandContext;
		private readonly Dictionary<Command, CommandHandlerStack> commandHandlers;
	}

}

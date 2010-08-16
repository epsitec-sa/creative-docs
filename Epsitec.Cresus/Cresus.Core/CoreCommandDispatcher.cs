//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreCommands</c> class implements the application wide commands.
	/// </summary>
	public sealed class CoreCommandDispatcher
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreCommandDispatcher"/> class.
		/// </summary>
		/// <param name="application">The application.</param>
		public CoreCommandDispatcher(CoreApplication application)
		{
			this.application     = application;
			this.dispatcher      = application.CommandDispatcher;
			this.commandContext  = application.CommandContext;
			this.commandHandlers = new List<ICommandHandler> ();
			this.commandHandlerStack = new Dictionary<Command, CommandHandlerStack> ();

			this.CreateCommandHandlers ();
			this.RegisterCommandHandlers ();
		}


		private void CreateCommandHandlers()
		{
			this.commandHandlers.Add (new CommandHandlers.CoreCommandHandler (this));
			this.commandHandlers.Add (new CommandHandlers.DatabaseCommandHandler (this));
			this.commandHandlers.Add (new CommandHandlers.NavigationCommandHandler (this));
		}

		private void RegisterCommandHandlers()
		{
			foreach (var handler in this.commandHandlers)
			{
				this.dispatcher.RegisterController (handler);
				this.commandContext.AttachCommandHandler (handler);
			}
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

		public bool DispatchGenericCommand(Command command)
		{
			CommandHandlerStack stack;

			var state = this.commandContext.GetCommandState (command);

			if ((state.Enable) &&
				(this.commandHandlerStack.TryGetValue (command, out stack)) &&
				(stack.ContainsCommandHandlers))
			{
				stack.Execute ();
				return true;
			}

			return false;
		}

		public CommandState GetState(Command command)
		{
			return this.commandContext.GetCommandState (command);
		}



		private CommandHandlerStack GetCommandHandlerStack(Command command)
		{
			CommandHandlerStack stack;

			if (this.commandHandlerStack.TryGetValue (command, out stack) == false)
			{
				stack = new CommandHandlerStack (command);
				this.commandHandlerStack[command] = stack;
			}

			return stack;
		}

		private readonly CoreApplication application;
		private readonly CommandDispatcher dispatcher;
		private readonly CommandContext commandContext;
		private readonly Dictionary<Command, CommandHandlerStack> commandHandlerStack;
		private readonly List<ICommandHandler> commandHandlers;
	}
}

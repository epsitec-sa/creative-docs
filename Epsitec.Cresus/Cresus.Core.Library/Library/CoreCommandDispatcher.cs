//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreCommands</c> class implements the application wide commands.
	/// </summary>
	public sealed class CoreCommandDispatcher : CoreAppComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreCommandDispatcher"/> class.
		/// </summary>
		/// <param name="app">The application.</param>
		private CoreCommandDispatcher(CoreApp app)
			: base (app)
		{
			this.dispatcher      = app.CommandDispatcher;
			this.commandContext  = app.CommandContext;
			this.commandHandlers = new List<ICommandHandler> ();
			this.commandHandlerStack = new Dictionary<Command, CommandHandlerStack> ();
		}


		public CommandDispatcher CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}

		public CommandContext CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		public override void ExecuteSetupPhase()
		{
			this.CreateCommandHandlers ();
			this.RegisterCommandHandlers ();
			this.SetupDefaultCommandStates ();
			
			base.ExecuteSetupPhase ();
		}

		public List<ICommandHandler> CommandHandlers
		{
			get
			{
				return this.commandHandlers;
			}
		}


		public T GetApplicationComponent<T>()
			where T : class, ICoreComponent
		{
			return this.Host.FindComponent<T> ();
		}

		private void SetupDefaultCommandStates()
		{
			this.Host.SetEnable (ApplicationCommands.Cut, false);
			this.Host.SetEnable (ApplicationCommands.Copy, false);
			this.Host.SetEnable (ApplicationCommands.Paste, false);
			this.Host.SetEnable (ApplicationCommands.Bold, false);
			this.Host.SetEnable (ApplicationCommands.Italic, false);
			this.Host.SetEnable (ApplicationCommands.Underlined, false);
			this.Host.SetEnable (ApplicationCommands.Subscript, false);
			this.Host.SetEnable (ApplicationCommands.Superscript, false);
			this.Host.SetEnable (ApplicationCommands.MultilingualEdition, false);
		}


		private void CreateCommandHandlers()
		{
			this.commandHandlers.AddRange (Factories.CoreCommandHandlerFactory.CreateCommandHandlers (this));
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

#if false
		public static Orchestrators.DataViewOrchestrator GetOrchestrator(CommandEventArgs e)
		{
			var controller = MainViewController.Find (e.CommandContextChain);

			if (controller == null)
			{
				return null;
			}
			else
			{
				return controller.Orchestrator;
			}
		}
#endif

		public void Dispatch(CommandDispatcher dispatcher, CommandEventArgs e, System.Action<Command> action = null)
		{
			var widget = e.Source as Widget;

			if (widget.KeyboardFocus)
			{
				widget.ClearFocus ();
			}
			else
			{
				widget = null;
			}

			if (action == null)
			{
				this.DispatchGenericCommand (e.Command);
			}
			else
			{
				action (e.Command);
			}

			if (widget != null)
			{
				widget.SetFocusOnTabWidget ();
			}
		}

		private sealed class CoreCommandDispatcherFactory : ICoreAppComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreApp data)
			{
				return true;
			}

			public CoreAppComponent Create(CoreApp app)
			{
				return new CoreCommandDispatcher (app);
			}

			public System.Type GetComponentType()
			{
				return typeof (CoreCommandDispatcher);
			}

			#endregion
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

		private readonly CommandDispatcher dispatcher;
		private readonly CommandContext commandContext;
		private readonly Dictionary<Command, CommandHandlerStack> commandHandlerStack;
		private readonly List<ICommandHandler> commandHandlers;
	}
}

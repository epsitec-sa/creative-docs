//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandContextChain</c> represents a chain (some kind of read-only stack)
	/// with all the <c>CommandContext</c> objects found when walking up a visual tree.
	/// The first command context will be the one which is nearest to the visual where
	/// the start was initiated.
	/// </summary>
	public sealed class CommandContextChain
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CommandContextChain"/> class.
		/// </summary>
		private CommandContextChain()
		{
			this.contexts = new List<Weak<CommandContext>> ();
		}

		/// <summary>
		/// Gets the command contexts.
		/// </summary>
		/// <value>The context enumeration.</value>
		public IEnumerable<CommandContext>		Contexts
		{
			get
			{
				if (this.contexts.Count > 0)
				{
					Weak<CommandContext>[] chain = this.contexts.ToArray ();

					for (int i = 0; i < chain.Length; i++)
					{
						CommandContext context = chain[i].Target;

						if (context == null)
						{
							this.contexts.Remove (chain[i]);
						}
						else
						{
							yield return context;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the first context in the context chain.
		/// </summary>
		/// <value>The first context or <c>null</c> if the chain is empty.</value>
		public CommandContext					FirstContext
		{
			get
			{
				return this.Contexts.FirstOrDefault ();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this chain is empty.
		/// </summary>
		/// <value><c>true</c> if this chain is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return this.contexts.Any (x => x.IsAlive) ? false : true;
			}
		}

		/// <summary>
		/// Gets the local enable state of the command. This walks all command
		/// contexts until either a local disable is found or a fence context
		/// is reached. If no local enable was found, use the default enable
		/// mode for the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns><c>false</c> if the command is disabled locally; otherwise, <c>true</c>.</returns>
		public bool GetLocalEnable(Command command)
		{
			bool defined = false;

			foreach (CommandContext context in this.Contexts)
			{
				bool? enable = context.GetLocalEnable (command);

				if (enable == false)
				{
					return false;
				}
				else if (enable.HasValue)
				{
					defined = true;
				}

				if (context.Fence)
				{
					break;
				}
			}

			if (defined)
			{
				return true;
			}
			else
			{
				//	If the command was neither disabled nor enabled in any of the contexts,
				//	use the command's default enable mode:

				var defaultEnableMode = command.CommandParameters.GetValueOrDefault (CommandDefaultEnableMode.Enabled);

				return defaultEnableMode != CommandDefaultEnableMode.Disabled;
			}
		}

		/// <summary>
		/// Builds the command context chain based on a visual.
		/// </summary>
		/// <param name="visual">The visual from where to search for the command contexts.</param>
		/// <returns>The command context chain.</returns>
		public static CommandContextChain BuildChain(Visual visual)
		{
			CommandContextChain that = null;

			var window = visual.Window;

			if (window != null)
			{
				CommandContextChain.BuildChain (window.FocusedWidget, ref that);
				CommandContextChain.BuildChainBasedOnChildren (window.Root, ref that, c => c.ActiveWithoutFocus);
			}

			CommandContextChain.BuildChain (visual, ref that);
			CommandContextChain.BuildChain (window, ref that);

			return that;
		}

		/// <summary>
		/// Builds the command context chain based on a window.
		/// </summary>
		/// <param name="window">The window from where to search for the command contexts.</param>
		/// <returns>The command context chain.</returns>
		public static CommandContextChain BuildChain(Window window)
		{
			CommandContextChain that = null;

			CommandContextChain.BuildChain (window, ref that);

			return that;
		}

		/// <summary>
		/// Builds the command context chain based on a dependency object. The
		/// resulting chain will either have zero or one element, unless the
		/// dependency object maps to a <c>Visual</c>, in which case it can
		/// have more elements.
		/// </summary>
		/// <param name="obj">The dependency object to consider.</param>
		/// <returns>The command context chain.</returns>
		public static CommandContextChain BuildChain(DependencyObject obj)
		{
			CommandContextChain that = null;

			CommandContextChain.BuildChain (obj, ref that);

			return that;
		}

		/// <summary>
		/// Gets the state of the command.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <returns>The command state.</returns>
		public CommandState GetCommandState(string commandName)
		{
			Command command = Command.Get (commandName);
			CommandContext context;
			
			return this.GetCommandState (command, out context);
		}

		/// <summary>
		/// Gets the state of the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>The command state.</returns>
		public CommandState GetCommandState(Command command)
		{
			CommandContext context;
			return this.GetCommandState (command, out context);
		}
		
		/// <summary>
		/// Gets the state of the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="context">The context where the command state was found, or <c>null</c> if none was found.</param>
		/// <returns>The command state.</returns>
		public CommandState GetCommandState(Command command, out CommandContext context)
		{
			CommandContext root = null;
			CommandState   state;

			foreach (CommandContext item in this.Contexts)
			{
				state = item.FindCommandState (command);
				
				if (state != null)
				{
					context = item;
					return state;
				}

				root = item;

				if (item.Fence)
				{
					break;
				}
			}

			if (root != null)
			{
				//	Create the command state in the root-level context.

				context = root;
				state   = context.GetCommandState (command);
			}
			else
			{
				context = null;
				state   = null;
			}
			
			return state;
		}

		private static CommandContextChain BuildChain(DependencyObject obj, ref CommandContextChain chain)
		{
			Visual visual = obj as Visual;
			Window window = obj as Window;

			if (visual != null)
			{
				CommandContextChain.BuildChain (visual, ref chain);
				return chain;
			}
			
			if (window != null)
			{
				CommandContextChain.BuildChain (window, ref chain);
				return chain;
			}

			return CommandContextChain.AddItemBoundCommandContextToChain (obj);
		}

		private static void BuildChain(Visual visual, ref CommandContextChain that)
		{
			while (visual != null)
			{
				that = CommandContextChain.AddItemBoundCommandContextToChain (visual, that);

				AbstractMenu menu = visual as AbstractMenu;

				if (menu != null)
				{
					CommandContextChain.BuildChain (menu.Host, ref that);
				}

				visual = visual.Parent;
			}
		}

		private static void BuildChainBasedOnChildren(Visual visual, ref CommandContextChain chain, System.Predicate<CommandContext> predicate)
		{
			if (visual != null)
			{
				var visibleChildren = visual.GetAllChildren (x => x.IsVisible);
				var activeContexts  = visibleChildren.Select (x => CommandContext.GetContext (x)).Where (x => (x != null) && predicate (x));
				
				foreach (var context in activeContexts)
				{
					if (chain == null)
					{
						chain = new CommandContextChain ();
					}

					chain.contexts.Add (new Weak<CommandContext> (context));
				}
			}
		}

		private static void BuildChain(Window window, ref CommandContextChain that)
		{
			while (window != null)
			{
				that = CommandContextChain.AddItemBoundCommandContextToChain (window, that);

				window = window.Owner ?? window.Parent;
			}

			//	TODO: ajouter ici la notion d'application/module/document
		}

		private static CommandContextChain AddItemBoundCommandContextToChain(DependencyObject item, CommandContextChain chain = null)
		{
			if (item == null)
			{
				return chain;
			}

			CommandContext context = CommandContext.GetContext (item);

			if (context != null)
			{
				if (chain == null)
				{
					chain = new CommandContextChain ();
				}

				chain.contexts.Add (new Weak<CommandContext> (context));
			}

			return chain;
		}

		private readonly List<Weak<CommandContext>>		contexts;
	}
}

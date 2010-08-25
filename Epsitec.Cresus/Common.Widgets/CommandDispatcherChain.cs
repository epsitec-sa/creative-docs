//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandDispatcherChain</c> class represents a chain of
	/// <see cref="CommandDispatcher"/>, which are visible from a given starting
	/// point in the visual tree (for instance).
	/// </summary>
	public sealed class CommandDispatcherChain
	{
		public CommandDispatcherChain()
		{
			this.chain = new List<Weak<CommandDispatcher>> ();
		}

		/// <summary>
		/// Gets a value indicating whether this chain is empty.
		/// </summary>
		/// <value><c>true</c> if this chain is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				for (int i = 0; i < this.chain.Count; i++)
				{
					if (this.chain[i].IsAlive)
					{
						return false;
					}
				}
				
				return true;
			}
		}

		/// <summary>
		/// Enumerates the dispatchers found in the chain.
		/// </summary>
		/// <value>The dispatchers.</value>
		public IEnumerable<CommandDispatcher>	Dispatchers
		{
			get
			{
				Weak<CommandDispatcher>[] chain = this.chain.ToArray ();
				bool enableForwarding = true;

				for (int i = 0; i < chain.Length; i++)
				{
					CommandDispatcher dispatcher = chain[i].Target;

					if (dispatcher == null)
					{
						this.chain.Remove (chain[i]);
					}
					else if (enableForwarding)
					{
						yield return dispatcher;

						//	Stop forwarding as soon as the command dispatcher says so or we
						//	have reached an application level dispatcher; we don't bubble
						//	the commands from one application to another.
						
						enableForwarding = dispatcher.AutoForwardCommands && (dispatcher.Level != CommandDispatcherLevel.Root);
					}
				}
			}
		}

		/// <summary>
		/// Checks if a command is known by some dispatcher in the command chain.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="depth">The depth at which the dispatcher was found.</param>
		/// <returns><c>true</c> if the chain contains a handler for the command;
		/// <c>false</c> otherwise.</returns>
		public bool Contains(Command command, out int depth)
		{
			depth = 0;
			
			foreach (CommandDispatcher dispatcher in this.Dispatchers)
			{
				if (dispatcher.Contains (command))
				{
					return true;
				}
				
				depth++;
			}
			
			return false;
		}

		/// <summary>
		/// Gets the best command in an enumeration. In this case, best means
		/// the command which is topmost in the chain. This is used by the
		/// <see cref="WindowRoot"/> class to decide which command to map to a
		/// multiply assigned keyboard shortcut, for instance.
		/// </summary>
		/// <param name="commands">The commands.</param>
		/// <returns>The best command found or <c>null</c>.</returns>
		public Command GetBestCommand(IEnumerable<Command> commands)
		{
			int     nearest  = int.MaxValue;
			Command selected = null;

			foreach (Command command in commands)
			{
				int depth;
				
				if ((this.Contains (command, out depth)) &&
					(depth < nearest))
				{
					nearest  = depth;
					selected = command;
				}
			}
			
			return selected;
		}


		/// <summary>
		/// Builds the dispatcher chain starting from the specified visual.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The dispatcher chain or <c>null</c>.</returns>
		public static CommandDispatcherChain BuildChain(Visual visual)
		{
			CommandDispatcherChain that = null;

			var window = visual.Window;

#if true
			if ((window != null) &&
				(window.FocusedWidget != null))
			{
				CommandDispatcherChain.BuildChain (window.FocusedWidget, ref that);
			}
#endif
			
			CommandDispatcherChain.BuildChain (visual, ref that);
			CommandDispatcherChain.BuildChain (window, ref that);

			return that;
		}

		/// <summary>
		/// Builds the dispatcher chain starting from the specified window.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>The dispatcher chain or <c>null</c>.</returns>
		public static CommandDispatcherChain BuildChain(Window window)
		{
			CommandDispatcherChain that = null;

			CommandDispatcherChain.BuildChain (window, ref that);

			return that;
		}

		/// <summary>
		/// Builds the dispatcher chain starting from the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The dispatcher chain or <c>null</c>.</returns>
		public static CommandDispatcherChain BuildChain(DependencyObject obj)
		{
			Visual visual = obj as Visual;
			Window window = obj as Window;

			if (visual != null)
			{
				return CommandDispatcherChain.BuildChain (visual);
			}
			if (window != null)
			{
				return CommandDispatcherChain.BuildChain (window);
			}
			
			CommandDispatcherChain that = null;

			if (obj != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (obj);

				if (dispatcher != null)
				{
					if (that == null)
					{
						that = new CommandDispatcherChain ();
					}
					
					that.chain.Add (new Weak<CommandDispatcher> (dispatcher));
				}
			}
			
			return that;
		}

		#region Private Methods

		private static void BuildChain(DependencyObject obj, ref CommandDispatcherChain that)
		{
			Visual visual = obj as Visual;
			Window window = obj as Window;

			if (visual != null)
			{
				CommandDispatcherChain.BuildChain (visual, ref that);
			}
			if (window != null)
			{
				CommandDispatcherChain.BuildChain (window, ref that);
			}
		}

		private static void BuildChain(Visual visual, ref CommandDispatcherChain that)
		{
			while (visual != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (visual);

				if (dispatcher != null)
				{
					if (that == null)
					{
						that = new CommandDispatcherChain ();
					}

					if (CommandDispatcherChain.Contains (that.chain, dispatcher) == false)
					{
						that.chain.Add (new Weak<CommandDispatcher> (dispatcher));
					}
				}

				AbstractMenu menu = visual as AbstractMenu;

				if (menu != null)
				{
					CommandDispatcherChain.BuildChain (menu.Host, ref that);
				}

				visual = visual.Parent;
			}
		}

		private static void BuildChain(Window window, ref CommandDispatcherChain that)
		{
			while (window != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (window);

				if (dispatcher != null)
				{
					if (that == null)
					{
						that = new CommandDispatcherChain ();
					}

					if (CommandDispatcherChain.Contains (that.chain, dispatcher) == false)
					{
						that.chain.Add (new Weak<CommandDispatcher> (dispatcher));
					}
				}

				window = window.Owner ?? window.Parent;
			}

			//	TODO: ajouter ici la notion d'application/module/document
		}

		private static bool Contains(IEnumerable<Weak<CommandDispatcher>> chain, CommandDispatcher dispatcher)
		{
			foreach (Weak<CommandDispatcher> item in chain)
			{
				if (item.Target == dispatcher)
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		readonly List<Weak<CommandDispatcher>>	chain;
	}
}

//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

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
			this.dispatchers = new List<Weak<CommandDispatcher>> ();
		}

		/// <summary>
		/// Gets a value indicating whether this chain is empty.
		/// </summary>
		/// <value><c>true</c> if this chain is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				for (int i = 0; i < this.dispatchers.Count; i++)
				{
					if (this.dispatchers[i].IsAlive)
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
				Weak<CommandDispatcher>[] chain = this.dispatchers.ToArray ();
				bool enableForwarding = true;

				for (int i = 0; i < chain.Length; i++)
				{
					CommandDispatcher dispatcher = chain[i].Target;

					if (dispatcher == null)
					{
						this.dispatchers.Remove (chain[i]);
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
			CommandDispatcherChain chain = new CommandDispatcherChain ();

			var window = visual.Window;

			if (window != null)
			{
				chain.BuildPartialChain (window.FocusedWidget);
				chain.BuildPartialChainBasedOnChildren (window.Root, c => c.ActiveWithoutFocus);
			}

			chain.BuildPartialChain (visual);
			chain.BuildPartialChain (window);
			
			return chain.RemoveDuplicates ();
		}

		/// <summary>
		/// Builds the dispatcher chain starting from the specified window.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>The dispatcher chain or <c>null</c>.</returns>
		public static CommandDispatcherChain BuildChain(Window window)
		{
			CommandDispatcherChain chain = new CommandDispatcherChain ();

			chain.BuildPartialChain (window);
			
			return chain.RemoveDuplicates ();
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
			
			CommandDispatcherChain chain = new CommandDispatcherChain ();

			if (obj != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (obj);

				if (dispatcher != null)
				{
					chain.dispatchers.Add (new Weak<CommandDispatcher> (dispatcher));
				}
			}

			return chain.RemoveDuplicates ();
		}

		#region Private Methods

		private void BuildPartialChain(DependencyObject obj)
		{
			Visual visual = obj as Visual;
			Window window = obj as Window;

			if (visual != null)
			{
				this.BuildPartialChain (visual);
			}
			if (window != null)
			{
				this.BuildPartialChain (window);
			}
		}

		private void BuildPartialChain(Visual visual)
		{
			while (visual != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (visual);

				if (dispatcher != null)
				{
					this.dispatchers.Add (new Weak<CommandDispatcher> (dispatcher));
				}

				AbstractMenu menu = visual as AbstractMenu;

				if (menu != null)
				{
					this.BuildPartialChain (menu.Host);
				}

				visual = visual.Parent;
			}
		}

		private void BuildPartialChainBasedOnChildren(Visual visual, System.Predicate<CommandDispatcher> predicate)
		{
			if (visual != null)
			{
				var visibleChildren   = visual.GetAllChildren (x => x.IsVisible);
				var activeDispatchers = visibleChildren.Select (x => CommandDispatcher.GetDispatcher (x)).Where (x => (x != null) && predicate (x));

				foreach (var dispatcher in activeDispatchers)
				{
					this.dispatchers.Add (new Weak<CommandDispatcher> (dispatcher));
				}
			}
		}

		private void BuildPartialChain(Window window)
		{
			while (window != null)
			{
				this.BuildPartialChainOfPrimaryDispatchers (window);
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (window);

				if (dispatcher != null)
				{
					this.dispatchers.Add (new Weak<CommandDispatcher> (dispatcher));
				}

				window = window.Owner ?? window.Parent;
			}

			//	TODO: ajouter ici la notion d'application/module/document
		}

		private void BuildPartialChainOfPrimaryDispatchers(Window window)
		{
			var childWidgets       = window.Root.FindAllChildren ();
			var childDispatchers   = childWidgets.Select (x => CommandDispatcher.GetDispatcher (x));
			var primaryDispatchers = childDispatchers.Where (x => (x != null) && (x.Level == CommandDispatcherLevel.Primary)).ToList ();

			if (primaryDispatchers.Count > 0)
			{
				this.dispatchers.AddRange (primaryDispatchers.Select (x => new Weak<CommandDispatcher> (x)));
			}
		}

		private CommandDispatcherChain RemoveDuplicates()
		{
			var items = this.dispatchers.Select (x => x.Target).Where (x => x != null).Distinct ().ToList ();

			if (items.Count == 0)
			{
				return null;
			}
			else
			{
				this.dispatchers.Clear ();
				this.dispatchers.AddRange (items.Select (x => new Weak<CommandDispatcher> (x)));
				return this;
			}
		}

		#endregion

		private readonly List<Weak<CommandDispatcher>>	dispatchers;
	}
}

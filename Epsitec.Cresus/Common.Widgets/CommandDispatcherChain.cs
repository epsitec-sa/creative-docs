//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public sealed class CommandDispatcherChain
	{
		public CommandDispatcherChain()
		{
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
				System.WeakReference[] chain = this.chain.ToArray ();
				bool enableForwarding = true;

				for (int i = 0; i < chain.Length; i++)
				{
					CommandDispatcher dispatcher = chain[i].Target as CommandDispatcher;

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
		/// <returns><c>true</c> if the command is known; <c>false</c> otherwise.</returns>
		public bool Knows(Command command, out int depth)
		{
			depth = 0;
			
			foreach (CommandDispatcher dispatcher in this.Dispatchers)
			{
				if (dispatcher.Knows (command))
				{
					return true;
				}
				
				depth++;
			}
			
			return false;
		}

		/// <summary>
		/// Selects the best command in an enumeration. In this case, best means
		/// the command which is topmost in the chain.
		/// </summary>
		/// <param name="commands">The commands.</param>
		/// <returns>The best command found or <c>null</c>.</returns>
		public Command SelectBestCommand(IEnumerable<Command> commands)
		{
			int     nearest  = int.MaxValue;
			Command selected = null;

			foreach (Command command in commands)
			{
				int depth;
				
				if ((this.Knows (command, out depth)) &&
					(depth < nearest))
				{
					nearest  = depth;
					selected = command;
				}
			}
			
			return selected;
		}
		

		public static IEnumerable<CommandDispatcher> EmptyDispatcherEnumeration
		{
			get
			{
				yield break;
			}
		}

		public static CommandDispatcherChain BuildChain(Visual visual)
		{
			CommandDispatcherChain that = null;

			CommandDispatcherChain.BuildChain (visual, ref that);
			CommandDispatcherChain.BuildChain (visual.Window, ref that);

			return that;
		}

		public static CommandDispatcherChain BuildChain(Window window)
		{
			CommandDispatcherChain that = null;

			CommandDispatcherChain.BuildChain (window, ref that);

			return that;
		}

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
					
					that.chain.Add (new System.WeakReference (dispatcher));
				}
			}
			
			return that;
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
						that.chain.Add (new System.WeakReference (dispatcher));
					}
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
						that.chain.Add (new System.WeakReference (dispatcher));
					}
				}

				window = window.Owner;
			}

			//	TODO: ajouter ici la notion d'application/module/document
		}

		private static bool Contains(IEnumerable<System.WeakReference> chain, CommandDispatcher dispatcher)
		{
			foreach (System.WeakReference item in chain)
			{
				if (item.Target == dispatcher)
				{
					return true;
				}
			}

			return false;
		}

		List<System.WeakReference> chain = new List<System.WeakReference> ();
	}
}

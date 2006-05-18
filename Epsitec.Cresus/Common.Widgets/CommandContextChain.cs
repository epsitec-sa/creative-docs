//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

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
		public CommandContextChain()
		{
		}

		/// <summary>
		/// Gets the command contexts.
		/// </summary>
		/// <value>The context enumeration.</value>
		public IEnumerable<CommandContext> Contexts
		{
			get
			{
				if ((this.chain != null) &&
					(this.chain.Count > 0))
				{
					System.WeakReference[] chain = this.chain.ToArray ();

					for (int i = 0; i < chain.Length; i++)
					{
						CommandContext context = chain[i].Target as CommandContext;

						if (context == null)
						{
							this.chain.Remove (chain[i]);
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
		/// Builds the command context chain based on a visual.
		/// </summary>
		/// <param name="visual">The visual from where to search for the command contexts.</param>
		/// <returns>The command context chain.</returns>
		public static CommandContextChain BuildChain(Visual visual)
		{
			CommandContextChain that = null;

			Visual item = visual;
			Window window = visual.Window;

			while (item != null)
			{
				CommandContext context = CommandContext.GetContext (item);

				if (context != null)
				{
					if (that == null)
					{
						that = new CommandContextChain ();
					}

					that.chain.Add (new System.WeakReference (context));
				}

				item = item.Parent;
			}

			while (window != null)
			{
				CommandContext context = CommandContext.GetContext (window);

				if (context != null)
				{
					if (that == null)
					{
						that = new CommandContextChain ();
					}

					that.chain.Add (new System.WeakReference (context));
				}

				window = window.Owner;
			}

			//	TODO: ajouter ici la notion d'application/module/document

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
		/// <param name="context">The context where the command state was found, or <c>null</c> is none was found.</param>
		/// <returns>The command state.</returns>
		public CommandState GetCommandState(Command command, out CommandContext context)
		{
			if ((this.chain != null) &&
				(this.chain.Count > 0))
			{
				CommandContext root = null;
				
				foreach (System.WeakReference item in this.chain)
				{
					context = item.Target as CommandContext;

					if (context != null)
					{
						CommandState state = context.FindCommandState (command);
						
						if (state != null)
						{
							return state;
						}

						root = context;
					}
				}

				if (root != null)
				{
					//	Create the command state in the root-level context.

					context = root;
					CommandState state = context.GetCommandState (command);
					
					return state;
				}
			}

			context = null;
			
			return null;
		}

		List<System.WeakReference> chain = new List<System.WeakReference> ();
	}
}

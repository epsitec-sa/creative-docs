//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public sealed class CommandContextChain
	{
		public CommandContextChain()
		{
		}

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

		public bool IsDisabled(Command command)
		{
			if ((this.chain != null) &&
				(this.chain.Count > 0))
			{
				foreach (System.WeakReference item in this.chain)
				{
					CommandContext context = item.Target as CommandContext;

					if (context != null)
					{
						if (context.GetLocalEnable (command) == false)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

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

		public CommandState GetCommandState(string commandName)
		{
			Command command = Command.Get (commandName);
			CommandContext context;
			
			return this.GetCommandState (command, out context);
		}
		
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

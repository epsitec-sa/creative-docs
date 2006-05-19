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

		public bool IsEmpty
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

		public IEnumerable<CommandDispatcher> Dispatchers
		{
			get
			{
				System.WeakReference[] chain = this.chain.ToArray ();

				for (int i = 0; i < chain.Length; i++)
				{
					CommandDispatcher dispatcher = chain[i].Target as CommandDispatcher;

					if (dispatcher == null)
					{
						this.chain.Remove (chain[i]);
					}
					else
					{
						yield return dispatcher;
					}
				}
			}
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

			Visual item = visual;
			Window window = visual.Window;

			while (item != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (item);

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

				item = item.Parent;
			}

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
			
			return that;
		}
		
		public static CommandDispatcherChain BuildChain(DependencyObject obj)
		{
			Visual visual = obj as Visual;

			if (visual != null)
			{
				return CommandDispatcherChain.BuildChain (visual);
			}
			
			CommandDispatcherChain that = null;

			if (obj != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (obj);

				if (dispatcher != null)
				{
					if (that != null)
					{
						that = new CommandDispatcherChain ();
					}
					
					that.chain.Add (new System.WeakReference (dispatcher));
				}
			}
			
			return that;
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

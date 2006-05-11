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

		public void BuildChain(Visual visual)
		{
			this.chain.Clear ();

			Visual item = visual;
			Window window = visual.Window;

			while (item != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (item);

				if (dispatcher != null)
				{
					this.chain.Add (new System.WeakReference (dispatcher));
				}

				item = item.Parent;
			}

			while (window != null)
			{
				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (window);
				
				if (dispatcher != null)
				{
					this.chain.Add (new System.WeakReference (dispatcher));
				}

				window = window.Owner;
			}
			
			//	TODO: ajouter ici la notion d'application/module/document
		}

		List<System.WeakReference> chain = new List<System.WeakReference> ();
	}
}

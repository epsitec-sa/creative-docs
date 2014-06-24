//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class AccountsMergeNodeGetter : INodeGetter<AccountsMergeNode>  // outputNodes
	{
		public void SetParams(Dictionary<DataObject, DataObject> todo)
		{
			this.nodes = new List<AccountsMergeNode> ();

			foreach (var x in todo)
			{
				var node = new AccountsMergeNode (x.Key, x.Value);
				this.nodes.Add (node);
			}
		}


		public int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public AccountsMergeNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.nodes.Count)
				{
					return this.nodes[index];
				}
				else
				{
					return AccountsMergeNode.Empty;
				}
			}
		}


		private List<AccountsMergeNode>			nodes;
	}
}

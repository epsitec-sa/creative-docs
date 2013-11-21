//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données quelconques, triées selon OrderValue.
	/// OrderNode -> OrderNode
	/// </summary>
	public class SortNodesGetter : AbstractNodesGetter<OrderNode>  // outputNodes
	{
		public SortNodesGetter(AbstractNodesGetter<OrderNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override OrderNode this[int index]
		{
			get
			{
				if (this.outputNodes != null && index >= 0 && index < this.outputNodes.Length)
				{
					return this.outputNodes[index];
				}
				else
				{
					return OrderNode.Empty;
				}
			}
		}

		public override void UpdateData()
		{
			var sorted = this.inputNodes.Nodes.OrderBy (x => x.OrderValue);
			this.outputNodes = sorted.ToArray ();
		}

		
		private readonly AbstractNodesGetter<OrderNode>	inputNodes;
		private OrderNode[]								outputNodes;
	}
}

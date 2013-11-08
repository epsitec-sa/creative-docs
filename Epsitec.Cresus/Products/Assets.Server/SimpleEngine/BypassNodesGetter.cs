//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Classe totalement inutile, juste pour l'exemple !
	/// Pourra servir de base pour implémenter un filtre, par exemple.
	/// </summary>
	public class BypassNodesGetter : INodesGetter
	{
		public BypassNodesGetter(INodesGetter inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		#region INodeGetter Members
		public IEnumerable<Node> Nodes
		{
			get
			{
				for (int i=0; i<this.NodesCount; i++)
				{
					yield return this.GetNode (i);
				}
			}
		}

		public int NodesCount
		{
			get
			{
				return this.inputNodes.NodesCount;
			}
		}

		public Node GetNode(int index)
		{
			return this.inputNodes.GetNode (index);
		}
		#endregion


		private readonly INodesGetter			inputNodes;
	}
}

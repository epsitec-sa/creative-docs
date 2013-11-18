//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public class NotFinalNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public NotFinalNodesGetter(AbstractNodesGetter<TreeNode> inputNodes)
		{
			this.inputNodes = inputNodes;
			this.outputNodes = new List<TreeNode> ();

		}


		public override int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public override TreeNode this[int index]
		{
			get
			{
				return this.outputNodes[index];
			}
		}

		public void UpdateData()
		{
			this.outputNodes.Clear ();
			this.outputNodes.AddRange (this.inputNodes.Nodes.Where (x => x.Type != NodeType.Final));
		}


		private readonly AbstractNodesGetter<TreeNode>	inputNodes;
		private readonly List<TreeNode>					outputNodes;
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class TreeToGuidNodesGetter : AbstractNodesGetter<GuidNode>
	{
		public TreeToGuidNodesGetter(AbstractNodesGetter<TreeNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public override int NodesCount
		{
			get
			{
				return this.inputNodes.NodesCount;
			}
		}

		public override GuidNode GetNode(int index)
		{
			var treeNode = this.inputNodes.GetNode (index);
			return new GuidNode (treeNode.Guid);
		}


		private readonly AbstractNodesGetter<TreeNode> inputNodes;
	}
}

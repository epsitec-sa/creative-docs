//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class LevelNodesGetter : INodesGetter
	{
		public LevelNodesGetter(INodesGetter inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;
		}


		public Timestamp? Timestamp;


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
			var node = this.inputNodes.GetNode (index);
			var level = this.GetLevel (node.Guid);

			return new Node (node.Guid, level);
		}
		#endregion


		private int GetLevel(Guid guid)
		{
			var obj = this.accessor.GetObject (this.baseType, guid);
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return 0;
		}


		private readonly INodesGetter			inputNodes;
		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
	}
}

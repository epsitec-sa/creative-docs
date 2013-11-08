//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class ObjectEventsNodesGetter : INodesGetter
	{
		public DataObject DataObject;


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
				if (this.DataObject == null)
				{
					return 0;
				}
				else
				{
					return this.DataObject.EventsCount;
				}
			}
		}

		public Node GetNode(int index)
		{
			var e = this.DataObject.GetEvent (index);

			if (e == null)
			{
				return Node.Empty;
			}
			else
			{
				return new Node (e.Guid);
			}
		}
		#endregion
	}
}

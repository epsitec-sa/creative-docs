//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public abstract class AbstractNodeFiller
	{
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

		public virtual int NodesCount
		{
			get
			{
				return 0;
			}
		}

		public virtual Node GetNode(int index)
		{
			return Node.Empty;
		}
	}
}

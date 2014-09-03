//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class ReportsNodeGetter : INodeGetter<GuidNode>  // outputNodes
	{
		public ReportsNodeGetter(IEnumerable<Guid> inputGuids)
		{
			this.inputGuids = inputGuids.ToArray ();
		}


		public int Count
		{
			get
			{
				return this.inputGuids.Length;
			}
		}

		public GuidNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.inputGuids.Length)
				{
					var guid = this.inputGuids[index];
					return new GuidNode (guid);
				}
				else
				{
					return GuidNode.Empty;
				}
			}
		}


		private readonly Guid[] inputGuids;
	}
}

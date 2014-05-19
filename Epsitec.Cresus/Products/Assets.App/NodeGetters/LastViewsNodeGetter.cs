//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class LastViewsNodeGetter : INodeGetter<LastViewNode>  // outputNodes
	{
		public void SetParams(List<LastViewNode> viewStates)
		{
			this.viewStates = viewStates;
		}


		public int Count
		{
			get
			{
				return this.viewStates.Count;
			}
		}

		public int SearchIndex(Guid navigationGuid)
		{
			for (int i=0; i<this.viewStates.Count; i++)
			{
				if (navigationGuid == this[i].NavigationGuid)
				{
					return i;
				}
			}

			return -1;
		}

		public LastViewNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.viewStates.Count)
				{
					return this.viewStates[index];
				}
				else
				{
					return LastViewNode.Empty;
				}
			}
		}


		private List<LastViewNode>			viewStates;
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodesGetter
{
	public class NavigationNodesGetter : AbstractNodesGetter<NavigationNode>  // outputNodes
	{
		public void SetParams(List<NavigationNode> viewStates)
		{
			this.viewStates = viewStates;
		}


		public override int Count
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

		public override NavigationNode this[int index]
		{
			get
			{
				int count = this.viewStates.Count;

				if (index >= 0 && index < count)
				{
					return this.viewStates[count-index-1];  // le dernier en premier
				}
				else
				{
					return NavigationNode.Empty;
				}
			}
		}


		private List<NavigationNode>			viewStates;
	}
}

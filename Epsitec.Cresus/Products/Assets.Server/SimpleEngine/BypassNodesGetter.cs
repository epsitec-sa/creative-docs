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
	public class BypassNodesGetter : AbstractNodesGetter<GuidNode>
	{
		public BypassNodesGetter(AbstractNodesGetter<GuidNode> inputNodes)
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
			return this.inputNodes.GetNode (index);
		}


		private readonly AbstractNodesGetter<GuidNode> inputNodes;
	}
}

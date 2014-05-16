//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Classe totalement inutile, juste pour l'exemple !
	/// Pourra servir de base pour implémenter un filtre, par exemple.
	/// GuidNode -> GuidNode
	/// </summary>
	public class BypassNodeGetter : AbstractNodeGetter<GuidNode>  // outputNodes
	{
		public BypassNodeGetter(AbstractNodeGetter<GuidNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override GuidNode this[int index]
		{
			get
			{
				return this.inputNodes[index];
			}
		}


		private readonly AbstractNodeGetter<GuidNode> inputNodes;
	}
}

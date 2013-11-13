﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Classe totalement inutile, juste pour l'exemple !
	/// Pourra servir de base pour implémenter un filtre, par exemple.
	/// GuidNode -> GuidNode
	/// </summary>
	public class BypassNodesGetter : AbstractNodesGetter<GuidNode>  // outputNodes
	{
		public BypassNodesGetter(AbstractNodesGetter<GuidNode> inputNodes)
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


		private readonly AbstractNodesGetter<GuidNode> inputNodes;
	}
}

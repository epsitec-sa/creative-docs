//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class ExpressionSimulationNodeGetter : INodeGetter<ExpressionSimulationNode>  // outputNodes
	{
		public ExpressionSimulationNodeGetter(List<ExpressionSimulationNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public ExpressionSimulationNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.inputNodes.Count)
				{
					return this.inputNodes[index];
				}
				else
				{
					return ExpressionSimulationNode.Empty;
				}
			}
		}


		private readonly List<ExpressionSimulationNode>		inputNodes;
	}
}

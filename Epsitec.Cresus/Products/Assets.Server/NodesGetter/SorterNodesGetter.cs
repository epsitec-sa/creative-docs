//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données quelconques, triées selon PrimaryOrderedValue
	/// et SecondaryOrderedValue (ComparableData).
	/// SortableNode -> SortableNode
	/// </summary>
	public class SorterNodesGetter : AbstractNodesGetter<SortableNode>  // outputNodes
	{
		public SorterNodesGetter(AbstractNodesGetter<SortableNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public SortingInstructions SortingInstructions;


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override SortableNode this[int index]
		{
			get
			{
				if (this.outputNodes != null && index >= 0 && index < this.outputNodes.Length)
				{
					return this.outputNodes[index];
				}
				else
				{
					return SortableNode.Empty;
				}
			}
		}

		public override void UpdateData()
		{
			this.outputNodes = SortingMachine<SortableNode>.Sorts
			(
				this.SortingInstructions,
				this.inputNodes.Nodes,
				x => x.PrimarySortValue,
				x => x.SecondarySortValue
			).ToArray ();
		}

		
		private readonly AbstractNodesGetter<SortableNode>	inputNodes;
		private SortableNode[]								outputNodes;
	}
}
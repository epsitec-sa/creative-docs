//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture à des données quelconques, triées selon PrimaryOrderedValue
	/// et SecondaryOrderedValue (ComparableData).
	/// SortableNode -> SortableNode
	/// </summary>
	public class SorterNodeGetter : INodeGetter<SortableNode>  // outputNodes
	{
		public SorterNodeGetter(INodeGetter<SortableNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public void SetParams(SortingInstructions instructions)
		{
			this.sortingInstructions = instructions;
			this.UpdateData ();
		}


		public int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public SortableNode this[int index]
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

		private void UpdateData()
		{
#if false
			this.outputNodes = SortingMachine<SortableNode>.Sorts
			(
				this.sortingInstructions,
				this.inputNodes.GetNodes (),
				x => x.PrimarySortValue,
				x => x.SecondarySortValue
			).ToArray ();
#else
			//	On ne trie plus !!!
			this.outputNodes = this.inputNodes.GetNodes ().ToArray ();
#endif
		}


		private readonly INodeGetter<SortableNode>	inputNodes;
		private SortableNode[]						outputNodes;
		private SortingInstructions					sortingInstructions;
	}
}
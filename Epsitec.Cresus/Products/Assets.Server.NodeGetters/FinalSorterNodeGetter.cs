//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class FinalSorterNodeGetter : INodeGetter<SortableCumulNode>  // outputNodes
	{
		public FinalSorterNodeGetter(INodeGetter<SortableCumulNode> inputNodes)
		{
			this.inputNodes = inputNodes;
		}


		public void SetParams(Timestamp? timestamp, SortingInstructions instructions)
		{
			this.timestamp = timestamp;
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

		public SortableCumulNode this[int index]
		{
			get
			{
				if (this.outputNodes != null && index >= 0 && index < this.outputNodes.Length)
				{
					return this.outputNodes[index];
				}
				else
				{
					return SortableCumulNode.Empty;
				}
			}
		}


		public decimal? GetValue(SortableCumulNode node, ObjectField field)
		{
			decimal value;
			if (node.Cumuls != null && node.Cumuls.TryGetValue (field, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		private void UpdateData()
		{
			this.outputNodes = SortingMachine<SortableCumulNode>.Sorts
			(
				this.sortingInstructions,
				this.inputNodes.GetNodes (),
				x => this.GetGroupSortValue (x),
				x => x.PrimarySortValue,
				x => x.SecondarySortValue
			).ToArray ();
		}

		private int GetGroupSortValue(SortableCumulNode node)
		{
			return node.GroupIndex.GetValueOrDefault ();
		}


		private readonly INodeGetter<SortableCumulNode> inputNodes;
		private SortableCumulNode[]					outputNodes;
		private Timestamp?							timestamp;
		private SortingInstructions					sortingInstructions;
	}
}
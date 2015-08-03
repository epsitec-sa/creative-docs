//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture à des données cumulées, triées selon le groupe, PrimaryOrderedValue
	/// et SecondaryOrderedValue (ComparableData).
	/// SortableCumulNode -> SortableCumulNode
	/// </summary>
	public class SorterCumulNodeGetter : INodeGetter<SortableCumulNode>  // outputNodes
	{
		public SorterCumulNodeGetter(INodeGetter<SortableCumulNode> inputNodes)
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


		public int GetIndex(Guid guid)
		{
			for (int i=0; i<this.outputNodes.Length; i++)
			{
				if (this.outputNodes[i].Guid == guid)
				{
					return i;
				}
			}

			return -1;
		}

		public decimal? GetValue(SortableCumulNode node, ObjectField field)
		{
			AbstractCumulValue value;
			if (node.Cumuls != null && node.Cumuls.TryGetValue (field, out value))
			{
				if (value is DecimalCumulValue)
				{
					return (decimal?) value.Value;
				}
				else
				{
					return null;
				}
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
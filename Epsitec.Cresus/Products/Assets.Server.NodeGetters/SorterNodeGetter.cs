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


		public void SetParams(SortingInstructions instructions, System.Func<Guid, bool> filter = null)
		{
			this.sortingInstructions = instructions;
			this.filter              = filter;

			this.UpdateData ();
		}

		public int SearchIndex(Guid value)
		{
			for (int i=0; i<this.outputNodes.Length; i++)
			{
				if (this.outputNodes[i].Guid == value)
				{
					return i;
				}
			}

			return -1;
		}


		public int Count
		{
			get
			{
				if (this.outputNodes == null)
				{
					return this.inputNodes.Count;
				}
				else
				{
					return this.outputNodes.Length;
				}
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
			this.outputNodes = SortingMachine<SortableNode>.Sorts
			(
				this.sortingInstructions,
				this.Nodes,
				null,
				x => x.PrimarySortValue,
				x => x.SecondarySortValue
			).ToArray ();
		}

		private IEnumerable<SortableNode> Nodes
		{
			get
			{
				if (this.filter == null)
				{
					return this.inputNodes.GetNodes ();
				}
				else
				{
					return this.inputNodes.GetNodes ().Where (x => this.filter (x.Guid));
				}
			}
		}


		private readonly INodeGetter<SortableNode>	inputNodes;
		private SortableNode[]						outputNodes;
		private SortingInstructions					sortingInstructions;
		private System.Func<Guid, bool>				filter;
	}
}
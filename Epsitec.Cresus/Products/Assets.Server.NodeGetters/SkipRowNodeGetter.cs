//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Permet de ne rendre accessible que certaines lignes. Les autres sont cachées.
	/// </summary>
	public class SkipRowNodeGetter : INodeGetter<SortableCumulNode>  // outputNodes
	{
		public SkipRowNodeGetter(DataAccessor accessor, INodeGetter<SortableCumulNode> objectNodes)
		{
			this.accessor    = accessor;
			this.objectNodes = objectNodes;

			this.visibleRows = new List<int> ();
		}


		public void SetParams(HashSet<int> visibleRows)
		{
			//	Donne les index des lignes visibles.
			if (visibleRows == null)
			{
				this.visibleRows.Clear ();

				this.filterEnable = false;
			}
			else
			{
				this.visibleRows.Clear ();
				this.visibleRows.AddRange (visibleRows);
				this.visibleRows.Sort ();

				this.filterEnable = true;
			}
		}


		public int Count
		{
			get
			{
				if (this.filterEnable == false)
				{
					return this.objectNodes.Count;
				}
				else
				{
					return this.visibleRows.Count;
				}
			}
		}

		public AbstractCumulValue GetValue(SortableCumulNode node, ObjectField field)
		{
			AbstractCumulValue value;

			if (node.Cumuls != null && node.Cumuls.TryGetValue (field, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		public SortableCumulNode this[int index]
		{
			get
			{
				if (this.filterEnable == false)
				{
					if (index >= 0 && index < this.objectNodes.Count)
					{
						return this.objectNodes[index];
					}
					else
					{
						return SortableCumulNode.Empty;
					}
				}
				else
				{
					if (index >= 0 && index < this.visibleRows.Count)
					{
						int i = this.visibleRows[index];
						return this.objectNodes[i];
					}
					else
					{
						return SortableCumulNode.Empty;
					}
				}
			}
		}


		public int GetBaseRow(int index)
		{
			if (this.filterEnable == false)
			{
				if (index >= 0 && index < this.objectNodes.Count)
				{
					return index;
				}
				else
				{
					return -1;
				}
			}
			else
			{
				if (index >= 0 && index < this.visibleRows.Count)
				{
					return this.visibleRows[index];
				}
				else
				{
					return -1;
				}
			}
		}


		private readonly DataAccessor					accessor;
		private readonly INodeGetter<SortableCumulNode>	objectNodes;
		private readonly List<int>						visibleRows;

		private bool									filterEnable;
	}
}

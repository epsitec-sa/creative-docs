//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des données quelconques en provenance de inputNodes.
	/// En fait, c'est la mise en série de 3 getters:
	/// 
	///     |
	///     o  GuidNode (BaseType.Groups)
	///     V
	/// ParentNodesGetter
	///     |
	///     o  ParentNode
	///     V
	/// LevelNodesGetter
	///     |
	///     o  LevelNode
	///     V
	/// TreeObjectsNodesGetter
	///     |
	///     o  TreeNode
	///     V
	/// 
	/// </summary>
	public class TreeNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public TreeNodesGetter(DataAccessor accessor, BaseType baseType, AbstractNodesGetter<GuidNode> inputNodes)
		{
			this.inputNodes        = inputNodes;
			this.parentNodesGetter = new ParentNodesGetter (inputNodes, accessor, baseType);
			this.levelNodesGetter  = new LevelNodesGetter (this.parentNodesGetter, accessor, baseType);
			this.treeObjectsGetter = new TreeObjectsNodesGetter (this.levelNodesGetter);
		}


		public SortingInstructions SortingInstructions;


		public Timestamp? Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				if (this.timestamp != value)
				{
					this.timestamp = value;
					this.UpdateData ();
				}
			}
		}

		public override int Count
		{
			get
			{
				return this.treeObjectsGetter.Count;
			}
		}

		public override TreeNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.treeObjectsGetter.Count)
				{
					return this.treeObjectsGetter[index];
				}
				else
				{
					return TreeNode.Empty;
				}
			}
		}

		public override void UpdateData()
		{
			this.parentNodesGetter.Timestamp = this.timestamp;
			this.parentNodesGetter.SortingInstructions = this.SortingInstructions;

			this.levelNodesGetter.SortingInstructions = this.SortingInstructions;

			this.levelNodesGetter.UpdateData ();
			this.treeObjectsGetter.UpdateData ();
		}


		#region TreeObjectsNodesGetter Facade
		public bool IsAllCompacted
		{
			get
			{
				return this.treeObjectsGetter.IsAllCompacted;
			}
		}

		public bool IsAllExpanded
		{
			get
			{
				return this.treeObjectsGetter.IsAllExpanded;
			}
		}

		public void CompactOrExpand(int index)
		{
			this.treeObjectsGetter.CompactOrExpand (index);
		}

		public void CompactAll()
		{
			this.treeObjectsGetter.CompactAll ();
		}

		public void ExpandAll()
		{
			this.treeObjectsGetter.ExpandAll ();
		}

		public int SearchBestIndex(Guid value)
		{
			return this.treeObjectsGetter.SearchBestIndex (value);
		}

		public int VisibleToAll(int index)
		{
			return this.treeObjectsGetter.VisibleToAll (index);
		}

		public int AllToVisible(int index)
		{
			return this.treeObjectsGetter.AllToVisible (index);
		}
		#endregion


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly ParentNodesGetter				parentNodesGetter;
		private readonly LevelNodesGetter				levelNodesGetter;
		private readonly TreeObjectsNodesGetter			treeObjectsGetter;

		private Timestamp?								timestamp;
	}
}

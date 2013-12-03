//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets en provenance de inputNodes.
	/// On part des groupes, qui sont ensuite fusionnés avec les objets.
	/// En fait, c'est la mise en série de plusieurs getters:
	/// 
	///     |
	///     o  GuidNode (BaseType.Groups)
	///     V
	/// GroupParentNodesGetter
	///     |
	///     o  ParentNode
	///     V
	/// GroupLevelNodesGetter
	///     |
	///     o  LevelNode                     SortableNode              GuidNode (BaseType.Objects)
	///     V
	/// MergeNodesGetter <-o- SorterNodesGetter <-o- SortableNodesGetter <-o-
	///     |
	///     o  LevelNode
	///     V
	/// TreeObjectsNodesGetter
	///     |
	///     o  TreeNode
	///     V
	/// 
	/// </summary>
	public class ObjectsNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public ObjectsNodesGetter(DataAccessor accessor, AbstractNodesGetter<GuidNode> groupNodes, AbstractNodesGetter<GuidNode> objectNodes)
		{
			this.objectNodesGetter1 = new SortableNodesGetter (objectNodes, accessor, BaseType.Objects);
			this.objectNodesGetter2 = new SorterNodesGetter (this.objectNodesGetter1);

			this.groupNodesGetter1 = new GroupParentNodesGetter (groupNodes, accessor);
			this.groupNodesGetter2 = new GroupLevelNodesGetter (this.groupNodesGetter1, accessor);

			this.mergeNodesGetter  = new MergeNodesGetter (accessor, this.groupNodesGetter2, this.objectNodesGetter2);
			this.treeObjectsGetter = new TreeObjectsNodesGetter (this.mergeNodesGetter);

			this.sortingInstructions = SortingInstructions.Empty;
		}


		public void SetParams(Timestamp? timestamp, Guid rootGuid, SortingInstructions instructions)
		{
			this.timestamp           = timestamp;
			this.rootGuid            = rootGuid;
			this.sortingInstructions = instructions;

			this.UpdateData ();
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

		private void UpdateData()
		{
			this.objectNodesGetter1.SetParams (this.timestamp, this.sortingInstructions);
			this.objectNodesGetter2.SetParams (this.sortingInstructions);

			this.groupNodesGetter1.SetParams (this.timestamp, this.sortingInstructions);
			this.groupNodesGetter2.SetParams (this.rootGuid, this.sortingInstructions, this.rootGuid.IsEmpty);

			this.mergeNodesGetter.SetParams (this.timestamp);
			this.treeObjectsGetter.SetParams (inputIsMerge: true);
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


		private readonly SortableNodesGetter		objectNodesGetter1;
		private readonly SorterNodesGetter			objectNodesGetter2;
		private readonly GroupParentNodesGetter		groupNodesGetter1;
		private readonly GroupLevelNodesGetter		groupNodesGetter2;
		private readonly MergeNodesGetter			mergeNodesGetter;
		private readonly TreeObjectsNodesGetter		treeObjectsGetter;

		private Timestamp?							timestamp;
		private Guid								rootGuid;
		private SortingInstructions					sortingInstructions;
	}
}

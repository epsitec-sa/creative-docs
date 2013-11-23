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
	/// ParentNodesGetter
	///     |
	///     o  ParentNode
	///     V
	/// LevelNodesGetter
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

			this.groupNodesGetter1 = new ParentNodesGetter (groupNodes, accessor, BaseType.Groups);
			this.groupNodesGetter2 = new LevelNodesGetter (this.groupNodesGetter1, accessor, BaseType.Groups);
			this.mergeNodesGetter  = new MergeNodesGetter (accessor, this.groupNodesGetter2, this.objectNodesGetter2);
			this.treeObjectsGetter = new TreeObjectsNodesGetter (this.mergeNodesGetter);

			this.SortingInstructions = SortingInstructions.Empty;
		}


		public Guid								RootGuid;
		public SortingInstructions				SortingInstructions;


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
			this.objectNodesGetter1.Timestamp = this.timestamp;
			this.objectNodesGetter1.SortingInstructions = this.SortingInstructions;

			this.objectNodesGetter2.SortingInstructions = this.SortingInstructions;

			this.groupNodesGetter1.Timestamp = this.timestamp;

			this.groupNodesGetter2.ForceEmpty = this.RootGuid.IsEmpty;
			this.groupNodesGetter2.RootGuid = this.RootGuid;

			this.mergeNodesGetter.Timestamp = this.timestamp;

			this.objectNodesGetter2.UpdateData ();
			this.groupNodesGetter2.UpdateData ();
			this.mergeNodesGetter.UpdateData ();
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


		private readonly SortableNodesGetter			objectNodesGetter1;
		private readonly SorterNodesGetter			objectNodesGetter2;
		private readonly ParentNodesGetter			groupNodesGetter1;
		private readonly LevelNodesGetter			groupNodesGetter2;
		private readonly MergeNodesGetter			mergeNodesGetter;
		private readonly TreeObjectsNodesGetter		treeObjectsGetter;

		private Timestamp?							timestamp;
	}
}

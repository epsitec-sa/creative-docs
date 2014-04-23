//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des groupes en provenance de inputNodes.
	/// En fait, c'est la mise en série de 3 getters:
	/// 
	///     |
	///     o  GuidNode (BaseType.Groups)
	///     V
	/// GroupParentNodeGetter
	///     |
	///     o  ParentNode
	///     V
	/// GroupLevelNodeGetter
	///     |
	///     o  LevelNode
	///     V
	/// TreeObjectsNodeGetter
	///     |
	///     o  TreeNode
	///     V
	/// 
	/// </summary>
	public class GroupTreeNodeGetter : AbstractNodeGetter<TreeNode>, ITreeFunctions  // outputNodes
	{
		public GroupTreeNodeGetter(DataAccessor accessor, BaseType baseType, AbstractNodeGetter<GuidNode> inputNodes)
		{
			this.inputNodes        = inputNodes;
			this.parentNodeGetter = new GroupParentNodeGetter (inputNodes, accessor, baseType);
			this.levelNodeGetter  = new GroupLevelNodeGetter (this.parentNodeGetter, accessor, baseType);
			this.treeObjectsGetter = new TreeObjectsNodeGetter (this.levelNodeGetter);
		}


		public void SetParams(Timestamp? timestamp, SortingInstructions instructions)
		{
			this.timestamp           = timestamp;
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
			this.parentNodeGetter.SetParams (this.timestamp, this.sortingInstructions);
			this.levelNodeGetter.SetParams(Guid.Empty, this.sortingInstructions, false);
			this.treeObjectsGetter.SetParams (inputIsMerge: false);
		}


		#region ITreeFonctions
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

		public void SetLevelAll(int level)
		{
			this.treeObjectsGetter.SetLevelAll (level);
		}

		public void CompactAll()
		{
			this.treeObjectsGetter.CompactAll ();
		}

		public void CompactOne()
		{
			this.treeObjectsGetter.CompactOne ();
		}

		public void ExpandOne()
		{
			this.treeObjectsGetter.ExpandOne ();
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


		private readonly AbstractNodeGetter<GuidNode>	inputNodes;
		private readonly GroupParentNodeGetter			parentNodeGetter;
		private readonly GroupLevelNodeGetter			levelNodeGetter;
		private readonly TreeObjectsNodeGetter			treeObjectsGetter;

		private Timestamp?								timestamp;
		private SortingInstructions						sortingInstructions;
	}
}

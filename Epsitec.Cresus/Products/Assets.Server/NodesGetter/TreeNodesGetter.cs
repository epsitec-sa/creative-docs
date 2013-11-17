//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des données quelconques en provenance
	/// de inputNodes. En fait, c'est la mise en série de 3 getters.
	/// GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
	/// </summary>
	public class TreeNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public TreeNodesGetter(DataAccessor accessor, BaseType baseType, AbstractNodesGetter<GuidNode> inputNodes)
		{
			this.inputNodes        = inputNodes;
			this.ppNodesGetter     = new ParentPositionNodesGetter (inputNodes, accessor, baseType);
			this.levelNodesGetter  = new LevelNodesGetter (this.ppNodesGetter, accessor, baseType);
			this.treeObjectsGetter = new TreeObjectsNodesGetter (this.levelNodesGetter);
		}


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

		public void UpdateData(TreeNodeOutputMode mode = TreeNodeOutputMode.All)
		{
			this.ppNodesGetter.Timestamp = this.timestamp;
			this.levelNodesGetter.UpdateData (mode);
			this.treeObjectsGetter.UpdateData ();
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
				return this.treeObjectsGetter[index];
			}
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

		public void CompactFinals()
		{
			this.treeObjectsGetter.CompactFinals ();
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
		private readonly ParentPositionNodesGetter		ppNodesGetter;
		private readonly LevelNodesGetter				levelNodesGetter;
		private readonly TreeObjectsNodesGetter			treeObjectsGetter;

		private Timestamp?								timestamp;
	}
}

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
	/// En fait, c'est la mise en série de 4 getters:
	/// 
	///                       rootGuid         GuidNode (BaseType.Objects)
	///                           v                v
	///        ParentPosition   Level            Merge            TreeObjects
	///   -o-> NodesGetter -o-> NodesGetter -o-> NodesGetter -o-> NodesGetter -o->
	///    |                |                |                |                |
	/// GuidNode   ParentPositionNode    LevelNode        LevelNode         TreeNode
	/// (BaseType.Groups)
	/// 
	/// </summary>
	public class ObjectsNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public ObjectsNodesGetter(DataAccessor accessor, AbstractNodesGetter<GuidNode> groupNodes, AbstractNodesGetter<GuidNode> objectNodes)
		{
			this.ppNodesGetter     = new ParentPositionNodesGetter (groupNodes, accessor, BaseType.Groups);
			this.levelNodesGetter  = new LevelNodesGetter (this.ppNodesGetter, accessor, BaseType.Groups);
			this.mergeNodesGetter  = new MergeNodesGetter (accessor, this.levelNodesGetter, objectNodes);
			this.treeObjectsGetter = new TreeObjectsNodesGetter (this.mergeNodesGetter);
		}


		public Guid RootGuid;


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
			this.UpdateData (this.RootGuid);
		}

		public void UpdateData(Guid rootGuid)
		{
			this.ppNodesGetter.Timestamp = this.timestamp;
			this.levelNodesGetter.UpdateData (rootGuid);
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


		private readonly ParentPositionNodesGetter		ppNodesGetter;
		private readonly LevelNodesGetter				levelNodesGetter;
		private readonly MergeNodesGetter				mergeNodesGetter;
		private readonly TreeObjectsNodesGetter			treeObjectsGetter;

		private Timestamp?								timestamp;
	}
}

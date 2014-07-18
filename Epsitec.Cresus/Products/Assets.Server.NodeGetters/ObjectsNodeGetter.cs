//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets en provenance de inputNodes.
	/// On part des groupes, qui sont ensuite fusionnés avec les objets.
	/// En fait, c'est la mise en série de plusieurs getters:
	/// 
	/// (BaseType.Groups)        (BaseType.Assets)
	///     |                         |
	///     o  GuidNode               o  GuidNode
	///     V                         |
	/// GroupParentNodeGetter         |
	///     |                         |
	///     o  ParentNode             |
	///     V                         |
	/// GroupLevelNodeGetter          |
	///     |                         |
	///     o  LevelNode              |
	///     V                         |
	/// MergeNodeGetter <-------------|
	///     |
	///     o  LevelNode
	///     V
	/// TreeObjectsNodeGetter
	///     |
	///     o  TreeNode
	///     V
	/// CumulNodeGetter
	///     |
	///     o  CumulNode
	///     V
	/// SortableCumulNodeGetter
	///     |
	///     o  SortableCumulNode
	///     V
	/// SorterCumulNodeGetter
	///     |
	///     o  SortableCumulNode
	///     V
	/// 
	/// </summary>
	public class ObjectsNodeGetter : INodeGetter<SortableCumulNode>, ITreeFunctions, IObjectsNodeGetter  // outputNodes
	{
		public ObjectsNodeGetter(DataAccessor accessor, INodeGetter<GuidNode> groupNodes, INodeGetter<GuidNode> objectNodes)
		{
			this.groupNodeGetter1 = new GroupParentNodeGetter (groupNodes, accessor, BaseType.Groups);
			this.groupNodeGetter2 = new GroupLevelNodeGetter (this.groupNodeGetter1, accessor, BaseType.Groups);

			this.mergeNodeGetter   = new MergeNodeGetter (accessor, this.groupNodeGetter2, objectNodes);
			this.treeObjectsGetter = new TreeObjectsNodeGetter (this.mergeNodeGetter);
			this.cumulNodeGetter   = new CumulNodeGetter (accessor, this.treeObjectsGetter);

			this.sortableNodeGetter = new SortableCumulNodeGetter (this.cumulNodeGetter, accessor, BaseType.Assets);
			this.sorterNodeGetter   = new SorterCumulNodeGetter (this.sortableNodeGetter);

			this.sortingInstructions = SortingInstructions.Empty;
		}


		public void SetParams(Timestamp? timestamp, Guid rootGuid, SortingInstructions instructions, List<ExtractionInstructions> extractionInstructions = null)
		{
			//	La liste des instructions d'extraction est utile pour la production de rapports.
			this.timestamp              = timestamp;
			this.rootGuid               = rootGuid;
			this.sortingInstructions    = instructions;
			this.extractionInstructions = extractionInstructions;

			this.UpdateData ();
		}


		public int Count
		{
			get
			{
				return this.sorterNodeGetter.Count;
			}
		}

		public SortableCumulNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.sorterNodeGetter.Count)
				{
					return this.sorterNodeGetter[index];
				}
				else
				{
					return SortableCumulNode.Empty;
				}
			}
		}


		public decimal? GetValue(DataObject obj, SortableCumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			return this.sorterNodeGetter.GetValue (node, field);
		}


		private void UpdateData()
		{
			this.groupNodeGetter1.SetParams (this.timestamp, this.sortingInstructions);
			this.groupNodeGetter2.SetParams (this.rootGuid, this.sortingInstructions, this.rootGuid.IsEmpty);

			this.mergeNodeGetter.SetParams (this.timestamp);
			this.treeObjectsGetter.SetParams (inputIsMerge: true);
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);

			this.sortableNodeGetter.SetParams (this.timestamp, this.sortingInstructions);
			this.sorterNodeGetter.SetParams (this.timestamp, this.sortingInstructions);
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
			this.UpdateData ();
		}

		public void CompactAll()
		{
			this.treeObjectsGetter.CompactAll ();
			this.UpdateData ();
		}

		public void CompactOne()
		{
			this.treeObjectsGetter.CompactOne ();
			this.UpdateData ();
		}

		public void ExpandOne()
		{
			this.treeObjectsGetter.ExpandOne ();
			this.UpdateData ();
		}

		public void ExpandAll()
		{
			this.treeObjectsGetter.ExpandAll ();
			this.UpdateData ();
		}

		public void SetLevel(int level)
		{
			this.treeObjectsGetter.SetLevel (level);
			this.UpdateData ();
		}

		public int GetLevel()
		{
			return this.treeObjectsGetter.GetLevel ();
		}

		public int SearchBestIndex(Guid value)
		{
			int index = this.treeObjectsGetter.SearchBestIndex (value);

			if (index != -1)
			{
				var guid = this.treeObjectsGetter[index].Guid;
				index = this.sorterNodeGetter.GetIndex (guid);
			}

			return index;
		}

		public int VisibleToAll(int index)
		{
			return index;
		}

		public int AllToVisible(int index)
		{
			return index;
		}
		#endregion


		private readonly GroupParentNodeGetter		groupNodeGetter1;
		private readonly GroupLevelNodeGetter		groupNodeGetter2;
		private readonly MergeNodeGetter			mergeNodeGetter;
		private readonly TreeObjectsNodeGetter		treeObjectsGetter;
		private readonly CumulNodeGetter			cumulNodeGetter;
		private readonly SortableCumulNodeGetter	sortableNodeGetter;
		private readonly SorterCumulNodeGetter		sorterNodeGetter;

		private Timestamp?							timestamp;
		private Guid								rootGuid;
		private SortingInstructions					sortingInstructions;
		private List<ExtractionInstructions>		extractionInstructions;
	}
}

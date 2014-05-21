﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	///     |
	///     o  GuidNode (BaseType.Groups)
	///     V
	/// GroupParentNodeGetter
	///     |
	///     o  ParentNode
	///     V
	/// GroupLevelNodeGetter
	///     |
	///     o  LevelNode                     SortableNode              GuidNode (BaseType.Objects)
	///     V
	/// MergeNodeGetter <-o- SorterNodeGetter <-o- SortableNodeGetter <-o-
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
	/// 
	/// </summary>
	public class ObjectsNodeGetter : INodeGetter<CumulNode>, ITreeFunctions, IObjectsNodeGetter  // outputNodes
	{
		public ObjectsNodeGetter(DataAccessor accessor, INodeGetter<GuidNode> groupNodes, INodeGetter<GuidNode> objectNodes)
		{
			this.objectNodeGetter1 = new SortableNodeGetter (objectNodes, accessor, BaseType.Assets);
			this.objectNodeGetter2 = new SorterNodeGetter (this.objectNodeGetter1);

			this.groupNodeGetter1 = new GroupParentNodeGetter (groupNodes, accessor, BaseType.Groups);
			this.groupNodeGetter2 = new GroupLevelNodeGetter (this.groupNodeGetter1, accessor, BaseType.Groups);

			this.mergeNodeGetter   = new MergeNodeGetter (accessor, this.groupNodeGetter2, this.objectNodeGetter2);
			this.treeObjectsGetter = new TreeObjectsNodeGetter (this.mergeNodeGetter);
			this.cumulNodeGetter   = new CumulNodeGetter (accessor, this.treeObjectsGetter);

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
				return this.cumulNodeGetter.Count;
			}
		}

		public CumulNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.cumulNodeGetter.Count)
				{
					return this.cumulNodeGetter[index];
				}
				else
				{
					return CumulNode.Empty;
				}
			}
		}


		public decimal? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			return this.cumulNodeGetter.GetValue (obj, node, field);
		}


		private void UpdateData()
		{
			this.objectNodeGetter1.SetParams (this.timestamp, this.sortingInstructions);
			this.objectNodeGetter2.SetParams (this.sortingInstructions);

			this.groupNodeGetter1.SetParams (this.timestamp, this.sortingInstructions);
			this.groupNodeGetter2.SetParams (this.rootGuid, this.sortingInstructions, this.rootGuid.IsEmpty);

			this.mergeNodeGetter.SetParams (this.timestamp);
			this.treeObjectsGetter.SetParams (inputIsMerge: true);
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
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
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public void CompactAll()
		{
			this.treeObjectsGetter.CompactAll ();
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public void CompactOne()
		{
			this.treeObjectsGetter.CompactOne ();
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public void ExpandOne()
		{
			this.treeObjectsGetter.ExpandOne ();
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public void ExpandAll()
		{
			this.treeObjectsGetter.ExpandAll ();
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public void SetLevel(int level)
		{
			this.treeObjectsGetter.SetLevel (level);
			this.cumulNodeGetter.SetParams (this.timestamp, this.extractionInstructions);
		}

		public int GetLevel()
		{
			return this.treeObjectsGetter.GetLevel ();
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


		private readonly SortableNodeGetter		objectNodeGetter1;
		private readonly SorterNodeGetter		objectNodeGetter2;
		private readonly GroupParentNodeGetter	groupNodeGetter1;
		private readonly GroupLevelNodeGetter	groupNodeGetter2;
		private readonly MergeNodeGetter		mergeNodeGetter;
		private readonly TreeObjectsNodeGetter	treeObjectsGetter;
		private readonly CumulNodeGetter		cumulNodeGetter;

		private Timestamp?						timestamp;
		private Guid							rootGuid;
		private SortingInstructions				sortingInstructions;
		private List<ExtractionInstructions>	extractionInstructions;
	}
}
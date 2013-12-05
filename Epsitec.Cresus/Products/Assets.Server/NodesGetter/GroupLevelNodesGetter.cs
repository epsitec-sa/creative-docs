﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des groupes. En entrée, on reçoit des données parent/orderValue
	/// désordonnées qui servent à reconstruire un arbre. En sortie, on fourni des données
	/// ordonnées avec une indication du level.
	/// ParentNode -> LevelNode
	/// </summary>
	public class GroupLevelNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public GroupLevelNodesGetter(AbstractNodesGetter<ParentNode> inputNodes, DataAccessor accessor)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;

			this.levelNodes = new List<LevelNode> ();
		}


		public void SetParams(Guid rootGuid, SortingInstructions instructions, bool forceEmpty)
		{
			this.rootGuid            = rootGuid;
			this.sortingInstructions = instructions;
			this.forceEmpty          = forceEmpty;

			this.UpdateData ();
		}


		public override int Count
		{
			get
			{
				return this.levelNodes.Count;
			}
		}

		public override LevelNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.levelNodes.Count)
				{
					return this.levelNodes[index];
				}
				else
				{
					return LevelNode.Empty;
				}
			}
		}

		private void UpdateData()
		{
			this.levelNodes.Clear ();

			if (this.forceEmpty)
			{
				return;
			}

			//	Crée un véritable arbre de tous les noeuds.
			ParentNode root;

			if (this.rootGuid.IsEmpty)
			{
				root = this.inputNodes.Nodes.Where (x => x.Parent.IsEmpty).FirstOrDefault ();
			}
			else
			{
				root = new ParentNode (this.rootGuid, Guid.Empty, ComparableData.Empty, ComparableData.Empty);
			}

			if (root.IsEmpty)
			{
				return;
			}

			var tree = new TreeNode (null, root);
			this.Insert (tree);

			//	Parcourt tout l'arbre pour obtenir une liste "à plat" des noeuds.
			var list = new List<TreeNode> ();
			list.Add (tree);
			tree.GetNodes (list);

			//	Construit la liste finale consultable en sortie.
			foreach (var treeNode in list)
			{
				int level = GroupLevelNodesGetter.GetLevel (treeNode);
				var n = new LevelNode (treeNode.Node.Guid, BaseType.Groups, level, null);
				this.levelNodes.Add (n);
			}
		}

		private void Insert(TreeNode tree)
		{
			//	Insertion récursive des noeuds dans l'arbre.
			var childrens = this.Sort (this.inputNodes.Nodes.Where (x => x.Parent == tree.Node.Guid));

			foreach (var children in childrens)
			{
				var n = new TreeNode (tree, children);
				tree.Childrens.Add (n);

				this.Insert (n);
			}
		}

		private IEnumerable<ParentNode> Sort(IEnumerable<ParentNode> nodes)
		{
			return SortingMachine<ParentNode>.Sorts
			(
				this.sortingInstructions,
				nodes,
				x => x.PrimaryOrderedValue,
				x => x.SecondaryOrderedValue
			);
		}

		private static int GetLevel(TreeNode treeNode)
		{
			//	Retourne le niveau d'une feuille dans l'arbre.
			int level = 0;

			while (treeNode.Parent != null)
			{
				treeNode = treeNode.Parent;
				level++;
			}

			return level;
		}


		private class TreeNode
		{
			public TreeNode(TreeNode parent, ParentNode node)
			{
				this.parent = parent;
				this.node   = node;

				this.childrens = new List<TreeNode> ();
			}

			public TreeNode Parent
			{
				get
				{
					return this.parent;
				}
			}

			public ParentNode Node
			{
				get
				{
					return this.node;
				}
			}

			public List<TreeNode> Childrens
			{
				get
				{
					return this.childrens;
				}
			}

			public void GetNodes(List<TreeNode> list)
			{
				//	Rempli la liste des noeuds récursivement.
				foreach (var children in this.childrens)
				{
					list.Add (children);
					children.GetNodes (list);
				}
			}

			private readonly TreeNode				parent;
			private readonly ParentNode				node;
			private readonly List<TreeNode>			childrens;
		}


		private readonly AbstractNodesGetter<ParentNode> inputNodes;
		private readonly DataAccessor					accessor;
		private readonly List<LevelNode>				levelNodes;

		private Guid									rootGuid;
		private SortingInstructions						sortingInstructions;
		private bool									forceEmpty;
	}
}

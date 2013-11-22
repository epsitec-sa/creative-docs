//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données. En entrée, on reçoit des données parent/orderValue
	/// désordonnées qui servent à reconstruire un arbre. En sortie, on fourni des données
	/// ordonnées avec une indication du level.
	/// ParentNode -> LevelNode
	/// </summary>
	public class LevelNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public LevelNodesGetter(AbstractNodesGetter<ParentNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.levelNodes = new List<LevelNode> ();
		}


		public bool								ForceEmpty;
		public Guid								RootGuid;


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

		public override void UpdateData()
		{
			this.UpdateData (this.ForceEmpty, this.RootGuid);
		}

		private void UpdateData(bool forceEmpty, Guid rootGuid)
		{
			this.levelNodes.Clear ();

			if (forceEmpty)
			{
				return;
			}

			//	Crée un véritable arbre de tous les noeuds.
			ParentNode root;

			if (rootGuid.IsEmpty)
			{
				root = this.inputNodes.Nodes.Where (x => x.Parent.IsEmpty).FirstOrDefault ();
			}
			else
			{
				root = new ParentNode (rootGuid, Guid.Empty, ComparableData.Empty, ComparableData.Empty);
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
				int level = LevelNodesGetter.GetLevel (treeNode);
				var n = new LevelNode (treeNode.Node.Guid, this.baseType, level);
				this.levelNodes.Add (n);
			}
		}

		private void Insert(TreeNode tree)
		{
			//	Insertion récursive des noeuds dans l'arbre.
			var childrens = this.inputNodes.Nodes.Where (x => x.Parent == tree.Node.Guid).OrderBy (x => x.PrimaryOrderedValue);

			foreach (var children in childrens)
			{
				var n = new TreeNode (tree, children);
				tree.Childrens.Add (n);

				this.Insert (n);
			}
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
			private readonly ParentNode		node;
			private readonly List<TreeNode>			childrens;
		}


		private readonly AbstractNodesGetter<ParentNode> inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly List<LevelNode>				levelNodes;
	}
}

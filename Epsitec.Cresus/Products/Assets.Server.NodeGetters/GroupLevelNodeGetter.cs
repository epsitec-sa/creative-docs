//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture à des groupes. En entrée, on reçoit des données parent/orderValue
	/// désordonnées qui servent à reconstruire un arbre. En sortie, on fourni des données
	/// ordonnées avec une indication du level.
	/// ParentNode -> LevelNode
	/// </summary>
	public class GroupLevelNodeGetter : INodeGetter<LevelNode>  // outputNodes
	{
		public GroupLevelNodeGetter(INodeGetter<ParentNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.levelNodes = new List<LevelNode> ();
			this.parentChildrens = new Dictionary<Guid, List<ParentNode>> ();
			this.showedNodes = new HashSet<Guid> ();
		}


		public void SetParams(Guid rootGuid, SortingInstructions instructions, bool forceEmpty, System.Func<Guid, bool> filter = null)
		{
			this.rootGuid            = rootGuid;
			this.sortingInstructions = instructions;
			this.forceEmpty          = forceEmpty;
			this.filter              = filter;

			this.UpdateData ();
		}


		public int Count
		{
			get
			{
				return this.levelNodes.Count;
			}
		}

		public LevelNode this[int index]
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
				root = this.inputNodes.GetNodes ().Where (x => x.Parent.IsEmpty).FirstOrDefault ();
			}
			else
			{
				root = new ParentNode (this.rootGuid, Guid.Empty, ComparableData.Empty, ComparableData.Empty);
			}

			if (root.IsEmpty)
			{
				return;
			}

			this.UpdateParentChildrens ();

			var tree = new InternalTreeNode (this, null, root);
			this.Insert (tree);

			//	Parcourt tout l'arbre pour obtenir une liste "à plat" des noeuds.
			var list = new List<InternalTreeNode> ();
			list.Add (tree);
			tree.PutNodes (list);

			//	Construit la liste finale consultable en sortie.
			foreach (var treeNode in list)
			{
				int level = GroupLevelNodeGetter.GetLevel (treeNode);
				var n = new LevelNode (treeNode.Node.Guid, this.baseType, level, null, null);
				this.levelNodes.Add (n);
			}
		}

		private void UpdateParentChildrens()
		{
			//	Met à jour le dictionnaire this.parentChildrens.
			//	key   -> Guid du parent
			//	value -> liste des ParentNode des enfants
			this.parentChildrens.Clear ();
			this.showedNodes.Clear ();

			foreach (var node in this.inputNodes.GetNodes ())
			{
				if (!node.Parent.IsEmpty)
				{
					List<ParentNode> childrens;
					if (!this.parentChildrens.TryGetValue (node.Parent, out childrens))
					{
						//	Nouveau parent inconnu pour lequel on crée une liste
						//	d'enfants vide.
						childrens = new List<ParentNode> ();
						this.parentChildrens[node.Parent] = childrens;
					}

					childrens.Add (node);  // ajoute l'enfant au parent

					if (this.filter != null && this.filter (node.Guid))
					{
						//	S'il s'agit d'une feuille visible, on l'ajoute dans showedNodes.
						this.showedNodes.Add (node.Guid);
					}
				}
			}
		}

		private void Insert(InternalTreeNode tree)
		{
			//	Insertion récursive des noeuds dans l'arbre.
			List<ParentNode> childrens;
			if (this.parentChildrens.TryGetValue (tree.Node.Guid, out childrens))
			{
				var sortedChildrend = this.Sort (childrens);

				foreach (var children in sortedChildrend)
				{
					var n = new InternalTreeNode (this, tree, children);
					tree.Childrens.Add (n);

					this.Insert (n);
				}
			}
		}

		private IEnumerable<ParentNode> Sort(IEnumerable<ParentNode> nodes)
		{
			return SortingMachine<ParentNode>.Sorts
			(
				this.sortingInstructions,
				nodes,
				null,
				x => x.PrimaryOrderedValue,
				x => x.SecondaryOrderedValue
			);
		}

		private static int GetLevel(InternalTreeNode treeNode)
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


		private class InternalTreeNode
		{
			public InternalTreeNode(GroupLevelNodeGetter nodeGetter, InternalTreeNode parent, ParentNode node)
			{
				this.nodeGetter = nodeGetter;
				this.parent     = parent;
				this.node       = node;

				this.childrens = new List<InternalTreeNode> ();
			}

			public InternalTreeNode				Parent
			{
				get
				{
					return this.parent;
				}
			}

			public ParentNode					Node
			{
				get
				{
					return this.node;
				}
			}

			public List<InternalTreeNode>		Childrens
			{
				get
				{
					return this.childrens;
				}
			}

			public void PutNodes(List<InternalTreeNode> list)
			{
				//	Rempli la liste des noeuds récursivement.
				if (this.IsVisible)
				{
					foreach (var children in this.childrens)
					{
						if (children.IsVisible)
						{
							list.Add (children);
						}

						children.PutNodes (list);
					}
				}
			}

			private bool						IsVisible
			{
				//	Indique si une ligne est visible. Elle l'est seulement si l'un de
				//	ses descendant (fils, petit-fils, etc.) est visible.
				get
				{
					if (this.nodeGetter.filter == null)  // pas de filtre ?
					{
						return true;
					}

					if (this.childrens.Count == 0)  // feuille de l'arbre ?
					{
						return this.nodeGetter.showedNodes.Contains (this.node.Guid);
					}
					else  // noeud de l'arbre ?
					{
						foreach (var children in this.childrens)
						{
							if (children.IsVisible)
							{
								return true;
							}
						}

						return false;
					}
				}
			}

			private readonly GroupLevelNodeGetter	nodeGetter;
			private readonly InternalTreeNode		parent;
			private readonly ParentNode				node;
			private readonly List<InternalTreeNode>	childrens;
		}


		private readonly INodeGetter<ParentNode>	inputNodes;
		private readonly DataAccessor				accessor;
		private readonly BaseType					baseType;
		private readonly List<LevelNode>			levelNodes;
		private readonly Dictionary<Guid, List<ParentNode>> parentChildrens;
		private readonly HashSet<Guid>				showedNodes;

		private Guid								rootGuid;
		private SortingInstructions					sortingInstructions;
		private bool								forceEmpty;
		private System.Func<Guid, bool>				filter;
	}
}

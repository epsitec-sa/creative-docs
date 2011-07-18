//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// Pour effectuer une opération complexe sur les lignes d’un document commercial (grouper, dégrouper,
	/// séparer ou souder), un arbre intermédiaire est créé :
	/// 1.	Les lignes sont converties dans un arbre qui reflète exactement la structure des groupes.
	/// 2.	Il est alors facile de travailler dans l’arbre pour effectuer une opération complexe.
	/// 3.	L’arbre est remis à plat en régénérant les « GroupIndex ».
	/// Ainsi, la numérotation est toujours parfaite et optimale (sans trous).
	/// Les phases 1 et 3 sont communes à toute les opérations. Pendant la phase numéro 2, on peut travailler
	/// sur une structure propre et claire, en oubliant complètement les « GroupIndex ».
	/// </summary>
	public class TreeEngine
	{
		public TreeEngine(IList<AbstractDocumentItemEntity> lines)
		{
			//	Construit l'arbre à partir des lignes du document comercial.
			this.root = new TreeNode ();

			foreach (var line in lines)
			{
				this.InsertEntity (line);
			}
		}

		public TreeNode Search(AbstractDocumentItemEntity entity)
		{
			//	Cherche la feuille correspondant à une entité.
			this.InitialiseForDeepNext ();

			var current = this.root;
			while (true)
			{
				current = TreeEngine.DeepNext (current);

				if (current == null)
				{
					break;
				}

				if (current.Entity == entity)
				{
					return current;
				}
			}

			return null;
		}

		public LinesError RegenerateAllGroupIndex()
		{
			//	Régénère tous les GroupIndex des noeuds et des feuilles.
			this.InitialiseForDeepNext ();

			//	Première passe.
			var current = this.root;
			while (true)
			{
				current = TreeEngine.DeepNext (current);

				if (current == null)
				{
					break;
				}

				var error = TreeEngine.RegenerateGroupIndexStep1 (current);

				if (error != LinesError.OK)
				{
					return error;
				}
			}

			//	Deuxième passe.
			current = this.root;
			while (true)
			{
				current = TreeEngine.DeepNext (current);

				if (current == null)
				{
					break;
				}

				TreeEngine.RegenerateGroupIndexStep2 (current);
			}

			return LinesError.OK;
		}

		private static LinesError RegenerateGroupIndexStep1(TreeNode node)
		{
			//	Régénère le GroupIndex d'un noeud ou d'une feuille.
			if (node.Entity == null)  // noeud ?
			{
				var regeneratedNode = node;
				int regeneratedGroupIndex = 0;

				while (true)
				{
					var parent = node.Parent;

					if (parent == null)
					{
						break;
					}

					int i = TreeEngine.GetIndex (parent, node);

					if (i+1 > 99)
					{
						return LinesError.Overflow;
					}

					if (parent.Deep >= 0 && i != -1)
					{
						regeneratedGroupIndex = LinesEngine.LevelReplace (regeneratedGroupIndex, parent.Deep, i+1);
					}

					node = parent;
				}

				regeneratedNode.GroupIndex = regeneratedGroupIndex;
			}

			return LinesError.OK;
		}

		private static void RegenerateGroupIndexStep2(TreeNode node)
		{
			//	Régénère le GroupIndex d'une feuille.
			if (node.Entity != null)  // feuille ?
			{
				//	ATTENTION:
				//	La propriété Entity.GroupIndex d'une feuille n'a pas la même signification que la
				//	propriété GroupIndex d'un noeud. Alors que cette dernière contient le chemin complet,
				//	Entity.GroupIndex occulte les 2 derniers digits. Si par exemple un noeud contient
				//	GroupIndex = 0502, sa première feuille devrait contenir Entity.GroupIndex = 010502,
				//	alors qu'en réalité elle ne contient que 0502 (comme le noeud parent) !
				node.Entity.GroupIndex = node.Parent.GroupIndex;
			}
		}

		private static int GetIndex(TreeNode parent, TreeNode node)
		{
			int index = 0;

			foreach (var child in parent.Childrens)
			{
				if (child == node)
				{
					break;
				}

				if (child.Entity == null)
				{
					index++;
				}
			}

			return index;
		}


		private void InsertEntity(AbstractDocumentItemEntity entity)
		{
			int level = LinesEngine.GetLevel (entity.GroupIndex);
			int progressGroupIndex = 0;

			var parent = this.root;

			for (int i = 0; i < level; i++)
			{
				int rank = LinesEngine.LevelExtract (entity.GroupIndex, i);
				progressGroupIndex = LinesEngine.LevelReplace (progressGroupIndex, i, rank);

				var next = parent.Childrens.Where (x => x.Entity == null && x.GroupIndex == progressGroupIndex).FirstOrDefault ();

				if (next == null)
				{
					next = new TreeNode (progressGroupIndex);
					parent.Childrens.Add (next);
				}

				parent = next;
			}

			parent.Childrens.Add (new TreeNode (entity));
		}


		private void InitialiseForDeepNext()
		{
			//	Initialise les propriétés Parent de tous les noeuds de l'arbre.
			//	Cela est indispensable avant de pouvoir utiliser DeepNext.
			this.root.Deep = 0;
			TreeEngine.InitialiseForDeepNext (this.root);
		}

		private static void InitialiseForDeepNext(TreeNode parent)
		{
			foreach (var node in parent.Childrens)
			{
				node.Parent = parent;
				node.Deep = parent.Deep+1;

				TreeEngine.InitialiseForDeepNext (node);
			}
		}

		private static TreeNode DeepNext(TreeNode node)
		{
			//	Retourne le noeud/feuille suivant, en parcourant l'arbre en profondeur.
			if (node.Childrens.Count == 0)
			{
				while (true)
				{
					if (node.Parent == null)
					{
						return null;
					}

					var parent = node.Parent;
					int index = parent.Childrens.IndexOf (node) + 1;

					if (index < parent.Childrens.Count)
					{
						return parent.Childrens[index];
					}

					node = parent;
				}
			}
			else
			{
				return node.Childrens[0];
			}
		}


		private readonly TreeNode root;
	}
}

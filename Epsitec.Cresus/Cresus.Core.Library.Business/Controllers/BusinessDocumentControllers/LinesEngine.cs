//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// C'est ici qu'est concentré toutes les opérations ayant trait aux lignes d'un document commercial.
	/// </summary>
	public class LinesEngine
	{
		public LinesEngine(BusinessContext businessContext, BusinessDocumentEntity businessDocumentEntity)
		{
			this.businessContext        = businessContext;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public LinesError LastError
		{
			//	Retourne la dernière erreur occasionnée par une action.
			get
			{
				return this.lastError;
			}
		}


		public List<LineInformations> CreateArticle(List<LineInformations> selection)
		{
			//	Crée un nouvel article.
			int index;
			AbstractDocumentItemEntity model;

			if (selection.Count == 0)
			{
				index = this.GetDefaultArticleInsertionIndex (out model);
			}
			else
			{
				index = this.businessDocumentEntity.Lines.IndexOf (selection.Last ().AbstractDocumentItemEntity) + 1;
				model = this.businessDocumentEntity.Lines[index-1];
			}

			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (ArticleQuantityType.Ordered);

			if (quantityColumnEntity == null)
			{
				this.lastError = LinesError.InvalidQuantity;
				return null;
			}

			var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
			newQuantity.Quantity = 1;
			newQuantity.QuantityColumn = quantityColumnEntity;

			var newLine = this.businessContext.CreateEntity<ArticleDocumentItemEntity> ();
			newLine.GroupIndex = model.GroupIndex;
			newLine.ArticleQuantities.Add (newQuantity);

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0));
		}

		public List<LineInformations> CreateQuantity(List<LineInformations> selection, ArticleQuantityType quantityType, int daysToAdd)
		{
			//	Crée une nouvelle quantité pour un article existant.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return null;
			}

			if (selection.Count != 1)
			{
				this.lastError = LinesError.InvalidSelection;
				return null;
			}

			var info = selection[0];
			var line = info.AbstractDocumentItemEntity;

			if (!(line is ArticleDocumentItemEntity))
			{
				this.lastError = LinesError.InvalidSelection;
				return null;
			}

			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (quantityType);

			if (quantityColumnEntity == null)
			{
				this.lastError = LinesError.InvalidQuantity;
				return null;
			}

			var article = line as ArticleDocumentItemEntity;
			var quantity = article.ArticleQuantities[info.SublineIndex];

			var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
			newQuantity.Quantity = 1;
			newQuantity.Unit = quantity.Unit;
			newQuantity.QuantityColumn = quantityColumnEntity;
			newQuantity.BeginDate = new Date (Date.Today.Ticks + Time.TicksPerDay*daysToAdd);  // n jours plus tard

			article.ArticleQuantities.Add (newQuantity);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, line, newQuantity, 0));
		}

		public List<LineInformations> CreateText(List<LineInformations> selection, bool isTitle)
		{
			//	Crée une nouvelle ligne de texte ou de titre.
			int index;
			AbstractDocumentItemEntity model;

			if (selection.Count == 0)
			{
				if (isTitle)
				{
					index = this.GetDefaultTitleInsertionIndex (out model);
				}
				else
				{
					index = this.GetDefaultArticleInsertionIndex (out model);
				}
			}
			else
			{
				index = this.businessDocumentEntity.Lines.IndexOf (selection.Last ().AbstractDocumentItemEntity) + 1;
				model = this.businessDocumentEntity.Lines[index-1];
			}

			var newLine = this.businessContext.CreateEntity<TextDocumentItemEntity> ();

			if (isTitle)
			{
				newLine.Text = string.Concat (LinesEngine.titlePrefixTags, LinesEngine.titlePostfixTags);
			}

			newLine.GroupIndex = model.GroupIndex;

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0));
		}


		public void Move(List<LineInformations> selection, int direction)
		{
			//	Déplace une ligne vers le haut ou vers le bas.
			//	C'est un mécanisme primitif, qui devra être remplacé par du drag & drop un jour !
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			var leaf = tree.Search (selection[0].AbstractDocumentItemEntity);
			var flat = tree.FlatLeafs;
			var index = flat.IndexOf (leaf);

			if (LinesEngine.IsFixed (leaf.Entity))
			{
				this.lastError = LinesError.Fixed;
				return;
			}

			TreeNode attachLeaf;
			int attachIndex = index;

			while (true)
			{
				attachIndex += direction;

				//	Vérifie si le déplacement est possible.
				if (attachIndex < 0)
				{
					this.lastError = LinesError.AlreadyOnTop;
					return;
				}

				if (attachIndex >= flat.Count)
				{
					this.lastError = LinesError.AlreadyOnBottom;
					return;
				}

				attachLeaf = flat[attachIndex];

				if (!LinesEngine.IsFixed (attachLeaf.Entity))
				{
					break;
				}
			}

			//	Met une copie de la feuille à sa nouvelle place.
			var newLeaf = new TreeNode (leaf.Entity);

			if (direction < 0)
			{
				int i = attachLeaf.Parent.Childrens.IndexOf (attachLeaf);

				//	Si la feuille originale n'a pas le même parent, il faut insérer après.
				if (attachLeaf.Parent.Childrens.Contains (leaf) == false)
				{
					i++;
				}

				attachLeaf.Parent.Childrens.Insert (i, newLeaf);
			}
			else
			{
				int i = attachLeaf.Parent.Childrens.IndexOf (attachLeaf);

				//	Si la feuille originale a le même parent, il faut insérer après.
				if (attachLeaf.Parent.Childrens.Contains (leaf) == true)
				{
					i++;
				}

				attachLeaf.Parent.Childrens.Insert (i, newLeaf);
			}

			//	Supprime la feuille initiale.
			leaf.Parent.Childrens.Remove (leaf);

			while ((leaf = leaf.Parent) != null)
			{
				if (leaf.Childrens.Count == 0 && leaf.Parent != null)
				{
					leaf.Parent.Childrens.Remove (leaf);
				}
				else
				{
					break;
				}
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);
		}

		private static bool IsFixed(AbstractDocumentItemEntity entity)
		{
			//	Retourne true si l'entité ne peut pas être déplacée.
			if (entity is SubTotalDocumentItemEntity ||
				entity is EndTotalDocumentItemEntity ||
				entity is TaxDocumentItemEntity      )
			{
				return true;
			}

			if (entity.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
			{
				return true;
			}

			return false;
		}


		public List<LineInformations> Delete(List<LineInformations> selection)
		{
			//	Supprime les lignes sélectionnées.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return null;
			}

			int last = 0;
			using (this.businessContext.SuspendUpdates ())
			{
				foreach (var info in selection)
				{
					var line     = info.AbstractDocumentItemEntity;
					var quantity = info.ArticleQuantityEntity;

					last = this.businessDocumentEntity.Lines.IndexOf (line);

					if (line is ArticleDocumentItemEntity && quantity != null && info.SublineIndex > 0)  // quantité ?
					{
						var article = line as ArticleDocumentItemEntity;
						article.ArticleQuantities.Remove (quantity);
					}
					else
					{
						this.businessDocumentEntity.Lines.Remove (line);
					}
				}
			}

			this.lastError = LinesError.OK;

			last = System.Math.Min (last, this.businessDocumentEntity.Lines.Count-1);
			var lineToSelect = this.businessDocumentEntity.Lines[last];
			return LinesEngine.MakeSingleSelection (new LineInformations (null, lineToSelect, null, 0));
		}

		public List<LineInformations> Duplicate(List<LineInformations> selection)
		{
			//	Duplique la ligne sélectionnée.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return null;
			}

			if (selection.Count != 1)
			{
				this.lastError = LinesError.InvalidSelection;
				return null;
			}

			var info = selection[0];
			var line = info.AbstractDocumentItemEntity;
			var index = this.businessDocumentEntity.Lines.IndexOf (line);

			if (index == -1)
			{
				this.lastError = LinesError.InvalidSelection;
				return null;
			}

			if (line.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
			{
				this.lastError = LinesError.InvalidSelection;
				return null;
			}

			var copy = line.CloneEntity (this.businessContext);
			this.businessDocumentEntity.Lines.Insert (index+1, copy);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, copy, null, 0));
		}


		public void MakeGroup(List<LineInformations> selection)
		{
			//	Fait un groupe.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			if (!this.IsCoherentSelection (selection))
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			if (LinesEngine.GetLevel (selection[0].AbstractDocumentItemEntity.GroupIndex) >= LinesEngine.maxGroupingDepth)
			{
				this.lastError = LinesError.MaxDeep;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			//	Crée le nouveau noeud qui regroupera les lignes sélectionnées.
			var group = new TreeNode ();

			var firstLeaf = tree.Search (selection[0].AbstractDocumentItemEntity);
			var parent = firstLeaf.Parent;
			int index = parent.Childrens.IndexOf (firstLeaf);
			parent.Childrens.Insert (index, group);  // insère le groupe à sa place

			//	Déplace les lignes dans le nouveau noeud du groupe.
			foreach (var info in selection)
			{
				var leaf = tree.Search (info.AbstractDocumentItemEntity);

				leaf.Parent.Childrens.Remove (leaf);
				group.Childrens.Add (leaf);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);
		}

		public void MakeUngroup(List<LineInformations> selection)
		{
			//	Défait un groupe.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			if (!this.IsCoherentSelection (selection))
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			if (LinesEngine.GetLevel (selection[0].AbstractDocumentItemEntity.GroupIndex) <= 1)
			{
				this.lastError = LinesError.MinDeep;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			var firstLeaf = tree.Search (selection[0].AbstractDocumentItemEntity);
			var group = firstLeaf.Parent;
			var parent = group.Parent;
			int index = parent.Childrens.IndexOf (group);

			//	Déplace le lignes sélectionnées hors du groupe.
			foreach (var info in selection)
			{
				var leaf = tree.Search (info.AbstractDocumentItemEntity);

				group.Childrens.Remove (leaf);
				parent.Childrens.Insert (++index, leaf);
			}

			//	Si le groupe est vide, supprime-le.
			if (group.Childrens.Count == 0)
			{
				parent.Childrens.Remove (group);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);
		}


		public void MakeSplit(List<LineInformations> selection)
		{
			//	Sépare la ligne sélectionnée d'avec la précédente.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			var firstLeaf = tree.Search (selection[0].AbstractDocumentItemEntity);
			var group = firstLeaf.Parent;
			var parent = group.Parent;

			if (group.Childrens.IndexOf (firstLeaf) == 0)  // déjà séparé ?
			{
				this.lastError = LinesError.AlreadySplited;
				return;
			}

			//	Crée le nouveau groupe juste après l'actuel.
			int index = parent.Childrens.IndexOf (group);
			var newGroup = new TreeNode ();
			parent.Childrens.Insert (index+1, newGroup);

			//	Déplace la ligne sélectionnée et les suivantes du même groupe dans le nouveau groupe.
			int start = group.Childrens.IndexOf (firstLeaf);
			while (start < group.Childrens.Count)
			{
				var node = group.Childrens[start];
				group.Childrens.RemoveAt (start);
				newGroup.Childrens.Add (node);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);
		}

		public void MakeCombine(List<LineInformations> selection)
		{
			//	Soude la ligne sélectionnée avec la précédente.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			var firstLeaf = tree.Search (selection[0].AbstractDocumentItemEntity);
			var group = firstLeaf.Parent;
			var parent = group.Parent;

			int index = group.Childrens.IndexOf (firstLeaf);
			if (index != 0)  // déjà soudé ?
			{
				this.lastError = LinesError.AlreadyCombined;
				return;
			}

			index = parent.Childrens.IndexOf (group);
			if (index == 0)  // déjà soudé ?
			{
				this.lastError = LinesError.AlreadyCombined;
				return;
			}

			var prevGroup = parent.Childrens[index-1];

			//	Déplace les lignes du groupe dans le groupe précédent.
			while (group.Childrens.Count != 0)
			{
				var node = group.Childrens[0];
				group.Childrens.RemoveAt (0);
				prevGroup.Childrens.Add (node);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);
		}


		private void Regenerate(TreeEngine tree)
		{
			//	Régénère la liste des lignes du document commercial selon le nouvel arbre.
			using (this.businessContext.SuspendUpdates ())
			{
				this.lastError = tree.Regenerate ();
			}
		}


		public bool IsCoherentSelection(List<LineInformations> selection)
		{
			//	Retourne true si toutes les lignes sélectionnées font partie du même groupe.
			if (selection.Count == 0)
			{
				return false;
			}

			var groupIndex = -1;
			foreach (var info in selection)
			{
				if (groupIndex == -1)
				{
					groupIndex = info.AbstractDocumentItemEntity.GroupIndex;
				}
				else
				{
					if (groupIndex != info.AbstractDocumentItemEntity.GroupIndex)
					{
						return false;
					}
				}
			}

			return true;
		}

	
		public FormattedText GetError(LinesError error)
		{
			//	Retourne le texte en clair correspondant à une erreur.
			switch (error)
			{
				case LinesError.InvalidSelection:
					return "L'opération est impossible avec les lignes sélectionnées.";

				case LinesError.EmptySelection:
					return "Aucune ligne n'est sélectionnée.";

				case LinesError.InvalidQuantity:
					return "La quantité à créer n'est pas définie dans les réglages globaux.";

				case LinesError.AlreadyOnTop:
					return "La ligne est déjà au sommet.";

				case LinesError.AlreadyOnBottom:
					return "La ligne est déjà à la fin.";

				case LinesError.MinDeep:
					return "Le groupe est déjà défait.";

				case LinesError.MaxDeep:
					return "Il n'est pas possible d'imbriquer plus profondément les lignes sélectionnées.";

				case LinesError.AlreadySplited:
					return "La ligne est déjà séparée de la précédente.";

				case LinesError.AlreadyCombined:
					return "La ligne est déjà soudée à la précédente.";

				case LinesError.Overflow:
					return "Il y a plus de 99 groupes successifs.";

				case LinesError.Fixed:
					return "Cette ligne ne peut pas être déplacée.";

				default:
					return null;
			}
		}


		#region Title manager
		public static FormattedText TitleToSimpleText(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return null;
			}

			string s = text.ToString ();

			s = s.Replace (LinesEngine.titlePrefixTags, "");
			s = s.Replace (LinesEngine.titlePostfixTags, "");

			return s;
		}

		public static FormattedText SimpleTextToTitle(FormattedText text)
		{
			return FormattedText.Concat (LinesEngine.titlePrefixTags, text, LinesEngine.titlePostfixTags);
		}

		public static bool IsTitle(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return false;
			}

			return text.ToString ().Contains (LinesEngine.titlePrefixTags);
		}
		#endregion


		#region GroupIndex operations
		public static bool LevelCompare(int groupIndex1, int groupIndex2, int levelCount)
		{
			for (int i = 0; i < levelCount; i++)
			{
				int n1 = LinesEngine.LevelExtract (groupIndex1, i);
				int n2 = LinesEngine.LevelExtract (groupIndex2, i);

				if (n1 != n2)
				{
					return false;
				}
			}

			return true;
		}

		public static int LevelReplace(int groupIndex, int level, int rank)
		{
			//	Remplace une paire de digits d'un niveau quelconque.
			//	groupIndex = 665544, level = 0, rank = 88 ->   665588
			//	groupIndex = 665544, level = 1, rank = 88 ->   668844
			//	groupIndex = 665544, level = 2, rank = 88 ->   885544
			//	groupIndex = 665544, level = 3, rank = 88 -> 88665544
			System.Diagnostics.Debug.Assert (groupIndex >= 0);
			System.Diagnostics.Debug.Assert (groupIndex <= 99999999);

			System.Diagnostics.Debug.Assert (level >= 0);
			System.Diagnostics.Debug.Assert (level < LinesEngine.maxGroupingDepth);

			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank <= 99);

			int result = 0;
			int f = 1;

			for (int i = 0; i < LinesEngine.maxGroupingDepth; i++)
			{
				if (i == level)
				{
					result += f * rank;
				}
				else
				{
					result += f * LinesEngine.LevelExtract (groupIndex, i);
				}

				f *= 100;
			}

			System.Diagnostics.Debug.Assert (result >= 0);
			System.Diagnostics.Debug.Assert (result <= 99999999);

			return result;
		}

		public static int LevelExtract(int groupIndex, int level)
		{
			//	Extrait une paire de digits.
			//	Retourne 0 si le niveau n'existe pas.
			//	groupIndex = 665544, level = 0 -> 44
			//	groupIndex = 665544, level = 1 -> 55
			//	groupIndex = 665544, level = 2 -> 66
			//	groupIndex = 665544, level = 3 ->  0
			//	groupIndex = 665544, level = 4 ->  0
			System.Diagnostics.Debug.Assert (groupIndex >= 0);
			System.Diagnostics.Debug.Assert (groupIndex <= 99999999);

			System.Diagnostics.Debug.Assert (level >= 0);

			if (level >= LinesEngine.maxGroupingDepth)
			{
				return 0;
			}
			else
			{
				int f = (int) System.Math.Pow (100, level);  // f = 1, 100, 10000 ou 1000000
				return (groupIndex/f) % 100;
			}
		}

		public static int GetLevel(int groupIndex)
		{
			//	Retourne le niveau, compris entre 0 et 4.
			//	       0 -> 0
			//	       4 -> 1
			//	      44 -> 1
			//	     544 -> 2
			//	    5544 -> 2
			//	   65544 -> 3
			//	  665544 -> 3
			//	 8665544 -> 4
			//	88665544 -> 4
			return AbstractDocumentItemEntity.GetGroupLevel (groupIndex);
		}
		#endregion


		private int GetDefaultTitleInsertionIndex(out AbstractDocumentItemEntity model)
		{
			int i = this.GetDefaultArticleInsertionIndex (out model);

			if (i == 0)
			{
				return 0;
			}

			i--;
			var g = this.businessDocumentEntity.Lines[i].GroupIndex;

			while (i >= 0 && g == this.businessDocumentEntity.Lines[i].GroupIndex)
			{
				i--;
			}

			i++;
			model = this.businessDocumentEntity.Lines[i];
			return i;
		}

		private int GetDefaultArticleInsertionIndex(out AbstractDocumentItemEntity model)
		{
			for (int i = this.businessDocumentEntity.Lines.Count-1; i >= 0; i--)
			{
				var line = this.businessDocumentEntity.Lines[i];

				if (line is ArticleDocumentItemEntity ||
					line is TextDocumentItemEntity)
				{
					model = this.businessDocumentEntity.Lines[i];
					return i+1;
				}
			}

			model = this.businessDocumentEntity.Lines[0];
			return 0;
		}


		private ArticleQuantityColumnEntity SearchArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return this.businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


		private static List<LineInformations> MakeSingleSelection(LineInformations info)
		{
			var list = new List<LineInformations> ();
			list.Add (info);

			return list;
		}


		private static readonly string titlePrefixTags  = "<font size=\"150%\"><b>";
		private static readonly string titlePostfixTags = "</b></font>";

		public static readonly int maxGroupingDepth = 4;

		private readonly BusinessContext				businessContext;
		private readonly BusinessDocumentEntity			businessDocumentEntity;

		private LinesError								lastError;
	}
}

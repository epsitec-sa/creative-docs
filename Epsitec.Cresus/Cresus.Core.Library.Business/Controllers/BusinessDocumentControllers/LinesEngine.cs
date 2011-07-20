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


		public List<LineInformations> CreateArticle(List<LineInformations> selection, bool isTax)
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
			newLine.GroupIndex = isTax ? 0 : model.GroupIndex;
			newLine.ArticleQuantities.Add (newQuantity);

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0));
		}

		public List<LineInformations> CreateQuantity(List<LineInformations> selection, ArticleQuantityType quantityType, int daysToAdd, bool simulation = false)
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

			if (simulation)
			{
				this.lastError = LinesError.OK;
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

		public List<LineInformations> CreateDiscount(List<LineInformations> selection)
		{
			//	Crée une nouvelle ligne de rabais.
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

			var newLine = this.businessContext.CreateEntity<SubTotalDocumentItemEntity> ();
			newLine.GroupIndex = model.GroupIndex;

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0));
		}


		public List<LineInformations> CreateGroup(List<LineInformations> selection)
		{
			//	Insère un nouveau groupe contenant un titre.
			int index;
			if (selection.Count == 0)
			{
				AbstractDocumentItemEntity model;
				index = this.GetDefaultArticleInsertionIndex (out model);
			}
			else
			{
				index = this.businessDocumentEntity.Lines.IndexOf (selection.Last ().AbstractDocumentItemEntity) + 1;
			}

			var line = this.businessDocumentEntity.Lines[index];

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			//	Crée le nouveau noeud qui contiendra le titre.
			var group = new TreeNode ();

			var leaf = tree.Search (line);
			var parent = leaf.Parent;
			index = parent.Childrens.IndexOf (leaf);
			parent.Childrens.Insert (index, group);  // insère le groupe à sa place

			//	Crée le titre.
			var title = this.businessContext.CreateEntity<TextDocumentItemEntity> ();
			title.Text = string.Concat (LinesEngine.titlePrefixTags, LinesEngine.titlePostfixTags);
			group.Childrens.Add (new TreeNode (title));

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);

			return LinesEngine.MakeSingleSelection (new LineInformations (null, title, null, 0));
		}


		public void Move(List<LineInformations> selection, int direction, bool simulation = false)
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

			if (simulation)
			{
				this.lastError = LinesError.OK;
				return;
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
			this.Regenerate (tree, simulation: simulation);
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


		public List<LineInformations> Duplicate(List<LineInformations> selection, bool simulation = false)
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

			if (simulation)
			{
				this.lastError = LinesError.OK;
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

		public List<LineInformations> Delete(List<LineInformations> selection, bool simulation = false)
		{
			//	Supprime les lignes sélectionnées.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return null;
			}

			if (simulation)
			{
				this.lastError = LinesError.OK;
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

					if (info.IsQuantity)  // quantité ?
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


		public void MakeGroup(List<LineInformations> selection, bool simulation = false)
		{
			//	Fait un groupe.
			selection = LinesEngine.PurgeSelection (selection);

			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			var rootEntity = this.GetRootEntity (selection);
			if (rootEntity == null)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			foreach (var info in selection)
			{
				if (AbstractDocumentItemEntity.GetGroupLevel (info.AbstractDocumentItemEntity.GroupIndex) >= AbstractDocumentItemEntity.maxGroupingDepth)
				{
					this.lastError = LinesError.MaxDeep;
					return;
				}
			}

			if (simulation)
			{
				this.lastError = LinesError.OK;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			//	Crée le nouveau noeud qui regroupera les lignes sélectionnées.
			var group = new TreeNode ();

			var firstLeaf = tree.Search (rootEntity);
			var parent = firstLeaf.Parent;
			int index = parent.Childrens.IndexOf (firstLeaf);
			parent.Childrens.Insert (index, group);  // insère le groupe à sa place

			//	Cherche tous les frères à grouper.
			var brothers = new List<TreeNode> ();
			foreach (var info in selection)
			{
				var node = tree.Search (info.AbstractDocumentItemEntity);

				while (node.Parent != firstLeaf.Parent)
				{
					node = node.Parent;
				}

				if (!brothers.Contains (node))
				{
					brothers.Add (node);
				}
			}

			//	Déplace les lignes dans le nouveau noeud du groupe.
			foreach (var brother in brothers)
			{
				brother.Parent.Childrens.Remove (brother);
				group.Childrens.Add (brother);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree, simulation: simulation);
		}

		public void MakeUngroup(List<LineInformations> selection, bool simulation = false)
		{
			//	Défait un groupe.
			selection = LinesEngine.PurgeSelection (selection);

			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			var rootEntity = this.GetRootEntity (selection);
			if (rootEntity == null)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			if (AbstractDocumentItemEntity.GetGroupLevel (rootEntity.GroupIndex) <= 1)
			{
				this.lastError = LinesError.MinDeep;
				return;
			}

			if (simulation)
			{
				this.lastError = LinesError.OK;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocumentEntity);

			var firstLeaf = tree.Search (rootEntity);
			var group = firstLeaf.Parent;
			var parent = group.Parent;
			int index = parent.Childrens.IndexOf (group);

			//	Cherche tous les frères à grouper.
			var brothers = new List<TreeNode> ();
			foreach (var info in selection)
			{
				var node = tree.Search (info.AbstractDocumentItemEntity);

				while (node.Parent != firstLeaf.Parent)
				{
					node = node.Parent;
				}

				if (!brothers.Contains (node))
				{
					brothers.Add (node);
				}
			}

			//	Déplace le lignes sélectionnées hors du groupe.
			foreach (var brother in brothers)
			{
				group.Childrens.Remove (brother);
				parent.Childrens.Insert (++index, brother);
			}

			//	Si le groupe est vide, supprime-le.
			if (group.Childrens.Count == 0)
			{
				parent.Childrens.Remove (group);
			}

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree, simulation: simulation);
		}


		public void MakeSplit(List<LineInformations> selection, bool simulation = false)
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

			if (simulation)
			{
				this.lastError = LinesError.OK;
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
			this.Regenerate (tree, simulation: simulation);
		}

		public void MakeCombine(List<LineInformations> selection, bool simulation = false)
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

			if (simulation)
			{
				this.lastError = LinesError.OK;
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
			this.Regenerate (tree, simulation: simulation);
		}

		public void MakeFlat(List<LineInformations> selection)
		{
			//	Remet à plat toutes les lignes.
			bool make = false;

			using (this.businessContext.SuspendUpdates ())
			{
				foreach (var line in this.businessDocumentEntity.Lines)
				{
					if (line.GroupIndex > 1)
					{
						line.GroupIndex = 1;
						make = true;
					}
				}
			}

			if (make)
			{
				this.lastError = LinesError.OK;
			}
			else
			{
				this.lastError = LinesError.AlreadyFlat;
			}
		}


		private void Regenerate(TreeEngine tree, bool simulation = false)
		{
			//	Régénère la liste des lignes du document commercial selon le nouvel arbre.
			using (this.businessContext.SuspendUpdates ())
			{
				this.lastError = tree.Regenerate (simulation: simulation);
			}
		}


		private AbstractDocumentItemEntity GetRootEntity(List<LineInformations> selection)
		{
			//	Cherche le GroupIndex le plus petit.
			int minGroupIndex = int.MaxValue;
			AbstractDocumentItemEntity rootEntity = null;

			foreach (var info in selection)
			{
				if (minGroupIndex > info.AbstractDocumentItemEntity.GroupIndex)
				{
					minGroupIndex = info.AbstractDocumentItemEntity.GroupIndex;
					rootEntity = info.AbstractDocumentItemEntity;
				}
			}

			if (minGroupIndex == 0 || rootEntity == null)
			{
				return null;
			}

			//	Vérifie la cohérence.
			int level = AbstractDocumentItemEntity.GetGroupLevel (minGroupIndex);

			foreach (var info in selection)
			{
				if (!AbstractDocumentItemEntity.GroupCompare (minGroupIndex, info.AbstractDocumentItemEntity.GroupIndex, level))
				{
					return null;
				}
			}

			return rootEntity;
		}

		public bool IsCreateQuantityEnabled(List<LineInformations> selection)
		{
			this.CreateQuantity (selection, ArticleQuantityType.Delayed, 7, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsMoveEnabled(List<LineInformations> selection, int direction)
		{
			this.Move (selection, direction, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsDuplicateEnabled(List<LineInformations> selection)
		{
			this.Duplicate (selection, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsDeleteEnabled(List<LineInformations> selection)
		{
			this.Delete (selection, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsGroupEnabled(List<LineInformations> selection)
		{
			this.MakeGroup (selection, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsUngroupEnabled(List<LineInformations> selection)
		{
			this.MakeUngroup (selection, simulation: true);
			return this.lastError == LinesError.OK;
		}

		public bool IsSplitEnabled(List<LineInformations> selection)
		{
			this.MakeSplit (selection, simulation: true);
			return this.lastError == LinesError.OK;

			//	Retourne true si la commande Split est active.
			if (selection.Count != 1)
			{
				return false;
			}

			var line = selection[0].AbstractDocumentItemEntity;
			var index = this.businessDocumentEntity.Lines.IndexOf (line);

			if (index == 0 || line.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
			{
				return false;
			}

			var prevLine = this.businessDocumentEntity.Lines[index-1];

			if (line.GroupIndex != prevLine.GroupIndex)
			{
				return false;
			}

			return true;
		}

		public bool IsCombineEnabled(List<LineInformations> selection)
		{
			this.MakeCombine (selection, simulation: true);
			return this.lastError == LinesError.OK;

			//	Retourne true si la commande Combine est active.
			if (selection.Count != 1)
			{
				return false;
			}

			var line = selection[0].AbstractDocumentItemEntity;
			var index = this.businessDocumentEntity.Lines.IndexOf (line);

			if (index == 0 || line.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
			{
				return false;
			}

			var prevLine = this.businessDocumentEntity.Lines[index-1];

			if (line.GroupIndex == prevLine.GroupIndex)
			{
				return false;
			}

			return true;
		}

		public bool IsFlatEnabled
		{
			//	Retourne true si la commande Flat est active.
			get
			{
				bool enable = false;

				foreach (var line in this.businessDocumentEntity.Lines)
				{
					if (line.GroupIndex > 1)
					{
						enable = true;
						break;
					}
				}

				return enable;
			}
		}

	
		public FormattedText GetError(LinesError error)
		{
			//	Retourne le texte en clair d'une erreur.
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

				case LinesError.AlreadyFlat:
					return "Les lignes sont déjà à plat.";

				case LinesError.Overflow:
					return "Il y a plus de 99 groupes successifs.";

				case LinesError.Fixed:
					return "Cette ligne ne peut pas être déplacée.";

				default:
					return null;
			}
		}


		#region Static title manager
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


		private static List<LineInformations> PurgeSelection(List<LineInformations> selection)
		{
			//	Purge les lignes "quantité pour un article" d'une sélection.
			return selection.Where (x => !x.IsQuantity).ToList ();
		}


		private int GetDefaultTitleInsertionIndex(out AbstractDocumentItemEntity model)
		{
			//	Retourne l'index par défaut où insérer un titre.
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
			//	Retourne l'index par défaut où insérer un article.
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
			//	Retourne une sélection constituée d'une seule ligne.
			var list = new List<LineInformations> ();
			list.Add (info);

			return list;
		}


		private static readonly string titlePrefixTags  = "<font size=\"150%\"><b>";
		private static readonly string titlePostfixTags = "</b></font>";

		private readonly BusinessContext				businessContext;
		private readonly BusinessDocumentEntity			businessDocumentEntity;

		private LinesError								lastError;
	}
}

//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class LineEngine
	{
		public LineEngine(BusinessContext businessContext, BusinessDocumentEntity businessDocument, DocumentLogic documentLogic)
		{
			this.businessContext  = businessContext;
			this.businessDocument = businessDocument;
			this.documentLogic    = documentLogic;
		}


		public LineError LastError
		{
			//	Retourne la dernière erreur occasionnée par une action.
			get
			{
				return this.lastError;
			}
		}


		public List<Line> CreateArticle(List<Line> selection, bool isTax)
		{
			//	Crée un nouvel article.
			int index;
			AbstractDocumentItemEntity model;

			if (selection.Count == 0)
			{
				index = this.GetDefaultArticleInsertionIndex (out model, skipGroupZero: true);
			}
			else
			{
				index = this.businessDocument.Lines.IndexOf (selection.Last ().DocumentItem) + 1;
				model = this.businessDocument.Lines[System.Math.Max (0, index-1)];

				//	Make sure we don't select an insertion position in the top level group, as
				//	it is reserved for discounts and taxes, and not for standard articles.

				while (model.GroupIndex == 0)
				{
					index--;
					
					if (index <= 0)
					{
						index = 0;
						model = null;
						break;
					}

					model = this.businessDocument.Lines[index-1];
				}
			}

			var articleQuantityType = this.documentLogic.MainArticleQuantityType;

			if (articleQuantityType == ArticleQuantityType.None)
			{
				articleQuantityType = ArticleQuantityType.Ordered;
			}

			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (articleQuantityType);

			if (quantityColumnEntity == null)
			{
				this.lastError = LineError.InvalidQuantity;
				return null;
			}

			var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
			newQuantity.Quantity = 1;
			newQuantity.QuantityColumn = quantityColumnEntity;

			var newLine = this.businessContext.CreateEntity<ArticleDocumentItemEntity> ();
			newLine.GroupIndex = isTax ? 0 : ((model == null) ? 1 : model.GroupIndex);
			newLine.ArticleQuantities.Add (newQuantity);

			this.businessDocument.Lines.Insert (index, newLine);

			this.lastError = LineError.OK;
			return LineEngine.MakeSingleSelection (new Line (null, newLine, null));
		}

		public List<Line> CreateQuantity(List<Line> selection, bool simulation = false)
		{
			//	Crée une nouvelle quantité pour un article existant.
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return null;
			}

			if (selection.Count != 1)
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			var info = selection[0];
			var line = info.DocumentItem;

			if (!(line is ArticleDocumentItemEntity))
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			//	Utilise le premier type de la liste de BusinessLogic.
			ArticleQuantityType quantityType = this.documentLogic.GetEnabledArticleQuantityTypes ().FirstOrDefault ();
			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (quantityType);

			if (quantityColumnEntity == null)
			{
				this.lastError = LineError.InvalidQuantity;
				return null;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
				return null;
			}

			var article = line as ArticleDocumentItemEntity;
			var quantity = article.ArticleQuantities[info.QuantityIndex];

			var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
			newQuantity.Quantity = 1;
			newQuantity.Unit = quantity.Unit;
			newQuantity.QuantityColumn = quantityColumnEntity;
			newQuantity.BeginDate = new Date (Date.Today.Ticks + Time.TicksPerDay*7);  // 7 jours plus tard, arbitrairement

			article.ArticleQuantities.Add (newQuantity);

			this.lastError = LineError.OK;
			return LineEngine.MakeSingleSelection (new Line (null, line, newQuantity));
		}

		public List<Line> CreateText(List<Line> selection, bool isTitle)
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
				index = this.businessDocument.Lines.IndexOf (selection.Last ().DocumentItem) + 1;
				model = this.businessDocument.Lines[index-1];
			}

			var newLine = this.businessContext.CreateEntity<TextDocumentItemEntity> ();

			if (isTitle)
			{
				newLine.Text = string.Concat (LineEngine.TitlePrefixTags, LineEngine.TitlePostfixTags);
			}

			//	Si la logique d'entreprise dit qu'on édite que les textes internes, met directement la bonne coche.
			if (this.documentLogic.IsMyEyesOnlyEditionEnabled)
			{
				newLine.Attributes = DocumentItemAttributes.MyEyesOnly;
			}

			newLine.GroupIndex = (model == null) ? 1 : model.GroupIndex;

			this.businessDocument.Lines.Insert (index, newLine);

			this.lastError = LineError.OK;
			return LineEngine.MakeSingleSelection (new Line (null, newLine, null));
		}

		public List<Line> CreateDiscount(List<Line> selection)
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
				index = this.businessDocument.Lines.IndexOf (selection.Last ().DocumentItem) + 1;
				model = this.businessDocument.Lines[index-1];
			}

			var newLine = this.businessContext.CreateEntity<SubTotalDocumentItemEntity> ();
			newLine.GroupIndex = (model == null) ? 1 : model.GroupIndex;

			this.businessDocument.Lines.Insert (index, newLine);

			this.lastError = LineError.OK;
			return LineEngine.MakeSingleSelection (new Line (null, newLine, null));
		}


		public List<Line> CreateGroup(List<Line> selection)
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
				index = this.businessDocument.Lines.IndexOf (selection.Last ().DocumentItem) + 1;
			}

			var line = this.businessDocument.Lines[index];

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

			//	Crée le nouveau noeud qui contiendra le titre.
			var group = new TreeNode ();

			var leaf = tree.Search (line);
			var parent = leaf.Parent;
			index = parent.Childrens.IndexOf (leaf);
			parent.Childrens.Insert (index, group);  // insère le groupe à sa place

			//	Crée le titre.
			var title = this.businessContext.CreateEntity<TextDocumentItemEntity> ();
			title.Text = string.Concat (LineEngine.TitlePrefixTags, LineEngine.TitlePostfixTags);
			group.Childrens.Add (new TreeNode (title));

			//	Régénère toutes les lignes selon le nouvel arbre.
			this.Regenerate (tree);

			return LineEngine.MakeSingleSelection (new Line (null, title, null));
		}


		public void Move(List<Line> selection, int direction, bool simulation = false)
		{
			//	Déplace une ligne vers le haut ou vers le bas.
			//	C'est un mécanisme primitif, qui devra être remplacé par du drag & drop un jour !
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			//	Vérifie si la sélection est compatible avec la logique d'entreprise.
			if (!this.IsBusinessLogicAccepted (selection))
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

			var leaf = tree.Search (selection[0].DocumentItem);
			var flat = tree.FlatLeafs;
			var index = flat.IndexOf (leaf);

			if (LineEngine.IsFixed (leaf.Entity))
			{
				this.lastError = LineError.Fixed;
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
					this.lastError = LineError.AlreadyOnTop;
					return;
				}

				if (attachIndex >= flat.Count)
				{
					this.lastError = LineError.AlreadyOnBottom;
					return;
				}

				attachLeaf = flat[attachIndex];

				if (!LineEngine.IsFixed (attachLeaf.Entity))
				{
					break;
				}
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
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


		public List<Line> Duplicate(List<Line> selection, bool simulation = false)
		{
			//	Duplique la ligne sélectionnée.
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return null;
			}

			if (selection.Count != 1)
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
				return null;
			}

			var info = selection[0];
			var line = info.DocumentItem;
			var index = this.businessDocument.Lines.IndexOf (line);

			if (index == -1)
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			if (line.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			var copy = line.CloneEntity (this.businessContext);
			this.businessDocument.Lines.Insert (index+1, copy);

			this.lastError = LineError.OK;
			return LineEngine.MakeSingleSelection (new Line (null, copy, null));
		}

		public List<Line> Delete(List<Line> selection, bool simulation = false)
		{
			//	Supprime les lignes sélectionnées.
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return null;
			}

			//	Vérifie si la sélection est compatible avec la logique d'entreprise.
			if (!this.IsBusinessLogicAccepted (selection))
			{
				this.lastError = LineError.InvalidSelection;
				return null;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
				return null;
			}

			int last = 0;
			using (this.businessContext.SuspendUpdates ())
			{
				foreach (var info in selection)
				{
					var line     = info.DocumentItem;
					var quantity = info.ArticleQuantity;

					last = this.businessDocument.Lines.IndexOf (line);

					if (info.IsQuantity)  // quantité ?
					{
						var article = line as ArticleDocumentItemEntity;
						article.ArticleQuantities.Remove (quantity);
					}
					else
					{
						this.businessDocument.Lines.Remove (line);
					}
				}
			}

			this.lastError = LineError.OK;

			last = System.Math.Min (last, this.businessDocument.Lines.Count-1);
			var lineToSelect = this.businessDocument.Lines[last];
			return LineEngine.MakeSingleSelection (new Line (null, lineToSelect, null));
		}


		public void MakeGroup(List<Line> selection, bool simulation = false)
		{
			//	Fait un groupe.
			selection = LineEngine.PurgeSelection (selection);

			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return;
			}

			var rootEntity = this.GetRootEntity (selection);
			if (rootEntity == null)
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			foreach (var info in selection)
			{
				if (AbstractDocumentItemEntity.GetGroupLevel (info.DocumentItem.GroupIndex) >= AbstractDocumentItemEntity.maxGroupingDepth)
				{
					this.lastError = LineError.MaxDeep;
					return;
				}
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

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
				var node = tree.Search (info.DocumentItem);

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

		public void MakeUngroup(List<Line> selection, bool simulation = false)
		{
			//	Défait un groupe.
			selection = LineEngine.PurgeSelection (selection);

			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return;
			}

			var rootEntity = this.GetRootEntity (selection);
			if (rootEntity == null)
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			if (AbstractDocumentItemEntity.GetGroupLevel (rootEntity.GroupIndex) <= 1)
			{
				this.lastError = LineError.MinDeep;
				return;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

			var firstLeaf = tree.Search (rootEntity);
			var group = firstLeaf.Parent;
			var parent = group.Parent;
			int index = parent.Childrens.IndexOf (group);

			//	Cherche tous les frères à grouper.
			var brothers = new List<TreeNode> ();
			foreach (var info in selection)
			{
				var node = tree.Search (info.DocumentItem);

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


		public void MakeSplit(List<Line> selection, bool simulation = false)
		{
			//	Sépare la ligne sélectionnée d'avec la précédente.
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

			var firstLeaf = tree.Search (selection[0].DocumentItem);
			var group = firstLeaf.Parent;
			var parent = group.Parent;

			if (parent == null)
			{
				this.lastError = LineError.AlreadySplited;
				return;
			}

			if (group.Childrens.IndexOf (firstLeaf) == 0)  // déjà séparé ?
			{
				this.lastError = LineError.AlreadySplited;
				return;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
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

		public void MakeCombine(List<Line> selection, bool simulation = false)
		{
			//	Soude la ligne sélectionnée avec la précédente.
			if (selection.Count == 0)
			{
				this.lastError = LineError.EmptySelection;
				return;
			}

			if (selection.Count != 1)
			{
				this.lastError = LineError.InvalidSelection;
				return;
			}

			//	Crée l'arbre à partie des lignes du document.
			var tree = new TreeEngine (this.businessDocument);

			var firstLeaf = tree.Search (selection[0].DocumentItem);
			var group = firstLeaf.Parent;
			var parent = group.Parent;

			if (parent == null)
			{
				this.lastError = LineError.AlreadyCombined;
				return;
			}

			int index = group.Childrens.IndexOf (firstLeaf);
			if (index != 0)  // déjà soudé ?
			{
				this.lastError = LineError.AlreadyCombined;
				return;
			}

			index = parent.Childrens.IndexOf (group);
			if (index == 0)  // déjà soudé ?
			{
				this.lastError = LineError.AlreadyCombined;
				return;
			}

			if (simulation)
			{
				this.lastError = LineError.OK;
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

		public void MakeFlat(List<Line> selection)
		{
			//	Remet à plat toutes les lignes.
			bool make = false;

			using (this.businessContext.SuspendUpdates ())
			{
				foreach (var line in this.businessDocument.Lines)
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
				this.lastError = LineError.OK;
			}
			else
			{
				this.lastError = LineError.AlreadyFlat;
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


		private AbstractDocumentItemEntity GetRootEntity(List<Line> selection)
		{
			//	Cherche le GroupIndex le plus petit.
			int minGroupIndex = int.MaxValue;
			AbstractDocumentItemEntity rootEntity = null;

			foreach (var info in selection)
			{
				if (minGroupIndex > info.DocumentItem.GroupIndex)
				{
					minGroupIndex = info.DocumentItem.GroupIndex;
					rootEntity = info.DocumentItem;
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
				if (!AbstractDocumentItemEntity.GroupCompare (minGroupIndex, info.DocumentItem.GroupIndex, level))
				{
					return null;
				}
			}

			return rootEntity;
		}

		public bool IsCreateQuantityEnabled(List<Line> selection)
		{
			this.CreateQuantity (selection, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsMoveEnabled(List<Line> selection, int direction)
		{
			this.Move (selection, direction, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsDuplicateEnabled(List<Line> selection)
		{
			this.Duplicate (selection, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsDeleteEnabled(List<Line> selection)
		{
			this.Delete (selection, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsGroupEnabled(List<Line> selection)
		{
			this.MakeGroup (selection, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsUngroupEnabled(List<Line> selection)
		{
			this.MakeUngroup (selection, simulation: true);
			return this.lastError == LineError.OK;
		}

		public bool IsSplitEnabled(List<Line> selection)
		{
			this.MakeSplit (selection, simulation: true);
			return this.lastError == LineError.OK;
#if false
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
#endif
		}

		public bool IsCombineEnabled(List<Line> selection)
		{
			this.MakeCombine (selection, simulation: true);
			return this.lastError == LineError.OK;
#if false
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
#endif
		}

		public bool IsFlatEnabled
		{
			//	Retourne true si la commande Flat est active.
			get
			{
				bool enable = false;

				foreach (var line in this.businessDocument.Lines)
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

		private bool IsBusinessLogicAccepted(List<Line> selection)
		{
			//	Indique si la sélection est compatible avec les contraintes de la logique d'entreprise.
			foreach (var info in selection)
			{
				if (!this.documentLogic.IsEditionEnabled (info))
				{
					return false;
				}
			}

			return true;
		}

	
		public FormattedText GetError(LineError error)
		{
			//	Retourne le texte en clair d'une erreur.
			switch (error)
			{
				case LineError.InvalidSelection:
					return "L'opération est impossible avec les lignes sélectionnées.";

				case LineError.EmptySelection:
					return "Aucune ligne n'est sélectionnée.";

				case LineError.InvalidQuantity:
					return "La quantité à créer n'est pas définie dans les réglages globaux.";

				case LineError.AlreadyOnTop:
					return "La ligne est déjà au sommet.";

				case LineError.AlreadyOnBottom:
					return "La ligne est déjà à la fin.";

				case LineError.MinDeep:
					return "Le groupe est déjà défait.";

				case LineError.MaxDeep:
					return "Il n'est pas possible d'imbriquer plus profondément les lignes sélectionnées.";

				case LineError.AlreadySplited:
					return "La ligne est déjà séparée de la précédente.";

				case LineError.AlreadyCombined:
					return "La ligne est déjà soudée à la précédente.";

				case LineError.AlreadyFlat:
					return "Les lignes sont déjà à plat.";

				case LineError.Overflow:
					return "Il y a plus de 99 groupes successifs.";

				case LineError.Fixed:
					return "Cette ligne ne peut pas être déplacée.";

				case LineError.OnlyQuantity:
					return "On ne peut supprimer que des quantités.";

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

			s = s.Replace (LineEngine.TitlePrefixTags, "");
			s = s.Replace (LineEngine.TitlePostfixTags, "");

			return s;
		}

		public static FormattedText SimpleTextToTitle(FormattedText text)
		{
			return FormattedText.Concat (LineEngine.TitlePrefixTags, text, LineEngine.TitlePostfixTags);
		}

		public static bool IsTitle(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return false;
			}

			return text.ToString ().Contains (LineEngine.TitlePrefixTags);
		}
		#endregion


		private static List<Line> PurgeSelection(List<Line> selection)
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
			var g = this.businessDocument.Lines[i].GroupIndex;

			while (i >= 0 && g == this.businessDocument.Lines[i].GroupIndex)
			{
				i--;
			}

			i++;
			model = this.businessDocument.Lines[i];
			return i;
		}

		private int GetDefaultArticleInsertionIndex(out AbstractDocumentItemEntity model, bool skipGroupZero = false)
		{
			//	Retourne l'index par défaut où insérer un article.
			for (int i = this.businessDocument.Lines.Count-1; i >= 0; i--)
			{
				var line = this.businessDocument.Lines[i];

				if (skipGroupZero && line.GroupIndex == 0)
				{
					continue;
				}

				if (line is ArticleDocumentItemEntity ||
					line is TextDocumentItemEntity)
				{
					model = this.businessDocument.Lines[i];
					return i+1;
				}
			}

			if (this.businessDocument.Lines.Count == 0)
			{
				model = null;
			}
			else
			{
				model = this.businessDocument.Lines[0];

				if (skipGroupZero && model.GroupIndex == 0)
				{
					model = null;
				}
			}

			return 0;
		}


		private ArticleQuantityColumnEntity SearchArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return this.businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


		private static List<Line> MakeSingleSelection(Line info)
		{
			//	Retourne une sélection constituée d'une seule ligne.
			var list = new List<Line> ();
			list.Add (info);

			return list;
		}


		private static readonly string TitlePrefixTags  = "<font size=\"150%\"><b>";
		private static readonly string TitlePostfixTags = "</b></font>";

		private readonly BusinessContext		businessContext;
		private readonly BusinessDocumentEntity	businessDocument;
		private readonly DocumentLogic			documentLogic;

		private LineError						lastError;
	}
}

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

			if (selection.Count == 0)
			{
				index = this.GetDefaultArticleInsertionIndex ();
			}
			else
			{
				index = this.businessDocumentEntity.Lines.IndexOf (selection.Last ().AbstractDocumentItemEntity) + 1;
			}

			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (ArticleQuantityType.Ordered);

			if (quantityColumnEntity == null)
			{
				this.lastError = LinesError.InvalidQuantity;
				return null;
			}

			var model = this.businessDocumentEntity.Lines[index-1];

			var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
			newQuantity.Quantity = 1;
			newQuantity.QuantityColumn = quantityColumnEntity;

			var newLine = this.businessContext.CreateEntity<ArticleDocumentItemEntity> ();
			newLine.GroupIndex = model.GroupIndex;
			newLine.ArticleQuantities.Add (newQuantity);

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0, 0));
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
			return LinesEngine.MakeSingleSelection (new LineInformations (null, line, newQuantity, 0, 0));
		}

		public List<LineInformations> CreateText(List<LineInformations> selection, bool isTitle)
		{
			//	Crée une nouvelle ligne de texte ou de titre.
			int index;

			if (selection.Count == 0)
			{
				index = this.GetDefaultArticleInsertionIndex ();
			}
			else
			{
				index = this.businessDocumentEntity.Lines.IndexOf (selection.Last ().AbstractDocumentItemEntity) + 1;
			}

			var model = this.businessDocumentEntity.Lines[index-1];

			var newLine = this.businessContext.CreateEntity<TextDocumentItemEntity> ();

			if (isTitle)
			{
				newLine.Text = string.Concat (LinesEngine.titlePrefixTags, LinesEngine.titlePostfixTags);
			}

			newLine.GroupIndex = model.GroupIndex;

			this.businessDocumentEntity.Lines.Insert (index, newLine);

			this.lastError = LinesError.OK;
			return LinesEngine.MakeSingleSelection (new LineInformations (null, newLine, null, 0, 0));
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

			var info = selection[0];
			var line = info.AbstractDocumentItemEntity;
			var index = this.businessDocumentEntity.Lines.IndexOf (line);

			if (index+direction < 0)
			{
				this.lastError = LinesError.AlreadyOnTop;
				return;
			}

			if (index+direction >= this.businessDocumentEntity.Lines.Count)
			{
				this.lastError = LinesError.AlreadyOnBottom;
				return;
			}

			this.businessDocumentEntity.Lines.RemoveAt (index);
			this.businessDocumentEntity.Lines.Insert (index+direction, line);

			this.lastError = LinesError.OK;
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
			return LinesEngine.MakeSingleSelection (new LineInformations (null, lineToSelect, null, 0, 0));
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
			return LinesEngine.MakeSingleSelection (new LineInformations (null, copy, null, 0, 0));
		}


		public void MakeGroup(List<LineInformations> selection, bool group)
		{
			//	Fait ou défait un groupe.
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

			using (this.businessContext.SuspendUpdates ())
			{
				foreach (var info in selection)
				{
					var line = info.AbstractDocumentItemEntity;

					var list = LinesEngine.GroupIndexSplit (line.GroupIndex);

					if (group)
					{
						list.Add (1);
					}
					else
					{
						list.RemoveAt (list.Count-1);
					}

					line.GroupIndex = LinesEngine.GroupIndexCombine (list);
				}
			}

			this.lastError = LinesError.OK;
		}

		public void ShiftGroup(List<LineInformations> selection, int increment)
		{
			//	Sépare la ligne sélectionnée d'avec la précédente.
			if (selection.Count == 0)
			{
				this.lastError = LinesError.EmptySelection;
				return;
			}

			var info = selection[0];
			var line = info.AbstractDocumentItemEntity;
			var index = this.businessDocumentEntity.Lines.IndexOf (line);

			if (index == -1)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			var initialList = LinesEngine.GroupIndexSplit (line.GroupIndex);
			var level = initialList.Count-1;

			if (initialList[level]+increment == 0 ||
				initialList[level]+increment >= 99)
			{
				this.lastError = LinesError.InvalidSelection;
				return;
			}

			using (this.businessContext.SuspendUpdates ())
			{
				for (int i = index; i < this.businessDocumentEntity.Lines.Count; i++)
				{
					var item = this.businessDocumentEntity.Lines[i];

					var list = LinesEngine.GroupIndexSplit (item.GroupIndex);

					if (!LinesEngine.GroupIndexCompare (list, initialList, level))
					{
						break;
					}

					if (level < list.Count)
					{
						list[level] += increment;
						item.GroupIndex = LinesEngine.GroupIndexCombine (list);
					}
				}
			}

			this.lastError = LinesError.OK;
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


		#region Group index list manager
		private static bool GroupIndexCompare(List<int> list1, List<int> list2, int deep)
		{
			if (list1.Count < deep || list2.Count < deep)
			{
				return false;
			}

			for (int i = 0; i < deep; i++)
			{
				if (list1[i] != list2[i])
				{
					return false;
				}
			}

			return true;
		}

		public static List<int> GroupIndexSplit(int groupIndex)
		{
			//	30201 retourne la liste 1,2,3.
			var list = new List<int> ();

			while (groupIndex != 0)
			{
				list.Add (groupIndex%100);
				groupIndex /= 100;
			}


			return list;
		}

		public static int GroupIndexCombine(List<int> list)
		{
			//	La liste 1,2,3 retourne 30201.
			int groupIndex = 0;
			int factor = 1;

			foreach (var n in list)
			{
				groupIndex += factor * n;
				factor *= 100;
			}

			return groupIndex;
		}
		#endregion


		private int GetDefaultArticleInsertionIndex()
		{
			for (int i = this.businessDocumentEntity.Lines.Count-1; i >= 0; i--)
			{
				var line = this.businessDocumentEntity.Lines[i];

				if (line is ArticleDocumentItemEntity ||
					line is TextDocumentItemEntity)
				{
					return i+1;
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


		private static List<LineInformations> MakeSingleSelection(LineInformations info)
		{
			var list = new List<LineInformations> ();
			list.Add (info);

			return list;
		}


		private static readonly string titlePrefixTags  = "<font size=\"150%\"><b>";
		private static readonly string titlePostfixTags = "</b></font>";

		private static readonly int maxGroupingDepth = 4;

		private readonly BusinessContext				businessContext;
		private readonly BusinessDocumentEntity			businessDocumentEntity;

		private LinesError								lastError;
	}
}

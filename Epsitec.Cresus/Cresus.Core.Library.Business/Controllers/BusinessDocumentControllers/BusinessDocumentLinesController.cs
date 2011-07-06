//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class BusinessDocumentLinesController
	{
		public BusinessDocumentLinesController(BusinessContext businessContext, DataContext dataContext, CoreData coreData, DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.businessContext        = businessContext;
			this.dataContext            = dataContext;
			this.coreData               = coreData;
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;

			this.articleLineInformations = new List<ArticleLineInformations> ();
			this.UpdateArticleLineInformations ();
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			//	Crée la toolbar.
			this.articleLineToolbarController = new ArticleLineToolbarController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.articleLineToolbarController.CreateUI (frame, this.Action);

			//	Crée la liste.
			this.articleLinesController = new ArticleLinesController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.articleLinesController.CreateUI (frame, this.CallbackSelectionChanged);

			//	Crée l'éditeur pour une ligne.
			this.editionArticleLineController = new ArticleLineEditorController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.editionArticleLineController.CreateUI (frame);
		}

		public void UpdateUI(int? sel = null)
		{
			this.articleLinesController.UpdateUI (this.articleLineInformations.Count, this.CallbackGetCellContent, sel);
			this.CallbackSelectionChanged ();
		}


		private FormattedText CallbackGetCellContent(int index, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var info = this.articleLineInformations[index];

			var line     = info.AbstractDocumentItemEntity;
			var quantity = info.ArticleQuantityEntity;

			if (quantity != null)
			{
				switch (columnType)
				{
					case ColumnType.QuantityAndUnit:
						return BusinessDocumentLinesController.GetArticleQuantityAndUnit (quantity);

					case ColumnType.Type:
						return BusinessDocumentLinesController.GetArticleType (quantity);

					case ColumnType.Date:
						return BusinessDocumentLinesController.GetArticleDate (quantity);
				}
			}

			if (info.QuantityIndex == 0)  // premère ligne ?
			{
				switch (columnType)
				{
					case ColumnType.ArticleId:
						return BusinessDocumentLinesController.GetArticleId (line);

					case ColumnType.ArticleDescription:
						return BusinessDocumentLinesController.GetArticleDescription (line);

					case ColumnType.Discount:
						return BusinessDocumentLinesController.GetArticleDiscount (line as ArticleDocumentItemEntity);

					case ColumnType.UnitPrice:
						return BusinessDocumentLinesController.GetArticleUnitPrice (line as ArticleDocumentItemEntity);

					case ColumnType.LinePrice:
						return BusinessDocumentLinesController.GetArticleLinePrice (line as ArticleDocumentItemEntity);

					case ColumnType.Vat:
						return BusinessDocumentLinesController.GetArticleVat (line as ArticleDocumentItemEntity);

					case ColumnType.Total:
						return BusinessDocumentLinesController.GetArticleTotal (line as ArticleDocumentItemEntity);
				}
			}
			else  // ligne suivante ?
			{
				switch (columnType)
				{
					case ColumnType.ArticleDescription:
						return "     \"";  // pour indiquer que c'est identique à la première ligne
				}
			}

			return null;
		}

		private bool CallbackSelectionChanged()
		{
			//	Appelé lorsque la sélection dans la liste a changé.
			if (this.articleLinesController.HasSingleSelection)
			{
				int? sel = this.articleLinesController.LastSelection;
				var info = this.articleLineInformations[sel.Value];

				this.editionArticleLineController.UpdateUI (info);
			}
			else
			{
				this.editionArticleLineController.UpdateUI (null);
			}

			return true;
		}


		private void Action(string commandName)
		{
			//	Câblage très provisoire des commandes !
			switch (commandName)
			{
				case "Lines.CreateArticle":
					this.ActionCreateArticle ();
					break;

				case "Lines.CreateText":
					this.ActionCreateText ();
					break;

				case "Lines.CreateTitle":
					this.ActionCreateTitle ();
					break;

				case "Lines.CreateDiscount":
					this.ActionCreateDiscount ();
					break;

				case "Lines.CreateTax":
					this.ActionCreateTax ();
					break;

				case "Lines.CreateQuantity":
					this.ActionCreateQuantity ();
					break;

				case "Lines.CreateGroup":
					this.ActionCreateGroup ();
					break;

				case "Lines.CreateGroupSeparator":
					this.ActionCreateGroupSeparator ();
					break;

				case "Lines.Duplicate":
					this.ActionDuplicate ();
					break;

				case "Lines.Delete":
					this.ActionDelete();
					break;

				case "Lines.Group":
					this.ActionGroup ();
					break;

				case "Lines.Ungroup":
					this.ActionUngroup ();
					break;

				case "Lines.Cancel":
					this.ActionCancel ();
					break;

				case "Lines.Ok":
					this.ActionOk ();
					break;

			}
		}

		private void ActionCreateArticle()
		{
			//	Insère une nouvelle ligne d'article.
			int? sel = this.articleLinesController.LastSelection;

			if (sel != null)
			{
				var info = this.articleLineInformations[sel.Value];

				var newLine = this.businessContext.CreateEntity<ArticleDocumentItemEntity> ();

				this.businessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

				this.UpdateAfterChange (newLine, null);
			}
		}

		private void ActionCreateText()
		{
			//	Insère une nouvelle ligne de texte.
			int? sel = this.articleLinesController.LastSelection;

			if (sel != null)
			{
				var info = this.articleLineInformations[sel.Value];

				var newLine = this.businessContext.CreateEntity<TextDocumentItemEntity> ();
				newLine.Text = "Coucou !!!";

				this.businessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

				this.UpdateAfterChange (newLine, null);
			}
		}

		private void ActionCreateTitle()
		{
			//	Insère une nouvelle ligne de titre.
			int? sel = this.articleLinesController.LastSelection;

			if (sel != null)
			{
				var info = this.articleLineInformations[sel.Value];

				var newLine = this.businessContext.CreateEntity<TextDocumentItemEntity> ();
				newLine.Text = "Titre !!!";

				this.businessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

				this.UpdateAfterChange (newLine, null);
			}
		}

		private void ActionCreateDiscount()
		{
			//	Insère une nouvelle ligne de rabais.
		}

		private void ActionCreateTax()
		{
			//	Insère une nouvelle ligne de taxe.
		}

		private void ActionCreateQuantity()
		{
			int? sel = this.articleLinesController.LastSelection;

			if (sel != null)
			{
				var info = this.articleLineInformations[sel.Value];
				var line = this.businessDocumentEntity.Lines.ElementAt (info.LineIndex);

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;
					var quantity = article.ArticleQuantities[info.QuantityIndex];

					var newQuantity = this.businessContext.CreateEntity<ArticleQuantityEntity> ();
					newQuantity.Quantity = 1;
					newQuantity.Unit = quantity.Unit;
					newQuantity.QuantityColumn = this.SearchArticleQuantityColumnEntity (ArticleQuantityType.Delayed);
					newQuantity.BeginDate = new Date (Date.Today.Ticks + Time.TicksPerDay*7);  // arbitrairement dans une semaine !

					article.ArticleQuantities.Add (newQuantity);

					this.UpdateAfterChange (line, newQuantity);
				}
			}
		}

		private void ActionCreateGroup()
		{
			//	Insère un nouveau groupe.
		}

		private void ActionCreateGroupSeparator()
		{
			//	Insère un nouveau groupe après le groupe en cours (donc au même niveau).
		}

		private void ActionDuplicate()
		{
			//	Duplique la ligne sélectionnée.
		}

		private void ActionDelete()
		{
			//	Supprime la ligne sélectionnée.
		}

		private void ActionGroup()
		{
			//	Groupe toutes les lignes sélectionnées.
		}

		private void ActionUngroup()
		{
			//	Défait le groupe sélectionné.
		}

		private void ActionCancel()
		{
			//	Annule la modification en cours.
		}

		private void ActionOk()
		{
			//	Valide la modification en cours.
		}


		private void UpdateAfterChange(AbstractDocumentItemEntity line, ArticleQuantityEntity quantity)
		{
			this.UpdateArticleLineInformations ();

			int? sel = this.GetArticleLineInformationsIndex (line, quantity);
			this.UpdateUI (sel);
		}

		private ArticleQuantityColumnEntity SearchArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return this.businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


#if false
		[Command (Library.Business.Res.Commands.Lines.CreateArticle)]
		public void ProcessCreateArticle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}
#endif


		private int? GetArticleLineInformationsIndex(AbstractDocumentItemEntity line, ArticleQuantityEntity quantity)
		{
			for (int i = 0; i < this.articleLineInformations.Count; i++)
			{
				var info = this.articleLineInformations[i];

				if (quantity == null)
				{
					if (info.AbstractDocumentItemEntity == line)
					{
						return i;
					}
				}
				else
				{
					if (info.AbstractDocumentItemEntity == line &&
						info.ArticleQuantityEntity == quantity)
					{
						return i;
					}
				}
			}

			return null;
		}

		private void UpdateArticleLineInformations()
		{
			this.articleLineInformations.Clear ();

			for (int i = 0; i < this.businessDocumentEntity.Lines.Count; i++)
			{
				var line = this.businessDocumentEntity.Lines[i];

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					for (int j = 0; j < article.ArticleQuantities.Count; j++)
					{
						var quantity = article.ArticleQuantities[j];

						this.articleLineInformations.Add (new ArticleLineInformations (line, quantity, i, j));
					}
				}
				else
				{
					this.articleLineInformations.Add (new ArticleLineInformations (line, null, i, 0));
				}
			}
		}


	
		#region AbstractDocumentItemEntity extensions
		// Toute cette partie de code doit être en accord avec Epsitec.Cresus.Core.EntityPrinters.DocumentMetadataPrinter, méthodes BuildXxxLine !
		// TODO: Un jour, il faudrait en extraire le code commun.

		private static FormattedText GetArticleQuantityAndUnit(ArticleQuantityEntity quantity)
		{
			return Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
		}

		private static FormattedText GetArticleType(ArticleQuantityEntity quantity)
		{
			return quantity.QuantityColumn.Name;
		}

		private static FormattedText GetArticleDate(ArticleQuantityEntity quantity)
		{
			return Misc.GetDateShortDescription (quantity.BeginDate, quantity.EndDate);
		}

		private static FormattedText GetArticleId(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				return ArticleDocumentItemHelper.GetArticleId (line);
			}

			return null;
		}

		private static FormattedText GetArticleDescription(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				return Helpers.ArticleDocumentItemHelper.GetArticleDescription (line, replaceTags: true, shortDescription: true);
			}

			if (item is TextDocumentItemEntity)
			{
				var line = item as TextDocumentItemEntity;

				return line.Text;
			}

			if (item is TaxDocumentItemEntity)
			{
				var line = item as TaxDocumentItemEntity;

				return FormattedText.Concat (line.Text, " (", Misc.PriceToString (line.BaseAmount), ")");
			}

			if (item is SubTotalDocumentItemEntity)
			{
				var line = item as SubTotalDocumentItemEntity;

				string discount = InvoiceDocumentHelper.GetAmount (line);

				if (line.Discount.DiscountRate.HasValue)
				{
					return FormattedText.Concat ("Rabais ", discount);  // Rabais 20.0%
				}
				else
				{
					return "Rabais";
				}
			}

			if (item is EndTotalDocumentItemEntity)
			{
				var line = item as EndTotalDocumentItemEntity;

				if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
				{
					return line.TextForPrice;
				}
				else if (line.FixedPriceAfterTax.HasValue)
				{
					return FormattedText.Join (FormattedText.HtmlBreak, line.TextForPrice, line.TextForFixedPrice);
				}
				else
				{
					return line.TextForPrice;
				}
			}

			return null;
		}

		private static string GetArticleDiscount(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				if (line.Discounts.Count != 0)
				{
					if (line.Discounts[0].DiscountRate.HasValue)
					{
						return Misc.PercentToString (line.Discounts[0].DiscountRate.Value);
					}

					if (line.Discounts[0].Value.HasValue)
					{
						return Misc.PriceToString (line.Discounts[0].Value.Value);
					}
				}
			}

			return null;
		}

		private static string GetArticleUnitPrice(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				return Misc.PriceToString (line.PrimaryUnitPriceBeforeTax);
			}

			return null;
		}

		private static string GetArticleLinePrice(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
				{
					return Misc.PriceToString (line.ResultingLinePriceBeforeTax);
				}
			}

			if (item is TaxDocumentItemEntity)
			{
				var line = item as TaxDocumentItemEntity;

				return Misc.PriceToString (line.ResultingTax);
			}

			if (item is SubTotalDocumentItemEntity)
			{
				var line = item as SubTotalDocumentItemEntity;

				string discount = InvoiceDocumentHelper.GetAmount (line);

				if (discount == null)
				{
					decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);
					return Misc.PriceToString (v1);
				}
				else
				{
					decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0);
					decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);

					string p1 = Misc.PriceToString (v1);
					string p2 = Misc.PriceToString (v3 - v1);
					string p3 = Misc.PriceToString (v3);

					return p2;
				}
			}

			if (item is EndTotalDocumentItemEntity)
			{
				var line = item as EndTotalDocumentItemEntity;

				if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
				{
					return Misc.PriceToString (line.PriceBeforeTax);
				}
			}

			return null;
		}

		private static string GetArticleVat(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
				{
					return Misc.PriceToString (line.ResultingLineTax1);
				}
			}

			if (item is SubTotalDocumentItemEntity)
			{
				var line = item as SubTotalDocumentItemEntity;

				string discount = InvoiceDocumentHelper.GetAmount (line);

				if (discount == null)
				{
					decimal v1 = line.ResultingTax.GetValueOrDefault (0);
					return Misc.PriceToString (v1);
				}
				else
				{
					decimal v1 = line.PrimaryTax.GetValueOrDefault (0);
					decimal v3 = line.ResultingTax.GetValueOrDefault (0);

					string p1 = Misc.PriceToString (v1);
					string p2 = Misc.PriceToString (v3 - v1);
					string p3 = Misc.PriceToString (v3);

					return p2;
				}
			}

			return null;
		}

		private static string GetArticleTotal(AbstractDocumentItemEntity item)
		{
			if (item is ArticleDocumentItemEntity)
			{
				var line = item as ArticleDocumentItemEntity;

				if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
				{
					return Misc.PriceToString (line.ResultingLinePriceBeforeTax + line.ResultingLineTax1);
				}
			}

			if (item is SubTotalDocumentItemEntity)
			{
				var line = item as SubTotalDocumentItemEntity;

				string discount = InvoiceDocumentHelper.GetAmount (line);

				if (discount == null)
				{
					decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);
					return Misc.PriceToString (v1);
				}
				else
				{
					decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0) + line.PrimaryTax.GetValueOrDefault (0);
					decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);

					string p1 = Misc.PriceToString (v1);
					string p2 = Misc.PriceToString (v3 - v1);
					string p3 = Misc.PriceToString (v3);

					return p2;
				}
			}

			if (item is EndTotalDocumentItemEntity)
			{
				var line = item as EndTotalDocumentItemEntity;

				if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
				{
				}
				else if (line.FixedPriceAfterTax.HasValue)
				{
					return Misc.PriceToString (line.FixedPriceAfterTax);
				}
				else
				{
					return Misc.PriceToString (line.PriceAfterTax);
				}
			}

			return null;
		}
		#endregion


		private readonly BusinessContext				businessContext;
		private readonly DataContext					dataContext;
		private readonly CoreData						coreData;
		private readonly DocumentMetadataEntity			documentMetadataEntity;
		private readonly BusinessDocumentEntity			businessDocumentEntity;
		private readonly List<ArticleLineInformations>	articleLineInformations;

		private ArticleLineToolbarController			articleLineToolbarController;
		private ArticleLinesController					articleLinesController;
		private ArticleLineEditorController				editionArticleLineController;
	}
}

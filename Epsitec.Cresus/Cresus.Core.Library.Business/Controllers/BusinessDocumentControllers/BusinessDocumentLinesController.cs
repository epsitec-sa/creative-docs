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
		public BusinessDocumentLinesController(AccessData accessData)
		{
			this.accessData = accessData;

			this.commandContext = new CommandContext ("BusinessDocumentLinesController");
			this.commandDispatcher = new CommandDispatcher ("BusinessDocumentLinesController", CommandDispatcherLevel.Secondary);
			this.commandDispatcher.RegisterController (this);

			this.lineInformations = new List<LineInformations> ();
			this.UpdateArticleLineInformations ();
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			CommandContext.SetContext (frame, this.commandContext);
			CommandDispatcher.SetDispatcher (frame, this.commandDispatcher);

			this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.CreateArticle).Enable = false;  // exemple !!!

			//	Crée la toolbar.
			//?this.lineToolbarController = new LineToolbarController (this.accessData.DocumentMetadataEntity, this.accessData.BusinessDocumentEntity);
			//?this.lineToolbarController.CreateUI (frame, this.Action);

			//	Crée la liste.
			this.linesController = new LinesController (this.accessData);
			this.linesController.CreateUI (frame, this.CallbackSelectionChanged);

			//	Crée l'éditeur pour une ligne.
			this.lineEditorController = new LineEditorController (this.accessData);
			this.lineEditorController.CreateUI (frame);
		}

		public void UpdateUI(int? sel = null)
		{
			this.linesController.UpdateUI (this.lineInformations.Count, this.CallbackGetCellContent, sel);
		}


		private FormattedText CallbackGetCellContent(int index, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var info = this.lineInformations[index];

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
			if (this.linesController.HasSingleSelection)
			{
				int? sel = this.linesController.LastSelection;
				var info = this.lineInformations[sel.Value];

				this.lineEditorController.UpdateUI (info);
			}
			else
			{
				this.lineEditorController.UpdateUI (null);
			}

			return true;
		}


		[Command (Library.Business.Res.CommandIds.Lines.CreateArticle)]
		public void ProcessCreateArticle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateArticle ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateText)]
		public void ProcessCreateText(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateText ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTitle)]
		public void ProcessCreateTitle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateTitle ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateDiscount)]
		public void ProcessCreateDiscount(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateDiscount ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTax)]
		public void ProcessCreateTax(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateTax ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateQuantity)]
		public void ProcessCreateQuantity(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateQuantity ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateGroup)]
		public void ProcessCreateGroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateGroup ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateGroupSeparator)]
		public void ProcessCreateGroupSeparator(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCreateGroupSeparator ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Duplicate)]
		public void ProcessDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionDuplicate ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Delete)]
		public void ProcessDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionDelete ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Group)]
		public void ProcessGroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionGroup ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Ungroup)]
		public void ProcessUngroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionUngroup ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Ok)]
		public void ProcessOk(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionOk ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.Cancel)]
		public void ProcessCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ActionCancel ();
		}

		private void Action(string commandName)
		{
			//	Câblage très provisoire des commandes !
#if false
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
#endif
		}

		private void ActionCreateArticle()
		{
			//	Insère une nouvelle ligne d'article.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];

				var newLine = this.accessData.BusinessContext.CreateEntity<ArticleDocumentItemEntity> ();

				this.accessData.BusinessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

				this.UpdateAfterChange (newLine, null);
			}
		}

		private void ActionCreateText()
		{
			//	Insère une nouvelle ligne de texte.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];

				var newLine = this.accessData.BusinessContext.CreateEntity<TextDocumentItemEntity> ();
				newLine.Text = "Coucou !!!";
				newLine.GroupLevel = 1;

				this.accessData.BusinessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

				this.UpdateAfterChange (newLine, null);
			}
		}

		private void ActionCreateTitle()
		{
			//	Insère une nouvelle ligne de titre.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];

				var newLine = this.accessData.BusinessContext.CreateEntity<TextDocumentItemEntity> ();
				newLine.Text = "Titre !!!";

				this.accessData.BusinessDocumentEntity.Lines.Insert (info.LineIndex+1, newLine);

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
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];
				var line = this.accessData.BusinessDocumentEntity.Lines.ElementAt (info.LineIndex);

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;
					var quantity = article.ArticleQuantities[info.QuantityIndex];

					var newQuantity = this.accessData.BusinessContext.CreateEntity<ArticleQuantityEntity> ();
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

			return this.accessData.BusinessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


		private int? GetArticleLineInformationsIndex(AbstractDocumentItemEntity line, ArticleQuantityEntity quantity)
		{
			for (int i = 0; i < this.lineInformations.Count; i++)
			{
				var info = this.lineInformations[i];

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
			this.lineInformations.Clear ();

			for (int i = 0; i < this.accessData.BusinessDocumentEntity.Lines.Count; i++)
			{
				var line = this.accessData.BusinessDocumentEntity.Lines[i];

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					for (int j = 0; j < article.ArticleQuantities.Count; j++)
					{
						var quantity = article.ArticleQuantities[j];

						this.lineInformations.Add (new LineInformations (line, quantity, i, j));
					}
				}
				else
				{
					this.lineInformations.Add (new LineInformations (line, null, i, 0));
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


		private readonly AccessData						accessData;
		private readonly List<LineInformations>			lineInformations;
		private readonly CommandContext					commandContext;
		private readonly CommandDispatcher				commandDispatcher;

		private LineToolbarController					lineToolbarController;
		private LinesController							linesController;
		private LineEditorController					lineEditorController;
	}
}

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

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class BusinessDocumentLinesController
	{
		public BusinessDocumentLinesController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;

			this.articleLineInformations = new List<ArticleLineInformations> ();
			this.UpdateArticleLineInformations ();
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


		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			//	Crée la toolbar.
			this.articleLineToolbarController = new ArticleLineToolbarController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.articleLineToolbarController.CreateUI (frame);

			//	Crée la liste.
			this.articleLinesController = new ArticleLinesController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.articleLinesController.CreateUI (frame);

			//	Crée l'éditeur pour une ligne.
			this.editionArticleLineController = new ArticleLineEditorController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.editionArticleLineController.CreateUI (frame);
		}

		public void UpdateUI(int? sel = null)
		{
			this.articleLinesController.UpdateUI (this.articleLineInformations.Count, this.GetCellContent, sel);
		}

		private FormattedText GetCellContent(int index, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var info = this.articleLineInformations[index];

			var line     = info.AbstractDocumentItemEntity;
			var quantity = info.ArticleQuantityEntity;

			if (quantity != null)
			{
				if (columnType == ColumnType.Quantity)
				{
					var value = BusinessDocumentLinesController.GetArticleQuantity (quantity);

					if (value != null)
					{
						return value.ToString ();
					}
				}

				if (columnType == ColumnType.Unit)
				{
					return BusinessDocumentLinesController.GetArticleUnit (quantity);
				}

				if (columnType == ColumnType.Type)
				{
					return BusinessDocumentLinesController.GetArticleType (quantity);
				}
			}

			if (info.QuantityIndex == 0)
			{
				if (columnType == ColumnType.Description)
				{
					return BusinessDocumentLinesController.GetArticleDescription (line);
				}

				if (columnType == ColumnType.Price)
				{
					var price = BusinessDocumentLinesController.GetArticlePrice (line as ArticleDocumentItemEntity);

					if (price != null)
					{
						return Misc.PriceToString (price);
					}
				}
			}
			else
			{
				return "     \"";  // pour indiquer que c'est identique à la première ligne
			}

			return null;
		}


#if false
		[Command (Library.Business.Res.Commands.Lines.CreateArticle)]
		public void ProcessCreateArticle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}
#endif

	
		#region ArticleDocumentItemEntity extensions
		private static decimal? GetArticleQuantity(ArticleQuantityEntity quantity)
		{
			return quantity.Quantity;
		}

		private static FormattedText GetArticleUnit(ArticleQuantityEntity quantity)
		{
			return quantity.Unit.Name;
		}

		private static FormattedText GetArticleType(ArticleQuantityEntity quantity)
		{
			return quantity.QuantityColumn.Name;
		}

		private static FormattedText GetArticleDescription(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return Helpers.ArticleDocumentItemHelper.GetArticleDescription (article, replaceTags: true, shortDescription: true);
			}

			if (line is TextDocumentItemEntity)
			{
				var text = line as TextDocumentItemEntity;
				return text.Text;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;

				if (tax.Text.IsNullOrEmpty)
				{
					return "TVA";
				}
				else
				{
					return tax.Text;
				}
			}

			if (line is SubTotalDocumentItemEntity)
			{
				return "Sous-total";
			}

			if (line is EndTotalDocumentItemEntity)
			{
				return "Grand total";
			}

			return null;
		}

		private static decimal? GetArticlePrice(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return article.PrimaryLinePriceBeforeTax;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;
				return tax.ResultingTax;
			}

			if (line is SubTotalDocumentItemEntity)
			{
				var total = line as SubTotalDocumentItemEntity;
				return total.FinalPriceBeforeTax;
			}

			if (line is EndTotalDocumentItemEntity)
			{
				var total = line as EndTotalDocumentItemEntity;
				return total.PriceAfterTax;
			}

			return null;
		}
		#endregion


		private static readonly double lineHeight = 17;

		private readonly DocumentMetadataEntity			documentMetadataEntity;
		private readonly BusinessDocumentEntity			businessDocumentEntity;
		private readonly List<ArticleLineInformations>	articleLineInformations;

		private ArticleLineToolbarController			articleLineToolbarController;
		private ArticleLinesController					articleLinesController;
		private ArticleLineEditorController				editionArticleLineController;
	}
}

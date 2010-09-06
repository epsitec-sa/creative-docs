//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class ArticleDocumentItemHelper
	{
		public static decimal? GetArticleVatRate(GenericArticleDocumentEntity document, ArticleDocumentItemEntity article)
		{
			//	Retourne le taux de tva auquel un article est soumi.
			// TODO: Il faudra probablement modifier cela pour chercher les informations dans une table !
			// TODO: Il devrait aussi y avoir une date en entrée !
			switch (article.ArticleDefinition.ArticleCategory.DefaultOutputVatCode)
			{
				case BusinessLogic.Finance.VatCode.Excluded:
				case BusinessLogic.Finance.VatCode.ZeroRated:
					return 0;

				case BusinessLogic.Finance.VatCode.StandardTax:
				case BusinessLogic.Finance.VatCode.StandardInputTaxOnInvestementOrOperatingExpenses:
				case BusinessLogic.Finance.VatCode.StandardInputTaxOnMaterialOrServiceExpenses:
				case BusinessLogic.Finance.VatCode.StandardTaxOnTurnover:
					return 0.076M;

				case BusinessLogic.Finance.VatCode.ReducedTax:
				case BusinessLogic.Finance.VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses:
				case BusinessLogic.Finance.VatCode.ReducedInputTaxOnMaterialOrServiceExpenses:
				case BusinessLogic.Finance.VatCode.ReducedTaxOnTurnover:
					return 0.024M;

				case BusinessLogic.Finance.VatCode.SpecialTax:
				case BusinessLogic.Finance.VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses:
				case BusinessLogic.Finance.VatCode.SpecialInputTaxOnMaterialOrServiceExpenses:
				case BusinessLogic.Finance.VatCode.SpecialTaxOnTurnover:
					return 0.036M;
			}

			return null;
		}


		public static decimal? GetArticlePrice(ArticleDocumentItemEntity article, System.DateTime date, BusinessLogic.Finance.CurrencyCode currency)
		{
			//	Il peut y avoir plusieurs prix, mais un seul prix à une date donnée pour une monnaie donnée.
			foreach (var price in article.ArticleDefinition.ArticlePrices)
			{
				if (price.BeginDate.HasValue && price.EndDate.HasValue)
				{
					if (date >= price.BeginDate && date <= price.EndDate && currency == price.CurrencyCode)
					{
						return price.ValueBeforeTax;
					}
				}
			}

			return null;
		}

		public static string GetArticleQuantityAndUnit(ArticleDocumentItemEntity article)
		{
			//	Retourne la quantité d'un article et l'unité correspondante. Si l'article n'est pas livrable,
			//	retourne zéro (par exemple "0 pce").
			string unit = null;

			foreach (var quantity in article.ArticleQuantities)
			{
				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Billed)
				{
					return Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
				}

				unit = quantity.Unit.Code;
			}

			if (unit != null)
			{
				return Misc.FormatUnit (0, unit);
			}

			return null;
		}

		public static decimal? GetArticleQuantity(ArticleDocumentItemEntity article)
		{
			foreach (var quantity in article.ArticleQuantities)
			{
				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Billed)
				{
					return quantity.Quantity;
				}
			}

			return null;
		}

		public static string GetArticleId(ArticleDocumentItemEntity article)
		{
			var x = article.ArticleDefinition;

			return TextFormatter.FormatText (x.IdA, "/~", x.IdB, "/~", x.IdC).ToSimpleText ();
		}

		public static FormattedText GetArticleDescription(ArticleDocumentItemEntity article, bool replaceTags=false, bool shortDescription=false)
		{
			FormattedText description = FormattedText.Null;

			if (shortDescription)  // description courte prioritaire ?
			{
				if (!article.ReplacementText.IsNullOrEmpty)
				{
					description = article.ReplacementText;
				}
				else if (!article.ArticleDefinition.ShortDescription.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.ShortDescription;
				}
				else if (!article.ArticleDefinition.LongDescription.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.LongDescription;
				}
			}
			else  // description longue prioritaire ?
			{
				if (!article.ReplacementText.IsNullOrEmpty)
				{
					description = article.ReplacementText;
				}
				else if (!article.ArticleDefinition.LongDescription.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.LongDescription;
				}
				else if (!article.ArticleDefinition.ShortDescription.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.ShortDescription;
				}
			}

			description = TextFormatter.FormatText (description);  // enlève les balises <div>, selon la langue

			if (replaceTags)
			{
				description = ArticleParameterHelper.ArticleDescriptionReplaceTags (article, description);
			}

			return description;
		}


		public static bool IsFixedTax(ArticleDocumentItemEntity article)
		{
			//	Retourne true s'il s'agit d'un article de taxe (par exemple des frais de port).
			if (article != null &&
				article.ArticleDefinition.IsActive () &&
				article.ArticleDefinition.ArticleCategory.IsActive ())
			{
				return article.ArticleDefinition.ArticleCategory.ArticleType == BusinessLogic.ArticleType.Freight ||
					   article.ArticleDefinition.ArticleCategory.ArticleType == BusinessLogic.ArticleType.Tax;
			}

			return false;
		}

		public static bool IsArticleForBL(ArticleDocumentItemEntity article)
		{
			//	Retourne true s'il s'agit d'un article qui doit figurer sur un BL.
			if (article != null &&
				article.ArticleDefinition.IsActive () &&
				article.ArticleDefinition.ArticleCategory.IsActive ())
			{
				return article.ArticleDefinition.ArticleCategory.ArticleType == BusinessLogic.ArticleType.Goods;  // marchandises ?
			}

			return false;
		}

		public static bool IsArticleForProd(ArticleDocumentItemEntity article, ArticleGroupEntity group)
		{
			//	Retourne true s'il s'agit d'un article qui doit figurer sur un ordre de production.
			if (article != null &&
				article.ArticleDefinition.IsActive () &&
				article.ArticleDefinition.ArticleCategory.IsActive ())
			{
				if (!article.ArticleDefinition.ArticleGroups.Contains (group))
				{
					return false;
				}

				return article.ArticleDefinition.ArticleCategory.ArticleType == BusinessLogic.ArticleType.Goods;  // marchandises ?
			}

			return false;
		}


		public static void UpdatePrices(GenericArticleDocumentEntity document, ArticleDocumentItemEntity article)
		{
			//	Recalcule une ligne d'une facture.
			var vatRate  = ArticleDocumentItemHelper.GetArticleVatRate (document, article).GetValueOrDefault (0);
			var quantity = ArticleDocumentItemHelper.GetArticleQuantity (article);

			if (quantity.HasValue)
			{
				decimal total = Misc.PriceConstrain (article.PrimaryUnitPriceBeforeTax * quantity.Value);

				if (article.NeverApplyDiscount == false &&
					article.Discounts.Count != 0)  // y a-t-il un rabais de ligne ?
				{
					if (article.Discounts[0].DiscountRate.HasValue)  // rabais en % ?
					{
						total = Misc.PriceConstrain (total * (1.0M - article.Discounts[0].DiscountRate.Value));
					}
					else if (article.Discounts[0].DiscountAmount.HasValue)  // rabais en francs ?
					{
						total = Misc.PriceConstrain (total - article.Discounts[0].DiscountAmount.Value);
					}
				}

				article.PrimaryLinePriceBeforeTax   = total;
				article.ResultingLinePriceBeforeTax = (int) total;  // arrondi au franc inférieur, pourquoi pas ?
				article.ResultingLineTax            = /* Misc.PriceConstrain */ (article.ResultingLinePriceBeforeTax.Value * vatRate);
			}
			else
			{
				article.PrimaryLinePriceBeforeTax   = 0;
				article.ResultingLinePriceBeforeTax = null;
				article.ResultingLineTax            = null;
			}
		}
	}
}

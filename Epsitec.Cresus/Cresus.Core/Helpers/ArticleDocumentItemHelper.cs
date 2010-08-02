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
				if (quantity.Code == "livré")
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
				if (quantity.Code == "livré")
				{
					return quantity.Quantity;
				}
			}

			return null;
		}

		public static string GetArticleId(ArticleDocumentItemEntity article)
		{
			var x = article.ArticleDefinition;

			return UIBuilder.FormatText (x.IdA, "/~", x.IdB, "/~", x.IdC).ToSimpleText ();
		}

		public static string GetArticleDescription(ArticleDocumentItemEntity article)
		{
			if (!string.IsNullOrEmpty (article.ReplacementText))
			{
				return article.ReplacementText;
			}

			if (!string.IsNullOrEmpty (article.ArticleDefinition.LongDescription))
			{
				return article.ArticleDefinition.LongDescription;
			}

			if (!string.IsNullOrEmpty (article.ArticleDefinition.ShortDescription))
			{
				return article.ArticleDefinition.ShortDescription;
			}

			return null;
		}


		public static bool IsFixedTax(ArticleDocumentItemEntity article)
		{
			//	Retourne true s'il s'agit d'un article de type 'frais de port'.
			if (article != null &&
				article.ArticleDefinition.IsActive () &&
				article.ArticleDefinition.ArticleCategory.IsActive ())
			{
				// TODO: Peut-être faudra-t-il faire cela autrement...
				return article.ArticleDefinition.ArticleCategory.Name == "Ports/emballages";
			}

			return false;
		}


		public static void UpdatePrices(ArticleDocumentItemEntity article)
		{
			//	Recalcule une ligne d'une facture.
			decimal vatRate = 0.076M;  // TODO: Cette valeur ne devrait pas tomber du ciel !
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

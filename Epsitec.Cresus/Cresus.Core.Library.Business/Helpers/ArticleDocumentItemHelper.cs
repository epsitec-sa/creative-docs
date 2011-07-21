//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Helpers
{
	public static class ArticleDocumentItemHelper
	{
		public static decimal? GetArticleVatRate(BusinessDocumentEntity document, ArticleDocumentItemEntity article)
		{
			//	Retourne le taux de tva auquel un article est soumi.
			// TODO: Il faudra probablement modifier cela pour chercher les informations dans une table !
			// TODO: Il devrait aussi y avoir une date en entrée !
			switch (article.ArticleDefinition.ArticleCategory.DefaultOutputVatCode)
			{
				case Business.Finance.VatCode.Excluded:
				case Business.Finance.VatCode.ZeroRated:
					return 0;

				case Business.Finance.VatCode.StandardTax:
				case Business.Finance.VatCode.StandardInputTaxOnInvestementOrOperatingExpenses:
				case Business.Finance.VatCode.StandardInputTaxOnMaterialOrServiceExpenses:
				case Business.Finance.VatCode.StandardTaxOnTurnover:
					return 0.076M;

				case Business.Finance.VatCode.ReducedTax:
				case Business.Finance.VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses:
				case Business.Finance.VatCode.ReducedInputTaxOnMaterialOrServiceExpenses:
				case Business.Finance.VatCode.ReducedTaxOnTurnover:
					return 0.024M;

				case Business.Finance.VatCode.SpecialTax:
				case Business.Finance.VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses:
				case Business.Finance.VatCode.SpecialInputTaxOnMaterialOrServiceExpenses:
				case Business.Finance.VatCode.SpecialTaxOnTurnover:
					return 0.036M;
			}

			return null;
		}


		public static decimal? GetArticlePrice(ArticleDocumentItemEntity article, System.DateTime date, Business.Finance.CurrencyCode currency)
		{
			//	Il peut y avoir plusieurs prix, mais un seul prix à une date donnée pour une monnaie donnée.
			foreach (var price in article.ArticleDefinition.ArticlePrices.Where (price => price.CurrencyCode == currency))
			{
				if (date.InRange (price))
				{
					return price.Value;
				}
			}

			return null;
		}

		public static string GetArticleQuantityAndUnit(ArticleDocumentItemEntity article)
		{
			//	Retourne la quantité d'un article et l'unité correspondante.
			string unit = null;

			foreach (var quantity in article.ArticleQuantities)
			{
				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Ordered)
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

		public static string GetArticleId(ArticleDocumentItemEntity article)
		{
			var x = article.ArticleDefinition;

			return TextFormatter.FormatText (x.IdA, "/~", x.IdB, "/~", x.IdC).ToSimpleText ();
		}

		public static FormattedText GetArticleText(ArticleDocumentItemEntity article, bool replaceTags=false, bool shortDescription=false)
		{
			//	Retourne la désignation courte ou longue d'un article.
			FormattedText description = FormattedText.Null;

			if (shortDescription)  // description courte ?
			{
				string shortReplacementTextInCurrentLanguage = TextFormatter.ConvertToText (article.ReplacementName);

				if (!string.IsNullOrEmpty (shortReplacementTextInCurrentLanguage))
				{
					description = article.ReplacementName;
				}
				else if (!article.ArticleDefinition.Name.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Name;
				}
			}
			else  // description longue ?
			{
				string longReplacementTextInCurrentLanguage  = TextFormatter.ConvertToText (article.ReplacementDescription);

				if (!string.IsNullOrEmpty (longReplacementTextInCurrentLanguage))
				{
					description = article.ReplacementDescription;
				}
				else if (!article.ArticleDefinition.Description.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Description;
				}
			}

			description = TextFormatter.FormatText (description);  // enlève les balises <div>, selon la langue

			if (replaceTags)
			{
				description = ArticleParameterHelper.ArticleDescriptionReplaceTags (article, description);
			}

			return description;
		}


		public static bool IsFixedTax(ArticleDefinitionEntity article)
		{
			//	Retourne true s'il s'agit d'un article de taxe (par exemple des frais de port).
			if (article != null &&
				article.IsNotNull () &&
				article.ArticleCategory.IsNotNull ())
			{
				return article.ArticleCategory.ArticleType == Business.ArticleType.Freight ||
					   article.ArticleCategory.ArticleType == Business.ArticleType.Tax ||
					   article.ArticleCategory.ArticleType == Business.ArticleType.Admin;
			}

			return false;
		}
	}
}

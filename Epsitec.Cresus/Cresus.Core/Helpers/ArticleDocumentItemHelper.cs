﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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
			//	Retourne la quantité d'un article et l'unité correspondante. Si l'article n'est pas livrable,
			//	retourne zéro (par exemple "0 pce").
			string unit = null;

			foreach (var quantity in article.ArticleQuantities)
			{
				if (quantity.QuantityType == Business.ArticleQuantityType.Billed)
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
				if (quantity.QuantityType == Business.ArticleQuantityType.Billed)
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

			string replacementTextInCurrentLanguage = TextFormatter.ConvertToText (article.ReplacementText);

			if (shortDescription)  // description courte prioritaire ?
			{
				if (!string.IsNullOrEmpty (replacementTextInCurrentLanguage))
				{
					description = article.ReplacementText;
				}
				else if (!article.ArticleDefinition.Name.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Name;
				}
				else if (!article.ArticleDefinition.Description.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Description;
				}
			}
			else  // description longue prioritaire ?
			{
				if (!string.IsNullOrEmpty (replacementTextInCurrentLanguage))
				{
					description = article.ReplacementText;
				}
				else if (!article.ArticleDefinition.Description.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Description;
				}
				else if (!article.ArticleDefinition.Name.IsNullOrEmpty)
				{
					description = article.ArticleDefinition.Name;
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
				article.ArticleDefinition.IsNotNull () &&
				article.ArticleDefinition.ArticleCategory.IsNotNull ())
			{
				return article.ArticleDefinition.ArticleCategory.ArticleType == Business.ArticleType.Freight ||
					   article.ArticleDefinition.ArticleCategory.ArticleType == Business.ArticleType.Tax;
			}

			return false;
		}

		public static bool IsArticleForBL(ArticleDocumentItemEntity article)
		{
			//	Retourne true s'il s'agit d'un article qui doit figurer sur un BL.
			if (article != null &&
				article.ArticleDefinition.IsNotNull () &&
				article.ArticleDefinition.ArticleCategory.IsNotNull ())
			{
				return article.ArticleDefinition.ArticleCategory.ArticleType == Business.ArticleType.Goods;  // marchandises ?
			}

			return false;
		}

		public static bool IsArticleForProd(ArticleDocumentItemEntity article, ArticleGroupEntity group)
		{
			//	Retourne true s'il s'agit d'un article qui doit figurer sur un ordre de production.
			if (article != null &&
				article.ArticleDefinition.IsNotNull () &&
				article.ArticleDefinition.ArticleCategory.IsNotNull ())
			{
				if (!article.ArticleDefinition.ArticleGroups.Contains (group))
				{
					return false;
				}

				return article.ArticleDefinition.ArticleCategory.ArticleType == Business.ArticleType.Goods;  // marchandises ?
			}

			return false;
		}


		public static void UpdatePrices(BusinessDocumentEntity document, ArticleDocumentItemEntity article)
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
					else if (article.Discounts[0].Value.HasValue)  // rabais en francs ?
					{
						total = Misc.PriceConstrain (total - article.Discounts[0].Value.Value);
					}
				}

				article.PrimaryLinePriceBeforeTax   = total;
				article.ResultingLinePriceBeforeTax = (int) total;  // arrondi au franc inférieur, pourquoi pas ?
				article.ResultingLineTax1           = /* Misc.PriceConstrain */ (article.ResultingLinePriceBeforeTax.Value * vatRate);
			}
			else
			{
				article.PrimaryLinePriceBeforeTax   = 0;
				article.ResultingLinePriceBeforeTax = null;
				article.ResultingLineTax1           = null;
			}
		}


		public static IDictionary<string, IList<object>> GetParameterCodesToValues(ArticleDocumentItemEntity articleItem)
		{
			articleItem.ThrowIfNull ("articleItem");

			var parameterDefinitions = articleItem.ArticleDefinition.ArticleParameterDefinitions;
			var parameterStringValues = ArticleParameterHelper.GetArticleParametersValues (articleItem, returnName: false);
			var parameterCodesToValues = new Dictionary<string, IList<object>> ();

			foreach (var parameterDefinition in parameterDefinitions)
			{
				string parameterCode = parameterDefinition.Code;

				if (!parameterStringValues.ContainsKey (parameterCode))
				{
					parameterStringValues[parameterCode] = "";
				}

				string parameterStringValue= parameterStringValues[parameterCode];

				IList<object> parameterObjectValues = ArticleDocumentItemHelper.GetParameterObjectValues (parameterDefinition, parameterStringValue);

				if (parameterObjectValues.Any ())
				{
					parameterCodesToValues[parameterCode] = parameterObjectValues;
				}
			}

			return parameterCodesToValues;
		}


		private static IList<object> GetParameterObjectValues(AbstractArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			var numericParameterDefinition = parameterDefinition as NumericValueArticleParameterDefinitionEntity;
			var enumParameterDefinition = parameterDefinition as EnumValueArticleParameterDefinitionEntity;
			var freeTextParameterDefinition = parameterDefinition as FreeTextValueArticleParameterDefinitionEntity;

			if (numericParameterDefinition != null)
			{
				return ArticleDocumentItemHelper.GetNumericParameterObjectValues (numericParameterDefinition, parameterStringValue);
			}
			else if (enumParameterDefinition != null)
			{
				return ArticleDocumentItemHelper.GetEnumParameterObjectValues (enumParameterDefinition, parameterStringValue);
			}
			else if (freeTextParameterDefinition != null)
			{
				return ArticleDocumentItemHelper.GetFreeTextParameterObjectValues (freeTextParameterDefinition, parameterStringValue);
			}
			else
			{
				throw new System.NotImplementedException ();
			}
		}


		private static IList<object> GetNumericParameterObjectValues(NumericValueArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			decimal value = InvariantConverter.ConvertFromString<decimal> (parameterStringValue);

			return new List<object> { value };
		}


		private static IList<object> GetEnumParameterObjectValues(EnumValueArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			List<object> parameterObjectValues = AbstractArticleParameterDefinitionEntity.Split (parameterStringValue).ToList<object> ();

			if (!ArticleDocumentItemHelper.CheckNbValuesForCardinality (parameterDefinition.Cardinality, parameterObjectValues.Count))
			{
				throw new System.Exception ("Invalid number of values for parameter");
			}

			return parameterObjectValues;
		}


		private static bool CheckNbValuesForCardinality(EnumValueCardinality cardinality, int nbValues)
		{
			switch (cardinality)
			{
				case EnumValueCardinality.ExactlyOne:
					return (nbValues == 1);

				case EnumValueCardinality.ZeroOrOne:
					return (nbValues <= 1);

				case EnumValueCardinality.Any:
					return true;

				case EnumValueCardinality.AtLeastOne:
					return (nbValues >= 1);

				default:
					throw new System.NotImplementedException ();
			}
		}


		private static IList<object> GetFreeTextParameterObjectValues(FreeTextValueArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			return new List<object> () { parameterStringValue };
		}


	}
}

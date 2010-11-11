//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class PriceCalculator
	{
		public PriceCalculator(BusinessDocumentEntity document, ArticleDocumentItemEntity articleItem)
		{
			this.document     = document;
			this.articleItem  = articleItem;
			this.articleDef   = this.articleItem.ArticleDefinition;
			this.currencyCode = this.document.BillingCurrencyCode;
			this.date         = this.document.PriceRefDate.GetValueOrDefault (Date.Today).ToDateTime ();
		}


		public void UpdateResultingLinePrice()
		{
			var roundingPolicy = this.priceRoundingMode.PriceRoundingPolicy;
			var quantity       = this.GetTotalQuantity ();
			var articlePrice   = this.GetArticlePrices (quantity).FirstOrDefault ();
			
			decimal unitPriceBeforeTax = articlePrice.ValueBeforeTax;
			decimal unitPriceAfterTax  = 0;

			if (roundingPolicy == RoundingPolicy.OnUnitPriceBeforeTax)
			{
				unitPriceBeforeTax = this.priceRoundingMode.Round (unitPriceBeforeTax);
			}

			TaxRateAmount[] taxRateAmounts = this.GetTaxRateAmounts (unitPriceBeforeTax);

			unitPriceAfterTax = taxRateAmounts.Sum (x => x.Amount);

			if (roundingPolicy == RoundingPolicy.OnUnitPriceAfterTax)
			{
				unitPriceAfterTax = this.priceRoundingMode.Round (unitPriceAfterTax);
			}

			//	.........
			
			if (taxRateAmounts.Length > 2)
			{
				throw new System.InvalidOperationException ("Cannot process more than 2 different tax rates");
			}
			
			this.articleItem.PrimaryUnitPriceBeforeTax = unitPriceBeforeTax;
			this.articleItem.PrimaryLinePriceBeforeTax = unitPriceBeforeTax * quantity;
		}

		public decimal GetTotalQuantity()
		{
			InclusionMode inclusionMode = this.IsRealBill () ? InclusionMode.Billed : InclusionMode.Ordered;
			return this.articleItem.ArticleQuantities.Sum (x => this.GetQuantity (x, inclusionMode));
		}

		private enum InclusionMode
		{
			All,
			Billed,
			Ordered,
		}

		private TaxRateAmount[] GetTaxRateAmounts(decimal articleValue)
		{
			TaxCalculator taxCalculator;
			TaxRateAmount[] taxRateAmounts;

			if ((this.articleItem.BeginDate.HasValue) &&
						(this.articleItem.EndDate.HasValue))
			{
				taxCalculator = new TaxCalculator (this.articleItem);
			}
			else
			{
				taxCalculator = new TaxCalculator (new Date (this.date));
			}

			taxRateAmounts = taxCalculator.GetTaxes (articleValue, this.articleItem.VatCode);
			return taxRateAmounts;
		}
		private decimal GetQuantity(ArticleQuantityEntity quantity, InclusionMode inclusionMode)
		{
			switch (quantity.QuantityType)
			{
				case ArticleQuantityType.Billed:
					return this.ConvertToBillingUnit (quantity.Quantity, quantity.Unit);

				case ArticleQuantityType.Delayed:
				case ArticleQuantityType.Ordered:
					return (inclusionMode == InclusionMode.Billed) ? 0M : this.ConvertToBillingUnit (quantity.Quantity, quantity.Unit);

				case ArticleQuantityType.Information:
					return (inclusionMode != InclusionMode.All) ? 0M : this.ConvertToBillingUnit (quantity.Quantity, quantity.Unit);
				
				case ArticleQuantityType.None:
					return 0M;

				default:
					throw new System.NotSupportedException (string.Format ("ArticleQuantityType.{0} not supported", quantity.QuantityType));
			}
		}

		private bool IsRealBill()
		{
			switch (this.document.BillingStatus)
			{
				case BillingStatus.CreditorBillOpen:
				case BillingStatus.CreditorBillClosed:
				case BillingStatus.DebtorBillOpen:
				case BillingStatus.DebtorBillClosed:
					return true;

				case BillingStatus.None:
				case BillingStatus.NotAnInvoice:
					return false;

				default:
					throw new System.NotSupportedException (string.Format ("BillingStatus.{0} not supported", this.document.BillingStatus));
			}
		}

		private decimal ConvertToBillingUnit(decimal value, UnitOfMeasureEntity unit)
		{
			//	TODO: convertir en unités de facturation selon ArticleDefinition.BillingUnit
			return value;
		}

		public IEnumerable<ArticlePriceEntity> GetArticlePrices(decimal quantity)
		{
			var prices = from price in this.articleDef.ArticlePrices
						 where price.CurrencyCode == this.currencyCode
						 where this.date.InRange (price)
						 where quantity.InRange (price.MinQuantity, price.MaxQuantity)
						 select price;

			return prices;
		}

		public IEnumerable<ArticleAccountingDefinitionEntity> GetArticleAccountingDefinition()
		{
			var accounting = (from def in this.articleDef.Accounting
							  where def.CurrencyCode == this.currencyCode
							  where this.date.InRange (def)
							  select def).ToArray ();

			if (accounting.Length > 0)
			{
				return accounting;
			}

			if (this.articleDef.ArticleCategory.IsNull ())
			{
				return EmptyEnumerable<ArticleAccountingDefinitionEntity>.Instance;
			}

			return from def in this.articleDef.ArticleCategory.DefaultAccounting
				   where def.CurrencyCode == this.currencyCode
				   where this.date.InRange (def)
				   select def;
		}

		private readonly BusinessDocumentEntity		document;
		private readonly ArticleDocumentItemEntity	articleItem;
		private readonly ArticleDefinitionEntity	articleDef;
		private readonly CurrencyCode				currencyCode;
		private readonly System.DateTime			date;

		private readonly PriceRoundingModeEntity	priceRoundingMode;
	}
}

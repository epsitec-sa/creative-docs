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
	public class ArticlePriceCalculator
	{
		public ArticlePriceCalculator(BusinessDocumentEntity document, ArticleDocumentItemEntity articleItem)
		{
			this.document     = document;
			this.articleItem  = articleItem;
			this.articleDef   = this.articleItem.ArticleDefinition;
			this.currencyCode = this.document.BillingCurrencyCode;
			this.date         = this.document.PriceRefDate.GetValueOrDefault (Date.Today).ToDateTime ();
		}


		public void ComputePrice()
		{
			var roundingPolicy = this.priceRoundingMode.PriceRoundingPolicy;
			var quantity       = this.GetTotalQuantity ();
			var articlePrice   = this.GetArticlePrices (quantity).FirstOrDefault ();

			decimal primaryUnitPriceBeforeTax   = this.ComputePrimaryUnitPriceBeforeTax (roundingPolicy, articlePrice);
			decimal primaryLinePriceBeforeTax   = this.ComputePrimaryLinePriceBeforeTax (roundingPolicy, primaryUnitPriceBeforeTax, quantity);
			decimal resultingLinePriceBeforeTax = this.ComputeResultingLinePriceBeforeTax (roundingPolicy, primaryLinePriceBeforeTax);
			
			Tax tax = this.ComputeTax (resultingLinePriceBeforeTax);

			this.articleItem.PrimaryUnitPriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryUnitPriceBeforeTax, this.currencyCode);
			this.articleItem.PrimaryLinePriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryLinePriceBeforeTax, this.currencyCode);
			this.articleItem.ResultingLinePriceBeforeTax = PriceCalculator.ClipPriceValue (resultingLinePriceBeforeTax, this.currencyCode);
			this.articleItem.ResultingLineTax1 = PriceCalculator.ClipPriceValue (tax.GetTax (0), this.currencyCode);
			this.articleItem.ResultingLineTax2 = PriceCalculator.ClipPriceValue (tax.GetTax (1), this.currencyCode);
			
			this.articleItem.FinalLinePriceBeforeTax = null;
			this.articleItem.TaxRate1 = PriceCalculator.ClipTaxRateValue (tax.GetTaxRate (0));
			this.articleItem.TaxRate2 = PriceCalculator.ClipTaxRateValue (tax.GetTaxRate (1));
		}

		private decimal ApplyDiscount(decimal price, DiscountEntity discount)
		{
			//	TODO: implement discount & rounding
			return price;
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

		private decimal ComputePrimaryUnitPriceBeforeTax(RoundingPolicy roundingPolicy, ArticlePriceEntity articlePrice)
		{
			decimal unitPriceBeforeTax = this.GetUnitPriceBeforeTax (articlePrice);

			if (roundingPolicy == RoundingPolicy.OnUnitPriceBeforeTax)
			{
				unitPriceBeforeTax = this.priceRoundingMode.Round (unitPriceBeforeTax);
			}

			if (roundingPolicy == RoundingPolicy.OnUnitPriceAfterTax)
			{
				Tax tax = this.ComputeTax (unitPriceBeforeTax);

				decimal unitPriceAfterTax = unitPriceBeforeTax + tax.TotalTax;

				unitPriceAfterTax  = this.priceRoundingMode.Round (unitPriceAfterTax);
				unitPriceBeforeTax = tax.ComputeAmountBeforeTax (unitPriceAfterTax);
			}
			
			return unitPriceBeforeTax;
		}

		private decimal ComputePrimaryLinePriceBeforeTax(RoundingPolicy roundingPolicy, decimal unitPriceBeforeTax, decimal quantity)
		{
			return unitPriceBeforeTax * quantity;
		}
		
		private decimal ComputeResultingLinePriceBeforeTax(RoundingPolicy roundingPolicy, decimal linePriceBeforeTax)
		{
			if (this.articleItem.FixedLinePriceBeforeTax.HasValue)
			{
				return this.articleItem.FixedLinePriceBeforeTax.Value;
			}

			foreach (var discount in this.articleItem.Discounts)
			{
				linePriceBeforeTax = this.ApplyDiscount (linePriceBeforeTax, discount);
			}

			if (roundingPolicy == RoundingPolicy.OnFinalPriceBeforeTax)
			{
				linePriceBeforeTax = this.priceRoundingMode.Round (linePriceBeforeTax);
			}

			if (roundingPolicy == RoundingPolicy.OnFinalPriceAfterTax)
			{
				Tax tax = this.ComputeTax (linePriceBeforeTax);

				decimal linePriceAfterTax = linePriceBeforeTax + tax.TotalTax;

				linePriceAfterTax  = this.priceRoundingMode.Round (linePriceAfterTax);
				linePriceBeforeTax = tax.ComputeAmountBeforeTax (linePriceAfterTax);
			}
			
			return linePriceBeforeTax;
		}

		private decimal GetUnitPriceBeforeTax(ArticlePriceEntity articlePrice)
		{
			return articlePrice.ValueBeforeTax;
		}

		private Tax ComputeTax(decimal articleValue)
		{
			TaxCalculator taxCalculator;
			
			if ((this.articleItem.BeginDate.HasValue) &&
				(this.articleItem.EndDate.HasValue))
			{
				taxCalculator = new TaxCalculator (this.articleItem);
			}
			else
			{
				taxCalculator = new TaxCalculator (new Date (this.date));
			}

			var tax = taxCalculator.ComputeTax (articleValue, this.articleItem.VatCode);
			
			if (tax.RateAmounts.Count > 2)
			{
				throw new System.InvalidOperationException ("Cannot process more than 2 different tax rates");
			}

			return tax;
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

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	public class ArticlePriceCalculator : AbstractPriceCalculator
	{
		public ArticlePriceCalculator(CoreData data, BusinessDocumentEntity document, ArticleDocumentItemEntity articleItem)
		{
			this.data = data;
			this.document     = document;
			this.articleItem  = articleItem;
			this.articleDef   = this.articleItem.ArticleDefinition;
			this.currencyCode = this.document.CurrencyCode;
			this.priceGroup   = this.document.PriceGroup.UnwrapNullEntity ();
			this.date         = this.document.PriceRefDate.GetValueOrDefault (Date.Today).ToDateTime ();

			this.notDiscountable = this.articleDef.ArticleCategory.IsNotNull () && this.articleDef.ArticleCategory.NeverApplyDiscount;

			if ((this.priceGroup.IsNotNull ()) &&
				(this.priceGroup.NeverApplyDiscount))
            {
				this.notDiscountable = true;
            }

			if ((this.priceGroup.IsNotNull ()) &&
				(this.priceGroup.DefaultRoundingMode.IsNotNull ()))
			{
				this.priceRoundingMode = this.priceGroup.DefaultRoundingMode;
			}
			else
			{
				//	TODO: use rounding mode found in the settings...

				this.priceRoundingMode = new PriceRoundingModeEntity ()
				{
					Modulo = 0.05M,
					AddBeforeModulo = 0.025M,
					RoundingPolicy = RoundingPolicy.OnFinalPriceAfterTax,
				};
			}
		}


		public ArticleDocumentItemEntity	ArticleItem
		{
			get
			{
				return this.articleItem;
			}
		}

		public Tax							Tax
		{
			get
			{
				return this.tax;
			}
		}

		public bool							NotDiscountable
		{
			get
			{
				return this.notDiscountable;
			}
		}

		
		public void ComputePrice()
		{
			var roundingPolicy = this.priceRoundingMode.RoundingPolicy;
			var quantity       = this.GetTotalQuantity ();
			var articlePrice   = this.GetArticlePrices (quantity).FirstOrDefault ();

			//	TODO: apply PriceGroup to price if articlePrice.ValueOverridesPriceGroup is set to false

			decimal primaryUnitPriceBeforeTax   = this.ComputePrimaryUnitPriceBeforeTax (roundingPolicy, articlePrice);
			decimal primaryLinePriceBeforeTax   = this.ComputePrimaryLinePriceBeforeTax (roundingPolicy, primaryUnitPriceBeforeTax, quantity);
			decimal resultingLinePriceBeforeTax = this.ComputeResultingLinePriceBeforeTax (roundingPolicy, primaryLinePriceBeforeTax);

			decimal primaryLineTax = this.ComputeTaxTotal (primaryLinePriceBeforeTax);
			
			this.tax = this.ComputeTax (resultingLinePriceBeforeTax);

			this.articleItem.PrimaryUnitPriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryUnitPriceBeforeTax, this.currencyCode);
			this.articleItem.PrimaryLinePriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryLinePriceBeforeTax, this.currencyCode);
			this.articleItem.PrimaryLinePriceAfterTax    = PriceCalculator.ClipPriceValue (primaryLinePriceBeforeTax + primaryLineTax, this.currencyCode);
			this.articleItem.ResultingLinePriceBeforeTax = PriceCalculator.ClipPriceValue (resultingLinePriceBeforeTax, this.currencyCode);
			this.articleItem.ResultingLineTax1 = PriceCalculator.ClipPriceValue (this.tax.GetTax (0), this.currencyCode);
			this.articleItem.ResultingLineTax2 = PriceCalculator.ClipPriceValue (this.tax.GetTax (1), this.currencyCode);
			
			this.articleItem.FinalLinePriceBeforeTax = null;

			this.articleItem.TaxRate1 = PriceCalculator.ClipTaxRateValue (this.tax.GetTaxRate (0));
			this.articleItem.TaxRate2 = PriceCalculator.ClipTaxRateValue (this.tax.GetTaxRate (1));
		}

		private decimal ComputeTaxTotal(decimal price)
		{
			return this.ComputeTax (price).TotalTax;
		}
		
		public override void ApplyFinalPriceAdjustment(decimal adjustment)
		{
			if (this.notDiscountable)
			{
				this.articleItem.FinalLinePriceBeforeTax = this.articleItem.ResultingLinePriceBeforeTax;
			}
			else
			{
				this.articleItem.FinalLinePriceBeforeTax = PriceCalculator.ClipPriceValue (this.articleItem.ResultingLinePriceBeforeTax * adjustment, this.currencyCode);
			}
		}

		private decimal ApplyDiscount(decimal price, DiscountEntity discount)
		{
			if (discount.IsNull ())
			{
				return price;
			}

			decimal tax = this.ComputeTaxTotal (price);

			if (discount.DiscountRate.HasValue)
			{
				decimal discountRatio = 1.00M - discount.DiscountRate.Value;
				return this.ApplyDiscountRatio (discount, price, tax, discountRatio);
			}
			
			if (discount.Value.HasValue)
			{
				decimal discountAmount = discount.Value.Value;
				return this.ApplyDiscountFixed (discount, price, tax, discountAmount);
			}
			
			return price;
		}


		private decimal ApplyDiscountRatio(DiscountEntity discount, decimal priceBeforeTax, decimal tax, decimal discountRatio)
		{
			if (discount.ValueIncludesTaxes)
			{
				decimal priceAfterTax = priceBeforeTax + tax;
				decimal taxRatio      = (priceAfterTax == 0) ? 0 : priceBeforeTax / priceAfterTax;
				
				return priceAfterTax * discountRatio * taxRatio;
			}
			else
			{
				return priceBeforeTax * discountRatio;
			}
		}

		private decimal ApplyDiscountFixed(DiscountEntity discount, decimal price, decimal tax, decimal discountAmount)
		{
			if (discount.ValueIncludesTaxes)
			{
				decimal priceBeforeTax = price;
				decimal priceAfterTax  = priceBeforeTax + tax;
				decimal taxRatio       = (priceAfterTax == 0) ? 0 : priceBeforeTax / priceAfterTax;
				
				return taxRatio * (priceAfterTax + discountAmount);
			}
			else
			{
				return price - discountAmount;
			}
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
			Tax tax = this.ComputeTax (linePriceBeforeTax);
			
			if (this.articleItem.FixedLinePrice.HasValue)
			{
				decimal fixedLinePrice = this.articleItem.FixedLinePrice.Value;

				if (this.articleItem.FixedLinePriceIncludesTaxes)
				{
					return tax.ComputeAmountBeforeTax (fixedLinePrice);
				}
				else
				{
					return fixedLinePrice;
				}
			}

			foreach (var discount in this.articleItem.Discounts)
			{
				linePriceBeforeTax = this.ApplyDiscount (linePriceBeforeTax, discount);
				
				if ((discount.RoundingMode.IsNotNull ()) &&
					(discount.RoundingMode.RoundingPolicy != RoundingPolicy.None))
				{
					roundingPolicy = discount.RoundingMode.RoundingPolicy;
				}
			}

			if (roundingPolicy == RoundingPolicy.OnFinalPriceBeforeTax)
			{
				linePriceBeforeTax = this.priceRoundingMode.Round (linePriceBeforeTax);
			}

			if (roundingPolicy == RoundingPolicy.OnFinalPriceAfterTax)
			{
				tax = this.ComputeTax (linePriceBeforeTax);
				decimal linePriceAfterTax = linePriceBeforeTax + tax.TotalTax;

				linePriceAfterTax  = this.priceRoundingMode.Round (linePriceAfterTax);
				linePriceBeforeTax = tax.ComputeAmountBeforeTax (linePriceAfterTax);
			}
			
			return linePriceBeforeTax;
		}

		private decimal GetUnitPriceBeforeTax(ArticlePriceEntity articlePrice)
		{
			if (articlePrice == null)
			{
				return 0;
			}

			decimal unitPrice = this.ComputeTotalUnitPriceBeforeTax (articlePrice);

			if (articlePrice.ValueIncludesTaxes)
			{
				//	Use a dummy amount to compute the taxes, just so that we can have the
				//	mean rate if the VAT is split over two years with different rates for
				//	the same code :

				Tax tax = this.ComputeTax (1000);
				return tax.ComputeAmountBeforeTax (unitPrice);
			}
			else
			{
				return unitPrice;
			}
		}

		private decimal ComputeTotalUnitPriceBeforeTax(ArticlePriceEntity articlePrice)
		{
			decimal totalPrice = articlePrice.Value;

			var parameterCodesToValues = ArticleParameterHelper.GetArticleParametersValues (this.articleItem, false);

			foreach (PriceCalculatorEntity priceCalculator in articlePrice.PriceCalculators)
			{
				decimal? price = null;

				try
				{
					price = this.ExecutePriceCalculator (parameterCodesToValues, priceCalculator);
				}
				catch
				{
					// TODO If a exception is thrown, then an error occurred while computing the price,
					// like the price table was not properly defined regarding the parameters of the
					// object, or the instantiation of the parameters of the object is not valid, or
					// etc. Should we do something about that?
					// Marc

					price = 0;
				}

				if (price.HasValue)
				{
					totalPrice += price.Value;
				}

				// TODO If the price is null, then the price was not defined for the calculator. Should
				// we do something about that?
				// Marc

			}

			return totalPrice;
		}


		private decimal? ExecutePriceCalculator(Dictionary<string, string> parameterCodesToValues, PriceCalculatorEntity priceCalculator)
		{
			decimal? result = null;

			DimensionTable priceTable = priceCalculator.GetPriceTable ();

			if (priceTable != null)
			{
				string[] key = priceTable.Dimensions
					.Select (d => parameterCodesToValues[d.Code])
					.ToArray ();

				result = priceTable[key];
			}

			return result;
		}


		private Tax ComputeTax(decimal articleValue)
		{
			TaxCalculator taxCalculator;
			
			if ((this.articleItem.BeginDate.HasValue) &&
				(this.articleItem.EndDate.HasValue))
			{
				taxCalculator = new TaxCalculator (this.data, this.articleItem);
			}
			else
			{
				taxCalculator = new TaxCalculator (this.data, new Date (this.date));
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
			return this.articleDef.GetArticlePrices (quantity, this.date, this.currencyCode, this.priceGroup);
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

		private readonly CoreData					data;
		private readonly BusinessDocumentEntity		document;
		private readonly ArticleDocumentItemEntity	articleItem;
		private readonly ArticleDefinitionEntity	articleDef;
		private readonly CurrencyCode				currencyCode;
		private readonly System.DateTime			date;
		private readonly PriceGroupEntity			priceGroup;

		private readonly PriceRoundingModeEntity	priceRoundingMode;
		
		private Tax									tax;
		private bool								notDiscountable;
	}
}

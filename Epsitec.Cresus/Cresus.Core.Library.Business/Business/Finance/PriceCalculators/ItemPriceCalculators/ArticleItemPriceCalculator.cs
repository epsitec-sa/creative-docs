//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators
{
	/// <summary>
	/// The <c>ArticleItemPriceCalculator</c> class computes the price of an article line.
	/// </summary>
	public class ArticleItemPriceCalculator : AbstractItemPriceCalculator
	{
		public ArticleItemPriceCalculator(IDocumentPriceCalculator priceCalculator, ArticleDocumentItemEntity articleItem)
			: this (priceCalculator.Data, priceCalculator.BusinessContext, priceCalculator.Document, priceCalculator.Metadata, articleItem)
		{
		}
		
		private ArticleItemPriceCalculator(CoreData data, IBusinessContext businessContext, BusinessDocumentEntity document, DocumentMetadataEntity metadata, ArticleDocumentItemEntity articleItem)
		{
			this.data            = data;
			this.businessContext = businessContext;
			this.document        = document;
			this.metadata        = metadata;
			this.articleItem     = articleItem;
			this.articleDef      = this.articleItem.ArticleDefinition;
			this.currencyCode    = this.document.CurrencyCode;
			this.priceGroup      = this.document.PriceGroup.UnwrapNullEntity ();
			this.date            = this.document.PriceRefDate.GetValueOrDefault (Date.Today).ToDateTime ();

			if (this.articleItem.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.DirtyArticleNotDiscountable))
			{
				bool articleNotDiscountable = false;

				if ((this.priceGroup.IsNotNull ()) &&
					(this.priceGroup.NeverApplyDiscount))
				{
					articleNotDiscountable = true;
				}
				else
				{
					articleNotDiscountable = this.articleDef.IsNotNull () && this.articleDef.ArticleCategory.IsNotNull () && this.articleDef.ArticleCategory.NeverApplyDiscount;
				}

				if (articleNotDiscountable)
				{
					this.articleItem.ArticleAttributes |=  ArticleDocumentItemAttributes.ArticleNotDiscountable;
					this.articleItem.ArticleAttributes &= ~ArticleDocumentItemAttributes.DirtyArticleNotDiscountable;
				}
				else
				{
					this.articleItem.ArticleAttributes &= ~ArticleDocumentItemAttributes.ArticleNotDiscountable;
					this.articleItem.ArticleAttributes &= ~ArticleDocumentItemAttributes.DirtyArticleNotDiscountable;
				}
			}
			
			this.notDiscountable = this.articleItem.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.ArticleNotDiscountable)
								|| this.articleItem.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.NeverApplyDiscount);

			if ((this.priceGroup.IsNotNull ()) &&
				(this.priceGroup.DefaultRoundingMode.IsNotNull ()))
			{
				this.priceRoundingMode = this.priceGroup.DefaultRoundingMode;
			}
			else
			{
				var settings = this.businessContext.GetCached<BusinessSettingsEntity> ();

				if ((settings.IsNull ()) ||
					(settings.Finance.IsNull ()) ||
					(settings.Finance.DefaultBillingRoundingMode.IsNull ()))
				{
					this.priceRoundingMode = new PriceRoundingModeEntity ()
					{
						Modulo = 0.05M,
						AddBeforeModulo = 0.025M,
						RoundingPolicy = RoundingPolicy.OnFinalPriceAfterTax,
					};
				}
				else
				{
					this.priceRoundingMode = settings.Finance.DefaultBillingRoundingMode;
				}
			}
		}


		public ArticleDocumentItemEntity		ArticleItem
		{
			get
			{
				return this.articleItem;
			}
		}

		public Tax								Tax
		{
			get
			{
				return this.tax;
			}
		}

		public bool								NotDiscountable
		{
			get
			{
				return this.notDiscountable;
			}
		}

		public bool								ProFormaOnly
		{
			get
			{
				return this.articleItem.Attributes.HasFlag (DocumentItemAttributes.ProFormaOnly);
			}
		}

		
		public void ComputePrice()
		{
			if ((this.articleItem.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.ArticlePricesFrozen)) &&
				(this.articleItem.ResultingLinePriceBeforeTax.HasValue))
			{
				this.tax = this.ComputeTax (this.articleItem.ResultingLinePriceBeforeTax.Value);
			}
			else
			{
				var roundingPolicy    = this.priceRoundingMode.RoundingPolicy;
				var unitPriceQuantity = this.articleItem.GetOrderedQuantity ();
				var realPriceQuantity = this.GetRealPriceQuantity ();
				var articlePrice      = this.GetArticlePrices (unitPriceQuantity, realPriceQuantity).FirstOrDefault ();

				//	TODO: apply PriceGroup to price if articlePrice.ValueOverridesPriceGroup is set to false

				decimal primaryUnitPriceBeforeTax   = this.ComputePrimaryUnitPriceBeforeTax (roundingPolicy, articlePrice);
				decimal primaryLinePriceBeforeTax   = this.ComputePrimaryLinePriceBeforeTax (roundingPolicy, primaryUnitPriceBeforeTax, realPriceQuantity);
				decimal resultingLinePriceBeforeTax = this.ComputeResultingLinePriceBeforeTax (roundingPolicy, primaryLinePriceBeforeTax);

				decimal primaryLineTax = this.ComputeTaxTotal (primaryLinePriceBeforeTax);

				this.tax = this.ComputeTax (resultingLinePriceBeforeTax);

				this.articleItem.PrimaryUnitPriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryUnitPriceBeforeTax, this.currencyCode);
				this.articleItem.PrimaryUnitPriceAfterTax    = PriceCalculator.ClipPriceValue (this.primaryUnitPriceAfterTax, this.currencyCode);
				this.articleItem.PrimaryLinePriceBeforeTax   = PriceCalculator.ClipPriceValue (primaryLinePriceBeforeTax, this.currencyCode);
				this.articleItem.PrimaryLinePriceAfterTax    = PriceCalculator.ClipPriceValue (primaryLinePriceBeforeTax + primaryLineTax, this.currencyCode);
				this.articleItem.ResultingLinePriceBeforeTax = PriceCalculator.ClipPriceValue (resultingLinePriceBeforeTax, this.currencyCode);
				this.articleItem.ResultingLineTax1 = PriceCalculator.ClipPriceValue (this.tax.GetTax (0), this.currencyCode);
				this.articleItem.ResultingLineTax2 = PriceCalculator.ClipPriceValue (this.tax.GetTax (1), this.currencyCode);

				decimal resultingLineTax  = this.articleItem.ResultingLineTax1.GetValueOrDefault () + this.articleItem.ResultingLineTax2.GetValueOrDefault ();
				decimal resultingTaxDelta = resultingLineTax - this.tax.TotalTax;

				this.articleItem.FinalLinePriceBeforeTax = null;

				this.articleItem.TaxRate1 = PriceCalculator.ClipTaxRateValue (this.tax.GetTaxRate (0));
				this.articleItem.TaxRate2 = PriceCalculator.ClipTaxRateValue (this.tax.GetTaxRate (1));

				this.articleItem.ArticleAttributes &= ~ArticleDocumentItemAttributes.DirtyArticlePrices;
			}
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

		private decimal ApplyDiscount(decimal price, PriceDiscountEntity discount)
		{
			if (discount.IsNull ())
			{
				return price;
			}

			decimal tax = this.ComputeTaxTotal (price);

			if (discount.DiscountRate.HasValue)
			{
				decimal discountRatio = 1.00M - System.Math.Abs (discount.DiscountRate.Value);
				return this.ApplyDiscountRatio (discount, price, tax, discountRatio);
			}
			
			if (discount.Value.HasValue)
			{
				decimal discountAmount = System.Math.Abs (discount.Value.Value);
				return this.ApplyDiscountFixed (discount, price, tax, discountAmount);
			}
			
			return price;
		}


		private decimal ApplyDiscountRatio(PriceDiscountEntity discount, decimal priceBeforeTax, decimal tax, decimal discountRatio)
		{
			if (discountRatio < 0)
			{
				discountRatio = 0;
			}

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

		private decimal ApplyDiscountFixed(PriceDiscountEntity discount, decimal price, decimal tax, decimal discountAmount)
		{
			if (discount.ValueIncludesTaxes)
			{
				decimal priceBeforeTax = price;
				decimal priceAfterTax  = priceBeforeTax + tax;
				decimal taxRatio       = (priceAfterTax == 0) ? 0 : priceBeforeTax / priceAfterTax;
				
				return System.Math.Max (0, taxRatio * (priceAfterTax - discountAmount));
			}
			else
			{
				return System.Math.Max (0, price - discountAmount);
			}
		}

		public decimal GetRealPriceQuantity()
		{
			switch (this.metadata.DocumentCategory.DocumentType)
			{
				case DocumentType.Invoice:
				case DocumentType.InvoiceProForma:
					return this.articleItem.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == ArticleQuantityType.Billed).Sum (x => this.articleDef.ConvertToBillingUnit (x.Quantity, x.Unit));

				default:
					return this.articleItem.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == ArticleQuantityType.Ordered).Sum (x => this.articleDef.ConvertToBillingUnit (x.Quantity, x.Unit));
			}
		}

		private decimal ComputePrimaryUnitPriceBeforeTax(RoundingPolicy roundingPolicy, ArticlePriceEntity articlePrice)
		{
			decimal unitPriceBeforeTax = this.GetUnitPriceBeforeTax (articlePrice);

			if (roundingPolicy == RoundingPolicy.OnUnitPriceBeforeTax)
			{
				unitPriceBeforeTax = this.priceRoundingMode.Round (unitPriceBeforeTax);
			}
			else if (roundingPolicy == RoundingPolicy.OnUnitPriceAfterTax)
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
			
			if ((this.articleItem.FixedPrice.HasValue) &&
				(this.articleItem.FixedLinePrice))
			{
				decimal fixedLinePrice = this.articleItem.FixedPrice.Value;

				if (this.articleItem.FixedPriceIncludesTaxes)
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
			decimal unitPrice;

			//	Use a dummy amount to compute the taxes, just so that we can have the
			//	mean rate if the VAT is split over two years with different rates for
			//	the same code :

			Tax tax = this.ComputeTax (1000);

			//	If the user specified a fixed unit price, use it as the base for all of our
			//	calculations :
			
			if ((this.articleItem.FixedPrice.HasValue) &&
				(this.articleItem.FixedUnitPrice))
			{
				unitPrice = this.articleItem.FixedPrice.Value;

				if (this.articleItem.FixedPriceIncludesTaxes)
				{
					this.primaryUnitPriceAfterTax = unitPrice;
					return tax.ComputeAmountBeforeTax (unitPrice);
				}
				else
				{
					this.primaryUnitPriceAfterTax = tax.ComputeAmountAfterTax (unitPrice);
					return unitPrice;
				}
			}

			//	If the business logic provides a reference unit price, then use it instead
			//	of what could be computed based on the article definition :

			if (this.articleItem.ReferenceUnitPriceBeforeTax.HasValue)
			{
				unitPrice = this.articleItem.ReferenceUnitPriceBeforeTax.Value;
				this.primaryUnitPriceAfterTax = tax.ComputeAmountAfterTax (unitPrice);
				return unitPrice;
			}

			if (articlePrice.IsNull ())
			{
				this.primaryUnitPriceAfterTax = null;
				return 0;
			}

			unitPrice = this.ComputeTotalUnitPriceBeforeTax (articlePrice);

			if (articlePrice.ValueIncludesTaxes)
			{
				this.primaryUnitPriceAfterTax = unitPrice;
				return tax.ComputeAmountBeforeTax (unitPrice);
			}
			else
			{
				this.primaryUnitPriceAfterTax = tax.ComputeAmountAfterTax (unitPrice);
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


		public IEnumerable<ArticlePriceEntity> GetArticlePrices(decimal quantity, decimal fallbackQuantity)
		{
			if (quantity == 0)
			{
				quantity = fallbackQuantity == 0 ? 1 : fallbackQuantity;
			}

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
		private readonly IBusinessContext			businessContext;
		private readonly DocumentMetadataEntity		metadata;
		private readonly BusinessDocumentEntity		document;
		private readonly ArticleDocumentItemEntity	articleItem;
		private readonly ArticleDefinitionEntity	articleDef;
		private readonly CurrencyCode				currencyCode;
		private readonly System.DateTime			date;
		private readonly PriceGroupEntity			priceGroup;

		private readonly PriceRoundingModeEntity	priceRoundingMode;
		
		private Tax									tax;
		private bool								notDiscountable;
		private decimal?							primaryUnitPriceAfterTax;
	}
}

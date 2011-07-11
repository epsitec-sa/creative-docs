//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators
{
	public class SubTotalItemPriceCalculator : AbstractItemPriceCalculator
	{
		public SubTotalItemPriceCalculator(IDocumentPriceCalculator priceCalculator, SubTotalDocumentItemEntity totalItem)
			: this (priceCalculator.Data, priceCalculator.Document, totalItem)
		{
		}

		public SubTotalItemPriceCalculator(CoreData data, BusinessDocumentEntity document, SubTotalDocumentItemEntity totalItem)
		{
			this.document  = document;
			this.totalItem = totalItem;
			this.discount  = this.totalItem.Discount.UnwrapNullEntity ();
			this.currencyCode = this.document.CurrencyCode;
		}


		public GroupItemPriceCalculator Group
		{
			get
			{
				return this.group;
			}
		}

		public SubTotalDocumentItemEntity Item
		{
			get
			{
				return this.totalItem;
			}
		}

		
		public void ComputePrice(GroupItemPriceCalculator groupPriceCalculator)
		{
			this.group = groupPriceCalculator;
			
			this.taxDiscountable    = this.group.TaxDiscountable;
			this.taxNotDiscountable = this.group.TaxNotDiscountable;

			decimal primaryPriceBeforeTax = this.group.TotalPriceBeforeTax;
			decimal primaryTax = this.group.TotalTax;

			this.totalItem.PrimaryPriceBeforeTax = PriceCalculator.ClipPriceValue (primaryPriceBeforeTax, this.currencyCode);
			this.totalItem.PrimaryTax            = PriceCalculator.ClipPriceValue (primaryTax, this.currencyCode);

			decimal resultingPriceBeforeTax = primaryPriceBeforeTax;
			decimal resultingTax            = primaryTax;

			this.ApplyDiscount (ref resultingPriceBeforeTax, ref resultingTax);
			this.ApplyFixedPrice (ref resultingPriceBeforeTax, ref resultingTax);

			decimal resultingPriceAfterTax  = resultingPriceBeforeTax + resultingTax;


			resultingPriceBeforeTax = PriceCalculator.ClipPriceValue (resultingPriceBeforeTax, this.currencyCode);
			resultingPriceAfterTax  = PriceCalculator.ClipPriceValue (resultingPriceAfterTax, this.currencyCode);

			//	Use the prices as the references rather than using the tax; this ensures that
			//	rounding errors affect the tax, not the price:
			
			this.totalItem.ResultingPriceBeforeTax = resultingPriceBeforeTax;
			this.totalItem.ResultingTax            = resultingPriceAfterTax - resultingPriceBeforeTax;

			this.totalItem.FinalPriceBeforeTax = null;
		}

		public override void ApplyFinalPriceAdjustment(decimal adjustment)
		{
			decimal finalTotal   = this.totalItem.ResultingPriceBeforeTax.Value;
			decimal discount     = this.group.TotalPriceBeforeTax - finalTotal;
			decimal discountable = this.group.TotalPriceBeforeTaxDiscountable - discount;

			decimal adjustedTotal = discountable * adjustment + this.group.TotalPriceBeforeTaxNotDiscountable;

			this.totalItem.FinalPriceBeforeTax = PriceCalculator.ClipPriceValue (adjustedTotal, this.currencyCode);
			this.group.AdjustFinalPrices (adjustedTotal);
		}

		private void ApplyFixedPrice(ref decimal priceBeforeTax, ref decimal tax)
		{
			if (this.totalItem.FixedPrice.HasValue)
			{
				bool computeIncludingTaxes = this.totalItem.FixedPriceIncludesTaxes;
				this.ApplyFixedDiscount (this.totalItem.FixedPrice.Value, ref priceBeforeTax, ref tax, computeIncludingTaxes);
			}
		}

		private void ApplyDiscount(ref decimal priceBeforeTax, ref decimal tax)
		{
			if (this.discount.IsNull ())
            {
				return;
            }
			
			if (this.discount.DiscountRate.HasValue)
			{
				decimal discountRatio = 1.00M - this.discount.DiscountRate.Value;
				this.ApplyDiscountRatio (ref priceBeforeTax, ref tax, discountRatio);
				return;
			}
			
			if (this.discount.Value.HasValue)
			{
				if (this.discount.ValueIncludesTaxes)
				{
					decimal discountedPriceAfterTax = priceBeforeTax + tax - this.discount.Value.Value;
					this.ApplyFixedDiscount (discountedPriceAfterTax, ref priceBeforeTax, ref tax, true);
					return;
				}
				else
				{
					decimal discountedPriceBeforeTax = priceBeforeTax - this.discount.Value.Value;
					this.ApplyFixedDiscount (discountedPriceBeforeTax, ref priceBeforeTax, ref tax, false);
					return;
				}
			}
		}


		private void ApplyDiscountRatio(ref decimal priceBeforeTax, ref decimal tax, decimal discountRatio)
		{
			if (this.discount.ValueIncludesTaxes)
			{
				decimal priceAfterTax = priceBeforeTax + tax;
				decimal discountedPriceAfterTax = priceAfterTax * discountRatio;
				
				if (this.discount.RoundingMode.IsNotNull ())
				{
					discountedPriceAfterTax = this.discount.RoundingMode.Round (discountedPriceAfterTax);
				}

				this.ApplyFixedDiscount (discountedPriceAfterTax, ref priceBeforeTax, ref tax, true);
			}
			else
			{
				decimal discountedPriceBeforeTax = priceBeforeTax * discountRatio;

				if (this.discount.RoundingMode.IsNotNull ())
				{
					discountedPriceBeforeTax = this.discount.RoundingMode.Round (discountedPriceBeforeTax);
				}

				this.ApplyFixedDiscount (discountedPriceBeforeTax, ref priceBeforeTax, ref tax, false);
			}
		}

		private void ApplyFixedDiscount(decimal discountedPrice, ref decimal priceBeforeTax, ref decimal tax, bool computeIncludingTaxes)
		{
			decimal totalBeforeTaxDiscountable;
			decimal totalTaxDiscountable;

			Tax taxDiscountable;

			if (computeIncludingTaxes)
			{
				this.group.ComputeDiscountAfterTax (discountedPrice, out totalBeforeTaxDiscountable, out totalTaxDiscountable, out taxDiscountable);
			}
			else
			{
				this.group.ComputeDiscountBeforeTax (discountedPrice, out totalBeforeTaxDiscountable, out totalTaxDiscountable, out taxDiscountable);
			}

			priceBeforeTax = totalBeforeTaxDiscountable + this.group.TotalPriceBeforeTaxNotDiscountable;
			tax = totalTaxDiscountable + this.group.TotalTaxNotDiscountable;

			this.taxDiscountable = taxDiscountable;
		}

		
		private readonly BusinessDocumentEntity		document;
		private readonly SubTotalDocumentItemEntity	totalItem;
		private readonly PriceDiscountEntity				discount;
		private readonly CurrencyCode				currencyCode;

		private GroupItemPriceCalculator				group;
		private Tax									taxDiscountable;
		private Tax									taxNotDiscountable;
	}
}

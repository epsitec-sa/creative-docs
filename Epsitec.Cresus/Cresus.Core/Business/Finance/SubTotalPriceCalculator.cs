//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class SubTotalPriceCalculator : AbstractPriceCalculator
	{
		public SubTotalPriceCalculator(BusinessDocumentEntity document, SubTotalDocumentItemEntity totalItem)
		{
			this.document  = document;
			this.totalItem = totalItem;
			this.discount  = this.totalItem.Discount.UnwrapNullEntity ();
			this.currencyCode = this.document.BillingCurrencyCode;
		}


		public GroupPriceCalculator Group
		{
			get
			{
				return this.group;
			}
		}

		
		public void ComputePrice(GroupPriceCalculator groupPriceCalculator)
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

		public void AdjustFinalPrice(decimal adjustmentRate)
		{
			decimal totalPrice = this.group.TotalPriceBeforeTax;
			decimal totalTax   = this.group.TotalTax;

			
		}

		private void ApplyFixedPrice(ref decimal priceBeforeTax, ref decimal tax)
		{
			if (this.totalItem.FixedPrice.HasValue)
			{
				this.ApplyFixedDiscount (this.totalItem.FixedPrice.Value, ref priceBeforeTax, ref tax, this.totalItem.FixedPriceIncludesTaxes);
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

		private void ApplyDiscount(ref decimal priceBeforeTax, ref decimal tax)
		{
			//	TODO: implement discount & rounding, based either on price before or after tax...
			//	Attention: les rabais ne peuvent �tre appliqu�s que sur la part compressible du
			//	montant !

			decimal priceAfterTax = priceBeforeTax + tax;

			if (this.discount.DiscountRate.HasValue)
			{
				decimal discountRatio = 1.00M - this.discount.DiscountRate.Value;
				
				if (this.discount.ValueIncludesTaxes)
				{
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
			else
			{
				//	TODO: ...
			}
		}
		

		
		private readonly BusinessDocumentEntity		document;
		private readonly SubTotalDocumentItemEntity	totalItem;
		private readonly DiscountEntity				discount;
		private readonly CurrencyCode				currencyCode;

		private GroupPriceCalculator				group;
		private Tax									taxDiscountable;
		private Tax									taxNotDiscountable;
	}
}

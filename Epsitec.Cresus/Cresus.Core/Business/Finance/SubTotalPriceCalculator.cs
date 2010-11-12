//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				decimal fixedPrice = this.totalItem.FixedPrice.Value;

				decimal totalBeforeTaxDiscountable;
				decimal totalTaxDiscountable;

				if (this.totalItem.FixedPriceIncludesTaxes)
				{
					this.group.ComputeDiscountAfterTax (fixedPrice, out totalBeforeTaxDiscountable, out totalTaxDiscountable);
				}
				else
				{
					this.group.ComputeDiscountBeforeTax (fixedPrice, out totalBeforeTaxDiscountable, out totalTaxDiscountable);
				}
					
				priceBeforeTax = totalBeforeTaxDiscountable + this.group.TotalPriceBeforeTaxNotDiscountable;
				tax = totalTaxDiscountable + this.group.TotalTaxNotDiscountable;
			}
		}

		private void ApplyDiscount(ref decimal priceBeforeTax, ref decimal tax)
		{
			//	TODO: implement discount & rounding, based either on price before or after tax...
			//	Attention: les rabais ne peuvent être appliqués que sur la part compressible du
			//	montant !
		}
		

		
		private readonly BusinessDocumentEntity		document;
		private readonly SubTotalDocumentItemEntity	totalItem;
		private readonly DiscountEntity				discount;
		private readonly CurrencyCode				currencyCode;

		private GroupPriceCalculator				group;
	}
}

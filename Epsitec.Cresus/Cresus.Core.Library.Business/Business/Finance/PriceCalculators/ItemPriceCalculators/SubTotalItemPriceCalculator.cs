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
			this.document     = document;
			this.totalItem    = totalItem;
			this.discount     = this.totalItem.Discount.UnwrapNullEntity ();
			this.currencyCode = this.document.CurrencyCode;

			if ((this.document.IsNotNull ()) &&
				(this.document.PriceGroup.IsNotNull ()))
			{
				this.billingMode = this.document.PriceGroup.BillingMode;
			}
		}


		public GroupItemPriceCalculator			Group
		{
			get
			{
				return this.group;
			}
		}

		public SubTotalDocumentItemEntity		Item
		{
			get
			{
				return this.totalItem;
			}
		}

		public BillingMode						BillingMode
		{
			get
			{
				return this.billingMode;
			}
		}

		
		public void ComputePrice(GroupItemPriceCalculator groupPriceCalculator)
		{
			this.group = groupPriceCalculator;
			
			this.taxDiscountable    = this.group.TaxDiscountable;
			this.taxNotDiscountable = this.group.TaxNotDiscountable;

			switch (this.billingMode)
			{
				case BillingMode.ExcludingTax:
					this.ComputePriceTaxExclusive ();
					break;

				case BillingMode.IncludingTax:
					this.ComputePriceTaxInclusive ();
					break;

				case BillingMode.None:
					break;
			}
		}


		private void ComputePriceTaxInclusive()
		{
			decimal totalPrice = this.group.TotalPriceBeforeTax + this.group.TotalTax;

			this.totalItem.PriceBeforeTax1 = null;
			this.totalItem.PriceBeforeTax2 = null;
			this.totalItem.PriceAfterTax1  = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);

			totalPrice = this.ApplyDiscount (totalPrice);

			this.totalItem.PriceAfterTax2 = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);
		}

		private void ComputePriceTaxExclusive()
		{
			decimal totalPrice = this.group.TotalPriceBeforeTax;
			decimal totalTax   = this.group.TotalTax;

			this.totalItem.PriceBeforeTax1 = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);
			this.totalItem.PriceAfterTax1  = null;
			this.totalItem.PriceAfterTax2  = null;

			totalPrice = this.ApplyDiscount (totalPrice);

			this.totalItem.PriceBeforeTax2 = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);
		}

		public override void ApplyFinalPriceAdjustment(decimal adjustment)
		{
#if false
			decimal finalTotal   = this.totalItem.ResultingPriceBeforeTax.Value;
			decimal discount     = this.group.TotalPriceBeforeTax - finalTotal;
			decimal discountable = this.group.TotalPriceBeforeTaxDiscountable - discount;

			decimal adjustedTotal = discountable * adjustment + this.group.TotalPriceBeforeTaxNotDiscountable;

			this.totalItem.FinalPriceBeforeTax = PriceCalculator.ClipPriceValue (adjustedTotal, this.currencyCode);
			this.group.AdjustFinalPrices (adjustedTotal);
#endif
		}

		private decimal ApplyDiscount(decimal price)
		{
			if (this.discount.IsNull ())
			{
				return price;
			}
			
			if (this.discount.DiscountRate.HasValue)
			{
				return this.ApplyRounding (price * (1.00M - System.Math.Abs (this.discount.DiscountRate.Value)));
			}
			
			if (this.discount.Value.HasValue)
			{
				return this.ApplyRounding (price - System.Math.Abs (this.discount.Value.Value));
			}

			return price;
		}


		private decimal ApplyRounding(decimal price)
		{
			if (this.discount.RoundingMode.IsNotNull ())
			{
				price = this.discount.RoundingMode.Round (price);
			}

			return PriceCalculator.ClipPriceValue (price, this.currencyCode);
		}

#if false
		private void ApplyFixedDiscount(decimal discountedPrice, ref decimal priceBeforeTax, ref decimal tax, bool computeIncludingTaxes)
		{
			if (discountedPrice < 0)
			{
				discountedPrice = 0;
			}

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
#endif
		
		private readonly BusinessDocumentEntity		document;
		private readonly BillingMode				billingMode;
		private readonly SubTotalDocumentItemEntity	totalItem;
		private readonly PriceDiscountEntity		discount;
		private readonly CurrencyCode				currencyCode;

		private GroupItemPriceCalculator			group;
		private Tax									taxDiscountable;
		private Tax									taxNotDiscountable;
	}
}

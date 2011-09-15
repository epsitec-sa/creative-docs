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
			this.billingMode  = PriceCalculator.GetBillingMode ();
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

			totalPrice = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);

			this.totalItem.PriceBeforeTax1 = null;
			this.totalItem.PriceBeforeTax2 = null;
			this.totalItem.PriceAfterTax1  = totalPrice;

			totalPrice = this.ApplyDiscount (totalPrice);
			totalPrice = this.ApplyRounding (totalPrice);
			totalPrice = this.ApplyRounding (totalPrice, RoundingPolicy.OnTotalPrice);

			this.totalItem.PriceAfterTax2 = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);
		}

		private void ComputePriceTaxExclusive()
		{
			decimal totalPrice = this.group.TotalPriceBeforeTax;
			decimal totalTax   = this.group.TotalTax;

			totalPrice = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);

			this.totalItem.PriceBeforeTax1 = totalPrice;
			this.totalItem.PriceAfterTax1  = null;
			this.totalItem.PriceAfterTax2  = null;

			totalPrice = this.ApplyDiscount (totalPrice);
			totalPrice = this.ApplyRounding (totalPrice);
			totalPrice = this.ApplyRounding (totalPrice, RoundingPolicy.OnTotalPrice);

			this.totalItem.PriceBeforeTax2 = PriceCalculator.ClipPriceValue (totalPrice, this.currencyCode);
		}

		public override void ApplyFinalPriceAdjustment(decimal adjustment)
		{
			//	TODO: implement
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
				decimal value = price * System.Math.Abs (this.discount.DiscountRate.Value);
				decimal total = price - this.ApplyRounding (value, RoundingPolicy.OnTotalRounding);

				return total;
			}

			if (this.discount.Value.HasValue)
			{
				decimal value = System.Math.Abs (System.Math.Abs (this.discount.Value.Value));
				decimal total = price - this.ApplyRounding (value, RoundingPolicy.OnTotalRounding);

				return total;
			}

			return price;
		}

		private decimal ApplyRounding(decimal price, RoundingPolicy policy)
		{
			return PriceCalculator.Round (price, policy, this.currencyCode);
		}

		private decimal ApplyRounding(decimal price)
		{
			if ((this.discount != null) &&
				(this.discount.RoundingMode.IsNotNull ()))
			{
				return this.discount.RoundingMode.Round (price);
			}
			else
			{
				return price;
			}
		}
		
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

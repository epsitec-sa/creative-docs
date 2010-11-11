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
		}

		
		public void ComputePrice(GroupPriceCalculator groupPriceCalculator)
		{
			this.group = groupPriceCalculator;

			decimal primaryPriceBeforeTax = this.group.TotalPriceBeforeTax;
			decimal primaryTax = this.group.TotalTax;

			this.totalItem.PrimaryPriceBeforeTax = primaryPriceBeforeTax;
			this.totalItem.PrimaryTax            = primaryTax;

			decimal resultingPriceBeforeTax = primaryPriceBeforeTax;
			decimal resultingTax            = primaryTax;

			this.ApplyDiscount (ref resultingPriceBeforeTax, ref resultingTax);
			this.ApplyFixedPrice (ref resultingPriceBeforeTax, ref resultingTax);
		}

		private void ApplyFixedPrice(ref decimal priceBeforeTax, ref decimal tax)
		{
			if (this.totalItem.FixedPrice.HasValue)
			{
				decimal fixedPrice = this.totalItem.FixedPrice.Value;

				if (this.totalItem.FixedPriceIncludesTaxes)
				{
				}
				else
				{

					priceBeforeTax = fixedPrice;
				}
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

		private GroupPriceCalculator				group;
	}
}

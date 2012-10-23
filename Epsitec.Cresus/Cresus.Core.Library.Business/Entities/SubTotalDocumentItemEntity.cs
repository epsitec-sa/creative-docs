//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class SubTotalDocumentItemEntity : ICopyableEntity<SubTotalDocumentItemEntity>
	{
		public override FormattedText GetCompactSummary()
		{
			var builder = new TextBuilder ();

			builder.Append ("Sous-total ");
			builder.Append (Misc.PriceToString (this.PriceBeforeTax2));
			builder.Append (" HT, ");
			builder.Append (Misc.PriceToString (this.PriceAfterTax2));
			builder.Append (" TTC");

			if (this.Discount.HasDiscountRate)
			{
				builder.Append (" (après rabais en %)");
			}
			else if (this.Discount.HasValue)
			{
				builder.Append (" (après rabais)");
			}

			return builder.ToFormattedText ();
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}

		public override void Process(IDocumentPriceCalculator priceCalculator)
		{
			priceCalculator.Process (new SubTotalItemPriceCalculator (priceCalculator, this));
		}
		
		#region ICloneable<SubTotalDocumentItemEntity> Members

		void ICopyableEntity<SubTotalDocumentItemEntity>.CopyTo(BusinessContext businessContext, SubTotalDocumentItemEntity copy)
		{
			copy.Attributes            = this.Attributes;
			copy.GroupIndex            = this.GroupIndex;

			copy.DisplayModes          = this.DisplayModes;
			copy.TextForPrimaryPrice   = this.TextForPrimaryPrice;
			copy.TextForResultingPrice = this.TextForResultingPrice;
			copy.TextForDiscount       = this.TextForDiscount;

			if (this.Discount.IsNotNull ())
			{
				copy.Discount = this.Discount.CloneEntity (businessContext);
			}

			copy.PriceBeforeTax1       = this.PriceBeforeTax1;
			copy.PriceBeforeTax2       = this.PriceBeforeTax2;
			copy.PriceAfterTax1        = this.PriceAfterTax1;
			copy.PriceAfterTax2        = this.PriceAfterTax2;
		}

		#endregion
	}
}

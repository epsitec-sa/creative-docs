//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PriceDocumentItemEntity
	{
		public override DocumentItemTabId TabId
		{
			get
			{
				return DocumentItemTabId.Price;
			}
		}

		public override FormattedText GetCompactSummary()
		{
			var builder = new TextBuilder ();

			builder.Append ("Sous-total ");
			builder.Append (Misc.PriceToString (this.ResultingPriceBeforeTax));

			if (this.Discount.DiscountRate.HasValue)
			{
				builder.Append (" (après rabais en %)");
			}
			else if (this.Discount.DiscountAmount.HasValue)
			{
				builder.Append (" (après rabais en francs)");
			}
			else if (this.FixedPriceAfterTax.HasValue)
			{
				builder.Append (" (montant arrêté)");
			}

			return builder.ToFormattedText ();
		}
	}
}

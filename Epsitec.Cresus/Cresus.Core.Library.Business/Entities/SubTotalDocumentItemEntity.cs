//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class SubTotalDocumentItemEntity
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
			else if (this.Discount.Value.HasValue)
			{
				builder.Append (" (après rabais en francs)");
			}
			else if (this.FixedPrice.HasValue)
			{
				builder.Append (" (montant arrêté)");
			}

			return builder.ToFormattedText ();
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}

		public override void Process(IDocumentPriceCalculator priceCalculator)
		{
			priceCalculator.Process (new SubTotalPriceCalculator (priceCalculator.Data, priceCalculator.Document, this));
		}
	}
}

//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class EndTotalDocumentItemEntity : ICopyableEntity<EndTotalDocumentItemEntity>
	{
		public override FormattedText GetCompactSummary()
		{
			var desc = this.TextForPrice;

			if (desc.IsNullOrEmpty ())
			{
				desc = "Grand total";
			}

			string total;

			if (this.FixedPriceAfterTax.HasValue)
			{
				total = Misc.PriceToString (this.FixedPriceAfterTax);
			}
			else
			{
				total = Misc.PriceToString (this.PriceAfterTax);
			}

			var text = TextFormatter.FormatText (desc, total);

			if (text.IsNullOrEmpty ())
			{
				return "<i>Grand total</i>";
			}
			else
			{
				return text;
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}

		#region ICloneable<EndTotalDocumentItemEntity> Members

		void ICopyableEntity<EndTotalDocumentItemEntity>.CopyTo(BusinessContext businessContext, EndTotalDocumentItemEntity copy)
		{
			copy.Attributes         = this.Attributes;
			copy.GroupIndex         = this.GroupIndex;

			copy.TextForPrice       = this.TextForPrice;
			copy.TextForFixedPrice  = this.TextForFixedPrice;
			copy.PriceBeforeTax     = this.PriceBeforeTax;
			copy.PriceAfterTax      = this.PriceAfterTax;
			copy.TotalRounding		= this.TotalRounding;
			copy.FixedPriceAfterTax = this.FixedPriceAfterTax;
		}

		#endregion
	}
}

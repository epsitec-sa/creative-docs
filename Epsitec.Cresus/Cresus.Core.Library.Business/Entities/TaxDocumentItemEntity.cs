//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public partial class TaxDocumentItemEntity : ICopyableEntity<TaxDocumentItemEntity>
	{
		public override FormattedText GetCompactSummary()
		{
			var desc = this.Text;
			var tax = Misc.PriceToString (this.ResultingTax);

			var text = TextFormatter.FormatText (desc, tax);

			if (text.IsNullOrEmpty)
			{
				return "<i>TVA</i>";
			}
			else
			{
				return TextFormatter.FormatText (string.Format ("{0:0.0%} :", this.Rate), text);
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}

		#region ICloneable<TaxDocumentItemEntity> Members

		void ICopyableEntity<TaxDocumentItemEntity>.CopyTo(IBusinessContext businessContext, TaxDocumentItemEntity copy)
		{
			copy.Attributes    = this.Attributes;
			copy.GroupIndex    = this.GroupIndex;

			copy.Text          = this.Text;
			copy.VatCode       = this.VatCode;
			copy.BaseAmount    = this.BaseAmount;
			copy.Rate          = this.Rate;
			copy.ResultingTax  = this.ResultingTax;
		}

		#endregion
	}
}

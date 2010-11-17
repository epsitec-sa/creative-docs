//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BillingDetailEntity
	{
		public FormattedText GetCompactSummary(BusinessDocumentEntity invoiceDocument)
		{
			string amount = Misc.PriceToString (this.AmountDue.Amount);
			FormattedText title = Misc.FirstLine (this.Text);
			FormattedText ratio = Helpers.InvoiceDocumentHelper.GetInstalmentName (invoiceDocument, this, true);

			if (ratio.IsNullOrWhiteSpace)
			{
				return TextFormatter.FormatText (amount, title);
			}
			else
			{
				return TextFormatter.FormatText (amount, ratio, title);
			}
		}

		public override EntityStatus GetEntityStatus ()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Text.GetEntityStatus ());
				a.Accumulate (this.TransactionId.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.InstalmentName.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}

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
		public FormattedText GetCompactSummary(InvoiceDocumentEntity invoiceDocument)
		{
			string amount = Misc.PriceToString (this.AmountDue.Amount);
			FormattedText title = Misc.FirstLine (this.Title);
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

		public override EntityStatus EntityStatus
		{
			get
			{
				var s1 = EntityStatusHelper.GetStatus (this.Title);
				var s2 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.TransactionId));
				var s3 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.InstalmentName));

				return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3);
			}
		}
	}
}

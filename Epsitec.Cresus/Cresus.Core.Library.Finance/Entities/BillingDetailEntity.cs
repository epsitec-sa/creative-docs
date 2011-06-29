//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BillingDetailEntity
	{
		public override FormattedText GetCompactSummary()
		{
			string amount = Misc.PriceToString (this.AmountDue.Amount);
			FormattedText title = this.Text.Lines.FirstOrDefault ();
			FormattedText ratio = FormattedText.Empty; //Helpers.InvoiceDocumentHelper.GetInstalmentName (invoiceDocument, this, true);

			if (ratio.IsNullOrWhiteSpace)
			{
				return TextFormatter.FormatText (amount, title);
			}
			else
			{
				return TextFormatter.FormatText (amount, ratio, title);
			}
		}

		public int? InstalmentRankForTile
		{
			get
			{
				if (this.InstalmentRank == null)
				{
					return null;
				}
				else
				{
					return this.InstalmentRank+1;
				}
			}
			set
			{
				if (value == null)
				{
					this.InstalmentRank = null;
				}
				else
				{
					this.InstalmentRank = value-1;
				}
			}
		}

		public override EntityStatus GetEntityStatus()
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

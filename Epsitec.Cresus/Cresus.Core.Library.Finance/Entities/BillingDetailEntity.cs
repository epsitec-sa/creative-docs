//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

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
	public partial class CurrencyEntity
	{
		public override string[] GetEntityKeywords()
		{
			return EnumKeyValues.GetEnumKeyValue (this.CurrencyCode).Values.Select (x => x.ToString ()).ToArray ();
		}
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.CurrencyCode);
		}
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.CurrencyCode, "\n",
				"Du", this.BeginDate, "—", TextFormatter.Command.IfEmpty, "au", this.EndDate, "—", TextFormatter.Command.IfEmpty, "\n",
				this.ExchangeRate, TextFormatter.FormatCommand ("#string {0:0.00000}"), "CHF →", this.ExchangeRateBase, this.CurrencyCode);
		}
	}
	public partial class ExchangeRateSourceEntity
	{
		public override string[] GetEntityKeywords()
		{
			List<string> keywords = new List<string> ();

			keywords.Add (string.IsNullOrWhiteSpace (this.Originator) ? "—" : this.Originator);
			keywords.AddRange (EnumKeyValues.GetEnumKeyValue (this.Type).Values.Select (x => x.ToString ()));

			return keywords.ToArray ();
		}
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Type, "\n", this.Originator);
		}
	}
}

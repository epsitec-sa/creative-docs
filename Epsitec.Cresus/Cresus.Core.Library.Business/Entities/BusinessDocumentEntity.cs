//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BusinessDocumentEntity : ICopyableEntity<BusinessDocumentEntity>
	{
		public override FormattedText GetSummary()
		{
			FormattedText billing  = BusinessDocumentEntity.GetShortMailContactSummary (this.BillToMailContact);
			FormattedText shipping = BusinessDocumentEntity.GetShortMailContactSummary (this.ShipToMailContact);

			FormattedText addresses;
			if (this.BillToMailContact == this.ShipToMailContact || (!this.BillToMailContact.IsNotNull () && !this.ShipToMailContact.IsNotNull ()))
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation et de livraison:</b>\n", billing);
			}
			else
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation:</b>\n", billing, "\n\n<b>• Adresse de livraison:</b>\n", shipping);
			}

			return TextFormatter.FormatText (
				this.BillingDate, ", ",
				InvoiceDocumentHelper.GetTotalPriceTTC (this), TextFormatter.FormatCommand ("#price()"), "\n",
				addresses);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("???");//"Facture n°", this.IdA);
		}


		private static FormattedText GetShortMailContactSummary(MailContactEntity x)
		{
			if (x.IsNotNull ())
			{
				return TextFormatter.FormatText (x.LegalPerson.Name, "\n",
												 x.NaturalPerson.Firstname, x.NaturalPerson.Lastname, "\n",
												 x.Address.Street.StreetName, "\n",
												 x.Address.Location.PostalCode, x.Address.Location.Name);
			}
			else
			{
				return TextFormatter.FormatText ("Pas encore défini").ApplyItalic ();
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				//a.Accumulate (this.IdA.GetEntityStatus ());
				//a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				//a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				//a.Accumulate (this.DocumentTitle.GetEntityStatus ());
				a.Accumulate (/*EntityStatus.Empty | */EntityStatus.Valid); // this.Description.GetEntityStatus ();
				a.Accumulate (this.Lines.Select (x => x.GetEntityStatus ()));
				//a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}

		#region ICopyableEntity<BusinessDocumentEntity> Members

		void ICopyableEntity<BusinessDocumentEntity>.CopyTo(IBusinessContext businessContext, BusinessDocumentEntity copy)
		{
			copy.BaseDocumentCode      = this.Code;
			copy.BillToMailContact     = this.BillToMailContact;
			copy.ShipToMailContact     = this.ShipToMailContact;
			copy.OtherPartyRelation    = this.OtherPartyRelation;
			copy.OtherPartyBillingMode = this.OtherPartyBillingMode;
			copy.OtherPartyTaxMode     = this.OtherPartyTaxMode;
			
			copy.Lines.AddRange (this.Lines.Select (x => x.CloneEntity (businessContext)));

			//	BillingDetails is not copied, since it is really specific to one invoice, and
			//	should not be shared between different invoices.

			copy.BillingStatus         = this.BillingStatus;
			copy.BillingDate           = this.BillingDate;
			copy.CurrencyCode          = this.CurrencyCode;
			copy.PriceRefDate          = this.PriceRefDate;
			copy.PriceGroup            = this.PriceGroup;
			copy.DebtorBookAccount     = this.DebtorBookAccount;
		}

		#endregion
	}
}

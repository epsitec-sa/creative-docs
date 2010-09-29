//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class InvoiceDocumentEntity
	{
		public FormattedText GetSummary()
		{
			string date = Misc.GetDateTimeShortDescription (this.LastModificationDate);
			string total = Misc.PriceToString (Helpers.InvoiceDocumentHelper.GetTotalPriceTTC (this));

			FormattedText billing  = InvoiceDocumentEntity.GetShortMailContactSummary (this.BillingMailContact);
			FormattedText shipping = InvoiceDocumentEntity.GetShortMailContactSummary (this.ShippingMailContact);

			FormattedText addresses;
			if (this.BillingMailContact == this.ShippingMailContact || (!this.BillingMailContact.IsNotNull () && !this.ShippingMailContact.IsNotNull ()))
			{
				addresses = FormattedText.Concat ("\n\n<b>• Adresse de facturation et de livraison:</b>\n", billing);
			}
			else
			{
				addresses = FormattedText.Concat ("\n\n<b>• Adresse de facturation:</b>\n", billing, "\n\n<b>• Adresse de livraison:</b>\n", shipping);
			}

			return TextFormatter.FormatText ("N°", this.IdA, "/~", this.IdB, "/~", this.IdC, ", ", date, ", ", total, addresses);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("N°", this.IdA);
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
				return Misc.Italic ("Pas encore défini");
			}
		}
	}
}

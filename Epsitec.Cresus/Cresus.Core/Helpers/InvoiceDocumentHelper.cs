//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class InvoiceDocumentHelper
	{
		public static FormattedText GetText(InvoiceDocumentEntity x)
		{
			string date = InvoiceDocumentHelper.GetDate (x);
			string total = Misc.DecimalToString (InvoiceDocumentHelper.GetTotal (x));

			var builder = new System.Text.StringBuilder ();

			foreach (var line in x.Lines)
			{
				if (line.Visibility)
				{
					if (line is ArticleDocumentItemEntity)
					{
						var quantity = ArticleDocumentItemHelper.GetArticleQuantity (line as ArticleDocumentItemEntity);
						var desc = Misc.FirstLine (ArticleDocumentItemHelper.GetArticleDescription (line as ArticleDocumentItemEntity));

						builder.Append ("● ");
						builder.Append (string.Join (" ", quantity, desc));
						builder.Append ("<br/>");
					}
				}
			}

			return UIBuilder.FormatText ("N°", x.IdA, "/~", x.IdB, "/~", x.IdC, ", ", date, "\n", builder.ToString (), "Total: ", total);
		}


		public static string GetDate(InvoiceDocumentEntity x)
		{
			System.DateTime date;
			if (x.LastModificationDate.HasValue)
			{
				date = x.LastModificationDate.Value;
			}
			else
			{
				date = System.DateTime.Now;
			}

			return Misc.GetDateTimeDescription (date);
		}

		public static string GetMailContact(InvoiceDocumentEntity x)
		{
			string legal = "";
			string natural = "";

			if (x.BillingMailContact != null)
			{
				if (x.BillingMailContact.LegalPerson.IsActive ())
				{
					var y = x.BillingMailContact.LegalPerson;
					legal = UIBuilder.FormatText (y.Name).ToString ();
				}

				if (x.BillingMailContact.NaturalPerson.IsActive ())
				{
					var y = x.BillingMailContact.NaturalPerson;
					natural = UIBuilder.FormatText (y.Title.Name, "~\n", y.Firstname, y.Lastname).ToString ();
				}

				return UIBuilder.FormatText (legal, "~\n", natural, "~\n", x.BillingMailContact.Address.Street.StreetName, "\n", x.BillingMailContact.Address.Location.PostalCode, x.BillingMailContact.Address.Location.Name).ToString ();
			}

			return null;
		}

		public static string GetConcerne(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return x.BillingDetails[0].Title;
			}

			return null;
		}

		public static string GetConditions(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return x.BillingDetails[0].AmountDue.PaymentMode.Description;
			}

			return null;
		}

		public static decimal GetTotal(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return Misc.CentRound (x.BillingDetails[0].AmountDue.Amount);
			}

			return 0;
		}

		public static string GetTitle(InvoiceDocumentEntity x)
		{
			return UIBuilder.FormatText ("<b>Facture", x.IdA, "/~", x.IdB, "/~", x.IdC, "</b>").ToString ();
		}
	}
}

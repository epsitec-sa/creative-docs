﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static FormattedText GetSummary(InvoiceDocumentEntity x)
		{
			string date = InvoiceDocumentHelper.GetDate (x);
			string total = Misc.PriceToString (InvoiceDocumentHelper.GetAmontDue (x));

			var builder = new System.Text.StringBuilder ();

			foreach (var line in x.Lines)
			{
				if (line.Visibility)
				{
					if (line is ArticleDocumentItemEntity)
					{
						var quantity = ArticleDocumentItemHelper.GetArticleQuantityAndUnit (line as ArticleDocumentItemEntity);
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


		public static string GetConditions(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return x.BillingDetails[0].AmountDue.PaymentMode.Description;
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

		public static void SetConcerne(InvoiceDocumentEntity x, DataLayer.DataContext dataContext, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				if (x.BillingDetails.Count != 0)
				{
					x.BillingDetails[0].Title = null;
				}
			}
			else
			{
				if (x.BillingDetails.Count == 0)
				{
					var billing = dataContext.CreateEmptyEntity<BillingDetailsEntity> ();
					x.BillingDetails.Add (billing);
				}

				x.BillingDetails[0].Title = value;
			}
		}


		public static decimal GetAmontDue(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return Misc.PriceRound (x.BillingDetails[0].AmountDue.Amount);
			}

			return 0;
		}

		public static void SetAmontDue(InvoiceDocumentEntity x, DataLayer.DataContext dataContext, decimal value)
		{
			if (x.BillingDetails.Count == 0)
			{
				var billing = dataContext.CreateEmptyEntity<BillingDetailsEntity> ();
				x.BillingDetails.Add (billing);
			}

			x.BillingDetails[0].AmountDue.Amount = value;
		}


		public static string GetTitle(InvoiceDocumentEntity x)
		{
			return UIBuilder.FormatText ("<b>Facture", x.IdA, "/~", x.IdB, "/~", x.IdC, "</b>").ToString ();
		}


		public static void UpdatePrices(InvoiceDocumentEntity x, DataLayer.DataContext dataContext)
		{
			//	Recalcule complètement une facture.
			var decimalType = DecimalType.Default;

			decimal vatRate = 0.076M;  // TODO: Cette valeur ne devrait pas tomber du ciel !
			decimal primaryTotalBeforeTax = 0;
			decimal amontDue = 0;

			foreach (var line in x.Lines)
			{
				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					ArticleDocumentItemHelper.UpdatePrices (article);

					primaryTotalBeforeTax += article.PrimaryLinePriceBeforeTax;
				}

				if (line is PriceDocumentItemEntity)
				{
					var price = line as PriceDocumentItemEntity;

					price.PrimaryPriceBeforeTax = primaryTotalBeforeTax;
					price.FinalPriceBeforeTax   = primaryTotalBeforeTax;

					price.PrimaryTax   = price.FinalPriceBeforeTax * vatRate;
					price.ResultingTax = price.FinalPriceBeforeTax * vatRate;

					if (price.Discount.IsActive ())  // rabais ?
					{
						price.ResultingPriceBeforeTax = price.PrimaryPriceBeforeTax * (1.0M - price.Discount.DiscountRate);
					}
					else  // total ?
					{
						price.ResultingPriceBeforeTax = decimalType.Range.ConstrainToZero (price.FixedPriceAfterTax / (1 + vatRate));
						price.FixedPriceAfterTax = (int) (price.PrimaryPriceBeforeTax * (1+vatRate) / 10) * 10M;

						amontDue = price.FixedPriceAfterTax.Value;
					}
				}
			}

			SetAmontDue (x, dataContext, amontDue);
		}


		public static void UpdateDialogs(InvoiceDocumentEntity x)
		{
			//	Met à jour le ou les dialogues d'aperçu avant impression ouverts.
			foreach (var dialog in CoreProgram.Application.AttachedDialogs)
			{
				foreach (var entity in dialog.Entities)
				{
					if (entity is InvoiceDocumentEntity)
					{
						var invoiceDocument = entity as InvoiceDocumentEntity;
						if (invoiceDocument == x)
						{
							dialog.Update ();
							break;
						}
					}
				}
			}
		}
	}
}

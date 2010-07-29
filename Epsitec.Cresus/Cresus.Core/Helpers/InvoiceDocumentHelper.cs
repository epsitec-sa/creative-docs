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
		public static FormattedText GetSummary(InvoiceDocumentEntity x)
		{
			string date = InvoiceDocumentHelper.GetDate (x);
			string total = Misc.PriceToString (InvoiceDocumentHelper.GetTotalPrice (x));

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


		public static string GetTitle(InvoiceDocumentEntity x, BillingDetailsEntity billingDetails, bool isBL)
		{
			string text = isBL ? "<b>Bulletin de livraison" : "<b>Facture";
			string title = UIBuilder.FormatText (text, x.IdA, "/~", x.IdB, "/~", x.IdC, "</b>").ToString ();

			return string.Concat (title, " ", InvoiceDocumentHelper.GetRatio (x, billingDetails, isBL));
		}

		public static string GetRatio(InvoiceDocumentEntity x, BillingDetailsEntity billingDetails, bool isBL)
		{
			int rank = 0;
			int count = 0;

			if (billingDetails != null && x.BillingDetails.Count > 1)
			{
				foreach (var d in x.BillingDetails)
				{
					if (d == billingDetails)
					{
						rank = count;
					}

					if (d.AmountDue.PaymentMode.Code == billingDetails.AmountDue.PaymentMode.Code)
					{
						count++;
					}
				}
			}

			if (count > 1)
			{
				return string.Format ("({0}/{1})", (rank+1).ToString (), count.ToString ());
			}

			return null;
		}


		public static void UpdatePrices(InvoiceDocumentEntity x, DataLayer.Context.DataContext dataContext)
		{
			//	Recalcule complètement une facture.
			//	
			//	Une ligne de total (PriceDocumentItemEntity) effectue un sous-total de tout ce qui précède,
			//	depuis le sous-total précédent. Sauf pour la dernière ligne qui effectue toujours un total complet:
			//	
			//	  Article A
			//	  Article B
			//	Total (A+B), en fait, sous-total de A+B
			//	  Article C
			//	  Article D
			//	Total (C+D), en fait, sous-total de C+D
			//	  Article E
			//	Total (A+B+C+D+E), en fait, grand total

			// TODO: Valider cela avec Pierre !

			decimal vatRate = 0.076M;  // TODO: Cette valeur ne devrait pas tomber du ciel !
			decimal primaryTotalBeforeTax    = 0;
			decimal primaryTotalTax          = 0;
			decimal primarySubtotalBeforeTax = 0;
			decimal primarySubtotalTax       = 0;
			decimal discountRate = 1;

			for (int i=0; i<x.Lines.Count; i++)
			{
				var line = x.Lines[i];
				bool isLastLine = (i == x.Lines.Count-1);

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					ArticleDocumentItemHelper.UpdatePrices (article);

					primarySubtotalBeforeTax += article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
					primarySubtotalTax       += article.ResultingLineTax.GetValueOrDefault (0);
				}

				if (line is PriceDocumentItemEntity)
				{
					var price = line as PriceDocumentItemEntity;

					//	Calcule PrimaryPriceBeforeTax et PrimaryTax, les prix sans rabais.
					if (isLastLine == false)  // sous-total ?
					{
						price.PrimaryPriceBeforeTax = primarySubtotalBeforeTax;
						price.PrimaryTax            = primarySubtotalTax;
					}
					else  // dernière ligne (grand total) ?
					{
						price.PrimaryPriceBeforeTax = primaryTotalBeforeTax + primarySubtotalBeforeTax;
						price.PrimaryTax            = primaryTotalTax       + primarySubtotalTax;
					}

					//	Calcule ResultingPriceBeforeTax et ResultingTax, les prix après rabais.
					if (price.FixedPriceAfterTax.HasValue)  // valeur imposée ?
					{
						price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.FixedPriceAfterTax.Value / (1.0M + vatRate));
						price.ResultingTax            = Misc.PriceConstrain (price.ResultingPriceBeforeTax.Value * vatRate);
					}
					else if (price.FixedPriceBeforeTax.HasValue)  // valeur imposée ?
					{
						price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.FixedPriceBeforeTax);
						price.ResultingTax            = Misc.PriceConstrain (price.ResultingPriceBeforeTax.GetValueOrDefault (0) * vatRate);
					}
					else
					{
						if (price.Discount.DiscountRate.HasValue || price.Discount.DiscountAmount.HasValue)  // rabais ?
						{
							if (price.Discount.DiscountRate.HasValue)
							{
								price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) * (1.0M - price.Discount.DiscountRate.GetValueOrDefault (0)));
							}
							else
							{
								price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) - price.Discount.DiscountAmount.GetValueOrDefault (0));
							}

							price.ResultingTax = Misc.PriceConstrain (price.ResultingPriceBeforeTax.GetValueOrDefault (0) * vatRate);
						}
						else
						{
							price.ResultingPriceBeforeTax = price.PrimaryPriceBeforeTax;
							price.ResultingTax            = price.PrimaryTax;
						}
					}

					//	Génère les textes appropriés.
					if (price.Discount.DiscountRate.HasValue || price.Discount.DiscountAmount.HasValue || price.FixedPriceAfterTax.HasValue)  // rabais ?
					{
						price.TextForPrimaryPrice   = isLastLine ? "Total avant rabais"  : "Sous-total avant rabais";
						price.TextForResultingPrice = isLastLine ? "<b>Total arrêté</b>" : "Sous-total après rabais";
					}
					else
					{
						price.TextForPrimaryPrice   = null;
						price.TextForResultingPrice = isLastLine ? "<b>Total arrêté</b>" : "Sous-total";
					}

					//	Sous-totaux et totaux.
					primaryTotalBeforeTax += price.ResultingPriceBeforeTax.GetValueOrDefault (0);
					primaryTotalTax       += price.ResultingTax.GetValueOrDefault (0);

					primarySubtotalBeforeTax = 0;
					primarySubtotalTax       = 0;
				}
			}

			InvoiceDocumentHelper.BackwardUpdatePrices (x, dataContext, discountRate);
		}

		private static void BackwardUpdatePrices(InvoiceDocumentEntity x, DataLayer.Context.DataContext dataContext, decimal discountRate)
		{
			//	Si le prix arrêté (FixedPriceAfterTax) est plus petit que le total réel, on doit
			//	appliquer les rabais à l'envers en remontant du pied de la facture pour arriver
			//	aux articles, afin de savoir à quel prix réel chaque article a été vendu; ces
			//	infos de 'remontée' sont alors stockées dans FinalPriceBeforeTax et FinalTax et
			//	servent à la comptabilisation uniquement, mais pas à l'impression d'une facture.
			for (int i=0; i<x.Lines.Count; i++)
			{
				var line = x.Lines[i];
				bool isLastLine = (i == x.Lines.Count-1);

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					article.FinalLinePriceBeforeTax = Misc.PriceConstrain (article.ResultingLinePriceBeforeTax * discountRate);
					article.FinalLineTax            = Misc.PriceConstrain (article.ResultingLineTax            * discountRate);
				}

				if (line is PriceDocumentItemEntity)
				{
					var price = line as PriceDocumentItemEntity;

					if (isLastLine == false)  // sous-total ?
					{
						price.FinalPriceBeforeTax = Misc.PriceConstrain (price.ResultingPriceBeforeTax * discountRate);
						price.FinalTax            = Misc.PriceConstrain (price.ResultingTax            * discountRate);
					}
					else  // dernière ligne (grand total) ?
					{
						// tout est déjà calculé
					}
				}
			}
		}


		public static bool HasAmount(PriceDocumentItemEntity price)
		{
			if (price.Discount.DiscountRate.HasValue)
			{
				return true;
			}

			if (price.Discount.DiscountAmount.HasValue)
			{
				return true;
			}

			if (price.FixedPriceAfterTax.HasValue || price.FixedPriceBeforeTax.HasValue)
			{
				return true;
			}

			return false;
		}

		public static string GetAmount(PriceDocumentItemEntity price)
		{
			if (price.Discount.DiscountRate.HasValue)
			{
				return Misc.PercentToString (price.Discount.DiscountRate);
			}

			if (price.Discount.DiscountAmount.HasValue)
			{
				return Misc.PriceToString (price.Discount.DiscountAmount);
			}

			if (price.FixedPriceAfterTax.HasValue || price.FixedPriceBeforeTax.HasValue)
			{
				return Misc.PriceToString (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) - price.ResultingPriceBeforeTax.GetValueOrDefault (0));
			}

			return null;
		}

		public static decimal? GetTotalPrice(InvoiceDocumentEntity x)
		{
			if (x.Lines.Count > 0)
			{
				var lastLine = x.Lines.Last ();

				if (lastLine is PriceDocumentItemEntity)
				{
					var price = lastLine as PriceDocumentItemEntity;

					return Misc.PriceConstrain (price.ResultingPriceBeforeTax.GetValueOrDefault (0) + price.ResultingTax.GetValueOrDefault (0));
				}
			}

			return null;
		}

	
#if false
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
#endif
	}
}

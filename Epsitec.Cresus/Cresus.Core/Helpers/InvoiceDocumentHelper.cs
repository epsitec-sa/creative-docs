﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class InvoiceDocumentHelper
	{
		public static FormattedText GetSummary(InvoiceDocumentEntity x)
		{
			string date = Misc.GetDateTimeShortDescription (x.LastModificationDate);
			string total = Misc.PriceToString (InvoiceDocumentHelper.GetTotalPriceTTC (x));

			FormattedText billing  = GetShortMailContactSummary (x.BillingMailContact);
			FormattedText shipping = GetShortMailContactSummary (x.ShippingMailContact);

			FormattedText addresses;
			if (x.BillingMailContact == x.ShippingMailContact || (!x.BillingMailContact.IsActive () && !x.ShippingMailContact.IsActive ()))
			{
				addresses = FormattedText.Concat ("\n\n<b>• Adresse de facturation et de livraison:</b>\n", billing);
			}
			else
			{
				addresses = FormattedText.Concat ("\n\n<b>• Adresse de facturation:</b>\n", billing, "\n\n<b>• Adresse de livraison:</b>\n", shipping);
			}

			return TextFormatter.FormatText ("N°", x.IdA, "/~", x.IdB, "/~", x.IdC, ", ", date, ", ", total, addresses);
		}

		private static FormattedText GetShortMailContactSummary(MailContactEntity x)
		{
			if (x.IsActive ())
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


		public static FormattedText GetMailContact(InvoiceDocumentEntity x)
		{
			FormattedText legal = "";
			FormattedText natural = "";

			if (x.BillingMailContact != null)
			{
				if (x.BillingMailContact.LegalPerson.IsActive ())
				{
					var y = x.BillingMailContact.LegalPerson;
					legal = TextFormatter.FormatText (y.Name);
				}

				if (x.BillingMailContact.NaturalPerson.IsActive ())
				{
					var y = x.BillingMailContact.NaturalPerson;
					natural = TextFormatter.FormatText (y.Title.Name, "~\n", y.Firstname, y.Lastname);
				}

				return TextFormatter.FormatText (legal, "~\n", natural, "~\n", x.BillingMailContact.Address.Street.StreetName, "\n", x.BillingMailContact.Address.Location.PostalCode, x.BillingMailContact.Address.Location.Name);
			}

			return FormattedText.Null;
		}


		public static FormattedText GetTitle(InvoiceDocumentEntity x, BillingDetailEntity billingDetails, Printers.DocumentType type)
		{
			string doc;

			switch (type)
			{
				case Printers.DocumentType.BL:
					doc = "Bulletin de livraison";
					break;

				case Printers.DocumentType.ProductionOrder:
					doc = "Ordre de production";
					break;

				case Printers.DocumentType.Offer:
					doc = "Offre";
					break;

				case Printers.DocumentType.Order:
					doc = "Commande";
					break;

				case Printers.DocumentType.OrderAcknowledge:
					doc = "Confirmation de commande";
					break;

				default:
					doc = "Facture";
					break;
			}

			string title = TextFormatter.FormatText (string.Concat("<b>", doc), x.IdA, "/~", x.IdB, "/~", x.IdC, "</b>").ToString ();

			return FormattedText.Concat (title, " ", InvoiceDocumentHelper.GetInstalmentName (x, billingDetails, true));
		}

		public static FormattedText GetInstalmentName(InvoiceDocumentEntity x, BillingDetailEntity billingDetails, bool parenthesis)
		{
			//	Retourne la description d'une mensualité. Si aucun texte n'est défini, il est généré automatiquement,
			//	sur le modèle "n/t", où n est le rang de la mensualité et t le nombre total.
			if (billingDetails == null)
			{
				return null;
			}

			if (billingDetails != null && billingDetails.InstalmentRank == null)
			{
				return null;
			}

			var instalmentName = TextFormatter.FormatText (billingDetails.InstalmentName);

			if (!instalmentName.IsNullOrEmpty)
			{
				if (parenthesis)
				{
					return TextFormatter.FormatText ("(", instalmentName, ")");
				}
				else
				{
					return instalmentName;
				}
			}

			int count = x.BillingDetails.Count (y => y.InstalmentRank != null);  // compte les mensualités

			if (parenthesis)
			{
				return string.Format ("({0}/{1})", (billingDetails.InstalmentRank+1).ToString (), count.ToString ());
			}
			else
			{
				return string.Format ("{0}/{1}", (billingDetails.InstalmentRank+1).ToString (), count.ToString ());
			}
		}


		public static int GetUserLinesCount(InvoiceDocumentEntity x)
		{
			//	Retourne le nombre de lignes gérables par l'utilisateur.
			return x.Lines.Count (y => (y is TextDocumentItemEntity || y is ArticleDocumentItemEntity || y is PriceDocumentItemEntity));
		}

		public static void UpdatePrices(InvoiceDocumentEntity x, DataContext dataContext)
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

			InvoiceDocumentHelper.UpdateAutoLines (x, dataContext);

			decimal vatRate = 0.076M;  // TODO: Cette valeur ne devrait pas tomber du ciel !
			decimal primaryTotalBeforeTax    = 0;
			decimal primaryTotalTax          = 0;
			decimal primarySubtotalBeforeTax = 0;
			decimal primarySubtotalTax       = 0;
			decimal discountRate = 1;

			int totalRank = 0;

			for (int i=0; i<x.Lines.Count; i++)
			{
				var line = x.Lines[i];

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					ArticleDocumentItemHelper.UpdatePrices (x, article);

					primarySubtotalBeforeTax += article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
					primarySubtotalTax       += article.ResultingLineTax.GetValueOrDefault (0);
				}

				if (line is PriceDocumentItemEntity)
				{
					var price = line as PriceDocumentItemEntity;

					//	Calcule PrimaryPriceBeforeTax et PrimaryTax, les prix sans rabais.
					price.PrimaryPriceBeforeTax = primarySubtotalBeforeTax;
					price.PrimaryTax            = primarySubtotalTax;

					//	Calcule ResultingPriceBeforeTax et ResultingTax, les prix après rabais.
					if (price.FixedPriceAfterTax.HasValue)  // valeur imposée ?
					{
						//	Utiliser une règle de 3 : ht final = ttc final / ttc calculé * ht calculé
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

							//	règle de 3
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
						price.TextForPrimaryPrice   = "Sous-total avant rabais";
						price.TextForResultingPrice = "Sous-total après rabais";
					}
					else
					{
						price.TextForPrimaryPrice   = null;
						price.TextForResultingPrice = "Sous-total";
					}

					//	Sous-totaux et totaux.
					primaryTotalBeforeTax += price.ResultingPriceBeforeTax.GetValueOrDefault (0);
					primaryTotalTax       += price.ResultingTax.GetValueOrDefault (0);

					primarySubtotalBeforeTax = 0;
					primarySubtotalTax       = 0;
				}

				if (line is TotalDocumentItemEntity)
				{
					var total = line as TotalDocumentItemEntity;

					if (totalRank == 0)  // ligne de total HT ?
					{
						total.PrimaryPriceBeforeTax = primaryTotalBeforeTax + primarySubtotalBeforeTax;
						total.TextForPrimaryPrice = "Total HT";
					}
					else  // ligne de total TTC ?
					{
						total.PrimaryPriceAfterTax = primaryTotalBeforeTax + primaryTotalTax + primarySubtotalBeforeTax + primarySubtotalTax;

						if (total.FixedPriceAfterTax.HasValue)
						{
							total.TextForPrimaryPrice = Misc.Bold ("Total TTC");
							total.TextForFixedPrice   = Misc.Bold (Misc.Italic ("Total arrêté à"));
						}
						else
						{
							total.TextForPrimaryPrice = Misc.Bold ("Total TTC");
							total.TextForFixedPrice   = null;
						}
					}

					totalRank++;
				}
			}

			InvoiceDocumentHelper.BackwardUpdatePrices(x, discountRate);
		}

		private static void BackwardUpdatePrices(InvoiceDocumentEntity x, decimal discountRate)
		{
			//	Si le prix arrêté (FixedPriceAfterTax) est plus petit que le total réel, on doit
			//	appliquer les rabais à l'envers en remontant du pied de la facture pour arriver
			//	aux articles, afin de savoir à quel prix réel chaque article a été vendu; ces
			//	infos de 'remontée' sont alors stockées dans FinalPriceBeforeTax et FinalTax et
			//	servent à la comptabilisation uniquement, mais pas à l'impression d'une facture.
			for (int i=0; i<x.Lines.Count; i++)
			{
				var line = x.Lines[i];

				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					article.FinalLinePriceBeforeTax = Misc.PriceConstrain (article.ResultingLinePriceBeforeTax * discountRate);
					article.FinalLineTax            = Misc.PriceConstrain (article.ResultingLineTax            * discountRate);
				}

				if (line is PriceDocumentItemEntity)
				{
					var price = line as PriceDocumentItemEntity;

					price.FinalPriceBeforeTax = Misc.PriceConstrain (price.ResultingPriceBeforeTax * discountRate);
					price.FinalTax            = Misc.PriceConstrain (price.ResultingTax            * discountRate);
				}
			}
		}

		private static void UpdateAutoLines(InvoiceDocumentEntity x, DataContext dataContext)
		{
			//	Met à jour toutes les lignes automatiques (taxes et total général).
			InvoiceDocumentHelper.CreateTotalLineTTC (x, dataContext);
			InvoiceDocumentHelper.UpdateTaxLines     (x, dataContext);
			InvoiceDocumentHelper.CreateTotalLineHT  (x, dataContext);
		}

		private static void UpdateTaxLines(InvoiceDocumentEntity x, DataContext dataContext)
		{
			//	Met à jour les lignes de taxe, placées juste avant le total général.

			// Cherche toutes les catégories utilisées par les articles.
			var categories = new List<ArticleCategoryEntity> ();

			foreach (var line in x.Lines)
			{
				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;
					var category = article.ArticleDefinition.ArticleCategory;

					if (!categories.Contains (category))
					{
						categories.Add (category);
					}
				}
			}

			// Crée ou supprime des lignes de taxe pour en avoir juste le bon nombre.
			InvoiceDocumentHelper.CreateOrDeleteTaxLines(x, dataContext, categories.Count);

			// Met à jour les lignes de taxe.
			for (int i = 0; i < categories.Count; i++)
			{
				var category = categories[i];
				var tax = x.Lines[x.Lines.Count-1-categories.Count+i] as TaxDocumentItemEntity;

				tax.VatCode = category.DefaultOutputVatCode;

				decimal amount, rate;
				InvoiceDocumentHelper.GetArticleTotalAmoutForTax (x, category.Name, out amount, out rate);
				tax.BaseAmount = amount;
				tax.Rate = rate;

				tax.ResultingTax = tax.BaseAmount * tax.Rate;
				tax.Text = string.Format ("TVA {0} pour {1}", Misc.PercentToString (rate), category.Name);
			}
		}

		private static void GetArticleTotalAmoutForTax(InvoiceDocumentEntity x, string categoryName, out decimal amount, out decimal rate)
		{
			//	Calcule le montant total et le taux de tva, pour une catégorie, parmi tous les articles d'une facture.
			amount = 0;
			rate   = 0;

			foreach (var line in x.Lines)
			{
				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					if (article.ArticleDefinition.ArticleCategory.Name == categoryName)
					{
						amount += article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
						rate = ArticleDocumentItemHelper.GetArticleVatRate (x, article).GetValueOrDefault (0);
					}
				}
			}
		}

		private static void CreateOrDeleteTaxLines(InvoiceDocumentEntity x, DataContext dataContext, int requiredTotal)
		{
			//	Crée ou supprime des lignes de taxe pour en avoir juste le bon nombre.
			//	Depuis la fin, on a toujours le total, les taxes puis les articles.
			int currentTotal = x.Lines.Count (y => y is TaxDocumentItemEntity);

			if (currentTotal < requiredTotal)  // faut-il créer de nouvelles lignes de taxe ?
			{
				for (int i = 0; i < requiredTotal-currentTotal; i++)
				{
					var tax = dataContext.CreateEntity<TaxDocumentItemEntity> ();
					tax.Visibility = true;

					x.Lines.Insert (x.Lines.Count-1, tax);  // insère juste avant le total final
				}
			}

			if (currentTotal > requiredTotal)  // faut-il supprimer des lignes de taxe ?
			{
				for (int i = 0; i < currentTotal-requiredTotal; i++)
				{
					var tax = x.Lines.LastOrDefault (y => y is TaxDocumentItemEntity) as TaxDocumentItemEntity;

					x.Lines.Remove (tax);
					dataContext.DeleteEntity (tax);
				}
			}
		}

		private static void CreateTotalLineTTC(InvoiceDocumentEntity x, DataContext dataContext)
		{
			//	Crée si nécessaire la dernière ligne de total TTC.
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);
			if (total != null)
			{
				return;  // la dernière ligne de total existe déjà
			}

			// Crée la dernière ligne de total.
			var lastPrice = dataContext.CreateEntity<TotalDocumentItemEntity> ();
			lastPrice.Visibility = true;

			x.Lines.Add (lastPrice);
		}

		private static void CreateTotalLineHT(InvoiceDocumentEntity x, DataContext dataContext)
		{
			//	Crée si nécessaire la dernière ligne de total HT.
			var total = InvoiceDocumentHelper.GetTotalEntityHT (x);
			if (total != null)
			{
				return;  // la ligne de total existe déjà
			}

			// Crée la ligne de total.
			var lastPrice = dataContext.CreateEntity<TotalDocumentItemEntity> ();
			lastPrice.Visibility = true;

			int index = x.Lines.Count-2;  // avant la ligne de total TTC

			while (index >= 0)
			{
				var line = x.Lines[index];

				if (line is TaxDocumentItemEntity)
				{
					index--;
				}
				else
				{
					break;
				}
			}

			x.Lines.Insert (index+1, lastPrice);
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


		public static decimal GetVatTotal(InvoiceDocumentEntity x)
		{
			decimal total = 0;

			foreach (var line in x.Lines)
			{
				if (line is TaxDocumentItemEntity)
				{
					var tax = line as TaxDocumentItemEntity;

					total += tax.ResultingTax.GetValueOrDefault (0);
				}
			}

			return total;
		}

		public static decimal? GetTotalPriceTTC(InvoiceDocumentEntity x)
		{
			//	Retourne le prix total TTC d'une facture, en tenant compte du total arrêté s'il existe.
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				if (total.FixedPriceAfterTax.HasValue)
				{
					return total.FixedPriceAfterTax;
				}
				else
				{
					return total.PrimaryPriceAfterTax;
				}
			}

			return null;
		}

		public static decimal? GetPrimaryPriceTTC(InvoiceDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				return total.PrimaryPriceAfterTax;
			}

			return null;
		}

		public static decimal? GetFixedPriceTTC(InvoiceDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				return total.FixedPriceAfterTax;
			}

			return null;
		}

		public static void SetFixedPriceTTC(InvoiceDocumentEntity x, decimal? value)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				total.FixedPriceAfterTax = value;
			}
		}

		public static decimal? GetPrimaryPriceHT(InvoiceDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityHT (x);

			if (total != null)
			{
				return total.PrimaryPriceBeforeTax;
			}

			return null;
		}

		private static TotalDocumentItemEntity GetTotalEntityHT(InvoiceDocumentEntity x)
		{
			//	Retourne la ligne de total HT qui vient avant les lignes de TVA.
			int index = x.Lines.Count-2;  // avant la ligne de total TTC

			while (index >= 0)
			{
				var line = x.Lines[index--];

				if (line is TotalDocumentItemEntity)
				{
					return line as TotalDocumentItemEntity;
				}
			}

			return null;
		}

		private static TotalDocumentItemEntity GetTotalEntityTTC(InvoiceDocumentEntity x)
		{
			//	Retourne la ligne de total TTC qui doit obligatoirement terminer une liste.
			if (x.Lines.Count > 0)
			{
				var lastLine = x.Lines.Last ();

				if (lastLine is TotalDocumentItemEntity)
				{
					return lastLine as TotalDocumentItemEntity;
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

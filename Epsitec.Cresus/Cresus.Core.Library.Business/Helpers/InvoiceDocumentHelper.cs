//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static FormattedText GetTitle(DocumentMetadataEntity metadata, BusinessDocumentEntity x, BillingDetailEntity billingDetails)
		{
			//	Retourne le titre du document imprimé.
			//	Par exemple "Facture 10256", "Offre 10257" ou "Bon pour commande 10258".
			var doc = InvoiceDocumentHelper.GetDocumentName (metadata);
			var title = TextFormatter.FormatText (string.Concat ("<b>", doc), metadata.IdA, "/~", metadata.IdB, "/~", metadata.IdC, "</b>").ToString ();

			return FormattedText.Concat (title, " ", InvoiceDocumentHelper.GetInstalmentName (x, billingDetails, true));
		}

		public static FormattedText GetDocumentName(DocumentMetadataEntity metadata)
		{
			//	Retourne le nom du document imprimé.
			//	Par exemple "Facture", "Offre" ou "Bon pour commande".
			if (metadata.DocumentCategory.IsNull () ||
				metadata.DocumentCategory.Name.IsNullOrEmpty)
			{
				return "Document";  // nom générique
			}
			else
			{
				return metadata.DocumentCategory.Name;
			}
		}

		public static FormattedText GetInstalmentName(BusinessDocumentEntity x, BillingDetailEntity billingDetails, bool parenthesis)
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


		public static int GetUserLinesCount(BusinessDocumentEntity x)
		{
			//	Retourne le nombre de lignes gérables par l'utilisateur.
			return x.Lines.Count (y => (y is TextDocumentItemEntity || y is ArticleDocumentItemEntity || y is SubTotalDocumentItemEntity));
		}

#if false
		public static void UpdatePrices(BusinessDocumentEntity x, DataContext dataContext)
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
					primarySubtotalTax       += article.ResultingLineTax1.GetValueOrDefault (0);
				}

				if (line is SubTotalDocumentItemEntity)
				{
					var price = line as SubTotalDocumentItemEntity;

					//	Calcule PrimaryPriceBeforeTax et PrimaryTax, les prix sans rabais.
					price.PrimaryPriceBeforeTax = primarySubtotalBeforeTax;
					price.PrimaryTax            = primarySubtotalTax;

					//	Calcule ResultingPriceBeforeTax et ResultingTax, les prix après rabais.
					if (price.FixedPrice.HasValue)  // valeur imposée ?
					{
						//	Utiliser une règle de 3 : ht final = ttc final / ttc calculé * ht calculé
						price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.FixedPrice.Value / (1.0M + vatRate));
						price.ResultingTax            = Misc.PriceConstrain (price.ResultingPriceBeforeTax.Value * vatRate);
					}
					else if (price.FixedPrice.HasValue)  // valeur imposée ?
					{
						price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.FixedPrice);
						price.ResultingTax            = Misc.PriceConstrain (price.ResultingPriceBeforeTax.GetValueOrDefault (0) * vatRate);
					}
					else
					{
						if (price.Discount.DiscountRate.HasValue || price.Discount.Value.HasValue)  // rabais ?
						{
							if (price.Discount.DiscountRate.HasValue)
							{
								price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) * (1.0M - price.Discount.DiscountRate.GetValueOrDefault (0)));
							}
							else
							{
								price.ResultingPriceBeforeTax = Misc.PriceConstrain (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) - price.Discount.Value.GetValueOrDefault (0));
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
					if (price.Discount.DiscountRate.HasValue || price.Discount.Value.HasValue || price.FixedPrice.HasValue)  // rabais ?
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

				if (line is EndTotalDocumentItemEntity)
				{
					var total = line as EndTotalDocumentItemEntity;

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
#endif

		private static void BackwardUpdatePrices(BusinessDocumentEntity x, decimal discountRate)
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
//					article.FinalLineTax1           = Misc.PriceConstrain (article.ResultingLineTax1           * discountRate);
				}

				if (line is SubTotalDocumentItemEntity)
				{
					var price = line as SubTotalDocumentItemEntity;

					price.FinalPriceBeforeTax = Misc.PriceConstrain (price.ResultingPriceBeforeTax * discountRate);
				}
			}
		}

		private static void UpdateAutoLines(BusinessDocumentEntity x, DataContext dataContext)
		{
			//	Met à jour toutes les lignes automatiques (taxes et total général).
			InvoiceDocumentHelper.CreateTotalLineTTC (x, dataContext);
			InvoiceDocumentHelper.UpdateTaxLines     (x, dataContext);
			InvoiceDocumentHelper.CreateTotalLineHT  (x, dataContext);
		}

		private static void UpdateTaxLines(BusinessDocumentEntity x, DataContext dataContext)
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

				tax.VatCode = category.DefaultOutputVatCode.GetValueOrDefault (Business.Finance.VatCode.None);

				decimal amount, rate;
				InvoiceDocumentHelper.GetArticleTotalAmoutForTax (x, category.Name, out amount, out rate);
				tax.BaseAmount = amount;
				tax.Rate = rate;

				tax.ResultingTax = tax.BaseAmount * tax.Rate;
				tax.Text = string.Format ("TVA {0} pour {1}", Misc.PercentToString (rate), category.Name);
			}
		}

		private static void GetArticleTotalAmoutForTax(BusinessDocumentEntity x, FormattedText categoryName, out decimal amount, out decimal rate)
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

		private static void CreateOrDeleteTaxLines(BusinessDocumentEntity x, DataContext dataContext, int requiredTotal)
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

		private static void CreateTotalLineTTC(BusinessDocumentEntity x, DataContext dataContext)
		{
			//	Crée si nécessaire la dernière ligne de total TTC.
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);
			if (total != null)
			{
				return;  // la dernière ligne de total existe déjà
			}

			// Crée la dernière ligne de total.
			var lastPrice = dataContext.CreateEntity<EndTotalDocumentItemEntity> ();
			lastPrice.Visibility = true;

			x.Lines.Add (lastPrice);
		}

		private static void CreateTotalLineHT(BusinessDocumentEntity x, DataContext dataContext)
		{
			//	Crée si nécessaire la dernière ligne de total HT.
			var total = InvoiceDocumentHelper.GetTotalEntityHT (x);
			if (total != null)
			{
				return;  // la ligne de total existe déjà
			}

			// Crée la ligne de total.
			var lastPrice = dataContext.CreateEntity<EndTotalDocumentItemEntity> ();
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


		public static bool HasAmount(SubTotalDocumentItemEntity price)
		{
			if (price.Discount.DiscountRate.HasValue)
			{
				return true;
			}

			if (price.Discount.Value.HasValue)
			{
				return true;
			}

			if (price.FixedPrice.HasValue)
			{
				return true;
			}

			return false;
		}

		public static string GetAmount(SubTotalDocumentItemEntity price)
		{
			if (price.Discount.DiscountRate.HasValue)
			{
				return Misc.PercentToString (price.Discount.DiscountRate);
			}

			if (price.Discount.Value.HasValue)
			{
				return Misc.PriceToString (price.Discount.Value);
			}

			if (price.FixedPrice.HasValue)
			{
				return Misc.PriceToString (price.PrimaryPriceBeforeTax.GetValueOrDefault (0) - price.ResultingPriceBeforeTax.GetValueOrDefault (0));
			}

			return null;
		}


		public static decimal GetVatTotal(BusinessDocumentEntity x)
		{
			decimal total = 0;

			foreach (var line in x.Lines)
			{
				if (line is TaxDocumentItemEntity)
				{
					var tax = line as TaxDocumentItemEntity;

					total += tax.ResultingTax;
				}
			}

			return total;
		}

		public static decimal? GetTotalPriceTTC(BusinessDocumentEntity x)
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
					return total.PriceAfterTax;
				}
			}

			return null;
		}

		public static decimal? GetPrimaryPriceTTC(BusinessDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				return total.PriceAfterTax;
			}

			return null;
		}

		public static decimal? GetFixedPriceTTC(BusinessDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				return total.FixedPriceAfterTax;
			}

			return null;
		}

		public static void SetFixedPriceTTC(BusinessDocumentEntity x, decimal? value)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityTTC (x);

			if (total != null)
			{
				total.FixedPriceAfterTax = value;
			}
		}

		public static decimal? GetPrimaryPriceHT(BusinessDocumentEntity x)
		{
			var total = InvoiceDocumentHelper.GetTotalEntityHT (x);

			if (total != null)
			{
				return total.PriceBeforeTax;
			}

			return null;
		}

		private static EndTotalDocumentItemEntity GetTotalEntityHT(BusinessDocumentEntity x)
		{
			//	Retourne la ligne de total HT qui vient avant les lignes de TVA.
			int index = x.Lines.Count-2;  // avant la ligne de total TTC

			while (index >= 0)
			{
				var line = x.Lines[index--];

				if (line is EndTotalDocumentItemEntity)
				{
					return line as EndTotalDocumentItemEntity;
				}
			}

			return null;
		}

		private static EndTotalDocumentItemEntity GetTotalEntityTTC(BusinessDocumentEntity x)
		{
			//	Retourne la ligne de total TTC qui doit obligatoirement terminer une liste.
			if (x.Lines.Count > 0)
			{
				var lastLine = x.Lines.Last ();

				if (lastLine is EndTotalDocumentItemEntity)
				{
					return lastLine as EndTotalDocumentItemEntity;
				}
			}

			return null;
		}

	
#if false
		public static void UpdateDialogs(BusinessDocumentEntity x)
		{
			//	Met à jour le ou les dialogues d'aperçu avant impression ouverts.
			foreach (var dialog in CoreProgram.Application.AttachedDialogs)
			{
				foreach (var entity in dialog.Entities)
				{
					if (entity is BusinessDocumentEntity)
					{
						var invoiceDocument = entity as BusinessDocumentEntity;
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

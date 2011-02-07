//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public static class Enumerations
	{
		public static IEnumerable<EnumKeyValues<Business.DocumentType>> GetAllPossibleDocumentType()
		{
			yield return EnumKeyValues.Create (Business.DocumentType.SalesQuote,          "Devis");
			yield return EnumKeyValues.Create (Business.DocumentType.OrderConfiguration,  "Choix des options pour commande");
			yield return EnumKeyValues.Create (Business.DocumentType.OrderBooking,        "Bon pour commande");
			yield return EnumKeyValues.Create (Business.DocumentType.OrderConfirmation,   "Confirmation de commande");
			yield return EnumKeyValues.Create (Business.DocumentType.ProductionOrder,     "Ordre de production");
			yield return EnumKeyValues.Create (Business.DocumentType.ProductionChecklist, "Liste de production");
			yield return EnumKeyValues.Create (Business.DocumentType.ShipmentChecklist,   "Liste d'expédition");
			yield return EnumKeyValues.Create (Business.DocumentType.DeliveryNote,        "Bulletin de livraison");
			yield return EnumKeyValues.Create (Business.DocumentType.Invoice,             "Facture");
			yield return EnumKeyValues.Create (Business.DocumentType.InvoiceProForma,     "Facture pro forma");
			yield return EnumKeyValues.Create (Business.DocumentType.PaymentReminder,     "Rappel");
			yield return EnumKeyValues.Create (Business.DocumentType.Receipt,             "Reçu");
			yield return EnumKeyValues.Create (Business.DocumentType.CreditMemo,          "Note de crédit");
			yield return EnumKeyValues.Create (Business.DocumentType.QuoteRequest,        "Demande d'offre");
			yield return EnumKeyValues.Create (Business.DocumentType.PurchaseOrder,       "Confirmation de commande");
			yield return EnumKeyValues.Create (Business.DocumentType.Summary,             "Résumé");
		}

		public static IEnumerable<EnumKeyValues<Business.DocumentSource>> GetAllPossibleDocumentSource()
		{
			yield return EnumKeyValues.Create (Business.DocumentSource.Generated, "Généré");
			yield return EnumKeyValues.Create (Business.DocumentSource.Internal,  "Interne");
			yield return EnumKeyValues.Create (Business.DocumentSource.External,  "Externe");
		}

		public static IEnumerable<EnumKeyValues<Business.DocumentFlowDirection>> GetAllPossibleDocumentFlowDirection()
		{
			yield return EnumKeyValues.Create (Business.DocumentFlowDirection.Outgoing, "Sortant");
			yield return EnumKeyValues.Create (Business.DocumentFlowDirection.Incoming, "Entrant");
			yield return EnumKeyValues.Create (Business.DocumentFlowDirection.Internal, "Interne");
		}

		public static IEnumerable<EnumKeyValues<Finance.TaxMode>> GetAllPossibleTaxModes()
		{
			yield return EnumKeyValues.Create (Finance.TaxMode.LiableForVat,    "Assujetti à la TVA");
			yield return EnumKeyValues.Create (Finance.TaxMode.NotLiableForVat, "Non-assujetti à la TVA");
			yield return EnumKeyValues.Create (Finance.TaxMode.ExemptFromVat,   "Exonéré");
		}

		public static IEnumerable<EnumKeyValues<Finance.CurrencyCode>> GetAllPossibleCurrencyCodes()
		{
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Chf, "CHF", "Franc suisse");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Eur, "EUR", "Euro");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Usd, "USD", "Dollar américain");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Gbp, "GBP", "Livre anglaise");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Jpy, "JPY", "Yen japonais");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Cny, "CNY", "Yuan chinois");
		}

		public static IEnumerable<EnumKeyValues<UnitOfMeasureCategory>> GetAllPossibleUnitOfMeasureCategories()
		{
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Unrelated, "Indépendant");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Unit,      "Unité");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Mass,      "Masse");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Length,    "Longueur");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Surface,   "Surface");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Volume,    "Volume");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Time,      "Temps");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Energy,    "Energie");
		}

		public static IEnumerable<EnumKeyValues<Finance.VatCode>> GetInputVatCodes()
		{
			var filter = new HashSet<Finance.VatCode>
			{
				Finance.VatCode.Excluded,
				Finance.VatCode.ZeroRated,
				Finance.VatCode.StandardInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.ReducedInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.SpecialInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.StandardInputTaxOnInvestementOrOperatingExpenses,
				Finance.VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses,
				Finance.VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses,
			};

			return Enumerations.GetAllPossibleVatCodes ().Where (x => filter.Contains (x.Key));
		}

		public static IEnumerable<EnumKeyValues<Finance.VatCode>> GetOutputVatCodes()
		{
			var filter = new HashSet<Finance.VatCode>
			{
				Finance.VatCode.Excluded,
				Finance.VatCode.ZeroRated,
				Finance.VatCode.StandardTaxOnTurnover,
				Finance.VatCode.ReducedTaxOnTurnover,
				Finance.VatCode.SpecialTaxOnTurnover,
			};

			return Enumerations.GetAllPossibleVatCodes ().Where (x => filter.Contains (x.Key));
		}

		public static IEnumerable<EnumKeyValues<Finance.VatCode>> GetAllPossibleVatCodes()
		{
			yield return EnumKeyValues.Create (Finance.VatCode.Excluded, "EXCLU", "Exclu de l'impôt");
			yield return EnumKeyValues.Create (Finance.VatCode.ZeroRated, "EXPORT", "Exporté (exonéré)");

//-			yield return EnumKeyValues.Create (Finance.VatCode.StandardTax, "NORM", "Taux normal");  // TODO: ces 3 codes sont des inventions !
//-			yield return EnumKeyValues.Create (Finance.VatCode.ReducedTax, "RED", "Taux réduit");
//-			yield return EnumKeyValues.Create (Finance.VatCode.SpecialTax, "HEB", "Taux spécial");

			yield return EnumKeyValues.Create (Finance.VatCode.StandardInputTaxOnMaterialOrServiceExpenses, "IPM", "Taux normal, matériel ou service");
			yield return EnumKeyValues.Create (Finance.VatCode.ReducedInputTaxOnMaterialOrServiceExpenses, "IPMRED", "Taux réduit, matériel ou service");
			yield return EnumKeyValues.Create (Finance.VatCode.SpecialInputTaxOnMaterialOrServiceExpenses, "IPMHEB", "Taux spécial, matériel ou service");

			yield return EnumKeyValues.Create (Finance.VatCode.StandardInputTaxOnInvestementOrOperatingExpenses, "IPI", "Taux normal, investissement ou exploitation");
			yield return EnumKeyValues.Create (Finance.VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses, "IPIRED", "Taux réduit, investissement ou exploitation");
			yield return EnumKeyValues.Create (Finance.VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses, "IP(I)HEB", "Taux spécial, investissement ou exploitation");

			yield return EnumKeyValues.Create (Finance.VatCode.StandardTaxOnTurnover, "TVA", "Taux normal, chiffre d'affaires");
			yield return EnumKeyValues.Create (Finance.VatCode.ReducedTaxOnTurnover, "TVARED", "Taux réduit, chiffre d'affaires");
			yield return EnumKeyValues.Create (Finance.VatCode.SpecialTaxOnTurnover, "TVAHEB", "Taux spécial, chiffre d'affaires");
		}

		public static IEnumerable<EnumKeyValues<ArticleType>> GetAllPossibleArticleTypes()
		{
			yield return EnumKeyValues.Create (ArticleType.Goods,        "Marchandise");
			yield return EnumKeyValues.Create (ArticleType.Service,      "Service");
			yield return EnumKeyValues.Create (ArticleType.Subscription, "Abonnement");
			yield return EnumKeyValues.Create (ArticleType.Charge,       "Frais");
			yield return EnumKeyValues.Create (ArticleType.Freight,      "Port et emballage");
			yield return EnumKeyValues.Create (ArticleType.Tax,          "Taxe");
		}

		public static IEnumerable<EnumKeyValues<EnumValueCardinality>> GetAllPossibleValueCardinalities()
		{
			yield return EnumKeyValues.Create (EnumValueCardinality.Any,        "Zéro, un ou plusieurs");
			yield return EnumKeyValues.Create (EnumValueCardinality.AtLeastOne, "Au moins un");
			yield return EnumKeyValues.Create (EnumValueCardinality.ExactlyOne, "Exactement un");
			yield return EnumKeyValues.Create (EnumValueCardinality.ZeroOrOne,  "Un ou aucun");
		}

		public static IEnumerable<EnumKeyValues<ArticleQuantityType>> GetAllPossibleValueArticleQuantityType()
		{
			yield return EnumKeyValues.Create (ArticleQuantityType.Ordered,     "Commandé");
			yield return EnumKeyValues.Create (ArticleQuantityType.Billed,      "Livré");
			yield return EnumKeyValues.Create (ArticleQuantityType.Delayed,     "Suivra");
			yield return EnumKeyValues.Create (ArticleQuantityType.Information, "Information");
		}
	}
}

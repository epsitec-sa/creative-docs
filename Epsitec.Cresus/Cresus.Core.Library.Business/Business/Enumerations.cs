//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public static class Enumerations
	{
		public static IEnumerable<EnumKeyValues<Business.PrintableEntities>> GetAllPrintableEntities()
		{
			yield return EnumKeyValues.Create (Business.PrintableEntities.Relation,          "Clients");
			yield return EnumKeyValues.Create (Business.PrintableEntities.ArticleDefinition, "Articles");
			yield return EnumKeyValues.Create (Business.PrintableEntities.DocumentMetadata,  "Documents");
		}

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
			return EnumKeyValues.FromEnum<Finance.TaxMode> ();
		}

		public static IEnumerable<EnumKeyValues<Finance.CurrencyCode>> GetAllPossibleCurrencyCodes()
		{
			return EnumKeyValues.FromEnum<Finance.CurrencyCode> ();
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
			return EnumKeyValues.FromEnum<Finance.VatCode> ();
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

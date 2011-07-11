//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class AffairActions
	{
		public static void CreateOrderBooking()
		{
			AffairActions.CreateDocument (DocumentType.SalesQuote, DocumentType.OrderBooking);
		}

		public static void CreateOrderConfirmation()
		{
			AffairActions.CreateDocument (DocumentType.OrderBooking, DocumentType.OrderConfirmation);
		}

		public static void CreateProductionOrder()
		{
			AffairActions.CreateDocument (DocumentType.OrderConfirmation, DocumentType.ProductionOrder);
		}

		public static void CreateProductionCheckList()
		{
			AffairActions.CreateDocument (DocumentType.ProductionOrder, DocumentType.ProductionChecklist);
		}

		public static void CreateInvoice()
		{
			AffairActions.CreateDocument (DocumentType.OrderConfirmation, DocumentType.Invoice, AffairActions.CopyInvoice);
		}



		private static BusinessDocumentEntity CopyInvoice(IBusinessContext businessContext, DocumentMetadataEntity metadata)
		{
			var invoice  = businessContext.CreateEntity<BusinessDocumentEntity> ();
			var template = metadata.BusinessDocument as BusinessDocumentEntity;

			invoice.BaseDocumentCode      = template.Code;
			invoice.BillToMailContact     = template.BillToMailContact;
			invoice.ShipToMailContact     = template.ShipToMailContact;
			invoice.OtherPartyRelation    = template.OtherPartyRelation;
			invoice.OtherPartyBillingMode = template.OtherPartyBillingMode;
			invoice.OtherPartyTaxMode     = template.OtherPartyTaxMode;
			invoice.BillingStatus         = Finance.BillingStatus.DebtorBillOpen;
			invoice.BillingDate           = Date.Today;
			invoice.CurrencyCode          = template.CurrencyCode;
			invoice.PriceRefDate          = template.PriceRefDate;
			invoice.PriceGroup            = template.PriceGroup;
			invoice.DebtorBookAccount     = template.DebtorBookAccount;

			var copy = template.Lines.Select (x => x.CloneEntity (businessContext));

			invoice.Lines.AddRange (copy);

			invoice.Lines.OfType<ArticleDocumentItemEntity> ().ForEach (x => AffairActions.UpdateBillingArticleLine (x));

			return invoice;
		}

		private static void UpdateBillingArticleLine(ArticleDocumentItemEntity line)
		{
			var ordered = line.GetOrderedQuantity ();

			if (ordered == 0)
			{
				line.BillingUnitPriceBeforeTax = null;
			}
			else
			{
				line.BillingUnitPriceBeforeTax = line.ResultingLinePriceBeforeTax / ordered;
			}
		}


		private static void CreateDocument(DocumentType docTypeOld, DocumentType docTypeNew, System.Func<IBusinessContext, DocumentMetadataEntity, BusinessDocumentEntity> businessDocumentResolver = null)
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();
			var currentDocument = currentAffair.Documents.LastOrDefault (x => x.DocumentCategory.DocumentType == docTypeOld);

			System.Diagnostics.Debug.Assert (currentDocument.IsNotNull (), string.Format ("No {0} document can be found", docTypeOld));

			if (currentDocument.IsNotNull ())
			{
				var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();

				documentMetadata.DocumentCategory = categoryRepo.Find (docTypeNew).First ();
				documentMetadata.BusinessDocument = businessDocumentResolver == null ? currentDocument.BusinessDocument : businessDocumentResolver (businessContext, currentDocument);

				currentAffair.Documents.Add (documentMetadata);
				
				currentDocument.DocumentState = DocumentState.Frozen;
			}
		}
	}
}

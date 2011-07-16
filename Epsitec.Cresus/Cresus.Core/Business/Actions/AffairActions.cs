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
			AffairActions.CreateDocument (DocumentType.OrderConfirmation, DocumentType.Invoice);
		}


		private static BusinessDocumentEntity CloneBusinessDocument(IBusinessContext businessContext, DocumentMetadataEntity metadata)
		{
			var template = metadata.BusinessDocument as BusinessDocumentEntity;
			var document = template.CloneEntity (businessContext);

			switch (metadata.DocumentCategory.DocumentType)
			{
				case DocumentType.InvoiceProForma:
				case DocumentType.Invoice:
					AffairActions.SetupInvoice (document);
					break;
			}

			return document;
		}

		private static void SetupInvoice(BusinessDocumentEntity document)
		{
			document.BillingStatus         = Finance.BillingStatus.DebtorBillOpen;
			document.BillingDate           = Date.Today;
			
			document.Lines.OfType<ArticleDocumentItemEntity> ().ForEach (x => AffairActions.SetupInvoiceArticleDocumentItem (x));
		}


		private static void SetupInvoiceArticleDocumentItem(ArticleDocumentItemEntity line)
		{
			var ordered = line.GetOrderedQuantity ();
			line.ReferenceUnitPriceBeforeTax = (ordered == 0) ? null : line.ResultingLinePriceBeforeTax / ordered;
		}


		private static void CreateDocument(DocumentType docTypeOld, DocumentType docTypeNew)
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
				documentMetadata.BusinessDocument = AffairActions.CloneBusinessDocument (businessContext, currentDocument);

				currentAffair.Documents.Add (documentMetadata);
				
				currentDocument.DocumentState = DocumentState.Frozen;
			}
		}
	}
}

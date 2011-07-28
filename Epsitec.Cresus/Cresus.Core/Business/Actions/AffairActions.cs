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

		public static void CreateDeliveryNote()
		{
			AffairActions.CreateDocument (DocumentType.OrderConfirmation, DocumentType.DeliveryNote);
		}

		public static void CreateInvoice()
		{
			AffairActions.CreateDocument (DocumentType.DeliveryNote, DocumentType.Invoice);
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
				documentMetadata.BusinessDocument = AffairActions.CloneBusinessDocument (businessContext, currentAffair, currentDocument, docTypeNew);

				currentAffair.Documents.Add (documentMetadata);

				currentDocument.DocumentState = DocumentState.Frozen;
			}
		}

		private static BusinessDocumentEntity CloneBusinessDocument(IBusinessContext businessContext, AffairEntity affair, DocumentMetadataEntity metadata, DocumentType docTypeNew)
		{
			var template = metadata.BusinessDocument as BusinessDocumentEntity;
			var document = template.CloneEntity (businessContext);

			switch (docTypeNew)
			{
				case DocumentType.DeliveryNote:
					AffairActions.SetupDeliveryNote (businessContext, affair, document);
					break;

				case DocumentType.InvoiceProForma:
				case DocumentType.Invoice:
					AffairActions.SetupInvoice (businessContext, document);
					break;
			}

			return document;
		}


		#region DeliveryNote
		private static void SetupDeliveryNote(IBusinessContext businessContext, AffairEntity affair, BusinessDocumentEntity document)
		{
			var deliveryNotes = affair.Documents.Where (x => x.BusinessDocument != document && x.DocumentCategory.DocumentType == DocumentType.DeliveryNote);

			for (int i = 0; i < document.Lines.Count; i++)
			{
				var article = document.Lines[i] as ArticleDocumentItemEntity;

				if (article != null)
				{
					//	Passe en revue toutes les quantités déjà livrées dans les bulletins de livraison existants.
					decimal shippedPreviously = 0;
					foreach (var deliveryNote in deliveryNotes)
					{
						shippedPreviously += AffairActions.SetupDeliveryNoteShipped (deliveryNote, i);
					}

					AffairActions.SetupDeliveryNoteArticleDocumentItem (businessContext, article, shippedPreviously);
				}
			}
		}

		private static decimal SetupDeliveryNoteShipped(DocumentMetadataEntity deliveryNote, int i)
		{
			//	Retourne la quantité livrée d'un article donné dans un bulletin de livraison.
			decimal shipped = 0;

			var businessDocument = deliveryNote.BusinessDocument as BusinessDocumentEntity;

			if (i < businessDocument.Lines.Count)
			{
				var article = businessDocument.Lines[i] as ArticleDocumentItemEntity;

				if (article != null)
				{
					foreach (var quantity in article.ArticleQuantities)
					{
						if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Shipped)
						{
							shipped += quantity.Quantity;
						}
					}
				}
			}

			return shipped;
		}

		private static void SetupDeliveryNoteArticleDocumentItem(IBusinessContext businessContext, ArticleDocumentItemEntity line, decimal shippedPreviously)
		{
			//	Cherche la quantité à livrer la plus probable.
			decimal shippedQuantity = 0;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Ordered)
				{
					shippedQuantity += quantity.Quantity;
				}

				if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Delayed ||
					quantity.QuantityColumn.QuantityType == ArticleQuantityType.Delayed ||
					quantity.QuantityColumn.QuantityType == ArticleQuantityType.Expected)
				{
					shippedQuantity -= quantity.Quantity;
				}
			}

			//	Crée la quantité à livrer.
			var quantityColumnEntity = AffairActions.SearchArticleQuantityColumnEntity (businessContext, ArticleQuantityType.Shipped);
			if (quantityColumnEntity != null)
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = shippedQuantity;
				newQuantity.QuantityColumn = quantityColumnEntity;
				newQuantity.BeginDate = new Date (Date.Today.Ticks);

				line.ArticleQuantities.Add (newQuantity);
			}

			//	Crée la quantité livrée précédemment.
			quantityColumnEntity = AffairActions.SearchArticleQuantityColumnEntity (businessContext, ArticleQuantityType.ShippedPreviously);
			if (quantityColumnEntity != null)
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = shippedPreviously;
				newQuantity.QuantityColumn = quantityColumnEntity;

				line.ArticleQuantities.Add (newQuantity);
			}
		}
		#endregion


		#region Invoice
		private static void SetupInvoice(IBusinessContext businessContext, BusinessDocumentEntity document)
		{
			document.BillingStatus = Finance.BillingStatus.DebtorBillOpen;
			document.BillingDate   = Date.Today;

			document.Lines.OfType<ArticleDocumentItemEntity> ().ForEach (x => AffairActions.SetupInvoiceArticleDocumentItem (businessContext, x));
		}

		private static void SetupInvoiceArticleDocumentItem(IBusinessContext businessContext, ArticleDocumentItemEntity line)
		{
			var ordered = line.GetOrderedQuantity ();
			line.ReferenceUnitPriceBeforeTax = (ordered == 0) ? null : line.ResultingLinePriceBeforeTax / ordered;

			//	Cherche la quantité à facturer la plus probable.
			decimal billedQuantity = 0;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Shipped)
				{
					billedQuantity += quantity.Quantity;
				}
			}

			//	Crée la quantité à facturer.
			var quantityColumnEntity = AffairActions.SearchArticleQuantityColumnEntity (businessContext, ArticleQuantityType.Billed);
			if (quantityColumnEntity != null)
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = billedQuantity;
				newQuantity.QuantityColumn = quantityColumnEntity;
				newQuantity.BeginDate = new Date (Date.Today.Ticks);

				line.ArticleQuantities.Add (newQuantity);
			}
		}
		#endregion


		private static ArticleQuantityColumnEntity SearchArticleQuantityColumnEntity(IBusinessContext businessContext, ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}
	}
}

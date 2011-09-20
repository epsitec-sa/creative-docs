//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	public class BusinessDocumentBusinessRules : GenericBusinessRule<BusinessDocumentEntity>
	{
		public override void ApplySetupRule(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			entity.Code = (string) ItemCodeGenerator.NewCode ();
			entity.CurrencyCode = Finance.CurrencyCode.Chf;
			entity.BillingDate = Date.Today;
			entity.BillingStatus = Finance.BillingStatus.NotAnInvoice;
			entity.PriceRefDate = Date.Today;
		}

		public override void ApplyUpdateRule(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var documentMetadata = businessContext.GetMasterEntity<DocumentMetadataEntity> ();

			DocumentPriceCalculator.Calculate (businessContext, entity, documentMetadata);
		}


		public static DocumentMetadataEntity GetDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentType documentType)
		{
			//	Cherche le document d'un type donné.
			var sourceDocuments = from document in activeAffair.Documents.Reverse ()
								  where document.DocumentCategory.DocumentType == documentType &&
									    document.BusinessDocument.VariantId.GetValueOrDefault () == activeVariantId
								  select document;

			return sourceDocuments.FirstOrDefault ();
		}

		public static DocumentMetadataEntity GetSourceDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentType sourceDocumentType)
		{
			var document = BusinessDocumentBusinessRules.FindSourceDocument (businessContext, activeAffair, activeVariantId, sourceDocumentType);
			System.Diagnostics.Debug.Assert (document != null);
			return document;
		}

		public static DocumentMetadataEntity CreateDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentType sourceDocumentType)
		{
			var documentCategory = BusinessDocumentBusinessRules.FindDocumentCategory (businessContext, sourceDocumentType);

			if (documentCategory.IsNotNull ())
			{
				var documentMetadata = BusinessDocumentBusinessRules.CreateDocumentMetadata (businessContext, documentCategory);
				var sourceDocument   = BusinessDocumentBusinessRules.FindSourceDocument (businessContext, activeAffair, activeVariantId, sourceDocumentType);

				if (sourceDocument.IsNotNull ())
				{
					//	Le nouveau document devient un clone du document source.
					documentMetadata.BusinessDocument = BusinessDocumentBusinessRules.CloneBusinessDocument (businessContext, activeAffair, sourceDocument, sourceDocumentType);

					// TODO: Ce n'est pas suffisant de geler le document "source" !
					// Une facture n'est jamais gelée, puisqu'elle ne sert jamais de source.
					sourceDocument.DocumentState = DocumentState.Inactive;
				}

				activeAffair.Documents.Add (documentMetadata);

				return documentMetadata;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Cannot create document of type {0}", sourceDocumentType));
			}
		}

		public static void AddPayment(DocumentMetadataEntity document, PaymentTransactionEntity payment)
		{
			//	Ajoute un PaymentTransaction à un document commercial.
			if (payment != null)
			{
				var businessDocument = document.BusinessDocument as BusinessDocumentEntity;

				businessDocument.PaymentTransactions.Add (payment);
				businessDocument.BillingDate = payment.PaymentDetail.Date;
			}
		}

		
		private static DocumentMetadataEntity CreateDocumentMetadata(IBusinessContext businessContext, DocumentCategoryEntity documentCategory)
		{
			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();
			
			documentMetadata.DocumentCategory = documentCategory;
			
			return documentMetadata;
		}

		private static DocumentMetadataEntity FindSourceDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentType sourceDocumentType)
		{
			var documentTypes = new HashSet<DocumentType> (DocumentLogic.GetProcessParentDocumentTypes (sourceDocumentType));

			//	Cherche le document source à utiliser comme modèle.
			var sourceDocuments = from document in activeAffair.Documents.Reverse ()
								  where documentTypes.Contains (document.DocumentCategory.DocumentType) &&
									    document.BusinessDocument.VariantId.GetValueOrDefault () == activeVariantId
								  select document;

			return sourceDocuments.FirstOrDefault ();
		}
		
		private static DocumentCategoryEntity FindDocumentCategory(IBusinessContext businessContext, DocumentType documentType)
		{
			var categoryRepository = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var documentCategories = categoryRepository.Find (documentType);
			var documentCategory   = documentCategories.FirstOrDefault ();
			
			return documentCategory;
		}

		private static BusinessDocumentEntity CloneBusinessDocument(IBusinessContext businessContext, AffairEntity affair, DocumentMetadataEntity metadata, DocumentType docTypeNew)
		{
			var template = metadata.BusinessDocument as BusinessDocumentEntity;
			var document = template.CloneEntity (businessContext);

			switch (docTypeNew)
			{
				case DocumentType.DeliveryNote:
					BusinessDocumentBusinessRules.SetupDeliveryNote (businessContext, affair, document);
					break;

				case DocumentType.InvoiceProForma:
				case DocumentType.Invoice:
					BusinessDocumentBusinessRules.SetupInvoice (businessContext, document);
					break;
			}

			return document;
		}


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
						shippedPreviously += BusinessDocumentBusinessRules.SetupDeliveryNoteShipped (deliveryNote, i);
					}

					BusinessDocumentBusinessRules.SetupDeliveryNoteArticleDocumentItem (businessContext, article, shippedPreviously);
				}
			}
		}

		private static decimal SetupDeliveryNoteShipped(DocumentMetadataEntity deliveryNote, int i)
		{
			//	Retourne la quantité livrée d'un article donné dans un bulletin de livraison.
			//	Ceci est très glissant et suppose que les lignes sont exactement les mêmes entre tous les bulletins
			//	de livraison. En principe, l'édition des lignes d'un bulletin de livraison est interdite, donc
			//	cela devrait jouer, mais bon...
			//	TODO: Ajouter un garde-fou !
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
					quantity.QuantityColumn.QuantityType == ArticleQuantityType.Expected)
				{
					shippedQuantity -= quantity.Quantity;
				}
			}

			//	Crée la quantité à livrer.
			var quantityColumnEntity = BusinessDocumentBusinessRules.FindArticleQuantityColumnEntity (businessContext, ArticleQuantityType.Shipped);
			if (quantityColumnEntity != null)
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = shippedQuantity - shippedPreviously;
				newQuantity.QuantityColumn = quantityColumnEntity;
				newQuantity.BeginDate = new Date (Date.Today.Ticks);

				line.ArticleQuantities.Add (newQuantity);
			}

			//	Crée la quantité livrée précédemment.
			var existingPreviously = line.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == ArticleQuantityType.ShippedPreviously).FirstOrDefault ();

			if (existingPreviously == null)
			{
				quantityColumnEntity = BusinessDocumentBusinessRules.FindArticleQuantityColumnEntity (businessContext, ArticleQuantityType.ShippedPreviously);
				if (quantityColumnEntity != null)
				{
					var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
					newQuantity.Quantity = shippedPreviously;
					newQuantity.QuantityColumn = quantityColumnEntity;

					line.ArticleQuantities.Add (newQuantity);
				}
			}
			else
			{
				existingPreviously.Quantity = shippedPreviously;
			}
		}

		private static void SetupInvoice(IBusinessContext businessContext, BusinessDocumentEntity document)
		{
			document.BillingStatus = Finance.BillingStatus.DebtorBillOpen;
			document.BillingDate   = Date.Today;

			foreach (var line in document.Lines.OfType<ArticleDocumentItemEntity> ())
			{
				BusinessDocumentBusinessRules.SetupInvoiceArticleDocumentItem (businessContext, line);
			}
		}

		private static void SetupInvoiceArticleDocumentItem(IBusinessContext businessContext, ArticleDocumentItemEntity line)
		{
			decimal orderedQuantity = line.GetQuantity (ArticleQuantityType.Ordered);
			decimal shippedQuantity = line.GetQuantity (ArticleQuantityType.Shipped);

			//	Crée la quantité à facturer.
			var quantityColumn = BusinessDocumentBusinessRules.FindArticleQuantityColumnEntity (businessContext, ArticleQuantityType.Billed);
			
			if (quantityColumn.IsNotNull ())
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();

				newQuantity.Quantity       = shippedQuantity;
				newQuantity.QuantityColumn = quantityColumn;
				newQuantity.BeginDate      = new Date (Date.Today.Ticks);

				line.HasPartialQuantities = (orderedQuantity != shippedQuantity);
				
				line.ArticleQuantities.Add (newQuantity);
			}
		}


		private static ArticleQuantityColumnEntity FindArticleQuantityColumnEntity(IBusinessContext businessContext, ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}
	}
}

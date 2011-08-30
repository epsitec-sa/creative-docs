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

			using (var calculator = new DocumentPriceCalculator (businessContext, entity, documentMetadata))
			{
				PriceCalculator.UpdatePrices (calculator);
			}
		}


		
		public static DocumentMetadataEntity CreateDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentType documentType)
		{
			var documentCategory = BusinessDocumentBusinessRules.FindDocumentCategory (businessContext, documentType);

			if (documentCategory.IsNotNull ())
			{
				var documentMetadata = BusinessDocumentBusinessRules.CreateDocumentMetadata (businessContext, documentCategory);
				var sourceDocument   = BusinessDocumentBusinessRules.FindSourceDocument (businessContext, activeAffair, activeVariantId, documentMetadata);

				if (sourceDocument.IsNotNull ())
				{
					//	Le nouveau document devient un clone du document source.
					documentMetadata.BusinessDocument = BusinessDocumentBusinessRules.CloneBusinessDocument (businessContext, activeAffair, sourceDocument, documentType);

					// TODO: Ce n'est pas suffisant de geler le document "source" !
					// Une facture n'est pas exnicolasemple jamais gelée, puisqu'elle ne sert jamais de source.
					sourceDocument.DocumentState = DocumentState.Inactive;
				}

				activeAffair.Documents.Add (documentMetadata);

				return documentMetadata;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Cannot create document of type {0}", documentType));
			}
		}

		
		private static DocumentMetadataEntity CreateDocumentMetadata(IBusinessContext businessContext, DocumentCategoryEntity documentCategory)
		{
			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();
			
			documentMetadata.DocumentCategory = documentCategory;
			
			return documentMetadata;
		}

		private static DocumentMetadataEntity FindSourceDocument(IBusinessContext businessContext, AffairEntity activeAffair, int activeVariantId, DocumentMetadataEntity documentMetadata)
		{
			var businessLogic = new BusinessLogic (businessContext, documentMetadata);
			var documentTypes = new HashSet<DocumentType> (businessLogic.ProcessParentDocumentTypes);

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
			var ordered = line.GetOrderedQuantity ();
			line.ReferenceUnitPriceBeforeTax = (ordered == 0) ? null : line.ResultingLinePriceBeforeTax / ordered;

			//	Cherche la quantité à facturer.
			decimal billedQuantity = 0;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Shipped)
				{
					billedQuantity += quantity.Quantity;
				}
			}

			//	Crée la quantité à facturer.
			var quantityColumnEntity = BusinessDocumentBusinessRules.FindArticleQuantityColumnEntity (businessContext, ArticleQuantityType.Billed);
			if (quantityColumnEntity != null)
			{
				var newQuantity = businessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = billedQuantity;
				newQuantity.QuantityColumn = quantityColumnEntity;
				newQuantity.BeginDate = new Date (Date.Today.Ticks);

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

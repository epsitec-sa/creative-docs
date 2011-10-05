//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Business.Rules;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class CustomerActions
	{
		public static void CreateAffairAndSalesQuote()
		{
			CustomerActions.CreateAffairAndFirstDocument (DocumentType.SalesQuote);
		}

		public static void CreateAffairAndDirectInvoice()
		{
			CustomerActions.CreateAffairAndFirstDocument (DocumentType.Invoice, "DirectInvoiceWorkflow");
		}

		public static void FinishDirectInvoice()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var activeAffair    = AffairActions.GetActiveAffair ();
			var activeVariantId = WorkflowArgs.GetActiveVariantId ().GetValueOrDefault ();
			var invoiceDocument = BusinessDocumentBusinessRules.GetDocument (businessContext, activeAffair, activeVariantId, DocumentType.Invoice);
			System.Diagnostics.Debug.Assert (invoiceDocument != null);

			//	Affiche le dialogue permettant de valider la facture directe en choisissant le moyen de paiement.
			var paymentTransaction = AffairActions.CreateInvoiceDialog (businessContext, invoiceDocument, true);

			if (paymentTransaction == null)  // annulation de l'utilisateur dans le dialogue ?
			{
				throw new WorkflowException (WorkflowCancellation.Transition);
			}
			else
			{
				BusinessDocumentBusinessRules.AddPayment (invoiceDocument, paymentTransaction);
			}
		}


		private static void CreateAffairAndFirstDocument(DocumentType documentType, string workflowName = null)
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var currentCustomer = businessContext.GetMasterEntity<CustomerEntity> ();

			//	Create the affair, the document metadata and the document itself; as these might
			//	be referred to from the business logic, make sure we temporarily register them as
			//	master entities :

			var affair           = businessContext.CreateMasterEntity<AffairEntity> ();
			var documentMetadata = businessContext.CreateMasterEntity<DocumentMetadataEntity> ();
			var businessDocument = businessContext.CreateMasterEntity<BusinessDocumentEntity> ();

			System.Diagnostics.Debug.Assert (affair.Customer == currentCustomer);

			if (CustomerActions.SetupDocumentMetadata (businessContext, documentMetadata, businessDocument, documentType))
			{
				CustomerActions.SetupBusinessDocument (businessContext, businessDocument, currentCustomer);

				affair.Customer = currentCustomer;
				affair.Workflow = WorkflowFactory.CreateDefaultWorkflow<AffairEntity> (businessContext, workflowName);

				affair.Documents.Add (documentMetadata);

				currentCustomer.Affairs.Add (affair);
			}

			//	Now that everything has been properly set up, we can remove the entities from
			//	the list of master entities. They will be registered appropriately when the
			//	UI panels are opened.

			businessContext.RemoveMasterEntity (businessDocument);
			businessContext.RemoveMasterEntity (documentMetadata);
			businessContext.RemoveMasterEntity (affair);

			workflowEngine.GetAssociated<NavigationOrchestrator> ().NavigateToTiles (affair, documentMetadata);
		}

		private static bool SetupDocumentMetadata(IBusinessContext businessContext, DocumentMetadataEntity documentMetadata, BusinessDocumentEntity businessDocument, DocumentType documentType)
		{
			var categoryRepository = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var documentCategories = categoryRepository.Find (documentType);

			if (documentCategories == null || documentCategories.Count () == 0)
			{
				return false;
			}
			else
			{
				var documentCategory = documentCategories.First ();

				documentMetadata.DocumentCategory = documentCategory;
				documentMetadata.DocumentTitle    = documentCategory.Name;
				documentMetadata.BusinessDocument = businessDocument;
				documentMetadata.DocumentState    = DocumentState.Draft;

				return true;
			}
		}

		private static void SetupBusinessDocument(IBusinessContext businessContext, BusinessDocumentEntity businessDocument, CustomerEntity currentCustomer)
		{
			//	Define default billing & shipping addresses :

			var defaultMailContact = currentCustomer.Relation.DefaultMailContact;

			businessDocument.VariantId         = 0;
			businessDocument.BillToMailContact = businessContext.GetLocalEntity (currentCustomer.GetMailContact (ContactGroupType.Billing) ?? defaultMailContact);
			businessDocument.ShipToMailContact = businessContext.GetLocalEntity (currentCustomer.GetMailContact (ContactGroupType.Shipping) ?? defaultMailContact);
		}
	}
}

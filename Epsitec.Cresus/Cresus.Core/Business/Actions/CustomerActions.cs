//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class CustomerActions
	{
		public static void CreateAffairAndSalesQuote()
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

			CustomerActions.SetupBusinessDocument (businessContext, businessDocument, currentCustomer);
			CustomerActions.SetupDocumentMetadata (businessContext, documentMetadata, businessDocument);

			affair.Customer = currentCustomer;
			affair.Workflow = WorkflowFactory.CreateDefaultWorkflow<AffairEntity> (businessContext);

			affair.Documents.Add (documentMetadata);

			currentCustomer.Affairs.Add (affair);

			//	Now that everything has been properly set up, we can remove the entities from
			//	the list of master entities. They will be registered appropriately when the
			//	UI panels are opened.

			businessContext.RemoveMasterEntity (businessDocument);
			businessContext.RemoveMasterEntity (documentMetadata);
			businessContext.RemoveMasterEntity (affair);
		}
		
		private static void SetupDocumentMetadata(IBusinessContext businessContext, DocumentMetadataEntity documentMetadata, BusinessDocumentEntity businessDocument)
		{
			var categoryRepo     = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var documentCategory = categoryRepo.Find (DocumentType.SalesQuote).First ();

			documentMetadata.DocumentCategory = documentCategory;
			documentMetadata.DocumentTitle    = documentCategory.Name;
			documentMetadata.BusinessDocument = businessDocument;
			documentMetadata.DocumentState    = DocumentState.Active;
		}

		private static void SetupBusinessDocument(IBusinessContext businessContext, BusinessDocumentEntity businessDocument, CustomerEntity currentCustomer)
		{
			//	Define default billing & shipping addresses :

			businessDocument.BillToMailContact = currentCustomer.GetMailContact (ContactGroupType.Billing);
			businessDocument.ShipToMailContact = currentCustomer.GetMailContact (ContactGroupType.Shipping);
		}

	}
}

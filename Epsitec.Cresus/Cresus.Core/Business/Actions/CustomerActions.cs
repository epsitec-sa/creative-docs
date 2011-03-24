//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var currentCustomer = businessContext.GetMasterEntity<CustomerEntity> ();

			var affair           = businessContext.CreateEntity<AffairEntity> ();
			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();
			var businessDocument = businessContext.CreateEntity<BusinessDocumentEntity> ();
			var documentCategory = categoryRepo.Find (DocumentType.SalesQuote).First ();

			documentMetadata.DocumentCategory = documentCategory;
			documentMetadata.DocumentTitle    = documentCategory.Name;
			documentMetadata.BusinessDocument = businessDocument;
			documentMetadata.DocumentState    = DocumentState.Active;

			affair.Customer = currentCustomer;
			affair.Workflow = WorkflowFactory.CreateDefaultWorkflow<AffairEntity> (businessContext);

			affair.Documents.Add (documentMetadata);

			currentCustomer.Affairs.Add (affair);
		}
	}
}

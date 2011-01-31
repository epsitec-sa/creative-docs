//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class DocumentActions
	{
		public static void CreateOrderBooking()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.Transition.BusinessContext;
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryRepository> ();
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();
			var currentDocument = businessContext.GetMasterEntity<DocumentMetadataEntity> ();

			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();

			documentMetadata.DocumentCategory = categoryRepo.Find (DocumentType.OrderBooking).First ();
			documentMetadata.BusinessDocument = currentDocument.BusinessDocument;
			documentMetadata.Workflow         = currentDocument.Workflow;

			currentAffair.Documents.Add (documentMetadata);
		}
		
		public static void CreateOrderConfirmation()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.Transition.BusinessContext;
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryRepository> ();
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();
			var currentDocument = businessContext.GetMasterEntity<DocumentMetadataEntity> ();

			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();

			documentMetadata.DocumentCategory = categoryRepo.Find (DocumentType.OrderConfirmation).First ();
			documentMetadata.BusinessDocument = currentDocument.BusinessDocument;
			documentMetadata.Workflow         = currentDocument.Workflow;

			currentAffair.Documents.Add (documentMetadata);
		}
	}
}

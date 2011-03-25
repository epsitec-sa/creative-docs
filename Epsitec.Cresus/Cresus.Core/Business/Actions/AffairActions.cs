//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();
			var currentDocument = currentAffair.Documents.LastOrDefault (x => x.DocumentCategory.DocumentType == DocumentType.SalesQuote);

			System.Diagnostics.Debug.Assert (currentDocument.IsNotNull (), "No sales quote document can be found");

			if (currentDocument.IsNotNull ())
			{
				var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();

				documentMetadata.DocumentCategory = categoryRepo.Find (DocumentType.OrderBooking).First ();
				documentMetadata.BusinessDocument = currentDocument.BusinessDocument;

				currentAffair.Documents.Add (documentMetadata);
			}
		}

		public static void CreateOrderConfirmation()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var categoryRepo    = businessContext.GetSpecificRepository<DocumentCategoryEntity.Repository> ();
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();
			var currentDocument = currentAffair.Documents.LastOrDefault (x => x.DocumentCategory.DocumentType == DocumentType.OrderBooking);

			System.Diagnostics.Debug.Assert (currentDocument.IsNotNull (), "No order booking document can be found");

			if (currentDocument.IsNotNull ())
			{
				var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();

				documentMetadata.DocumentCategory = categoryRepo.Find (DocumentType.OrderConfirmation).First ();
				documentMetadata.BusinessDocument = currentDocument.BusinessDocument;

				currentAffair.Documents.Add (documentMetadata);
			}
		}
	}
}

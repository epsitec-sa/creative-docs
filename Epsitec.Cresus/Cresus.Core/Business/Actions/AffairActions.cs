﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		public static void CreateSalesQuote()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.Transition.BusinessContext;
			var categoryRepo    = businessContext.Data.GetRepository<DocumentCategoryEntity> () as DocumentCategoryRepository;
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();

			var documentMetadata = businessContext.CreateEntity<DocumentMetadataEntity> ();
			var categorySalesQuote = categoryRepo.Find (DocumentType.SalesQuote).OrderBy (x => x.Rank).FirstOrDefault ();

			var businessDocument = businessContext.CreateEntity<BusinessDocumentEntity> ();

			documentMetadata.DocumentCategory = businessContext.GetLocalEntity (categorySalesQuote);
			documentMetadata.BusinessDocument = businessDocument;

			currentAffair.Documents.Add (documentMetadata);
		}
	}
}

//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Rules;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Workflows;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class AffairActions
	{
		public static void CreateOrderBooking()
		{
			AffairActions.CreateDocument (DocumentType.OrderBooking);
		}

		public static void CreateOrderConfirmation()
		{
			AffairActions.CreateDocument (DocumentType.OrderConfirmation);
		}

		public static void CreateProductionOrder()
		{
			AffairActions.CreateDocument (DocumentType.ProductionOrder);
		}

		public static void CreateProductionCheckList()
		{
			AffairActions.CreateDocument (DocumentType.ProductionChecklist);
		}

		public static void CreateDeliveryNote()
		{
			AffairActions.CreateDocument (DocumentType.DeliveryNote);
		}

		public static void CreateInvoice()
		{
			AffairActions.CreateDocument (DocumentType.Invoice);
		}

		public static void CreateOfferVariant()
		{
		}


		public static void ValidateActiveDocument()
		{
			var currentAffair = AffairActions.GetActiveAffair ();
			var workflowTransition = AffairActions.GetCurrentTransition ();
			var workflowThread     = workflowTransition.Thread;
			var workflowArgs       = workflowThread.GetArgs ();
		}


		private static void CreateDocument(DocumentType newDocumentType)
		{
			var businessContext  = WorkflowExecutionEngine.Current.BusinessContext;
			var activeAffair     = AffairActions.GetActiveAffair ();
			var activeVariantId  = WorkflowArgs.GetActiveVariantId ();
			var documentMetadata = BusinessDocumentBusinessRules.CreateDocument (businessContext, activeAffair, activeVariantId, newDocumentType);

			int? variantId = documentMetadata.BusinessDocument.VariantId;

//-			System.Diagnostics.Debug.Assert (variantId.HasValue);
		}


		private static AffairEntity GetActiveAffair()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext as BusinessContext;
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();

			return currentAffair;
		}


		private static int GetNextVariantId()
		{
			var affair = AffairActions.GetActiveAffair ();

			var variantIds = new HashSet<int> (affair.Documents.Select (x => x.BusinessDocument.VariantId.GetValueOrDefault ()));

			if (variantIds.Count == 0)
			{
				return 0;
			}
			else
			{
				return variantIds.OrderByDescending (x => x).FirstOrDefault () + 1;
			}
		}

		private static WorkflowTransition GetCurrentTransition()
		{
			return WorkflowExecutionEngine.Current.Transition;
		}
	}
}

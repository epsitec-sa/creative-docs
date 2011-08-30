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
		public static void CreateSalesQuoteVariant()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.SalesQuote,
				(affair, document) =>
				{
					document.BusinessDocument.VariantId = AffairActions.GetNextVariantId (affair);
				});
		}

		public static void CreateOrderBooking()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.OrderBooking);
		}

		public static void CreateOrderConfirmation()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.OrderConfirmation);
		}

		public static void CreateProductionOrder()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.ProductionOrder);
		}

		public static void CreateProductionCheckList()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.ProductionChecklist);
		}

		public static void CreateDeliveryNote()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.DeliveryNote);
		}

		public static void CreateInvoice()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.Invoice);
		}


		public static void ValidateActiveDocument()
		{
			var currentAffair = AffairActions.GetActiveAffair ();
			var workflowTransition = AffairActions.GetCurrentTransition ();
			var workflowThread     = workflowTransition.Thread;
			var workflowArgs       = workflowThread.GetArgs ();
		}


		private static void CreateAndFocusDocument(DocumentType newDocumentType, System.Action<AffairEntity, DocumentMetadataEntity> setupAction = null)
		{
			var businessContext  = WorkflowExecutionEngine.Current.BusinessContext;
			var activeAffair     = AffairActions.GetActiveAffair ();
			var activeVariantId  = WorkflowArgs.GetActiveVariantId ().GetValueOrDefault ();
			var documentMetadata = BusinessDocumentBusinessRules.CreateDocument (businessContext, activeAffair, activeVariantId, newDocumentType);

			if (setupAction != null)
			{
				setupAction (activeAffair, documentMetadata);
			}

			WorkflowArgs.SetActiveVariantId (documentMetadata.BusinessDocument.VariantId);
		}


		internal static AffairEntity GetActiveAffair()
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext as BusinessContext;
			var currentAffair   = businessContext.GetMasterEntity<AffairEntity> ();

			return currentAffair;
		}


		internal static int GetNextVariantId(AffairEntity affair)
		{
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

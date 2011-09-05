//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Rules;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Workflows;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class AffairActions
	{
		public static void CreateSalesQuoteVariant()
		{
			AffairActions.CreateAndFocusDocument (DocumentType.SalesQuote, AffairActions.SetupVariantAndBindWithDocument);
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
			var currentAffair  = AffairActions.GetActiveAffair ();
			var workflowThread = WorkflowExecutionEngine.GetCurrentWorkflowThread ();
			var workflowArgs   = workflowThread.GetArgs ();
		}


		private static void CreateAndFocusDocument(DocumentType newDocumentType, System.Action<AffairEntity, DocumentMetadataEntity> setupAction = null)
		{
			var workflowEngine  = WorkflowExecutionEngine.Current;
			var businessContext = workflowEngine.BusinessContext;
			var activeAffair    = AffairActions.GetActiveAffair ();
			var activeVariantId = WorkflowArgs.GetActiveVariantId ().GetValueOrDefault ();
			var sourceDocument  = BusinessDocumentBusinessRules.GetSourceDocument (businessContext, activeAffair, activeVariantId, newDocumentType);

			PaymentTransactionEntity paymentTransaction = null;

			if (newDocumentType == DocumentType.Invoice)
			{
				//	Affiche le dialogue permettant de choisir le moyen de paiement, avant de créer la facture.
				paymentTransaction = AffairActions.CreateInvoiceDialog (businessContext, sourceDocument, false);

				if (paymentTransaction == null)  // annulation de l'utilisateur dans le dialogue ?
				{
					throw new WorkflowException (WorkflowCancellation.Transition);
				}
			}

			var documentMetadata = BusinessDocumentBusinessRules.CreateDocument (businessContext, activeAffair, activeVariantId, newDocumentType);

			if (setupAction != null)
			{
				setupAction (activeAffair, documentMetadata);
			}

			BusinessDocumentBusinessRules.AddPayment (documentMetadata, paymentTransaction);

			WorkflowArgs.SetActiveVariantId (documentMetadata.BusinessDocument.VariantId);

			workflowEngine.GetAssociated<NavigationOrchestrator> ().NavigateToTiles (activeAffair, documentMetadata);
		}

		private static void SetupVariantAndBindWithDocument(AffairEntity affair, DocumentMetadataEntity document)
		{
			var variantId = AffairActions.GetNextVariantId (affair);
			var thread    = WorkflowExecutionEngine.GetCurrentWorkflowThread ();

			document.BusinessDocument.VariantId = variantId;

			var name        = TextFormatter.FormatText ("Variante", variantId);
			var description = TextFormatter.FormatText ("Workflow associé à la variante", variantId);

			WorkflowExecutionEngine.SetWorkflowThreadNameAndDescription (thread, name, description);
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


		public static PaymentTransactionEntity CreateInvoiceDialog(IBusinessContext businessContext, DocumentMetadataEntity documentMetadata, bool invoiceValidation)
		{
			//	Choix interactif du moyen de paiement.
			using (var dialog = new Dialogs.CreateInvoiceDialog (businessContext, documentMetadata, invoiceValidation))
			{
				dialog.IsModal = true;
				dialog.OpenDialog ();

				if (dialog.Result == DialogResult.Accept)
				{
					return dialog.PaymentTransaction;
				}
				else
				{
					return null;
				}
			}
		}
	}
}

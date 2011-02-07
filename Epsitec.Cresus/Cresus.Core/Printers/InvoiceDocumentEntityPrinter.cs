//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	public class InvoiceDocumentEntityPrinter : AbstractEntityPrinter<DocumentMetadataEntity>
	{
		public InvoiceDocumentEntityPrinter(CoreData coreData, DocumentMetadataEntity metadata)
			: base (metadata)
		{
			BusinessDocumentEntity doc = metadata.BusinessDocument;

			this.documentPrinters.Add (new InvoiceDocumentPrinter (coreData, this, metadata, doc));

			if (doc.BillToMailContact != null)
			{
				this.documentPrinters.Add (new MailContactLabelDocumentPrinter (coreData, this, doc.BillToMailContact));
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.Offer, "Offre", "Offre pour le client.");

				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.Order, "Commande", "Commande pour le client.");

				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionOrder ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.OrderAcknowledge, "Confirmation de commande", "Confirmation de commande pour le client.");
				
				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.ProductionOrder, "Ordres de production", "Ordres de production, pour chaque atelier.");
				
				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionProductionOrder ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.BL, "Bulletin de livraison", "Bulletin de livraison, sans prix.");

				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionBL ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.Invoice, "Facture", "Facture avec ou sans bulletin de versement.");

				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionEsr ();

				type.AddBasePrinterUnit ();
				type.AddEsrPrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

#if false
			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.InvoiceWithInsideESR, "Facture avec BV intégré", "Facture avec un bulletin de versement intégré au bas de chaque page.");
				
				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionEsr ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.InvoiceWithOutsideESR, "Facture avec BV séparé", "Facture avec un bulletin de versement imprimé sur une page séparée.");
				
				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionEsr ();

				type.AddBasePrinterUnit ();
				type.AddEsrPrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (DocumentType.InvoiceWithoutESR, "Facture sans BV", "Facture simple sans bulletin de versement.");
				
				type.AddDocumentOptionInvoice ();
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionSpecimen ();

				type.AddBasePrinterUnit ();
				type.AddLabelPrinterUnit ();

				this.DocumentTypes.Add (type);
			}
#endif
		}


		protected override DocumentType AdjustContinuousDocumentType(DocumentType type)
		{
			return type;
		}


		internal static bool CheckCompatibleEntity(DocumentMetadataEntity metadata)
		{
			return metadata.BusinessDocument.IsNotNull ();
		}
	}
}

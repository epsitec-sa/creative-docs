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

	public class InvoiceDocumentEntityPrinter : AbstractEntityPrinter<InvoiceDocumentEntity>
	{
		public InvoiceDocumentEntityPrinter(InvoiceDocumentEntity entity)
			: base (entity)
		{
			this.documentPrinters.Add (new InvoiceDocumentPrinter (this, this.entity));

			DocumentTypeDefinition type;

			type = new DocumentTypeDefinition (DocumentType.Offer, "Offre", "Offre pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.Order, "Commande", "Commande pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionOrder ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.OrderAcknowledge, "Confirmation de commande", "Confirmation de commande pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.ProductionOrder, "Ordres de production", "Ordres de production, pour chaque atelier.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionProductionOrder ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.BL, "Bulletin de livraison", "Bulletin de livraison, sans prix.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionBL ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithInsideESR, "Facture avec BV intégré", "Facture avec un bulletin de versement intégré au bas de chaque page.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionEsr ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithOutsideESR, "Facture avec BV séparé", "Facture avec un bulletin de versement imprimé sur une page séparée.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionEsr ();
			type.AddBasePrinterUnit ();
			type.AddEsrPrinterUnit ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithoutESR, "Facture sans BV", "Facture simple sans bulletin de versement.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddBasePrinterUnit ();
			this.DocumentTypes.Add (type);
		}
	}
}

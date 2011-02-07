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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	public class RelationEntityPrinter : AbstractEntityPrinter<RelationEntity>
	{
		public RelationEntityPrinter(CoreData coreData, RelationEntity entity)
			: base (entity)
		{
			this.documentPrinters.Add (new RelationDocumentPrinter (coreData, this, this.entity));

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (Business.DocumentType.Summary, "Résumé du client", "Une ou plusieurs pages A4 avec un résumé du client.");
				type.DocumentOptions.Add (new DocumentOptionDefinition ("Données à inclure :"));
				type.DocumentOptions.Add (new DocumentOptionDefinition (DocumentOption.RelationMail, null, "Adresses", true));
				type.DocumentOptions.Add (new DocumentOptionDefinition (DocumentOption.RelationTelecom, null, "Téléphones", true));
				type.DocumentOptions.Add (new DocumentOptionDefinition (DocumentOption.RelationUri, null, "Emails", true));
				type.AddDocumentOptionOrientation ();
				type.AddDocumentOptionMargin ();
				type.AddDocumentOptionSpecimen ();
				type.AddBasePrinterUnit ();
				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (Business.DocumentType.Debug1, "Test #1", "Page fixe de test pour l'objet TextBand.");
				this.DocumentTypes.Add (type);
			}

			{
				DocumentTypeDefinition type = new DocumentTypeDefinition (Business.DocumentType.Debug2, "Test #2", "Page fixe de test pour l'objet TableBand.");
				this.DocumentTypes.Add (type);
			}
		}
	}
}

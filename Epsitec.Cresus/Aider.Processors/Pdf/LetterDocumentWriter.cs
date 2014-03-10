//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Pdf.LetterDocument;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Epsitec.Aider.Entities;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Pdf
{
	internal sealed class LetterDocumentWriter
	{
		public LetterDocumentWriter(AiderContactEntity contact, LabelLayout layout)
		{
			this.contact = contact;
			this.layout  = layout;
		}


		public void WriteStream(System.IO.Stream stream)
		{
			var setup			= new LetterDocumentSetup ();

			var report			= this.GetReport (setup);

			var addressBuilder = new System.Text.StringBuilder ();

			addressBuilder.Append (this.contact.DisplayAddress.ToString ());

			var contentTemplateBuilder = new System.Text.StringBuilder ();

			contentTemplateBuilder.Append (string.Format ("Blabla..."));

			var topReference = TextFormatter.FormatText ("Top");

			report.GeneratePdf (stream, topReference, addressBuilder.ToString (), contentTemplateBuilder.ToString ());
		}

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
		}

		private readonly AiderContactEntity		contact;
		private readonly LabelLayout			layout;
	}
}

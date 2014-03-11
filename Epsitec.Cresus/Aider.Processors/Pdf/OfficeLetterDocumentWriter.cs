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
	internal sealed class OfficeLetterDocumentWriter
	{
		public OfficeLetterDocumentWriter(AiderOfficeLetterReportEntity letter, AiderOfficeSenderEntity settings, LabelLayout layout)
		{
			this.letter		= letter;
			this.layout		= layout;
			this.settings	= settings;
		}


		public void WriteStream(System.IO.Stream stream)
		{
			var setup			= new LetterDocumentSetup ();

			var report			= this.GetReport (setup);

			var addressBuilder	= new System.Text.StringBuilder ();

			addressBuilder
				.Append (this.letter.RecipientContact.Address.AddressLine1 + "<br/>")
				.Append (this.letter.RecipientContact.DisplayName + "<br/>")
				.Append (this.letter.RecipientContact.Address.PostBox + "<br/>")
				.Append (this.letter.RecipientContact.Address.StreetUserFriendly + "<br/>")
				.Append (this.letter.RecipientContact.Address.GetDisplayZipCode () + " ")
				.Append (this.letter.RecipientContact.Address.Town);

			var contentTemplateBuilder = new System.Text.StringBuilder ();

			contentTemplateBuilder.Append (this.letter.GetLetterContent ());

			var topReference = TextFormatter.FormatText (this.settings.Office.OfficeName);

			report.GeneratePdf (stream, topReference, addressBuilder.ToString (), contentTemplateBuilder.ToString ());
		}

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
		}

		private readonly AiderOfficeLetterReportEntity		letter;
		private readonly AiderOfficeSenderEntity			settings;
		private readonly LabelLayout						layout;
	}
}

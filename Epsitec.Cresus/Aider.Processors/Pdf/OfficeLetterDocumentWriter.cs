//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Pdf.LetterDocument;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Processors.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Aider.Processors.Pdf
{
	internal sealed class OfficeLetterDocumentWriter
	{
		public OfficeLetterDocumentWriter(BusinessContext context, AiderOfficeSenderEntity settings, LabelLayout layout)
		{
			this.layout	  = layout;
			this.context  = context;
			this.settings = settings;
		}


		public void WriteStream(System.IO.Stream stream, AiderOfficeLetterReportEntity letter)
		{
			var setup	= this.GetSetup ();
			var report	= this.GetReport (setup);
			var content = new System.Text.StringBuilder ();

			content.Append (letter.GetLetterContent ());
			
			var topLogoPath  = CoreContext.GetFileDepotPath ("assets", "eerv-logo.png");
			var topLogo	     = string.Format ("<img src=\"{0}\" />", topLogoPath);
			var topReference = "<b>" + this.settings.Office.OfficeName + "</b>";

			var senderAddress    = Builders.BuildAddress (letter.Office.OfficeMainContact, false);

			var recipientAddress = new StringBuilder()
										.Append (Builders.BuildAddress (letter.RecipientContact, true))
										.Append ("<br/><br/>")
										.Append (letter.TownAndDate)
										.ToString ();
			
			report.GeneratePdf (stream, topLogo, topReference, senderAddress, recipientAddress, content);
		}

		private LetterDocumentSetup GetSetup()
		{
			var setup = new LetterDocumentSetup ();

			//First tab for centered elements
			setup.TextStyle.TabInsert (new Common.Drawing.TextStyle.Tab (725, Common.Drawing.TextTabType.Center, Common.Drawing.TextTabLine.None));

			return setup;
		}

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
		}

		private readonly BusinessContext		 context;
		private readonly AiderOfficeSenderEntity settings;
		private readonly LabelLayout			 layout;
	}
}

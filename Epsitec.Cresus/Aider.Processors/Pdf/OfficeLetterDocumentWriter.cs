//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
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
using Epsitec.Aider.Reporting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Pdf
{
	public sealed class OfficeLetterDocumentWriter : AbstractDocumentWriter<AiderOfficeLetterReportEntity>
	{
		public OfficeLetterDocumentWriter()
		{
		}


		public override void WriteStream(System.IO.Stream stream, AiderOfficeLetterReportEntity letter)
		{
			var setup	= this.GetSetup ();
			var report	= this.GetReport (setup);
			var content = new System.Text.StringBuilder ();

			content.Append (letter.GetFormattedText ());
			
			var topLogo	     = string.Format ("<img src=\"{0}\" />", CoreContext.GetFileDepotPath ("assets", "logo-eerv.png"));
			var topReference = "<b>" + this.sender.Office.OfficeName + "</b>";

			var senderAddressBlock    = letter.Office.OfficeMainContact.GetAddressLabelText (PostalAddressType.Compact);
			var recipientAddressBlock = OfficeLetterDocumentWriter.GetRecipientAddress (letter);
			var placeAndDate		  = OfficeLetterDocumentWriter.GetPlaceAndDate (letter);
			report.GeneratePdf (stream, topLogo, topReference, senderAddressBlock, recipientAddressBlock, placeAndDate, content);
		}

		private static string GetRecipientAddress(AiderOfficeLetterReportEntity letter)
		{
			return TextFormatter.FormatText (
				letter.RecipientContact.GetAddressLabelText (PostalAddressType.Default)).ToString ();
		}

		private static string GetPlaceAndDate(AiderOfficeLetterReportEntity letter)
		{
			return TextFormatter.FormatText (letter.TownAndDate).ToString ();
		}

		private LetterDocumentSetup GetSetup()
		{
			var setup = new LetterDocumentSetup ();

			//First tab for centered elements
			setup.TextStyle.TabInsert (new Common.Drawing.TextStyle.Tab (725, Common.Drawing.TextTabType.Center, Common.Drawing.TextTabLine.None));
			
			setup.TextStyle.Font = Font.GetFont ("Verdana", "");
			setup.TextStyle.FontSize = 31.75; // 9pt => 9 x 25.4/72 = 3.175mm --- SL used 33.835

			return setup;
		}

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
		}
	}
}

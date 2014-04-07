//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.LetterDocument;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using Epsitec.Aider.Entities;


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
			setup.TextStyle.TabInsert (new TextStyle.Tab (72.5.Millimeters (), TextTabType.Center, TextTabLine.None));
			setup.TextStyle.TabInsert (new TextStyle.Tab (90.0.Millimeters (), TextTabType.Left, TextTabLine.None));
			
			setup.TextStyle.Font = Font.GetFont ("Verdana", "");
			setup.TextStyle.FontSize = 9.0.Points ();

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

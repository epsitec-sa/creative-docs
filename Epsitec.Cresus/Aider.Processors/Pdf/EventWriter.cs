//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Pdf.LetterDocument;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Data.Common;

namespace Epsitec.Aider.Processors.Pdf
{
	public sealed class EventWriter : AbstractDocumentWriter<AiderEventOfficeReportEntity>
	{
		public EventWriter()
		{
		}

		public override void WriteStream(System.IO.Stream stream, AiderEventOfficeReportEntity officeReport)
		{
			var setup   = this.GetSetup ();
			var report  = this.GetReport (setup);
			
			var lines = new List<string> ();
			
			/*foreach (var participant in officeReport.Participants.OrderBy (x => x.Contact.DisplayName))
			{
				var contact	= participant.Contact;
				var person	= contact.Person;
				var address	= contact.Address;
				
				var fullName = FormattedText.Escape (person.GetFullName ());
				var street	 = FormattedText.Escape (address.GetShortStreetAddress ());
				var zip		 = address.GetDisplayZipCode();
				var town	 = FormattedText.Escape (address.Town.Name);
				var bDate	 = person.GetFormattedBirthdayDate ();
				var parish	 = "";
				
				if (person.HasDerogation)
				{
					var parishOrigin = (sender.Office.ParishGroupPathCache == person.ParishGroupPathCache) ? ParishOrigin.Geographic : ParishOrigin.Active;
					parish = person.GetParishLocationName (context, parishOrigin) ?? "—";
				}
				
				lines.Add (fullName + " (" + bDate + "), " + town + ", " + street + "<tab/>" + parish + "<br/>");
			}

			lines = lines.Distinct ().ToList ();

			for (int i = 0; i < lines.Count; i++)
			{
				lines[i] = string.Format ("<tab/>{0}.<tab/>{1}", i+1, lines[i]);
			}*/


			var topLogoPath   = CoreContext.GetFileDepotPath ("assets", "logo-eerv.png");
			var topLogo	      = string.Format (@"<img src=""{0}"" />", topLogoPath);
			var headerContent = new FormattedText (topLogo) + officeReport.GetFormattedText ();
			var footerContent = TextFormatter.FormatText ("Extrait d'AIDER le", Date.Today.ToShortDateString ());
			
			report.AddTopLeftLayer (headerContent, 50);
			var formattedContent = new FormattedText (string.Concat (lines));
			report.AddBottomRightLayer (footerContent, 100);
			report.GeneratePdf (stream, formattedContent);
		}

		private ListingDocument GetReport(ListingDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			var report = new ListingDocument (exportPdfInfo, setup);

			report.HeaderHeight = 40.0.Millimeters ();
			report.FooterHeight = 5.0.Millimeters ();

			return report;
		}

		private ListingDocumentSetup GetSetup()
		{
			var setup = new ListingDocumentSetup ();

			setup.TextStyle.TabInsert (new TextStyle.Tab (5.0.Millimeters (), TextTabType.DecimalDot, TextTabLine.None));
			setup.TextStyle.TabInsert (new TextStyle.Tab (10.0.Millimeters (), TextTabType.DecimalDot, TextTabLine.None));
			setup.TextStyle.TabInsert (new TextStyle.Tab (120.0.Millimeters (), TextTabType.Left, TextTabLine.None));

			setup.TextStyle.Font = Font.GetFont ("Verdana", "");
			setup.TextStyle.FontSize = 9.0.Points ();
			
			return setup;
		}
	}
}

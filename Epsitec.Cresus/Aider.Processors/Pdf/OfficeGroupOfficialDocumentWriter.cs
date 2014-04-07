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

namespace Epsitec.Aider.Processors.Pdf
{
	public sealed class OfficeGroupOfficialDocumentWriter : AbstractDocumentWriter<AiderOfficeGroupParticipantReportEntity>
	{
		public OfficeGroupOfficialDocumentWriter()
		{
		}

		public override void WriteStream(System.IO.Stream stream, AiderOfficeGroupParticipantReportEntity officeReport)
		{
			var setup   = this.GetSetup ();
			var report  = this.GetReport (setup);
			var content = new System.Text.StringBuilder ();

			content.Append (officeReport.GetFormattedText ());

			var no = 0;
			
			foreach (var participant in officeReport.Participants.OrderBy (x => x.Contact.DisplayName))
			{
				no++;

				var contact		= participant.Contact;
				var person		= contact.Person;
				var address		= contact.Address;
				var fullName	= person.GetFullName ();
				var street		= address.StreetHouseNumberAndComplement;
				var zip			= address.GetDisplayZipCode();
				var town		= address.Town.Name;
				var bDate		= person.BirthdayDay + "." + person.BirthdayMonth + "." + person.BirthdayYear;

				var parish		= "";
				
				//We display derogations ?
				if (person.HasDerogation)
				{
					if (sender.Office.ParishGroupPathCache == person.ParishGroupPathCache)
					{
						parish = person.GetGeoParishGroup (this.context).Name.Substring (11).Trim();
					}
					else
					{
						parish = person.ParishGroup.Name.Substring (11).Trim();
					}

					content.Append ("<tab/>" + no + ".<tab/>" + fullName + " (" + bDate + "), " + town + ", " + street + "<tab/>" + parish + "<br/>");
				}
				else
				{
					content.Append ("<tab/>" + no + ".<tab/>" + fullName + " (" + bDate + "), " + town + ", " + street + "<br/>");
				}	
			}


			var topLogo	     = string.Format ("<img src=\"{0}\" />", CoreContext.GetFileDepotPath ("assets", "logo-eerv.png"));
			var bottomReference	= "Extrait d'AIDER le " + System.DateTime.Now.ToString ("d MMM yyyy");

			var formattedContent = new FormattedText (content);
			report.AddTopLeftLayer (topLogo, 50);
			report.AddBottomRightLayer (bottomReference, 100);
			report.GeneratePdf (stream, formattedContent);
		}

		private TextDocument GetReport(TextDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new TextDocument (exportPdfInfo, setup);
		}

		private TextDocumentSetup GetSetup()
		{
			var setup = new TextDocumentSetup ();

			setup.TextStyle.TabInsert (new TextStyle.Tab (5.0.Millimeters (), TextTabType.DecimalDot, TextTabLine.None));
			setup.TextStyle.TabInsert (new TextStyle.Tab (10.0.Millimeters (), TextTabType.DecimalDot, TextTabLine.None));
			setup.TextStyle.TabInsert (new TextStyle.Tab (120.0.Millimeters (), TextTabType.Left, TextTabLine.None));

			setup.TextStyle.Font = Font.GetFont ("Verdana", "");
			setup.TextStyle.FontSize = 9.0.Points ();
			
			return setup;
		}
	}
}

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
			var act     = officeReport.Event;
			var lines = new List<string> ();

			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
					this.WriteBlessingAct (act, lines);
					break;
			}

			var topLogoPath   = CoreContext.GetFileDepotPath ("assets", "logo-eerv.png");
			var topLogo	      = string.Format (@"<img src=""{0}"" />", topLogoPath);
			var headerContent = new FormattedText (topLogo);
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
					headerContent += new FormattedText ("<b>Registre des bénédictions</b><br/><br/>Acte n°" + officeReport.EventNumber);
					break;
			}

			var footerContent = TextFormatter.FormatText ("Extrait d'AIDER le", Date.Today.ToShortDateString ());
			report.AddTopLeftLayer (headerContent, 50);
			var formattedContent = new FormattedText (string.Concat (lines));
			report.AddBottomRightLayer (footerContent, 100);
			report.GeneratePdf (stream, formattedContent);
		}

		private void WriteEventPlaceAndDateLine (AiderEventEntity act, List<string> lines)
		{
			var placeAndDate = act.Place.Name + " le, " + act.Date.Value.ToShortDateString () + "<br/>";
			lines.Add (placeAndDate);
		}

		private void WriteMinisterLine(AiderEventEntity act, List<string> lines)
		{
			var minister = "Ministre officiant: " + act.GetMinister ().GetFullName () + "<br/>";
			lines.Add (minister);
		}

		private void WriteBlessingAct (AiderEventEntity act, List<string> lines)
		{
			var blessedPerson = act.GetMainActors ();
			lines.Add ("<b>Bénédiction</b><br/>");
			this.WriteEventPlaceAndDateLine (act, lines);
			foreach (var actor in blessedPerson)
			{
				lines.Add ("<br/><tab/><b>" + actor.GetFullName () + "</b><br/>");
				this.WriteSonOf (actor, act, lines);
			}
			this.WriteMinisterLine (act, lines);
		}

		private void WriteSonOf(AiderPersonEntity person, AiderEventEntity act, List<string> lines)
		{
			var filiation = "fils de ";

			if (person.eCH_Person.PersonSex == Enumerations.PersonSex.Female)
			{
				filiation = "fille de ";
			}

			var father = act.GetActor (Enumerations.EventParticipantRole.Father);
			var mother = act.GetActor (Enumerations.EventParticipantRole.Mother);
			if (father != null && mother != null)
			{
				lines.Add (filiation + father.GetFullName () + " et de " + mother.GetFullName () + "<br/>");
				return;
			}

			if (father != null && mother == null)
			{
				lines.Add (filiation + father.GetFullName () + "<br/>");
				return;
			}

			if (father == null && mother != null)
			{
				lines.Add (filiation + mother.GetFullName () + "<br/>");
				return;
			}

			lines.Add (filiation + "<i>(non-renseigné)</i>"  + "<br/>");
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

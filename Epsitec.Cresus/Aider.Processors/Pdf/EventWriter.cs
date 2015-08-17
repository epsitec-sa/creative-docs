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
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Processors.Pdf
{
	public sealed class EventWriter : AbstractDocumentWriter<AiderEventOfficeReportEntity>
	{
		public EventWriter()
		{
		}
		public override void WriteBatchStream(System.IO.Stream stream, IEnumerable<AiderEventOfficeReportEntity> officeReports)
		{
			throw new System.NotImplementedException ();
		}

		public override void WriteStream(System.IO.Stream stream, AiderEventOfficeReportEntity officeReport)
		{
			var setup   = this.GetSetup ();
			var report  = this.GetReport (setup);
			var act     = officeReport.Event;
			
			// header
			var topLogoPath   = CoreContext.GetFileDepotPath ("assets", "logo-eerv.png");
			var topLogo	      = new FormattedText (string.Format (@"<img src=""{0}"" />", topLogoPath));
			report.AddTopLeftLayer (topLogo, 100, 100);

			var headerContent = new FormattedText ("<b>Extrait du " + act.GetRegitryName () + "</b><br/>");
			headerContent += new FormattedText ("<b>de la " + officeReport.Office.OfficeName + "</b><br/><br/>");

			report.AddTopLeftLayer (headerContent, 200, 500);
		
			// footer
			var footerContent = TextFormatter.FormatText ("Extrait du registre informatique de l'EERV le ", Date.Today.ToShortDateString ());
			report.AddBottomRightLayer (footerContent, 100);


			// content
			var lines = new List<string> ();
			this.WriteGroupAct (act, lines);
			var formattedContent = new FormattedText (string.Concat (string.Join ("<br/>", lines)));

			report.GeneratePdf (stream, formattedContent);
		}

		private void WriteGroupAct (AiderEventEntity act, List<string> lines)
		{
			var actors = act.GetMainActors ();

			
			if (act.State != Enumerations.EventState.Validated)
			{
				lines.Add ("<b>Acte à valider :</b><br/>");
			}
			else
			{
				lines.Add ("<b>Acte N°" + act.Report.GetEventNumber () + "</b><br/>");
			}

			switch (act.Kind)
			{
				case Enumerations.EventKind.Branches:
					lines.Add ("<b>Rameaux " + act.Date.Value.Year + "</b><br/>");
					break;
				case Enumerations.EventKind.CultOfTheAlliance:
					lines.Add ("<b>Culte de l'alliance " + act.Date.Value.Year + "</b><br/>");
					break;
				case Enumerations.EventKind.Other:
					lines.Add ("<b>" + act.Comment + " " + act.Date.Value.Year + "</b><br/>");
					break;
			}
			
			foreach (var actor in actors)
			{
				this.AddActorHeaderLines (act, actor, lines);
			}
	
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
					this.AddContentLines (actors, System.Tuple.Create<string, string, string> (
						"a été béni", 
						"a été bénie", 
						"ont étés bénis"
					), lines);
					break;
				case Enumerations.EventType.Baptism:
					this.AddContentLines (actors, System.Tuple.Create<string, string, string> (
						"a été baptisé",
						"a été baptisée",
						"ont étés baptisés"
					), lines);
					break;
				case Enumerations.EventType.Confirmation:
					this.AddContentLines (actors, System.Tuple.Create<string, string, string> (
						"a effectué sa confirmation",
						"a effectué sa confirmation",
						"ont effectués leurs confirmations"
					), lines);
					break;
				case Enumerations.EventType.EndOfCatechism:
					this.AddContentLines (actors, System.Tuple.Create<string, string, string> (
						"a effectué sa fin de catéchisme",
						"a effectué sa fin de catéchisme",
						"ont effectués leurs fin de catéchisme"
					), lines);
					break;
				case Enumerations.EventType.FuneralService:
					lines.Add ("<br/><br/><tab/><b>les funérailles ont eu lieu</b><br/><br/>");
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					lines.Add ("<br/><br/><tab/><b>ont été mariés</b><br/><br/>");
					break;
			}
			
			this.AddActFooterLines (act, lines);
		}

		/// <summary>
		/// Formulas Triple Item:
		/// Item1: For male actor
		/// Item2: For female actor
		/// Item3:For multi actors
		/// </summary>
		/// <param name="actors"></param>
		/// <param name="formulas"></param>
		private void AddContentLines(List<AiderEventParticipantEntity> actors, System.Tuple<string, string, string> formulas, List<string> lines)
		{
			if (actors.Count > 1)
			{
				lines.Add ("<br/><br/><tab/><b>" + formulas.Item3 + "</b><br/><br/>");
			}
			else
			{
				if (actors[0].GetSex () == PersonSex.Female)
				{
					lines.Add ("<br/><br/><tab/><b>" + formulas.Item2 + "</b><br/><br/>");
				}
				else
				{
					lines.Add ("<br/><br/><tab/><b>" + formulas.Item1 + "</b><br/><br/>");
				}
			}
		}

		private void AddActFooterLines(AiderEventEntity act, List<string> lines)
		{
			lines.Add (this.GetWhen (act));
			lines.Add ("lieu :<tab/>" + act.Place.Name + this.Tabs () + "à :<tab/>" + act.Town.Name);
			lines.Add (this.GetMinisterLine (act));
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
				case Enumerations.EventType.Baptism:
					lines.Add (this.GetParticipantLine (act, EventParticipantRole.GodFather) + this.Tabs () + this.GetParticipantLine (act, EventParticipantRole.GodMother));
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					lines.Add (this.GetParticipantLine (act, EventParticipantRole.FirstWitness) + this.Tabs () + this.GetParticipantLine (act, EventParticipantRole.SecondWitness));
					break;
			}
			
			if (act.State == Enumerations.EventState.Validated)
			{
				lines.Add ("<br/>Visa :<tab/>" + act.Validator.DisplayName);
				lines.Add ("<br/><br/><br/><br/><br/><br/>");
				lines.Add ("<tab/><tab/>Extrait certifié conforme à l'original.<br/>");
				lines.Add ("<tab/><tab/>Lieu :<tab/>…………………………………………………………………<br/>");
				lines.Add ("<tab/><tab/>Date :<tab/>…………………………………………………………………<br/>");
				lines.Add ("<tab/><tab/>Signature :<tab/>…………………………………………………………………<br/>");
			}
		}

		private void AddActorHeaderLines(AiderEventEntity act, AiderEventParticipantEntity actor, List<string> lines)
		{
			lines.Add (this.GetLastNameLine (actor) + this.Tabs () + this.GetFirstNameLine (actor));
			lines.Add (this.GetBirthDateLine (actor) + this.Tabs () + this.GetTownLine (actor));
			lines.Add (this.GetParishLine (actor));
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
					lines.Add (this.GetSonOfLine (act, EventParticipantRole.BlessedChild));
					break;
				case Enumerations.EventType.Baptism:
					lines.Add (this.GetSonOfLine (act, EventParticipantRole.ChildBatise));
					break;
				case Enumerations.EventType.Confirmation:
					lines.Add (this.GetSonOfLine (act, EventParticipantRole.Confirmant));
					break;
				case Enumerations.EventType.EndOfCatechism:
					lines.Add (this.GetSonOfLine (act, EventParticipantRole.Catechumen));
					break;
				case Enumerations.EventType.FuneralService:
					lines.Add (this.GetConfessionLine (actor));
					lines.Add (this.GetSonOfLine (act, EventParticipantRole.DeceasedPerson));
					
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					lines.Add (this.GetConfessionLine (actor));
					if (act.GetParticipantByRole (Enumerations.EventParticipantRole.Husband) == actor)
					{
						lines.Add (this.GetHusbandSonOfLine (act));
					}
					if (act.GetParticipantByRole (Enumerations.EventParticipantRole.Spouse) == actor)
					{
						lines.Add (this.GetSpouseSonOfLine (act));
					}
					
					break;
			}
		}

		private string Tabs ()
		{
			return "<tab/>";
		}

		private string GetWhen(AiderEventEntity act)
		{
			return  "le :<tab/>" + act.Date.Value.ToDateTime ().ToString ("dd MMMM yyyy");
		}

		private string GetMinisterLine(AiderEventEntity act)
		{
			var line = "par :<tab/>";
			var ministers = act.GetMinisters ();
			if (ministers.Count > 1)
			{
				var ministersInfo = new List<string> ();
				foreach (var minister in ministers)
				{
					ministersInfo.Add (this.GetMinisterInfo (minister));
				}
				line += string.Join (" et ", ministersInfo);
			}
			else
			{
				line += this.GetMinisterInfo (ministers[0]);
			}			
			return line;
		}

		private string GetMinisterInfo (AiderEventParticipantEntity minister)
		{
			if (minister.IsExternal == false && minister.Person.Employee.IsNotNull ())
			{
				var position = minister.Person.Employee.EmployeeType.ToString ();
				return minister.GetFullName () + " (" + position + ")";
			}
			else
			{
				return minister.GetFullName ();
			}
		}

		private string GetParticipantLine(AiderEventEntity act, Enumerations.EventParticipantRole role)
		{
			var line = this.GetRoleLabel (role);
			line += act.GetActorFullName (role);
			return line;
		}

		private string GetRoleLabel(Enumerations.EventParticipantRole role)
		{
			return Res.Types.Enum.EventParticipantRole.FindValueFromEnumValue (role).Caption.DefaultLabel + " :<tab/>";
		}

		private string GetParishLine(AiderEventParticipantEntity person)
		{
			var parish = "Paroisse :<tab/>";
			parish += person.GetParishName ();
			return parish;
		}

		private string GetBirthDateLine(AiderEventParticipantEntity person)
		{
			var bd = "Né le :<tab/>";
			if (person.GetSex () == PersonSex.Female)
			{
				bd = "Née le :<tab/>";
			}

			bd += person.GetBirthDate ().Value.ToShortDateString ();
			return bd;
		}

		private string GetConfessionLine(AiderEventParticipantEntity person)
		{
			var confession = Res.Types.Enum.PersonConfession.FindValueFromEnumValue (person.GetConfession ()).Caption.DefaultLabel;
			return "Confession :<tab/>" + confession;
		}

		private string GetFirstNameLine(AiderEventParticipantEntity person)
		{
			var firstName = "Prénom :<tab/><b>";
			var names = person.GetFirstName ().Split (' ');
			firstName += names[0] + "</b> " + string.Join (" ", names.Skip (1));
			return firstName;
		}

		private string GetLastNameLine(AiderEventParticipantEntity person)
		{
			var lastName = "Nom :<tab/><b>";
			lastName += person.GetLastName () + "</b>";
			return lastName;
		}

		private string GetTownLine(AiderEventParticipantEntity person)
		{
			var from = "Domicilié à :<tab/>";
			if (person.GetSex () == PersonSex.Female)
			{
				from = "Domiciliée à :<tab/>";
			}

			from += person.GetTown ();

			return from;
		}

		private string GetSonOfLine(AiderEventEntity act, EventParticipantRole role)
		{
			var actorSex = act.GetActorSex (role);
			var filiation = "Fils de :<tab/>";
			if (actorSex == PersonSex.Female)
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActorFullName (EventParticipantRole.Father);
			var mother = act.GetActorFullName (EventParticipantRole.Mother);
			if (!string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father + " et " + mother;
			}

			if (!string.IsNullOrWhiteSpace (father) && string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father;
			}

			if (string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + mother;
			}

			return "";
		}

		private string GetHusbandSonOfLine(AiderEventEntity act)
		{
			var spouseSex = act.GetActorSex (EventParticipantRole.Husband);
			var filiation = "Fils de :<tab/>";
			if (spouseSex == PersonSex.Female)
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActorFullName (EventParticipantRole.HusbandFather);
			var mother = act.GetActorFullName (EventParticipantRole.HusbandMother);
			if (!string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father + " et " + mother;
			}

			if (!string.IsNullOrWhiteSpace (father) && string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father;
			}

			if (string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + mother;
			}

			return "";
		}

		private string GetSpouseSonOfLine(AiderEventEntity act)
		{
			var spouseSex = act.GetActorSex (EventParticipantRole.Spouse);
			var filiation = "Fils de :<tab/>";
			if (spouseSex == PersonSex.Female)
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActorFullName (EventParticipantRole.SpouseFather);
			var mother = act.GetActorFullName (EventParticipantRole.SpouseMother);
			if (!string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father + " et " + mother;
			}

			if (!string.IsNullOrWhiteSpace (father) && string.IsNullOrWhiteSpace (mother))
			{
				return filiation + father;
			}

			if (string.IsNullOrWhiteSpace (father) && !string.IsNullOrWhiteSpace (mother))
			{
				return filiation + mother;
			}

			return "";
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

			setup.TextStyle.TabInsert (new TextStyle.Tab (20.0.Millimeters (), TextTabType.Left, TextTabLine.Dot));
			setup.TextStyle.TabInsert (new TextStyle.Tab (90.0.Millimeters (), TextTabType.Left, TextTabLine.Dot));
			setup.TextStyle.TabInsert (new TextStyle.Tab (110.0.Millimeters (), TextTabType.Left, TextTabLine.Dot));
			setup.TextStyle.Font = Font.GetFont ("Verdana", "");
			setup.TextStyle.FontSize = 9.0.Points ();
			
			return setup;
		}
	}
}

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
		private void AddContentLines(List<AiderPersonEntity> actors, System.Tuple<string, string, string> formulas, List<string> lines)
		{
			if (actors.Count > 1)
			{
				lines.Add ("<br/><br/><tab/><b>" + formulas.Item3 + "</b><br/><br/>");
			}
			else
			{
				if (this.IsFemale (actors[0]))
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
			lines.Add (this.GetWhen (act) + this.Tabs () + "lieu :<tab/>" + act.Place.Name);
			lines.Add (this.GetMinisterLine (act));
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
				case Enumerations.EventType.Baptism:
					lines.Add (this.GetGodFatherLine (act) + this.Tabs () + this.GetGodMotherLine (act));
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					lines.Add (this.GetFirstWitnessLine (act) + this.Tabs () + this.GetSecondWitnessLine (act));
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

		private void AddActorHeaderLines(AiderEventEntity act, AiderPersonEntity actor, List<string> lines)
		{
			lines.Add (this.GetLastNameLine (actor) + this.Tabs () + this.GetFirstNameLine (actor));
			lines.Add (this.GetBirthDateLine (actor) + this.Tabs () + this.GetTownLine (actor));
			lines.Add (this.GetParishLine (actor));
			switch (act.Type)
			{
				case Enumerations.EventType.Blessing:
				case Enumerations.EventType.Baptism:
				case Enumerations.EventType.Confirmation:
				case Enumerations.EventType.EndOfCatechism:
				case Enumerations.EventType.FuneralService:
					lines.Add (this.GetSonOfLine (actor, act));
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					if (act.GetParticipantByRole (Enumerations.EventParticipantRole.Husband).Person == actor)
					{
						lines.Add (this.GetHusbandSonOfLine (actor, act));
					}
					if (act.GetParticipantByRole (Enumerations.EventParticipantRole.Spouse).Person == actor)
					{
						lines.Add (this.GetSpouseSonOfLine (actor, act));
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
			var minister = act.GetMinister ();
			if (minister.Employee.IsNotNull ())
			{
				var position = minister.Employee.EmployeeType.ToString ();
				line += minister.GetFullName () + " (" + position + ")";
			}
			else
			{
				line += minister.GetFullName ();
			}
			return line;
		}

		private string GetFirstWitnessLine(AiderEventEntity act)
		{
			var line = "Premier témoin :<tab/>";
			var witness = act.GetActor (Enumerations.EventParticipantRole.FirstWitness);
			if (witness.IsNotNull ())
			{
				line += witness.GetFullName ();
			}
			else
			{
				return "";
			}

			return line;
		}

		private string GetSecondWitnessLine(AiderEventEntity act)
		{
			var line = "Second témoin :<tab/>";
			var witness = act.GetActor (Enumerations.EventParticipantRole.SecondWitness);
			if (witness.IsNotNull ())
			{
				line += witness.GetFullName ();
			}
			else
			{
				return "";
			}

			return line;
		}

		private string GetGodFatherLine(AiderEventEntity act)
		{
			var line = "Parrain :<tab/>";
			var gotFather = act.GetActor (Enumerations.EventParticipantRole.GodFather);
			if (gotFather.IsNotNull ())
			{
				line += gotFather.GetFullName ();
			}
			else
			{
				return "";
			}

			return line;
		}

		private string GetGodMotherLine(AiderEventEntity act)
		{
			var line = "Marraine :<tab/>";
			var gotMother = act.GetActor (Enumerations.EventParticipantRole.GodMother);
			if (gotMother.IsNotNull ())
			{
				line += gotMother.GetFullName ();
			}
			else
			{
				return "";
			}

			return line;
		}
		
		private string GetParishLine(AiderPersonEntity person)
		{
			var parish = "Paroisse :<tab/>";
			parish += person.ParishGroup.Name;
			return parish;
		}

		private string GetBirthDateLine(AiderPersonEntity person)
		{
			var bd = "Né le :<tab/>";
			if (this.IsFemale (person))
			{
				bd = "Née le :<tab/>";
			}

			bd += person.eCH_Person.PersonDateOfBirth.Value.ToShortDateString ();
			return bd;
		}

		private string GetFirstNameLine(AiderPersonEntity person)
		{
			var firstName = "Prénom :<tab/><b>";
			var names = person.eCH_Person.PersonFirstNames.Split (' ');
			firstName += names[0] + "</b> " + string.Join (" ", names.Skip (1));
			return firstName;
		}

		private string GetLastNameLine(AiderPersonEntity person)
		{
			var firstName = "Nom :<tab/><b>";
			firstName += person.eCH_Person.PersonOfficialName + "</b>";
			return firstName;
		}

		private string GetTownLine(AiderPersonEntity person)
		{
			var from = "Domicilié à :<tab/>";
			if (this.IsFemale (person))
			{
				from = "Domiciliée à :<tab/>";
			}

			if (person.IsGovernmentDefined && person.IsDeclared)
			{
				from += person.eCH_Person.GetAddress ().Town;				
			}
			else
			{
				from += person.MainContact.GetAddress ().Town.Name;
			}

			return from;
		}

		private string GetSonOfLine(AiderPersonEntity person, AiderEventEntity act)
		{
			var filiation = "Fils de :<tab/>";
			if (this.IsFemale (person))
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActor (Enumerations.EventParticipantRole.Father);
			var mother = act.GetActor (Enumerations.EventParticipantRole.Mother);
			if (father != null && mother != null)
			{
				return filiation + father.GetFullName () + " et " + mother.GetFullName ();
			}

			if (father != null && mother == null)
			{
				return filiation + father.GetFullName ();
			}

			if (father == null && mother != null)
			{
				return filiation + mother.GetFullName ();
			}

			return "";
		}

		private string GetHusbandSonOfLine(AiderPersonEntity person, AiderEventEntity act)
		{
			var filiation = "Fils de :<tab/>";
			if (this.IsFemale (person))
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActor (Enumerations.EventParticipantRole.HusbandFather);
			var mother = act.GetActor (Enumerations.EventParticipantRole.HusbandMother);
			if (father != null && mother != null)
			{
				return filiation + father.GetFullName () + " et " + mother.GetFullName ();
			}

			if (father != null && mother == null)
			{
				return filiation + father.GetFullName ();
			}

			if (father == null && mother != null)
			{
				return filiation + mother.GetFullName ();
			}

			return "";
		}

		private string GetSpouseSonOfLine(AiderPersonEntity person, AiderEventEntity act)
		{
			var filiation = "Fils de :<tab/>";
			if (this.IsFemale (person))
			{
				filiation = "Fille de :<tab/>";
			}

			var father = act.GetActor (Enumerations.EventParticipantRole.SpouseFather);
			var mother = act.GetActor (Enumerations.EventParticipantRole.SpouseMother);
			if (father != null && mother != null)
			{
				return filiation + father.GetFullName () + " et " + mother.GetFullName ();
			}

			if (father != null && mother == null)
			{
				return filiation + father.GetFullName ();
			}

			if (father == null && mother != null)
			{
				return filiation + mother.GetFullName ();
			}

			return "";
		}

		private bool IsFemale (AiderPersonEntity person)
		{
			return person.eCH_Person.PersonSex == Enumerations.PersonSex.Female;
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

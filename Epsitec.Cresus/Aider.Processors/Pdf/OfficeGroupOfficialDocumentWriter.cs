//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Pdf.LetterDocument;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Pdf
{
	internal sealed class OfficeGroupOfficialDocumentWriter
	{
		public OfficeGroupOfficialDocumentWriter(BusinessContext context, AiderOfficeSenderEntity sender, LabelLayout layout)
		{
			this.context = context;
			this.layout	 = layout;
			this.sender  = sender;
		}

		public void WriteStream(System.IO.Stream stream, AiderOfficeGroupParticipantReportEntity officeReport)
		{
			var setup   = new TextDocumentSetup ();
			var report  = this.GetReport (setup);
			var content = new System.Text.StringBuilder ();

			content.Append (officeReport.GetReportContent ());

			var no = 0;

			foreach (var participant in officeReport.Participants)
			{
				var contact		= participant.Contact;
				var person		= contact.Person;
				var address		= contact.Address;
				var fullName	= person.GetFullName ();
				var street		= address.StreetUserFriendly;
				var streetNo	= address.StreetHouseNumberAndComplement;
				var zip			= address.GetDisplayZipCode();
				var town		= address.Town.Name;
				var bDate		= person.BirthdayDay + "." + person.BirthdayMonth + "." + person.BirthdayYear;

				no++;
				content.Append (no + "." + fullName + ", " + street + " " + streetNo + ", " + zip + " " + town + " - " + bDate + "<br/>");
			}

			var topLogo			= string.Format ("<img src=\"{0}\" width=\"378\" height=\"298\"/>",@"S:\Epsitec.Cresus\Aider\Images\logo.png");
			var topReference	= "<b>" + this.sender.Office.OfficeName + "</b>";
			var bottomReference	= "Extrait d'AIDER le " + System.DateTime.Now.ToString ("d MMM yyyy");
			
			report.AddTopLeftLayer (topLogo + topReference, 100);
			report.AddBottomRightLayer (bottomReference, 100);
			report.GeneratePdf (stream,content.ToString ());
		}

		private TextDocument GetReport(TextDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new TextDocument (exportPdfInfo, setup);
		}

		private string BuildAddress(AiderContactEntity contact, bool withAddressLine)
		{
			var sb = new System.Text.StringBuilder ();
			if (withAddressLine)
			{
				if (!string.IsNullOrEmpty (contact.Address.AddressLine1))
				{
					sb.Append (contact.Address.AddressLine1 + "<br/>");
				}			
			}

			sb.Append (contact.Person.GetFullName () + "<br/>");

			if (!string.IsNullOrEmpty (contact.Address.PostBox))
			{
				sb.Append (contact.Address.PostBox + "<br/>");
			}

			sb.Append (contact.Address.StreetUserFriendly + "<br/>");

			if(contact.Address.Town.Country.IsoCode != "CH")
			{
				sb.Append(contact.Address.Town.Country.IsoCode  + "-" + contact.Address.GetDisplayZipCode ()); 
			}
			else
			{
				sb.Append (contact.Address.GetDisplayZipCode ());
				
			}

			sb.Append (" " + contact.Address.Town.Name);
			return sb.ToString ();
		}

		private readonly BusinessContext		 context;
		private readonly AiderOfficeSenderEntity sender;
		private readonly LabelLayout			 layout;
	}
}

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
			var setup					= new LetterDocumentSetup ();
			var report					= this.GetReport (setup);

			var contentTemplateBuilder = new System.Text.StringBuilder ();

			contentTemplateBuilder.Append (this.letter.GetLetterContent ());

			var topLogo			= "";
			var topReference	= "<b>" + this.settings.Office.OfficeName + "</b>";

			report.GeneratePdf (stream, topLogo ,topReference, this.BuildAddress (this.settings.OfficialContact, false), this.BuildAddress (this.letter.RecipientContact, true), contentTemplateBuilder.ToString ());
		}

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
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

		private readonly AiderOfficeLetterReportEntity		letter;
		private readonly AiderOfficeSenderEntity			settings;
		private readonly LabelLayout						layout;
	}
}

//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeLetterReportEntity
	{
		public static AiderOfficeLetterReportEntity Create(BusinessContext context, AiderContactEntity recipient, AiderOfficeSenderEntity sender, string documentName, string templateName, FormattedContent content)
		{
			var letter = context.CreateAndRegisterEntity<AiderOfficeLetterReportEntity> ();

			letter.Name				= documentName;
			letter.CreationDate		= Date.Today;
			letter.TownAndDate		= ReportBuilder.GetTownAndDate (sender.Office.OfficeMainContact.Address, letter.CreationDate);
			letter.RecipientContact = recipient;
			letter.Office			= sender.Office;
			letter.DataTemplate     = templateName;
			
			letter.SetContent (content);

			sender.Office.AddDocumentInternal (letter);
			
			return letter;
		}
	}
}

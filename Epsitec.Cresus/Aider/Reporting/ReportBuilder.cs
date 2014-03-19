//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	public static class ReportBuilder
	{
		public static FormattedText GetCompactAddress(AiderContactEntity contact)
		{
			return ReportBuilder.GetAddress (contact, withAddressLine: false);
		}

		public static FormattedText GetFullAddress(AiderContactEntity contact)
		{
			return ReportBuilder.GetAddress (contact, withAddressLine: true);
		}


		private static FormattedText GetAddress(AiderContactEntity contact, bool withAddressLine)
		{
			var buffer = new System.Text.StringBuilder ();

			switch (contact.ContactType)
			{
				case Enumerations.ContactType.Legal:
					buffer.Append (FormattedText.Escape (contact.LegalPerson.Name));
					buffer.Append ("<br/>");
					break;

				case Enumerations.ContactType.PersonHousehold:
				case Enumerations.ContactType.Deceased:
				case Enumerations.ContactType.PersonAddress:
					buffer.Append (contact.Person.MrMrs == Enumerations.PersonMrMrs.Monsieur ? "Monsieur" : "Madame");
					buffer.Append ("<br/>");
					buffer.Append (FormattedText.Escape (contact.Person.CallNameDisplay + " " + contact.Person.eCH_Person.PersonOfficialName));
					buffer.Append ("<br/>");
					break;

				default:
					break;
			}

			if (withAddressLine)
			{
				buffer.Append (contact.Address.GetPostalAddress ().ToString ());
			}
			else
			{
				buffer.Append (contact.Address.GetShortPostalAddress ().ToString ());
			}
			
			return buffer.ToString ();
		}


		public static FormattedText GetTownAndDate(AiderAddressEntity address, Date date)
		{
			return FormattedText.Escape (address.Town.Name) + ", le " + date.ToString ("dd MMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
		}

		public static string GetTemplate(string templateName)
		{
			if (string.IsNullOrEmpty (templateName))
			{
				return null;
			}

			var path = CoreContext.GetFileDepotPath ("assets", templateName + ".txt");

			if (System.IO.File.Exists (path))
			{
				return System.IO.File.ReadAllText (path, System.Text.Encoding.UTF8);
			}

			return null;
		}

		public static FormattedText GetReport(string templateName, IContent content)
		{
			if (content == null)
			{
				return FormattedText.Null;
			}

			var template = ReportBuilder.GetTemplate (templateName);

			if (template == null)
			{
				return FormattedText.Null;
			}

			return content.GetContentText (template.Replace ("\r\n", ""));
		}

		public static FormattedText GetReport(string templateName, string format, byte[] blob)
		{
			IContent content = null;

			switch (format)
			{
				case "FormattedContent":
					content = new FormattedContent ();
					break;

				default:
					break;
			}

			if (content == null)
			{
				return FormattedText.Null;
			}

			content.Setup (blob);

			return ReportBuilder.GetReport (templateName, content);
		}
	}
}

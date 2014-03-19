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
		public static string GetCompactAddress(AiderContactEntity contact)
		{
			return ReportBuilder.GetAddress (contact, withAddressLine: false);
		}

		public static string GetFullAddress(AiderContactEntity contact)
		{
			return ReportBuilder.GetAddress (contact, withAddressLine: true);
		}


		private static string GetAddress(AiderContactEntity contact, bool withAddressLine)
		{
			var buffer = new System.Text.StringBuilder ();

			if (withAddressLine)
			{
				if (!string.IsNullOrEmpty (contact.Address.AddressLine1))
				{
					buffer.Append (contact.Address.AddressLine1 + "<br/>");
				}
			}

			switch (contact.ContactType)
			{
				case Enumerations.ContactType.Legal:
					buffer.Append (contact.LegalPerson.Name + "<br/>");
					break;
				
				case Enumerations.ContactType.PersonAddress:
					buffer.Append (contact.Person.GetFullName () + "<br/>");
					break;
				
				case Enumerations.ContactType.PersonHousehold:
					buffer.Append (contact.Household.DisplayName + "<br/>");
					break;
				
				default:
					buffer.Append (contact.Person.GetFullName () + "<br/>");
					break;
			}

			if (!string.IsNullOrEmpty (contact.Address.PostBox))
			{
				buffer.Append ("CP " + contact.Address.PostBox + "<br/>");
			}

			buffer.Append (contact.Address.StreetUserFriendly + "<br/>");

			if (contact.Address.Town.Country.IsoCode != "CH")
			{
				buffer.Append (contact.Address.Town.Country.IsoCode  + "-" + contact.Address.GetDisplayZipCode ());
			}
			else
			{
				buffer.Append (contact.Address.GetDisplayZipCode ());
			}

			buffer.Append (" " + contact.Address.Town.Name);
			return buffer.ToString ();
		}


		public static string GetTownAndDate(AiderAddressEntity address, Date date)
		{
			return address.Town.Name + ", le " + date.ToString ("dd MMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
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

			return content.GetContentText (template);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Entities;

namespace Epsitec.Aider.Processors.Helpers
{
	public static class Builders
	{
		public static string BuildAddress(AiderContactEntity contact, bool withAddressLine)
		{
			var sb = new System.Text.StringBuilder ();
			if (withAddressLine)
			{
				if (!string.IsNullOrEmpty (contact.Address.AddressLine1))
				{
					sb.Append (contact.Address.AddressLine1 + "<br/>");
				}
			}

			switch (contact.ContactType)
			{
				case Enumerations.ContactType.Legal:
					sb.Append (contact.LegalPerson.Name + "<br/>");
					break;
				case Enumerations.ContactType.PersonAddress:
					sb.Append (contact.Person.GetFullName () + "<br/>");
					break;
				case Enumerations.ContactType.PersonHousehold:
					sb.Append (contact.Household.DisplayName + "<br/>");
					break;
				default:
					sb.Append (contact.Person.GetFullName () + "<br/>");
					break;
			}

			if (!string.IsNullOrEmpty (contact.Address.PostBox))
			{
				sb.Append ("CP " + contact.Address.PostBox + "<br/>");
			}

			sb.Append (contact.Address.StreetUserFriendly + "<br/>");

			if (contact.Address.Town.Country.IsoCode != "CH")
			{
				sb.Append (contact.Address.Town.Country.IsoCode  + "-" + contact.Address.GetDisplayZipCode ());
			}
			else
			{
				sb.Append (contact.Address.GetDisplayZipCode ());

			}

			sb.Append (" " + contact.Address.Town.Name);
			return sb.ToString ();
		}
	}
}

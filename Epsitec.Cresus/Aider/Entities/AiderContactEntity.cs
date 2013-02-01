//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderContactEntity
	{
		public override FormattedText GetSummary()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return Resources.FormattedText ("Ce contact n'est pas défini.<br/><br/>Utilisez une <i>action</i> pour associer ce contact à une personne ou à une entreprise.");
			}

			return TextFormatter.FormatText (this.DisplayName, "\n", this.DisplayZipCode, this.DisplayAddress);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}

		public void RefreshCache()
		{
			if (this.Person.IsNotNull ())
			{
				this.DisplayName = this.Person.DisplayName;
			}
			else
			{
				this.DisplayName = "—";
			}

			if (this.Household.IsNotNull ())
			{
				this.Address = this.Household.Address;
			}

			if ((this.Address.IsNotNull ()) &&
				(this.Address.Town.IsNotNull ()))
			{
				this.RefreshDisplayAddressAndZipCode (this.Address);
			}
			else
			{
				this.DisplayAddress = "";
				this.DisplayZipCode = "";
			}
		}
		
		private void RefreshDisplayAddressAndZipCode(AiderAddressEntity address)
		{
			var town    = address.Town;
			var country = town.Country;

			this.DisplayAddress = address.GetDisplayAddress ().ToSimpleText ();
			this.DisplayZipCode = country.IsoCode == "CH" ? town.ZipCode : country.IsoCode + "-" + town.ZipCode;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderHouseholdEntity household, HouseholdRole role)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.PersonHousehold);

			contact.Person = person;
			contact.Address = household.Address;
			contact.Household = household;
			contact.HouseholdRole = role;

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AddressType type)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.PersonAddress);

			contact.Person = person;
			contact.Address = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();
			contact.AddressType = type;

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.Legal);

			contact.LegalPerson = legalPerson;
			contact.Address = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();

			return contact;
		}

		private static AiderContactEntity Create(BusinessContext businessContext, ContactType type)
		{
			var contact = businessContext.CreateAndRegisterEntity<AiderContactEntity> ();

			contact.ContactType = type;

			return contact;
		}
	}
}

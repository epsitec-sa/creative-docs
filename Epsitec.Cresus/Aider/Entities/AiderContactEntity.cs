//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;
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
			this.Address = this.GetAddress ();

			this.DisplayName          = this.GetDisplayName ();
			this.DisplayZipCode       = this.GetDisplayZipCode ();
			this.DisplayAddress       = this.GetDisplayAddress ();
			this.DisplayVisibility    = this.GetDisplayVisibilityStatus ();
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (this.GetParishGroup ());

			if (this.Person.IsNotNull ())
			{
				this.PersonMrMrs    = this.Person.MrMrs;
				this.PersonFullName = string.Concat (this.Person.CallName, " ", this.Person.eCH_Person.PersonOfficialName);
			}
		}


		/// <summary>
		/// You should always use this method to get the address of the entity, unless you are sure
		/// that the entity has not be modified before you access the address property. The only
		/// places where you can is probably in brick wall for the controllers and when you just
		/// have loaded the entity in memory from a fresh BusinessContext.
		/// </summary>
		public AiderAddressEntity GetAddress()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return null;

				case ContactType.Legal:
					return this.LegalPerson.Address;

				case ContactType.PersonAddress:
					return this.Address;

				case ContactType.PersonHousehold:
					return this.Household.Address;

				default:
					throw new NotImplementedException ();
			}
		}


		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderHouseholdEntity household, bool isHead)
		{
			var role = isHead
				? HouseholdRole.Head
				: HouseholdRole.None;

			return AiderContactEntity.Create (businessContext, person, household, role);
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderHouseholdEntity household, HouseholdRole role)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.PersonHousehold);

			contact.Person        = person;
			contact.Household     = household;
			contact.HouseholdRole = role;

			person.AddContactInternal (contact);
			household.AddContactInternal (contact);

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AddressType type)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.PersonAddress);

			contact.Person      = person;
			contact.Address     = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();
			contact.AddressType = type;

			person.AddContactInternal (contact);

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			var contact = AiderContactEntity.Create (businessContext, ContactType.Legal);

			contact.LegalPerson = legalPerson;

			legalPerson.AddContactInternal (contact);

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderLegalPersonEntity legalPerson, PersonMrMrs personMrMrs, string personFullName, ContactRole personRole = ContactRole.None)
		{
			var contact = AiderContactEntity.Create (businessContext, legalPerson);

			if (string.IsNullOrEmpty (personFullName) == false)
			{
				contact.PersonMrMrs            = personMrMrs;
				contact.PersonFullName         = personFullName;
				contact.LegalPersonContactRole = personRole;
				contact.LegalPersonPrincipal   = true;
			}

			return contact;
		}


		public static void Delete(BusinessContext businessContext, AiderContactEntity contact)
		{
			if (contact.ContactType == ContactType.PersonAddress)
			{
				businessContext.DeleteEntity (contact.Address);
			}

			var person = contact.Person;
			if (person.IsNotNull ())
			{
				person.RemoveContactInternal (contact);
			}

			var household = contact.Household;
			if (household.IsNotNull ())
			{
				household.RemoveContactInternal (contact);
			}

			var legalPerson = contact.LegalPerson;
			if (legalPerson.IsNotNull ())
			{
				legalPerson.RemoveContactInternal (contact);
			}

			businessContext.DeleteEntity (contact);
		}


		private string GetDisplayName()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return this.GetNoneDisplayName ();

				case ContactType.Legal:
					return this.GetLegalDisplayName ();

				case ContactType.PersonAddress:
					return this.GetPersonAddressDisplayName ();

				case ContactType.PersonHousehold:
					return this.GetPersonHouseholdDisplayName ();

				default:
					throw new NotImplementedException ();
			}
		}

		private string GetNoneDisplayName()
		{
			return "—";
		}

		private string GetLegalDisplayName()
		{
			var name = this.LegalPerson.Name;

			if (this.Person.IsNotNull ())
			{
				name += " (" + this.Person.GetDisplayName () + ")";
			}

			return name;
		}

		private string GetPersonAddressDisplayName()
		{
			return this.Person.GetDisplayName () + " (°)";
		}

		private string GetPersonHouseholdDisplayName()
		{
			return this.Person.GetDisplayName ();
		}


		private string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}

		private string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}

		private PersonVisibilityStatus GetDisplayVisibilityStatus()
		{
			var hidden = (this.Person.IsNotNull () && this.Person.Visibility != PersonVisibilityStatus.Default)
				|| (this.Household.IsNotNull () && this.Household.DisplayVisibility != PersonVisibilityStatus.Default)
				|| (this.LegalPerson.IsNotNull () && this.LegalPerson.Visibility != PersonVisibilityStatus.Default);

			return hidden
				? PersonVisibilityStatus.Hidden
				: PersonVisibilityStatus.Default;
		}

		private AiderGroupEntity GetParishGroup()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return null;

				case ContactType.Legal:
					return this.LegalPerson.ParishGroup;

				case ContactType.PersonAddress:
				case ContactType.PersonHousehold:
					return this.Person.Parish.Group;

				default:
					throw new NotImplementedException ();
			}
		}


		private static AiderContactEntity Create(BusinessContext businessContext, ContactType type)
		{
			var contact = businessContext.CreateAndRegisterEntity<AiderContactEntity> ();

			contact.ContactType = type;

			return contact;
		}
	}
}

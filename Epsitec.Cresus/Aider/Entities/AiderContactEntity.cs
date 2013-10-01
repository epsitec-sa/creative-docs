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
		public bool HasFullAddress()
		{
			var address = this.GetAddress ();

			if (address.IsNull ())
			{
				return false;
			}

			return address.Town.IsNotNull ()
				&& ((string.IsNullOrEmpty (address.Street) == false) || (string.IsNullOrEmpty (address.PostBox) == false));
		}
		
		
		public override FormattedText GetSummary()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return Resources.FormattedText ("Ce contact n'est pas défini.<br/><br/>Utilisez une <i>action</i> pour associer ce contact à une personne ou à une entreprise.");
			}

			return TextFormatter.FormatText (this.DisplayName, "\n", this.DisplayZipCode, this.DisplayAddress);
		}

		public FormattedText GetAddressLabelText()
		{
			return this.GetAddressLabelText (this.GetAddressRecipientText ());
		}

		private string GetAddressRecipientText()
		{
			switch (this.ContactType)
			{
				case Enumerations.ContactType.Legal:
					return this.GetLegalPersonRecipientText ();

				case Enumerations.ContactType.PersonAddress:
				case Enumerations.ContactType.PersonHousehold:
					return this.GetPersonRecipientText ();

				case Enumerations.ContactType.Deceased:
				default:
					// Is that right or should we throw here?
					return "";
			}
		}

		private string GetLegalPersonRecipientText()
		{
			return StringUtils.Join
			(
				"\n",
				this.LegalPersonContactMrMrs.GetLongText (),
				this.LegalPersonContactFullName,
				this.LegalPerson.Name
			);
		}

		private string GetPersonRecipientText()
		{
			return this.GetPersonRecipientText (this.Person.MrMrs.GetLongText ());
		}

		public FormattedText GetAddressOfParentsLabelText()
		{
			return this.GetAddressLabelText (this.GetAddressRecipientParentText ());
		}

		private string GetAddressRecipientParentText()
		{
			switch (this.ContactType)
			{
				case Enumerations.ContactType.Legal:
					// This case would be wierd, so we return the normal text, and not the text with
					// the parent stuff, as it would make no sense for a legal person.
					return this.GetLegalPersonRecipientText ();

				case Enumerations.ContactType.PersonAddress:
				case Enumerations.ContactType.PersonHousehold:
					return this.GetPersonParentRecipientText ();

				case Enumerations.ContactType.Deceased:
				default:
					// Is that right or should we throw here?
					return "";
			}
		}

		private string GetPersonParentRecipientText()
		{
			return this.GetPersonRecipientText ("Aux parents de");
		}

		private string GetPersonRecipientText(string title)
		{
			return StringUtils.Join ("\n", title, this.Person.GetFullName ());
		}

		private FormattedText GetAddressLabelText(string recipient)
		{
			return TextFormatter.FormatText
			(
				recipient,
				"\n",
				this.Address.GetPostalAddress ()
			);
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
				case ContactType.Deceased:
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


		partial void GetFullAddressTextSingleLine(ref string value)
		{
			this.GetFullAddressTextMultiLine (ref value);

			value = value.Replace ("\n", ", ");
		}


		partial void SetFullAddressTextSingleLine(string value)
		{
			throw new NotImplementedException ("Do not use this method");
		}


		partial void GetFullAddressTextMultiLine(ref string value)
		{
			value = this.GetAddressLabelText ().ToSimpleText ();
		}


		partial void SetFullAddressTextMultiLine(string value)
		{
			throw new NotImplementedException ("Do not use this method");
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
			if (person.IsDeceased)
			{
				throw new System.ArgumentException ("Cannot create contact for a dead person.");
			}
			
			var contact = AiderContactEntity.Create (businessContext, ContactType.PersonHousehold);

			contact.Person        = person;
			contact.Household     = household;
			contact.HouseholdRole = role;

			person.AddContactInternal (contact);
			household.AddContactInternal (contact);

			return contact;
		}

		public static AiderContactEntity CreateDeceased(BusinessContext businessContext, AiderPersonEntity person)
		{
			if (person.IsAlive)
			{
				throw new System.ArgumentException ("Cannot create contact for a living person.");
			}

			var contact = AiderContactEntity.Create (businessContext, ContactType.Deceased);

			contact.Person = person;

			person.AddContactInternal (contact);

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AddressType type)
		{
			if (person.IsDeceased)
			{
				throw new System.ArgumentException ("Cannot create contact for a dead person.");
			}

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
				contact.LegalPersonContactMrMrs     = personMrMrs;
				contact.LegalPersonContactFullName  = personFullName;
				contact.LegalPersonContactRole      = personRole;
				contact.LegalPersonContactPrincipal = true;
			}

			return contact;
		}

		public static void DeleteParticipations(BusinessContext businessContext, AiderContactEntity contact)
		{
			var example = new AiderGroupParticipantEntity ()
			{
				Contact = contact
			};

			var participations = businessContext.DataContext.GetByExample (example);

			foreach (var participation in participations)
			{
				businessContext.DeleteEntity (participation);
			}
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

			AiderContactEntity.DeleteParticipations (businessContext, contact);
			
			businessContext.DeleteEntity (contact);
		}


		public string GetDisplayName()
		{
			switch (this.ContactType)
			{
				case ContactType.None:
					return this.GetNoneDisplayName ();

				case ContactType.Legal:
					return this.GetLegalDisplayName ();

				case ContactType.PersonAddress:
					return this.GetPersonAddressDisplayName ();

				case ContactType.Deceased:
					return this.GetPersonDeceasedDisplayName ();
					
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

			if (!string.IsNullOrEmpty (this.LegalPersonContactFullName))
			{
				name += " (" + this.LegalPersonContactFullName + ")";
			}

			return name;
		}

		private string GetPersonAddressDisplayName()
		{
			return this.Person.GetDisplayName () + " (°)";
		}

		private string GetPersonDeceasedDisplayName()
		{
			return this.Person.GetDisplayName ();
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
			var hidden =
				(this.Person.IsNotNull () && this.Person.Visibility != PersonVisibilityStatus.Default) ||
				(this.Household.IsNotNull () && this.Household.DisplayVisibility != PersonVisibilityStatus.Default) ||
				(this.LegalPerson.IsNotNull () && this.LegalPerson.Visibility != PersonVisibilityStatus.Default);

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

				case ContactType.Deceased:
				case ContactType.PersonAddress:
				case ContactType.PersonHousehold:
					return this.Person.ParishGroup;

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

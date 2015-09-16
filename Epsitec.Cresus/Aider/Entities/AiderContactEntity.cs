//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Helpers;

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

		public FormattedText GetAddressLabelText(PostalAddressType type = PostalAddressType.Default)
		{
			return this.GetAddressLabelText (this.GetAddressRecipientText (), type);
		}

		public FormattedText GetCustomAddressLabelText(string customRecipient, PostalAddressType type = PostalAddressType.Default)
		{
			return this.GetAddressLabelText (customRecipient, type);
		}

		public string GetAddressRecipientText()
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
			List<string> lines = new List<string> ();

			lines.Add (AiderContactEntity.GetContactTitleLongText (this.LegalPersonContactMrMrs, this.ContactTitle));
			lines.Add (this.LegalPersonContactFullName);
			lines.AddRange (this.LegalPerson.GetNameLines ());

			return StringUtils.Join ("\n", lines);
		}

		private string GetPersonRecipientText()
		{
			return this.GetPersonRecipientText (AiderContactEntity.GetContactTitleLongText (this.Person.MrMrs, this.ContactTitle));
		}

		public FormattedText GetAddressOfParentsLabelText(PostalAddressType type = PostalAddressType.Default)
		{
			return this.GetAddressLabelText (this.GetAddressRecipientParentText (), type);
		}

		public string GetAddressRecipientParentText()
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

		private FormattedText GetAddressLabelText(string recipient, PostalAddressType type)
		{
			return TextFormatter.FormatText (recipient, "\n", this.Address.GetPostalAddress (type));
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

		public void RefreshRoleCache(DataContext dataContext)
		{
			var participations = this.FindRoleCacheParticipations (dataContext);

			foreach (var participation in participations)
			{
				var role		= AiderParticipationsHelpers.BuildRoleFromParticipation (participation)
																.GetRole (participation);

				var rolePath	= AiderParticipationsHelpers.GetRolePath (participation);

				participation.RoleCache		= role;
				participation.RolePathCache = rolePath;
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
				case ContactType.Deceased:
					return this.Address;

				case ContactType.PersonHousehold:
					return this.Household.Address;

				default:
					throw new System.NotImplementedException ();
			}
		}


		internal void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Add (participation);
		}

		internal void RemoveParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Remove (participation);
		}



		partial void GetFullAddressTextSingleLine(ref string value)
		{
			this.GetFullAddressTextMultiLine (ref value);

			value = value.Replace ("\n", ", ");
		}

		partial void SetFullAddressTextSingleLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}

		partial void GetFullAddressTextMultiLine(ref string value)
		{
			value = this.GetAddressLabelText ().ToSimpleText ();
		}

		partial void SetFullAddressTextMultiLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}

		partial void GetGroups(ref IList<AiderGroupParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		partial void GetDebugIds(ref string value)
		{
			EntityKey? keyPerson    = null;
			EntityKey? keyHousehold = null;

			var person    = this.Person;
			var household = this.Household;

			this.ExecuteWithDataContext (
				context =>
				{
					keyPerson    = person.IsNull () ? null : context.GetNormalizedEntityKey (person);
					keyHousehold = household.IsNull () ? null : context.GetNormalizedEntityKey (household);
				});

			var buffer = new System.Text.StringBuilder ();

			if (keyPerson.HasValue)
			{
				buffer.Append (keyPerson.Value.ToString ());
			}
			else
			{
				buffer.Append ("<null>");
			}

			buffer.Append (":");

			if (keyHousehold.HasValue)
			{
				buffer.Append (keyHousehold.Value.ToString ());
			}
			else
			{
				buffer.Append ("<null>");
			}

			value = buffer.ToString ();
		}


		public static string GetContactTitleShortText(PersonMrMrs? personMrMrs, PersonMrMrsTitle? personTitle)
		{
			if (personTitle.HasValue && (personTitle.Value != PersonMrMrsTitle.Auto))
			{
				return personTitle.GetShortText ();
			}
			else
			{
				return personMrMrs.GetShortText ();
			}
		}

		public static string GetContactTitleLongText(PersonMrMrs? personMrMrs, PersonMrMrsTitle? personTitle)
		{
			if (personTitle.HasValue && (personTitle.Value != PersonMrMrsTitle.Auto))
			{
				return personTitle.GetLongText ();
			}
			else
			{
				return personMrMrs.GetLongText ();
			}
		}


		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderHouseholdEntity household, bool isHead)
		{
			var role = isHead
				? HouseholdRole.Head
				: HouseholdRole.None;

			return AiderContactEntity.Create (businessContext, person, household, role);
		}

		public static AiderContactEntity ChangeHousehold(BusinessContext businessContext, AiderContactEntity contact, AiderHouseholdEntity newHousehold, bool isHead)
		{
			var role = isHead
				? HouseholdRole.Head
				: HouseholdRole.None;

			contact.Household.RemoveContactInternal (contact);
			AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, contact.Household);
			contact.Person.RemoveContactInternal (contact);

			contact.Household     = newHousehold;
			contact.HouseholdRole = role;

			contact.Person.AddContactInternal (contact);
			newHousehold.AddContactInternal (contact);


			var newSubscription = AiderSubscriptionEntity.FindSubscription (businessContext, newHousehold);
			if (newSubscription.IsNotNull ())
			{
				newSubscription.RefreshCache ();
			}

			return contact;
		}

		public static AiderContactEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderHouseholdEntity household, HouseholdRole role)
		{
			if (person.IsDeceased)
			{
				throw new System.ArgumentException ("Cannot create contact for a dead person.");
			}

			if (person.eCH_Person.AdultMaritalStatus == PersonMaritalStatus.None)
			{
				role = HouseholdRole.None;
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

			contact.Person      = person;
			contact.Address     = AiderAddressEntity.Create (businessContext, person.Address);
			contact.AddressType = AddressType.LastKnow;
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
			var groups = contact.Groups.ToList ();

			foreach (var group in groups)
			{
				contact.RemoveParticipationInternal (group);

				businessContext.DeleteEntity (group);
			}
		}

		public static void Delete(BusinessContext businessContext, AiderContactEntity contact, bool deleteParticipations = true)
		{
			if (contact.IsNull ())
			{
				return;
			}
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

				AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, household);
			}

			var legalPerson = contact.LegalPerson;
			if (legalPerson.IsNotNull ())
			{
				legalPerson.RemoveContactInternal (contact);
			}

			if (deleteParticipations)
			{
				AiderContactEntity.DeleteParticipations (businessContext, contact);
			}
			
			businessContext.DeleteEntity (contact);
		}
		
		public static void DeleteDuplicateContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contacts)
		{
			var processed = new List<AiderContactEntity> ();
			var participationsBackup = new List<AiderGroupParticipantEntity> ();
			foreach (var contact in contacts)
			{
				var type       = contact.ContactType;
				var candidates = processed.Where (x => x.ContactType == type);
				AiderContactEntity remove = null;

				foreach (var candidate in candidates)
				{	
					switch (type)
					{
						case ContactType.Deceased:
							if (candidate.Person == contact.Person)
							{
								remove = candidate;
								goto processDuplicate;
							}
							break;
						
						case ContactType.Legal:
							if ((candidate.LegalPerson == contact.LegalPerson) &&
								(candidate.LegalPersonContactFullName == contact.LegalPersonContactFullName) &&
								(candidate.LegalPersonContactMrMrs == contact.LegalPersonContactMrMrs) &&
								(candidate.LegalPersonContactPrincipal == contact.LegalPersonContactPrincipal) &&
								(candidate.LegalPersonContactRole == contact.LegalPersonContactRole))
							{
								remove = candidate;
								goto processDuplicate;
							}
							break;
						
						case ContactType.PersonAddress:
							if ((candidate.Person == contact.Person) &&
								(candidate.AddressType == contact.AddressType) &&
								(candidate.Address.GetSummary () == contact.Address.GetSummary ()))
							{
								remove = candidate;
								goto processDuplicate;
							}
							break;

						case ContactType.PersonHousehold:
							if ((candidate.Person == contact.Person) &&
								(candidate.Household == contact.Household))
							{
								remove = candidate;
								goto processDuplicate;
							}
							break;
						
						default:
							break;
					}
				}

				processed.Add (contact);

			processDuplicate:
				if (remove != null)
				{
					if (contact.participations != null)
					{
						participationsBackup.AddRange (contact.participations);
					}	
					AiderContactEntity.Delete (businessContext, contact);
				}
			}

			// restore backuped participations
			foreach (var participation in participationsBackup)
			{
				foreach (var contact in processed)
				{
					if (contact.participations != null)
					{
						if (!contact.participations.Contains (participation))
						{
							contact.AddParticipationInternal (participation);
						}
					} 
					else
					{
						contact.AddParticipationInternal (participation);
					}
				}
				
			}
			
		}

		public static void DeleteBadContact(BusinessContext businessContext, AiderContactEntity goodContact, AiderContactEntity badContact)
		{
			var participationsBackup = new List<AiderGroupParticipantEntity> ();

			if (badContact.participations != null)
			{
				participationsBackup.AddRange (badContact.participations);
			}
			AiderContactEntity.Delete (businessContext, badContact);
	
			// restore backuped participations
			foreach (var participation in participationsBackup)
			{

				if (goodContact.participations != null)
				{
					if (!goodContact.participations.Contains (participation))
					{
						goodContact.AddParticipationInternal (participation);
					}
				}
				else
				{
					goodContact.AddParticipationInternal (participation);
				}

			}
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
					throw new System.NotImplementedException ();
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
			return this.Person.GetDisplayName () + AiderContactEntity.AddressContactSuffix;
		}

		private string GetPersonDeceasedDisplayName()
		{
			return this.Person.GetDisplayName ();
		}

		private string GetPersonHouseholdDisplayName()
		{
			return this.Person.GetDisplayName ();
		}

		private IList<AiderGroupParticipantEntity> GetParticipations()
		{
			if (this.participations == null)
			{
				this.participations = this.ExecuteWithDataContext
				(
					d => this.FindParticipations (d),
					() => new List<AiderGroupParticipantEntity> ()
				);
			}

			return this.participations;
		}
		
		private IList<AiderGroupParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true);

			return dataContext
				.GetByRequest<AiderGroupParticipantEntity> (request)
				.OrderBy (g => g.GetSummaryWithHierarchicalGroupName ().ToString ())
				.ToList ();
		}

		private IList<AiderGroupParticipantEntity> FindRoleCacheParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateRoleCacheParticipantRequest (dataContext, this, true);
			
			return dataContext
				.GetByRequest<AiderGroupParticipantEntity> (request)
				.ToList ();
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
					throw new System.NotImplementedException ();
			}
		}


		private static AiderContactEntity Create(BusinessContext businessContext, ContactType type)
		{
			var contact = businessContext.CreateAndRegisterEntity<AiderContactEntity> ();

			contact.ContactType = type;

			return contact;
		}

		
		private IList<AiderGroupParticipantEntity>	participations;


		public static readonly string			AddressContactSuffix	= " (°)";
	}
}

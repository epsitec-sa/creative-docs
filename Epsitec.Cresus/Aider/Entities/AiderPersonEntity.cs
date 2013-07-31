//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonEntity : IAiderWarningExampleFactoryGetter
	{
		public bool								IsGovernmentDefined
		{
			get
			{
				return (this.eCH_Person.IsNotNull ())
					&& (this.eCH_Person.DataSource == Enumerations.DataSource.Government);
			}
		}

		public bool								IsAlive
		{
			get
			{
				return this.eCH_Person.IsNull () || this.eCH_Person.IsDeceased == false;
			}
		}

		public bool								IsDeceased
		{
			get
			{
				return !this.IsAlive;
			}
		}
		
		public int?								Age
		{
			get
			{
				if (this.eCH_Person.IsNull ())
				{
					return null;
				}

				var birthdate = this.eCH_Person.PersonDateOfBirth;
				var deathdate = this.eCH_Person.PersonDateOfDeath;

				if (birthdate == null)
				{
					return null;
				}
				else if (deathdate == null)
				{
					return birthdate.Value.ComputeAge ();
				}
				else
				{
					return birthdate.Value.ComputeAge (deathdate.Value);
				}
			}
		}


		public FormattedText GetCompactSummary(AiderHouseholdEntity household)
		{
			if ((household.IsNotNull ()) &&
				(household.IsHead (this)))
			{
				var boldName = TextFormatter.FormatText (this.DisplayName).ApplyBold ();
				return TextFormatter.FormatText (boldName, "(~", this.Age, "~)");
			}
			else
			{
				return this.GetCompactSummary ();
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "(~", this.Age, "~)");
		}

		public override FormattedText GetSummary()
		{
			return this.GetCoordinatesSummary ();
		}

		public FormattedText GetPersonalDataSummary()
		{
			return TextFormatter.FormatText (
				this.Title, this.MrMrs, TextFormatter.Command.IfEmpty, "\n",
				this.eCH_Person.PersonFirstNames, this.eCH_Person.PersonOfficialName, "(~", this.OriginalName, "~)", "\n",
				this.eCH_Person.PersonDateOfBirth, "~ – ~", this.eCH_Person.PersonDateOfDeath, "\n",
				this.Confession, "~\n",
				this.Profession, "~\n");
		}

		public string GetCallName()
		{
			if (string.IsNullOrWhiteSpace (this.CallName))
			{
				return eCH_PersonEntity.GetDefaultFirstName (this.eCH_Person);
			}

			return this.CallName;
		}

		public string GetShortCallName()
		{
			return NameProcessor.GetAbbreviatedFirstname (this.GetCallName ());
		}


		public string GetFullName()
		{
			return this.GetFullName (this.GetCallName ());
		}

		public string GetShortFullName()
		{
			return this.GetFullName (this.GetShortCallName ());
		}

		public string GetFullName(string firstname)
		{
			return StringUtils.Join (" ", firstname, this.eCH_Person.PersonOfficialName);
		}

		public string GetDisplayName()
		{
			var lastname = this.eCH_Person.PersonOfficialName;
			var firstname = this.GetCallName ();

			var name = TextFormatter.FormatText (lastname, ",", firstname).ToSimpleText ();

			if (this.eCH_Person.IsDeceased)
			{
				name += " †";
			}

			return name;
		}

		public string GetIconName(string prefix)
		{
			string suffix;

			if (this.eCH_Person.PersonSex == PersonSex.Female)
			{
				suffix = ".AiderPerson.Female-";
			}
			else
			{
				suffix = ".AiderPerson.Male-";
			}

			if (this.Language == Enumerations.Language.German)
			{
				return prefix + suffix + "German";
			}
			else
			{
				return prefix + suffix + "French";
			}
		}

		public static string GetIconName(string prefix, PersonMrMrs? personMrMrs, Language? language = null)
		{
			string suffix;

			switch (personMrMrs)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
					suffix = ".AiderPerson.Female-";
					break;
				default:
					suffix = ".AiderPerson.Male-";
					break;
			}

			if (language == Enumerations.Language.German)
			{
				return prefix + suffix + "German";
			}
			else
			{
				return prefix + suffix + "French";
			}
		}


		public FormattedText GetGroupTitle()
		{
			int nbGroups = this.Groups.Count;

			return TextFormatter.FormatText ("Groupes (", nbGroups, ")");
		}

		public FormattedText GetGroupText()
		{
			var groups = this.Groups
				.Select (g => g.GetSummaryWithHierarchicalGroupName ())
				.CreateSummarySequence (10, "...");

			var text = TextFormatter.Join ("\n", groups);

			return TextFormatter.FormatText (text);
		}

		public FormattedText GetWarningsTitle()
		{
			return TextFormatter.FormatText ("Averstissements (" + this.Warnings.Count + ")");
		}

		public FormattedText GetWarningsDescription()
		{
			var warnings = this.Warnings
				.Select (w => w.Title)
				.CreateSummarySequence (10, "...");

			var text = TextFormatter.Join ("\n", warnings);

			return TextFormatter.FormatText (text);
		}

		public bool IsMemberOf(AiderGroupEntity group)
		{
			return this.GetMemberships (group).Any ();
		}

		public IEnumerable<AiderGroupParticipantEntity> GetMemberships(AiderGroupEntity group)
		{
			return this.GetParticipations ().Where (g => g.Group == group);
		}

		public AiderContactEntity GetMainContact()
		{
			var mainAddress = this.GetAddress ();

			return this.Contacts.FirstOrDefault (c => c.GetAddress () == mainAddress);
		}


		internal void RefreshCache()
		{
			//	This is called by AiderPersonBusinessRules.ApplyUpdateRule in order to refresh
			//	the cached data whenever the person data gets edited.

			this.RefreshDisplayName ();
			this.RefreshBirthdayDate ();
		}

		internal void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Add (participation);
		}

		internal void RemoveParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Remove (participation);
		}

		internal void AddWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Add (warning);
		}

		internal void RemoveWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Remove (warning);
		}


		internal void ProcessPersonDeath()
		{
			//	TODO: process the death of the person (remove from all associated groups)
		}

		internal void ProcessPersonRevival()
		{
			//	TODO: process the death of the person (remove from all associated groups)
		}


		partial void OnParishGroupChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (newValue);
		}

		partial void OnVisibilityChanging(PersonVisibilityStatus oldValue, PersonVisibilityStatus newValue)
		{
			switch (oldValue)
			{
				case PersonVisibilityStatus.Deceased:
					this.ProcessPersonRevival ();
					break;
			}

			switch (newValue)
			{
				case PersonVisibilityStatus.Deceased:
					this.ProcessPersonDeath ();
					break;
			}
		}



		partial void GetAddress(ref AiderAddressEntity value)
		{
			value = this.GetAddress ();
		}

		partial void SetAddress(AiderAddressEntity value)
		{
			throw new NotImplementedException ();
		}

		partial void GetGroups(ref IList<AiderGroupParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		partial void GetHouseholds(ref IList<AiderHouseholdEntity> value)
		{
			value = this.GetHouseholds ().OrderBy (x => x.DisplayName).AsReadOnlyCollection ();
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().OrderBy (x => x.DisplayAddress).AsReadOnlyCollection ();
		}

		partial void GetAdditionalAddresses(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ()
				.Where (x => x.ContactType == ContactType.PersonAddress)
				.OrderBy (x => x.DisplayAddress)
				.AsReadOnlyCollection ();
		}

		partial void GetWarnings(ref IList<AiderPersonWarningEntity> value)
		{
			value = this.GetWarnings ().AsReadOnlyCollection ();
		}

		partial void GetCallNameDisplay(ref string value)
		{
			value = this.GetCallName ();
		}



		private AiderAddressEntity GetAddress()
		{
			//	A person's address is the one which was explicitely defined to be the default
			//	(AddressType = Default), or the first household address, or any fully defined
			//	available address for the person if everything else failed:

			var defaultAddress = 
				this.AdditionalAddresses.Where (x => x.AddressType == AddressType.Default).Select (x => x.Address).FirstOrDefault () ??
				this.Households.Select (x => x.Address).FirstOrDefault () ??
				this.AdditionalAddresses.Where (x => x.HasFullAddress ()).Select (x => x.Address).FirstOrDefault ();

			return defaultAddress;
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

		private ISet<AiderHouseholdEntity> GetHouseholds()
		{
			if (this.households == null)
			{
				this.households = this.Contacts
					.Where (x => x.Household.IsNotNull ())
					.Select (x => x.Household)
					.ToSet ();
			}

			return this.households;
		}

		private ISet<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = this.ExecuteWithDataContext
				(
					d => this.FindContacts (d).ToSet (),
					() => new HashSet<AiderContactEntity> ()
				);
			}

			return this.contacts;
		}

		private IList<AiderPersonWarningEntity> GetWarnings()
		{
			if (this.warnings == null)
			{
				this.warnings = this.ExecuteWithDataContext
				(
					d => this.FindWarnings (d),
					() => new List<AiderPersonWarningEntity> ()
				);
			}

			return this.warnings;
		}


		private IList<AiderGroupParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true);

			return dataContext
				.GetByRequest<AiderGroupParticipantEntity> (request)
				.OrderBy (g => g.GetSummaryWithHierarchicalGroupName ().ToString ())
				.ToList ();
		}

		private IList<AiderContactEntity> FindContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				Person = this
			};

			return dataContext.GetByExample (example);
		}

		private IList<AiderPersonWarningEntity> FindWarnings(DataContext dataContext)
		{
			var controller = AiderWarningController.Current;

			return controller
				.GetWarnings<AiderPersonWarningEntity> (this)
				.ToList ();
		}


		private void RefreshDisplayName()
		{
			this.DisplayName = this.GetDisplayName ();
		}

		private void RefreshBirthdayDate()
		{
			Date? date = null;

			if (this.eCH_Person.IsNotNull ())
			{
				date = this.eCH_Person.PersonDateOfBirth;
			}

			if (date.HasValue == false)
			{
				this.BirthdayDay   = 0;
				this.BirthdayMonth = 0;
				this.BirthdayYear  = 0;
			}
			else
			{
				this.BirthdayDay   = date.Value.Day;
				this.BirthdayMonth = date.Value.Month;
				this.BirthdayYear  = date.Value.Year;
			}
		}


		private FormattedText GetCoordinatesSummary()
		{
			var lines = this.GetCoordinatesSummaryLines ();

			return TextFormatter.FormatText (lines.Select (x => x.AppendLineIfNotNull ()));
		}

		private IEnumerable<FormattedText> GetCoordinatesSummaryLines()
		{
			// Gets the full name
			var fullNameText = this.GetFullName ();

			if (!fullNameText.IsNullOrWhiteSpace ())
			{
				yield return fullNameText;
			}

			//	Gets the default mail address
			//	TODO: ...
		}



		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
			this.ClearHouseholdCache ();
		}

		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
			this.ClearHouseholdCache ();
		}


		private void ClearHouseholdCache()
		{
			this.households = null;
		}




		#region IAiderWarningExampleFactoryGetter Members

		AiderWarningExampleFactory IAiderWarningExampleFactoryGetter.GetWarningExampleFactory()
		{
			return AiderPersonEntity.warningExampleFactory;
		}

		#endregion


		private static readonly AiderWarningExampleFactory warningExampleFactory = new AiderWarningExampleFactory<AiderPersonEntity, AiderPersonWarningEntity> ((example, source) => example.Person = source);

		private IList<AiderGroupParticipantEntity>	participations;
		private IList<AiderPersonWarningEntity>		warnings;
		private ISet<AiderHouseholdEntity>			households;
		private ISet<AiderContactEntity>			contacts;
	}
}

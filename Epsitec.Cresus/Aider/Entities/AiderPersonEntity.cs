﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

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
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "(~", this.ComputeAge (), "~)");
		}

		public FormattedText GetCompactSummary(AiderHouseholdEntity household)
		{
			if (household.IsNotNull ())
			{
				if (household.IsHead (this))
				{
					var boldName = TextFormatter.FormatText (this.DisplayName).ApplyBold ();
					return TextFormatter.FormatText (boldName, "(~", this.ComputeAge (), "~)");
				}
			}

			return this.GetCompactSummary ();
		}

		public override FormattedText GetSummary()
		{
			return this.GetCoordinatesSummary ();
		}


		public FormattedText GetCoordinatesSummary()
		{
			var lines = this.GetCoordinatesSummaryLines ();

			return TextFormatter.FormatText (lines.Select (x => x.AppendLineIfNotNull ()));
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

		public bool IsAlive
		{
			get
			{
				return this.eCH_Person.IsNull () || this.eCH_Person.IsDeceased == false;
			}
		}

		public bool IsDeceased
		{
			get
			{
				return !this.IsAlive;
			}
		}
		
		public int? ComputeAge()
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


		public IEnumerable<FormattedText> GetCoordinatesSummaryLines()
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


		public string GetFullName()
		{
			return StringUtils.Join
			(
			" ",
			this.eCH_Person.PersonFirstNames,
			this.eCH_Person.PersonOfficialName
			);
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
			var callname = this.GetCallName ();

			if (string.IsNullOrEmpty (callname))
			{
				return "";
			}

			var split = callname.Split (' ', '-');

			return string.Concat (split.Select (x => AiderPersonEntity.GetNameAbbreviation (x)).ToArray ());
		}

		public static string GetNameAbbreviation(string name)
		{
			name = name.ToLowerInvariant ();

			if ((name.StartsWith ("ch")) ||
				(name.StartsWith ("ph")))
			{
				var c1 = name.Substring (0, 1).ToUpperInvariant ();
				var c2 = name.Substring (1, 1);

				return string.Concat (c1, c2, ".");
			}
			else
			{
				var c1 = name.Substring (0, 1).ToUpperInvariant ();

				return string.Concat (c1, ".");
			}
		}

		public string GetShortFullName()
		{
			return StringUtils.Join (" ", this.GetShortCallName (), this.eCH_Person.PersonOfficialName);
		}


		public bool IsGovernmentDefined()
		{
			return this.eCH_Person.DataSource == Enumerations.DataSource.Government;
		}


		public void RefreshCache()
		{
			this.DisplayName = this.GetDisplayName ();

			this.RefreshBirthdayDate ();
		}


		private void RefreshBirthdayDate()
		{
			var date = this.eCH_Person.PersonDateOfBirth;

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


		public string GetParishGroupPathCache()
		{
			return this.Parish.Group.Path;
		}

		internal string GetIconName(string prefix)
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


		partial void GetGroups(ref IList<AiderGroupParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}


		private IList<AiderGroupParticipantEntity> GetParticipations()
		{
			if (this.participations == null)
			{
				this.participations = this.ExecuteWithDataContext
				(
					d => this.GetParticipations(d),
					() => new List<AiderGroupParticipantEntity> ()
				);
			}

			return this.participations;
		}


		private IList<AiderGroupParticipantEntity> GetParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, false, true, false);

			return dataContext
				.GetByRequest<AiderGroupParticipantEntity> (request)
				.OrderBy (g => g.GetSummaryWithHierarchicalGroupName ().ToString ())
				.ToList ();
		}


		public void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Add (participation);
		}


		public void RemoveParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Remove (participation);
		}

		public bool IsMemberOf(AiderGroupEntity group)
		{
			return this.GetParticipations ().Any (g => g.Group == group);
		}

		partial void GetWarnings(ref IList<AiderPersonWarningEntity> value)
		{
			value = this.GetWarnings ().AsReadOnlyCollection ();
		}

		private IList<AiderPersonWarningEntity> GetWarnings()
		{
			if (this.warnings == null)
			{
				this.warnings = this.ExecuteWithDataContext
				(
					d => this.GetWarnings (d),
					() => new List<AiderPersonWarningEntity> ()
				);
			}

			return this.warnings;
		}

		private IList<AiderPersonWarningEntity> GetWarnings(DataContext dataContext)
		{
			var controller = AiderWarningController.Current;

			return controller
				.GetWarnings<AiderPersonWarningEntity> (this)
				.ToList ();
		}

		public void AddWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Add (warning);
		}

		public void RemoveWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Remove (warning);
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
					d => this.GetContacts (d).ToSet (),
					() => new HashSet<AiderContactEntity> ()
				);
			}

			return this.contacts;
		}


		private IEnumerable<AiderContactEntity> GetContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				Person = this
			};

			return dataContext.GetByExample (example);
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

		private IList<AiderGroupParticipantEntity> participations;
		private IList<AiderPersonWarningEntity> warnings;
		private ISet<AiderHouseholdEntity> households;
		private ISet<AiderContactEntity> contacts;
	}
}

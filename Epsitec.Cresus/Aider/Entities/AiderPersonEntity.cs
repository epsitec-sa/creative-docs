//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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
				return this.eCH_Person.IsNull () || this.eCH_Person.PersonDateOfDeath == null;
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


		public FormattedText GetRelatedPersonsSummary(int nbToDisplay)
		{
			var lines = this.GetRelatedPersonsSummaryLines (nbToDisplay);

			return TextFormatter.FormatText (lines.Join ("\n"));
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


		public IEnumerable<string> GetRelatedPersonsSummaryLines(int nbToDisplay)
		{
			var relatedPersons = this.GetRelatedPersons ().ToList ();

			foreach (var relatedPerson in relatedPersons.Take (nbToDisplay))
			{
				yield return relatedPerson.GetFullName ();
			}

			if (relatedPersons.Count > nbToDisplay)
			{
				yield return "...";
			}
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

		public string GetShortCallName()
		{
			var callname = this.CallName;

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


		public IEnumerable<AiderPersonEntity> GetRelatedPersons()
		{
			// TODO Add stuff like godfather and godmother

			return this.Housemates.Concat (this.Children).Concat (this.Parents);
		}

		partial void GetHousemates(ref IList<AiderPersonEntity> value)
		{
			value = new List<AiderPersonEntity> ();

			//	TODO
		}

		partial void GetChildren(ref IList<AiderPersonEntity> value)
		{
			value = new List<AiderPersonEntity> ();

			// TODO
		}

		partial void GetParents(ref IList<AiderPersonEntity> value)
		{
			value = new List<AiderPersonEntity> ();

			// TODO
		}

		partial void GetGroups(ref IList<AiderGroupParticipantEntity> value)
		{
			if (this.groupList == null)
			{
				var dataContext = BusinessContextPool.GetCurrentContext (this).DataContext;

				var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true, true, false);

				this.groupList = dataContext
					.GetByRequest<AiderGroupParticipantEntity> (request)
					.AsReadOnlyCollection ();
			}

			value = this.groupList;
		}
		
		partial void GetRelationships(ref IList<AiderPersonRelationshipEntity> value)
		{
			value = new List<AiderPersonRelationshipEntity> ();

			//	TODO
		}

		partial void GetWarnings(ref IList<AiderPersonWarningEntity> value)
		{
			value = new List<AiderPersonWarningEntity> ();

			var warningController = AiderWarningController.Current;
			var warnings = warningController.GetWarnings<AiderPersonWarningEntity> (this);

			value.AddRange (warnings);
		}

		partial void GetParishGroup(ref AiderGroupEntity value)
		{
			value = this.Parish.Group; 
		}

		partial void SetParishGroup(AiderGroupEntity value)
		{
			if (this.Parish.Group == value)
			{
				return;
			}

			if (value == null)
			{
				BusinessContextPool.GetCurrentContext (this).DeleteEntity (this.Parish.Group);
			}
			else
			{
				this.Parish.Group = value;
			}
		}

		partial void GetHouseholds(ref IList<AiderHouseholdEntity> value)
		{
			value = this.GetHouseholds ().OrderBy (x => x.DisplayName).AsReadOnlyCollection ();
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().OrderBy (x => x.DisplayAddress).AsReadOnlyCollection ();
		}

		partial void GetAdditionalAddresses(ref IList<AiderAddressEntity> value)
		{
			value = this.GetContacts ()
				.Where (x => x.ContactType == ContactType.PersonAddress)
				.OrderBy (x => x.DisplayAddress)
				.Select (x => x.Address).AsReadOnlyCollection ();
		}

		private IEnumerable<AiderHouseholdEntity> GetHouseholds()
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

		private IEnumerable<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = new HashSet<AiderContactEntity> ();

				var businessContext = BusinessContextPool.GetCurrentContext (this);
				var dataContext     = businessContext.DataContext;

				if (dataContext.IsPersistent (this))
				{
					var example  = new AiderContactEntity ()
					{
						Person = this
					};
					var contacts = dataContext.GetByExample (example);

					this.contacts.UnionWith (contacts);
				}
			}

			return this.contacts;
		}

		
		internal static string GetDisplayName(AiderPersonEntity person)
		{
			var display = TextFormatter.FormatText (person.eCH_Person.PersonOfficialName, ",", person.CallName);

			if (person.eCH_Person.IsDeceased)
			{
				return display.ToSimpleText () + " †";
			}
			else
			{
				return display.ToSimpleText ();
			}
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
				.Select (g => g.GetSummaryWithGroupName ())
				.CreateSummarySequence (10, "...");

			var text = TextFormatter.Join ("\n", groups);

			return TextFormatter.FormatText (text);
		}


		#region IAiderWarningExampleFactoryGetter Members

		AiderWarningExampleFactory IAiderWarningExampleFactoryGetter.GetWarningExampleFactory()
		{
			return AiderPersonEntity.warningExampleFactory;
		}

		#endregion


		private static readonly AiderWarningExampleFactory warningExampleFactory = new AiderWarningExampleFactory<AiderPersonEntity, AiderPersonWarningEntity> ((example, source) => example.Person = source);

		private IList<AiderGroupParticipantEntity> groupList;
		private HashSet<AiderHouseholdEntity> households;
		private HashSet<AiderContactEntity> contacts;
	}
}

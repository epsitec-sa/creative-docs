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

		
		public int? ComputeAge()
		{
			if (this.eCH_Person.IsNull ())
			{
				return null;
			}

			var birthdate = this.eCH_Person.PersonDateOfBirth;

			if (birthdate == null)
			{
				return null;
			}
			else
			{
				return birthdate.Value.ComputeAge ();
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

			// Gets the default mail address
			var defaultMailaddress = this.GetMailAddresses ().FirstOrDefault ();

			if (defaultMailaddress.IsNotNull ())
			{
				var addressLines = defaultMailaddress
				.GetAddressLines ()
				.Where (l => !l.IsNullOrWhiteSpace ());

				foreach (var mailAddressLine in addressLines)
				{
					yield return mailAddressLine;
				}
			}

			// Gets the default phone
			var defaultPhone = this.GetPhones ().FirstOrDefault ();

			if (defaultPhone != null)
			{
				yield return "Tel: " + defaultPhone;
			}

			// Gets the default email
			var defaultEmailAddress = this.GetEmailAddresses ().FirstOrDefault ();

			if (defaultEmailAddress != null)
			{
				yield return "Email: " + defaultEmailAddress;
			}
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


		public IEnumerable<AiderAddressEntity> GetMailAddresses()
		{
			return this.GetAddresses ().Where (a => a.Town.IsNotNull ());
		}


		public IEnumerable<FormattedText> GetPhones()
		{
			return this.GetAddresses ().SelectMany (a => a.GetPhones ());
		}


		public IEnumerable<string> GetEmailAddresses()
		{
			return this.GetAddresses ().Select (a => a.Email).Where (e => !e.IsNullOrWhiteSpace ());
		}


		public IEnumerable<AiderAddressEntity> GetAddresses()
		{
			if (this.Household1.IsNotNull () && this.Household1.Address.IsNotNull ())
			{
				yield return this.Household1.Address;
			}

			if (this.Household2.IsNotNull () && this.Household2.Address.IsNotNull ())
			{
				yield return this.Household2.Address;
			}

			if (this.AdditionalAddress1.IsNotNull ())
			{
				yield return this.AdditionalAddress1;
			}

			if (this.AdditionalAddress2.IsNotNull ())
			{
				yield return this.AdditionalAddress2;
			}

			if (this.AdditionalAddress3.IsNotNull ())
			{
				yield return this.AdditionalAddress3;
			}

			if (this.AdditionalAddress4.IsNotNull ())
			{
				yield return this.AdditionalAddress4;
			}
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

		partial void GetIsHouseholdHead(ref bool value)
		{
			foreach (var household in this.GetHouseholds ())
			{
				if ((household.Head1 == this) ||
					(household.Head2 == this))
				{
					value = true;
					return;
				}
			}

			value = false;
		}

		partial void OnIsHouseholdHeadChanged(bool oldValue, bool newValue)
		{
			if (newValue == false)
			{
				//	This person is no longer the head of the household.

				foreach (var household in this.GetHouseholds())
				{
					if (household.Head1 == this)
					{
						household.Head1 = household.Head2;
						household.Head2 = household.GetEntityContext ().CreateEmptyEntity<AiderPersonEntity> ();
					}
					else if (household.Head2 == this)
					{
						household.Head2 = household.GetEntityContext ().CreateEmptyEntity<AiderPersonEntity> ();
					}
				}
			}
			else
			{
				//	Make this person the head of the household. If there is already one or
				//	two heads defined, replace the oldest one. This only considers the first
				//	household if there are two of them.

				if ((this.Household1.IsNull ()) &&
					(this.Household2.IsNull ()))
				{
					//	No op
				}
				else if (this.Household1.IsNotNull ())
				{
					this.Household1.Head2 = this.Household1.Head1;
					this.Household1.Head1 = this;
				}
				else
				{
					this.Household2.Head2 = this.Household2.Head1;
					this.Household2.Head1 = this;
				}
			}
		}

		partial void GetHousemates(ref IList<AiderPersonEntity> value)
		{
			IEnumerable<AiderPersonEntity> housemates = Enumerable.Empty<AiderPersonEntity> ();

			if (this.Household1.IsNotNull ())
			{
				housemates = housemates.Concat (this.Household1.Members);
			}

			if (this.Household2.IsNotNull ())
			{
				housemates = housemates.Concat (this.Household2.Members);
			}

			value = housemates.Where (p => p != this).ToList ();
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
		
		partial void GetAdditionalAddresses(ref IList<AiderAddressEntity> value)
		{
			if (this.additionalAddresses == null)
			{
				this.additionalAddresses = new Helpers.AiderPersonAdditionalContactAddressList (this);
			}

			value = this.additionalAddresses;
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

		private IEnumerable<AiderHouseholdEntity> GetHouseholds()
		{
			if (this.households == null)
			{
				this.households = new HashSet<AiderHouseholdEntity> ();
				
				var businessContext = BusinessContextPool.GetCurrentContext (this);
				var dataContext     = businessContext.DataContext;

				if (dataContext.IsPersistent (this))
				{
					var example  = new AiderContactEntity () { Person = this };
					var contacts = dataContext.GetByExample (example);

					this.households.UnionWith (contacts.Where (x => x.Household.IsNotNull ()).Select (x => x.Household));
				}
			}

			return this.households;
		}

		
		internal static string GetDisplayName(AiderPersonEntity person)
		{
			var display = TextFormatter.FormatText (person.eCH_Person.PersonOfficialName, ",", person.CallName);

			return display.ToSimpleText ();
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


		public void SetHousehold(BusinessContext businessContext, AiderHouseholdEntity newHousehold, bool isMainHousehold)
		{
			if (isMainHousehold)
			{
				this.SetHousehold1 (businessContext, newHousehold);
			}
			else
			{
				this.SetHousehold2 (businessContext, newHousehold);
			}
		}

		public void SetHousehold1(BusinessContext businessContext, AiderHouseholdEntity newHousehold)
		{
			this.SetHousehold (businessContext, newHousehold, p => p.Household1, (p, h) => p.Household1 = h);
		}

		public void SetHousehold2(BusinessContext businessContext, AiderHouseholdEntity newHousehold)
		{
			this.SetHousehold (businessContext, newHousehold, p => p.Household2, (p, h) => p.Household2 = h);
		}

		private void SetHousehold(BusinessContext businessContext, AiderHouseholdEntity newHousehold, Func<AiderPersonEntity, AiderHouseholdEntity> getter, Action<AiderPersonEntity, AiderHouseholdEntity> setter)
		{
			var oldHousehold = getter (this);

			if (oldHousehold == newHousehold)
			{
				return;
			}

			setter (this, newHousehold);

			if (newHousehold.IsNotNull ())
			{
				newHousehold.Add (this);
			}

			if (oldHousehold.IsNotNull ())
			{
				oldHousehold.Remove (this);

				if (oldHousehold.Members.Count == 0)
				{
					businessContext.DeleteEntity (oldHousehold);
				}
			}
		}


		public static AiderPersonEntity Create(BusinessContext businessContext)
		{
			return businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
		}


		#region IAiderWarningExampleFactoryGetter Members

		AiderWarningExampleFactory IAiderWarningExampleFactoryGetter.GetWarningExampleFactory()
		{
			return AiderPersonEntity.warningExampleFactory;
		}

		#endregion


		private static readonly AiderWarningExampleFactory warningExampleFactory = new AiderWarningExampleFactory<AiderPersonEntity, AiderPersonWarningEntity> ((example, source) => example.Person = source);

		private AiderPersonAdditionalContactAddressList additionalAddresses;
		private IList<AiderGroupParticipantEntity> groupList;
		private HashSet<AiderHouseholdEntity> households;
	}
}

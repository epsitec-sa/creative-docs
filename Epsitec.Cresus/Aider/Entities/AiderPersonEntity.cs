//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;

using System;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Aider.Controllers;


namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonEntity : IAiderWarningExampleFactoryGetter
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
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
				.Where (l => !l.IsNullOrWhiteSpace);

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


		partial void GetHousemates(ref IList<AiderPersonEntity> value)
		{
			IEnumerable<AiderPersonEntity> housemates = EmptyEnumerable<AiderPersonEntity>.Instance;

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

		partial void GetHouseholds(ref IList<AiderHouseholdEntity> value)
		{
			if (this.householdList == null)
			{
				this.householdList = new Helpers.AiderPersonHouseholdList (this);
			}

			value = this.householdList;
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

		
		internal static FormattedText GetDisplayName(AiderPersonEntity person)
		{
			return TextFormatter.FormatText (person.eCH_Person.PersonOfficialName, ",", person.CallName);
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
		
		internal FormattedText GetParishDescription()
		{
			if (this.Parish.IsNull ())
			{
				return FormattedText.Empty;
			}

			var parish = this.Parish;

			return TextFormatter.FormatText ("Dans la paroisse depuis", parish.StartDate, "\n\n~", parish.Comment.Text);

			/*
			var context    = BusinessContextPool.GetCurrentContext (this);
			var repository = context.GetRepository<AiderGroupParticipantEntity> ();
			var example    = repository.CreateExample ();

			example.Group = this.Parish;
			
			var participant = repository.GetByExample (example).FirstOrDefault ();

			if (participant.IsNull ())
			{
				return FormattedText.Empty;
			}
			var distantPast = new Date (1900, 1, 1);

			var groups = repository.GetAllEntities ().OrderByDescending (x => x.EndDate ?? x.StartDate ?? distantPast).ToArray ();
			*/

		}


		#region IAiderWarningExampleFactoryGetter Members

		AiderWarningExampleFactory IAiderWarningExampleFactoryGetter.GetWarningExampleFactory()
		{
			return AiderPersonEntity.warningExampleFactory;
		}

		#endregion


		private static readonly AiderWarningExampleFactory warningExampleFactory = new AiderWarningExampleFactory<AiderPersonEntity, AiderPersonWarningEntity> ((example, source) => example.Person = source);

		private Helpers.AiderPersonAdditionalContactAddressList additionalAddresses;
		private Helpers.AiderPersonHouseholdList householdList;
	}
}

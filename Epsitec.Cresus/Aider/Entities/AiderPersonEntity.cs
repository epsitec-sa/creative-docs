using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderPersonEntity
	{


		public override FormattedText GetCompactSummary()
		{
			var fullName = this.GetFullName ();

			return TextFormatter.FormatText (fullName);
		}


		public override FormattedText GetSummary()
		{
			return this.GetCoordinatesSummary ();
		}


		public FormattedText GetCoordinatesSummary()
		{
			var lines = this.GetCoordinatesSummaryLines ();

			return TextFormatter.FormatText (lines.Join ("\n"));
		}


		public FormattedText GetPersonalDataSummary()
		{
			var lines = this.GetPersonalDataSummaryLines ();

			return TextFormatter.FormatText (lines.Join ("\n"));
		}


		public FormattedText GetRelatedPersonsSummary(int nbToDisplay)
		{
			var lines = this.GetRelatedPersonsSummaryLines (nbToDisplay);

			return TextFormatter.FormatText (lines.Join ("\n"));
		}


		public IEnumerable<string> GetCoordinatesSummaryLines()
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


		public IEnumerable<string> GetPersonalDataSummaryLines()
		{
			// Gets the text with the title and honorific
			var honorific = this.MrMrs.AsText ();
			var title = this.Title;

			var honorificTitleText = "";

			if (!honorific.IsNullOrWhiteSpace ())
			{
				honorificTitleText = honorific;
			}

			if (!title.IsNullOrWhiteSpace ())
			{
				if (honorificTitleText.IsNullOrWhiteSpace ())
				{
					honorificTitleText = title;
				}
				else
				{
					honorificTitleText += " (" + title + ")";
				}
			}

			if (!honorificTitleText.IsNullOrWhiteSpace ())
			{
				yield return honorificTitleText;
			}

			// Gets the text for the name sex and language
			var fullNameText = this.GetFullName ();

			if (!fullNameText.IsNullOrWhiteSpace ())
			{
				var sexText = this.eCH_Person.PersonSex.AsShortText();

				if (!sexText.IsNullOrWhiteSpace ())
				{
					fullNameText += " (" + sexText + ")";
				}

				var languageText = this.Language.AsShortText ();

				if (!languageText.IsNullOrWhiteSpace ())
				{
					fullNameText += " (" + languageText + ")";
				}

				yield return fullNameText;
			}

			// Gets the text for the dates of birth and death.
			var datesText = this.eCH_Person.DateOfBirth;

			var dateOfDeath = this.DateOfDeath;

			if (!datesText.IsNullOrWhiteSpace () && dateOfDeath.HasValue)
			{
				datesText += " - " + dateOfDeath.Value.ToDateTime ().ToShortDateString ();
			}

			if (!datesText.IsNullOrWhiteSpace ())
			{
				yield return datesText;
			}

			// Gets the text for the confession
			var confessionText = this.Confession.AsText ();

			if (!confessionText.IsNullOrWhiteSpace ())
			{
				yield return confessionText;
			}

			// Gets the text for the profession
			var professionText = this.Profession;

			if (!professionText.IsNullOrWhiteSpace ())
			{
				yield return professionText;
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


		public IEnumerable<string> GetPhones()
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


		partial void GetAdditionalAddresses(ref IList<AiderAddressEntity> value)
		{
			if (this.additionalAddresses == null)
			{
				this.additionalAddresses = new Helpers.AiderPersonAdditionalContactAddressList (this);
			}

			value = this.additionalAddresses;
		}


		private Helpers.AiderPersonAdditionalContactAddressList additionalAddresses;
	}


}

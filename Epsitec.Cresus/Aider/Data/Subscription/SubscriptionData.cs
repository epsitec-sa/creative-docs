//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.Data.Subscription
{
	internal sealed class SubscriptionData
	{
		public SubscriptionData
		(
			string corporateName,
			string title,
			string firstname,
			string lastname,
			string firstAddressLine,
			string postBox,
			string streetName,
			int? houseNumber,
			string houseNumberComplement,
			string zipCode,
			string town,
			string countryCode,
			string comment,
			int? regionalEdition,
			int? nbCopies,
			bool? isLegalPerson
		)
		{
			if ((string.IsNullOrWhiteSpace (postBox) == false) &&
				(postBox.IsInteger ()))
			{
				postBox = "Case postale " + postBox;
			}

			this.CorporateName         = corporateName;
			this.Title                 = title;
			this.Firstname             = firstname;
			this.Lastname              = lastname;
			this.FirstAddressLine      = firstAddressLine;
			this.PostBox               = postBox;
			this.StreetName            = streetName;
			this.HouseNumber           = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode               = zipCode;
			this.Town                  = town;
			this.CountryCode           = countryCode;
			this.Comment               = comment;
			this.RegionalEdition       = regionalEdition;
			this.NbCopies              = nbCopies;
			this.isLegalPerson         = isLegalPerson;
		}

		public SubscriptionData
		(
			string title,
			string firstname,
			string lastname,
			string streetName,
			int? houseNumber,
			string houseNumberComplement,
			string zipCode,
			string town,
			string countryCode,
			PersonSex sex,
			PersonMaritalStatus maritalStatus,
			PersonConfession confession,
			Epsitec.Common.Types.Date? birthDate,
			string phone,
			string mobile,
			string email,
			string householdToken,
			string comment,
			string profession,
			string nationality,
			string origin
		)
		{
			this.isRichData = true;

			this.Title                 = title;
			this.Firstname             = firstname;
			this.Lastname              = lastname;
			this.StreetName            = streetName;
			this.HouseNumber           = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode               = zipCode;
			this.Town                  = town;
			this.CountryCode           = countryCode;
			this.Sex                   = sex;
			this.MaritalStatus         = maritalStatus;
			this.Confession            = confession;
			this.BirthDate             = birthDate;
			this.Phone                 = phone;
			this.Mobile                = mobile;
			this.Email                 = email;
			this.HouseholdToken        = householdToken;
			this.Comment               = comment;
			this.Profession            = profession;
			this.Nationality = nationality;
			this.Origin = origin;
		}


		/// <summary>
		/// This indicates whether the subscription represents a legal person or a physical person.
		/// </summary>
		public bool IsLegalPerson
		{
			get
			{
				return this.isLegalPerson ?? !string.IsNullOrEmpty (this.CorporateName);
			}
		}

		public bool IsRichData
		{
			get
			{
				return this.isRichData;
			}
		}

		/// <summary>
		/// Sometimes, for the physical persons, we have two persons in one subscription. Their
		/// first name and title are separated by the string " et ". They always share the same
		/// last name.
		/// </summary>
		public int GetNbPersons()
		{
			return this.Title.Contains (" et ") && this.Firstname.Contains (" et ")
				? 2
				: 1;
		}


		public string GetPersonFirstname(int index)
		{
			return SubscriptionData.GetValue (this.Firstname, index);
		}


		public string GetPersonTitle(int index)
		{
			return SubscriptionData.GetValue (this.Title, index);
		}


		private static string GetValue(string value, int index)
		{
			var splitIndex = value.IndexOf (" et ");

			if (splitIndex < 0)
			{
				if (index != 0)
				{
					throw new System.ArgumentException ();
				}

				return value;
			}
			else
			{
				if (index == 0)
				{
					return value.Substring (0, splitIndex);
				}
				else if (index == 1)
				{
					return value.Substring (splitIndex + 4);
				}
				else
				{
					throw new System.ArgumentException ();
				}
			}
		}


		public readonly string CorporateName;
		public readonly string Title;
		public readonly string Firstname;
		public readonly string Lastname;
		public readonly string FirstAddressLine;
		public readonly string PostBox;
		public readonly string StreetName;
		public readonly int? HouseNumber;
		public readonly string HouseNumberComplement;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string CountryCode;
		public readonly string Comment;
		public readonly int? RegionalEdition;
		public readonly int? NbCopies;
		private readonly bool? isLegalPerson;
		private readonly bool isRichData;
		public readonly Epsitec.Common.Types.Date? BirthDate;
		public readonly PersonSex Sex;
		public readonly PersonMaritalStatus MaritalStatus;
		public readonly PersonConfession Confession;
		public readonly string Phone;
		public readonly string Mobile;
		public readonly string Email;
		public readonly string HouseholdToken;
		public readonly string Profession;
		public readonly string Nationality;
		public readonly string Origin;
	}
}

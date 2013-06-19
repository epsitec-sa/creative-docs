using Epsitec.Aider.Enumerations;

using System;


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
			bool? isLegalPerson
		)
		{
			this.CorporateName = corporateName;
			this.Title = title;
			this.Firstname = firstname;
			this.Lastname = lastname;
			this.FirstAddressLine = firstAddressLine;
			this.PostBox = postBox;
			this.StreetName = streetName;
			this.HouseNumber = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode = zipCode;
			this.Town = town;
			this.CountryCode = countryCode;
			this.Comment = comment;
			this.isLegalPerson = isLegalPerson;
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


		/// <summary>
		/// Sometimes, for the physical persons, we have two persons in one subcription. Their
		/// firstname and title are separated by the string " et ". They always share the same
		/// lastname.
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
					throw new ArgumentException ();
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
					throw new ArgumentException ();
				}
			}
		}


		public static PersonSex GuessSex(string title)
		{
			switch (TextParser.ParsePersonMrMrs (title))
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
					return PersonSex.Female;

				case PersonMrMrs.Monsieur:
					return PersonSex.Male;

				default:
					return PersonSex.Unknown;
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
		private readonly bool? isLegalPerson;


	}


}

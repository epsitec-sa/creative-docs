using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal sealed class SubscriptionDataLoader
	{


		public static IEnumerable<Subscription> LoadSubscriptions
		(
			FileInfo fileWeb,
			FileInfo fileDoctor,
			FileInfo filePro
		)
		{
			var addressChecker = new AddressChecker ();

			var subscriptionsWeb = SubscriptionDataReader
				.ReadWebSubscriptions (fileWeb)
				.Select (r => SubscriptionDataLoader.LoadWebSubscription (r, addressChecker));

			var subscriptionsDoctor	= SubscriptionDataReader
				.ReadDoctorSubscriptions (fileDoctor)
				.Select (r => SubscriptionDataLoader.LoadDoctorSubscription (r, addressChecker));

			var subscriptionsPro = SubscriptionDataReader
				.ReadProSubscriptions (filePro)
				.Select (r => SubscriptionDataLoader.LoadProSubscription (r, addressChecker));

			var subscriptions = subscriptionsWeb
				.Concat (subscriptionsDoctor)
				.Concat (subscriptionsPro)
				.Distinct (SubscriptionDataLoader.GetSubscriptionComparer ())
				.ToList ();

			addressChecker.DisplayWarnings ();

			return subscriptions;
		}


		private static Subscription LoadWebSubscription
		(
			Dictionary<WebSubscriptionHeader, string> record,
			AddressChecker addressChecker
		)
		{
			var rawCorporateName = record[WebSubscriptionHeader.CorporateName] ?? "";
			var rawTitle = record[WebSubscriptionHeader.Title] ?? "";
			var rawFirstname = record[WebSubscriptionHeader.Firstname] ?? "";
			var rawLastname = record[WebSubscriptionHeader.Lastname] ?? "";

			var corporateName = NamePatchEngine.SanitizeCapitalization (rawCorporateName);
			var title = rawTitle;
			var firstname = NamePatchEngine.SanitizeCapitalization (rawFirstname);
			var lastname = NamePatchEngine.SanitizeCapitalization (rawLastname);

			var zipCode = record[WebSubscriptionHeader.ZipCode] ?? "";
			var town = record[WebSubscriptionHeader.Town] ?? "";
			var countryCode = record[WebSubscriptionHeader.CountryCode];

			// We assume that the country is Switzerland if it is not set explicitely in the
			// record.
			if (string.IsNullOrWhiteSpace (countryCode))
			{
				countryCode = "CH";
			}

			var isIsSwitzerland = countryCode == "CH";

			// We check the addresses in Switzerland.
			if (isIsSwitzerland)
			{
				addressChecker.FixZipCodeAndTown (ref zipCode, ref town);
			}

			var rawAddress = record[WebSubscriptionHeader.Address] ?? "";

			string firstAddressLine;
			string streetName;
			int? houseNumber;
			string houseNumberComplement;

			SubscriptionDataLoader.ParseWebAddress
			(
				rawAddress, out firstAddressLine, out streetName, out houseNumber,
				out houseNumberComplement
			);

			// We check the addresses in Switzerland.
			if (isIsSwitzerland)
			{
				addressChecker.FixStreetName
				(
					ref firstAddressLine, ref streetName, houseNumber, ref zipCode, ref town
				);
			}

			var postBox = record[WebSubscriptionHeader.PostBox] ?? "";
			var comment = record[WebSubscriptionHeader.Comment] ?? "";

			return new Subscription
			(
				corporateName, title, firstname, lastname, firstAddressLine, postBox, streetName,
				houseNumber, houseNumberComplement, zipCode, town, countryCode, comment, null
			);
		}


		private static void ParseWebAddress
		(
			string address,
			out string firstAddressLine,
			out string streetName,
			out int? houseNumber,
			out string houseNumberComplement
		)
		{
			firstAddressLine = "";
			streetName = "";
			houseNumber = null;
			houseNumberComplement = "";

			address = address.Trim ();

			if (string.IsNullOrEmpty (address))
			{
				return;
			}

			// The address and the complement are separated by a " - " if there is a complement.

			var separatorIndex = address.IndexOf (" - ");
			if (separatorIndex >= 0)
			{
				firstAddressLine = address.Substring (0, separatorIndex).Trim ();
				address = address.Substring (separatorIndex + 3).Trim ();
			}

			// If there is a /, we strip what comes after, as it is probably the appartement
			// number.

			var slashIndex = address.IndexOf ('/');
			if (slashIndex >= 0)
			{
				address = address.Substring (0, slashIndex).Trim ();
			}

			SubscriptionDataLoader.ParseAddress
			(
				address, out streetName, out houseNumber, out houseNumberComplement
			);
		}


		private static Subscription LoadDoctorSubscription
		(
			Dictionary<DoctorSubscriptionHeader, string> record,
			AddressChecker addressChecker
		)
		{
			var rawCorporateName = record[DoctorSubscriptionHeader.CorporateName] ?? "";
			var rawTitle = record[DoctorSubscriptionHeader.Title] ?? "";
			var rawFirstname = record[DoctorSubscriptionHeader.Firstname] ?? "";
			var rawLastname = record[DoctorSubscriptionHeader.Lastname] ?? "";

			var corporateName = NamePatchEngine.SanitizeCapitalization (rawCorporateName);
			var title = rawTitle;
			var firstname = NamePatchEngine.SanitizeCapitalization (rawFirstname);
			var lastname = NamePatchEngine.SanitizeCapitalization (rawLastname);

			var zipCode = record[DoctorSubscriptionHeader.ZipCode] ?? "";
			var town = record[DoctorSubscriptionHeader.Town] ?? "";

			addressChecker.FixZipCodeAndTown (ref zipCode, ref town);

			var rawAddress1 = record[DoctorSubscriptionHeader.Address1] ?? "";
			var rawAddress2 = record[DoctorSubscriptionHeader.Address2] ?? "";

			string firstAddressLine;
			string streetName;
			int? houseNumber;
			string houseNumberComplement;

			SubscriptionDataLoader.ParseDoctorAddress
			(
				rawAddress1, rawAddress2, out firstAddressLine, out streetName, out houseNumber,
				out houseNumberComplement
			);

			addressChecker.FixStreetName
			(
				ref firstAddressLine, ref streetName, houseNumber, ref zipCode, ref town
			);

			var countryCode = "CH";
			var postBox = "";
			var comment = "";

			return new Subscription
			(
				corporateName, title, firstname, lastname, firstAddressLine, postBox, streetName,
				houseNumber, houseNumberComplement, zipCode, town, countryCode, comment, true
			);
		}


		private static void ParseDoctorAddress
		(
			string address1,
			string address2,
			out string firstAddressLine,
			out string streetName,
			out int? houseNumber,
			out string houseNumberComplement
		)
		{
			firstAddressLine = "";
			streetName = "";
			houseNumber = null;
			houseNumberComplement = "";

			firstAddressLine = address2.Trim ();

			if (string.IsNullOrEmpty (address1))
			{
				return;
			}

			address1 = address1.Trim ();

			// The address and the complement are separated by a "/" if there is a complement.

			var separatorIndex = address1.IndexOf ("/");
			if (separatorIndex >= 0)
			{
				firstAddressLine = address1.Substring (0, separatorIndex).Trim ();
				address1 = address1.Substring (separatorIndex + 1).Trim ();
			}

			SubscriptionDataLoader.ParseAddress
			(
				address1, out streetName, out houseNumber, out houseNumberComplement
			);
		}


		private static Subscription LoadProSubscription
		(
			Dictionary<ProSubscriptionHeader, string> record,
			AddressChecker addressChecker
		)
		{
			var rawPersonInName1 = record[ProSubscriptionHeader.PersonInName1] ?? "";
			var rawName1 = record[ProSubscriptionHeader.Name1] ?? "";
			var rawName2 = record[ProSubscriptionHeader.Name2] ?? "";
			var rawName3 = record[ProSubscriptionHeader.Name3] ?? "";

			string corporateName;
			string personName;

			SubscriptionDataLoader.ParseProNames
			(
				rawPersonInName1, rawName1, rawName2, rawName3, out corporateName, out personName
			);

			var zipCode = record[ProSubscriptionHeader.ZipCode] ?? "";
			var town = record[ProSubscriptionHeader.Town] ?? "";

			addressChecker.FixZipCodeAndTown (ref zipCode, ref town);

			var rawAddress = record[ProSubscriptionHeader.Address] ?? "";

			string firstAddressLine = "";
			string streetName;
			int? houseNumber;
			string houseNumberComplement;

			SubscriptionDataLoader.ParseAddress
			(
				rawAddress, out streetName, out houseNumber, out houseNumberComplement
			);

			addressChecker.FixStreetName
			(
				ref firstAddressLine, ref streetName, houseNumber, ref zipCode, ref town
			);

			var postBox = record[ProSubscriptionHeader.PostBox] ?? "";

			var title = "";
			var firstname = "";
			var countryCode = "CH";
			var comment = "";

			return new Subscription
			(
				corporateName, title, firstname, personName, firstAddressLine, postBox, streetName,
				houseNumber, houseNumberComplement, zipCode, town, countryCode, comment, true
			);
		}


		private static void ParseProNames
		(
			string personInName1,
			string name1,
			string name2,
			string name3,
			out string corporateName,
			out string personName
		)
		{
			if (personInName1 == "x")
			{
				personName = name1;
				corporateName = StringUtils.Join (" ", name2, name3);
			}
			else
			{
				personName = "";
				corporateName = StringUtils.Join (" ", name1, name2, name3);
			}
		}


		private static void ParseAddress
		(
			string address,
			out string streetName,
			out int? houseNumber,
			out string houseNumberComplement
		)
		{
			streetName = "";
			houseNumber = null;
			houseNumberComplement = "";

			if (string.IsNullOrEmpty (address))
			{
				return;
			}

			// We look for the last digit part, which should be the house number. We take the last
			// so we don't mess up streets like "rue du 1er mai".

			var digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			int digitIndex = address.LastIndexOfAny (digits);
			while (digitIndex > 0 && char.IsDigit (address[digitIndex - 1]))
			{
				digitIndex--;
			}

			if (digitIndex < 0)
			{
				streetName = address;
			}
			else
			{
				streetName = address.Substring (0, digitIndex).Trim ();
				address = address.Substring (digitIndex);

				var houseNumberChars = address
					.TakeWhile (c => char.IsDigit (c))
					.ToArray ();

				var houseNumberComplementChars = address
					.SkipWhile (c => char.IsDigit (c))
					.ToArray ();

				houseNumber = int.Parse (new string (houseNumberChars));
				houseNumberComplement = new string (houseNumberComplementChars).Trim ();
			}
		}


		private static LambdaComparer<Subscription> GetSubscriptionComparer()
		{
			return new LambdaComparer<Subscription>
			(
				(s1, s2) =>
				{
					return s1.CorporateName == s2.CorporateName
						&& s1.Title == s2.Title
						&& s1.Firstname == s2.Firstname
						&& s1.Lastname == s2.Lastname
						&& s1.FirstAddressLine == s2.FirstAddressLine
						&& s1.PostBox == s2.PostBox
						&& s1.StreetName == s2.StreetName
						&& s1.HouseNumber == s2.HouseNumber
						&& s1.HouseNumberComplement ==s2.HouseNumberComplement
						&& s1.ZipCode == s2.ZipCode
						&& s1.Town == s2.Town
						&& s1.CountryCode == s2.CountryCode
						&& s1.Comment == s2.Comment
						&& s1.IsCompany == s2.IsCompany;
				},
				(s) =>
				{
					return (s.Lastname + s.Firstname + s.Town).GetHashCode ();
				}
			);
		}


	}


}

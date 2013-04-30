using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Data.Platform.MatchSort;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;

namespace Epsitec.Aider.Data.Subscription
{


	internal static class SubscriptionFileWriter
	{


		public static void Write(CoreData coreData, FileInfo file)
		{
			var lines = SubscriptionFileWriter.GetLines (coreData);

			SubscriptionFileLine.Write (lines, file);
		}


		private static IEnumerable<SubscriptionFileLine> GetLines(CoreData coreData)
		{
			var lines = new List<SubscriptionFileLine> ();

			var encodingHelper = new EncodingHelper (SubscriptionFileLine.GetEncoding ());

			using (var etl = new MatchSortEtl ("MAT[CH]sort.csv"))
			{
				AiderEnumerator.Execute (coreData, (b, subscriptions) =>
				{
					var l = SubscriptionFileWriter.GetLines (subscriptions, etl, encodingHelper);

					lines.AddRange (l);
				});
			}

			return lines;
		}


		private static IEnumerable<SubscriptionFileLine> GetLines
		(
			IEnumerable<AiderSubscriptionEntity> subscriptions,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			foreach (var subscription in subscriptions)
			{
				if (subscription.Count == 0)
				{
					continue;
				}

				yield return SubscriptionFileWriter.GetLine
				(
					subscription, etl, encodingHelper
				);
			}
		}


		private static SubscriptionFileLine GetLine
		(
			AiderSubscriptionEntity subscription,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			switch (subscription.SubscriptionType)
			{
				case SubscriptionType.Household:
					return SubscriptionFileWriter.GetHouseholdLine
					(
						subscription, etl, encodingHelper
					);

				case SubscriptionType.LegalPerson:
					return SubscriptionFileWriter.GetLegalPersonLine
					(
						subscription, etl, encodingHelper
					);

				default:
					throw new NotImplementedException ();
			}
		}


		private static SubscriptionFileLine GetHouseholdLine
		(
			AiderSubscriptionEntity subscription,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			var household = subscription.Household;
			var address = subscription.GetAddress ();
			var town = address.Town;
			var country = town.Country;

			var subscriptionNumber = subscription.Id;
			var copiesCount = subscription.Count;
			var editionId = subscription.GetEditionId ();
			var postmanNumber = SubscriptionFileWriter.GetPostmanNumber (address, etl);

			var title = SubscriptionFileWriter.Process
			(
				household.GetHonorific (false),
				SubscriptionFileLine.TitleLength,
				encodingHelper
			);

			string lastname;
			string firstname;

			SubscriptionFileWriter.GetHouseholdName
			(
				household,
				encodingHelper,
				out firstname,
				out lastname
			);

			string addressComplement;
			string street;
			string houseNumber;

			SubscriptionFileWriter.GetHouseholdFirstAddressPart
			(
				address,
				encodingHelper,
				out addressComplement,
				out street,
				out houseNumber
			);

			var zipCode = SubscriptionFileWriter.Process
			(
				SubscriptionFileWriter.GetZipCode (town),
				SubscriptionFileLine.ZipCodeLength,
				encodingHelper
			);

			var townName = SubscriptionFileWriter.Process
			(
				town.Name,
				SubscriptionFileLine.TownLength,
				encodingHelper
			);

			var countryName = SubscriptionFileWriter.Process
			(
				country.Name,
				SubscriptionFileLine.CountryLength,
				encodingHelper
			);

			var isSwitzerland = country.IsSwitzerland ();

			return new SubscriptionFileLine
			(
				subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
				addressComplement, street, houseNumber, postmanNumber, zipCode, townName,
				countryName, DistributionMode.Surface, isSwitzerland
			);
		}


		private static void GetHouseholdName
		(
			AiderHouseholdEntity household,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			var firstnames = household.GetFirstnames ();
			var lastnames = household.GetLastnames ();

			if (firstnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames[0],
					lastnames[0],
					encodingHelper,
					out firstname,
					out lastname
				);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames,
					lastnames[0],
					encodingHelper,
					out firstname,
					out lastname
				);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 2)
			{
				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames,
					lastnames,
					encodingHelper,
					out firstname,
					out lastname
				);
			}
			else
			{
				throw new NotSupportedException ();
			}
		}


		private static void GetHouseholdName
		(
			string rawFirstname,
			string rawLastname,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			// This case is simple, we have a single person, so firstname and lastname are its
			// firstname and lastname.

			SubscriptionFileWriter.GetFirstAndLastname
			(
				rawFirstname,
				rawLastname,
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				false,
				out firstname,
				out lastname
			);
		}


		private static void GetHouseholdName
		(
			List<string> rawFirstnames,
			string rawLastname,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			// Here we have two persons with a common lastname. So we want to put both firstnames
			// in firstname and the lastname in lastname. Both firstnames are supposed to be joined
			// by a "et".

			var shortFirstnames = rawFirstnames
				.Select (n => NameProcessor.GetAbbreviatedFirstname (n));

			// TODO Shorten the last name in a proper way if necessary.
			lastname = SubscriptionFileWriter.Process
			(
				rawLastname,
				SubscriptionFileLine.LastnameLength,
				encodingHelper
			);

			bool truncatedFirstname;
			bool abbreviatedFirstname = false;

			firstname = SubscriptionFileWriter.Process
			(
				string.Join (" et ", rawFirstnames),
				SubscriptionFileLine.FirstnameLength,
				encodingHelper,
				out truncatedFirstname
			);

			// If the first name has been truncated, we abbreviate it.
			if (truncatedFirstname)
			{
				Debug.WriteLine ("Firstname exceeding 30 chars:" + firstname);

				firstname = SubscriptionFileWriter.Process
				(
					string.Join (" et ", shortFirstnames),
					SubscriptionFileLine.FirstnameLength,
					encodingHelper
				);

				abbreviatedFirstname = true;
			}

			// If the name and the first name are too long together, we must shorten them.
			var nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
			if (nameLength > SubscriptionFileLine.NameLengthMax)
			{
				Debug.WriteLine ("Name exceeding 43 chars:" + lastname + ", " + firstname);

				// Abbreviate the first name if we haven't done it already.
				if (!abbreviatedFirstname)
				{
					firstname = SubscriptionFileWriter.Process
					(
						string.Join (" et ", shortFirstnames),
						SubscriptionFileLine.FirstnameLength,
						encodingHelper
					);
				}

				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength > SubscriptionFileLine.NameLengthMax)
				{
					// TODO Do something more intelligent here.
					var maxFirstnameLength = SubscriptionFileLine.NameLengthMax - lastname.Length - 1;
					firstname = firstname.Truncate (maxFirstnameLength);
				}
			}
		}


		private static void GetHouseholdName
		(
			List<string> rawFirstnames,
			List<string> rawLastnames,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			// Here we have to persons with different lastnames. So we want to put the firstname and
			// the lastname of the first person in firstname and the firstname and the lastname of
			// the second person in the lastname. Both fields are supposed to be joined by a "et",
			// either at the end of the firstname or at the start of the lastname.

			var forceShortNames = new bool[] { false, true };

			foreach (var param in forceShortNames)
			{
				var firstnames = new string[2];
				var lastnames = new string[2];

				for (int i = 0; i < 2; i++)
				{
					string fn;
					string ln;

					SubscriptionFileWriter.GetFirstAndLastname
					(
						rawFirstnames[i],
						rawLastnames[i],
						encodingHelper,
						SubscriptionFileLine.FirstnameLength,
						SubscriptionFileLine.FirstnameLength,
						SubscriptionFileLine.FirstnameLength,
						false,
						out fn,
						out ln
					);

					firstnames[i] = fn;
					lastnames[i] = ln;
				}

				firstname = firstnames[0] + " " + lastnames[0];
				lastname = firstnames[1] + " " + lastnames[1];

				if (firstname.Length < SubscriptionFileLine.FirstnameLength - 3)
				{
					firstname += " et";
				}
				else if (lastname.Length > SubscriptionFileLine.LastnameLength - 3)
				{
					lastname = "et " + lastname;
				}
				else
				{
					// If we can't place the separator, try with shorter names.
					continue;
				}

				var nameLength = SubscriptionFileLine.GetNameLength (firstname, lastname);
				if (nameLength > SubscriptionFileLine.NameLengthMax)
				{
					// If the names are too long, try with shorter names.
					continue;
				}

				// The names where short enough to fit on the fields and to satisfy the constraints,
				// so we return thew now.
				return;
			}

			// TODO Better handle this case. Maybe we should make a fallback on a single head and
			// change the title to Famille.
			throw new NotSupportedException ();
		}


		private static void GetFirstAndLastname
		(
			string rawFirstname,
			string rawLastname,
			EncodingHelper encodingHelper,
			int maxFirstnameLength,
			int maxLastnameLength,
			int maxFullnameLength,
			bool forceShortFirstname,
			out string firstname,
			out string lastname
		)
		{
			// TODO Shorten the last name in a proper way if necessary.
			lastname = SubscriptionFileWriter.Process
			(
				rawLastname,
				maxLastnameLength,
				encodingHelper
			);

			bool truncatedFirstname;
			bool abbreviatedFirstname = false;

			firstname = SubscriptionFileWriter.Process
			(
				rawFirstname,
				maxFirstnameLength,
				encodingHelper,
				out truncatedFirstname
			);

			// If the first name has been truncated, we abbreviate it.
			if (truncatedFirstname || forceShortFirstname)
			{
				Debug.WriteLine ("Firstname exceeding 30 chars:" + firstname);

				firstname = SubscriptionFileWriter.Process
				(
					NameProcessor.GetAbbreviatedFirstname (rawFirstname),
					maxFirstnameLength,
					encodingHelper
				);

				abbreviatedFirstname = true;
			}

			// If the name and the first name are too long together, we must shorten them.
			var nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
			if (nameLength > maxFullnameLength)
			{
				Debug.WriteLine ("Name exceeding 43 chars:" + lastname + ", " + firstname);

				// Abbreviate the first name if we haven't done it already.
				if (!abbreviatedFirstname)
				{
					firstname = SubscriptionFileWriter.Process
					(
						NameProcessor.GetAbbreviatedFirstname (rawFirstname),
						maxFirstnameLength,
						encodingHelper
					);
				}

				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength > maxFullnameLength)
				{
					// TODO Do something more intelligent here.
					var maxLength = maxFullnameLength - lastname.Length - 1;
					firstname = firstname.Truncate (maxLength);
				}
			}
		}


		private static void GetHouseholdFirstAddressPart
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber
		)
		{
			if (string.IsNullOrEmpty (address.PostBox))
			{
				SubscriptionFileWriter.GetHouseholdFirstAddressPartRegular
				(
					address,
					encodingHelper,
					out addressComplement,
					out street,
					out houseNumber
				);
			}
			else
			{
				SubscriptionFileWriter.GetHouseholdFirstAddressPartPostBox
				(
					address,
					encodingHelper,
					out addressComplement,
					out street,
					out houseNumber
				);
			}
		}


		private static void GetHouseholdFirstAddressPartRegular
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber)
		{
			addressComplement = SubscriptionFileWriter.Process
			(
				address.AddressLine1,
				SubscriptionFileLine.AddressComplementLength,
				encodingHelper
			);

			street = SubscriptionFileWriter.Process
			(
				address.StreetUserFriendly,
				SubscriptionFileLine.StreetLength,
				encodingHelper
			);

			houseNumber = SubscriptionFileWriter.Process
			(
				address.HouseNumberAndComplement.Replace (" ", ""),
				SubscriptionFileLine.HouseNumberLength,
				encodingHelper
			);

			if (!SubscriptionFileLine.SwissHouseNumberRegex.IsMatch (houseNumber))
			{
				Debug.WriteLine ("Invalid house number: " + houseNumber);

				// TODO Do something more intelligent here.

				var digits = houseNumber
					.TakeWhile (c => char.IsDigit (c))
					.Take (7);

				var letters = houseNumber
					.SkipWhile (c => char.IsDigit (c))
					.TakeWhile (c => !char.IsDigit (c))
					.Take (3);

				var chars = digits.Concat (letters).ToArray ();

				houseNumber = new String (chars);
			}
		}


		private static void GetHouseholdFirstAddressPartPostBox
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber
		)
		{
			// If we have a post box, we put it in place of the street, we put the street (with
			// house number and complement) in place of the complement and we drop the complement.

			addressComplement = SubscriptionFileWriter.Process
			(
				address.StreetHouseNumberAndComplement,
				SubscriptionFileLine.AddressComplementLength,
				encodingHelper
			);

			street = SubscriptionFileWriter.Process
			(
				address.PostBox,
				SubscriptionFileLine.StreetLength,
				encodingHelper
			);

			houseNumber = "";
		}


		private static SubscriptionFileLine GetLegalPersonLine
		(
			AiderSubscriptionEntity subscription,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			// TODO Manage the cases with the legal persons.

			// TODO Manage the case where we have a legal person with a contact person. We might
			// want to put the title, firstname and lastname of the person AND the name of the
			// company. So we might want to put the name of the company in the address complement.
			// Except that if the address has a post box or already has a complement, we're screwed.

			throw new NotImplementedException ();
		}


		private static string Process(string value, int maxLength, EncodingHelper encodingHelper)
		{
			bool truncated;

			return SubscriptionFileWriter.Process (value, maxLength, encodingHelper, out truncated);
		}


		private static string Process
		(
			string value,
			int maxLength,
			EncodingHelper encodingHelper,
			out bool truncated
		)
		{
			truncated = false;

			if (value == null)
			{
				return "";
			}

			var converted = encodingHelper.ConvertToEncoding (value);

			if (converted.Length > maxLength)
			{
				truncated = true;

				var truncatedValue = converted.Truncate (maxLength);

				Debug.WriteLine ("Value too long: " + value + ". Truncated to: " + truncatedValue);

				return truncatedValue;
			}
			else
			{
				return converted;
			}
		}


		private static string GetZipCode(AiderTownEntity town)
		{
			return town.Country.IsSwitzerland ()
				? InvariantConverter.ToString (town.SwissZipCode.Value) + town.SwissZipCodeAddOn
				: town.ZipCode;
		}


		private static int GetPostmanNumber(AiderAddressEntity address, MatchSortEtl etl)
		{
			// The specs requires 0 for an addresses outside Switzerland.
			if (!address.Town.Country.IsSwitzerland ())
			{
				return SubscriptionFileLine.ForeignPostmanNumber;
			}

			// The specs requires 999 for adresses with a post box.
			if (!String.IsNullOrEmpty (address.PostBox))
			{
				return SubscriptionFileLine.SwissPostmanNumberPostbox;
			}

			var zipCode = address.Town.SwissZipCode.ToString ();
			var zipAddOn = address.Town.SwissZipCodeAddOn;
			var street = address.StreetUserFriendly;
			var number = address.HouseNumber.ToString ();
			var complement = address.HouseNumberComplement;

			var postmanNumber = etl.GetMessenger (zipCode, zipAddOn, street, number, complement);

			if (postmanNumber == null)
			{
				// TODO Correct this.

				return 123;
			}

			return Convert.ToInt32 (postmanNumber);
		}


	}


}

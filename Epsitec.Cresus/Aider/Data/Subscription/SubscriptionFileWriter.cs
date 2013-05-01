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
				() => rawFirstname,
				() => NameProcessor.GetAbbreviatedFirstname (rawFirstname),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastname (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				false,
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

			SubscriptionFileWriter.GetFirstAndLastname
			(
				() => string.Join (" et ", rawFirstnames),
				() => string.Join
					(
						" et ",
						rawFirstnames.Select (n => NameProcessor.GetAbbreviatedFirstname (n))
					),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastname (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				false,
				false,
				out firstname,
				out lastname
			);
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

			for (int step = 0; step < 3; step++)
			{
				bool forceShortFirstname = step > 0;
				bool forceShortLastname = step > 1;

				var firstnames = new string[2];
				var lastnames = new string[2];

				for (int i = 0; i < 2; i++)
				{
					string fn;
					string ln;

					// We substract 5, so we are sure that we leave a place for the firstname and
					// the lastname, because both of the must be smaller that the maximum length.
					var maxNamePartLength = SubscriptionFileLine.FirstnameLength - 5;

					// We substract 1, because later on we will join both names by a space.
					var maxFullNameLength = SubscriptionFileLine.FirstnameLength - 1;

					SubscriptionFileWriter.GetFirstAndLastname
					(
						() => rawFirstnames[i],
						() => NameProcessor.GetAbbreviatedFirstname (rawFirstnames[i]),
						() => rawLastnames[i],
						() => NameProcessor.GetShortenedLastname (rawLastnames[i]),
						encodingHelper,
						maxNamePartLength,
						maxNamePartLength,
						maxFullNameLength,
						forceShortFirstname,
						forceShortLastname,
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
			Func<string> firstnameGetter,
			Func<string> shortFirstnameGetter,
			Func<string> lastnameGetter,
			Func<string> shortLastnameGetter,
			EncodingHelper encodingHelper,
			int maxFirstnameLength,
			int maxLastnameLength,
			int maxFullnameLength,
			bool forceShortFirstname,
			bool forceShortLastname,
			out string firstname,
			out string lastname
		)
		{
			bool truncatedLastname;
			bool shortenedLastname = false;

			lastname = SubscriptionFileWriter.Process
			(
				lastnameGetter (),
				maxLastnameLength,
				encodingHelper,
				out truncatedLastname
			);

			// If the lastname has been truncated, we shorten it.
			if (truncatedLastname || forceShortLastname)
			{
				if (truncatedLastname)
				{
					Debug.WriteLine ("Lastname exceeding 30 chars:" + lastname);
				}

				lastname = SubscriptionFileWriter.Process
				(
					shortLastnameGetter (),
					maxLastnameLength,
					encodingHelper
				);

				shortenedLastname = true;
			}

			bool truncatedFirstname;
			bool shortenedFirstname = false;

			firstname = SubscriptionFileWriter.Process
			(
				firstnameGetter (),
				maxFirstnameLength,
				encodingHelper,
				out truncatedFirstname
			);

			// If the first name has been truncated, we shorten it.
			if (truncatedFirstname || forceShortFirstname)
			{
				if (truncatedFirstname)
				{
					Debug.WriteLine ("Firstname exceeding 30 chars:" + firstname);
				}

				firstname = SubscriptionFileWriter.Process
				(
					shortFirstnameGetter (),
					maxFirstnameLength,
					encodingHelper
				);

				shortenedFirstname = true;
			}

			// If the lastname and the firstname are short enough, we return.
			var nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
			if (nameLength <= maxFullnameLength)
			{
				return;
			}

			// The lastname and the first name are too long when put together. We try to shorten
			// them.

			Debug.WriteLine ("Name exceeding 43 chars:" + lastname + ", " + firstname);

			// Shorten the firstname if we haven't done it already.
			if (!shortenedFirstname)
			{
				firstname = SubscriptionFileWriter.Process
				(
					shortFirstnameGetter (),
					maxFirstnameLength,
					encodingHelper
				);

				// If the lastname and the firstname are short enough, we return.
				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength <= maxFullnameLength)
				{
					return;
				}
			}

			// Shorten the lastname if we haven't done it already.
			if (!shortenedLastname)
			{
				lastname = SubscriptionFileWriter.Process
				(
					shortLastnameGetter (),
					maxLastnameLength,
					encodingHelper,
					out shortenedLastname
				);

				// If the lastname and the firstname are short enough, we return.
				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength <= maxFullnameLength)
				{
					return;
				}
			}

			// We could not shorten the first name and last name enough. We shall let our fury fall
			// upon them like Conan the Barbarian! Which means, we truncate them at their maximum
			// length.
			// The truncation algorithm assumes that the max length of the first name and that the
			// max length of the last name are both smaller than the max length of the full name.
			Debug.WriteLine ("Name cannot be shortened enough:" + lastname + ", " + firstname);

			var maxLength = maxFullnameLength - lastname.Length - 1;
			firstname = firstname.Truncate (maxLength);
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

				throw new NotSupportedException ("Invalid house number");
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

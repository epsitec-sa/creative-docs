using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Data.Platform;

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
			var etl = new MatchSortEtl ();
			var encodingHelper = new EncodingHelper (SubscriptionFileLine.GetEncoding ());

			AiderEnumerator.Execute (coreData, (b, subscriptions) =>
			{
				foreach (var subscription in subscriptions)
				{
					if (subscription.Count == 0)
					{
						continue;
					}

					var line = SubscriptionFileWriter.GetLine (subscription, etl, encodingHelper);

					lines.Add (line);
				}
			});

			return lines;
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
			var address = subscription.GetAddress ();
			var town = address.Town;
			var country = town.Country;

			var subscriptionNumber = subscription.Id;
			var copiesCount = subscription.Count;
			var editionId = subscription.GetEditionId ();
			var postmanNumber = SubscriptionFileWriter.GetPostmanNumber (address, etl);

			var title = SubscriptionFileWriter.Process
			(
				subscription.Household.GetHonorific (false),
				SubscriptionFileLine.TitleLength,
				encodingHelper
			);

			var lastname = SubscriptionFileWriter.Process
			(
				subscription.Household.GetLastname (),
				SubscriptionFileLine.LastnameLength,
				encodingHelper
			);

			bool truncated;
			bool abbreviatedFirstname = false;

			var firstname = SubscriptionFileWriter.Process
			(
				subscription.Household.GetFirstname (false),
				SubscriptionFileLine.FirstnameLength,
				encodingHelper,
				out truncated
			);

			// If the first name has been truncated, we abbreviate it.
			if (truncated)
			{
				Debug.WriteLine ("Firstname exceeding 30 chars:" + firstname);

				firstname = SubscriptionFileWriter.Process
				(
					subscription.Household.GetFirstname (true),
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
						subscription.Household.GetFirstname (true),
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

			string addressComplement;
			string street;
			string houseNumber;

			if (string.IsNullOrEmpty (address.PostBox))
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
			else
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

			// TODO Finish this code. It is probably too simple for what we want.

			//var zipCode = address.Town.SwissZipCode.ToString ();
			//var street = address.StreetUserFriendly;
			//var houseNumber = address.HouseNumber.ToString ();

			//var houses = etl.HouseAtStreet (zipCode, street, houseNumber);

			//return Convert.ToInt32 (houses.First ().stageNumber);

			return 123;
		}


	}


}

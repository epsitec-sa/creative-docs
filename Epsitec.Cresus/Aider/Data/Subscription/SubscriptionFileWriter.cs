using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

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
			// TODO Do Weird stuff with post box (put 999 in postman number and put it in complement
			// or somewhere else. Because otherwise, we're screwed if we already have a complement).

			// TODO Manage the case where we have a legal person with a contact person. We might
			// want to put the title, firstname and lastname of the person AND the name of the
			// company. So we might want to put the name of the company in the address complement.
			// Except that if the address has a post box or already has a complement, we're screwed.

			var address = subscription.GetAddress ();
			var town = address.Town;
			var country = town.Country;

			var subscriptionNumber = subscription.Id;
			var copiesCount = subscription.Count;
			var editionId = subscription.GetEditionId ();

			// Maybe here we want to have two cases, one for households and one for legal persons,
			// as the truncation logic if the name is too long, we might want to have 2 different
			// algorithms for its truncation.

			var title = SubscriptionFileWriter.Process
			(
				subscription.GetHonorific (),
				SubscriptionFileLine.TitleLength,
				encodingHelper
			);

			var lastname = SubscriptionFileWriter.Process
			(
				subscription.GetLastname (),
				SubscriptionFileLine.LastnameLength,
				encodingHelper
			);

			var firstname = SubscriptionFileWriter.Process
			(
				subscription.GetFirstname (),
				SubscriptionFileLine.FirstnameLength,
				encodingHelper
			);

			var nameLength = SubscriptionFileLine.GetNameLength (title, lastname, firstname);
			if (nameLength > SubscriptionFileLine.NameLengthMax)
			{
				var message ="Name exceeding 43 chars:"
					+ title + ", "
					+ lastname + ", "
					+ firstname;

				Debug.WriteLine (message);

				// TODO Do something more intelligent here.

				firstname = "";

				nameLength = SubscriptionFileLine.GetNameLength (title, lastname, firstname);

				if (nameLength > SubscriptionFileLine.NameLengthMax)
				{
					title = "";
				}
			}

			var addressComplement = SubscriptionFileWriter.Process
			(
				address.AddressLine1,
				SubscriptionFileLine.AddressComplementLength,
				encodingHelper
			);

			var street = SubscriptionFileWriter.Process
			(
				address.StreetUserFriendly,
				SubscriptionFileLine.StreetLength,
				encodingHelper
			);

			var houseNumber = SubscriptionFileWriter.Process
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
					.Take (3);

				var chars = digits.Concat (letters).ToArray ();

				houseNumber = new String (chars);
			}

			var postmanNumber = SubscriptionFileWriter.GetPostmanNumber (address, etl);

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


		private static string Process(string value, int maxLength, EncodingHelper encodingHelper)
		{
			if (value == null)
			{
				return "";
			}

			var converted = encodingHelper.ConvertToEncoding (value);

			if (converted.Length > maxLength)
			{
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
			return 123;
			var zipCode = address.Town.SwissZipCode.ToString ();
			var street = address.StreetUserFriendly;
			var houseNumber = address.HouseNumber.ToString ();

			var houses = etl.HouseAtStreet (zipCode, street, houseNumber);

			return Convert.ToInt32 (houses.First ().runningNumber);
		}


	}


}

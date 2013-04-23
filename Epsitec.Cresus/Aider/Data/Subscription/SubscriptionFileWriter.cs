using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;
using Epsitec.Data.Platform;

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

			AiderEnumerator.Execute (coreData, (b, subscriptions) =>
			{
				foreach (var subscription in subscriptions)
				{
					if (subscription.Count == 0)
					{
						continue;
					}

					var line = SubscriptionFileWriter.GetLine (subscription,etl);

					lines.Add (line);
				}
			});

			return lines;
		}


		private static SubscriptionFileLine GetLine(AiderSubscriptionEntity subscription,MatchSortEtl etl)
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
				SubscriptionFileLine.titleLength
			);

			var lastname = SubscriptionFileWriter.Process
			(
				subscription.GetLastname (),
				SubscriptionFileLine.lastnameLength
			);

			var firstname = SubscriptionFileWriter.Process
			(
				subscription.GetFirstname (),
				SubscriptionFileLine.firstnameLength
			);

			var nameLength = SubscriptionFileLine.GetNameLength (title, lastname, firstname);
			if (nameLength > SubscriptionFileLine.nameLengthMax)
			{
				var message ="Name exceeding 43 chars:"
					+ title + ", "
					+ lastname + ", "
					+ firstname;

				Debug.WriteLine (message);

				// TODO Do something more intelligent here.

				firstname = "";

				nameLength = SubscriptionFileLine.GetNameLength (title, lastname, firstname);

				if (nameLength > SubscriptionFileLine.nameLengthMax)
				{
					title = "";
				}
			}

			var addressComplement = SubscriptionFileWriter.Process
			(
				address.AddressLine1,
				SubscriptionFileLine.addressComplementLength
			);

			var street = SubscriptionFileWriter.Process
			(
				address.StreetUserFriendly,
				SubscriptionFileLine.streetLength
			);

			var houseNumber = SubscriptionFileWriter.Process
			(
				address.HouseNumberAndComplement.Replace (" ", ""),
				SubscriptionFileLine.houseNumberLength
			);

			if (!SubscriptionFileLine.swissHouseNumberRegex.IsMatch (houseNumber))
			{
				Debug.WriteLine ("Invalid house number: " + houseNumber);

				// TODO Do something more intelligent here.

				var chars = houseNumber
					.Where (c => char.IsDigit (c))
					.Take (10)
					.ToArray ();

				houseNumber = new String (chars);
			}

			var postmanNumber = SubscriptionFileWriter.GetPostmanNumber (address,etl);

			var zipCode = SubscriptionFileWriter.Process
			(
				SubscriptionFileWriter.GetZipCode (town),
				SubscriptionFileLine.zipCodeLength
			);

			var townName = SubscriptionFileWriter.Process
			(
				town.Name,
				SubscriptionFileLine.townLength
			);

			var countryName = SubscriptionFileWriter.Process
			(
				country.Name,
				SubscriptionFileLine.countryLength
			);

			var isSwitzerland = country.IsSwitzerland ();

			return new SubscriptionFileLine
			(
				subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
				addressComplement, street, houseNumber, postmanNumber, zipCode, townName,
				countryName, DistributionMode.Surface, isSwitzerland
			);
		}


		private static string Process(string value, int maxLength)
		{
			if (value == null)
			{
				return "";
			}

			var asciiValue = value.ToASCII ();

			if (asciiValue.Length > maxLength)
			{
				var truncatedValue = asciiValue.Truncate (maxLength);

				Debug.WriteLine ("Value too long: " + value + ". Truncated to: " + truncatedValue);

				return truncatedValue;
			}
			else
			{
				return asciiValue;
			}
		}


		private static string GetZipCode(AiderTownEntity town)
		{
			return town.Country.IsSwitzerland ()
				? InvariantConverter.ToString (town.SwissZipCode.Value) + town.SwissZipCodeAddOn
				: town.ZipCode;
		}


		private static int? GetPostmanNumber(AiderAddressEntity address,MatchSortEtl etl)
		{
			// TODO Get the postman id
			// Check Swiss Post and no Postal Case
			// 

			if (!address.Town.Country.IsSwitzerland ())
				return null;
			if (!String.IsNullOrEmpty (address.PostBox))
				return 999;

			var houses = etl.HouseAtStreet (address.Town.SwissZipCode.ToString(), address.StreetUserFriendly, address.HouseNumber.ToString());

			return Convert.ToInt32(houses.First().runningNumber);
			//	? (int?) new Random ().Next (1, 999)
			//	: null;

			
		}


	}


}

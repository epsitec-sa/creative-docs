using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.IO;

using System.Text;
using System.Text.RegularExpressions;


namespace Epsitec.Aider.Data.Subscription
{


	// NOTE This class is based on the first specification that we received from Tamedia. I commit
	// it know because Pierre wants it, but the specifications have changed a little. Therefore it
	// must be somewhat adapted to match the new specifications.


	internal sealed class SubscriptionFileLine
	{


		public SubscriptionFileLine
		(
			string subscriptionNumber,
			int copiesCount,
			string editionId,
			string title,
			string lastname,
			string firstname,
			string addressComplement,
			string street,
			string houseNumber,
			int postmanNumber,
			string zipCode,
			string town,
			string country,
			DistributionMode distributionMode,
			bool isSwitzerland
		)
		{
			this.CheckArguments
			(
				subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
				addressComplement, street, houseNumber, postmanNumber, zipCode, town, country,
				isSwitzerland
			);

			this.SubscriptionNumber = subscriptionNumber;
			this.CopiesCount = copiesCount;
			this.EditionId = editionId;
			this.Title = title;
			this.Lastname = lastname;
			this.Firstname = firstname;
			this.AddressComplement = addressComplement;
			this.Street = street;
			this.HouseNumber = houseNumber;
			this.PostmanNumber = postmanNumber;
			this.ZipCode = zipCode;
			this.Town = town;
			this.Country = country;
			this.DistributionMode = distributionMode;
		}


		private void CheckArguments
		(
			string subscriptionNumber,
			int copiesCount,
			string editionId,
			string title,
			string lastname,
			string firstname,
			string addressComplement,
			string street,
			string houseNumber,
			int postmanNumber,
			string zipCode,
			string town,
			string country,
			bool isSwitzerland
		)
		{
			this.CheckString
			(
				subscriptionNumber,
				"subscriptionNumber",
				SubscriptionFileLine.subscriptionNumberLength,
				false
			);

			copiesCount.ThrowIf
			(
				x => x < SubscriptionFileLine.copiesCountMin,
				"copiesCount too small"
			);
			copiesCount.ThrowIf
			(
				x => x > SubscriptionFileLine.copiesCountMax,
				"copiesCount too large"
			);

			this.CheckString (editionId, "editionId", SubscriptionFileLine.editionIdLength, false);
			this.CheckString (title, "title", SubscriptionFileLine.titleLength, true);
			this.CheckString (lastname, "lastname", SubscriptionFileLine.lastnameLength, true);
			this.CheckString (firstname, "firstname", SubscriptionFileLine.firstnameLength, true);

			var nameLength = SubscriptionFileLine.GetNameLength (title, firstname, lastname);
			nameLength.ThrowIf (x => x > SubscriptionFileLine.nameLengthMax, "name too long");

			this.CheckString
			(
				addressComplement,
				"addressComplement",
				SubscriptionFileLine.addressComplementLength,
				true
			);

			this.CheckString (street, "street", SubscriptionFileLine.streetLength, true);
			
			this.CheckString
			(
				houseNumber,
				"houseNumber",
				SubscriptionFileLine.houseNumberLength,
				true
			);

			this.CheckString (zipCode, "zipCode", SubscriptionFileLine.zipCodeLength, false);
			this.CheckString (town, "town", SubscriptionFileLine.townLength, false);
			this.CheckString (country, "country", SubscriptionFileLine.countryLength, false);

			// TODO Ensure that these checks are ok.

			if (isSwitzerland)
			{
				houseNumber.ThrowIf
				(
					x => !SubscriptionFileLine.swissHouseNumberRegex.IsMatch (x),
					"houseNumber invalid"
				);

				zipCode.ThrowIf (x => !x.IsNumeric (), "invalid zip code");
				zipCode.ThrowIf
				(
					x => x.Length != SubscriptionFileLine.swissZipCodeLength,
					"invalid zip code"
				);

				postmanNumber.ThrowIf
				(
					x => x < SubscriptionFileLine.swissPostmanNumberMin,
					"postmanNumber too small"
				);

				postmanNumber.ThrowIf
				(
					x => x > SubscriptionFileLine.swissPostmanNumberMax,
					"postmanNumber too large"
				);
			}
			else
			{
				houseNumber.ThrowIf
				(
					x => x.Length > SubscriptionFileLine.foreignHouseNumberLengthMax,
					"houseNumber too long"
				);

				postmanNumber.ThrowIf
				(
					x => x != SubscriptionFileLine.foreignHouseNumberLengthMax,
					"postmanNumber invalid"
				);
			}
		}


		private void CheckString(string value, string name, int length, bool allowEmpty)
		{
			// TODO Check that we must accept all ASCII characters here.

			value.ThrowIfNull (name);
			value.ThrowIf (x => !x.IsASCII (), name + " is not ASCII");
			value.ThrowIf (x => x.Length > length, name + "too long");

			if (!allowEmpty)
			{
				value.ThrowIf (v => v.Length == 0, name + " is empty");
			}
		}


		public string GetText()
		{
			// TODO Check that the padding is to the right and not to the left.

			// TODO Check the order of firstname and lastname.

			return this.SubscriptionNumber.PadRight (SubscriptionFileLine.subscriptionNumberLength)
				+ this.GetCopiesCount ().PadRight (SubscriptionFileLine.copiesCountLength)
				+ this.EditionId.PadRight (SubscriptionFileLine.editionIdLength)
				+ this.Title.PadRight (SubscriptionFileLine.titleLength)
				+ this.Lastname.PadRight (SubscriptionFileLine.lastnameLength)
				+ this.Firstname.PadRight (SubscriptionFileLine.firstnameLength)
				+ this.AddressComplement.PadRight (SubscriptionFileLine.addressComplementLength)
				+ this.Street.PadRight (SubscriptionFileLine.streetLength)
				+ this.HouseNumber.PadRight (SubscriptionFileLine.houseNumberLength)
				+ this.GetPostmanNumber ().PadRight (SubscriptionFileLine.postmanNumberLength)
				+ this.ZipCode.PadRight (SubscriptionFileLine.zipCodeLength)
				+ this.Town.PadRight (SubscriptionFileLine.townLength)
				+ this.Country.PadRight (SubscriptionFileLine.countryLength)
				+ this.GetDistributionMode ().PadRight (SubscriptionFileLine.distributionModeLength)
				+ SubscriptionFileLine.lineEnding;
		}


		private string GetCopiesCount()
		{
			return InvariantConverter.ToString (this.CopiesCount);
		}


		private string GetPostmanNumber()
		{
			return InvariantConverter.ToString (this.PostmanNumber);
		}


		private string GetDistributionMode()
		{
			switch (this.DistributionMode)
			{
				case DistributionMode.Surface:
					return "0";

				case DistributionMode.Plane:
					return "1";

				default:
					throw new NotImplementedException ();
			}
		}


		public static void Write(IEnumerable<SubscriptionFileLine> lines, FileInfo file)
		{
			// TODO Check that we use the good encoding (ASCII 7 bit)

			using (var stream = file.Open (FileMode.Create, FileAccess.Write))
			using (var streamWriter = new StreamWriter (stream, Encoding.ASCII))
			{
				foreach (var line in lines)
				{
					// The text contains the line ending.
					var text = line.GetText ();

					streamWriter.Write (text);
				}
			}
		}


		public static int GetNameLength(string title, string firstname, string lastname)
		{
			int length = 0;
			int notEmpty = 0;

			if (!string.IsNullOrEmpty (title))
			{
				length += title.Length;
				notEmpty += 1;
			}

			if (!string.IsNullOrEmpty (firstname))
			{
				length += firstname.Length;
				notEmpty += 1;
			}

			if (!string.IsNullOrEmpty (lastname))
			{
				length += lastname.Length;
				notEmpty += 1;
			}

			// Adds the count of the spaces between the title, the firstname and the lastname.
			if (notEmpty > 0)
			{
				length += notEmpty - 1;
			}

			return length;
		}


		public readonly string SubscriptionNumber;
		public readonly int CopiesCount;
		public readonly string EditionId;
		public readonly string Title;
		public readonly string Lastname;
		public readonly string Firstname;
		public readonly string AddressComplement;
		public readonly string Street;
		public readonly string HouseNumber;
		public readonly int PostmanNumber;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string Country;
		public readonly DistributionMode DistributionMode;


		public static readonly int subscriptionNumberLength = 10;
		public static readonly int copiesCountLength = 5;
		public static readonly int editionIdLength = 2;
		public static readonly int titleLength = 20;
		public static readonly int lastnameLength = 30;
		public static readonly int firstnameLength = 30;
		public static readonly int addressComplementLength = 30;
		public static readonly int streetLength = 30;
		public static readonly int houseNumberLength = 30;
		public static readonly int postmanNumberLength = 3;
		public static readonly int zipCodeLength = 10;
		public static readonly int townLength = 30;
		public static readonly int countryLength = 30;
		public static readonly int distributionModeLength = 1;


		// TODO Check the bounds on the swiss postman number

		public static readonly int copiesCountMin = 1;
		public static readonly int copiesCountMax = 99999;
		public static readonly int nameLengthMax = 43;
		public static readonly int swissZipCodeLength = 6;
		public static readonly int swissPostmanNumberMin = 1;
		public static readonly int swissPostmanNumberMax = 999;
		public static readonly int swissPostmanNumberPostbox = 999;
		public static readonly Regex swissHouseNumberRegex = new Regex (@"^\d{0,8}[a-zA-z]{0,2}$");
		public static readonly int foreignPostmanNumber = 0;
		public static readonly int foreignHouseNumberLengthMax = 10;


		// TODO Check that this is the good ling ending.

		private static readonly string lineEnding = "\n";


	}


}

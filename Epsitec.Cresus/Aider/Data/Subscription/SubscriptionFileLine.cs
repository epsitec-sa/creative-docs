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
				SubscriptionFileLine.SubscriptionNumberLength,
				false
			);

			copiesCount.ThrowIf
			(
				x => x < SubscriptionFileLine.CopiesCountMin,
				"copiesCount too small"
			);
			copiesCount.ThrowIf
			(
				x => x > SubscriptionFileLine.CopiesCountMax,
				"copiesCount too large"
			);

			this.CheckString (editionId, "editionId", SubscriptionFileLine.EditionIdLength, false);
			this.CheckString (title, "title", SubscriptionFileLine.TitleLength, true);
			this.CheckString (lastname, "lastname", SubscriptionFileLine.LastnameLength, true);
			this.CheckString (firstname, "firstname", SubscriptionFileLine.FirstnameLength, true);

			var nameLength = SubscriptionFileLine.GetNameLength (title, firstname, lastname);
			nameLength.ThrowIf (x => x > SubscriptionFileLine.NameLengthMax, "name too long");

			this.CheckString
			(
				addressComplement,
				"addressComplement",
				SubscriptionFileLine.AddressComplementLength,
				true
			);

			this.CheckString (street, "street", SubscriptionFileLine.StreetLength, true);
			
			this.CheckString
			(
				houseNumber,
				"houseNumber",
				SubscriptionFileLine.HouseNumberLength,
				true
			);

			this.CheckString (zipCode, "zipCode", SubscriptionFileLine.ZipCodeLength, false);
			this.CheckString (town, "town", SubscriptionFileLine.TownLength, false);
			this.CheckString (country, "country", SubscriptionFileLine.CountryLength, false);

			// TODO Ensure that these checks are ok.

			if (isSwitzerland)
			{
				houseNumber.ThrowIf
				(
					x => !SubscriptionFileLine.SwissHouseNumberRegex.IsMatch (x),
					"houseNumber invalid"
				);

				zipCode.ThrowIf (x => !x.IsNumeric (), "invalid zip code");
				zipCode.ThrowIf
				(
					x => x.Length != SubscriptionFileLine.SwissZipCodeLength,
					"invalid zip code"
				);

				postmanNumber.ThrowIf
				(
					x => x < SubscriptionFileLine.SwissPostmanNumberMin,
					"postmanNumber too small"
				);

				postmanNumber.ThrowIf
				(
					x => x > SubscriptionFileLine.SwissPostmanNumberMax,
					"postmanNumber too large"
				);
			}
			else
			{
				houseNumber.ThrowIf
				(
					x => x.Length > SubscriptionFileLine.ForeignHouseNumberLengthMax,
					"houseNumber too long"
				);

				postmanNumber.ThrowIf
				(
					x => x != SubscriptionFileLine.ForeignHouseNumberLengthMax,
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
			// TODO Check the order of firstname and lastname.

			return this.SubscriptionNumber.PadRight (SubscriptionFileLine.SubscriptionNumberLength)
				+ this.GetCopiesCount ().PadLeft (SubscriptionFileLine.CopiesCountLength)
				+ this.EditionId.PadRight (SubscriptionFileLine.EditionIdLength)
				+ this.Title.PadRight (SubscriptionFileLine.TitleLength)
				+ this.Lastname.PadRight (SubscriptionFileLine.LastnameLength)
				+ this.Firstname.PadRight (SubscriptionFileLine.FirstnameLength)
				+ this.AddressComplement.PadRight (SubscriptionFileLine.AddressComplementLength)
				+ this.Street.PadRight (SubscriptionFileLine.StreetLength)
				+ this.HouseNumber.PadRight (SubscriptionFileLine.HouseNumberLength)
				+ this.GetPostmanNumber ().PadRight (SubscriptionFileLine.PostmanNumberLength)
				+ this.ZipCode.PadLeft (SubscriptionFileLine.ZipCodeLength)
				+ this.Town.PadRight (SubscriptionFileLine.TownLength)
				+ this.Country.PadRight (SubscriptionFileLine.CountryLength)
				+ this.GetDistributionMode ().PadRight (SubscriptionFileLine.DistributionModeLength)
				+ SubscriptionFileLine.LineEnding;
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


		public static readonly int SubscriptionNumberLength = 10;
		public static readonly int CopiesCountLength = 5;
		public static readonly int EditionIdLength = 2;
		public static readonly int TitleLength = 20;
		public static readonly int LastnameLength = 30;
		public static readonly int FirstnameLength = 30;
		public static readonly int AddressComplementLength = 30;
		public static readonly int StreetLength = 30;
		public static readonly int HouseNumberLength = 30;
		public static readonly int PostmanNumberLength = 3;
		public static readonly int ZipCodeLength = 10;
		public static readonly int TownLength = 30;
		public static readonly int CountryLength = 30;
		public static readonly int DistributionModeLength = 1;


		// TODO Check the bounds on the swiss postman number

		public static readonly int CopiesCountMin = 1;
		public static readonly int CopiesCountMax = 99999;
		public static readonly int NameLengthMax = 43;
		public static readonly int SwissZipCodeLength = 6;
		public static readonly int SwissPostmanNumberMin = 1;
		public static readonly int SwissPostmanNumberMax = 999;
		public static readonly int SwissPostmanNumberPostbox = 999;
		public static readonly Regex SwissHouseNumberRegex = new Regex (@"^\d{0,8}[a-zA-z]{0,2}$");
		public static readonly int ForeignPostmanNumber = 0;
		public static readonly int ForeignHouseNumberLengthMax = 10;


		// TODO Check that this is the good ling ending.

		private static readonly string LineEnding = "\n";


	}


}

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;
using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.IO;

using System.Text;
using System.Text.RegularExpressions;


namespace Epsitec.Aider.Data.Subscription
{


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
			int? districtNumber,
			string zipCode,
			string town,
			string country,
			DistributionMode distributionMode,
			bool isSwitzerland,
			string canton
		)
		{
			this.CheckArguments
			(
				subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
				addressComplement, street, houseNumber, districtNumber, zipCode, town, country,
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
			this.DistrictNumber = districtNumber;
			this.ZipCode = zipCode;
			this.Town = town;
			this.Country = country;
			this.DistributionMode = distributionMode;
			this.Canton = canton;
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
			int? districtNumber,
			string zipCode,
			string town,
			string country,
			bool isSwitzerland
		)
		{
			var encodingHelper = new EncodingHelper (SubscriptionFileLine.GetEncoding ());

			this.CheckString
			(
				subscriptionNumber,
				"subscriptionNumber",
				SubscriptionFileLine.SubscriptionNumberLength,
				false,
				encodingHelper
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

			this.CheckString
			(
				editionId,
				"editionId",
				SubscriptionFileLine.EditionIdLength,
				false,
				encodingHelper
			);

			this.CheckString
			(
				title,
				"title",
				SubscriptionFileLine.TitleLength,
				true,
				encodingHelper
			);

			this.CheckString
			(
				lastname,
				"lastname",
				SubscriptionFileLine.LastnameLength,
				true,
				encodingHelper
			);

			this.CheckString
			(
				firstname,
				"firstname",
				SubscriptionFileLine.FirstnameLength,
				true,
				encodingHelper
			);

			var nameLength = SubscriptionFileLine.GetNameLength (firstname, lastname);
			nameLength.ThrowIf (x => x > SubscriptionFileLine.NameLengthMax, "name too long");

			this.CheckString
			(
				addressComplement,
				"addressComplement",
				SubscriptionFileLine.AddressComplementLength,
				true,
				encodingHelper
			);

			this.CheckString
			(
				street,
				"street",
				SubscriptionFileLine.StreetLength,
				true,
				encodingHelper
			);
			
			this.CheckString
			(
				houseNumber,
				"houseNumber",
				SubscriptionFileLine.HouseNumberLength,
				true,
				encodingHelper
			);

			this.CheckString
			(
				zipCode,
				"zipCode",
				SubscriptionFileLine.ZipCodeLength,
				false,
				encodingHelper
			);

			this.CheckString
			(
				town,
				"town",
				SubscriptionFileLine.TownLength,
				false,
				encodingHelper
			);

			this.CheckString
			(
				country,
				"country",
				SubscriptionFileLine.CountryLength,
				false,
				encodingHelper
			);

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

				if (districtNumber.HasValue)
				{
					districtNumber.ThrowIf
					(
						x => x < SubscriptionFileLine.SwissDistrictNumberMin,
						"districtNumber too small"
					);

					districtNumber.ThrowIf
					(
						x => x > SubscriptionFileLine.SwissDistrictNumberMax,
						"districtNumber too large"
					);
				}
			}
			else
			{
				districtNumber.ThrowIf
				(
					x => x != SubscriptionFileLine.ForeignDistrictNumber,
					"districtNumber invalid"
				);
			}
		}


		private void CheckString
		(
			string value,
			string name,
			int length,
			bool allowEmpty,
			EncodingHelper encodingHelper
		)
		{
			value.ThrowIfNull (name);
			value.ThrowIf (x => !encodingHelper.IsWithinEncoding (x), name + " is not valid");
			value.ThrowIf (x => x.Length > length, name + "too long");

			if (!allowEmpty)
			{
				value.ThrowIf (v => v.Length == 0, name + " is empty");
			}
		}


		public string GetText()
		{
			return this.SubscriptionNumber.PadRight (SubscriptionFileLine.SubscriptionNumberLength)
				+ this.GetCopiesCount ().PadLeft (SubscriptionFileLine.CopiesCountLength)
				+ this.EditionId.PadRight (SubscriptionFileLine.EditionIdLength)
				+ this.Title.PadRight (SubscriptionFileLine.TitleLength)
				+ this.Lastname.PadRight (SubscriptionFileLine.LastnameLength)
				+ this.Firstname.PadRight (SubscriptionFileLine.FirstnameLength)
				+ this.AddressComplement.PadRight (SubscriptionFileLine.AddressComplementLength)
				+ this.Street.PadRight (SubscriptionFileLine.StreetLength)
				+ this.HouseNumber.PadRight (SubscriptionFileLine.HouseNumberLength)
				+ this.GetDistrictNumber ().PadRight (SubscriptionFileLine.DistrictNumberLength)
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


		private string GetDistrictNumber()
		{
			return this.DistrictNumber.HasValue
				? InvariantConverter.ToString (this.DistrictNumber)
				: "";
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
			var encoding = SubscriptionFileLine.GetEncoding ();

			using (var stream = file.Open (FileMode.Create, FileAccess.Write))
			using (var streamWriter = new StreamWriter (stream, encoding))
			{
				foreach (var line in lines)
				{
					// The text contains the line ending.
					var text = line.GetText ();

					streamWriter.Write (text);
				}
			}
		}


		public static int GetNameLength(string firstname, string lastname)
		{
			int length = firstname.Length + lastname.Length;

			// Adds the count of the space between the firstname and the lastname.
			if (firstname.Length > 0 && lastname.Length > 0)
			{
				length += 1;
			}

			return length;
		}


		public static Encoding GetEncoding()
		{
			return Encoding.GetEncoding ("ISO-8859-1");
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
		public readonly int? DistrictNumber;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string Country;
		public readonly DistributionMode DistributionMode;
		public readonly string Canton;

		public static readonly int SubscriptionNumberLength = 10;
		public static readonly int CopiesCountLength = 5;
		public static readonly int EditionIdLength = 2;
		public static readonly int TitleLength = 20;
		public static readonly int LastnameLength = 30;
		public static readonly int FirstnameLength = 30;
		public static readonly int AddressComplementLength = 30;
		public static readonly int StreetLength = 30;
		public static readonly int HouseNumberLength = 10;
		public static readonly int DistrictNumberLength = 3;
		public static readonly int ZipCodeLength = 10;
		public static readonly int TownLength = 30;
		public static readonly int CountryLength = 30;
		public static readonly int DistributionModeLength = 1;


		public static readonly int CopiesCountMin = 1;
		public static readonly int CopiesCountMax = 99999;
		public static readonly int NameLengthMax = 47;
		public static readonly int SwissZipCodeLength = 6;
		public static readonly int SwissDistrictNumberMin = 1;
		public static readonly int SwissDistrictNumberMax = 999;
		public static readonly int SwissDistrictNumberPostbox = 999;
		public static readonly Regex SwissHouseNumberRegex = new Regex (@"^\d{0,7}[a-zA-z]{0,3}$");
		public static readonly int ForeignDistrictNumber = 0;


		private static readonly string LineEnding = "\r\n";


	}


}

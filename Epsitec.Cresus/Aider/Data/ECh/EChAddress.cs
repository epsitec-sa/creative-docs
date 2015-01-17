//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Data.Platform;

namespace Epsitec.Aider.Data.ECh
{
	internal sealed partial class EChAddress
	{
		public EChAddress(string addressLine1, string street, string houseNumber, string town, int swissZipCode, int swissZipCodeAddOn, int swissZipCodeId, string countryCode)
		{
			this.AddressLine1      = addressLine1 ?? "";
			this.Street            = street ?? "";
			this.HouseNumber       = houseNumber ?? "";
			this.Town              = town ?? "";
			this.SwissZipCode      = swissZipCode;
			this.SwissZipCodeAddOn = swissZipCodeAddOn;
			this.SwissZipCodeId    = SwissPostZipRepository.Current.FindOnrp (swissZipCodeId, swissZipCode, town);
			this.CountryCode       = countryCode;
		}

		
		public readonly string AddressLine1;
		public readonly string Street;
		public readonly string HouseNumber;
		public readonly string Town;
		public readonly int SwissZipCode;
		public readonly int	SwissZipCodeAddOn;
		public readonly int SwissZipCodeId;
		public readonly string CountryCode;


		public override string ToString()
		{
			return string.Format ("{0} {1} @ {2} {3}", this.Street, this.HouseNumber, this.SwissZipCode, this.Town);
		}

		public FormattedText GetSwissPostalAddress()
		{
			var friendlyStreet = SwissPostStreet.ConvertToUserFriendlyStreetName (this.Street).CapitalizeFirstLetter ();

			return TextFormatter.FormatText (
				this.AddressLine1, "\n",
				friendlyStreet, this.HouseNumber.ToUpperInvariant (), "\n",
				this.SwissZipCode, this.Town, "\n");
		}
	}
}

//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Data.Ech
{
	internal sealed partial class EChAddress
	{
		// NOTE: Here we discard the fields addressLine1 and dwellingNumber.


		public EChAddress(string addressLine1, string street, string houseNumber, string town, string swissZipCode, string swissZipCodeAddOn, string swissZipCodeId, string countryCode)
		{
			this.addressLine1      = addressLine1;
			this.street            = street;
			this.houseNumber       = houseNumber;
			this.town              = town;
			this.swissZipCode      = swissZipCode;
			this.swissZipCodeAddOn = swissZipCodeAddOn;
			this.swissZipCodeId    = swissZipCodeId;
			this.countryCode       = countryCode;

			if (this.countryCode == "CH")
			{
				PatchEngine.ApplyFix (this);
			}
		}


		public string							AddressLine1
		{
			get
			{
				return this.addressLine1;
			}
		}

		public string							Street
		{
			get
			{
				return this.street;
			}
		}
		
		public string							HouseNumber
		{
			get
			{
				return this.houseNumber;
			}
		}

		public string							Town
		{
			get
			{
				return this.town;
			}
		}

		public string							SwissZipCode
		{
			get
			{
				return this.swissZipCode;
			}
		}

		public string							SwissZipCodeAddOn
		{
			get
			{
				return this.swissZipCodeAddOn;
			}
		}

		public string							SwissZipCodeId
		{
			get
			{
				return this.swissZipCodeId;
			}
		}

		public string							CountryCode
		{
			get
			{
				return this.countryCode;
			}
		}


		private void Patch(IEnumerable<SwissPostStreetInformation> hits)
		{
			int house = InvariantConverter.ParseInt (this.houseNumber);
			var info  = hits.FirstOrDefault (x => x.MatchHouseNumber (house));

			if (info == null)
			{
				//	Cannot apply patch -- unknown street or house number
				return;
			}

			var zip = SwissPostZipRepository.Current.FindZips (info.ZipCode, info.ZipComplement).FirstOrDefault ();

			if (zip == null)
			{
				//	Cannot apply patch -- unknown zip
				return;
			}

			this.street = info.StreetName;
			this.town   = zip.LongName;
			
			this.swissZipCode      = zip.ZipCode.ToString ("0000");
			this.swissZipCodeAddOn = zip.ZipComplement == 0 ? "" : zip.ZipComplement.ToString ("0");
			this.swissZipCodeId    = zip.OnrpCode.ToString ("0");
		}

		private string							addressLine1;
		private string							street;
		private string							houseNumber;
		private string							town;
		private string							swissZipCode;
		private string							swissZipCodeAddOn;
		private string							swissZipCodeId;
		private string							countryCode;
	}
}

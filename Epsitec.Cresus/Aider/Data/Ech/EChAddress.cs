//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

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

			PatchEngine.ApplyFix (this);
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

//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Data.ECh
{
	/// <summary>
	/// The <c>EChAddress</c> class represents an eCH address element. The street and
	/// zip code information will be changed so as to match ... the names from the
	/// MAT[CH]street database.
	/// </summary>
	internal sealed partial class EChAddress
	{
		public EChAddress(string addressLine1, string street, string houseNumber, string town, string swissZipCode, string swissZipCodeAddOn, string swissZipCodeId, string countryCode)
		{
			this.addressLine1      = addressLine1;
			this.street            = street;
			this.houseNumber       = houseNumber;
			this.town              = town;
			this.swissZipCode      = InvariantConverter.ParseInt (swissZipCode);
			this.swissZipCodeAddOn = InvariantConverter.ParseInt (swissZipCodeAddOn);
			this.swissZipCodeId    = InvariantConverter.ParseInt (swissZipCodeId);
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

		public int								SwissZipCode
		{
			get
			{
				return this.swissZipCode;
			}
		}

		public int								SwissZipCodeAddOn
		{
			get
			{
				return this.swissZipCodeAddOn;
			}
		}

		public int								SwissZipCodeId
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
			
			this.swissZipCode      = zip.ZipCode;
			this.swissZipCodeAddOn = zip.ZipComplement;
			this.swissZipCodeId    = zip.OnrpCode;
		}

		
		private readonly string					addressLine1;
		private string							street;
		private readonly string					houseNumber;
		private string							town;
		private int								swissZipCode;
		private int								swissZipCodeAddOn;
		private int								swissZipCodeId;
		private readonly string					countryCode;
	}
}

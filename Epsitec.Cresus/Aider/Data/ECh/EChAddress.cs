//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


namespace Epsitec.Aider.Data.ECh
{
	
	
	internal sealed partial class EChAddress
	{
		
		
		public EChAddress(string addressLine1, string street, string houseNumber, string town, int swissZipCode, int swissZipCodeAddOn, int swissZipCodeId, string countryCode)
		{
			this.AddressLine1 = addressLine1;
			this.Street = street;
			this.HouseNumber = houseNumber;
			this.Town = town;
			this.SwissZipCode = swissZipCode;
			this.SwissZipCodeAddOn = swissZipCodeAddOn;
			this.SwissZipCodeId = swissZipCodeId;
			this.CountryCode = countryCode;
		}

		
		public readonly string AddressLine1;
		public readonly string Street;
		public readonly string HouseNumber;
		public readonly string Town;
		public readonly int SwissZipCode;
		public readonly int	SwissZipCodeAddOn;
		public readonly int SwissZipCodeId;
		public readonly string CountryCode;


	}


}

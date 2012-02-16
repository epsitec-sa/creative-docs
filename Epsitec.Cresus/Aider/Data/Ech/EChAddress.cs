namespace Epsitec.Aider.Data.Ech
{


	internal sealed class EChAddress
	{

		// NOTE: Here we discard the fields addressLine1 and dwellingNumber.


		public EChAddress(string addressLine1, string street, string houseNumber, string town, string swissZipCode, string swissZipCodeAddOn, string swissZipCodeId, string countryCode)
		{
#if false
			if ((string.IsNullOrEmpty (addressLine1) == false) &&
				(addressLine1.StartsWith ("c/o") == false) &&
				(street == town))
			{
				street = addressLine1;
				addressLine1 = "";
			}
#endif

			EChAddressFixesRepository.Current.ApplyFix (ref swissZipCode, ref street, addressLine1);

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
		public readonly string SwissZipCode;
		public readonly string SwissZipCodeAddOn;
		public readonly string SwissZipCodeId;
		public readonly string CountryCode;


	}


}

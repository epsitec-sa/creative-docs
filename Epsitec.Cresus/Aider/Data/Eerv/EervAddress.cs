namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervAddress
	{


		public EervAddress(string firstAddressLine, string streetName, int? houseNumber, string houseNumberComplement, string zipCode, string town, string countryCode)
		{
			this.FirstAddressLine = firstAddressLine;
			this.StreetName = streetName;
			this.HouseNumber = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode = zipCode;
			this.Town = town;
			this.CountryCode = countryCode;
		}


		public bool IsInSwitzerland()
		{
			return this.CountryCode == "CH";
		}


		public readonly string FirstAddressLine;
		public readonly string StreetName;
		public readonly int? HouseNumber;
		public readonly string HouseNumberComplement;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string CountryCode;


	}


}


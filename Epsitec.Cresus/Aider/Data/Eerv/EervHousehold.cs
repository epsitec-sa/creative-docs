using System;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal sealed class EervHousehold
	{


		public EervHousehold(string id, string firstAddressLine, string streetName, int? houseNumber, string houseNumberComplement, string zipCode, string city, string faxNumber, string privatePhoneNumber, string professionalPhoneNumber, string remarks)
		{
			this.Id = id;
			this.FirstAddressLine = firstAddressLine;
			this.StreetName = streetName;
			this.HouseNumber = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode = zipCode;
			this.City = city;
			this.FaxNumber = faxNumber;
			this.PrivatePhoneNumber = privatePhoneNumber;
			this.ProfessionalPhoneNumber = professionalPhoneNumber;
			this.Remarks = remarks;
		}


		public readonly string Id;
		public readonly string FirstAddressLine;
		public readonly string StreetName;
		public readonly int? HouseNumber;
		public readonly string HouseNumberComplement;
		public readonly string ZipCode;
		public readonly string City;
		public readonly string FaxNumber;
		public readonly string PrivatePhoneNumber;
		public readonly string ProfessionalPhoneNumber;
		public readonly string Remarks;
	
	
	}


}


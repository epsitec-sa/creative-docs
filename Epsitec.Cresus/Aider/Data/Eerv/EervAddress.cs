using Epsitec.Common.Support.Extensions;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervAddress
	{


		public EervAddress(string firstAddressLine, string streetName, int? houseNumber, string houseNumberComplement, string zipCode, string town)
		{
			this.FirstAddressLine = firstAddressLine;
			this.StreetName = streetName;
			this.HouseNumber = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode = zipCode;
			this.Town = town;
		}


		public bool IsInSwitzerland()
		{
			return this.ZipCode.Length == 4 && this.ZipCode.IsInteger ();
		}


		public readonly string FirstAddressLine;
		public readonly string StreetName;
		public readonly int? HouseNumber;
		public readonly string HouseNumberComplement;
		public readonly string ZipCode;
		public readonly string Town;


	}


}


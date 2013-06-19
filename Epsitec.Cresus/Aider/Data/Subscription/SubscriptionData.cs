namespace Epsitec.Aider.Data.Subscription
{


	internal sealed class SubscriptionData
	{


		public SubscriptionData
		(
			string corporateName,
			string title,
			string firstname,
			string lastname,
			string firstAddressLine,
			string postBox,
			string streetName,
			int? houseNumber,
			string houseNumberComplement,
			string zipCode,
			string town,
			string countryCode,
			string comment,
			bool? isCompany
		)
		{
			this.CorporateName = corporateName;
			this.Title = title;
			this.Firstname = firstname;
			this.Lastname = lastname;
			this.FirstAddressLine = firstAddressLine;
			this.PostBox = postBox;
			this.StreetName = streetName;
			this.HouseNumber = houseNumber;
			this.HouseNumberComplement = houseNumberComplement;
			this.ZipCode = zipCode;
			this.Town = town;
			this.CountryCode = countryCode;
			this.Comment = comment;
			this.IsCompany = isCompany;
		}


		public readonly string CorporateName;
		public readonly string Title;
		public readonly string Firstname;
		public readonly string Lastname;
		public readonly string FirstAddressLine;
		public readonly string PostBox;
		public readonly string StreetName;
		public readonly int? HouseNumber;
		public readonly string HouseNumberComplement;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string CountryCode;
		public readonly string Comment;
		public readonly bool? IsCompany;


	}


}

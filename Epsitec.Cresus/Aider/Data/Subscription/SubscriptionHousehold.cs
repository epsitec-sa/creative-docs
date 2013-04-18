using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal sealed class SubscriptionHousehold
	{


		public SubscriptionHousehold
		(
			EntityKey entityKey,
			string addressLine1,
			string postbox,
			string street,
			string houseNumber,
			string zipCode,
			string town,
			string countryCode,
			IEnumerable<string> lastnames,
			int memberMaxAge,
			int memberCount
		)
		{
			this.EntityKey = entityKey;
			this.AddressLine1 = addressLine1;
			this.Postbox = postbox;
			this.Street = street;
			this.HouseNumber = houseNumber;
			this.ZipCode = zipCode;
			this.Town = town;
			this.CountryCode = countryCode;
			this.Lastnames = lastnames
				.OrderBy (n => n)
				.ToList ()
				.AsReadOnly ();
			this.MemberMaxAge = memberMaxAge;
			this.MemberCount = memberCount;
		}


		public readonly EntityKey EntityKey;
		public readonly string AddressLine1;
		public readonly string Postbox;
		public readonly string Street;
		public readonly string HouseNumber;
		public readonly string ZipCode;
		public readonly string Town;
		public readonly string CountryCode;
		public readonly IEnumerable<string> Lastnames;
		public readonly int MemberMaxAge;
		public readonly int MemberCount;


	}


}

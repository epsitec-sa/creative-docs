using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal class SubscriptionHouseholdComparer : IEqualityComparer<SubscriptionHousehold>
	{


		public bool Equals(SubscriptionHousehold h1, SubscriptionHousehold h2)
		{
			if (h1 == null && h2 == null)
			{
				return true;
			}

			if (h1 == null || h2 == null)
			{
				return false;
			}

			return SubscriptionHouseholdComparer.HaveSameAddress (h1, h2)
				&& SubscriptionHouseholdComparer.HaveSameLastname (h1, h2);	
		}


		private static bool HaveSameAddress(SubscriptionHousehold h1, SubscriptionHousehold h2)
		{
			return h1.AddressLine1 == h2.AddressLine1
				&& h1.HouseNumber == h2.HouseNumber
				&& h1.Street == h2.Street
				&& h1.Postbox == h2.Postbox
				&& h1.ZipCode == h2.ZipCode
				&& h1.Town == h2.Town
				&& h1.CountryCode == h2.CountryCode;
		}

		private static bool HaveSameLastname(SubscriptionHousehold h1, SubscriptionHousehold h2)
		{
			var allNames = h1.Lastnames
				.Concat (h2.Lastnames)
				.Distinct ();

			return allNames.Count () == 1;
		}


		public int GetHashCode(SubscriptionHousehold household)
		{
			if (household == null)
			{
				return 0;
			}

			int result = 1000000007;

			var addressLine1 = household.AddressLine1;
			if (!string.IsNullOrEmpty (addressLine1))
			{
				result = 37 * result + addressLine1.GetHashCode ();
			}

			var houseNumber = household.HouseNumber;
			if (!string.IsNullOrEmpty (houseNumber))
			{
				result = 37 * result + houseNumber.GetHashCode ();
			}

			var street = household.Street;
			if (!string.IsNullOrEmpty (street))
			{
				result = 37 * result + street.GetHashCode ();
			}

			var postbox = household.Postbox;
			if (!string.IsNullOrEmpty (postbox))
			{
				result = 37 * result + postbox.GetHashCode ();
			}

			var zipCode = household.ZipCode;
			if (!string.IsNullOrEmpty (zipCode))
			{
				result = 37 * result + zipCode.GetHashCode ();
			}

			var town = household.Town;
			if (!string.IsNullOrEmpty (town))
			{
				result = 37 * result + town.GetHashCode ();
			}

			var countryCode = household.CountryCode;
			if (!string.IsNullOrEmpty (countryCode))
			{
				result = 37 * result + countryCode.GetHashCode ();
			}

			foreach (var lastname in household.Lastnames)
			{
				result = 37 * result + lastname.GetHashCode ();
			}

			return result;
		}


	}


}

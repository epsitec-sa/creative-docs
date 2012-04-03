using System.Collections.Generic;

using Epsitec.Aider.Entities;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal sealed class AiderAddressEntityComparer : IEqualityComparer<AiderAddressEntity>
	{
		
		
		// NOTE The house number complement of the addresses aretotally discarded by this comaparer
		// implementation.
		
		
		#region IEqualityComparer<eCH_AddressEntity> Members
		
		
		public bool Equals(AiderAddressEntity x, AiderAddressEntity y)
		{
			var bothAreNull = x == null && y == null;

			var bothAreEqual = x != null && y != null &&
				x.Street == y.Street &&
				x.HouseNumber == y.HouseNumber &&
				x.Town == y.Town;

			return bothAreNull || bothAreEqual;
		}

		
		public int GetHashCode(AiderAddressEntity address)
		{
			int result = 0;

			if (address != null)
			{
				result = 1000000007;

				if (address.Street != null)
				{
					result = 37 * result + address.Street.GetHashCode();
				}

				if (address.HouseNumber != null)
				{
					result = 37 * result + address.HouseNumber.GetHashCode();
				}

				result = 37 * result + address.Town.GetHashCode();
			}

			return result;
		}

		
		#endregion


		public static AiderAddressEntityComparer Instance
		{
			get
			{
				return AiderAddressEntityComparer.instance;
			}
		}


		private static readonly AiderAddressEntityComparer instance = new AiderAddressEntityComparer();


	}


}
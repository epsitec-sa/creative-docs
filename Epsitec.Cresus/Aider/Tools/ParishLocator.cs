//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Tools
{
	public static class ParishLocator
	{
		public static AiderGroupEntity FindParish(BusinessContext context, AiderAddressEntity address)
		{
			if (address.IsNull ())
			{
				return null;
			}

			var parishName = ParishLocator.FindParishName (ParishAddressRepository.Current, address);

			if (parishName == null)
			{
				return null;
			}

			var repository  = context.GetRepository<AiderGroupEntity> ();
			var example     = repository.CreateExample ();

			example.GroupDef = new AiderGroupDefEntity ()
			{
				DefType = Enumerations.GroupDefType.Parish
			};

			example.Name = parishName;

			var groups = repository.GetByExample (example).Where (x => x.EndDate == null).ToArray ();

			if (groups.Length > 0)
			{
				return groups[0];
			}
			else
			{
				return null;
			}
		}

		public static string FindParishName(ParishAddressRepository repository, AiderAddressEntity address)
		{
			var zipCode = address.Town.SwissZipCode.GetValueOrDefault ();
			var townName = address.Town.Name;
			var streetName = address.Street;
			var houseNumber = InvariantConverter.ToString (address.HouseNumber.GetValueOrDefault ());

			return ParishLocator.FindParishName (repository, zipCode, townName, streetName, houseNumber);
		}

		public static string FindParishName(ParishAddressRepository repository, eCH_AddressEntity address)
		{
			var zipCode = address.SwissZipCode;
			var townName = address.Town;
			var streetName = address.Street;
			var houseNumber = ParishLocator.StripHouseNumber (address.HouseNumber);
			
			return ParishLocator.FindParishName (repository, zipCode, townName, streetName, houseNumber);
		}

		/// <summary>
		/// Strips an house number from its terminal non digit part. This will remove any "bis",
		/// "ter", "A", "B", "C", etc at the end of the house number.
		/// </summary>
		private static string StripHouseNumber(string houseNumber)
		{
			string result = null;

			if (houseNumber != null)
			{
				int index = 0;

				while (index < houseNumber.Length && Char.IsDigit (houseNumber[index]))
				{
					index++;
				}

				if (index > 0)
				{
					result = houseNumber.Substring (0, index);
				}
			}

			return result;
		}

		private static string FindParishName(ParishAddressRepository repository, int zipCode, string townName, string streetName, string houseNumber)
		{
			var normalizedStreetName = SwissPostStreet.NormalizeStreetName (streetName);
			var normalizedHouseNumber = SwissPostStreet.NormalizeHouseNumber (houseNumber);

			return repository.FindParishName (zipCode, townName, normalizedStreetName, normalizedHouseNumber);
		}
	}
}

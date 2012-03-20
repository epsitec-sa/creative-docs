//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

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

		private static string FindParishName(ParishAddressRepository repository, int zipCode, string townName, string streetName, string houseNumber)
		{
			var normalizedStreetName = SwissPostStreet.NormalizeStreetName (streetName);
			var normalizedHouseNumber = SwissPostStreet.StripAndNormalizeHouseNumber (houseNumber);

			return repository.FindParishName (zipCode, townName, normalizedStreetName, normalizedHouseNumber);
		}
	}
}

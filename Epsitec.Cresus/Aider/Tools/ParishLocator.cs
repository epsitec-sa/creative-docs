//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

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

			var zipCode     = address.Town.SwissZipCode.GetValueOrDefault ();
			var townName    = address.Town.Name;
			var streetName  = SwissPostStreet.NormalizeStreetName (address.Street);
			var houseNumber = address.HouseNumber.GetValueOrDefault ();

			var parishName  = ParishAddressRepository.Current.FindParishName (zipCode, townName, streetName, houseNumber);

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
	}
}

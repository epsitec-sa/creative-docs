//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Repositories
{
	public class LocationRepository : Repository<LocationEntity>
	{
		public LocationRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}

		public IEnumerable<LocationEntity> GetByCountry(CountryEntity country)
		{
			var example = this.CreateExample ();
			example.Country = country;
			
			return this.GetByExample (example);
		}
	}
}

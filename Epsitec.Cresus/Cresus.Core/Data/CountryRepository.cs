//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{

	class CountryRepository : GenericRepository<CountryEntity>
	{

		public CountryRepository(DbInfrastructure dbInfrastructure, DataContext dataContext)
			: base (dbInfrastructure, dataContext)
		{
			
		}

		public CountryEntity GetCountryByCode(string code)
		{
			CountryEntity example = new Entities.CountryEntity ()
			{
				Code = code,
			};

			return this.GetEntityByExample (example);
		}

		public CountryEntity GetCountryByName(string name)
		{
			CountryEntity example = new Entities.CountryEntity ()
			{
				Name = name,
			};

			return this.GetEntityByExample (example);
		}

		public IEnumerable<CountryEntity> GetAllCountries()
		{
			CountryEntity example = new Entities.CountryEntity ();

			return this.GetEntitiesByExample (example);
		}

	}

}

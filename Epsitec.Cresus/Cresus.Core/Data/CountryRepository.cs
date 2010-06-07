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


	public class CountryRepository : Repository
	{


		public CountryRepository(DataContext dataContext) : base (dataContext)
		{	
		}


		public CountryEntity CreateCountryExample()
		{
			return this.CreateExample<CountryEntity> ();
		}


		public IEnumerable<CountryEntity> GetCountriesByExample(CountryEntity example)
		{
			return this.GetEntitiesByExample<CountryEntity> (example);
		}


		public CountryEntity GetCountryByExample(CountryEntity example)
		{
			return this.GetEntityByExample<CountryEntity> (example);
		}
		

		public IEnumerable<CountryEntity> GetAllCountries()
		{
			CountryEntity example = new Entities.CountryEntity ();

			return this.GetCountriesByExample (example);
		}

		
		public CountryEntity GetCountryByCode(string code)
		{
			CountryEntity example = new Entities.CountryEntity ()
			{
				Code = code,
			};

			return this.GetCountryByExample (example);
		}


		public CountryEntity GetCountryByName(string name)
		{
			CountryEntity example = new Entities.CountryEntity ()
			{
				Name = name,
			};

			return this.GetCountryByExample (example);
		}


	}


}

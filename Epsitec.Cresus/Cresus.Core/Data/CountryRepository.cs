//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class CountryRepository : Repository
	{


		public CountryRepository(DataContext dataContext) : base (dataContext)
		{	
		}


		public IEnumerable<CountryEntity> GetCountriesByExample(CountryEntity example)
		{
			return this.GetEntitiesByExample<CountryEntity> (example);
		}


		public IEnumerable<CountryEntity> GetCountriesByExample(CountryEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<CountryEntity> (example, index, count);
		}
		

		public IEnumerable<CountryEntity> GetAllCountries()
		{
			CountryEntity example = this.CreateCountryExample ();

			return this.GetCountriesByExample (example);
		}

		
		public IEnumerable<CountryEntity> GetCountriesByCode(string code)
		{
			CountryEntity example = this.CreateCountryExampleByCode (code);

			return this.GetCountriesByExample (example);
		}


		public IEnumerable<CountryEntity> GetCountriesByName(string name)
		{
			CountryEntity example = this.CreateCountryExampleByName (name);

			return this.GetCountriesByExample (example);
		}


		public CountryEntity CreateCountryExample()
		{
			return this.CreateExample<CountryEntity> ();
		}


		private CountryEntity CreateCountryExampleByCode(string code)
		{
			CountryEntity example = this.CreateCountryExample ();
			example.Code = code;

			return example;
		}


		private CountryEntity CreateCountryExampleByName(string name)
		{
			CountryEntity example = this.CreateCountryExample ();
			example.Name = name;

			return example;
		}


	}


}

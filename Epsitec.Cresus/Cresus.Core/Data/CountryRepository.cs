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


		public CountryEntity GetCountryByExample(CountryEntity example)
		{
			return this.GetEntityByExample<CountryEntity> (example);
		}
		

		public IEnumerable<CountryEntity> GetAllCountries()
		{
			CountryEntity example = this.CreateCountryExample ();

			return this.GetCountriesByExample (example);
		}

		
		public CountryEntity GetCountryByCode(string code)
		{
			CountryEntity example = this.CreateCountryExampleByCode (code);

			return this.GetCountryByExample (example);
		}


		public CountryEntity GetCountryByName(string name)
		{
			CountryEntity example = this.CreateCountryExampleByName (name);

			return this.GetCountryByExample (example);
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

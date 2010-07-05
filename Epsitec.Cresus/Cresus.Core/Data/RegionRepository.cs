using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class RegionRepository : Repository
	{


		public RegionRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<RegionEntity> GetRegionsByExample(RegionEntity example)
		{
			return this.GetEntitiesByExample<RegionEntity> (example);
		}


		public IEnumerable<RegionEntity> GetRegionsByExample(RegionEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<RegionEntity> (example, constrainer);
		}


		public IEnumerable<RegionEntity> GetRegionsByExample(RegionEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<RegionEntity> (example, index, count);
		}


		public IEnumerable<RegionEntity> GetRegionsByExample(RegionEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<RegionEntity> (example, constrainer, index, count);
		}


		public IEnumerable<RegionEntity> GetAllRegions()
		{
			RegionEntity example = this.CreateRegionExample ();

			return this.GetRegionsByExample (example);
		}


		public IEnumerable<RegionEntity> GetAllRegions(int index, int count)
		{
			RegionEntity example = this.CreateRegionExample ();

			return this.GetRegionsByExample (example, index, count);
		}


		public IEnumerable<RegionEntity> GetRegionsByName(string name)
		{
			RegionEntity example = this.CreateRegionExampleByName (name);

			return this.GetRegionsByExample (example);
		}


		public IEnumerable<RegionEntity> GetRegionsByName(string name, int index, int count)
		{
			RegionEntity example = this.CreateRegionExampleByName (name);

			return this.GetRegionsByExample (example, index, count);
		}


		public IEnumerable<RegionEntity> GetRegionsByCode(string code)
		{
			RegionEntity example = this.CreateRegionExampleByCode (code);

			return this.GetRegionsByExample (example);
		}


		public IEnumerable<RegionEntity> GetRegionsByCode(string code, int index, int count)
		{
			RegionEntity example = this.CreateRegionExampleByCode (code);

			return this.GetRegionsByExample (example, index, count);
		}


		public IEnumerable<RegionEntity> GetRegionsByCountry(CountryEntity country)
		{
			RegionEntity example = this.CreateRegionExampleByCountry (country);

			return this.GetRegionsByExample (example);
		}


		public IEnumerable<RegionEntity> GetRegionsByCountry(CountryEntity country, int index, int count)
		{
			RegionEntity example = this.CreateRegionExampleByCountry (country);

			return this.GetRegionsByExample (example, index, count);
		}


		public RegionEntity CreateRegionExample()
		{
			return this.CreateExample<RegionEntity> ();
		}


		private RegionEntity CreateRegionExampleByName(string name)
		{
			RegionEntity example = this.CreateRegionExample ();
			example.Name = name;

			return example;
		}


		private RegionEntity CreateRegionExampleByCode(string code)
		{
			RegionEntity example = this.CreateRegionExample ();
			example.Code = code;

			return example;
		}


		private RegionEntity CreateRegionExampleByCountry(CountryEntity country)
		{
			RegionEntity example = this.CreateRegionExample ();
			example.Country = country;

			return example;
		}


	}


}

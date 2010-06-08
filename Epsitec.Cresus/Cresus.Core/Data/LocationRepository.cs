using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{

	public class LocationRepository : Repository
	{


		public LocationRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<LocationEntity> GetLocationsByExample(LocationEntity example)
		{
			return this.GetEntitiesByExample<LocationEntity> (example);
		}


		public IEnumerable<LocationEntity> GetLocationsByExample(LocationEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<LocationEntity> (example, index, count);
		}


		public IEnumerable<LocationEntity> GetAllLocations()
		{
			LocationEntity example = this.CreateLocationExample ();

			return this.GetLocationsByExample (example);
		}


		public IEnumerable<LocationEntity> GetAllLocations(int index, int count)
		{
			LocationEntity example = this.CreateLocationExample ();

			return this.GetLocationsByExample (example, index, count);
		}


		public IEnumerable<LocationEntity> GetLocationsByName(string name)
		{
			LocationEntity example = this.CreateLocationExampleByName (name);

			return this.GetLocationsByExample (example);
		}


		public IEnumerable<LocationEntity> GetLocationsByName(string name, int index, int count)
		{
			LocationEntity example = this.CreateLocationExampleByName (name);

			return this.GetLocationsByExample (example, index, count);
		}


		public IEnumerable<LocationEntity> GetLocationsByPostalCode(string postalCode)
		{
			LocationEntity example = this.CreateLocationExampleByPostalCode (postalCode);

			return this.GetLocationsByExample (example);
		}


		public IEnumerable<LocationEntity> GetLocationsByPostalCode(string postalCode, int index, int count)
		{
			LocationEntity example = this.CreateLocationExampleByPostalCode (postalCode);

			return this.GetLocationsByExample (example, index, count);
		}


		public IEnumerable<LocationEntity> GetLocationsByCountry(CountryEntity country)
		{
			LocationEntity example = this.CreateLocationExampleByCountry (country);

			return this.GetLocationsByExample (example);
		}


		public IEnumerable<LocationEntity> GetLocationsByCountry(CountryEntity country, int index, int count)
		{
			LocationEntity example = this.CreateLocationExampleByCountry (country);

			return this.GetLocationsByExample (example, index, count);
		}


		public IEnumerable<LocationEntity> GetLocationsByRegion(RegionEntity region)
		{
			LocationEntity example = this.CreateLocationExampleByRegion (region);

			return this.GetLocationsByExample (example);
		}


		public IEnumerable<LocationEntity> GetLocationsByRegion(RegionEntity region, int index, int count)
		{
			LocationEntity example = this.CreateLocationExampleByRegion (region);

			return this.GetLocationsByExample (example, index, count);
		}


		public LocationEntity CreateLocationExample()
		{
			return this.CreateExample<LocationEntity> ();
		}


		private LocationEntity CreateLocationExampleByName(string name)
		{
			LocationEntity example = this.CreateLocationExample ();
			example.Name = name;

			return example;
		}


		private LocationEntity CreateLocationExampleByPostalCode(string postalCode)
		{
			LocationEntity example = this.CreateLocationExample ();
			example.PostalCode = postalCode;

			return example;
		}


		private LocationEntity CreateLocationExampleByCountry(CountryEntity country)
		{
			LocationEntity example = this.CreateLocationExample ();
			example.Country = country;

			return example;
		}


		private LocationEntity CreateLocationExampleByRegion(RegionEntity region)
		{
			LocationEntity example = this.CreateLocationExample ();
			example.Region = region;

			return example;
		}


	}


}

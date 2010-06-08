﻿using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class StreetRepository : Repository
	{

		public StreetRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<StreetEntity> GetStreetsByExample(StreetEntity example)
		{
			return this.GetEntitiesByExample<StreetEntity> (example);
		}


		public IEnumerable<StreetEntity> GetStreetsByExample(StreetEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<StreetEntity> (example, index, count);
		}


		public IEnumerable<StreetEntity> GetAllStreets()
		{
			StreetEntity example = this.CreateStreetExample ();

			return this.GetStreetsByExample (example);
		}


		public IEnumerable<StreetEntity> GetStreetsByName(string streetName)
		{
			StreetEntity example = this.CreateStreetExampleByName (streetName);

			return this.GetStreetsByExample (example);
		}


		public IEnumerable<StreetEntity> GetStreetsByComplement(string complement)
		{
			StreetEntity example = this.CreateStreetExampleByComplement (complement);

			return this.GetStreetsByExample (example);
		}


		public StreetEntity CreateStreetExample()
		{
			return this.CreateExample<StreetEntity> ();
		}


		private StreetEntity CreateStreetExampleByName(string streetName)
		{
			StreetEntity example = this.CreateStreetExample ();
			example.StreetName = streetName;

			return example;
		}


		private StreetEntity CreateStreetExampleByComplement(string complement)
		{
			StreetEntity example = this.CreateStreetExample ();
			example.Complement = complement;

			return example;
		}


	}


}
